using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using HEAL.Attic;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Problems.CooperativeProblem;
using HeuristicLab.Selection;

namespace HeuristicLab.Algorithms.CoevolutionaryAlgorithm {
  [StorableType("8D1EDC9E-1EE5-4F20-8718-C14286511C92")]
  public class GA : DeepCloneable {
    #region Properties
    [Storable]
    private readonly TreeRequirements _treeRequirements;
    public ISelectionStrategy Selector { get; }
    [Storable]
    public List<IndividualGA> Population { get; set; } 
    //[Storable]
    //public List<IndividualGA> Elites { get; set; }
    [Storable]
    public List<double[]> Fitness { get; set; }
    //[Storable]
    //public List<double[]> ElitesFitness { get; set; }
    [Storable]
    public int QualityLength { get; private set; }
    [Storable]
    public double PActive { get; set; }
    public bool IsSingleObjective { get;}
    #endregion

    #region Constructors
    [StorableConstructor]
    protected GA(StorableConstructorFlag _) { }
    public GA(int qualityLength, TreeRequirements treeRequirements, ISelectionStrategy selector, bool isSingleObjective, int eliteSize = 1) {

      QualityLength = qualityLength;
      Population = new List<IndividualGA>();
      Fitness = new List<double[]>();
      Selector = selector;
      _treeRequirements = treeRequirements;
      IsSingleObjective = isSingleObjective;
    }
    public GA(int qualityLength, TreeRequirements treeRequirements, ISelectionStrategy selector, double pActive = 0.0, int eliteSize = 1) {
      QualityLength = qualityLength;
      Population = new List<IndividualGA>();
      Fitness = new List<double[]>();
      Selector = selector;
      _treeRequirements = treeRequirements;
      PActive = pActive;
    }
    protected GA(GA original, Cloner cloner) {
      QualityLength = original.QualityLength;
      Population = original.Population.Select(cloner.Clone).ToList();
      Fitness = original.Fitness.Select(arr => arr.ToArray()).ToList();
      Selector = cloner.Clone(original.Selector);
      _treeRequirements = cloner.Clone(original._treeRequirements);
      IsSingleObjective = original.IsSingleObjective;
    }
    #endregion
    public void Initialization(int popSize, CooperativeProblem problem, IRandom random) {
      for (int i = 0; i < popSize; i++) {
        var individual = new IndividualGA(_treeRequirements, QualityLength, problem, random);
        Population.Add(individual);
      }
    }
    private List<double[]> Evaluation(List<IndividualGA> pop, CooperativeProblem problem, IRandom random) {
      List<double[]> fit = new List<double[]>();
      if (IsSingleObjective) {
        foreach (var individual in pop) {
          var qualites = individual.EvaluateSingleObjective(random, problem);
          fit.Add(qualites);
        }
      } else {
        foreach (var individual in pop) {
          var qualities = individual.Evaluate(random, problem);
          fit.Add(qualities);
        }
      }
      return fit;
    }
    private int FindElitesIndex(CooperativeProblem problem) {
      List<double[]> tempFit = new List<double[]>();
      tempFit.AddRange(Fitness);
      
      bool sortDescending = false;
      bool maximization = problem.Maximization[0];
      if (maximization) 
        sortDescending = true;
      List<int> indexes = Enumerable.Range(0, tempFit.Count).ToList();
      if (IsSingleObjective) {
        if (sortDescending) {
          indexes.Sort((i1, i2) => -tempFit[i1][0].CompareTo(tempFit[i2][0]));
        } else {
          indexes.Sort((i1, i2) => tempFit[i1][0].CompareTo(tempFit[i2][0]));
        }
      } else {
        if (sortDescending) {
          indexes.Sort((i1, i2) => -tempFit[i1][QualityLength - 1].CompareTo(tempFit[i2][QualityLength - 1]));
        } else {
          indexes.Sort((i1, i2) => tempFit[i1][QualityLength - 1].CompareTo(tempFit[i2][QualityLength - 1]));
        }
      }
      int bestIndex = indexes[0];
      //for (int i = 0; i < indexes.Count; i++) {
      //  if (i < elitesSize) {
      //    ElitesFitness.Add(tempFit[indexes[i]]);
      //    Elites.Add((IndividualGA)Population[indexes[i]].Clone());
      //  } else
      //    break;
      //}
      return bestIndex;
    }
    private List<IndividualGA> Selection(int numSelectedIndividuals, IRandom random, CooperativeProblem problem) {
      List<double> fitnessValues;
      if (IsSingleObjective) {
        fitnessValues = Fitness.Select(fitness => fitness.First()).ToList();
      } else {
        fitnessValues = Fitness.Select(fitness => fitness.Last()).ToList();
      }
      
      List<int> selectedIndexes = Selector.Select(numSelectedIndividuals, random, fitnessValues, problem);
      List<IndividualGA> selectedIndividuals = new List<IndividualGA>(numSelectedIndividuals);
      foreach (var index in selectedIndexes) {
        selectedIndividuals.Add((IndividualGA)Population[index].Clone());
      }
      return selectedIndividuals;
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
        countEvaluations +=  fit.Count;
        Fitness.AddRange(fit);
      }
      var bestIndex = FindElitesIndex(problem);
      var elites = (IndividualGA)Population[bestIndex].Clone();
      var selectedParents = Selection(numSelectedIndividuals, random, problem);
      List<IndividualGA> offspring = new List<IndividualGA>();

