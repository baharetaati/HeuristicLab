using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Problems.CooperativeProblem;

namespace HeuristicLab.Algorithms.CoevolutionaryAlgorithm {
  [Item("GA with Offspring Selection", "Selects among the offspring population those that are designated successful and discards the unsuccessful offspring, except for some lucky losers. It expects the parent scopes to be below the first sub-scope, and offspring scopes to be below the second sub-scope separated again in two sub-scopes, the first with the failed offspring and the second with successful offspring.")]
  [StorableType("2ECB21A0-0D40-4513-A384-69D4418821A9")]
  public class GAWithOffspringSelection : DeepCloneable {

    #region Properties
    [Storable]
    private readonly TreeRequirements _treeRequirements;
    public ISelectionStrategy Selector { get; }
    [Storable]
    public List<IndividualGA> Population { get; set; }
    [Storable]
    public List<double[]> Fitness { get; set; }
    [Storable]
    public int QualityLength { get; private set; }
    [Storable]
    public double MaximumSelectionPressure { get; private set; }
    [Storable]
    public double SuccessRatio { get; private set; }
    [Storable]
    public double ComparisonFactor { get; private set; }
    #endregion

    #region Constructors and Cloning
    [StorableConstructor]
    protected GAWithOffspringSelection(StorableConstructorFlag _) { }
    public GAWithOffspringSelection(int qualityLength, TreeRequirements treeRequirements, ISelectionStrategy selector, double maxSelPressure, double successRate, double comparisonFactor) {
      QualityLength = qualityLength;
      Population = new List<IndividualGA>();
      Fitness = new List<double[]>();
      Selector = selector;
      _treeRequirements = treeRequirements;
      MaximumSelectionPressure = maxSelPressure;
      SuccessRatio = successRate;
      ComparisonFactor = comparisonFactor;
    }
    protected GAWithOffspringSelection(GAWithOffspringSelection original, Cloner cloner) : base(original, cloner) {
      QualityLength = original.QualityLength;
      Population = original.Population.Select(cloner.Clone).ToList();
      Fitness = original.Fitness.Select(arr => arr.ToArray()).ToList();
      Selector = cloner.Clone(original.Selector);
      _treeRequirements = cloner.Clone(original._treeRequirements);
      MaximumSelectionPressure = original.MaximumSelectionPressure;
      SuccessRatio = original.SuccessRatio;
      ComparisonFactor = original.ComparisonFactor;
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new GAWithOffspringSelection(this, cloner);
    }
    #endregion
    public void Initialization(int popSize, CooperativeProblem problem, IRandom random) {
      for (int i = 0; i < popSize; i++) {
        var individual = new IndividualGA(_treeRequirements, QualityLength, problem, random);
        Population.Add(individual);
      }
    }
    private List<IndividualGA> Selection(int numSelectedIndividuals, IRandom random, CooperativeProblem problem) {
      List<double> fitnessValues;
      fitnessValues = Fitness.Select(fitness => fitness.Last()).ToList();

      List<int> selectedIndexes = Selector.Select(numSelectedIndividuals, random, fitnessValues, problem);
      List<IndividualGA> selectedIndividuals = new List<IndividualGA>(numSelectedIndividuals);
      foreach (var index in selectedIndexes) {
        selectedIndividuals.Add((IndividualGA)Population[index].Clone());
      }
      return selectedIndividuals;
    }
    private ISymbolicExpressionTree Crossover(ISymbolicExpressionTree parent1, ISymbolicExpressionTree parent2, TreeRequirements _treeRequirements, IRandom random) {
      var crossOver = new SubtreeCrossover();
      var childTree = SubtreeCrossover.Cross(random, parent1, parent2, crossOver.CrossoverProbability, crossOver.InternalCrossoverPointProbability.Value, _treeRequirements.MaxTreeLength, _treeRequirements.MaxTreeDepth);
      return childTree;
    }
    public int Apply(int populationSize, int numSelectedIndividuals, CooperativeProblem problem, int iterationNum, IRandom random) {
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
      var selectedParents = Selection(numSelectedIndividuals, random, problem);
      var newPop = new List<IndividualGA>();
      var pool = new List<IndividualGA>();
      bool exitLoop = false;
      while (newPop.Count < SuccessRatio * populationSize || !exitLoop) {
        for (int i = 0; i < selectedParents.Count; i += 2) {
          var childTree = Crossover(selectedParents[i].Solution, selectedParents[i + 1].Solution, _treeRequirements, random);
          var child = new IndividualGA(_treeRequirements, childTree, QualityLength);
          int randomIndex = random.Next(_treeRequirements.Manipulators.Count); // Generate a random index

          if (random.NextDouble() < _treeRequirements.MutationProbability) {
            ISymbolicExpressionTreeManipulator chosenManipulator = _treeRequirements.Manipulators[randomIndex]; // Select the manipulator at the random index
            child.Mutate(chosenManipulator, random);
          }
          
          var qualities = child.Evaluate(random, problem);
          countEvaluations++;
          double val = Math.Min(selectedParents[i].Quality[problem.NumObjectives], selectedParents[i + 1].Quality[problem.NumObjectives]);
          if (ComparisonFactor == 0.0) {
            val = Math.Max(selectedParents[i].Quality[problem.NumObjectives], selectedParents[i + 1].Quality[problem.NumObjectives]);
          } 
          if (!problem.Maximization[0]) {
            if (qualities[problem.NumObjectives] < val) {
              newPop.Add(child);
            } else {
              pool.Add(child);
            }
          }
          var pActive = (newPop.Count + pool.Count) / populationSize;
          if (MaximumSelectionPressure < pActive) {
            exitLoop = true;
            break;
          }
        }
      }

      while (newPop.Count < populationSize) {
        if (pool.Count > 0) {
          int randomIndex = random.Next(pool.Count);
          newPop.Add(pool[randomIndex]);
          pool.RemoveAt(randomIndex);
        } else {
          int randomIndex = random.Next(populationSize);
          newPop.Add(Population[randomIndex]);
          Population.RemoveAt(randomIndex);
        }
      }

      Population.Clear();
      Fitness.Clear();

      foreach (var individual in newPop) {
        Fitness.Add(individual.Quality.ToArray());
      }
      Population.AddRange(newPop);
      return countEvaluations;
    }
  }
}