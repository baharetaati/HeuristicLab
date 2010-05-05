#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

using HeuristicLab.Analysis;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Selection;

namespace HeuristicLab.Algorithms.OffspringSelectionGeneticAlgorithm {
  /// <summary>
  /// An operator which represents the main loop of an offspring selection genetic algorithm.
  /// </summary>
  [Item("OffspringSelectionGeneticAlgorithmMainLoop", "An operator which represents the main loop of an offspring selection genetic algorithm.")]
  [StorableClass]
  public sealed class OffspringSelectionGeneticAlgorithmMainLoop : AlgorithmOperator {
    #region Parameter properties
    public ValueLookupParameter<IRandom> RandomParameter {
      get { return (ValueLookupParameter<IRandom>)Parameters["Random"]; }
    }
    public ValueLookupParameter<BoolValue> MaximizationParameter {
      get { return (ValueLookupParameter<BoolValue>)Parameters["Maximization"]; }
    }
    public ScopeTreeLookupParameter<DoubleValue> QualityParameter {
      get { return (ScopeTreeLookupParameter<DoubleValue>)Parameters["Quality"]; }
    }
    public ValueLookupParameter<IOperator> SelectorParameter {
      get { return (ValueLookupParameter<IOperator>)Parameters["Selector"]; }
    }
    public ValueLookupParameter<IOperator> CrossoverParameter {
      get { return (ValueLookupParameter<IOperator>)Parameters["Crossover"]; }
    }
    public ValueLookupParameter<PercentValue> MutationProbabilityParameter {
      get { return (ValueLookupParameter<PercentValue>)Parameters["MutationProbability"]; }
    }
    public ValueLookupParameter<IOperator> MutatorParameter {
      get { return (ValueLookupParameter<IOperator>)Parameters["Mutator"]; }
    }
    public ValueLookupParameter<IOperator> EvaluatorParameter {
      get { return (ValueLookupParameter<IOperator>)Parameters["Evaluator"]; }
    }
    public ValueLookupParameter<IntValue> ElitesParameter {
      get { return (ValueLookupParameter<IntValue>)Parameters["Elites"]; }
    }
    public ValueLookupParameter<IntValue> MaximumGenerationsParameter {
      get { return (ValueLookupParameter<IntValue>)Parameters["MaximumGenerations"]; }
    }
    public ValueLookupParameter<VariableCollection> ResultsParameter {
      get { return (ValueLookupParameter<VariableCollection>)Parameters["Results"]; }
    }
    public ValueLookupParameter<IOperator> AnalyzerParameter {
      get { return (ValueLookupParameter<IOperator>)Parameters["Analyzer"]; }
    }
    public ValueLookupParameter<DoubleValue> SuccessRatioParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["SuccessRatio"]; }
    }
    public ValueLookupParameter<DoubleValue> ComparisonFactorLowerBoundParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["ComparisonFactorLowerBound"]; }
    }
    public ValueLookupParameter<DoubleValue> ComparisonFactorUpperBoundParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["ComparisonFactorUpperBound"]; }
    }
    public ValueLookupParameter<IOperator> ComparisonFactorModifierParameter {
      get { return (ValueLookupParameter<IOperator>)Parameters["ComparisonFactorModifier"]; }
    }
    public ValueLookupParameter<DoubleValue> MaximumSelectionPressureParameter {
      get { return (ValueLookupParameter<DoubleValue>)Parameters["MaximumSelectionPressure"]; }
    }
    public ValueLookupParameter<BoolValue> OffspringSelectionBeforeMutationParameter {
      get { return (ValueLookupParameter<BoolValue>)Parameters["OffspringSelectionBeforeMutation"]; }
    }
    #endregion

    [StorableConstructor]
    private OffspringSelectionGeneticAlgorithmMainLoop(bool deserializing) : base() { }
    public OffspringSelectionGeneticAlgorithmMainLoop()
      : base() {
      Initialize();
    }

    private void Initialize() {
      #region Create parameters
      Parameters.Add(new ValueLookupParameter<IRandom>("Random", "A pseudo random number generator."));
      Parameters.Add(new ValueLookupParameter<BoolValue>("Maximization", "True if the problem is a maximization problem, otherwise false."));
      Parameters.Add(new ScopeTreeLookupParameter<DoubleValue>("Quality", "The value which represents the quality of a solution."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("BestKnownQuality", "The best known quality value found so far."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Selector", "The operator used to select solutions for reproduction."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Crossover", "The operator used to cross solutions."));
      Parameters.Add(new ValueLookupParameter<PercentValue>("MutationProbability", "The probability that the mutation operator is applied on a solution."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Mutator", "The operator used to mutate solutions."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Evaluator", "The operator used to evaluate solutions."));
      Parameters.Add(new ValueLookupParameter<IntValue>("Elites", "The numer of elite solutions which are kept in each generation."));
      Parameters.Add(new ValueLookupParameter<IntValue>("MaximumGenerations", "The maximum number of generations which should be processed."));
      Parameters.Add(new ValueLookupParameter<VariableCollection>("Results", "The variable collection where results should be stored."));
      Parameters.Add(new ValueLookupParameter<IOperator>("Analyzer", "The operator used to analyze each generation."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("SuccessRatio", "The ratio of successful to total children that should be achieved."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("ComparisonFactorLowerBound", "The lower bound of the comparison factor (start)."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("ComparisonFactorUpperBound", "The upper bound of the comparison factor (end)."));
      Parameters.Add(new ValueLookupParameter<IOperator>("ComparisonFactorModifier", "The operator used to modify the comparison factor."));
      Parameters.Add(new ValueLookupParameter<DoubleValue>("MaximumSelectionPressure", "The maximum selection pressure that terminates the algorithm."));
      Parameters.Add(new ValueLookupParameter<BoolValue>("OffspringSelectionBeforeMutation", "True if the offspring selection step should be applied before mutation, false if it should be applied after mutation."));
      #endregion

      #region Create operators
      VariableCreator variableCreator = new VariableCreator();
      Placeholder comparisonFactorModifier = new Placeholder();
      Placeholder analyzer1 = new Placeholder();
      ResultsCollector resultsCollector1 = new ResultsCollector();
      OffspringSelectionGeneticAlgorithmMainOperator mainOperator = new OffspringSelectionGeneticAlgorithmMainOperator();
      IntCounter generationsCounter = new IntCounter();
      Comparator maxGenerationsComparator = new Comparator();
      Comparator maxSelectionPressureComparator = new Comparator();
      Placeholder analyzer2 = new Placeholder();
      ResultsCollector resultsCollector2 = new ResultsCollector();
      ConditionalBranch conditionalBranch1 = new ConditionalBranch();
      ConditionalBranch conditionalBranch2 = new ConditionalBranch();

      variableCreator.CollectedValues.Add(new ValueParameter<IntValue>("Generations", new IntValue(0)));
      variableCreator.CollectedValues.Add(new ValueParameter<IntValue>("EvaluatedSolutions", new IntValue(0)));
      variableCreator.CollectedValues.Add(new ValueParameter<DoubleValue>("SelectionPressure", new DoubleValue(0)));
      variableCreator.CollectedValues.Add(new ValueParameter<DoubleValue>("CurrentSuccessRatio", new DoubleValue(0)));

      comparisonFactorModifier.Name = "Modify ComparisonFactor (placeholder)";
      comparisonFactorModifier.OperatorParameter.ActualName = ComparisonFactorModifierParameter.Name;

      analyzer1.Name = "Analyzer (placeholder)";
      analyzer1.OperatorParameter.ActualName = AnalyzerParameter.Name;

      resultsCollector1.CollectedValues.Add(new LookupParameter<IntValue>("Generations"));
      resultsCollector1.CollectedValues.Add(new LookupParameter<IntValue>("Evaluated Solutions", null, "EvaluatedSolutions"));
      resultsCollector1.CollectedValues.Add(new LookupParameter<DoubleValue>("Curent Comparison Factor", null, "ComparisonFactor"));
      resultsCollector1.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Selection Pressure", null, "SelectionPressure"));
      resultsCollector1.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Success Ratio", null, "CurrentSuccessRatio"));
      resultsCollector1.ResultsParameter.ActualName = ResultsParameter.Name;

      mainOperator.ComparisonFactorParameter.ActualName = "ComparisonFactor";
      mainOperator.CrossoverParameter.ActualName = CrossoverParameter.Name;
      mainOperator.CurrentSuccessRatioParameter.ActualName = "CurrentSuccessRatio";
      mainOperator.ElitesParameter.ActualName = ElitesParameter.Name;
      mainOperator.EvaluatedSolutionsParameter.ActualName = "EvaluatedSolutions";
      mainOperator.EvaluatorParameter.ActualName = EvaluatorParameter.Name;
      mainOperator.MaximizationParameter.ActualName = MaximizationParameter.Name;
      mainOperator.MaximumSelectionPressureParameter.ActualName = MaximumSelectionPressureParameter.Name;
      mainOperator.MutationProbabilityParameter.ActualName = MutationProbabilityParameter.Name;
      mainOperator.MutatorParameter.ActualName = MutatorParameter.Name;
      mainOperator.OffspringSelectionBeforeMutationParameter.ActualName = OffspringSelectionBeforeMutationParameter.Name;
      mainOperator.QualityParameter.ActualName = QualityParameter.Name;
      mainOperator.RandomParameter.ActualName = RandomParameter.Name;
      mainOperator.SelectionPressureParameter.ActualName = "SelectionPressure";
      mainOperator.SelectorParameter.ActualName = SelectorParameter.Name;
      mainOperator.SuccessRatioParameter.ActualName = SuccessRatioParameter.Name;

      generationsCounter.Increment = new IntValue(1);
      generationsCounter.ValueParameter.ActualName = "Generations";

      maxGenerationsComparator.Comparison = new Comparison(ComparisonType.GreaterOrEqual);
      maxGenerationsComparator.LeftSideParameter.ActualName = "Generations";
      maxGenerationsComparator.ResultParameter.ActualName = "TerminateMaximumGenerations";
      maxGenerationsComparator.RightSideParameter.ActualName = MaximumGenerationsParameter.Name;

      maxSelectionPressureComparator.Comparison = new Comparison(ComparisonType.GreaterOrEqual);
      maxSelectionPressureComparator.LeftSideParameter.ActualName = "SelectionPressure";
      maxSelectionPressureComparator.ResultParameter.ActualName = "TerminateSelectionPressure";
      maxSelectionPressureComparator.RightSideParameter.ActualName = MaximumSelectionPressureParameter.Name;

      analyzer2.Name = "Analyzer (placeholder)";
      analyzer2.OperatorParameter.ActualName = AnalyzerParameter.Name;

      resultsCollector2.CollectedValues.Add(new LookupParameter<IntValue>("Generations"));
      resultsCollector2.CollectedValues.Add(new LookupParameter<IntValue>("Evaluated Solutions", null, "EvaluatedSolutions"));
      resultsCollector2.CollectedValues.Add(new LookupParameter<DoubleValue>("Curent Comparison Factor", null, "ComparisonFactor"));
      resultsCollector2.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Selection Pressure", null, "SelectionPressure"));
      resultsCollector2.CollectedValues.Add(new LookupParameter<DoubleValue>("Current Success Ratio", null, "CurrentSuccessRatio"));
      resultsCollector2.ResultsParameter.ActualName = ResultsParameter.Name;

      conditionalBranch1.Name = "MaximumSelectionPressure reached?";
      conditionalBranch1.ConditionParameter.ActualName = "TerminateSelectionPressure";

      conditionalBranch2.Name = "MaximumGenerations reached?";
      conditionalBranch2.ConditionParameter.ActualName = "TerminateMaximumGenerations";
      #endregion

      #region Create operator graph
      OperatorGraph.InitialOperator = variableCreator;
      variableCreator.Successor = comparisonFactorModifier;
      comparisonFactorModifier.Successor = analyzer1;
      analyzer1.Successor = resultsCollector1;
      resultsCollector1.Successor = mainOperator;
      mainOperator.Successor = generationsCounter;
      generationsCounter.Successor = maxGenerationsComparator;
      maxGenerationsComparator.Successor = maxSelectionPressureComparator;
      maxSelectionPressureComparator.Successor = analyzer2;
      analyzer2.Successor = resultsCollector2;
      resultsCollector2.Successor = conditionalBranch1;
      conditionalBranch1.FalseBranch = conditionalBranch2;
      conditionalBranch1.TrueBranch = null;
      conditionalBranch1.Successor = null;
      conditionalBranch2.FalseBranch = mainOperator;
      conditionalBranch2.TrueBranch = null;
      conditionalBranch2.Successor = null;
      #endregion
    }
  }
}
