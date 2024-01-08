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
 
  [Item("Hybrid Cooperative Algorithm", "A hybrid cooperative approach hybridizing a GA and NSGA2")]
  [Creatable(CreatableAttribute.Categories.PopulationBasedAlgorithms, Priority = 700)] // Ask how to initialize Priority field
  [StorableType("CCD6D33A-829D-4402-BE6C-A4B6EB6A72FB")]


  public class HybridCooperativeApproach : BasicAlgorithm {

    //public string Filename { get; set; }
    
    #region Problem Properties
    public override Type ProblemType {
      get { return typeof(CooperativeProblem); }
    }
    public new CooperativeProblem Problem {
      get { return (CooperativeProblem) base.Problem; }
      set { base.Problem = value; }
    }
    #endregion
    public override bool SupportsPause {
      get { return true; }
    }
    #region Parameter Properties
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
    //public IFixedValueParameter<BoolValue> MaximizationParameter {
    //  get { return (IFixedValueParameter<BoolValue>)Parameters["Maximization"]; }
    //}
    // GA Parameters
    private IFixedValueParameter<IntValue> GAPopulationSizeParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters["GAPopulationSize"]; }
    }
    private IFixedValueParameter<IntValue> ElitesGAParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters["ElitesGA"]; }
    }
    private IValueParameter<IntValue> GAMaximumGenerationsParameter {
      get { return (IValueParameter<IntValue>)Parameters["GAMaximumGenerations"]; }
    }
    public IFixedValueParameter<IntValue> GAMaximumEvaluatedSolutionsParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters["GAMaximumEvaluatedSolutions"]; }
    }
    public IFixedValueParameter<BoolValue> IsGASingleObjectiveParameter {
      get { return (IFixedValueParameter<BoolValue>)Parameters["Is GA Single Objective"]; }
    }
    public IConstrainedValueParameter<ISelectionStrategy> GASelectorParameter {
      get { return (IConstrainedValueParameter<ISelectionStrategy>)Parameters["GASelector"]; }
    }
    //private IFixedValueParameter<IntValue> MigrationIntervalGAToNSGA2Parameter {
    //  get { return (IFixedValueParameter<IntValue>)Parameters["Migration Interval from GA to NSGA2"]; }
    //}
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
    // NSGA2 Parameters
    private IFixedValueParameter<IntValue> NSGA2PopulationSizeParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters["NSGA2PopulationSize"]; }
    }
    private IFixedValueParameter<IntValue> NSGA2MaximumGenerationsParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters["NSGA2MaximumGenerations"]; }
    }
    public IFixedValueParameter<IntValue> NSGA2MaximumEvaluatedSolutionsParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters["NSGA2MaximumEvaluatedSolutions"]; }
    }
    public IFixedValueParameter<IntValue> GenerationsForNSGA2ExecutionParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters["Generations for Excution of NSGA2"]; }
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
    // GA Parameters
    public int GAPopulationSize {
      get { return GAPopulationSizeParameter.Value.Value; }
      set { GAPopulationSizeParameter.Value.Value = value; }
    }
    public int GAMaximumGenerations {
      get { return GAMaximumGenerationsParameter.Value.Value; }
      set { GAMaximumGenerationsParameter.Value.Value = value; }
    }
    public int GAMaximumEvaluatedSolutions {
      get { return GAMaximumEvaluatedSolutionsParameter.Value.Value; }
      set { GAMaximumEvaluatedSolutionsParameter.Value.Value = value; }
    }
    public int ElitesGA {
      get { return ElitesGAParameter.Value.Value; }
      set { ElitesGAParameter.Value.Value = value; }
    }
    public ISelectionStrategy GASelector {
      get { return GASelectorParameter.Value; }
      set { GASelectorParameter.Value = value; }
    }
    public bool IsGASingleObjective {
      get { return IsGASingleObjectiveParameter.Value.Value; }
      set { IsGASingleObjectiveParameter.Value.Value = value; }
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
    // NSGA2 Parameters
    public int NSGA2PopulationSize {
      get { return NSGA2PopulationSizeParameter.Value.Value; }
      set { NSGA2PopulationSizeParameter.Value.Value = value; }
    }
    public int NSGA2MaximumGenerations {
      get { return NSGA2MaximumGenerationsParameter.Value.Value; }
      set { NSGA2MaximumGenerationsParameter.Value.Value = value; }
    }
    public int NSGA2MaximumEvaluatedSolutions {
      get { return NSGA2MaximumEvaluatedSolutionsParameter.Value.Value; }
      set { NSGA2MaximumEvaluatedSolutionsParameter.Value.Value = value; }
    }
    public int GenerationsForNSGA2Execution {
      get { return GenerationsForNSGA2ExecutionParameter.Value.Value; }
      set { GenerationsForNSGA2ExecutionParameter.Value.Value = value; }
    }
    #endregion
    #region Results Properties
    // GA results
    private int ResultsGAIterations {
      get { return ((IntValue)Results["GAGenerations"].Value).Value; }
      set { ((IntValue)Results["GAGenerations"].Value).Value = value; }
    }
    private int ResultsGAEvaluations {
      get { return ((IntValue)Results["GAEvaluations"].Value).Value; }
      set { ((IntValue)Results["GAEvaluations"].Value).Value = value; }
    }
    public ISymbolicRegressionSolution ResultsBestSolutionGA {
      get { return (ISymbolicRegressionSolution)Results["Best Solution for GA"].Value; }
      set { Results["Best Solution for GA"].Value = value; }
    }
    private DoubleArray ResultsBestQualityGA {
      get { return (DoubleArray)Results["Best Quality for GA"].Value; }
      set { Results["Best Quality for GA"].Value = new DoubleArray(value.ToArray()); }
    }
    private DoubleArray ResultsWorstQualityGA {
      get { return (DoubleArray)Results["Worst Quality for GA"].Value; }
      set { Results["Worst Quality for GA"].Value = new DoubleArray(value.ToArray()); }
    }
    private double ResultsAvgQualityGA {
      get { return ((DoubleValue)Results["Avg Quality for GA"].Value).Value; }
      set { ((DoubleValue)Results["Avg Quality for GA"].Value).Value = value; }
    }
    private double ResultsActivePressure {
      get { return ((DoubleValue)Results["Active Selection Pressure"].Value).Value; }
      set { ((DoubleValue)Results["Active Selection Pressure"].Value).Value = value; }
    }
    public DoubleMatrix ResultsQualitiesGA {
      get { return (DoubleMatrix)Results["GA Qualities"].Value; }
      set { Results["GA Qualities"].Value = value; }
    }
    private DataTable ResultsQualitiesGATable {
      get { return ((DataTable)Results["GA Timetable Qualities"].Value); }
    }
    private DataRow ResultsQualitiesGABest {
      get { return ResultsQualitiesGATable.Rows["Best Quality"]; }
    }
    private DataRow ResultsQualitiesGAWorst {
      get { return ResultsQualitiesGATable.Rows["Worst Quality"]; }
    }
    private DataRow ResultsQualitiesGAAverage {
      get { return ResultsQualitiesGATable.Rows["Average Quality"]; }
    }
    private IndexedDataTable<double> ResultsGAQualitiesPerEvaluation {
      get { return ((IndexedDataTable<double>)Results["GA Qualities Per Evaluation"].Value); }
    }
    public ItemList<IntValue> ResultsGARunIntervalInGenerations {
      get { return (ItemList<IntValue>)Results["GA Run Interval Generations"].Value; }
      set { Results["GA Run Interval Generations"].Value = value; }
    }
    // NSGA2 results
    private int ResultsNSGA2Iterations {
      get { return ((IntValue)Results["NSGA2Generations"].Value).Value; }
      set { ((IntValue)Results["NSGA2Generations"].Value).Value = value; }
    }
    private int ResultsNSGA2Evaluations {
      get { return ((IntValue)Results["NSGA2Evaluations"].Value).Value; }
      set { ((IntValue)Results["NSGA2Evaluations"].Value).Value = value; }
    }
    public ISymbolicRegressionSolution ResultsChosenSolutionNSGA2 {
      get { return (ISymbolicRegressionSolution)Results["Best Solution for NSGA2"].Value; }
      set { Results["Best Solution for NSGA2"].Value = value; }
    }
    public ItemList<ISymbolicRegressionSolution> ResultsNonDominatedSolutionsNSGA2 {
      get { return (ItemList<ISymbolicRegressionSolution>)Results["Non Dominated Solution for NSGA2"].Value; }
      set { Results["Non Dominated Solution for NSGA2"].Value = value; }
    }
    public ItemList<DoubleArray> ResultsBestQualitiesNSGA2 {
      get { return (ItemList<DoubleArray>)Results["Best Qualities for NSGA2"].Value; }
      set { Results["Best Qualities for NSGA2"].Value = value; }
    }
    private DoubleArray ResultsBestQualityNSGA2 {
      get { return (DoubleArray)Results["Best Quality for NSGA2"].Value; }
      set { Results["Best Quality for NSGA2"].Value = new DoubleArray(value.ToArray()); }
    }
    public DoubleMatrix ResultsParetoFrontNSGA2 {
      get { return (DoubleMatrix)Results["NSGA2 Pareto Front"].Value; }
      set { Results["NSGA2 Pareto Front"].Value = value; }
    }
    private double ResultsHypervolumeNSGA2 {
      get { return ((DoubleValue)Results["Hypervolume for NSGA2"].Value).Value; }
      set { ((DoubleValue)Results["Hypervolume for NSGA2"].Value).Value = value; }
    }
    private double ResultsHypervolumeNSGA2WithGAContribution {
      get { return ((DoubleValue)Results["Hypervolume for NSGA2 With GA Contribution"].Value).Value; }
      set { ((DoubleValue)Results["Hypervolume for NSGA2 With GA Contribution"].Value).Value = value; }
    }
    //public DoubleMatrix ResultsHypervolumeInfo {
    //  get { return (DoubleMatrix)Results["Hypervolume Info"].Value; }
    //  set { Results["Hypervolume Info"].Value = value; }
    //}
    //public IntMatrix ResultsGAContribution {
    //  get { return (IntMatrix)Results["GA Contribution"].Value; }
    //  set { Results["GA Contribution"].Value = value; }
    //}
    //private DataTable ResultsCommunicationGAToNSGA {
    //  get { return ((DataTable)Results["Communication GA --> NSGA2"].Value); }
    //}
    //private DataRow ResultsGACommunicatedPointsRow {
    //  get { return ResultsCommunicationGAToNSGA.Rows["Communicated GA --> NSGA2"]; }
    //}
    //private DataRow ResultsGAContributedPointsRow {
    //  get { return ResultsCommunicationGAToNSGA.Rows["Contributed GA --> NSGA2"]; }
    //}
    private ScatterPlot ResultsScatterPlot {
      get { return (ScatterPlot)Results["Pareto Front Analysis"].Value; }
      set { Results["Pareto Front Analysis"].Value = value; }
    }
    private DataTable ResultsHypervolumeData {
      get { return ((DataTable)Results["Hypervolume Data"].Value); }
    }
    private DataRow ResultsNSGA2HypervolumeWithoutGAContributionRow {
      get { return ResultsHypervolumeData.Rows["NSGA2 Hypervolume Without GA Contribution"]; }
    }
    private DataRow ResultsNSGA2HypervolumeWithGAContributionRow {
      get { return ResultsHypervolumeData.Rows["NSGA2 Hypervolume With GA Contribution"]; }
    }
    
    public ItemList<IntValue> ResultsNSGA2RunIntervalInGenerations {
      get { return (ItemList<IntValue>)Results["NSGA2 Run Interval Generations"].Value; }
      set { Results["NSGA2 Run Interval Generations"].Value = value; }
    }
    //private DataRow ResultsHypervolumeContributionOfGARow {
    //  get { return ResultsHypervolumeData.Rows["Hypervolume Contribution of GA"]; }
    //}
    //private IndexedDataTable<double> ResultsNSGA2HypervolumePerEvaluation {
    //  get { return ((IndexedDataTable<double>)Results["NSGA2 Hypervolume Per Evaluation"].Value); }
    //}
    #endregion
    #region Storable fields
    [Storable]
    private IRandom random = new MersenneTwister();
    [Storable]
    private GA ga;
    [Storable]
    private int numSelectedIndividualsGA;
    [Storable]
    private int numSelectedIndividualNSGA;
    [Storable]
    private TreeRequirements treeRequirements;
    [Storable]
    private NSGA2 nsga2;
    [Storable]
    private bool pauseNSGA2 = false;
    [Storable]
    private int nsga2Interval = 0;
    [Storable]
    private int gaInterval = 0;
    //[Storable]
    //public GAWithOffspringSelection offspringSelector;
    
    //[Storable]
    //private int migrationIntervalGAToNSGA2 = 100;
    //[Storable]
    //private int iterationTemporaryNSGA2 = 0;
    //[Storable]
    //private List<Point2D<double>> previousParetoPoints;
    //[Storable]
    //private List<Point2D<double>> recentlyAddedParetoPoints;
    #endregion
    [StorableConstructor]
    protected HybridCooperativeApproach(StorableConstructorFlag _) : base(_) {}
    protected HybridCooperativeApproach(HybridCooperativeApproach original, Cloner cloner)
      : base(original, cloner) {
        random = cloner.Clone(original.random);
        ga = cloner.Clone(original.ga);
        nsga2 = cloner.Clone(original.nsga2);
        treeRequirements = cloner.Clone(original.treeRequirements);
        numSelectedIndividualsGA = original.numSelectedIndividualsGA;
        numSelectedIndividualNSGA = original.numSelectedIndividualNSGA;
        pauseNSGA2 = original.pauseNSGA2;
        //offspringSelector = original.offspringSelector;
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new HybridCooperativeApproach(this, cloner);
    }
    public HybridCooperativeApproach() {
      Parameters.Add(new FixedValueParameter<IntValue>("Seed", "The random seed used to initialize the new pseudo random number generator.", new IntValue(0)));
      Parameters.Add(new FixedValueParameter<BoolValue>("SetSeedRandomly", "True if the random seed should be set to a random value, otherwise false.", new BoolValue(true)));
      Parameters.Add(new FixedValueParameter<IntValue>("GAPopulationSize", "The size of the population of solutions.", new IntValue(1000)));
      Parameters.Add(new FixedValueParameter<IntValue>("NSGA2PopulationSize", "The size of the population of solutions.", new IntValue(1000)));
      Parameters.Add(new FixedValueParameter<IntValue>("GAMaximumGenerations", "The maximum number of generations which should be processed.", new IntValue(1000)));
      Parameters.Add(new FixedValueParameter<IntValue>("NSGA2MaximumGenerations", "The maximum number of generations which should be processed.", new IntValue(1000)));
      Parameters.Add(new FixedValueParameter<IntValue>("GAMaximumEvaluatedSolutions", "The maximum number of solutions which should be evaluated.", new IntValue(1000000)));
      Parameters.Add(new FixedValueParameter<IntValue>("NSGA2MaximumEvaluatedSolutions", "The maximum number of solutions which should be evaluated.", new IntValue(1000000)));
      Parameters.Add(new FixedValueParameter<IntValue>("Maximum Runtime", "The maximum runtime in seconds after which the algorithm stops. Use -1 to specify no limit for the runtime", new IntValue(3600)));
      Parameters.Add(new FixedValueParameter<IntValue>("MaxTreeLength", "The maximum tree length for expression trees.", new IntValue(25))); 
      Parameters.Add(new FixedValueParameter<IntValue>("MaxTreeDepth", "The maximum tree depth for expression trees.", new IntValue(20)));
      Parameters.Add(new FixedValueParameter<PercentValue>("MutationProbability", "The probability that the mutation operator is applied on a solution.", new PercentValue(0.25)));
      Parameters.Add(new FixedValueParameter<IntValue>("ElitesGA", "The numer of elite solutions which are kept in each generation.", new IntValue(1)));
      Parameters.Add(new FixedValueParameter<BoolValue>("Is GA Single Objective", "To define if GA should be single- or multi-objective", new BoolValue(false)));
      Parameters.Add(new FixedValueParameter<IntValue>("Migration Interval from GA to NSGA2", "Migration interval from GA to NSGA2.", new IntValue(100)));
      Parameters.Add(new FixedValueParameter<IntValue>("Generations for Excution of NSGA2", "Generation numbers NSGA2 runs", new IntValue(20)));
      Parameters.Add(new FixedValueParameter<BoolValue>("ActiveOffspringSelector", "To define if GA should be single- or multi-objective", new BoolValue(true)));
      Parameters.Add(new FixedValueParameter<DoubleValue>("MaximumSelectionPressure", "The maximum selection pressure which prematurely terminates the offspring selection step.", new DoubleValue(1.0)));
      Parameters.Add(new FixedValueParameter<DoubleValue>("SuccessRatio", "The ratio of successful offspring that has to be produced.", new DoubleValue(0.8)));
      Parameters.Add(new FixedValueParameter<DoubleValue>("ComparisonFactor", "A factor to compare offspring with parents", new DoubleValue(1.0)));
      //Parameters.Add(new FixedValueParameter<DoubleValue>("ActiveSelectionPressure", "Active selection pressure for offspring selection", new DoubleValue(0.0)));
      
      //Parameters.Add(new FixedValueParameter<BoolValue>("Maximization", "If the problem is maximization or not (Just for the Run-Length Distribution)", new BoolValue(false)));
      GAMaximumGenerationsParameter.Hidden = true;
      NSGA2MaximumGenerationsParameter.Hidden = true;

      var set = new ItemSet<ISelectionStrategy> { new ProportionalSelection(), new TournamentSelection(), new TournamentSelection3(), new TournamentSelection4(), new TournamentSelection5(), new TournamentSelection6(), new TournamentSelection7(), new PercentageTournamentSelection(), new GeneralizedRankSelection(), new GeneralizedRankSelection3(), new GeneralizedRankSelection4(), new GeneralizedRankSelection5()};
      Parameters.Add(new ConstrainedValueParameter<ISelectionStrategy>("GASelector", "The selection mechanism for GA", set, set.First()));
      //RegisterEventHandlers();
    }
    protected override void Initialize(CancellationToken cancellationToken) {
      if (SetSeedRandomly) {
        Seed = RandomSeedGenerator.GetSeed();
      }

      random.Reset(Seed);

      treeRequirements = new TreeRequirements(MaxTreeLength, MaxTreeDepth, MutationProbability);
      
      // GA Parameters
      numSelectedIndividualsGA = 2 * (GAPopulationSize - ElitesGA);
      if (ActiveOffspringSelector) {
        ga = new GA(Problem.NumObjectives + 1, treeRequirements, GASelector);
      }
      else {
        ga = new GA(Problem.NumObjectives + 1, treeRequirements, GASelector, IsGASingleObjective, ElitesGA);
      }
      // NSGA2 Parameters
      numSelectedIndividualNSGA = 2 * NSGA2PopulationSize;
      nsga2 = new NSGA2(Problem.NumObjectives, treeRequirements);
      GAMaximumGenerations = GAMaximumEvaluatedSolutions / GAPopulationSize;
      NSGA2MaximumGenerations = NSGA2MaximumEvaluatedSolutions / NSGA2PopulationSize;
      pauseNSGA2 = false;
      
      nsga2Interval = 0;
      gaInterval = 0;
      
      InitResults();
      base.Initialize(cancellationToken);
    }
    private void InitResults() {
      var problem = Problem as CooperativeProblem;
      if (problem == null) return;
      Results.Add(new Result("GAGenerations", "The number of gererations evaluated", new IntValue(0)));
      Results.Add(new Result("GAEvaluations", "The number of function evaluations performed", new IntValue(0)));
      Results.Add(new Result("NSGA2Generations", "The number of gererations evaluated", new IntValue(0)));
      Results.Add(new Result("NSGA2Evaluations", "The number of function evaluations performed", new IntValue(0)));
      Results.Add(new Result("Best Solution for GA", "The best so-far solution found by GA", (IItem)null));
      Results.Add(new Result("Best Quality for GA", "Current best quality for GA", new DoubleArray()));
      Results.Add(new Result("Worst Quality for GA", "Current worst quality for GA", new DoubleArray()));
      Results.Add(new Result("Avg Quality for GA", "Current average quality", new DoubleValue(0.0)));
      Results.Add(new Result("GA Qualities", "Current GA Qualities matrix", new DoubleMatrix()));
      Results.Add(new Result("Active Selection Pressure", "Active selection pressure for offspring selection", new DoubleValue(0.0)));

      Results.Add(new Result("Non Dominated Solution for NSGA2", "The non-dominated solutions for NSGA2", new ItemList<ISymbolicRegressionSolution>())); 
      Results.Add(new Result("Best Qualities for NSGA2", "The best qualities found by NSGA2", new ItemList<DoubleArray>()));
      Results.Add(new Result("Best Solution for NSGA2", "The best so-far solution found by NSGA2", (IItem)null));
      Results.Add(new Result("Best Quality for NSGA2", "The best so-far quality found by NSGA2", new DoubleArray()));
      Results.Add(new Result("Hypervolume for NSGA2", "Current hypervolume for NSGA2", new DoubleValue(0.0)));
      Results.Add(new Result("Hypervolume for NSGA2 With GA Contribution", "NSGA2 hypervolume with GA contribution", new DoubleValue(0.0)));
      Results.Add(new Result("NSGA2 Pareto Front", "Current NSGA2 pareto front", new DoubleMatrix()));
      Results.Add(new Result("GA Run Interval Generations", "GA Generation Interval", new ItemList<IntValue>()));
      Results.Add(new Result("NSGA2 Run Interval Generations", "NSGA2 Generation Interval", new ItemList<IntValue>()));

      
      //Results.Add(new Result("Hypervolume Info", "Hypervolume information", new DoubleMatrix()));
      //Results.Add(new Result("GA Contribution", "GA Contribution to the NSGA2 Pareto Front", new IntMatrix()));

      var table = new DataTable("GA Timetable Qualities");
      table.Rows.Add(new DataRow("Best Quality"));
      table.Rows.Add(new DataRow("Worst Quality"));
      var avgQualityRows = new DataRow("Average Quality"); // Best quality after the elite
      avgQualityRows.VisualProperties.LineStyle = DataRowVisualProperties.DataRowLineStyle.Dot;

      table.Rows.Add(avgQualityRows);

      Results.Add(new Result("GA Timetable Qualities", table));

      var numColumns = problem.NumObjectives;
      
      ResultsParetoFrontNSGA2 = new DoubleMatrix(NSGA2PopulationSize, numColumns);
      ResultsQualitiesGA = new DoubleMatrix(GAPopulationSize, numColumns + 1);
      //ResultsHypervolumeInfo = new DoubleMatrix(GAMaximumGenerations, 3);
      //ResultsGAContribution = new IntMatrix(GAMaximumGenerations, 2);

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
      //previousParetoPoints = new List<Point2D<double>>();
      //recentlyAddedParetoPoints = new List<Point2D<double>>();
      //communicatedPoints = new List<Point2D<double>>();
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

      //var tableForCommunication = new DataTable("Communication GA --> NSGA2");

      //var gaCommunicatedPointsRow = new DataRow("Communicated GA --> NSGA2");
      //gaCommunicatedPointsRow.VisualProperties.LineStyle = DataRowVisualProperties.DataRowLineStyle.Solid;
      //gaCommunicatedPointsRow.VisualProperties.LineWidth = 3;
      //gaCommunicatedPointsRow.VisualProperties.Color = Color.DarkOrchid;
      //tableForCommunication.Rows.Add(gaCommunicatedPointsRow);


      //var gaContributedPointsRow = new DataRow("Contributed GA --> NSGA2");
      //gaContributedPointsRow.VisualProperties.LineStyle = DataRowVisualProperties.DataRowLineStyle.DashDotDot;
      //gaContributedPointsRow.VisualProperties.LineWidth = 3;
      //gaContributedPointsRow.VisualProperties.Color = Color.DarkOrange;
      //tableForCommunication.Rows.Add(gaContributedPointsRow);


      //Results.Add(new Result("Communication GA --> NSGA2", tableForCommunication));

      var tableHypervolume = new DataTable("Hypervolume Data");

      var nsga2HypervolumeRow = new DataRow("NSGA2 Hypervolume Without GA Contribution");
      nsga2HypervolumeRow.VisualProperties.LineStyle = DataRowVisualProperties.DataRowLineStyle.Solid;
      nsga2HypervolumeRow.VisualProperties.LineWidth = 3;
      nsga2HypervolumeRow.VisualProperties.Color = Color.BlueViolet;
      tableHypervolume.Rows.Add(nsga2HypervolumeRow);

      var nsag2GAHypervolumeRow = new DataRow("NSGA2 Hypervolume With GA Contribution");
      nsag2GAHypervolumeRow.VisualProperties.LineStyle = DataRowVisualProperties.DataRowLineStyle.DashDotDot;
      nsag2GAHypervolumeRow.VisualProperties.LineWidth = 3;
      nsag2GAHypervolumeRow.VisualProperties.Color = Color.DeepPink;
      tableHypervolume.Rows.Add(nsag2GAHypervolumeRow);

      //var gaHypervolumeContributionRow = new DataRow("Hypervolume Contribution of GA");
      //gaHypervolumeContributionRow.VisualProperties.LineStyle = DataRowVisualProperties.DataRowLineStyle.DashDotDot;
      //gaHypervolumeContributionRow.VisualProperties.LineWidth = 3;
      //gaHypervolumeContributionRow.VisualProperties.Color = Color.DarkSeaGreen;
      //tableHypervolume.Rows.Add(gaHypervolumeContributionRow);
      Results.Add(new Result("Hypervolume Data", tableHypervolume));

      var qualitiesPerEvaluation = new IndexedDataTable<double>("GA Qualities Per Evaluation") {
        VisualProperties = {
          XAxisTitle = "Evaluations",
          YAxisTitle = "Quality"
        },
        Rows = { new IndexedDataRow<double>("First-hit Graph") { VisualProperties = {
          ChartType = DataRowVisualProperties.DataRowChartType.StepLine,
          LineWidth = 2
        } } }
      };

      Results.Add(new Result("GA Qualities Per Evaluation", qualitiesPerEvaluation));

      var nsga2HypervolumePerEvaluation = new IndexedDataTable<double>("NSGA2 Hypervolume Per Evaluation") {
        VisualProperties = {
          XAxisTitle = "Evaluations",
          YAxisTitle = "Hypervolume"
        },
        Rows = { new IndexedDataRow<double>("First-hit Graph") { VisualProperties = {
          ChartType = DataRowVisualProperties.DataRowChartType.StepLine,
          LineWidth = 2
        } } }
      };

      Results.Add(new Result("NSGA2 Hypervolume Per Evaluation", nsga2HypervolumePerEvaluation));
    }
    protected override void Run(CancellationToken cancellationToken) {
      bool IsAnalysisPerformedLastly = false;
      while ((ResultsGAEvaluations < GAMaximumEvaluatedSolutions && ResultsGAIterations < GAMaximumGenerations) || (ResultsNSGA2Evaluations < NSGA2MaximumEvaluatedSolutions && ResultsNSGA2Iterations < NSGA2MaximumGenerations)) {
        try {
          Iterate();
          cancellationToken.ThrowIfCancellationRequested();
        } catch (Exception ex) {
          Console.WriteLine(ex.Message);
          Console.WriteLine(ex.StackTrace);
          throw;
        } finally {
          Analyze();
          if (ResultsGAEvaluations == GAMaximumEvaluatedSolutions && ResultsNSGA2Evaluations == NSGA2MaximumEvaluatedSolutions) {
            IsAnalysisPerformedLastly = true;
          }
        }
      }
      if (!IsAnalysisPerformedLastly) {
        Analyze();
      }
      
    }
    private void Iterate() {
      if (!pauseNSGA2) {
        var nsgaCountEvaluations = 0;
        var countNSGAEvaluations = nsga2.Apply(NSGA2PopulationSize, numSelectedIndividualNSGA, Problem, ResultsNSGA2Iterations, random);
        nsgaCountEvaluations += countNSGAEvaluations;
        ResultsNSGA2Evaluations += nsgaCountEvaluations;
        nsga2Interval++;
      } else if (pauseNSGA2) {
          var gaCountEvaluations = 0;
          int countGAEvaluations;
          if (ActiveOffspringSelector) {
            countGAEvaluations = ga.ApplyOffspringSelection(GAPopulationSize, numSelectedIndividualsGA, Problem, ResultsGAIterations, MaximumSelectionPressure, SuccessRatio, random);
          } else {
            countGAEvaluations = ga.Apply(GAPopulationSize, numSelectedIndividualsGA, Problem, ResultsGAIterations, random);
          }
          gaCountEvaluations += countGAEvaluations;
          ResultsGAEvaluations += gaCountEvaluations;
          gaInterval++;          
      }
    }
    private void Analyze() {
      //GA resutls
      if (!pauseNSGA2) {
        NSGA2Analyzer();
        ResultsNSGA2Iterations++;
      } else {
        GAAnalyzer();
        CommunicationGAToNSGA2();
        ResultsGAIterations++;
      }
      if (ResultsGAEvaluations == GAMaximumEvaluatedSolutions && ResultsNSGA2Evaluations < NSGA2MaximumEvaluatedSolutions) {
        if (pauseNSGA2) {
          pauseNSGA2 = false;
          nsga2Interval = 0;
          ResultsGARunIntervalInGenerations.Add(new IntValue(gaInterval));
          Console.WriteLine($"gaInterval = {gaInterval}");
        }
      } else if (ResultsNSGA2Evaluations == NSGA2MaximumEvaluatedSolutions && ResultsGAEvaluations < GAMaximumEvaluatedSolutions) {
        if (!pauseNSGA2) {
          pauseNSGA2 = true;
          gaInterval = 0;
          ResultsNSGA2RunIntervalInGenerations.Add(new IntValue(nsga2Interval));
          Console.WriteLine($"nsga2Interval = {nsga2Interval}");
        }
      }
      if (ResultsNSGA2Evaluations == NSGA2MaximumEvaluatedSolutions) {
        ResultsNSGA2RunIntervalInGenerations.Add(new IntValue(nsga2Interval));
      }
      ResultsNSGA2HypervolumeWithoutGAContributionRow.Values.Add(ResultsHypervolumeNSGA2);
      ResultsNSGA2HypervolumeWithGAContributionRow.Values.Add(ResultsHypervolumeNSGA2WithGAContribution);
    }
    private void GAAnalyzer() {
      if (IsGASingleObjective) {
        SingleObjectiveGAAnalyzer();
      } else {
        WeightedSumGAAnalyzer();
      }
    }
   
    private void WeightedSumGAAnalyzer() {
      ISymbolicExpressionTree treeGA = null;
        
      double[] bestQualityGA = new double[ga.QualityLength];
      double[] worstQualityGA = new double[ga.QualityLength];
      double bestQlty;
      double worstQlty;
      int weightedSumIndex = ga.QualityLength - 1;
      bool maximization = Problem.Maximization[0];
      if (maximization) {
        bestQlty = double.MinValue;
        worstQlty= double.MaxValue;
      } else {
        bestQlty = double.MaxValue;
        worstQlty = double.MinValue;
      }
      if (maximization) {
        foreach (var individual in ga.Population) {
          if (individual.Quality[weightedSumIndex] > bestQlty) {
            bestQlty = individual.Quality[weightedSumIndex];
            bestQualityGA = individual.Quality;
          }
        }

        foreach (var individual in ga.Population) {
          if (individual.Quality[weightedSumIndex] < worstQlty) {
            worstQlty = individual.Quality[weightedSumIndex];
            worstQualityGA = individual.Quality;
            treeGA = individual.Solution;
          }
        }
      } else {
        foreach (var individual in ga.Population) {
          if (individual.Quality[weightedSumIndex] < bestQlty) {
            bestQlty = individual.Quality[weightedSumIndex];
            bestQualityGA = individual.Quality;
            //treeGA = individual.Solution;
          }
        }
        foreach (var individual in ga.Population) {
          if (individual.Quality[weightedSumIndex] > worstQlty) {
            worstQlty = individual.Quality[weightedSumIndex];
            worstQualityGA = individual.Quality;
            treeGA = individual.Solution;
          }
          
        }
      }

      ResultsBestSolutionGA = new SymbolicRegressionModel(Problem.ProblemData.TargetVariable, (ISymbolicExpressionTree)treeGA.Clone(), Problem.SymbolicExpressionTreeInterpreter, Problem.EstimationLimits.Lower, Problem.EstimationLimits.Upper).CreateRegressionSolution(Problem.ProblemData);
      ResultsAvgQualityGA = CalcualteAverage();
      double evalNo = ResultsGAEvaluations;
      //double weightedSumValue = bestQualityGA[Problem.NumObjectives];
      if (maximization) {
        if ((ResultsBestQualityGA.Count() != 0 && bestQlty > ResultsBestQualityGA[Problem.NumObjectives]) || (ResultsBestQualityGA.Count() == 0 )) {
          ResultsGAQualitiesPerEvaluation.Rows["First-hit Graph"].Values.Add(Tuple.Create(evalNo, bestQlty));
        }
      } else {
        if ((ResultsBestQualityGA.Count() != 0 && bestQlty < ResultsBestQualityGA[Problem.NumObjectives]) || (ResultsBestQualityGA.Count() == 0 && bestQualityGA.Count() != 0)) {
          ResultsGAQualitiesPerEvaluation.Rows["First-hit Graph"].Values.Add(Tuple.Create(evalNo, bestQlty));
        }
      }

      ResultsBestQualityGA = new DoubleArray(bestQualityGA); 
      ResultsWorstQualityGA = new DoubleArray(worstQualityGA);

      ResultsQualitiesGABest.Values.Add(bestQlty);
      ResultsQualitiesGAAverage.Values.Add(ResultsAvgQualityGA);

      ResultsQualitiesGAWorst.Values.Add(worstQlty);

      var gaFitness = ga.Fitness;
      for (int i = 0; i < GAPopulationSize; i++) {
        for (int k = 0; k < ga.QualityLength; k++) {
          ResultsQualitiesGA[i, k] = gaFitness[i][k];
        }
      }
      if (ActiveOffspringSelector) {
        ResultsActivePressure = ga.PActive;
      }
    }
    private void SingleObjectiveGAAnalyzer() {
      ISymbolicExpressionTree treeGA = null;

      double[] bestQualityGA = new double[ga.QualityLength];
      double[] worstQualityGA = new double[ga.QualityLength];
      double bestQlty;
      double worstQlty;
      if (Problem.Maximization[0]) {
        bestQlty = double.MinValue;
        worstQlty = double.MaxValue;
        foreach (var individual in ga.Population) {
          if (individual.Quality[0] > bestQlty) {
            bestQlty = individual.Quality[0];
            bestQualityGA = individual.Quality;
            treeGA = individual.Solution;
          }
          if (individual.Quality[0] < worstQlty) {
            worstQlty = individual.Quality[0];
            worstQualityGA = individual.Quality;
          }
        }
      } else {
        bestQlty = double.MaxValue;
        worstQlty = double.MinValue;
        foreach (var individual in ga.Population) {
          if (individual.Quality[0] < bestQlty) {
            bestQlty = individual.Quality[0];
            bestQualityGA = individual.Quality;
            treeGA = individual.Solution;
          }
          if (individual.Quality[0] > worstQlty) {
            worstQlty = individual.Quality[0];
            worstQualityGA = individual.Quality;
          }
        }
      }
      ResultsBestSolutionGA = new SymbolicRegressionModel(Problem.ProblemData.TargetVariable, (ISymbolicExpressionTree)treeGA.Clone(), Problem.SymbolicExpressionTreeInterpreter, Problem.EstimationLimits.Lower, Problem.EstimationLimits.Upper).CreateRegressionSolution(Problem.ProblemData);

      ResultsAvgQualityGA = CalcualteAverage();

      ResultsBestQualityGA = new DoubleArray(bestQualityGA);
      ResultsWorstQualityGA = new DoubleArray(worstQualityGA);

      ResultsQualitiesGABest.Values.Add(bestQlty);
      ResultsQualitiesGAAverage.Values.Add(ResultsAvgQualityGA);

      ResultsQualitiesGAWorst.Values.Add(worstQlty);
    }
    private void NSGA2Analyzer() {
      ISymbolicExpressionTree treeNSGA2 = null;
      double[] bestQualityNSGA = new double[nsga2.QualityLength];
      
      double bestQlty;
      bool maximization = Problem.Maximization[0];
      if (maximization) {
        bestQlty = double.MinValue;
        foreach (var individual in nsga2.Population) {
          if (individual.Quality[0] > bestQlty) {
            bestQlty = individual.Quality[0];
            bestQualityNSGA = individual.Quality;
            treeNSGA2 = individual.Solution;
          }
        }
      } else {
        bestQlty = double.MaxValue;
        foreach (var individual in nsga2.Population) {
          if (individual.Quality[0] < bestQlty) {
            bestQlty = individual.Quality[0];
            bestQualityNSGA = individual.Quality;
            treeNSGA2 = individual.Solution;
          }
        }
      }

      //double evalNo = ResultsNSGA2Evaluations;

      //if (maximization) {
      //  if ((ResultsBestQualityNSGA2.Count() != 0 && bestQlty > ResultsBestQualityNSGA2[0]) || (ResultsBestQualityNSGA2.Count() == 0)) {
      //    pauseNSGA2 = true;
      //  }
      //} else {
      //  if ((ResultsBestQualityNSGA2.Count() != 0 && bestQlty < ResultsBestQualityNSGA2[0]) || (ResultsBestQualityNSGA2.Count() == 0)) {
      //    pauseNSGA2 = true;
      //  }
      //}
      
      
      ResultsBestQualityNSGA2 = new DoubleArray(bestQualityNSGA);
      ResultsChosenSolutionNSGA2 = new SymbolicRegressionModel(Problem.ProblemData.TargetVariable, (ISymbolicExpressionTree)treeNSGA2.Clone(), Problem.SymbolicExpressionTreeInterpreter, Problem.EstimationLimits.Lower, Problem.EstimationLimits.Upper).CreateRegressionSolution(Problem.ProblemData);

      var nsga2Fitness = nsga2.Fitness;
      for (int i = 0; i < NSGA2PopulationSize; i++) {
        for (int k = 0; k < Problem.NumObjectives; k++) {
          ResultsParetoFrontNSGA2[i, k] = nsga2Fitness[i][k];
        }
      }

      ResultsParetoFrontNSGA2.RowNames = GetRowNames(ResultsParetoFrontNSGA2);
      ResultsParetoFrontNSGA2.ColumnNames = GetColumnNames(ResultsParetoFrontNSGA2);

      ResultsNonDominatedSolutionsNSGA2.Clear();
      ResultsBestQualitiesNSGA2.Clear();
      foreach (var individual in nsga2.CurrentFronts[0]) {
        if (ResultsNSGA2Iterations == NSGA2MaximumGenerations - 1) {
          var nonDominatedTree = individual.Solution;
          ResultsNonDominatedSolutionsNSGA2.Add(new SymbolicRegressionModel(Problem.ProblemData.TargetVariable, (ISymbolicExpressionTree)nonDominatedTree.Clone(), Problem.SymbolicExpressionTreeInterpreter, Problem.EstimationLimits.Lower, Problem.EstimationLimits.Upper).CreateRegressionSolution(Problem.ProblemData));
        }
        var nonDominatedQuality = new DoubleArray(individual.Quality);
        ResultsBestQualitiesNSGA2.Add(nonDominatedQuality);
      }
      
      var refPoints = new double[] { 1.0, MaxTreeLength };
      List<double[]> nsga2ParetoFrontQualities = new List<double[]>();

      if (nsga2.CurrentFronts[0].Count > 0) {
        foreach (var ind in nsga2.CurrentFronts[0]) {
          nsga2ParetoFrontQualities.Add(ind.Quality.ToArray());
        }
      }
      var transformedFront = nsga2ParetoFrontQualities.Select(solution => new double[] { 1 - solution[0], solution[1] });
      var maximizationArray = new bool[] {false, false};
      var nsga2Hypervolume = Hypervolume.Calculate(transformedFront, refPoints, maximizationArray);
      if (nsga2Interval > 5 && nsga2Hypervolume > ResultsHypervolumeNSGA2 && ResultsGAEvaluations < GAMaximumEvaluatedSolutions && ResultsNSGA2Evaluations < NSGA2MaximumEvaluatedSolutions) {
        ResultsNSGA2RunIntervalInGenerations.Add(new IntValue(nsga2Interval));
        Console.WriteLine($"nsga2Interval = {nsga2Interval}");
        pauseNSGA2 = true;
        gaInterval = 0;
        //AdjustWeights();
      }
      ResultsHypervolumeNSGA2 = nsga2Hypervolume;
      
      AnalyzeParetoFront();
    }
    private double CalcualteAverage() {
      double sum = 0.0;
      double[] qualities = new double[ga.Fitness.Count];
      if (IsGASingleObjective) {
        for (int i = 0; i < ga.Fitness.Count; i++) {
          qualities[i] = ga.Fitness[i][0];
        }
      } else {
        for (int i = 0; i < ga.Fitness.Count; i++) {
          qualities[i] = ga.Fitness[i][Problem.NumObjectives];
        }
      }

      foreach (var val in qualities) {
        sum += val;
      }
      var avg = sum / qualities.Length;

      return avg;
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
    private void AnalyzeParetoFront() {
      var dataPointsParetoFront = new List<Point2D<double>>();
      var previousParetoPoints = new List<Point2D<double>>();
      bool maximization = Problem.Maximization[0];
      foreach (var individual in nsga2.CurrentFronts[0]) {
        double[] fitnessValues = individual.Quality;
        double qlty = fitnessValues[0]; 
        if (maximization) {
          qlty = 1- qlty;
        }
        double treeLength = fitnessValues[1]; // Tree length 
        dataPointsParetoFront.Add(new Point2D<double>(treeLength, qlty));
      }
      if (ResultsNSGA2Iterations == 0) {
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
    private void CommunicationGAToNSGA2() {
      //if (pauseNSGA2) {
        // Scenario two: Considering the best error values
        var gaBestSolutions = GABestErrorSelection();
        var gaBestQualities = new List<double[]>();
        if (gaBestSolutions.Count > 0) {
          foreach (var individual in gaBestSolutions) {
            double[] indQualities = individual.Quality.Take(Problem.NumObjectives).ToArray();
            gaBestQualities.Add(indQualities);
          }
        }
        //gaParetoFrontQualities.AddRange(gaBestQualities);
        //Removing Duplicate Solutions
        for (int i = 0; i < gaBestQualities.Count; i++) {
          for (int j = i + 1; j < gaBestQualities.Count; j++) {
            // Compare arrays element by element
            bool isDuplicate = true;
            for (int k = 0; k < gaBestQualities[i].Length; k++) {
              if (gaBestQualities[i][k] != gaBestQualities[j][k]) {
                isDuplicate = false;
                break;
              }
            }
            // If a duplicate is found, remove it
            if (isDuplicate) {
              gaBestQualities.RemoveAt(j);
              j--; // Adjust the index after removal
            }
          }
        }

        List<double[]> gaContributionParetoFront = new List<double[]>();
        List<double[]> nsga2ParetoFrontQualities = new List<double[]>();
        
        if (nsga2.CurrentFronts[0].Count > 0) {
          foreach (var ind in nsga2.CurrentFronts[0]) {
            nsga2ParetoFrontQualities.Add(ind.Quality.ToArray());
          }
        }

        int gaContribution = 0;
        for (int i = 0; i < gaBestQualities.Count; i++) {
          bool isDominated = false;
          foreach (var nsgaQlty in nsga2ParetoFrontQualities) {
            var result = DominationCalculator<ISymbolicExpressionTree>.Dominates(gaBestQualities[i].ToArray(), nsgaQlty, Problem.Maximization, true);
            if (result == DominationResult.IsDominated) {
              isDominated = true;
            }
          }
          if (!isDominated) {
            nsga2ParetoFrontQualities.Add(gaBestQualities[i].ToArray());
            gaContributionParetoFront.Add(gaBestQualities[i].ToArray());
            gaBestQualities.RemoveAt(i);
            //gaBestQualities[i] = null;
            i--;
            gaContribution++;
          }
        }
        
        if (gaContribution > 0) {
          var refPoints = new double[] { 1.0, MaxTreeLength };
          nsga2ParetoFrontQualities.AddRange(gaContributionParetoFront);
          var transformedFront = nsga2ParetoFrontQualities.Select(solution => new double[] { 1 - solution[0], solution[1] });
          var maximizationArray = new bool[] { false, false };
          var nsga2HypervolumeIncludingGASolutions = Hypervolume.Calculate(transformedFront, refPoints, maximizationArray);
          ResultsHypervolumeNSGA2WithGAContribution = nsga2HypervolumeIncludingGASolutions;
          nsga2.AppendToNSGA2Population(gaBestSolutions, Problem, NSGA2PopulationSize);
          AnalyzeParetoFrontWhileCommunication(gaBestQualities, gaContributionParetoFront);
          if (gaInterval > 5 && ResultsHypervolumeNSGA2WithGAContribution > ResultsHypervolumeNSGA2 && ResultsNSGA2Evaluations < NSGA2MaximumEvaluatedSolutions && ResultsGAEvaluations < GAMaximumEvaluatedSolutions) {
            pauseNSGA2 = false;
            ResultsGARunIntervalInGenerations.Add(new IntValue(gaInterval));
            Console.WriteLine($"gaInterval = {gaInterval}");
            nsga2Interval = 0;
          }
        }
      //}
    }
    //if (!pauseNSGA2 && iterationTemporaryNSGA2 == GenerationsForNSGA2Execution && ResultsGAEvaluations < GAMaximumEvaluatedSolutions) {
    //  pauseNSGA2 = true;
    //}
    // Scenario One: Non dominated solutions
    //List<double[]> nsga2ParetoFrontQualities = new List<double[]>();
    //if (nsga2.CurrentFronts[0].Count > 0) {
    //  foreach (var ind in nsga2.CurrentFronts[0]) {
    //    nsga2ParetoFrontQualities.Add(ind.Quality.ToArray());
    //  }

    //  List<double[]> gaContributionParetoFront = new List<double[]>();
    //  List<double[]> paretoFrontIncludingGASolutions = new List<double[]>();


    //var refPoints = new double[] { 2, MaxTreeLength };
    //var nsga2Hypervolume = Hypervolume.Calculate(nsga2ParetoFrontQualities, refPoints, Problem.Maximization);
    //double evalNo = ResultsNSGA2Evaluations;
    //if (nsga2Hypervolume > ResultsHypervolumeNSGA2) {
    //  ResultsNSGA2HypervolumePerEvaluation.Rows["First-hit Graph"].Values.Add(Tuple.Create(evalNo, nsga2Hypervolume));
    //  ResultsHypervolumeNSGA2 = nsga2Hypervolume;

    //}
    ////ResultsHypervolumeInfo[ResultsGAIterations, 0] = nsga2Hypervolume;
    //ResultsNSGA2HypervolumeWithoutGAContributionRow.Values.Add(nsga2Hypervolume);

    //double nsga2HypervolumeIncludingGASolutions;
    //if (gaContribution > 0) {
    //  nsga2HypervolumeIncludingGASolutions = Hypervolume.Calculate(paretoFrontIncludingGASolutions, refPoints, Problem.Maximization);
    //} else {
    //  nsga2HypervolumeIncludingGASolutions = nsga2Hypervolume;
    //}
    //double hypervolumeContribution = nsga2HypervolumeIncludingGASolutions - nsga2Hypervolume;
    ////ResultsHypervolumeInfo[ResultsGAIterations, 1] = nsga2HypervolumeIncludingGASolutions;
    //ResultsNSGA2HypervolumeWithGAContributionRow.Values.Add(nsga2HypervolumeIncludingGASolutions);
    ////ResultsHypervolumeInfo[ResultsGAIterations, 2] = hypervolumeContribution;
    //ResultsHypervolumeContributionOfGARow.Values.Add(hypervolumeContribution);

    //gaParetoFrontQualities.RemoveAll(item => item == null);
    private void AdjustWeights() {
      if (ga.Fitness.Count > 0) {
        var crowdingDist = nsga2.CrowdingDistances;
        List<int> indexes = Enumerable.Range(0, crowdingDist.Count).ToList();
        indexes.Sort((i1, i2) => -crowdingDist[i1].CompareTo(crowdingDist[i2]));

        int halfCount = indexes.Count / 2;
        List<int> selectedIndices = indexes.Take(halfCount).ToList();

        List<double[]> selectedFit = new List<double[]>();

        foreach (var ind in selectedIndices) {
          selectedFit.Add(nsga2.Fitness[ind].ToArray());
        }

        double[] averageErrors = Enumerable.Range(0, selectedFit[0].Length)
      .Select(i => selectedFit.Average(arr => arr[i]))
      .ToArray();

        var err_avg = averageErrors[0];
        var treeLen_avg = averageErrors[1];

        double bestErr = ga.Fitness.Max(arr => arr[0]);
        double bestTreeLen = ga.Fitness.Min(arr => arr[1]);

        // Tree length normalization
        double minTreeLength = 0.0;
        double maxTreeLength = MaxTreeLength;
        double minNormalizedTreeLength = 0.0;
        double maxNormalizedTreeLength = 1.0;

        double normalizedAvgTreeLength = (double)(treeLen_avg - minTreeLength) / (maxTreeLength - minTreeLength);
        double normalizedBestTreeLength = (double)(bestTreeLen - minNormalizedTreeLength) / (maxTreeLength - minNormalizedTreeLength) * (maxNormalizedTreeLength - minNormalizedTreeLength) + minNormalizedTreeLength;

        double errDif = Math.Abs(err_avg - bestErr);
        double treeLenDif = Math.Abs(normalizedAvgTreeLength - normalizedBestTreeLength);
        Console.WriteLine($"errDif = {errDif} vs. treeLenDif = {treeLenDif}");

        double delta = Math.Abs(errDif - treeLenDif);
        double threshold = .001;
        double[] weights = Problem.Weights.ToArray();
        double w1 = weights[0];
        double w2 = Math.Abs(weights[1]);

        if (delta <= threshold) {
          w1 = .5;
          w2 = .5;
        } else if (errDif > treeLenDif) {
          if (weights[0] != 1.0 && weights[0] + .1 <= 1) {
            w1 = weights[0] + .1;
            w2 = 1 - w1;
          }
        } else {
          if (Math.Abs(weights[1]) != 1.0 && Math.Abs(weights[1]) + .1 <= 1) {
            w2 = Math.Abs(weights[1]) + .1;
            w1 = 1 - w2;
          }
        }
        Problem.UpdateWeights(w1, w2);
      }
    }
    private List<IndividualGA> GABestErrorSelection() {

      var qualities = new List<double[]>();
      var weightedSumValues = new List<double>();
      bool maximization = Problem.Maximization[0];
      foreach (var individual in ga.Population) {
        double[] indQualities = individual.Quality.Take(Problem.NumObjectives).ToArray();
        double indQualities2 = individual.Quality[Problem.NumObjectives];
        qualities.Add(indQualities);
        weightedSumValues.Add(indQualities2);
      }

      //List<int> indexes = Enumerable.Range(0, ga.Fitness.Count).ToList();
      List<int> indexes2 = Enumerable.Range(0, ga.Fitness.Count).ToList();

      if (!maximization) {
        //indexes.Sort((i1, i2) => qualities[i1][0].CompareTo(qualities[i2][0]));
        indexes2.Sort((i1, i2) => weightedSumValues[i1].CompareTo(weightedSumValues[i2]));
      } else {
        //indexes.Sort((i1, i2) => -qualities[i1][0].CompareTo(qualities[i2][0]));
        indexes2.Sort((i1, i2) => -weightedSumValues[i1].CompareTo(weightedSumValues[i2]));
      }

      //int numSelectedBasedOnNMSE = GAPopulationSize / 4;
      int numSelectedBasedOnWS = GAPopulationSize / 4;

      //List<double[]> selectedFitness = new List<double[]>();
      List<IndividualGA> selectedSolutions = new List<IndividualGA>();

      //for (int i = 0; i < numSelectedBasedOnNMSE; i++) {
      //  selectedFitness.Add(qualities[indexes[i]].ToArray());
      //}

      for (int i = 0; i < numSelectedBasedOnWS; i++) {
        //selectedFitness.Add(qualities[indexes2[i]].ToArray());
        selectedSolutions.Add((IndividualGA)ga.Population[indexes2[i]].Clone());
      }
      //ResultsGAContribution[ResultsIterations, 0] = numSelectedForCommunication;
      return selectedSolutions;
    }
    private List<double[]> GANonDominatedSortingSelection() {
      int[] rank;
      var solutions = new List<ISymbolicExpressionTree>();
      var qualities = new List<double[]>();
      foreach (var individual in ga.Population) {
        solutions.Add((ISymbolicExpressionTree)individual.Solution.Clone());
        double[] indQualities = individual.Quality.Take(Problem.NumObjectives).ToArray();
        qualities.Add(indQualities);
      }
      RankAndCrowdingSort sorter = new RankAndCrowdingSort();
      var fronts = sorter.FastNonDominatedSorting(solutions, qualities, Problem.Maximization, out rank);

      List<ISymbolicExpressionTree> paretoFrontSolutions = fronts[0].Select(tuple => tuple.Item1).ToList();
      List<double[]> gaParetoFrontQualities = fronts[0].Select(tuple => tuple.Item2).ToList();

      int gaNonDominatedSolutionsCount = 0;
      for (int i = 0; i < rank.Length; i++) {
        if (rank[i] == 0) {
          gaNonDominatedSolutionsCount++;
          ga.Population[i].BeingCommunicated = true;

        }
      }
      return gaParetoFrontQualities;
    }
    private void AnalyzeParetoFrontWhileCommunication(List<double[]> gaPF, List<double[]> gaContributePF) {

      var dataPointsParetoFront = new List<Point2D<double>>();
      bool maximization = Problem.Maximization[0];
      if (gaPF.Count > 0) {
        foreach (var q in gaPF) {
          if (maximization) {
            q[0] = 1 - q[0];
          }
          dataPointsParetoFront.Add(new Point2D<double>(q[1], q[0]));
        }
      }
     
      var dataPointsContributedPF = new List<Point2D<double>>();
      if (gaContributePF.Count > 0) {
        foreach (var q in gaContributePF) {
          if (maximization) {
            q[0] = 1 - q[0];
          }
          dataPointsContributedPF.Add(new Point2D<double>(q[1], q[0]));
        }
      }

      //if (dataPointsParetoFront.Count > 0) {
      //  ResultsScatterPlot.Rows["Communicated Points"].Points.Replace(dataPointsParetoFront);
      //}

      if (dataPointsContributedPF.Count > 0) {
        ResultsScatterPlot.Rows["Contributed Points"].Points.Replace(dataPointsContributedPF);
      }
      var bestSofarQuality = new Point2D<double>(ResultsBestQualityGA[1], ResultsBestQualityGA[0]);
      if (maximization) {
        bestSofarQuality = new Point2D<double>(ResultsBestQualityGA[1], 1-ResultsBestQualityGA[0]);
      }
      
      ResultsScatterPlot.Rows["So-far Best Points"].Points.Replace(bestSofarQuality.ToEnumerable());

      //foreach (var individual in nsga2.CurrentFronts[0]) {
      //  double[] fitnessValues = individual.Quality;

      //  double qlty = fitnessValues[0];
      //  if (Problem.Maximization[0]) {
      //    qlty = 1 - qlty;
      //  }
      //  double treeLength = fitnessValues[1]; // Tree length 
      //  dataPointsParetoFront.Add(new Point2D<double>(treeLength, qlty));
      //}
      //foreach (var quality in fQualities) {
      //  dataPointsParetoFront.Add(new Point2D<double>(quality[1], quality[0]));
      //}
      //foreach (var point in dataPointsParetoFront) {
      //  bool existsInPreviousParetoPoints = previousParetoPoints.Any(currentPoint =>
      // currentPoint.X == point.X && currentPoint.Y == point.Y);

      //  bool existsInNewParetoPoints = recentlyAddedParetoPoints.Any(currentPoint =>
      // currentPoint.X == point.X && currentPoint.Y == point.Y);

      //  if (!existsInPreviousParetoPoints && !existsInNewParetoPoints) {
      //    communicatedPoints.Add(point);
      //  }
      //}
      //if (communicatedPoints.Count > 0) {
      //  ResultsScatterPlot.Rows["Communicated Points"].Points.AddRange(communicatedPoints);
      //}

    }
    //private void ParameterizeSelectors() {
    //  if (GASelector is PercentageTournamentSelection) {
    //    GASelector = new PercentageTournamentSelection(GAPopulationSize);
    //  }
    //}
    //private void RegisterEventHandlers() {
    //  GAPopulationSizeParameter.Value.ValueChanged += new EventHandler(GAPopulationSize_ValueChanged);
    //  GASelectorParameter.ValueChanged += new EventHandler(GAPopulationSize_ValueChanged);
    //}
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
