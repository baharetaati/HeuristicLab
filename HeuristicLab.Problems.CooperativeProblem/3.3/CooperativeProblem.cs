using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Optimization;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Problems.Instances;
using HEAL.Attic;
using System.Runtime.InteropServices;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Problems.CooperativeProblem.Interfaces;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using HeuristicLab.Problems.Instances;
using HeuristicLab.Problems.Instances.DataAnalysis;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Regression;
using HeuristicLab.Random;
using System.Windows.Forms.DataVisualization.Charting;
using System.Collections.Concurrent;

namespace HeuristicLab.Problems.CooperativeProblem {
  [Item("Cooperative Problem", "Cooperative Problem including two problem types.")]
  [StorableType("78E1CBDE-76A7-4365-8DFB-AEF9A2A59963")]
  public class CooperativeProblem : MultiObjectiveBasicProblem<SymbolicExpressionTreeEncoding>, ICooperativeProblem, IProblemInstanceConsumer<IRegressionProblemData> {
    private const double PunishmentFactor = 10;
    private const double PercentageOfRows = 1.0;
    private const double ThersholdForFirstObjective = .1;
    private const double penaltyFactor = 0.0;
    private const int InitialMaximumTreeDepth = 8;
    private const int InitialMaximumTreeLength = 25;
    private const double MinNormalizedTreeLengthValue = 0.0;
    private const double MaxNormalizedTreeLengthValue = 1.0;
    private const double SymbolicExpressionTreeMinimumLength = 1.0;

