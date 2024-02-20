using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Problems.CooperativeProblem;

namespace HeuristicLab.Algorithms.CoevolutionaryAlgorithm {
  [StorableType("77A598F7-5523-4740-B12F-2802577C422B")]
  public class IndividualGA : Individual {
    [Storable]
    public double[] Weight { get; set; }
    
    #region Constructors
    [StorableConstructor]
    //Offspring creation
    protected IndividualGA(StorableConstructorFlag _): base(_) { }
    public IndividualGA(TreeRequirements treeRequirements, ISymbolicExpressionTree solution, int qualityLength, double[] weight) : base(treeRequirements, solution, qualityLength) {
      Weight = weight.ToArray();
      
    }
    //Initializing solutions
    public IndividualGA(TreeRequirements treeRequirements, int qualityLength, double[] weight, CooperativeProblem problem, IRandom random)
         : base(treeRequirements, qualityLength, problem, random) {
      Weight = weight.ToArray();
      
    }
    public IndividualGA(TreeRequirements treeRequirements, ISymbolicExpressionTree solution, double[] quality, double normalizedTreeLength, double[] weight) : base(treeRequirements, solution, quality, normalizedTreeLength) {
      Weight = weight.ToArray();
    }
    protected IndividualGA(IndividualGA original, Cloner cloner):base(original, cloner) {
      Weight = original.Weight;
      
    }
    #endregion
    #region Cloning
    public override IDeepCloneable Clone(Cloner cloner) {
      return new IndividualGA(this, cloner);
    }
    #endregion
    public override double[] Evaluate(IRandom random, CooperativeProblem problem) {
      //bool maximization = problem.Maximization[0];
      var qlty = problem.EvaluateMultiObjectivePearsonRsquaredError(Solution, random);
      
      //if (maximization) {
        
      //} else {
      //  qlty = problem.EvaluateMultiObjectivePearsonRsquaredError(Solution, random);
      //  //qlty = problem.EvaluateUnconstrainedMultiObjectiveProblem(Solution, random);
      //}


      Quality[0] = qlty[0];
      Quality[1] = qlty[1];
      NormalizedTreeLength = (double)(qlty[1] - _treeRequirements.MinTreeLength) / (_treeRequirements.MaxTreeLength - _treeRequirements.MinTreeLength);
      var f1 = Weight[0] * Math.Abs(qlty[0]);
      var f2 = Weight[1] * Math.Abs(NormalizedTreeLength);
      Quality[2] = Math.Max(f1, f2);

      return Quality;
    }
 
  }
}
