using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using HEAL.Attic;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Problems.CooperativeProblem;

namespace HeuristicLab.Algorithms.CoevolutionaryAlgorithm {
  [StorableType("74439610-BEAF-4BEE-97BD-E7F16B2F95C9")]
  public class NSGA2 : DeepCloneable {
    #region Properties
    [Storable]
    CrowdedTournamentSelection selector;
    [Storable]
    RankAndCrowdingSort sorter;
    [Storable]
    private readonly TreeRequirements _treeRequirements;
    [Storable]
    public List<IndividualNSGA2> Population { get; set; } 
    [Storable]
    public List<double[]> Fitness { get; set; }
    [Storable]
    public int QualityLength { get; private set; }
    [Storable]
    public List<List<IndividualNSGA2>> CurrentFronts { get; set; }
    [Storable]
    public List<int> Ranks { get; set; }
    [Storable]
    public List<double> CrowdingDistances { get; set; }
    #endregion

    #region Constructors
    [StorableConstructor]
    protected NSGA2(StorableConstructorFlag _) { }
    public NSGA2(int qualityLength, TreeRequirements treeRequirements) {
      QualityLength = qualityLength;
      Population = new List<IndividualNSGA2>();
      Fitness = new List<double[]>();
      selector = new CrowdedTournamentSelection();
      _treeRequirements = treeRequirements;
      sorter = new RankAndCrowdingSort();
      CurrentFronts = new List<List<IndividualNSGA2>>();
      Ranks = new List<int>();
      CrowdingDistances = new List<double>();
    }
    protected NSGA2(NSGA2 original, Cloner cloner) : base(original, cloner) {
      QualityLength = original.QualityLength;
      Population = original.Population.Select(cloner.Clone).ToList();
      Fitness = original.Fitness.Select(arr => arr.ToArray()).ToList();
      selector = cloner.Clone(original.selector);
      _treeRequirements = cloner.Clone(original._treeRequirements);
      sorter = cloner.Clone(original.sorter);
      CurrentFronts = original.CurrentFronts != null ? original.CurrentFronts.Select(front => front.Select(individual => cloner.Clone(individual)).ToList()).ToList() : null;
      Ranks = original.Ranks.ToList();
      CrowdingDistances = original.CrowdingDistances.ToList();
    }
    #endregion
    private void Initialization(int popSize, CooperativeProblem problem, IRandom random) {
      if (_treeRequirements != null) {
        for (int i = 0; i < popSize; i++) {
          var individual = new IndividualNSGA2(_treeRequirements, QualityLength, problem, random);
          Population.Add(individual);
        }
      }
      //StoredPop.AddRange(Population);
      //Console.WriteLine("Initialization");
      //for(int i=0; i < popSize; i++) {
      //  Console.WriteLine($"individual[{i}]");
      //  foreach (var node in Population[i].Solution.IterateNodesBreadth()) {
      //    Console.Write($"{node}  ");
      //  }
      //  Console.WriteLine();
      //}

    }
    private void ConvertFromGA(List<IndividualGA> gaCommunicatedIndividuals) {
      foreach (var ind in gaCommunicatedIndividuals) {
        var sol = (ISymbolicExpressionTree)ind.Solution.Clone();
        var quality = ind.Quality.Take(QualityLength).ToArray();
        Population.Add(new IndividualNSGA2(_treeRequirements, sol, quality));
        Fitness.Add(quality);
      }
    }
    public void AppendToNSGA2Population(List<IndividualGA> gaCommunicatedIndividuals, CooperativeProblem problem, int populationSize) {
      ConvertFromGA(gaCommunicatedIndividuals);
      RankAndCrowdingSorter(problem);
      UpdatePopulation(populationSize);
    }
    private List<double[]> Evaluation(List<IndividualNSGA2> pop, CooperativeProblem problem, IRandom random) {
      List<double[]> fit = new List<double[]>();
      foreach (var individual in pop) {
        var qualities = individual.Evaluate(random, problem);
        fit.Add(qualities);
      }
      
      return fit;
    }
    public void RankAndCrowdingSorter(CooperativeProblem problem) {
      int[] rank;
      var solutions = new List<ISymbolicExpressionTree>();
      
      //List<double[]> qualitiesWithoutWeightedSumValue = Fitness.Select(arr => arr.Take(2).ToArray()).ToList();
      foreach (var individual in Population) {
        solutions.Add((ISymbolicExpressionTree)individual.Solution.Clone());
      }
      // Generating fronts and ranks
      var fronts = sorter.FastNonDominatedSorting(solutions, Fitness, problem.Maximization, out rank);
      // =================== Scenario one ===================
      // Clear CurrentParetoFront, Population, Fitness, Ranks, and CrowdingDistances
      CurrentFronts.Clear();
      for (int i = 0; i < fronts.Count; i++) {
        if (fronts[i].Count > 0) {
          List<IndividualNSGA2> front = new List<IndividualNSGA2>();
          List<ISymbolicExpressionTree> frontSolutions = fronts[i].Select(tuple => tuple.Item1).ToList();
          List<double[]> frontQualities = fronts[i].Select(tuple => tuple.Item2).ToList();
          var distances = sorter.CalculateCrowdingDistance(frontQualities, problem.Maximization);
          var sortedIndexes = sorter.RankBasedCrowdedComparisonSorter(distances);
          foreach (var index in sortedIndexes) {
            var ind = new IndividualNSGA2(_treeRequirements, (ISymbolicExpressionTree)frontSolutions[index].Clone(), frontQualities[index], i, distances[index]);
            front.Add(ind);
          }
          CurrentFronts.Add(front);
        }
      }
      // =================== Scenario two ===================
      //var maxRank = rank.Max();
      //for (int i = 0; i < maxRank + 1; i++) {
      //  List<double[]> frontQualities = new List<double[]>();
      //  List<IndividualNSGA2> front = new List<IndividualNSGA2>();
      //  List<IndividualNSGA2> currentFront = new List<IndividualNSGA2>();
      //  for (int k = 0; k < rank.Length; k++) {
      //    if (rank[k] == i) {
      //      Population[k].SetRank(i);
      //      front.Add(Population[k]);
      //      frontQualities.Add(Population[k].Quality);
      //    }
      //  }
      //  var distances = sorter.CalculateCrowdingDistance(frontQualities);
      //  for (int j = 0; j < front.Count; j++) {
      //    front[j].SetCrowdingDistance(distances[j]);
      //  }
      //  var sortedIndexes = sorter.RankBasedCrowdedComparisonSorter(distances);
        
      //  foreach (var index in sortedIndexes) {
      //    currentFront.Add(front[index]);
      //  }
      //  CurrentFronts.Add(currentFront);
      //}
      
    }
    public void UpdatePopulation(int populationSize) {
      Population.Clear();
      Fitness.Clear();
      Ranks.Clear();
      CrowdingDistances.Clear();
      bool reachesPopulationSize = false;
      foreach (var front in CurrentFronts) {
        if (Population.Count + front.Count <= populationSize) {
          // If adding all individuals from the current front doesn't exceed the size constraint, add them all.
          Population.AddRange(front);
        } else {
          // If adding all individuals from the current front would exceed the size constraint, add them one by one.
          for (int i = 0; i < front.Count; i++) {
            if (Population.Count < populationSize) {
              Population.Add(front[i]);
            } else {
              reachesPopulationSize = true;
              break;
            }
          }

          // If we've reached the population size constraint, break out of the loop.
          if (reachesPopulationSize) {
            break;
          }
        }
      }
      for (int i = 0; i < Population.Count; i++) {
        Fitness.Add(Population[i].Quality);
        Ranks.Add(Population[i].Rank);
        CrowdingDistances.Add(Population[i].CrowdingDistance);
      }
     
    }
    private List<IndividualNSGA2> Selection(IRandom random, int numSelectedParents) {
      List<IndividualNSGA2> selectedParents = new List<IndividualNSGA2>();
      var selectedIndexes = selector.Select(random, numSelectedParents, Ranks, CrowdingDistances);
      foreach (var index in selectedIndexes) {
        selectedParents.Add((IndividualNSGA2)Population[index].Clone());
      }
      return selectedParents;
    }
    private ISymbolicExpressionTree Crossover(ISymbolicExpressionTree parent1, ISymbolicExpressionTree parent2, IRandom random) {
      var crossOver = new SubtreeCrossover();
      var childTree = SubtreeCrossover.Cross(random, parent1, parent2, crossOver.CrossoverProbability, crossOver.InternalCrossoverPointProbability.Value, _treeRequirements.MaxTreeLength, _treeRequirements.MaxTreeDepth);
      return childTree;
    }
    public int Apply(int populationSize, int numSelectedIndividuals, CooperativeProblem problem, int iterationNum, IRandom random) {
      var countEvaluations = 0;
      if (iterationNum == 0) {
        Initialization(populationSize, problem, random);
        Fitness.Clear();
        var fit = Evaluation(Population, problem, random);
        countEvaluations += fit.Count;
        Fitness.AddRange(fit);
        RankAndCrowdingSorter(problem);
        UpdatePopulation(populationSize);
      }
      var selectedParents = Selection(random, numSelectedIndividuals);
      List<IndividualNSGA2> offspring = new List<IndividualNSGA2>();
      for (int i = 0; i < selectedParents.Count; i += 2) {
        var childTree = Crossover(selectedParents[i].Solution, selectedParents[i + 1].Solution, random);
        var child = new IndividualNSGA2(_treeRequirements, childTree, QualityLength);
        int randomIndex = random.Next(_treeRequirements.Manipulators.Count); // Generate a random index

        if (random.NextDouble() < _treeRequirements.MutationProbability) {
          ISymbolicExpressionTreeManipulator chosenManipulator = _treeRequirements.Manipulators[randomIndex]; // Select the manipulator at the random index
          child.Mutate(chosenManipulator, random);
        }
        offspring.Add(child);
      }
      var offspringFit = Evaluation(offspring, problem, random);
      countEvaluations += offspringFit.Count;
      Population.AddRange(offspring);
      Fitness.AddRange(offspringFit);
      RankAndCrowdingSorter(problem);
      UpdatePopulation(populationSize);
      
      return countEvaluations;
    }
    #region Cloning
    public override IDeepCloneable Clone(Cloner cloner) {
      return new NSGA2(this, cloner);

    }
    #endregion
  }
}
