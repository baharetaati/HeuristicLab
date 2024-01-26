﻿using System;
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
      //bool maximization = problem.Maximization[0];
      //if (maximization) {
      //    Quality = problem.EvaluateUnConstrainedWeightedSumPearsonRsquaredError(Solution, random);
      //  } else {
      //    //Quality = problem.EvaluateUnconstrainedWeightedSumMeanSquaredError(Solution, random);
      //    //Quality = problem.EvaluateUnconstrainedWeightedSumMethod(Solution, random);
      //    Quality = problem.EvaluateUnConstrainedWeightedSumPearsonRsquaredError(Solution, random);
      //    //Quality = problem.EvaluateRandomlyWeightedSumMethod(Solution, random);
      //  }
      
      var qlty = problem.EvaluateMultiObjectivePearsonRsquaredError(Solution, random);
      Quality[0] = qlty[0];
      Quality[1] = qlty[1];
      var normalizedTreeLength = (double)(qlty[1] - 1) / (problem.SymbolicExpressionTreeMaximumLength - 1);
      var f1 = Weight[0] * Math.Abs(qlty[0]);
      var f2 = Weight[1] * Math.Abs(normalizedTreeLength);
      Quality[2] = Math.Max(f1, f2);

      return Quality;
    }
    
  }
}
