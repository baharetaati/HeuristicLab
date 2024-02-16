using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using HEAL.Attic;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Problems.CooperativeProblem;
using HeuristicLab.Problems.Instances;
using HeuristicLab.Selection;

namespace HeuristicLab.Algorithms.CoevolutionaryAlgorithm {
  [StorableType("8D1EDC9E-1EE5-4F20-8718-C14286511C92")]
  public class DecompositionBasedGA : BaseAlg {
    #region Properties
    public ISelectionStrategy<double> Selector { get; }
    private int segmentNo { get; }
    [Storable]
    public List<List<IndividualGA>> Population { get; set; } = new List<List<IndividualGA>>();
    [Storable]
    public List<double[]> Fitness { get; set; }
    [Storable]
    public int QualityLength { get; private set; }
    [Storable]
    public List<double[]> Weights { get; set; }
    [Storable]
    public List<IndividualGA> Elites { get; set; }
    [Storable]
    public double BestQuality { get; set; }
    [Storable]
    public double WorstQuality { get; set; }
    [Storable]
    public IndividualGA Elite { get; set; }
    [Storable]
    public double AverageQuality { get; set; }
    [Storable]
    public OffspringParentsComparisonTypes OffspringComparisonType { get; set; }
    [Storable]
    public double ActiveSelectionPressure { get; set; }
    [Storable]
    public int CountSuccessfulOffspring { get; set; }
    [Storable]
    public int CountUnsuccessfulOffspring { get; set; }
    #endregion