      for (int i = 0; i < selectedParents.Count; i += 2) {
        var childTree = Crossover(selectedParents[i].Solution, selectedParents[i + 1].Solution, random);
        var child = new IndividualGA(_treeRequirements, childTree, QualityLength);
        int randomIndex = random.Next(_treeRequirements.Manipulators.Count); // Generate a random index

        if (random.NextDouble() < _treeRequirements.MutationProbability) {
          ISymbolicExpressionTreeManipulator chosenManipulator = _treeRequirements.Manipulators[randomIndex]; // Select the manipulator at the random index
          child.Mutate(chosenManipulator, random);
        }
        offspring.Add(child);
      }
      Population.Clear();
      Fitness.Clear();
      Population.Add(elites);
      Fitness.Add(elites.Quality);
     
      
      var offspringFit = Evaluation(offspring, problem, random);
      countEvaluations += offspringFit.Count;
      Population.AddRange(offspring);
      Fitness.AddRange(offspringFit);
      
      return countEvaluations;
    }
    public int ApplyOffspringSelection(int populationSize, int numSelectedIndividuals, CooperativeProblem problem, int iterationNum, double maxSelPressure, double successRate, IRandom random) {
      var countEvaluations = 0;

      if (iterationNum == 0) {
        Initialization(populationSize, problem, random);
        Fitness.Clear();
        List<double[]> fit = new List<double[]>();
        foreach (var individual in Population) {
          var qualities = individual.Evaluate(random, problem);
          fit.Add(qualities);
        }
        countEvaluations += fit.Count;
        Fitness.AddRange(fit);
      }

      var bestIndex = FindElitesIndex(problem);
      var elites = (IndividualGA)Population[bestIndex].Clone();
      var selectedParents = Selection(numSelectedIndividuals, random, problem);
      var newPop = new List<IndividualGA>();
      var pool = new List<IndividualGA>();
      newPop.Add(elites);
      bool exitLoop = false;
      bool maximization = problem.Maximization[0];

      while (newPop.Count < successRate * populationSize && !exitLoop) {
        for (int i = 0; i < selectedParents.Count; i += 2) {
          var childTree = Crossover(selectedParents[i].Solution, selectedParents[i + 1].Solution, random);
          var child = new IndividualGA(_treeRequirements, childTree, QualityLength);
          int randomIndex = random.Next(_treeRequirements.Manipulators.Count); // Generate a random index

          if (random.NextDouble() < _treeRequirements.MutationProbability) {
            ISymbolicExpressionTreeManipulator chosenManipulator = _treeRequirements.Manipulators[randomIndex]; // Select the manipulator at the random index
            child.Mutate(chosenManipulator, random);
          }

          var qualities = child.Evaluate(random, problem);
          countEvaluations++;
          double bestParentQuality;
          double worstParentQuality;
          if (maximization) {
            bestParentQuality = Math.Max(selectedParents[i].Quality[QualityLength - 1], selectedParents[i + 1].Quality[QualityLength - 1]);
            worstParentQuality = Math.Min(selectedParents[i].Quality[QualityLength - 1], selectedParents[i + 1].Quality[QualityLength - 1]);
          } else {
            bestParentQuality = Math.Min(selectedParents[i].Quality[QualityLength - 1], selectedParents[i + 1].Quality[QualityLength - 1]);
            worstParentQuality = Math.Max(selectedParents[i].Quality[QualityLength - 1], selectedParents[i + 1].Quality[QualityLength - 1]);
          }
          if ((maximization && qualities[QualityLength - 1] > bestParentQuality)
            || (!maximization && qualities[QualityLength - 1] < bestParentQuality)) {
            newPop.Add(child);
          } else {
            pool.Add(child);
          }
          
          PActive = (double)(newPop.Count + pool.Count) / populationSize;
          if (maxSelPressure < PActive) {
            exitLoop = true;
            break;
          }
        }
      }

      HashSet<IndividualGA> poolSet = new HashSet<IndividualGA>(pool);
      HashSet<IndividualGA> populationSet = new HashSet<IndividualGA>(Population);

      while (newPop.Count < populationSize) {
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

      Population.Clear();
      Fitness.Clear();

      foreach (var individual in newPop) {
        Fitness.Add(individual.Quality);
      }
      if (newPop.Count > populationSize || newPop.Count < populationSize)
        throw new Exception($"Offspring Selection Population = {newPop.Count}, Population Size = {populationSize}");
      
      Population.AddRange(newPop);
      return countEvaluations;
    }
    #region Cloning 
    public override IDeepCloneable Clone(Cloner cloner) {
      return new GA(this, cloner);
    }
    #endregion
  }
}
