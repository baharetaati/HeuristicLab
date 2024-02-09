using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using HEAL.Attic;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Problems.CooperativeProblem;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Regression;


namespace HeuristicLab.Algorithms.CoevolutionaryAlgorithm {
  [StorableType("19705893-7AE0-4EB2-AC4F-6AB18FAB7E4F")]
  public class SingleObjectiveGA : BaseAlg {
    #region Properties
    public ISelectionStrategy<double> Selector { get; }
    [Storable]
    public List<SingleObjectiveIndividual> Population { get; set; } = new List<SingleObjectiveIndividual>();
    [Storable]
    public List<double> Fitness { get; set; }
    [Storable]
    public SingleObjectiveIndividual Elite { get; set; }
    [Storable]
    public double BestQuality { get; set; }
    [Storable]
    public double WorstQuality { get; set; }
    [Storable]
    public double AverageQuality { get; set; }
    [Storable]
    public double ActiveSelectionPressure { get; set; }
    [Storable]
    public int CountSuccessfulOffspring { get; set; }
    [Storable]
    public int CountUnsuccessfulOffspring { get; set; }
    #endregion

    #region Constructors
    [StorableConstructor]
    protected SingleObjectiveGA(StorableConstructorFlag _) : base(_){ }
    public SingleObjectiveGA(TreeRequirements treeRequirements, ISelectionStrategy<double> selector):base(treeRequirements) {

      Population = new List<SingleObjectiveIndividual>();
      Elite = null;
      Fitness = new List<double>();
      Selector = selector;
      BestQuality = -1.0;
      WorstQuality = -1.0;
      AverageQuality = -1.0;
      ActiveSelectionPressure = -1.0;
      CountSuccessfulOffspring = 0;
      CountUnsuccessfulOffspring = 0;
    }
    public SingleObjectiveGA(TreeRequirements treeRequirements, ISelectionStrategy<double> selector, double pActive = 0.0):base(treeRequirements) {
      Population = new List<SingleObjectiveIndividual>();
      Elite = null;
      Fitness = new List<double>();
      Selector = selector;
      BestQuality = -1.0;
      WorstQuality = -1.0;
      AverageQuality = -1.0;
      ActiveSelectionPressure = -1.0;
      CountSuccessfulOffspring = 0;
      CountUnsuccessfulOffspring = 0;
    }
    protected SingleObjectiveGA(SingleObjectiveGA original, Cloner cloner):base(original, cloner) {
      Population = original.Population.Select(cloner.Clone).ToList();
      Elite = cloner.Clone(original.Elite);
      Fitness = original.Fitness;
      Selector = cloner.Clone(original.Selector);
      BestQuality = original.BestQuality;
      WorstQuality = original.WorstQuality;
      AverageQuality = original.AverageQuality;
      ActiveSelectionPressure = original.ActiveSelectionPressure;
      CountUnsuccessfulOffspring = original.CountSuccessfulOffspring;
      CountUnsuccessfulOffspring = original.CountUnsuccessfulOffspring;
    }
    #endregion
    public void Initialization(int popSize, CooperativeProblem problem, IRandom random) {
      if (_treeRequirements != null) {
        for (int i = 0; i < popSize; i++) {
          var individual = new SingleObjectiveIndividual(_treeRequirements, problem, random);
          Population.Add(individual);
        }
      }
    }
    private List<double> Evaluation(List<SingleObjectiveIndividual> pop, CooperativeProblem problem, IRandom random) {
      List<double> fit = new List<double>();
      foreach (var individual in pop) {
        var qlty = individual.Evaluate(random, problem);
        fit.Add(qlty);
      }
      return fit;
    }
    private int FindEliteIndex(List<double> fit, bool maximization) {
      List<double> tempFit = new List<double>();
      tempFit.AddRange(fit);

      bool sortDescending = false;
      //bool maximization = problem.Maximization[0];
      if (maximization)
        sortDescending = true;
      List<int> indexes = Enumerable.Range(0, tempFit.Count).ToList();
      if (sortDescending) {
        indexes.Sort((i1, i2) => -tempFit[i1].CompareTo(tempFit[i2]));
      } else {
        indexes.Sort((i1, i2) => tempFit[i1].CompareTo(tempFit[i2]));
      }
      
      int bestIndex = indexes.First();
      BestQuality = fit[bestIndex];
      WorstQuality = fit[indexes.Last()];
      AverageQuality = CalculateAverage(fit);
      return bestIndex;
    }
    private List<SingleObjectiveIndividual> Selection(List<double> fit, List<SingleObjectiveIndividual> pop, int numSelectedIndividuals, IRandom random, bool maximization) {
      
      List<int> selectedIndexes = Selector.Select(numSelectedIndividuals, random, fit, maximization);
      List<SingleObjectiveIndividual> selectedIndividuals = new List<SingleObjectiveIndividual>(numSelectedIndividuals);
      foreach (var index in selectedIndexes) {
        selectedIndividuals.Add((SingleObjectiveIndividual)pop[index].Clone());
      }
      return selectedIndividuals;
    }
    private void UpdatePopulation(IRandom random, int popSize, List<SingleObjectiveIndividual> pop, List<double> fit, CooperativeProblem problem) {
      var sortDescending = false;
      var maximization = problem.Maximization[0];
      //var nextGenerationSlctr = new GeneralizedRankSelection();

      List<int> indexes = Enumerable.Range(0, fit.Count).ToList();
      //List<int> indexes = nextGenerationSlctr.Select(popSize, random, fit, problem);
      if (maximization) {
        sortDescending = true;
      }
      if (sortDescending) {
        indexes.Sort((i1, i2) => -fit[i1].CompareTo(fit[i2]));
      } else {
        indexes.Sort((i1, i2) => fit[i1].CompareTo(fit[i2]));
      }
      List<SingleObjectiveIndividual> newPopulation = new List<SingleObjectiveIndividual>();
      List<double> newFitness = new List<double>();
      

      for (int i = 0; i < popSize; i++) {
        newPopulation.Add((SingleObjectiveIndividual)pop[indexes[i]].Clone());
        newFitness.Add(fit[indexes[i]]);
      }
      Population.Clear();
      Fitness.Clear();
      Population.AddRange(newPopulation);
      Fitness.AddRange(newFitness);
    }
    public int Apply(int populationSize, int numSelectedIndividuals, CooperativeProblem problem, IRandom random) {
      var countEvaluations = 0;
      bool maximization = problem.Maximization[0];
      var selectedParents = Selection(Fitness, Population, numSelectedIndividuals, random, maximization);
      List<SingleObjectiveIndividual> offspring = new List<SingleObjectiveIndividual>();
      for (int j = 0; j < selectedParents.Count; j += 2) {
        var childTree = Crossover((ISymbolicExpressionTree)selectedParents[j].Solution.Clone(), (ISymbolicExpressionTree) selectedParents[j + 1].Solution.Clone(), random);
        var child = new SingleObjectiveIndividual(_treeRequirements, childTree);
        int randomIndex = random.Next(_treeRequirements.Manipulators.Count); // Generate a random index

        if (random.NextDouble() < _treeRequirements.MutationProbability) {
          ISymbolicExpressionTreeManipulator chosenManipulator = _treeRequirements.Manipulators[randomIndex]; // Select the manipulator at the random index
          child.Mutate(chosenManipulator, random);
        }
        offspring.Add(child);
      }
      //Population.Clear();
      //Fitness.Clear();
      
      //Population.Add((SingleObjectiveIndividual)Elites.Clone());
      //Fitness.Add(Elites.Quality);
      var offspringFit = Evaluation(offspring, problem, random);
      for (int i = 0; i < offspringFit.Count; i++) {
        if (offspringFit[i] == 1.0) {
          Console.WriteLine("Printing offspring expression tree ================");
          foreach (var node in offspring[i].Solution.IterateNodesBreadth()) {
            Console.Write($"{node.Symbol.ToString()}");
            Console.WriteLine();
          }
        }
      }
      countEvaluations += offspringFit.Count;
      Population.AddRange(offspring);
      Fitness.AddRange(offspringFit);
      UpdatePopulation(random, populationSize, Population, Fitness, problem);
      if (Population.Count > populationSize || Population.Count < populationSize)
        throw new Exception($"Current Population Size = {Population.Count}, Population Size = {populationSize}");
      int bestIndex = FindEliteIndex(Fitness, maximization);
      Elite = (SingleObjectiveIndividual)Population[bestIndex].Clone();

      return countEvaluations;
  }
    public int ApplyOffspringSelection(int populationSize, int numSelectedIndividuals, CooperativeProblem problem, double maxSelPressure, double successRate, IRandom random) {
      var countEvaluations = 0;
      bool maximization = problem.Maximization[0];
      var selectedParents = Selection(Fitness, Population, numSelectedIndividuals, random, maximization);
      var newPop = new List<SingleObjectiveIndividual>();
      var pool = new List<SingleObjectiveIndividual>();
      newPop.Add((SingleObjectiveIndividual)Elite.Clone());
      bool exitLoop = false;
      while (newPop.Count < successRate * populationSize && !exitLoop) {
        for (int k = 0; k < selectedParents.Count; k += 2) {
          var childTree = Crossover(selectedParents[k].Solution, selectedParents[k + 1].Solution, random);
          var child = new SingleObjectiveIndividual(_treeRequirements, childTree);
          int randomIndex = random.Next(_treeRequirements.Manipulators.Count); // Generate a random index

          if (random.NextDouble() < _treeRequirements.MutationProbability) {
            ISymbolicExpressionTreeManipulator chosenManipulator = _treeRequirements.Manipulators[randomIndex]; // Select the manipulator at the random index
            child.Mutate(chosenManipulator, random);
          }

          var quality = child.Evaluate(random, problem);
          countEvaluations++;
          double bestParentQuality;
          double worstParentQuality;
          if (maximization) {
            bestParentQuality = Math.Max(selectedParents[k].Quality, selectedParents[k + 1].Quality);
            worstParentQuality = Math.Min(selectedParents[k].Quality, selectedParents[k + 1].Quality);
          } else {
            bestParentQuality = Math.Min(selectedParents[k].Quality, selectedParents[k + 1].Quality);
            worstParentQuality = Math.Max(selectedParents[k].Quality, selectedParents[k + 1].Quality);
          }
          if ((maximization && quality > bestParentQuality)
            || (!maximization && quality < bestParentQuality)) {
            newPop.Add(child);
          } else {
            pool.Add(child);
          }
          ActiveSelectionPressure = (double)(newPop.Count + pool.Count) / populationSize;
          if (maxSelPressure < ActiveSelectionPressure) {
            exitLoop = true;
            break;
          }
        }
      }

      HashSet<SingleObjectiveIndividual> poolSet = new HashSet<SingleObjectiveIndividual>(pool);
      HashSet<SingleObjectiveIndividual> populationSet = new HashSet<SingleObjectiveIndividual>(Population);

      while (newPop.Count < populationSize) {
        if (poolSet.Count > 0) {
          int randomIndex = random.Next(poolSet.Count);
          SingleObjectiveIndividual selectedElement = poolSet.ElementAt(randomIndex);
          newPop.Add(selectedElement);
          poolSet.Remove(selectedElement);
        } else {
          int randomIndex = random.Next(populationSet.Count);
          SingleObjectiveIndividual selectedElement = populationSet.ElementAt(randomIndex);
          newPop.Add(selectedElement);
          populationSet.Remove(selectedElement);
        }
      }
      Population.Clear();
      Population.AddRange(newPop);
      if (Population.Count > populationSize || Population.Count < populationSize)
        throw new Exception($"Offspring Selection Population = {newPop.Count}, Population Size = {populationSize}");
      Fitness.Clear();
      foreach (var ind in Population) {
        Fitness.Add(ind.Quality);
      }
      int bestIndex = FindEliteIndex(Fitness, maximization);
      Elite = (SingleObjectiveIndividual)Population[bestIndex].Clone();
      return countEvaluations;
    
    }
    public int ApplyForInitialization(IRandom random, int populationSize, CooperativeProblem problem) {
      int countEvaluations = 0;
      bool maximization = problem.Maximization[0];
      Initialization(populationSize, problem, random);
      Fitness.Clear();
      var fit = Evaluation(Population, problem, random);
      countEvaluations += fit.Count;
      Fitness.AddRange(fit);

      int bestIndex = FindEliteIndex(Fitness, maximization);

      Elite = (SingleObjectiveIndividual)Population[bestIndex].Clone();

      return countEvaluations;
    }
    public int ApplyExtremeOffspringSelection(int populationSize, int numSelectedIndividuals, CooperativeProblem problem, IRandom random) {
      int countEvaluations = 0;
      bool maximization = problem.Maximization[0];
      var selectedParents = Selection(Fitness, Population, numSelectedIndividuals, random, maximization);
      var newPop = new List<SingleObjectiveIndividual>();
      newPop.Add((SingleObjectiveIndividual)Elite.Clone());
      int countSuccess = 0;
      int countUnsuccess = 0;
      var qualities = new List<double>();
      qualities.Add(Elite.Quality);
      double tmpSelPress = 0.0;
      double tmpSelPressInc = 1.0 / populationSize;
      int badOffspring = 0;
      while (newPop.Count < populationSize) {
        for (int k = 0; k < selectedParents.Count; k += 2) {
            var childTree = Crossover((ISymbolicExpressionTree)selectedParents[k].Solution.Clone(), (ISymbolicExpressionTree)selectedParents[k + 1].Solution.Clone(), random);
            var child = new SingleObjectiveIndividual(_treeRequirements, childTree);
            int randomIndex = random.Next(_treeRequirements.Manipulators.Count); // Generate a random index

            if (random.NextDouble() < _treeRequirements.MutationProbability) {
              ISymbolicExpressionTreeManipulator chosenManipulator = _treeRequirements.Manipulators[randomIndex]; // Select the manipulator at the random index
              child.Mutate(chosenManipulator, random);
            }

            var quality = child.Evaluate(random, problem);
            
            countEvaluations++;
            double bestParentQuality;
            double worstParentQuality;
            if (maximization) {
              bestParentQuality = Math.Max(selectedParents[k].Quality, selectedParents[k + 1].Quality);
              worstParentQuality = Math.Min(selectedParents[k].Quality, selectedParents[k + 1].Quality);
            } else {
              bestParentQuality = Math.Min(selectedParents[k].Quality, selectedParents[k + 1].Quality);
              worstParentQuality = Math.Max(selectedParents[k].Quality, selectedParents[k + 1].Quality);
            }
            if ((maximization && quality > bestParentQuality)
              || (!maximization && quality < bestParentQuality)) {
              countSuccess++;
              newPop.Add((SingleObjectiveIndividual)child.Clone());
              qualities.Add(quality);
            } else {
              countUnsuccess++;
              //pool.Add(child);
            }
            tmpSelPress += tmpSelPressInc;
            if (newPop.Count == populationSize) {
              break;
            }
        }
      }
      Console.WriteLine($"badOffspringCount = {badOffspring}");
      Population.Clear();
      Population.AddRange(newPop);
      //if (Population.Count > populationSize || Population.Count < populationSize)
      //  throw new Exception($"Offspring Selection Population = {Population.Count}, Population Size = {populationSize}");
      Fitness.Clear();
      Fitness.AddRange(qualities);
      int bestIndex = FindEliteIndex(Fitness, maximization);
      Elite = (SingleObjectiveIndividual)Population[bestIndex].Clone();
      ActiveSelectionPressure = (double)(countUnsuccess + countSuccess) / populationSize;
      //ActiveSelectionPressure = tmpSelPress;
      CountSuccessfulOffspring = countSuccess;
      CountUnsuccessfulOffspring = countUnsuccess;
      return countEvaluations;
    }
    public List<SingleObjectiveIndividual> ApplySelectionAndClearation(IRandom random, int numSelectedIndividuals, bool maximization) {
      var selectedParents = Selection(Fitness, Population, numSelectedIndividuals, random, maximization);
      Population.Clear();
      Fitness.Clear();
      Population.Add((SingleObjectiveIndividual)Elite.Clone());
      Fitness.Add(Elite.Quality);
      return selectedParents;
    }
    public int ApplyBatchWiseExtremeOffspringSelection(int populationSize, List<SingleObjectiveIndividual> selectedParents, CooperativeProblem problem, IRandom random) {
      int countEvaluations = 0;
      bool maximization = problem.Maximization[0];
      var newPop = new List<SingleObjectiveIndividual>();
      int countSuccess = 0;
      int countUnsuccess = 0;
      var qualities = new List<double>();
      double tmpSelPress = 0.0;
      double tmpSelPressInc = 1.0 / populationSize;
      
      for (int k = 0; k < selectedParents.Count; k += 2) {
        var childTree = Crossover((ISymbolicExpressionTree)selectedParents[k].Solution.Clone(), (ISymbolicExpressionTree)selectedParents[k + 1].Solution.Clone(), random);
        var child = new SingleObjectiveIndividual(_treeRequirements, childTree);
        int randomIndex = random.Next(_treeRequirements.Manipulators.Count); // Generate a random index

        if (random.NextDouble() < _treeRequirements.MutationProbability) {
          ISymbolicExpressionTreeManipulator chosenManipulator = _treeRequirements.Manipulators[randomIndex]; // Select the manipulator at the random index
          child.Mutate(chosenManipulator, random);
        }

        var quality = child.Evaluate(random, problem);
        countEvaluations++;
        double bestParentQuality;
        double worstParentQuality;
        if (maximization) {
          bestParentQuality = Math.Max(selectedParents[k].Quality, selectedParents[k + 1].Quality);
          worstParentQuality = Math.Min(selectedParents[k].Quality, selectedParents[k + 1].Quality);
        } else {
          bestParentQuality = Math.Min(selectedParents[k].Quality, selectedParents[k + 1].Quality);
          worstParentQuality = Math.Max(selectedParents[k].Quality, selectedParents[k + 1].Quality);
        }
        if ((maximization && quality > bestParentQuality)
          || (!maximization && quality < bestParentQuality)) {
          countSuccess++;
          newPop.Add((SingleObjectiveIndividual)child.Clone());
          qualities.Add(quality);
        } else {
          countUnsuccess++;
          //pool.Add(child);
        }
        tmpSelPress += tmpSelPressInc;
        if (newPop.Count == populationSize-Population.Count) {
          break;
        }
      }
      
      Population.AddRange(newPop);
      //if (Population.Count > populationSize || Population.Count < populationSize)
      //  throw new Exception($"Offspring Selection Population = {Population.Count}, Population Size = {populationSize}");
      Fitness.AddRange(qualities);
      if (Population.Count == populationSize) {
        int bestIndex = FindEliteIndex(Fitness, maximization);
        Elite = (SingleObjectiveIndividual)Population[bestIndex].Clone();
      }
      //ActiveSelectionPressure = (double)(countUnsuccess + countSuccess) / populationSize;
      ActiveSelectionPressure = tmpSelPress;
      CountSuccessfulOffspring = countSuccess;
      CountUnsuccessfulOffspring = countUnsuccess;
      return countEvaluations;
    }
      public DoubleMatrix CalculateDoubleMatrix() {
    DoubleMatrix convertedFit = new DoubleMatrix(Fitness.Count, 1);
    for (int i = 0; i < Fitness.Count; i++) {
      convertedFit[i, 0] = Fitness[i];
    }
    return convertedFit;
    }
    #region Cloning 
    public override IDeepCloneable Clone(Cloner cloner) {
    return new SingleObjectiveGA(this, cloner);
  }
  #endregion
  }
}