    #region Constructors
    [StorableConstructor]
    protected DecompositionBasedGA(StorableConstructorFlag _) : base(_){ }
    public DecompositionBasedGA(int qualityLength, TreeRequirements treeRequirements, ISelectionStrategy<double> selector, OffspringParentsComparisonTypes offspringParentsComparisonTypes, int _segmentNo, int eliteSize = 1) :base(treeRequirements){

      QualityLength = qualityLength;
      Population = new List<List<IndividualGA>>();
      Elites = new List<IndividualGA>();
      Fitness = new List<double[]>();
      Selector = selector;
      Weights = new List<double[]>(); 
      BestQuality = -1.0;
      WorstQuality= -1.0;
      AverageQuality = 0.0;
      Elite = null;
      OffspringComparisonType = offspringParentsComparisonTypes;
      ActiveSelectionPressure = 0.0;
      CountSuccessfulOffspring = 0;
      CountUnsuccessfulOffspring = 0;
      segmentNo = _segmentNo;
    }
    protected DecompositionBasedGA(DecompositionBasedGA original, Cloner cloner):base(original,cloner) {
      QualityLength = original.QualityLength;
      Population = original.Population != null ? original.Population.Select(subPop => subPop.Select(individual => cloner.Clone(individual)).ToList()).ToList() : null;
      Elites = original.Elites.Select(cloner.Clone).ToList();
      Fitness = original.Fitness.Select(arr => arr.ToArray()).ToList();
      Selector = cloner.Clone(original.Selector);
      Weights = original.Weights.Select(arr => arr.ToArray()).ToList();
      BestQuality = original.BestQuality;
      WorstQuality = original.WorstQuality;
      AverageQuality = original.AverageQuality;
      Elite = cloner.Clone(original.Elite);
      OffspringComparisonType = original.OffspringComparisonType;
      ActiveSelectionPressure = original.ActiveSelectionPressure;
      CountSuccessfulOffspring = original.CountSuccessfulOffspring;
      CountUnsuccessfulOffspring = original.CountUnsuccessfulOffspring;
      segmentNo = original.segmentNo;
    }
    #endregion
    public void Initialization(int popSize, CooperativeProblem problem, IRandom random) {
      var referenceVectors = DasAndDennisTechnique.GetWeightVectors(segmentNo, problem.NumObjectives);
      Weights = referenceVectors.Select(vector => vector.ToArray()).ToList();
      foreach (var w in Weights) {
        Console.WriteLine($"w[0]={w[0]} w[1]={w[1]}");
      }
      var initialWeightsCount = Weights.Count;
      int countPerWeight = popSize / initialWeightsCount;
      for (int i = 0; i < initialWeightsCount; i++) {
        if (Population.Count <= i) {
          Population.Add(new List<IndividualGA>());
        }
        for (int j = 0; j < countPerWeight; j++) {
          var individual = new IndividualGA(_treeRequirements, QualityLength, Weights[i], problem, random);
          Population[i].Add(individual);
        }
      }
      int assignedCount = countPerWeight * initialWeightsCount;
      if (assignedCount != popSize) {
        var dif = popSize - assignedCount;
        List<int> shuffledIndices = Enumerable.Range(0, initialWeightsCount).OrderBy(x => random.Next()).ToList();
        List<int> selectedIndices = shuffledIndices.Take(dif).ToList();
        for (int p = 0; p < dif; p++) {
          var selectedInd = selectedIndices[p];
          var individual = new IndividualGA(_treeRequirements, QualityLength, Weights[selectedInd], problem, random);
          Population[selectedInd].Add(individual);
        }
      }
    }
    private List<double[]> Evaluation(List<IndividualGA> pop, CooperativeProblem problem, IRandom random) {
      List<double[]> fit = new List<double[]>();
     
      foreach (var individual in pop) {
        var qualities = individual.Evaluate(random, problem);
        fit.Add(qualities);
      }
     
      return fit;
    }
    public void SetWeight(double[] weight, int index) {
      if (index >= 0 && index < Weights.Count){
        Weights[index] = weight;
      }
    }
    private void SetElite(bool maximization) {
      double bestQlty = double.MaxValue;
      int bestIndex = -1;
      if (maximization) {
        bestQlty = double.MinValue;
      }

      for (int i=0; i<Elites.Count; i++) {
        if ((!maximization && Elites[i].Quality[QualityLength-1] < bestQlty) ||
          (maximization && Elites[i].Quality[QualityLength - 1] > bestQlty)) {
          bestIndex = i;
          bestQlty = Elites[i].Quality[QualityLength-1];
        }
      }
      if (bestIndex == -1) {
        throw new Exception($"SetElite() index {bestIndex} is not valid");
      }
      Elite = (IndividualGA)Elites[bestIndex].Clone();
    }
    private int FindElitesIndex(List<double[]> fit, bool maximization) {
      List<double[]> tempFit = new List<double[]>();
      tempFit.AddRange(fit);
      
      bool sortDescending = false;
     
      if (maximization) 
        sortDescending = true;
      List<int> indexes = Enumerable.Range(0, tempFit.Count).ToList();
      
      if (sortDescending) {
        indexes.Sort((i1, i2) => -tempFit[i1][QualityLength - 1].CompareTo(tempFit[i2][QualityLength - 1]));
      } else {
        indexes.Sort((i1, i2) => tempFit[i1][QualityLength - 1].CompareTo(tempFit[i2][QualityLength - 1]));
      }
    
      int bestIndex = indexes[0];
      
      return bestIndex;
    }
    private List<IndividualGA> Selection(List<double[]> fit, List<IndividualGA> subPop, int numSelectedIndividuals, IRandom random, bool maximization) {
      List<double> fitnessValues;
     
      fitnessValues = fit.Select(fitness => fitness.Last()).ToList();
           
      List<int> selectedIndexes = Selector.Select(numSelectedIndividuals, random, fitnessValues, maximization);
      List<IndividualGA> selectedIndividuals = new List<IndividualGA>(numSelectedIndividuals);
      foreach (var index in selectedIndexes) {
        selectedIndividuals.Add((IndividualGA)subPop[index].Clone());
      }
      return selectedIndividuals;
    }
    private IndividualGA EpsilonLexicaseSelection(List<double[]> fit, List<IndividualGA> subPop, int numSelectedParents, IRandom random, CooperativeProblem problem) {
      List<IndividualGA> pool = subPop;
      List<double[]> currentFit = fit;
      IndividualGA selectedIndividual = null;
      int m = problem.NumObjectives;
      bool maximization;
      List<int> shuffledObjectiveIndices = new List<int>();
      for (int i = 0; i < m+1; i++) {
        shuffledObjectiveIndices.Add(i);
      }
      shuffledObjectiveIndices = shuffledObjectiveIndices.OrderBy(i => Guid.NewGuid()).ToList();
      for (int i = 0; i < m+1; i++) {
        List<double> currentCase = new List<double>(); 
        if (shuffledObjectiveIndices[i] == 1) {
          foreach (var individual in pool) {
            currentCase.Add(individual.NormalizedTreeLength);
          }
        } else {
          currentCase = currentFit.Select(array => array[shuffledObjectiveIndices[i]]).ToList();
        }
        
        if (shuffledObjectiveIndices[i] == m) {
          maximization = problem.Maximization[0];
        } else {
          maximization = problem.Maximization[shuffledObjectiveIndices[i]];
        }
        
        List<int> selectedIndices = Selector.Select(1, random, currentCase, maximization);
        pool = pool.Where((individual, index) => selectedIndices.Contains(index)).ToList();
        currentFit = currentFit.Where((fitArray, index) => selectedIndices.Contains(index)).ToList();
        if (pool.Count == 1) {
          break;
        }
      }
      selectedIndividual = (IndividualGA)pool[0].Clone();
      //if (pool.Count > 1) {
      //  int randIndex = random.Next(pool.Count);
      //  selectedIndividual = (IndividualGA)pool[randIndex].Clone();
      //} else {
      //  selectedIndividual = (IndividualGA)pool[0].Clone();
      //}
      
      if (selectedIndividual == null) {
        throw new ArgumentNullException("The selected individual should not be null");
      }
      return selectedIndividual;
    }
    private void FindBestWorstAverageQuality(bool maximization) {
      double bestQlty = double.MaxValue;
      double worstQlty = double.MinValue;
      int errIndex = 0;

      if (maximization) {
        bestQlty = double.MinValue;
        worstQlty = double.MaxValue;
      }
      foreach (var subPop in Population) {
        foreach (var individual in subPop) {
          if ((maximization && individual.Quality[errIndex] > bestQlty) ||
               (!maximization && individual.Quality[errIndex] < bestQlty)) {
            bestQlty = individual.Quality[errIndex];
          }
        }
      }
      foreach (var subPop in Population) {
        foreach (var individual in subPop) {
          if ((maximization && individual.Quality[errIndex] < worstQlty) ||
               (!maximization && individual.Quality[errIndex] > worstQlty)) {
            worstQlty = individual.Quality[errIndex];
          }
        }
      }
      BestQuality = bestQlty;
      WorstQuality = worstQlty;
     
      var errValues = CreateFitList(errIndex);
      AverageQuality = CalculateAverage(errValues);
    }
    public int Apply(int populationSize, CooperativeProblem problem, IRandom random) {
      var countEvaluations = 0;
      bool maximization = problem.Maximization[0];
      
      var subPopCount = Population.Count;
      
      List < IndividualGA > tmpElites = new List <IndividualGA>();
      var tmpFit = new List<double[]>();
      var countsIndividuals = Population.Select(subpopulation => subpopulation.Count).ToList();
      int subPopIndex = 0;
      int countAllBadModels = 0;
      for (int i = 0; i < subPopCount; i++) {
        var fit = new List<double[]>();
        for (int k = subPopIndex; k < countsIndividuals[i] + subPopIndex; k++) {
          fit.Add(Fitness[k]);
        }
        int numSelectedIndividuals = 2 * (countsIndividuals[i] - 1);
        var selectedParents = Selection(fit, Population[i], numSelectedIndividuals, random, maximization);
        
        List<IndividualGA> offspring = new List<IndividualGA>();
        int countBadNMSEParents = 0;
        int countBadNMSEOffspring = 0;
        for (int j = 0; j < selectedParents.Count; j += 2) {
          if (selectedParents[j].Quality[0] == 1.0) {
            countBadNMSEParents++;
          }
          if (selectedParents[j+1].Quality[0] == 1.0) {
            countBadNMSEParents++;
          }
          //Console.WriteLine($"{selectedParents[j].Quality[0]}, {selectedParents[j].Quality[1]}, {selectedParents[j].Quality[2]}");
          //Console.WriteLine($"{selectedParents[j+1].Quality[0]}, {selectedParents[j+1].Quality[1]}, {selectedParents[j+1].Quality[2]}");
          var childTree = Crossover((ISymbolicExpressionTree)selectedParents[j].Solution.Clone(), (ISymbolicExpressionTree)selectedParents[j + 1].Solution.Clone(), random);
          var child = new IndividualGA(_treeRequirements, childTree, QualityLength, Weights[i].ToArray());
          int randomIndex = random.Next(_treeRequirements.Manipulators.Count); // Generate a random index

          if (random.NextDouble() < _treeRequirements.MutationProbability) {
            ISymbolicExpressionTreeManipulator chosenManipulator = _treeRequirements.Manipulators[randomIndex]; // Select the manipulator at the random index
            child.Mutate(chosenManipulator, random);
          }
          offspring.Add(child);
        }
        Population[i].Clear();
        Population[i].Add((IndividualGA)Elites[i].Clone());
        tmpFit.Add(Elites[i].Quality.ToArray());
        if (Elites[i].Quality[0] == 1.0) {
          countAllBadModels++;
        }
        var offspringFit = Evaluation(offspring, problem, random);
        countEvaluations += offspringFit.Count;
        Population[i].AddRange(offspring);
        for (int c = 0; c < offspringFit.Count; c++) {
          if (offspringFit[c][0] == 1.0) {
            countBadNMSEOffspring++;
          }
        }
        var bestIndex = FindElitesIndex(offspringFit, maximization);
        tmpElites.Add((IndividualGA)Population[i][bestIndex].Clone());
        tmpFit.AddRange(offspringFit);
        subPopIndex += countsIndividuals[i];
        
        Console.WriteLine($"Population[{i}] badSelectedParents = {countBadNMSEParents} and badOffspring = {countBadNMSEOffspring} out of {countsIndividuals[i]}");
        Console.WriteLine($"Weights[{i}] {Weights[i][0]} {Weights[i][1]}");
        countAllBadModels += countBadNMSEOffspring;
      }
      Console.WriteLine($"count bad models = {countAllBadModels} out of {populationSize}");
      Elites.Clear();
      Elites.AddRange(tmpElites);
      SetElite(maximization);
      Fitness.Clear();
      Fitness.AddRange(tmpFit);
      int countBadElites = 0;
      foreach (var indElite in Elites) {
        if (indElite.Quality[0] == 1.0) {
          countBadElites++;
        }
      }
      Console.WriteLine($"count bad elites = {countBadElites} out of {Elites.Count}");
      FindBestWorstAverageQuality(maximization);
      
      return countEvaluations;
    }
    public int ApplyOffspringSelection(int populationSize, CooperativeProblem problem, int iterationNum, double maxSelPressure, double successRate, IRandom random) {
      bool useEpsilonLexicaseSelection = false;
      if ((Selector is EpsilonLexicaseSelection) || (Selector is AdaptiveEpsilonLexicaseSelection)) {
        useEpsilonLexicaseSelection = true;
      }
      var countEvaluations = 0;
      bool maximization = problem.Maximization[0];
      if (iterationNum == 0) {
        Initialization(populationSize, problem, random);
        Fitness.Clear();
        Elites.Clear();
        int numSubPopulations = Population.Count;

        for (int i = 0; i < numSubPopulations; i++) {
          List<IndividualGA> subpopulation = Population[i];
          var fit = Evaluation(subpopulation, problem, random);
          countEvaluations += fit.Count;
          Fitness.AddRange(fit);
          var bestIndex = FindElitesIndex(fit, maximization);
          Elites.Add((IndividualGA)Population[i][bestIndex].Clone());
        }
      }
      var subPopCount = Population.Count;
      var countsIndividuals = Population.Select(subpopulation => subpopulation.Count).ToList();
      int subPopIndex = 0;
      List<IndividualGA> newElites = new List<IndividualGA>();
      List<double[]> newFit = new List<double[]>();
      int countAllBadModels = 0;
      double adaptiveEps = 0.0;
      if (OffspringComparisonType == OffspringParentsComparisonTypes.EpsilonLexicaseBasedComparison) {
        double[] fitness = new double[populationSize];
        for (int f = 0; f < populationSize; f++) {
          fitness[f] = Fitness[f][0];
        }
        adaptiveEps = AdaptiveEpsilonLexicaseSelection.CalculateMAD(fitness);
      }
      for (int i = 0; i < subPopCount; i++) {
        var fit = new List<double[]>();
        for (int k = subPopIndex; k < countsIndividuals[i] + subPopIndex; k++) {
          fit.Add(Fitness[k].ToArray());
        }
        int countBadSelectedParents = 0;
        int countBadOffspring = 0;
        int numSelectedIndividuals = 2 * (countsIndividuals[i] - 1);
        var selectedParents = new List<IndividualGA>();
        if (useEpsilonLexicaseSelection) {
          for (int selected = 0; selected < numSelectedIndividuals; selected++) {
            selectedParents.Add(EpsilonLexicaseSelection(fit, Population[i], numSelectedIndividuals, random, problem));
          }
        } else {
          selectedParents = Selection(fit, Population[i], numSelectedIndividuals, random, maximization);
        }
        
        foreach (var ind in selectedParents) {
          if (ind.Quality[0] == 1.0) {
            countBadSelectedParents++;
          }
        }
        var newPop = new List<IndividualGA>();
        var pool = new List<IndividualGA>();
        newPop.Add((IndividualGA)Elites[i].Clone());
        bool exitLoop = false;

        while (newPop.Count < successRate * countsIndividuals[i] && !exitLoop) {
          var parentsCount = selectedParents.Count;
          for (int k = 0; k < parentsCount; k += 2) {
            var childTree = Crossover((ISymbolicExpressionTree)selectedParents[k].Solution.Clone(), (ISymbolicExpressionTree)selectedParents[k + 1].Solution.Clone(), random);
            var child = new IndividualGA(_treeRequirements, childTree, QualityLength, Weights[i].ToArray());
            int randomIndex = random.Next(_treeRequirements.Manipulators.Count); // Generate a random index

            if (random.NextDouble() < _treeRequirements.MutationProbability) {
              ISymbolicExpressionTreeManipulator chosenManipulator = _treeRequirements.Manipulators[randomIndex]; // Select the manipulator at the random index
              child.Mutate(chosenManipulator, random);
            }

            var qualities = child.Evaluate(random, problem);
            countEvaluations++;
            bool addToNewPopulation = ComparisonForOffspringSelection(random, (IndividualGA) selectedParents[k].Clone(), (IndividualGA) selectedParents[k + 1].Clone(), (IndividualGA) child.Clone(), problem.Maximization, adaptiveEps);
            if (addToNewPopulation) {
              newPop.Add(child);
            } else {
              //double randSel = random.NextDouble();
              pool.Add((IndividualGA)child.Clone());
              //if (randSel < 0.5) {
              //  pool.Add((IndividualGA)child.Clone());
              //} else {
              //  pool.Add((IndividualGA)selectedParents[k].Clone());
              //}
              
            }
            var pActive = (double)(newPop.Count + pool.Count) / countsIndividuals[i];
            if (maxSelPressure < pActive) {
              exitLoop = true;
              break;
            }
          }
        }
        
        HashSet<IndividualGA> poolSet = new HashSet<IndividualGA>(pool);
        HashSet<IndividualGA> populationSet = new HashSet<IndividualGA>(Population[i]);
        
        while (newPop.Count < countsIndividuals[i]) {
          if (poolSet.Count > 0) {
            int randomIndex = random.Next(poolSet.Count);
            IndividualGA selectedElement = poolSet.ElementAt(randomIndex);
            newPop.Add(selectedElement);
            poolSet.Remove(selectedElement);
          } else {
            int randomIndex = random.Next(populationSet.Count);
            IndividualGA selectedElement = populationSet.ElementAt(randomIndex);
            newPop.Add(selectedElement);
            populationSet.Remove(selectedElement);
          } 
        }
        Population[i].Clear();
        if (newPop.Count > countsIndividuals[i] || newPop.Count < countsIndividuals[i])
          throw new Exception($"New Subpopulation[{i}] = {newPop.Count}, Subpopulation[{i}] Size = {countsIndividuals[i]}");
        fit.Clear();
        foreach (var ind in newPop) {
          var quality = ind.Quality.ToArray();
          newFit.Add(quality);
          fit.Add(quality);
        }
        var bestIndex = FindElitesIndex(fit, maximization);
        newElites.Add((IndividualGA)newPop[bestIndex].Clone());
        Population[i].AddRange(newPop);
        subPopIndex += countsIndividuals[i];
        foreach (var qlty in fit) {
          if (qlty[0] == 1.0) {
            countBadOffspring++;
          }
        }
        Console.WriteLine($"Sub Population[{i}] Bad Parents No = {countBadSelectedParents} Bad Offspring No = {countBadOffspring} out of {countsIndividuals[i]}");
        countAllBadModels += countBadOffspring;
      }
      Fitness.Clear();
      Fitness.AddRange(newFit);
      Elites.Clear();
      Elites.AddRange(newElites);
      SetElite(maximization);
      int countBadElites = 0;
      foreach (var indElited in Elites) {
        if (indElited.Quality[0] == 1.0) {
          countBadElites++;
        }
      }
      Console.WriteLine($"All Bad Models No = {countAllBadModels} out of {populationSize}");
      Console.WriteLine($"Bad Elites No = {countBadElites} out of {Elites.Count}");
      FindBestWorstAverageQuality(maximization);
      return countEvaluations;
    }
    public int ApplyForInitialization(IRandom random, int populationSize, CooperativeProblem problem) {
      var countEvaluations = 0;
      bool maximization = problem.Maximization[0];
      Initialization(populationSize, problem, random);
      Fitness.Clear();
      Elites.Clear();
      int numSubPopulations = Population.Count;

      for (int i = 0; i < numSubPopulations; i++) {
        List<IndividualGA> subpopulation = Population[i];
        var fit = Evaluation(subpopulation, problem, random);
        countEvaluations += fit.Count;
        Fitness.AddRange(fit);
        var bestIndex = FindElitesIndex(fit, maximization);
        Elites.Add((IndividualGA)Population[i][bestIndex].Clone());
      }
      SetElite(maximization);
      FindBestWorstAverageQuality(maximization);
      return countEvaluations;
    }
      public int ApplyStrictOffspringSelection(int populationSize, CooperativeProblem problem, IRandom random) {
      var countEvaluations = 0;
      bool maximization = problem.Maximization[0];
      
      var subPopCount = Population.Count;
      var countsIndividuals = Population.Select(subpopulation => subpopulation.Count).ToList();
      int subPopIndex = 0;
      int countSuccessfulOffspring = 0;
      int countUnsuccessfulOffspring = 0;
      List<IndividualGA> newElites = new List<IndividualGA>();
      List<double[]> newFit = new List<double[]>();
      int countAllBadModels = 0;
      double adaptiveEps = 0.0;
      if (OffspringComparisonType == OffspringParentsComparisonTypes.EpsilonLexicaseBasedComparison) {
        double[] fitness = new double[populationSize];
        for (int f = 0; f < populationSize; f++) {
          fitness[f] = Fitness[f][0];
        }
        adaptiveEps = AdaptiveEpsilonLexicaseSelection.CalculateMAD(fitness);
      }
      for (int i = 0; i < subPopCount; i++) {
        var fit = new List<double[]>();
        for (int k = subPopIndex; k < countsIndividuals[i] + subPopIndex; k++) {
          fit.Add(Fitness[k].ToArray());
        }
        int countBadSelectedParents = 0;
        int countBadOffspring = 0;
        int numSelectedIndividuals = 2 * (countsIndividuals[i] - 1);
        
        var selectedParents = Selection(fit, Population[i], numSelectedIndividuals, random, maximization);
        foreach (var ind in selectedParents) {
          if (ind.Quality[0] == 1.0) {
            countBadSelectedParents++;
          }
        }
        var newPop = new List<IndividualGA>();
        newPop.Add((IndividualGA)Elites[i].Clone());
        while (newPop.Count < countsIndividuals[i] ) {
          var parentsCount = selectedParents.Count;
          for (int k = 0; k < parentsCount; k += 2) {
              var childTree = Crossover((ISymbolicExpressionTree)selectedParents[k].Solution.Clone(), (ISymbolicExpressionTree)selectedParents[k + 1].Solution.Clone(), random);
              var child = new IndividualGA(_treeRequirements, childTree, QualityLength, Weights[i].ToArray());
              int randomIndex = random.Next(_treeRequirements.Manipulators.Count); // Generate a random index

              if (random.NextDouble() < _treeRequirements.MutationProbability) {
                ISymbolicExpressionTreeManipulator chosenManipulator = _treeRequirements.Manipulators[randomIndex]; // Select the manipulator at the random index
                child.Mutate(chosenManipulator, random);
              }

              var qualities = child.Evaluate(random, problem);
              countEvaluations++;
              bool addToNewPopulation = ComparisonForOffspringSelection(random, (IndividualGA) selectedParents[k].Clone(), (IndividualGA) selectedParents[k + 1].Clone(), (IndividualGA) child.Clone(), problem.Maximization, adaptiveEps);

              if (addToNewPopulation) {
                newPop.Add((IndividualGA)child.Clone());
                countSuccessfulOffspring++;
              } else {
                countUnsuccessfulOffspring++;
              }
              Console.WriteLine($"countSuccessfulOffspring={countSuccessfulOffspring} countUnsuccessfulOffspring={countUnsuccessfulOffspring}");
              
              if (newPop.Count == countsIndividuals[i]) {
                break;
              }
            
          }
        }
        Population[i].Clear();
        if (newPop.Count > countsIndividuals[i] || newPop.Count < countsIndividuals[i])
          throw new Exception($"New Subpopulation[{i}] = {newPop.Count}, Subpopulation[{i}] Size = {countsIndividuals[i]}");
        fit.Clear();
        foreach (var ind in newPop) {
          var quality = ind.Quality.ToArray();
          newFit.Add(quality);
          fit.Add(quality);
        }
        var bestIndex = FindElitesIndex(fit, maximization);
        newElites.Add((IndividualGA)newPop[bestIndex].Clone());
        Population[i].AddRange(newPop);
        subPopIndex += countsIndividuals[i];
        foreach (var qlty in fit) {
          if (qlty[0] == 1.0) {
            countBadOffspring++;
          }
        }
        Console.WriteLine($"Sub Population[{i}] Bad Parents No = {countBadSelectedParents} Bad Offspring No = {countBadOffspring} out of {countsIndividuals[i]}");
        countAllBadModels += countBadOffspring;
      }
      Fitness.Clear();
      Fitness.AddRange(newFit);
      Elites.Clear();
      Elites.AddRange(newElites);
      SetElite(maximization);
      int countBadElites = 0;
      foreach (var indElited in Elites) {
        if (indElited.Quality[0] == 1.0) {
          countBadElites++;
        }
      }
      Console.WriteLine($"All Bad Models No = {countAllBadModels} out of {populationSize}");
      Console.WriteLine($"Bad Elites No = {countBadElites} out of {Elites.Count}");
      FindBestWorstAverageQuality(maximization);

      ActiveSelectionPressure = (double)(countSuccessfulOffspring + countUnsuccessfulOffspring) / populationSize;
      CountSuccessfulOffspring = countSuccessfulOffspring;
      CountUnsuccessfulOffspring = countUnsuccessfulOffspring;
      return countEvaluations;
    }
    private DominationResult ComparisonBasedOnDomination(double[] childQuality, double[] parentQuality, bool[] maximization) {
      return DominationCalculator<ISymbolicExpressionTree>.Dominates(childQuality, parentQuality, maximization, true); 
    }
    public List<double> CreateFitList(int index) {
      List<double> fitList = Fitness.Select(arr => arr[index]).ToList();
      return fitList;
    }
    public DoubleMatrix CalculateDoubleMatrix() {
      DoubleMatrix convertedFit = new DoubleMatrix(Fitness.Count, QualityLength);
      for (int i = 0; i < Fitness.Count; i++) {
        for (int j = 0; j < QualityLength; j++) {
          convertedFit[i, j] = Fitness[i][j];
        }
      }
      return convertedFit;
    }
    public bool ComparisonForOffspringSelection(IRandom random,IndividualGA firstParent,IndividualGA secondParent, IndividualGA child, bool[] maximization, double adaptiveEps) {
      bool addToNewPopulation = false;
      double[] firstParentQuality = firstParent.Quality;
      double[] secondParentQuality = secondParent.Quality;
      double[] childQuality = child.Quality;
      switch (OffspringComparisonType) {
        case OffspringParentsComparisonTypes.DominationBaseComparison:
          double[] parent1Quality = firstParentQuality.Take(QualityLength - 1).ToArray();
          double[] parent2Quality = secondParentQuality.Take(QualityLength - 1).ToArray();
          DominationResult dominationResults;

          double[] chosenParentQuality = new double[QualityLength - 1];
          dominationResults = ComparisonBasedOnDomination(parent1Quality, parent2Quality, maximization);
          if (dominationResults == DominationResult.Dominates) {
            chosenParentQuality = parent1Quality;
          } else if (dominationResults == DominationResult.IsDominated) {
            chosenParentQuality = parent2Quality;
          } else {
            int randomChoosing = random.Next(2);
            if (randomChoosing == 0) {
              chosenParentQuality = parent1Quality;
            } else {
              chosenParentQuality = parent2Quality;
            }
          }
          childQuality = childQuality.Take(QualityLength - 1).ToArray();
          dominationResults = ComparisonBasedOnDomination(childQuality, chosenParentQuality, maximization);
          if (dominationResults == DominationResult.Dominates)
            addToNewPopulation = true;
          break;
        case OffspringParentsComparisonTypes.RandomIndexComparison:
        case OffspringParentsComparisonTypes.AccuracyBasedComparison:
        case OffspringParentsComparisonTypes.WeightedBasedComparison:
          int chosenIndex = 0;
          bool elementWiseMaximization = false;
          if (OffspringComparisonType == OffspringParentsComparisonTypes.RandomIndexComparison) {
            chosenIndex = random.Next(QualityLength);
          }
          if (OffspringComparisonType == OffspringParentsComparisonTypes.WeightedBasedComparison) {
            chosenIndex = QualityLength - 1;
          }
          if (chosenIndex == 0 || chosenIndex == QualityLength - 1) {
            elementWiseMaximization = maximization[0];
          }
          double bestParentQuality;
          double worstParentQuality;
          if (elementWiseMaximization) {
            bestParentQuality = Math.Max(firstParentQuality[chosenIndex], secondParentQuality[chosenIndex]);
            worstParentQuality = Math.Min(firstParentQuality[chosenIndex], secondParentQuality[chosenIndex]);
          } else {
            bestParentQuality = Math.Min(firstParentQuality[chosenIndex], secondParentQuality[chosenIndex]);
            worstParentQuality = Math.Max(firstParentQuality[chosenIndex], secondParentQuality[chosenIndex]);
          }
          if ((elementWiseMaximization && childQuality[chosenIndex] > bestParentQuality)
            || (!elementWiseMaximization && childQuality[chosenIndex] < bestParentQuality)) {
            addToNewPopulation = true;
          }
          break;
        case OffspringParentsComparisonTypes.EpsilonLexicaseBasedComparison:
          bool accuracyWiseMaximization = maximization[0];
          if (accuracyWiseMaximization) {
            bestParentQuality = Math.Max(firstParentQuality[0], secondParentQuality[0]);
            worstParentQuality = Math.Min(firstParentQuality[0], secondParentQuality[0]);
          } else {
            bestParentQuality = Math.Min(firstParentQuality[0], secondParentQuality[0]);
            worstParentQuality = Math.Max(firstParentQuality[0], secondParentQuality[0]);
          }
          if ((accuracyWiseMaximization && childQuality[0] > bestParentQuality)
            || (!accuracyWiseMaximization && childQuality[0] < bestParentQuality)) {
            addToNewPopulation = true;
          } else if ((accuracyWiseMaximization && childQuality[0] * (1 + adaptiveEps) > bestParentQuality)
             || (!accuracyWiseMaximization && childQuality[0] < bestParentQuality * (1 + adaptiveEps))) {
            double bestParentTreeLength = Math.Min(firstParent.NormalizedTreeLength, secondParent.NormalizedTreeLength);
            if (child.NormalizedTreeLength < bestParentTreeLength) {
              addToNewPopulation = true;
            }
          }
          break;
        default:
          throw new ArgumentException("Invalid comparison type");
      }
      return addToNewPopulation;
    }
    #region Cloning 
    public override IDeepCloneable Clone(Cloner cloner) {
      return new DecompositionBasedGA(this, cloner);
    }
    #endregion
  }
}
