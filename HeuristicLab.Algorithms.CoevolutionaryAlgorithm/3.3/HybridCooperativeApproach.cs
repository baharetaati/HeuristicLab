using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HEAL.Attic;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Random;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using HeuristicLab.Problems.CooperativeProblem;
using System.Globalization;
using HeuristicLab.Selection;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Markup;
using System.Drawing;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Regression;
using HeuristicLab.Problems.TestFunctions.MultiObjective;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Algorithms.CoevolutionaryAlgorithm {
 
  [Item("Hybrid Cooperative Algorithm", "A hybrid cooperative approach hybridizing a DecompositionBasedGA and NSGA2")]
  [Creatable(CreatableAttribute.Categories.PopulationBasedAlgorithms, Priority = 700)] // Ask how to initialize Priority field
  [StorableType("CCD6D33A-829D-4402-BE6C-A4B6EB6A72FB")]


  public class HybridCooperativeApproach : Orchestrator {
    [StorableConstructor]
    protected HybridCooperativeApproach(StorableConstructorFlag _) : base(_) {}
    protected HybridCooperativeApproach(HybridCooperativeApproach original, Cloner cloner)
      : base(original, cloner) {
      alg1QualityLength = original.alg1QualityLength;
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new HybridCooperativeApproach(this, cloner);
    }
    public HybridCooperativeApproach() : base(){
      Parameters.Add(new FixedValueParameter<BoolValue>("Run Two Algorithms Separately", "To define if two algorithms should be run independently", new BoolValue(true)));
      Parameters.Add(new FixedValueParameter<BoolValue>("ActiveOffspringSelector", "To define if DecompositionBasedGA should be single- or multi-objective", new BoolValue(true)));

    }
    protected override void Initialize(CancellationToken cancellationToken) {
      treeRequirements = new TreeRequirements(MaxTreeLength, MaxTreeDepth, MutationProbability);

      // Algorithm 1 Parameters
      alg1QualityLength = Problem.NumObjectives + 1;
      //alg1 = new DecompositionBasedGA(alg1QualityLength, treeRequirements, Alg1Selector);
      alg1 = new DecompositionBasedGA(alg1QualityLength, treeRequirements, Alg1Selector, OffspringParentsComparisonTypes.EpsilonLexicaseBasedComparison);
      numSelectedIndividualsGA = 2 * (Alg1PopulationSize - ElitesAlg1);
      gaInterval = 0;
      adjustWeightInterval = Alg1MaximumGenerations / 5;
      

      // Algorithm 2 Parameters
      numSelectedIndividualNSGA = 2 * Alg2PopulationSize;
      alg2 = new NSGA2(Problem.NumObjectives, treeRequirements);
      Alg1MaximumGenerations = Alg1MaximumEvaluatedSolutions / Alg1PopulationSize;
      Alg2MaximumGenerations = Alg2MaximumEvaluatedSolutions / Alg2PopulationSize;
      pauseNSGA2 = false;
      nsga2Interval = 0;

      if (SetSeedRandomly) {
        Seed = RandomSeedGenerator.GetSeed();
      }

      random.Reset(Seed);
      InitResults();
      base.Initialize(cancellationToken);
    }
    protected override void Run(CancellationToken cancellationToken) {
      while ((ResultsAlg1Evaluations < Alg1MaximumEvaluatedSolutions) && (ResultsActivePressure <= 100) /*|| (ResultsAlg2Evaluations < Alg2MaximumEvaluatedSolutions)*/) {
        try {
          Iterate();
          cancellationToken.ThrowIfCancellationRequested();
        } catch (Exception ex) {
          Console.WriteLine(ex.Message);
          Console.WriteLine(ex.StackTrace);
          throw;
        } finally {
          Analyze();
        }
      }
    }
    private void IterateSeparately() {
      //if (ResultsAlg2Evaluations < Alg2MaximumEvaluatedSolutions) {
      //  var nsgaCountEvaluations = 0;
      //  var countNSGAEvaluations = alg2.Apply(Alg2PopulationSize, numSelectedIndividualNSGA, Problem, ResultsAlg2Iterations, random);
      //  nsgaCountEvaluations += countNSGAEvaluations;
      //  ResultsAlg2Evaluations += nsgaCountEvaluations;
      //}

      if (ResultsAlg1Evaluations < Alg1MaximumEvaluatedSolutions) {
        //var gaCountEvaluations = 0;
        int countGAEvaluations;
        if (ResultsAlg1Iterations == 0) {
          countGAEvaluations = alg1.ApplyForInitialization(random, Alg1PopulationSize, Problem);
          ResultsAlg1Evaluations += countGAEvaluations;
        }
        else {
          if (ActiveOffspringSelector && (ResultsActivePressure <= 100)) {
            //var selectedParents = alg1.ApplySelectionAndClearation(random, numSelectedIndividualsGA, Problem.Maximization[0]);
            //ResultsActivePressure = 0.0;
            //ResultsCountSuccessfulOffspring = 0;
            //ResultsCountUnsuccessfulOffspring = 0;
            //while (alg1.Population.Count < Alg1PopulationSize) {
            //  countGAEvaluations = alg1.ApplyBatchWiseExtremeOffspringSelection(Alg1PopulationSize, selectedParents, Problem, random);
            //  ResultsActivePressure += alg1.ActiveSelectionPressure;
            //  ResultsCountSuccessfulOffspring += alg1.CountSuccessfulOffspring;
            //  ResultsCountUnsuccessfulOffspring += alg1.CountUnsuccessfulOffspring;
            //  ResultsAlg1Evaluations += countGAEvaluations;
            //  //gaCountEvaluations += countGAEvaluations;

            //}
            countGAEvaluations = alg1.ApplyExtremeOffspringSelection(Alg1PopulationSize, Problem, random);
            ResultsActivePressure = alg1.ActiveSelectionPressure;
            ResultsCountSuccessfulOffspring = alg1.CountSuccessfulOffspring;
            ResultsCountUnsuccessfulOffspring = alg1.CountUnsuccessfulOffspring;
            ResultsAlg1Evaluations += countGAEvaluations;
          } else {
            countGAEvaluations = alg1.Apply(Alg1PopulationSize, Problem, random);
            ResultsAlg1Evaluations += countGAEvaluations;
          }
        }
        
      }
    }
    
    private void Iterate() {
      if (RunSeparately) {
        IterateSeparately();
      } else {
        IterateOrchestrator();
      }

    }
    private void Analyze() {
      NSGA2Analyzer();
      
      GAAnalyzer();
      
      //DecompositionBasedGA resutls
      //if (!pauseNSGA2) {
      //  NSGA2Analyzer();
      //  ResultsAlg2Iterations++;
      //} else {
      //  GAAnalyzer();
      //  //CommunicationGAToNSGA2();
      //  ResultsAlg1Iterations++;
      //  if (ResultsAlg1Iterations % adjustWeightInterval == 0) {
      //    AdjustWeights();
      //  }
      //}
      //if (ResultsAlg1Evaluations == Alg1MaximumEvaluatedSolutions && ResultsAlg2Evaluations < Alg2MaximumEvaluatedSolutions) {
      //  if (pauseNSGA2) {
      //    pauseNSGA2 = false;
      //    nsga2Interval = 0;
      //    ResultsAlg1RunIntervalInGenerations.Add(new IntValue(gaInterval));
      //  }
      //} else if (ResultsAlg2Evaluations == Alg2MaximumEvaluatedSolutions && ResultsAlg1Evaluations < Alg1MaximumEvaluatedSolutions) {
      //  if (!pauseNSGA2) {
      //    pauseNSGA2 = true;
      //    gaInterval = 0;
      //    ResultsAlg2RunIntervalInGenerations.Add(new IntValue(nsga2Interval));
      //  }
      //}
      //if (ResultsAlg2Evaluations == Alg2MaximumEvaluatedSolutions) {
      //  ResultsAlg2RunIntervalInGenerations.Add(new IntValue(nsga2Interval));
      //}
      //ResultsAlg2HypervolumeWithoutGAContributionRow.Values.Add(ResultsHypervolumeAlg2);
      //ResultsAlg2HypervolumeWithGAContributionRow.Values.Add(ResultsHypervolumeAlg2WithGAContribution);
    }
    
    

    
    //private List<IndividualGA> GABestErrorSelection() {

    //  var qualities = new List<double[]>();
    //  var weightedSumValues = new List<double>();
    //  bool maximization = Problem.Maximization[0];
    //  foreach (var individual in alg1.Elites) {
    //    double[] indQualities = individual.Quality.Take(Problem.NumObjectives).ToArray();
    //    double indQualities2 = individual.Quality[Problem.NumObjectives];
    //    qualities.Add(indQualities);
    //    weightedSumValues.Add(indQualities2);
    //  }

    //  //List<int> indexes = Enumerable.Range(0, ga.Fitness.Count).ToList();
    //  List<int> indexes2 = Enumerable.Range(0, alg1.Fitness.Count).ToList();

    //  if (!maximization) {
    //    //indexes.Sort((i1, i2) => qualities[i1][0].CompareTo(qualities[i2][0]));
    //    indexes2.Sort((i1, i2) => weightedSumValues[i1].CompareTo(weightedSumValues[i2]));
    //  } else {
    //    //indexes.Sort((i1, i2) => -qualities[i1][0].CompareTo(qualities[i2][0]));
    //    indexes2.Sort((i1, i2) => -weightedSumValues[i1].CompareTo(weightedSumValues[i2]));
    //  }

    //  //int numSelectedBasedOnNMSE = Alg1PopulationSize / 4;
    //  int numSelectedBasedOnWS = Alg1PopulationSize / 4;

    //  //List<double[]> selectedFitness = new List<double[]>();
    //  List<IndividualGA> selectedSolutions = new List<IndividualGA>();

    //  //for (int i = 0; i < numSelectedBasedOnNMSE; i++) {
    //  //  selectedFitness.Add(qualities[indexes[i]].ToArray());
    //  //}

    //  for (int i = 0; i < numSelectedBasedOnWS; i++) {
    //    //selectedFitness.Add(qualities[indexes2[i]].ToArray());
    //    selectedSolutions.Add((IndividualGA)alg1.Elites[indexes2[i]].Clone());
    //  }
    //  //ResultsGAContribution[ResultsIterations, 0] = numSelectedForCommunication;
    //  return selectedSolutions;
    //}
    //private List<double[]> GANonDominatedSortingSelection() {
    //  int[] rank;
    //  var solutions = new List<ISymbolicExpressionTree>();
    //  var qualities = new List<double[]>();
    //  foreach (var individual in alg1.Elites) {
    //    solutions.Add((ISymbolicExpressionTree)individual.Solution.Clone());
    //    double[] indQualities = individual.Quality.Take(Problem.NumObjectives).ToArray();
    //    qualities.Add(indQualities);
    //  }
    //  RankAndCrowdingSort sorter = new RankAndCrowdingSort();
    //  var fronts = sorter.FastNonDominatedSorting(solutions, qualities, Problem.Maximization, out rank);

    //  List<ISymbolicExpressionTree> paretoFrontSolutions = fronts[0].Select(tuple => tuple.Item1).ToList();
    //  List<double[]> gaParetoFrontQualities = fronts[0].Select(tuple => tuple.Item2).ToList();

    //  int gaNonDominatedSolutionsCount = 0;
    //  for (int i = 0; i < rank.Length; i++) {
    //    if (rank[i] == 0) {
    //      gaNonDominatedSolutionsCount++;
    //      alg1.Elites[i].BeingCommunicated = true;

    //    }
    //  }
    //  return gaParetoFrontQualities;
    //}
    
   
    

  }
}