    #region Parameter Properties
    public IValueParameter<IRegressionProblemData> ProblemDataParameter {
      get { return (IValueParameter<IRegressionProblemData>)Parameters["ProblemData"]; }
    }
    public IValueParameter<ISymbolicDataAnalysisGrammar> SymbolicExpressionTreeGrammarParameter {
      get { return (IValueParameter<ISymbolicDataAnalysisGrammar>)Parameters["SymbolicExpressionTreeGrammar"]; }
    }
    public IValueParameter<ISymbolicDataAnalysisExpressionTreeInterpreter> SymbolicExpressionTreeInterpreterParameter {
      get { return (IValueParameter<ISymbolicDataAnalysisExpressionTreeInterpreter>)Parameters["SymbolicExpressionTreeInterpreter"]; }
    }
    public IValueParameter<ISymbolicExpressionTreeCreator> SymbolicExpressionTreeCreatorParameter {
      get { return (IValueParameter<ISymbolicExpressionTreeCreator>)Parameters["SymbolicExpressionTreeCreator"]; }
    }
    public IFixedValueParameter<IntValue> NumObjectivesParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters["NumObjectives"]; }
    }
    public IFixedValueParameter<IntValue> NumConstraintsParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters["NumConstraints"]; }
    }
    public IValueParameter<BoolArray> MaximizationParameter {
      get { return (IValueParameter<BoolArray>)Parameters["Maximization"]; }
    }
    public IFixedValueParameter<DoubleLimit> EstimationLimitsParameter {
      get { return (IFixedValueParameter<DoubleLimit>)Parameters["EstimationLimits"]; }
    }
    public IFixedValueParameter<IntRange> TrainingPartitionParameter {
      get { return (IFixedValueParameter<IntRange>)Parameters["TrainingPartition"]; }
    }
    public IFixedValueParameter<IntValue> SymbolicExpressionTreeMaximumDepthParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters["SymbolicExpressionTreeMaximumDepth"]; }
    }
    public IFixedValueParameter<IntValue> SymbolicExpressionTreeMaximumLengthParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters["SymbolicExpressionTreeMaximumLength"]; }
    }
    public IValueParameter<DoubleArray> WeightsParameter {
      get { return (IValueParameter<DoubleArray>)Parameters["Weights"]; }
    }
    #endregion

    #region Properties
    public IRegressionProblemData ProblemData { get => ProblemDataParameter.Value; set => ProblemDataParameter.Value = value; }
    public ISymbolicDataAnalysisGrammar SymbolicExpressionTreeGrammar {
      get { return SymbolicExpressionTreeGrammarParameter.Value; }
      set { SymbolicExpressionTreeGrammarParameter.Value = value; }
    }
    public ISymbolicDataAnalysisExpressionTreeInterpreter SymbolicExpressionTreeInterpreter { get => SymbolicExpressionTreeInterpreterParameter.Value; set => SymbolicExpressionTreeInterpreterParameter.Value = value; }
    public ISymbolicExpressionTreeCreator SymbolicExpressionTreeCreator {
      get { return SymbolicExpressionTreeCreatorParameter.Value; }
      set { SymbolicExpressionTreeCreatorParameter.Value = value; }
    }
    public int NumObjectives { get => NumObjectivesParameter.Value.Value; set => NumObjectivesParameter.Value.Value = value; }
    public int NumConstraints { get => NumConstraintsParameter.Value.Value; set => NumConstraintsParameter.Value.Value = value; }
    public int SymbolicExpressionTreeMaximumDepth { get => SymbolicExpressionTreeMaximumDepthParameter.Value.Value; set => SymbolicExpressionTreeMaximumDepthParameter.Value.Value = value; }
    public int SymbolicExpressionTreeMaximumLength { get => SymbolicExpressionTreeMaximumLengthParameter.Value.Value; set => SymbolicExpressionTreeMaximumLengthParameter.Value.Value = value; }
    public override bool[] Maximization {
      get {
        if (!Parameters.ContainsKey("Maximization")) return new bool[] {false, false};
        return MaximizationParameter.Value.ToArray();
      }
    }
    public DoubleLimit EstimationLimits {
      get { return EstimationLimitsParameter.Value; }
    }
    public IntRange TrainingPartition {
      get { return TrainingPartitionParameter.Value; }
    }
    //public ProbabilisticTreeCreator Creator { get; private set; }
    public DoubleArray Weights {
      get { return WeightsParameter.Value; }
      set { WeightsParameter.Value = value; }
    }
    #endregion
    #region Storable Fields
    //[Storable]
    //private List<CooperativeProblemType> problemSet = new List<CooperativeProblemType>();
    //[Storable]
    //private DoubleArray Weights = new DoubleArray();
    #endregion
    [StorableConstructor]
    protected CooperativeProblem(StorableConstructorFlag _) : base(_) { }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEventHandlers();
    }

    protected CooperativeProblem(CooperativeProblem original, Cloner cloner)
      : base(original, cloner) {
      RegisterEventHandlers();
    }


    public override IDeepCloneable Clone(Cloner cloner) {
      return new CooperativeProblem(this, cloner);
    }
    public CooperativeProblem()
      : base() {
      Parameters.Add(new ValueParameter<IRegressionProblemData>("ProblemData", "A predefined symbolic regression dataset", new RegressionProblemData()));
      Parameters.Add(new ValueParameter<ISymbolicDataAnalysisGrammar>("SymbolicExpressionTreeGrammar", "The grammar that should be used for symbolic expression tree."));
      Parameters.Add(new ValueParameter<ISymbolicDataAnalysisExpressionTreeInterpreter>("SymbolicExpressionTreeInterpreter", "An interpreter for symbolic expression trees"));
      Parameters.Add(new ValueParameter<ISymbolicExpressionTreeCreator>("SymbolicExpressionTreeCreator", "A solution creator for symbolic expression trees"));
      Parameters.Add(new FixedValueParameter<IntValue>("NumObjectives", "Added to the dimensionality of the solution vector (number of objectives).", new IntValue(2)));
      Parameters.Add(new FixedValueParameter<IntValue>("NumConstraints", "Added to the dimensionality of the solution vector (number of constraints).", new IntValue(0)));
      Parameters.Add(new FixedValueParameter<IntValue>("SymbolicExpressionTreeMaximumDepth", "Maximum symbolic expression tree depth", new IntValue(8)));
      Parameters.Add(new FixedValueParameter<IntValue>("SymbolicExpressionTreeMaximumLength", "Maximum symbolic expression tree length", new IntValue(25)));
      Parameters.Add(new FixedValueParameter<DoubleLimit>("EstimationLimits", "The lower and upper limit for the estimated value that can be returned by the symbolic regression model."));
      Parameters.Add(new FixedValueParameter<IntRange>("TrainingPartition", "The partition of the problem data training partition, that should be used to calculate the fitness of an individual."));
      Parameters.Add(new ValueParameter<DoubleArray>("Weights", "The weight values for each objective in the weighted sum GA.", new DoubleArray(new[] { 0.9, 0.1 })));

      SymbolicExpressionTreeGrammar = new TypeCoherentExpressionGrammar();
      //SymbolicExpressionTreeGrammar = new LinearScalingGrammar();
      SymbolicExpressionTreeInterpreter = new SymbolicDataAnalysisExpressionTreeLinearInterpreter();
      SymbolicExpressionTreeCreator = new ProbabilisticTreeCreator();
      //Creator = new ProbabilisticTreeCreator();
      EstimationLimitsParameter.Hidden = true;
      SymbolicExpressionTreeInterpreterParameter.Hidden = true;
      TrainingPartition.Start = ProblemData.TrainingPartition.Start;
      TrainingPartition.End = ProblemData.TrainingPartition.End;

      SymbolicExpressionTreeMaximumDepthParameter.Value.Value = InitialMaximumTreeDepth;
      SymbolicExpressionTreeMaximumLengthParameter.Value.Value = InitialMaximumTreeLength;
      
      ConfigueGrammar();
      UpdateParameterValues();
      //UpdateGrammar();
      UpdateEstimationLimits();
      RegisterEventHandlers();
      
    }
    public sealed override double[] Evaluate(Individual individual, IRandom random) {
      return Evaluate(individual.SymbolicExpressionTree(), random);
    }
    public double EvaluateSingleObjectivePearsonRsquaredError(ISymbolicExpressionTree tree, IRandom random) {
      var rows = GenerateRowsToEvaluate(random);
      double r2 = SymbolicRegressionSingleObjectivePearsonRSquaredEvaluator.Calculate(
        tree, ProblemData, rows, SymbolicExpressionTreeInterpreter, true,
        EstimationLimits.Lower, EstimationLimits.Upper);
      return 1-r2;
    }
    public double EvaluateSingleObjectiveNormalizedMeanSquaredError(ISymbolicExpressionTree tree, IRandom random) {
      OnlineCalculatorError errorState;
      var rows = GenerateRowsToEvaluate(random);
      var estimatedValues = SymbolicExpressionTreeInterpreter.GetSymbolicExpressionTreeValues(tree, ProblemData.Dataset, rows);
      var targetValues = ProblemData.Dataset.GetDoubleValues(ProblemData.TargetVariable, rows);
      var boundedEstimatedValues = estimatedValues.LimitToRange(EstimationLimits.Lower, EstimationLimits.Upper);

      double nmse = OnlineNormalizedMeanSquaredErrorCalculator.Calculate(targetValues, boundedEstimatedValues, out errorState);
      if (errorState != OnlineCalculatorError.None) nmse = double.MaxValue;
      if (nmse > 1)
        nmse = 1.0;

      //if (nmse == 1.0) {
      //  Console.WriteLine("nmse == 1.0");
      //  foreach (var node in tree.IterateNodesBreadth()) {
      //    Console.WriteLine($"{node.Symbol.Name}");
      //  }
      //}
      return nmse;
    }
    public double EvaluateSingleObjectiveMeanSquaredError(ISymbolicExpressionTree tree, IRandom random) {
      var rows = GenerateRowsToEvaluate(random);
      double mse = SymbolicRegressionSingleObjectiveMeanSquaredErrorEvaluator.Calculate(tree, ProblemData, rows, SymbolicExpressionTreeInterpreter, true, EstimationLimits.Lower, EstimationLimits.Upper);

      return mse;

    }
    public double[] Evaluate(ISymbolicExpressionTree tree, IRandom random) {
      OnlineCalculatorError errorState;
      var rows = GenerateRowsToEvaluate(random);
      var estimatedValues = SymbolicExpressionTreeInterpreter.GetSymbolicExpressionTreeValues(tree, ProblemData.Dataset, rows);
      var targetValues = ProblemData.Dataset.GetDoubleValues(ProblemData.TargetVariable, rows);
      var boundedEstimatedValues = estimatedValues.LimitToRange(EstimationLimits.Lower, EstimationLimits.Upper);

      double nmse = OnlineNormalizedMeanSquaredErrorCalculator.Calculate(targetValues, boundedEstimatedValues, out errorState);
      if (errorState != OnlineCalculatorError.None) nmse = double.MaxValue;
      if (nmse > 1)
        nmse = random.NextDouble();
      return new double[] { nmse, tree.Length };
    }
    public double[] EvaluateUnconstrainedWeightedSumMeanSquaredError(ISymbolicExpressionTree tree, IRandom random) {
      var rows = GenerateRowsToEvaluate(random);
      double mse = SymbolicRegressionSingleObjectiveMeanSquaredErrorEvaluator.Calculate(tree, ProblemData, rows, SymbolicExpressionTreeInterpreter, true, EstimationLimits.Lower, EstimationLimits.Upper);
      List<double> objectives = new List<double>();
      objectives.Add(mse);
      double treeLength = tree.Length;
      objectives.Add(treeLength);
      double weightedSumValue = 0.0;
      for (int i = 0; i < objectives.Count; i++) {
        //Console.WriteLine($"Weights[{i}] = {Weights[i]}");
        weightedSumValue += Weights[i] * objectives[i];
      }
      objectives.Add(weightedSumValue);
      return objectives.ToArray();
    }
    public double[] EvaluateUnconstrainedWeightedSumMethod(ISymbolicExpressionTree tree, IRandom random) {
            
      OnlineCalculatorError errorState;
      var rows = GenerateRowsToEvaluate(random);
      var estimatedValues = SymbolicExpressionTreeInterpreter.GetSymbolicExpressionTreeValues(tree, ProblemData.Dataset, rows);
      var targetValues = ProblemData.Dataset.GetDoubleValues(ProblemData.TargetVariable, rows);
      var boundedEstimatedValues = estimatedValues.LimitToRange(EstimationLimits.Lower, EstimationLimits.Upper);

      double nmse = OnlineNormalizedMeanSquaredErrorCalculator.Calculate(targetValues, boundedEstimatedValues, out errorState);
      if (errorState != OnlineCalculatorError.None) nmse = double.MaxValue;
      if (nmse > 1)
        nmse = 1.0;

      double treeLength = tree.Length;

      List<double> objectives = new List<double>();
      objectives.Add(nmse);
      objectives.Add(treeLength);
      var normalizedTreeLength = (double)(treeLength - SymbolicExpressionTreeMinimumLength) / (SymbolicExpressionTreeMaximumLength - SymbolicExpressionTreeMinimumLength);

      // Weighted sum method
      //double weightedSumValue = 0.0;
      //for (int i = 0; i < objectives.Count; i++) {
      //  if (i == 1) {

      //    weightedSumValue += Weights[i] * normalizedTreeLength;
      //  }
      //  else
      //    weightedSumValue += Weights[i] * objectives[i];
      //}
      //objectives.Add(weightedSumValue);

      //Tchebycheff norm function
      var f1 = Weights[0] * Math.Abs(nmse);
      var f2 = Weights[1] * Math.Abs(normalizedTreeLength);

      objectives.Add(Math.Max(f1, f2));
      return objectives.ToArray();
    }
    public double[] EvaluateRandomlyWeightedSumMethod(ISymbolicExpressionTree tree, IRandom random) {
      OnlineCalculatorError errorState;
      var rows = GenerateRowsToEvaluate(random);
      var estimatedValues = SymbolicExpressionTreeInterpreter.GetSymbolicExpressionTreeValues(tree, ProblemData.Dataset, rows);
      var targetValues = ProblemData.Dataset.GetDoubleValues(ProblemData.TargetVariable, rows);
      var boundedEstimatedValues = estimatedValues.LimitToRange(EstimationLimits.Lower, EstimationLimits.Upper);

      double nmse = OnlineNormalizedMeanSquaredErrorCalculator.Calculate(targetValues, boundedEstimatedValues, out errorState);
      if (errorState != OnlineCalculatorError.None) nmse = double.MaxValue;
      if (nmse > 1)
        nmse = 1.0;

      double treeLength = tree.Length;

      List<double> objectives = new List<double>();
      objectives.Add(nmse);
      objectives.Add(treeLength);
      double weightedSumValue = 0.0;
      double maxTreeLength = SymbolicExpressionTreeMaximumLength;
      var randWeights = GenerateWeightsRandomly(random);
      for (int i = 0; i < objectives.Count; i++) {
        if (i == 1) {
          double normalizedTreeLength = (treeLength - MinNormalizedTreeLengthValue) / (maxTreeLength - MinNormalizedTreeLengthValue) * (MaxNormalizedTreeLengthValue - MinNormalizedTreeLengthValue) + MinNormalizedTreeLengthValue;
          weightedSumValue += randWeights[i] * normalizedTreeLength;
        } else
          weightedSumValue += randWeights[i] * objectives[i];
      }
      objectives.Add(weightedSumValue);
      return objectives.ToArray();
    }
    public double[] EvaluateUnConstrainedWeightedSumPearsonRsquaredError(ISymbolicExpressionTree tree, IRandom random) {
      var rows = GenerateRowsToEvaluate(random);
      
      double r2 = SymbolicRegressionSingleObjectivePearsonRSquaredEvaluator.Calculate(
        tree, ProblemData, rows, SymbolicExpressionTreeInterpreter, true,
        EstimationLimits.Lower, EstimationLimits.Upper);

      //if (r2 < ThersholdForFirstObjective) {
      //  penaltyFactor = 10;
      //}
      List<double> objectives = new List<double>();
      objectives.Add(1-r2);
      var treeLength = tree.Length;
      objectives.Add(treeLength);
      var normalizedTreeLength = (double)(treeLength - SymbolicExpressionTreeMinimumLength) / (SymbolicExpressionTreeMaximumLength - SymbolicExpressionTreeMinimumLength);
      //double weightedSumValue = 0.0;
      //for (int i = 0; i < objectives.Count; i++) {
      //  if (i == 0) {
      //    weightedSumValue += Weights[i] * objectives[i];
      //  } else {

      //    weightedSumValue += Weights[i] * (1-normalizedTreeLength);
      //  }
      //}
      //objectives.Add(weightedSumValue + penaltyFactor);

      //Tchebycheff norm function
      var f1 = Weights[0] * Math.Abs(1-r2);
      var f2 = Weights[1] * Math.Abs(normalizedTreeLength);
      Console.WriteLine($"Weights = [{Weights[0]}, {Weights[1]}]");
      objectives.Add(Math.Max(f1, f2));
      return objectives.ToArray();
    }

    public double[] EvaluateUnconstrainedMultiObjectiveProblem(ISymbolicExpressionTree tree, IRandom random) {
      OnlineCalculatorError errorState;

      var rows = GenerateRowsToEvaluate(random);
      var estimatedValues = SymbolicExpressionTreeInterpreter.GetSymbolicExpressionTreeValues(tree, ProblemData.Dataset, rows);
      
      var targetValues = ProblemData.Dataset.GetDoubleValues(ProblemData.TargetVariable, rows);
      var boundedEstimatedValues = estimatedValues.LimitToRange(EstimationLimits.Lower, EstimationLimits.Upper);

      double nmse = OnlineNormalizedMeanSquaredErrorCalculator.Calculate(targetValues, boundedEstimatedValues, out errorState);
      
      if (errorState != OnlineCalculatorError.None) nmse = double.MaxValue;

      if (nmse > 1)
        nmse = 1.0;
      
      return new double[] {nmse, tree.Length};
    }
    public double[] EvaluateMultiObjectiveWithComplexity(ISymbolicExpressionTree tree, IRandom random) {
      OnlineCalculatorError errorState;

      var rows = GenerateRowsToEvaluate(random);
      var estimatedValues = SymbolicExpressionTreeInterpreter.GetSymbolicExpressionTreeValues(tree, ProblemData.Dataset, rows);

      var targetValues = ProblemData.Dataset.GetDoubleValues(ProblemData.TargetVariable, rows);
      var boundedEstimatedValues = estimatedValues.LimitToRange(EstimationLimits.Lower, EstimationLimits.Upper);

      double nmse = OnlineNormalizedMeanSquaredErrorCalculator.Calculate(targetValues, boundedEstimatedValues, out errorState);

      if (errorState != OnlineCalculatorError.None) nmse = double.MaxValue;

      if (nmse > 1)
        nmse = 1.0;

    
      return new double[] { nmse, SymbolicDataAnalysisModelComplexityCalculator.CalculateComplexity(tree) };
    }
    public double[] EvaluateMultiObjectiveMeanSquaredError(ISymbolicExpressionTree tree, IRandom random) {
      //OnlineCalculatorError errorState;
      var rows = GenerateRowsToEvaluate(random);
      double mse = SymbolicRegressionSingleObjectiveMeanSquaredErrorEvaluator.Calculate(tree, ProblemData, rows, SymbolicExpressionTreeInterpreter, true, EstimationLimits.Lower, EstimationLimits.Upper);
      //var estimatedValues = SymbolicExpressionTreeInterpreter.GetSymbolicExpressionTreeValues(tree, ProblemData.Dataset, rows);
      //var targetValues = ProblemData.Dataset.GetDoubleValues(ProblemData.TargetVariable, rows);
      //var boundedEstimatedValues = estimatedValues.LimitToRange(EstimationLimits.Lower, EstimationLimits.Upper);

      //double mse = OnlineMeanSquaredErrorCalculator.Calculate(targetValues, boundedEstimatedValues, out errorState);
      //if (errorState != OnlineCalculatorError.None) mse = double.MaxValue;
      

      return new double[] {mse, tree.Length};

    }
    public double[] EvaluateMultiObjectivePearsonRsquaredError(ISymbolicExpressionTree tree, IRandom random) {
      var rows = GenerateRowsToEvaluate(random);
      double r2 = SymbolicRegressionSingleObjectivePearsonRSquaredEvaluator.Calculate(
        tree, ProblemData, rows, SymbolicExpressionTreeInterpreter, true,
        EstimationLimits.Lower, EstimationLimits.Upper);

      return new double[2] { r2, tree.Length };
    }
    public void UpdateWeights(double w1, double w2) {
      Weights = new DoubleArray(new[] { w1, w2 });
    }
    private void ConfigueGrammar() {
      var grammar = SymbolicExpressionTreeGrammar as TypeCoherentExpressionGrammar;
      //var grammar = SymbolicExpressionTreeGrammar as LinearScalingGrammar;
      if (grammar != null) grammar.ConfigureAsDefaultRegressionGrammar();
      SymbolicExpressionTreeGrammar.ConfigureVariableSymbols(ProblemData);
    }
    private void UpdateGrammar() {
      var problemData = ProblemData;
      var grammar = SymbolicExpressionTreeGrammar;

      //grammar.MaximumFunctionArguments = MaximumFunctionArguments.Value;
      //grammar.MaximumFunctionDefinitions = MaximumFunctionDefinitions.Value;

      grammar.ConfigureVariableSymbols(problemData);
    }
    private void UpdateParameterValues() {
      //MaximizationParameter.Value = (BoolArray)new BoolArray(UpdateMaximization()).AsReadOnly();
      //UpdateWeights();
    }
    private void UpdateEstimationLimits() {
      if (ProblemData.TrainingIndices.Any()) {
        var targetValues = ProblemData.Dataset.GetDoubleValues(ProblemData.TargetVariable, ProblemData.TrainingIndices).ToList();
        var mean = targetValues.Average();
        var range = targetValues.Max() - targetValues.Min();
        EstimationLimits.Upper = mean + PunishmentFactor * range;
        EstimationLimits.Lower = mean - PunishmentFactor * range;
        
      } else {
        
        EstimationLimits.Upper = double.MaxValue;
        EstimationLimits.Lower = double.MinValue;
      }
    }
    private bool[] UpdateMaximization() {
      return new bool[NumObjectives + NumConstraints];
    }
    protected IEnumerable<int> GenerateRowsToEvaluate(IRandom random) {
      IEnumerable<int> rows;
      int samplesStart = ProblemData.TrainingPartition.Start;
      int samplesEnd = ProblemData.TrainingPartition.End;
      int testPartitionStart = ProblemData.TestPartition.Start;
      int testPartitionEnd = ProblemData.TestPartition.End;
      if (samplesEnd < samplesStart) throw new ArgumentException("Start value is larger than end value.");

      if (PercentageOfRows.IsAlmost(1.0)) {
        rows = Enumerable.Range(samplesStart, samplesEnd - samplesStart);
      } else {
        int seed = random.Next();
        int count = (int)((samplesEnd - samplesStart) * PercentageOfRows);
        if (count == 0) count = 1;

        rows = RandomEnumerable.SampleRandomNumbers(seed, samplesStart, samplesEnd, count);
      }

      rows = rows.Where(i => i < testPartitionStart || testPartitionEnd <= i);
      //if (ValidRowIndicatorParameter.ActualValue != null) {
      //  string indicatorVar = ValidRowIndicatorParameter.ActualValue.Value;
      //  var problemData = ProblemDataParameter.ActualValue;
      //  var indicatorRow = problemData.Dataset.GetReadOnlyDoubleValues(indicatorVar);
      //  rows = rows.Where(r => !indicatorRow[r].IsAlmost(0.0));
      //}
      return rows;
    }
    private double[] GenerateWeightsRandomly(IRandom random) {
      var firstRandWeight = random.NextDouble();
      var secondRandWeight = 1 - firstRandWeight;
      //model accuracy weight should be equal or higher
      if (firstRandWeight < secondRandWeight) {
        var tempWeight = firstRandWeight;
        firstRandWeight = secondRandWeight;
        secondRandWeight = tempWeight;
      }
      var randWeights = new double[] {firstRandWeight, secondRandWeight};
      return randWeights;
    }
    #region Events
    public void RegisterEventHandlers() {
      NumObjectivesParameter.Value.ValueChanged += NumObjectivesOnValueChanged;
      ProblemDataParameter.ValueChanged += new EventHandler(ProblemDataParameter_ValueChanged);
      ProblemDataParameter.Value.Changed += (object sender, EventArgs e) => OnProblemDataChanged();
    }
    public event EventHandler ProblemDataChanged;
    protected virtual void OnProblemDataChanged() {
      TrainingPartition.Start = ProblemData.TrainingPartition.Start;
      TrainingPartition.End = ProblemData.TrainingPartition.End;

      UpdateGrammar();
      UpdateEstimationLimits();

      var handler = ProblemDataChanged;
      if (handler != null) handler(this, EventArgs.Empty);

      OnReset();
    }
    private void ProblemDataParameter_ValueChanged(object sender, EventArgs e) {
      
      ProblemDataParameter.Value.Changed += (object s, EventArgs args) => OnProblemDataChanged();
      OnProblemDataChanged();
    }
    protected virtual void NumObjectivesOnValueChanged(object sender, EventArgs e) { }
    public void Load(IRegressionProblemData data) {
      ProblemData = data;
    }
    #endregion
  }
}
