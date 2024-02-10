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
using HeuristicLab.Encodings.RealVectorEncoding;
using System.Net.Mime;

namespace HeuristicLab.Algorithms.CoevolutionaryAlgorithm {
  [StorableType("5EF90076-0721-4503-B60A-6417CDAEEADA")]
  public abstract class Orchestrator : CooperativeApproachBase{
    const double Epsilon = 1e-6;
    static readonly double[] IdealPoint = new double[] { 0.0, 1.0 };
    #region Results Properties
    public ItemList<IntValue> ResultsAlg1RunIntervalInGenerations {
      get { return (ItemList<IntValue>)Results["Algorithm1 Run Interval Generations"].Value; }
      set { Results["Algorithm1 Run Interval Generations"].Value = value; }
    }
    public double ResultsHypervolumeAlg2WithGAContribution {
      get { return ((DoubleValue)Results["Hypervolume for Algorithm2 With Algorithm1 Contribution"].Value).Value; }
      set { ((DoubleValue)Results["Hypervolume for Algorithm2 With Algorithm1 Contribution"].Value).Value = value; }
    }
    public double ResultsHypervolumeAlg2 {
      get { return ((DoubleValue)Results["Hypervolume for Algorithm2"].Value).Value; }
      set { ((DoubleValue)Results["Hypervolume for Algorithm2"].Value).Value = value; }
    }
    public DataTable ResultsHypervolumeData {
      get { return ((DataTable)Results["Hypervolume Data"].Value); }
    }
    public DataRow ResultsAlg2HypervolumeWithoutGAContributionRow {
      get { return ResultsHypervolumeData.Rows["Algorithm2 Hypervolume Without Algorithm1 Contribution"]; }
    }
    public DataRow ResultsAlg2HypervolumeWithGAContributionRow {
      get { return ResultsHypervolumeData.Rows["Algorithm2 Hypervolume With Algorithm1 Contribution"]; }
    }
    public ItemList<IntValue> ResultsAlg2RunIntervalInGenerations {
      get { return (ItemList<IntValue>)Results["Algorithm2 Run Interval Generations"].Value; }
      set { Results["Algorithm2 Run Interval Generations"].Value = value; }
    }
    #endregion
    #region Storable fields
    [Storable]
    public bool pauseAlg2 = false;
    [Storable]
    public int nsga2Interval = 0;
    [Storable]
    public int gaInterval = 0;
    [Storable]
    public int adjustWeightInterval = 0;
    #endregion
    #region Constructors
    [StorableConstructor]
    protected Orchestrator(StorableConstructorFlag _) : base(_) { }
    protected Orchestrator(Orchestrator original, Cloner cloner) {
      pauseAlg2 = original.pauseAlg2;
      gaInterval = original.gaInterval;
      adjustWeightInterval = original.adjustWeightInterval;
    }
    public Orchestrator() {

    }
    #endregion
    public override void InitResults() {
      base.InitResults();
      Results.Add(new Result("Hypervolume for Algorithm2", "Current hypervolume for NSGA2", new DoubleValue(0.0)));
      Results.Add(new Result("Hypervolume for Algorithm2 With Algorithm1 Contribution", "NSGA2 hypervolume with Algorithm1 contribution", new DoubleValue(0.0)));
      Results.Add(new Result("Algorithm1 Run Interval Generations", "Algorithm1 Generation Interval", new ItemList<IntValue>()));
      Results.Add(new Result("Algorithm2 Run Interval Generations", "Algorithm2 Generation Interval", new ItemList<IntValue>()));

      var tableHypervolume = new DataTable("Hypervolume Data");

      var nsga2HypervolumeRow = new DataRow("Algorithm2 Hypervolume Without Algorithm1 Contribution");
      nsga2HypervolumeRow.VisualProperties.LineStyle = DataRowVisualProperties.DataRowLineStyle.Solid;
      nsga2HypervolumeRow.VisualProperties.LineWidth = 3;
      nsga2HypervolumeRow.VisualProperties.Color = Color.BlueViolet;
      tableHypervolume.Rows.Add(nsga2HypervolumeRow);

      var nsag2GAHypervolumeRow = new DataRow("Algorithm2 Hypervolume With Algorithm1 Contribution");
      nsag2GAHypervolumeRow.VisualProperties.LineStyle = DataRowVisualProperties.DataRowLineStyle.DashDotDot;
      nsag2GAHypervolumeRow.VisualProperties.LineWidth = 3;
      nsag2GAHypervolumeRow.VisualProperties.Color = Color.DeepPink;
      tableHypervolume.Rows.Add(nsag2GAHypervolumeRow);

      Results.Add(new Result("Hypervolume Data", tableHypervolume));
    }
    public void IterateOrchestrator() {
      if (!pauseAlg2) {
        if (ResultsAlg2Evaluations < Alg2MaximumEvaluatedSolutions) {
          var nsgaCountEvaluations = 0;
          var countNSGAEvaluations = alg2.Apply(Alg2PopulationSize, numSelectedIndividualNSGA, Problem, ResultsAlg2Iterations, random);
          nsgaCountEvaluations += countNSGAEvaluations;
          ResultsAlg2Evaluations += nsgaCountEvaluations;
          nsga2Interval++;
        } else {
          pauseAlg2 = true;
        }
      } else if (pauseAlg2) {
        if (ResultsAlg1Evaluations < Alg1MaximumEvaluatedSolutions) {
          var gaCountEvaluations = 0;
          int countGAEvaluations;
          if (ResultsAlg1Iterations == 0) {
            countGAEvaluations = alg1.ApplyForInitialization(random, Alg1PopulationSize, Problem);
          }
          else {
            if (ActiveOffspringSelector) {
              countGAEvaluations = alg1.ApplyExtremeOffspringSelection(Alg1PopulationSize, Problem, random);
              ResultsActivePressure = alg1.ActiveSelectionPressure;
              ResultsCountSuccessfulOffspring = alg1.CountSuccessfulOffspring;
              ResultsCountUnsuccessfulOffspring = alg1.CountUnsuccessfulOffspring;
            } else {
              countGAEvaluations = alg1.Apply(Alg1PopulationSize, Problem, random);
            }
          }
          
          gaCountEvaluations += countGAEvaluations;
          ResultsAlg1Evaluations += gaCountEvaluations;
          gaInterval++;
        } else {
          pauseAlg2 = false;
        }

      }
    }
    public void AnalyzeOrchestrator() {
      if (!pauseAlg2) {
        NSGA2Analyzer();
        double[] refPoints;
        bool maximization = Problem.Maximization[0];
        bool[] maximizationArray = Problem.Maximization;
        if (maximization) {
          refPoints = new double[] { 0.0, MaxTreeLength };
        } else {
          refPoints = new double[] { 1.0, MaxTreeLength};
        }

        List<double[]> nsga2ParetoFrontQualities = new List<double[]>();

        if (alg2.CurrentFronts[0].Count > 0) {
          foreach (var ind in alg2.CurrentFronts[0]) {
            nsga2ParetoFrontQualities.Add(ind.Quality.ToArray());
          }
        }
        //var transformedFront = nsga2ParetoFrontQualities.Select(solution => new double[] { 1 - solution[0], solution[1] });

        var nsga2Hypervolume = HypervolumeCalculation.Calculate(nsga2ParetoFrontQualities, refPoints, maximizationArray);
        
        
        if (nsga2Interval > 5 && nsga2Hypervolume > ResultsHypervolumeAlg2 && ResultsAlg1Evaluations < Alg1MaximumEvaluatedSolutions && ResultsAlg2Evaluations < Alg2MaximumEvaluatedSolutions) {
          ResultsAlg2RunIntervalInGenerations.Add(new IntValue(nsga2Interval));
          Console.WriteLine($"nsga2Interval = {nsga2Interval}");
          pauseAlg2 = true;
          gaInterval = 0;
          if (ResultsAlg1Iterations != 0) {
            AdjustWeights(nsga2ParetoFrontQualities);
          }
          
        }
        ResultsHypervolumeAlg2 = nsga2Hypervolume;
        ResultsAlg2HypervolumeWithoutGAContributionRow.Values.Add(nsga2Hypervolume);
        ResultsAlg2HypervolumeWithGAContributionRow.Values.Add(ResultsHypervolumeAlg2WithGAContribution);
      } else {
        GAAnalyzer();
        CommunicationGAToNSGA2();
      }
      
    }
    private void AdjustWeights(List<double[]> paretoFront) {
      List<double[]> gaElites = new List<double[]>();
      int numObjectives = Problem.NumObjectives;
      foreach (var elite in alg1.Elites) {
        gaElites.Add(elite.Quality.Take(numObjectives).ToArray());
      }
      if (paretoFront.Count > 0) {
        double[] sparsityLevel = new double[paretoFront.Count];
        List<double[]> distances = new List<double[]>();
        for (int i = 0; i < paretoFront.Count; i++) {
          double[] paretoPoint = paretoFront[i];
          double sl = 1.0;
          double[] paretoDist = new double[gaElites.Count];
          for (int j = 0; j < gaElites.Count; j++) {
            double[] gaElite = gaElites[j];
            double dist = Math.Sqrt((gaElite[0] - paretoPoint[0]) * (gaElite[0] - paretoPoint[0]) + (gaElite[1] - paretoPoint[1]) * (gaElite[1] - paretoPoint[1]));
            sl *= dist;
            paretoDist[j] = dist;
          }
          distances.Add(paretoDist);
          sparsityLevel[i] = sl;
        }
        double minValue = sparsityLevel.Min();
        int sparsePointIndex = Array.IndexOf(sparsityLevel, minValue);
        double[] sdist = distances[sparsePointIndex];
        minValue = sdist.Min();
        int nearestEliteIndex = Array.IndexOf(sdist, minValue);

        double[] weight = new double[numObjectives];
        double sum = 0.0;
        for (int i = 0; i < numObjectives; i++) {
          sum += 1.0 / (double)(paretoFront[sparsePointIndex][i] - IdealPoint[i]+Epsilon);
        }
        for (int i = 0; i < numObjectives; i++) {
          weight[i] = (double)(1.0 / paretoFront[sparsePointIndex][i] - IdealPoint[i] + Epsilon) / sum;
        }
        Console.WriteLine($"weight[0]={weight[0]}, weight[1]={weight[1]}");
        alg1.SetWeight(weight, nearestEliteIndex);
      }
    }
    private void CommunicationGAToNSGA2() {
      //if (pauseAlg2) {
      // Scenario two: Considering the best error values
      //var gaBestSolutions = GABestErrorSelection();
      var gaBestQualities = new List<double[]>();
      //if (gaBestSolutions.Count > 0) {
      //  foreach (var individual in gaBestSolutions) {
      //    double[] indQualities = individual.Quality.Take(Problem.NumObjectives).ToArray();
      //    gaBestQualities.Add(indQualities);
      //  }
      //}
      if (alg1.Elites.Count > 0) {
        foreach (var individual in alg1.Elites) {
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

      if (alg2.CurrentFronts[0].Count > 0) {
        foreach (var ind in alg2.CurrentFronts[0]) {
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
      var maximization = Problem.Maximization[0];
      if (gaContribution > 0) {
        double[] refPoints;
        bool[] maximizationArray;
        if (maximization) {
          refPoints = new double[] { 0.0, MaxTreeLength };
          maximizationArray = new bool[] { true, false };
        } else {
          refPoints = new double[] { 1.0, MaxTreeLength };
          maximizationArray = new bool[] { false, false };
        }
        nsga2ParetoFrontQualities.AddRange(gaContributionParetoFront);
        //var transformedFront = nsga2ParetoFrontQualities.Select(solution => new double[] { 1 - solution[0], solution[1] });
        var nsga2HypervolumeIncludingGASolutions = HypervolumeCalculation.Calculate(nsga2ParetoFrontQualities, refPoints, maximizationArray);
        ResultsHypervolumeAlg2WithGAContribution = nsga2HypervolumeIncludingGASolutions;
        ResultsAlg2HypervolumeWithGAContributionRow.Values.Add(nsga2HypervolumeIncludingGASolutions);
        ResultsAlg2HypervolumeWithoutGAContributionRow.Values.Add(ResultsHypervolumeAlg2);
        alg2.AppendToNSGA2Population(alg1.Elites, Problem, Alg2PopulationSize);
        AnalyzeParetoFrontWhileCommunication(gaBestQualities, gaContributionParetoFront);
        if (gaInterval > 5 && ResultsHypervolumeAlg2WithGAContribution > ResultsHypervolumeAlg2 && ResultsAlg2Evaluations < Alg2MaximumEvaluatedSolutions && ResultsAlg1Evaluations < Alg1MaximumEvaluatedSolutions) {
          pauseAlg2 = false;
          ResultsAlg1RunIntervalInGenerations.Add(new IntValue(gaInterval));
          Console.WriteLine($"gaInterval = {gaInterval}");
          nsga2Interval = 0;
        }
      }
      //}
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

      if (dataPointsParetoFront.Count > 0) {
        ResultsScatterPlot.Rows["Communicated Points"].Points.Replace(dataPointsParetoFront);
      }

      if (dataPointsContributedPF.Count > 0) {
        ResultsScatterPlot.Rows["Contributed Points"].Points.Replace(dataPointsContributedPF);
      }
      var dataBestSofarPoints = new List<Point2D<double>>();
      for (int i = 0; i < alg1.Elites.Count; i++) {
        var qualities = alg1.Elites[i].Quality.ToArray();
        var bestSofarQuality = new Point2D<double>(qualities[1], qualities[0]);
        if (maximization) {
          bestSofarQuality = new Point2D<double>(qualities[1], 1 - qualities[0]);
        }
        dataBestSofarPoints.Add(bestSofarQuality);
        
      }

      if (dataBestSofarPoints.Count > 0) {
        ResultsScatterPlot.Rows["So-far Best Points"].Points.Replace(dataBestSofarPoints);
      }
      
    }
  }
}
