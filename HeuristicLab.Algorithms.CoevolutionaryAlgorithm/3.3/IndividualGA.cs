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
    
    #region Constructors
    [StorableConstructor]
    //Offspring creation
    protected IndividualGA(StorableConstructorFlag _): base(_) { }
    public IndividualGA(TreeRequirements treeRequirements, ISymbolicExpressionTree solution, int qualityLength) : base(treeRequirements, solution, qualityLength) { }
    //Initializing solutions
    public IndividualGA(TreeRequirements treeRequirements, int qualityLength, CooperativeProblem problem, IRandom random)
         : base(treeRequirements, qualityLength, problem, random) {}
    protected IndividualGA(IndividualGA original, Cloner cloner):base(original, cloner) {
      
    }
    #endregion
    #region Cloning
    public override IDeepCloneable Clone(Cloner cloner) {
      return new IndividualGA(this, cloner);
    }
    #endregion
    public double[] EvaluateSingleObjective(IRandom random, CooperativeProblem problem) {
      bool maximization = problem.Maximization[0];
      if (maximization) {
        Quality[0] = problem.EvaluateSingleObjectivePearsonRsquaredError(Solution, random);
      } else {
        Quality[0] = problem.EvaluateSingleObjectiveNormalizedMeanSquaredError(Solution, random);
        //Quality[0] = problem.EvaluateSingleObjectiveMeanSquaredError(Solution, random);
      }
      
      return Quality;
    }
    public override double[] Evaluate(IRandom random, CooperativeProblem problem) {
      bool maximization = problem.Maximization[0];
      if (maximization) {
        Quality = problem.EvaluateUnConstrainedWeightedSumPearsonRsquaredError(Solution, random);
      } else {
        //Quality = problem.EvaluateUnconstrainedWeightedSumMeanSquaredError(Solution, random);
        Quality = problem.EvaluateUnconstrainedWeightedSumMethod(Solution, random);
        //Quality = problem.EvaluateRandomlyWeightedSumMethod(Solution, random);
      }
      return Quality;
    }
    
  }
}
