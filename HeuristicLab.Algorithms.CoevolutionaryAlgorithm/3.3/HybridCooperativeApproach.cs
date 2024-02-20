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
 
  [Item("Hybrid Cooperative Algorithm", "A hybrid cooperative approach hybridizing a DecompositionBasedGA and NSGA2")]
  [Creatable(CreatableAttribute.Categories.PopulationBasedAlgorithms, Priority = 700)] // Ask how to initialize Priority field
  [StorableType("CCD6D33A-829D-4402-BE6C-A4B6EB6A72FB")]


  public class HybridCooperativeApproach : Orchestrator {
    
    //[Storable]
    //public TreeRequirements treeRequirements;

    [StorableConstructor]
    protected HybridCooperativeApproach(StorableConstructorFlag _) : base(_) {}
    protected HybridCooperativeApproach(HybridCooperativeApproach original, Cloner cloner)
      : base(original, cloner) {}
    public override IDeepCloneable Clone(Cloner cloner) {
      return new HybridCooperativeApproach(this, cloner);
    }
    public HybridCooperativeApproach() : base(){}
    protected override void Initialize(CancellationToken cancellationToken) {
      base.Initialize(cancellationToken);
    }
    protected override void Run(CancellationToken cancellationToken) {
      while ((ResultsAlg1Evaluations < Alg1MaximumEvaluatedSolutions && ResultsActivePressure <= MaximumSelectionPressure) || ResultsAlg2Evaluations < Alg2MaximumEvaluatedSolutions) {
        try {
          Iterate();
          cancellationToken.ThrowIfCancellationRequested();
        } catch (Exception ex) {
          Console.WriteLine(ex.Message);
          Console.WriteLine(ex.StackTrace);
          throw;
        } finally {
          Analyze();
        }
      }
    }
    
  }
}
