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
  [StorableType("A73F5D69-BCA6-46A5-95C4-AF57936DFAA1")]
  public enum OffspringParentsComparisonTypes {
    DominationBaseComparison,
    RandomIndexComparison,
    AccuracyBasedComparison,
    WeightedBasedComparison,
    EpsilonLexicaseBasedComparison
  }
  public abstract class CooperativeApproachBase : BasicAlgorithm{
    #region Problem Properties
    public override Type ProblemType {
      get { return typeof(CooperativeProblem); }
    }
    public new CooperativeProblem Problem {
      get { return (CooperativeProblem)base.Problem; }
      set { base.Problem = value; }
    }
    #endregion
    public override bool SupportsPause {
      get { return true; }
    }
    #region Parameter Properties
    // Common Parameters
    private IFixedValueParameter<IntValue> SeedParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters["Seed"]; }
    }
    private IFixedValueParameter<BoolValue> SetSeedRandomlyParameter {
      get { return (IFixedValueParameter<BoolValue>)Parameters["SetSeedRandomly"]; }
    }
    private IFixedValueParameter<IntValue> MaxTreeLengthParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters["MaxTreeLength"]; }
    }
    private IFixedValueParameter<IntValue> MaxTreeDepthParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters["MaxTreeDepth"]; }
    }
    private IFixedValueParameter<PercentValue> MutationProbabilityParameter {
      get { return (IFixedValueParameter<PercentValue>)Parameters["MutationProbability"]; }
    }
    private IFixedValueParameter<PercentValue> CrossoverProbabilityParameter {
      get { return (IFixedValueParameter<PercentValue>)Parameters["CrossoverProbability"]; }
    }
    public IFixedValueParameter<IntValue> MaximumRuntimeParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters["Maximum Runtime"]; }
    }
    public IFixedValueParameter<BoolValue> RunSeparatelyParameter {
      get { return (IFixedValueParameter<BoolValue>)Parameters["Run Two Algorithms Separately"]; }
    }
    // Algorithm 1 Parameters
    private IFixedValueParameter<IntValue> Alg1PopulationSizeParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters["Alg1PopulationSize"]; }
    }
    private IFixedValueParameter<IntValue> ElitesAlg1Parameter {
      get { return (IFixedValueParameter<IntValue>)Parameters["ElitesAlg1"]; }
    }
    private IValueParameter<IntValue> Alg1MaximumGenerationsParameter {
      get { return (IValueParameter<IntValue>)Parameters["Alg1MaximumGenerations"]; }
    }
    public IFixedValueParameter<IntValue> Alg1MaximumEvaluatedSolutionsParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters["Alg1MaximumEvaluatedSolutions"]; }
    }
    public IConstrainedValueParameter<ISelectionStrategy<double>> Alg1SelectorParameter {
      get { return (IConstrainedValueParameter<ISelectionStrategy<double>>)Parameters["Alg1Selector"]; }
    }
    public IFixedValueParameter<BoolValue> ActiveOffspringSelectorParameter {
      get { return (IFixedValueParameter<BoolValue>)Parameters["ActiveOffspringSelector"]; }
    }
    public IFixedValueParameter<DoubleValue> MaximumSelectionPressureParameter {
      get { return (IFixedValueParameter<DoubleValue>)Parameters["MaximumSelectionPressure"]; }
    }
    public IFixedValueParameter<DoubleValue> SuccessRatioParameter {
      get { return (IFixedValueParameter<DoubleValue>)Parameters["SuccessRatio"]; }
    }
    public IFixedValueParameter<DoubleValue> ComparisonFactorParameter {
      get { return (IFixedValueParameter<DoubleValue>)Parameters["ComparisonFactor"]; }
    }
    // Algorithm 2 Parameters
    private IFixedValueParameter<IntValue> Alg2PopulationSizeParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters["Alg2PopulationSize"]; }
    }
    private IFixedValueParameter<IntValue> Alg2MaximumGenerationsParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters["Alg2MaximumGenerations"]; }
    }
    public IFixedValueParameter<IntValue> Alg2MaximumEvaluatedSolutionsParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters["Alg2MaximumEvaluatedSolutions"]; }
    }
    public IFixedValueParameter<IntValue> GenerationsForAlg2ExecutionParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters["Generations for Excution of Algorithm 2"]; }
    }
    #endregion

    #region Properties
    public int Seed {
      get { return SeedParameter.Value.Value; }
      set { SeedParameter.Value.Value = value; }
    }
    public bool SetSeedRandomly {
      get { return SetSeedRandomlyParameter.Value.Value; }
      set { SetSeedRandomlyParameter.Value.Value = value; }
    }
    public int MaxTreeLength {
      get { return MaxTreeLengthParameter.Value.Value; }
      set { MaxTreeLengthParameter.Value.Value = value; }
    }
    public int MaxTreeDepth {
      get { return MaxTreeDepthParameter.Value.Value; }
      set { MaxTreeDepthParameter.Value.Value = value; }
    }
    public double MutationProbability {
      get { return MutationProbabilityParameter.Value.Value; }
      set { MutationProbabilityParameter.Value.Value = value; }
    }
    public double CrossoverProbability {
      get { return CrossoverProbabilityParameter.Value.Value; }
      set { CrossoverProbabilityParameter.Value.Value = value; }
    }
    public int MaximumRuntime {
      get { return MaximumRuntimeParameter.Value.Value; }
      set { MaximumRuntimeParameter.Value.Value = value; }
    }
    // Algorithm 1 Parameters
    public int Alg1PopulationSize {
      get { return Alg1PopulationSizeParameter.Value.Value; }
      set { Alg1PopulationSizeParameter.Value.Value = value; }
    }
    public int Alg1MaximumGenerations {
      get { return Alg1MaximumGenerationsParameter.Value.Value; }
      set { Alg1MaximumGenerationsParameter.Value.Value = value; }
    }
    public int Alg1MaximumEvaluatedSolutions {
      get { return Alg1MaximumEvaluatedSolutionsParameter.Value.Value; }
      set { Alg1MaximumEvaluatedSolutionsParameter.Value.Value = value; }
    }
    public int ElitesAlg1 {
      get { return ElitesAlg1Parameter.Value.Value; }
      set { ElitesAlg1Parameter.Value.Value = value; }
    }
    public ISelectionStrategy<double> Alg1Selector {
      get { return Alg1SelectorParameter.Value; }
      set { Alg1SelectorParameter.Value = value; }
    }
    public bool RunSeparately {
      get { return RunSeparatelyParameter.Value.Value; }
      set { RunSeparatelyParameter.Value.Value = value; }
    }
    public bool ActiveOffspringSelector {
      get { return ActiveOffspringSelectorParameter.Value.Value; }
      set { ActiveOffspringSelectorParameter.Value.Value = value; }
    }
    public double MaximumSelectionPressure {
      get { return MaximumSelectionPressureParameter.Value.Value; }
      set { MaximumSelectionPressureParameter.Value.Value = value; }
    }
    public double SuccessRatio {
      get { return SuccessRatioParameter.Value.Value; }
      set { SuccessRatioParameter.Value.Value = value; }
    }
    public double ComparisonFactor {
      get { return ComparisonFactorParameter.Value.Value; }
      set { ComparisonFactorParameter.Value.Value = value; }
    }
    // Algorithm 2 Parameters
    public int Alg2PopulationSize {
      get { return Alg2PopulationSizeParameter.Value.Value; }
      set { Alg2PopulationSizeParameter.Value.Value = value; }
    }
    public int Alg2MaximumGenerations {
      get { return Alg2MaximumGenerationsParameter.Value.Value; }
      set { Alg2MaximumGenerationsParameter.Value.Value = value; }
    }
    public int Alg2MaximumEvaluatedSolutions {
      get { return Alg2MaximumEvaluatedSolutionsParameter.Value.Value; }
      set { Alg2MaximumEvaluatedSolutionsParameter.Value.Value = value; }
    }
    public int GenerationsForAlg2Execution {
      get { return GenerationsForAlg2ExecutionParameter.Value.Value; }
      set { GenerationsForAlg2ExecutionParameter.Value.Value = value; }
    }
    #endregion

    #region Results Properties
    // Algorithm 1 results
    public int ResultsAlg1Iterations {
      get { return ((IntValue)Results["Alg1Generations"].Value).Value; }
      set { ((IntValue)Results["Alg1Generations"].Value).Value = value; }
    }
    public int ResultsAlg1Evaluations {
      get { return ((IntValue)Results["Alg1Evaluations"].Value).Value; }
      set { ((IntValue)Results["Alg1Evaluations"].Value).Value = value; }
    }
    public ISymbolicRegressionSolution ResultsBestSolutionAlg1 {
      get { return (ISymbolicRegressionSolution)Results["Best Solution for Algorithm1"].Value; }
      set { Results["Best Solution for Algorithm1"].Value = value; }
    }
    //public DoubleArray ResultsBestQualityAlg1 {
    //  get { return (DoubleArray)Results["Best Quality for DecompositionBasedGA"].Value; }
    //  set { Results["Best Quality for DecompositionBasedGA"].Value = new DoubleArray(value.ToArray()); }
    //}
    public double ResultsBestQualityAlg1 {
      get { return ((DoubleValue)Results["Best Quality for Algorithm1"].Value).Value; }
      set { ((DoubleValue)Results["Best Quality for Algorithm1"].Value).Value = value; }
    }
    public double ResultsWorstQualityAlg1 {
      get { return ((DoubleValue)Results["Worst Quality for Algorithm1"].Value).Value; }
      set { ((DoubleValue)Results["Worst Quality for Algorithm1"].Value).Value = value; }
    }
    public double ResultsAvgQualityAlg1 {
      get { return ((DoubleValue)Results["Avg Quality for Algorithm1"].Value).Value; }
      set { ((DoubleValue)Results["Avg Quality for Algorithm1"].Value).Value = value; }
    }
    public double ResultsActivePressure {
      get { return ((DoubleValue)Results["Active Selection Pressure"].Value).Value; }
      set { ((DoubleValue)Results["Active Selection Pressure"].Value).Value = value; }
    }
    public int ResultsCountSuccessfulOffspring {
      get { return ((IntValue)Results["CountSuccessfulOffspring"].Value).Value; }
      set { ((IntValue)Results["CountSuccessfulOffspring"].Value).Value = value; }
    }
    public int ResultsCountUnsuccessfulOffspring {
      get { return ((IntValue)Results["CountUnsuccessfulOffspring"].Value).Value; }
      set { ((IntValue)Results["CountUnsuccessfulOffspring"].Value).Value = value; }
    }
    public DoubleMatrix ResultsQualitiesAlg1 {
      get { return (DoubleMatrix)Results["Algorithm1 Qualities"].Value; }
      set { Results["Algorithm1 Qualities"].Value = value; }
    }
    //public DoubleMatrix ResultsElitesAlg1 {
    //  get { return (DoubleMatrix)Results["Algorithm1 Elites"].Value; }
    //  set { Results["Algorithm1 Elites"].Value = value; }
    //}
    public DataTable ResultsQualitiesAlg1Table {
      get { return ((DataTable)Results["Algorithm1 Timetable Qualities"].Value); }
    }
    public DataRow ResultsQualitiesAlg1Best {
      get { return ResultsQualitiesAlg1Table.Rows["Best Quality"]; }
    }
    public DataRow ResultsQualitiesAlg1Worst {
      get { return ResultsQualitiesAlg1Table.Rows["Worst Quality"]; }
    }
    public DataRow ResultsQualitiesAlg1Average {
      get { return ResultsQualitiesAlg1Table.Rows["Average Quality"]; }
    }
    //public IndexedDataTable<double> ResultsGAQualitiesPerEvaluation {
    //  get { return ((IndexedDataTable<double>)Results["DecompositionBasedGA Qualities Per Evaluation"].Value); }
    //}
    
    // NSGA2 results
    public int ResultsAlg2Iterations {
      get { return ((IntValue)Results["Alg2Generations"].Value).Value; }
      set { ((IntValue)Results["Alg2Generations"].Value).Value = value; }
    }
    public int ResultsAlg2Evaluations {
      get { return ((IntValue)Results["Alg2Evaluations"].Value).Value; }
      set { ((IntValue)Results["Alg2Evaluations"].Value).Value = value; }
    }
    //public ISymbolicRegressionSolution ResultsChosenSolutionAlg2 {
    //  get { return (ISymbolicRegressionSolution)Results["Best Solution for Algorithm2"].Value; }
    //  set { Results["Best Solution for Algorithm2"].Value = value; }
    //}
    public ItemList<ISymbolicRegressionSolution> ResultsNonDominatedSolutionsAlg2 {
      get { return (ItemList<ISymbolicRegressionSolution>)Results["Non Dominated Solution for Algorithm2"].Value; }
      set { Results["Non Dominated Solution for Algorithm2"].Value = value; }
    }
    public ItemList<DoubleArray> ResultsBestQualitiesAlg2 {
      get { return (ItemList<DoubleArray>)Results["Best Qualities for Algorithm2"].Value; }
      set { Results["Best Qualities for Algorithm2"].Value = value; }
    }
    //public DoubleArray ResultsBestQualityAlg2 {
    //  get { return (DoubleArray)Results["Best Quality for Algorithm2"].Value; }
    //  set { Results["Best Quality for Algorithm2"].Value = new DoubleArray(value.ToArray()); }
    //}
    public DoubleMatrix ResultsParetoFrontAlg2 {
      get { return (DoubleMatrix)Results["Algorithm2 Pareto Front"].Value; }
      set { Results["Algorithm2 Pareto Front"].Value = value; }
    }
    
    public ScatterPlot ResultsScatterPlot {
      get { return (ScatterPlot)Results["Pareto Front Analysis"].Value; }
      set { Results["Pareto Front Analysis"].Value = value; }
    }
   
    #endregion
    #region Storable fields
    [Storable]
    public IRandom random = new MersenneTwister();
    [Storable]
    public DecompositionBasedGA alg1;
    [Storable]
    public int numSelectedIndividualsGA;
    [Storable]
    public int numSelectedIndividualNSGA;
    [Storable]
    public int alg1QualityLength;
    [Storable]
    public TreeRequirements treeRequirements;
    [Storable]
    public NSGA2 alg2;
    #endregion
    [StorableConstructor]
    protected CooperativeApproachBase(StorableConstructorFlag _) : base(_) { }

    protected CooperativeApproachBase(CooperativeApproachBase original, Cloner cloner)
      : base(original, cloner) {
      random = cloner.Clone(original.random);
      alg1 = cloner.Clone(original.alg1);
      alg2 = cloner.Clone(original.alg2);
      treeRequirements = cloner.Clone(original.treeRequirements);
      numSelectedIndividualsGA = original.numSelectedIndividualsGA;
      numSelectedIndividualNSGA = original.numSelectedIndividualNSGA;
    }
    public CooperativeApproachBase() {
      Parameters.Add(new FixedValueParameter<IntValue>("Seed", "The random seed used to initialize the new pseudo random number generator.", new IntValue(0)));
      Parameters.Add(new FixedValueParameter<BoolValue>("SetSeedRandomly", "True if the random seed should be set to a random value, otherwise false.", new BoolValue(true)));
      Parameters.Add(new FixedValueParameter<IntValue>("Alg1PopulationSize", "The size of the population of solutions.", new IntValue(500)));
      Parameters.Add(new FixedValueParameter<IntValue>("Alg2PopulationSize", "The size of the population of solutions.", new IntValue(500)));
      Parameters.Add(new FixedValueParameter<IntValue>("Alg1MaximumGenerations", "The maximum number of generations which should be processed.", new IntValue(1000)));
      Parameters.Add(new FixedValueParameter<IntValue>("Alg2MaximumGenerations", "The maximum number of generations which should be processed.", new IntValue(1000)));
      Parameters.Add(new FixedValueParameter<IntValue>("Alg1MaximumEvaluatedSolutions", "The maximum number of solutions which should be evaluated.", new IntValue(500000)));
      Parameters.Add(new FixedValueParameter<IntValue>("Alg2MaximumEvaluatedSolutions", "The maximum number of solutions which should be evaluated.", new IntValue(500000)));
      Parameters.Add(new FixedValueParameter<IntValue>("Maximum Runtime", "The maximum runtime in seconds after which the algorithm stops. Use -1 to specify no limit for the runtime", new IntValue(3600)));
      Parameters.Add(new FixedValueParameter<IntValue>("MaxTreeLength", "The maximum tree length for expression trees.", new IntValue(25)));
      Parameters.Add(new FixedValueParameter<IntValue>("MaxTreeDepth", "The maximum tree depth for expression trees.", new IntValue(8)));
      Parameters.Add(new FixedValueParameter<PercentValue>("MutationProbability", "The probability that the mutation operator is applied on a solution.", new PercentValue(0.25)));
      Parameters.Add(new FixedValueParameter<IntValue>("ElitesAlg1", "The numer of elite solutions which are kept in each generation.", new IntValue(1)));
      Parameters.Add(new FixedValueParameter<IntValue>("Migration Interval from DecompositionBasedGA to Algorithm2", "Migration interval from DecompositionBasedGA to Algorithm2.", new IntValue(100)));
      Parameters.Add(new FixedValueParameter<IntValue>("Generations for Excution of Algorithm 2", "Generation numbers Algorithm2 runs", new IntValue(20)));
      
      Parameters.Add(new FixedValueParameter<DoubleValue>("MaximumSelectionPressure", "The maximum selection pressure which prematurely terminates the offspring selection step.", new DoubleValue(1.0)));
      Parameters.Add(new FixedValueParameter<DoubleValue>("SuccessRatio", "The ratio of successful offspring that has to be produced.", new DoubleValue(1.0)));
      Parameters.Add(new FixedValueParameter<DoubleValue>("ComparisonFactor", "A factor to compare offspring with parents", new DoubleValue(1.0)));

      Alg1MaximumGenerationsParameter.Hidden = true;
      Alg2MaximumGenerationsParameter.Hidden = true;

      var set = new ItemSet<ISelectionStrategy<double>> { new ProportionalSelection(), new TournamentSelection(), new TournamentSelection3(), new TournamentSelection4(), new TournamentSelection5(), new TournamentSelection6(), new TournamentSelection7(), new PercentageTournamentSelection(), new GeneralizedRankSelection(), new GeneralizedRankSelection3(), new GeneralizedRankSelection4(), new GeneralizedRankSelection5(), new EpsilonLexicaseSelection(.5), new AdaptiveEpsilonLexicaseSelection() };
      Parameters.Add(new ConstrainedValueParameter<ISelectionStrategy<double>>("Alg1Selector", "The selection mechanism for DecompositionBasedGA", set, set.First()));
    }
    public virtual void InitResults() {
      var problem = Problem as CooperativeProblem;
      if (problem == null) return;
      Results.Add(new Result("Alg1Generations", "The number of gererations evaluated", new IntValue(0)));
      Results.Add(new Result("Alg1Evaluations", "The number of function evaluations performed", new IntValue(0)));
      Results.Add(new Result("Best Solution for Algorithm1", "The best so-far solution found", (IItem)null));
      Results.Add(new Result("Best Quality for Algorithm1", "Current best quality", new DoubleValue(0.0)));
      Results.Add(new Result("Worst Quality for Algorithm1", "Current worst quality for Algorithm1", new DoubleValue(0.0)));
      Results.Add(new Result("Avg Quality for Algorithm1", "Current average quality", new DoubleValue(0.0)));
      Results.Add(new Result("Algorithm1 Qualities", "Current Algorithm1 Qualities matrix", new DoubleMatrix()));
      Results.Add(new Result("Algorithm1 Elites", "Current Algorithm1 Elites matrix", new DoubleMatrix()));
      Results.Add(new Result("Active Selection Pressure", "Active selection pressure for offspring selection", new DoubleValue(0.0)));
      Results.Add(new Result("CountSuccessfulOffspring", "The number of successful offspring in offspring selection", new IntValue(0)));
      Results.Add(new Result("CountUnsuccessfulOffspring", "The number of unsuccessful offspring in offspring selection", new IntValue(0)));

      Results.Add(new Result("Alg2Generations", "The number of gererations evaluated", new IntValue(0)));
      Results.Add(new Result("Alg2Evaluations", "The number of function evaluations performed", new IntValue(0)));
      Results.Add(new Result("Non Dominated Solution for Algorithm2", "The non-dominated solutions for NSGA2", new ItemList<ISymbolicRegressionSolution>()));
      Results.Add(new Result("Best Qualities for Algorithm2", "The best qualities found by NSGA2", new ItemList<DoubleArray>()));
      //Results.Add(new Result("Best Solution for NSGA2", "The best so-far solution found by NSGA2", (IItem)null));
      //Results.Add(new Result("Best Quality for NSGA2", "The best so-far quality found by NSGA2", new DoubleArray()));
      
      Results.Add(new Result("Algorithm2 Pareto Front", "Current NSGA2 pareto front", new DoubleMatrix()));

      var table = new DataTable("Algorithm1 Timetable Qualities");
      table.Rows.Add(new DataRow("Best Quality"));
      table.Rows.Add(new DataRow("Worst Quality"));
      var avgQualityRows = new DataRow("Average Quality"); // Best quality after the elite
      avgQualityRows.VisualProperties.LineStyle = DataRowVisualProperties.DataRowLineStyle.Dot;

      table.Rows.Add(avgQualityRows);

      Results.Add(new Result("Algorithm1 Timetable Qualities", table));

      var numColumns = problem.NumObjectives;

      ResultsParetoFrontAlg2 = new DoubleMatrix(Alg2PopulationSize, numColumns);
      //ResultsQualitiesAlg1 = new DoubleMatrix(Alg1PopulationSize, numColumns + 1);
      ResultsQualitiesAlg1 = new DoubleMatrix(Alg1PopulationSize, numColumns + 1);
      //ResultsElitesAlg1 = new DoubleMatrix(11, numColumns + 1);

      ScatterPlot scatterPlot = new ScatterPlot("Quality vs Tree Size", "");
      scatterPlot.VisualProperties.XAxisTitle = "Tree Size";
      scatterPlot.VisualProperties.YAxisTitle = "Quality";
      scatterPlot.VisualProperties.XAxisMinimumAuto = false;
      scatterPlot.VisualProperties.XAxisMaximumAuto = false;
      scatterPlot.VisualProperties.YAxisMinimumAuto = false;
      scatterPlot.VisualProperties.YAxisMaximumAuto = false;

      scatterPlot.VisualProperties.XAxisMinimumFixedValue = 0;
      scatterPlot.VisualProperties.XAxisMaximumFixedValue = MaxTreeLength;
      scatterPlot.VisualProperties.YAxisMinimumFixedValue = 0;
      scatterPlot.VisualProperties.YAxisMaximumFixedValue = 2;
      Results.Add(new Result("Pareto Front Analysis", "A scatterplot displaying the evaluated solutions and (if available) the analytically optimal front", scatterPlot));

      var paretoFrontRow = new ScatterPlotDataRow("Pareto Front", "", new List<Point2D<double>>());
      paretoFrontRow.VisualProperties.PointSize = 8;
      paretoFrontRow.VisualProperties.IsVisibleInLegend = true;
      paretoFrontRow.VisualProperties.PointStyle = ScatterPlotDataRowVisualProperties.ScatterPlotDataRowPointStyle.Square;
      paretoFrontRow.VisualProperties.Color = Color.Blue;
      ResultsScatterPlot.Rows.Add(paretoFrontRow);

      var newPointsRow = new ScatterPlotDataRow("New Points", "", new List<Point2D<double>>());
      newPointsRow.VisualProperties.PointSize = 8;
      newPointsRow.VisualProperties.IsVisibleInLegend = true;
      newPointsRow.VisualProperties.PointStyle = ScatterPlotDataRowVisualProperties.ScatterPlotDataRowPointStyle.Circle;
      newPointsRow.VisualProperties.Color = Color.Orange;
      ResultsScatterPlot.Rows.Add(newPointsRow);

      var communicatedPointsRow = new ScatterPlotDataRow("Communicated Points", "", new List<Point2D<double>>());
      communicatedPointsRow.VisualProperties.PointSize = 8;
      communicatedPointsRow.VisualProperties.IsVisibleInLegend = true;
      communicatedPointsRow.VisualProperties.PointStyle = ScatterPlotDataRowVisualProperties.ScatterPlotDataRowPointStyle.Triangle;
      communicatedPointsRow.VisualProperties.Color = Color.Purple;
      ResultsScatterPlot.Rows.Add(communicatedPointsRow);

      var ContributedPointsRow = new ScatterPlotDataRow("Contributed Points", "", new List<Point2D<double>>());
      ContributedPointsRow.VisualProperties.PointSize = 8;
      ContributedPointsRow.VisualProperties.IsVisibleInLegend = true;
      ContributedPointsRow.VisualProperties.PointStyle = ScatterPlotDataRowVisualProperties.ScatterPlotDataRowPointStyle.Star4;
      ContributedPointsRow.VisualProperties.Color = Color.Red;
      ResultsScatterPlot.Rows.Add(ContributedPointsRow);

      var SofarBestPointFromGA = new ScatterPlotDataRow("So-far Best Points", "", new List<Point2D<double>>());
      SofarBestPointFromGA.VisualProperties.PointSize = 8;
      SofarBestPointFromGA.VisualProperties.IsVisibleInLegend = true;
      SofarBestPointFromGA.VisualProperties.PointStyle = ScatterPlotDataRowVisualProperties.ScatterPlotDataRowPointStyle.Cross;
      SofarBestPointFromGA.VisualProperties.Color = Color.Green;
      ResultsScatterPlot.Rows.Add(SofarBestPointFromGA);

      //var qualitiesPerEvaluation = new IndexedDataTable<double>("Algorithm1 Qualities Per Evaluation") {
      //  VisualProperties = {
      //    XAxisTitle = "Evaluations",
      //    YAxisTitle = "Quality"
      //  },
      //  Rows = { new IndexedDataRow<double>("First-hit Graph") { VisualProperties = {
      //    ChartType = DataRowVisualProperties.DataRowChartType.StepLine,
      //    LineWidth = 2
      //  } } }
      //};

      //Results.Add(new Result("Algorithm1 Qualities Per Evaluation", qualitiesPerEvaluation));

      //var nsga2HypervolumePerEvaluation = new IndexedDataTable<double>("NSGA2 Hypervolume Per Evaluation") {
      //  VisualProperties = {
      //    XAxisTitle = "Evaluations",
      //    YAxisTitle = "Hypervolume"
      //  },
      //  Rows = { new IndexedDataRow<double>("First-hit Graph") { VisualProperties = {
      //    ChartType = DataRowVisualProperties.DataRowChartType.StepLine,
      //    LineWidth = 2
      //  } } }
      //};

      //Results.Add(new Result("NSGA2 Hypervolume Per Evaluation", nsga2HypervolumePerEvaluation));
    }
    protected override void Initialize(CancellationToken cancellationToken) {
      base.Initialize(cancellationToken);
    }



    //Analyzers
    public void GAAnalyzer() {
      ISymbolicExpressionTree treeGA = (ISymbolicExpressionTree)alg1.Elite.Solution.Clone();

      if (alg1.Elite != null) {
        ResultsBestSolutionAlg1 = new SymbolicRegressionModel(Problem.ProblemData.TargetVariable, (ISymbolicExpressionTree)treeGA.Clone(), Problem.SymbolicExpressionTreeInterpreter, Problem.EstimationLimits.Lower, Problem.EstimationLimits.Upper).CreateRegressionSolution(Problem.ProblemData);
      }
      
      ResultsAvgQualityAlg1 = alg1.AverageQuality;
      ResultsBestQualityAlg1 = alg1.BestQuality;
      ResultsWorstQualityAlg1 = alg1.WorstQuality;

      ResultsQualitiesAlg1Best.Values.Add(ResultsBestQualityAlg1);
      ResultsQualitiesAlg1Average.Values.Add(ResultsAvgQualityAlg1);

      ResultsQualitiesAlg1Worst.Values.Add(ResultsWorstQualityAlg1);
      ResultsQualitiesAlg1 = alg1.CalculateDoubleMatrix();
      ResultsAlg1Iterations++;

      //for (int i = 0; i < alg1.Elites.Count; i++) {
      //  var qualities = alg1.Elites[i].Quality.ToArray();
      //  for (int j = 0; j < alg1QualityLength; j++) {
      //    ResultsElitesAlg1[i, j] = qualities[j];
      //  }
      //}

    }
    public void NSGA2Analyzer() {
     
      //double evalNo = ResultsAlg2Evaluations;

      //if (maximization) {
      //  if ((ResultsBestQualityAlg2.Count() != 0 && bestQlty > ResultsBestQualityAlg2[0]) || (ResultsBestQualityAlg2.Count() == 0)) {
      //    pauseAlg2 = true;
      //  }
      //} else {
      //  if ((ResultsBestQualityAlg2.Count() != 0 && bestQlty < ResultsBestQualityAlg2[0]) || (ResultsBestQualityAlg2.Count() == 0)) {
      //    pauseAlg2 = true;
      //  }
      //}

      var nsga2Fitness = alg2.Fitness;
      for (int i = 0; i < Alg2PopulationSize; i++) {
        for (int k = 0; k < Problem.NumObjectives; k++) {
          ResultsParetoFrontAlg2[i, k] = nsga2Fitness[i][k];
        }
      }

      ResultsParetoFrontAlg2.RowNames = GetRowNames(ResultsParetoFrontAlg2);
      ResultsParetoFrontAlg2.ColumnNames = GetColumnNames(ResultsParetoFrontAlg2);

      ResultsNonDominatedSolutionsAlg2.Clear();
      ResultsBestQualitiesAlg2.Clear();
      foreach (var individual in alg2.CurrentFronts[0]) {
        if (ResultsAlg2Iterations == Alg2MaximumGenerations - 1) {
          var nonDominatedTree = individual.Solution;
          ResultsNonDominatedSolutionsAlg2.Add(new SymbolicRegressionModel(Problem.ProblemData.TargetVariable, (ISymbolicExpressionTree)nonDominatedTree.Clone(), Problem.SymbolicExpressionTreeInterpreter, Problem.EstimationLimits.Lower, Problem.EstimationLimits.Upper).CreateRegressionSolution(Problem.ProblemData));
        }
        var nonDominatedQuality = new DoubleArray(individual.Quality);
        ResultsBestQualitiesAlg2.Add(nonDominatedQuality);
      }
      

      AnalyzeParetoFront();
      ResultsAlg2Iterations++;
    }
    private void AnalyzeParetoFront() {
      var dataPointsParetoFront = new List<Point2D<double>>();
      var previousParetoPoints = new List<Point2D<double>>();
      bool maximization = Problem.Maximization[0];
      foreach (var individual in alg2.CurrentFronts[0]) {
        double[] fitnessValues = individual.Quality;
        double qlty = fitnessValues[0];
        //if (maximization) {
        //  qlty = 1- qlty;
        //}
        double treeLength = fitnessValues[1]; // Tree length 
        dataPointsParetoFront.Add(new Point2D<double>(treeLength, qlty));
      }
      if (ResultsAlg2Iterations == 0) {
        previousParetoPoints.AddRange(dataPointsParetoFront);
        ResultsScatterPlot.Rows["Pareto Front"].Points.Replace(previousParetoPoints);
      } else {
        List<Point2D<double>> pointsToRemove = new List<Point2D<double>>();
        previousParetoPoints.AddRange(ResultsScatterPlot.Rows["Pareto Front"].Points);
        foreach (var point in previousParetoPoints) {
          bool existsInCurrentFront = dataPointsParetoFront.Any(currentPoint =>
        currentPoint.X == point.X && currentPoint.Y == point.Y);
          if (!existsInCurrentFront) {
            pointsToRemove.Add(point);
          }
        }
        if (pointsToRemove.Count > 0) {
          foreach (var pointToRemove in pointsToRemove) {
            previousParetoPoints.Remove(pointToRemove);
          }
        }
        var newPoints = new List<Point2D<double>>();
        foreach (var point in dataPointsParetoFront) {
          bool existsInCurrentFront = previousParetoPoints.Any(currentPoint =>
         currentPoint.X == point.X && currentPoint.Y == point.Y);
          if (!existsInCurrentFront) {
            newPoints.Add(point);
          }
        }
        if (newPoints.Count > 0) {
          if (ResultsScatterPlot.Rows["New Points"].Points.Count > 0) {
            previousParetoPoints.AddRange(ResultsScatterPlot.Rows["New Points"].Points);
          }
          ResultsScatterPlot.Rows["New Points"].Points.Replace(newPoints);
        }
        ResultsScatterPlot.Rows["Pareto Front"].Points.Replace(previousParetoPoints);
      }

    }
    private IEnumerable<string> GetRowNames(DoubleMatrix front) {
      for (int i = 1; i <= front.Rows; i++) {
        yield return "Solution " + i.ToString();
      }
    }
    private IEnumerable<string> GetColumnNames(DoubleMatrix front) {
      for (int i = 1; i <= front.Columns; i++) {
        yield return "Objective " + i.ToString();
      }
    }

    #region Events
    protected override void OnExecutionTimeChanged() {
      base.OnExecutionTimeChanged();
      if (CancellationTokenSource == null) return;
      if (MaximumRuntime == -1) return;
      if (ExecutionTime.TotalSeconds > MaximumRuntime) CancellationTokenSource.Cancel();
    }
    #endregion
  }
}
