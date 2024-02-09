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
  [StorableType("7D219469-B81D-4857-BD4C-1F476ADECB03")]
  public class IndividualNSGA2 : Individual {
    //[Storable]
    //public int QualityLength { get; set; }
    
    [Storable]
    public int Rank { get; set; }
    [Storable]
    public double CrowdingDistance { get; set; }
    #region Constructors
    [StorableConstructor]
    protected IndividualNSGA2(StorableConstructorFlag _) : base(_) { }
    //Recreating the individual of NSGA2
    public IndividualNSGA2(TreeRequirements treeRequirements, ISymbolicExpressionTree solution, double[] quality, int rank, double crowdingDist):base(treeRequirements, solution, quality) {
      Rank = rank;
      CrowdingDistance = crowdingDist;
    }
    //Offspring creation
    public IndividualNSGA2(TreeRequirements treeRequirements, ISymbolicExpressionTree solution, int qualityLength) : base(treeRequirements, solution, qualityLength) {
      Rank = -1;
      CrowdingDistance = -1.0;
    }
    //Initializing solutions
    public IndividualNSGA2(TreeRequirements treeRequirements, int qualityLength, CooperativeProblem problem, IRandom random)
         : base(treeRequirements, qualityLength, problem, random) {

      Rank = -1;
      CrowdingDistance = -1.0;

    }
    public IndividualNSGA2(TreeRequirements treeRequirements, ISymbolicExpressionTree solution, double[] quality) : base(treeRequirements, solution, quality) {
      Rank = -1;
      CrowdingDistance = -1.0;
    }
    protected IndividualNSGA2(IndividualNSGA2 original, Cloner cloner):base(original, cloner) {
      Rank = original.Rank; 
      CrowdingDistance = original.CrowdingDistance;
    }
    #endregion
    #region Cloning
    public override IDeepCloneable Clone(Cloner cloner) {
      return new IndividualNSGA2(this, cloner);
    }
    #endregion
    public void SetRank(int rank) { this.Rank = rank; }
    public void SetCrowdingDistance(double crowdingDist) { this.CrowdingDistance = crowdingDist; }
    public override double[] Evaluate(IRandom random, CooperativeProblem problem) {
      bool maximization = problem.Maximization[0];
      if (maximization) {
        Quality = problem.EvaluateMultiObjectivePearsonRsquaredError(Solution, random);
      } else {
        Quality = problem.EvaluateUnconstrainedMultiObjectiveProblem(Solution, random);
      }

      return Quality;
    }
  }
}
