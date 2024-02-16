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

namespace HeuristicLab.Algorithms.CoevolutionaryAlgorithm {
  [StorableType("40C3E34E-72E3-4110-8472-3C19420E6221")]
  public class BaseAlg : DeepCloneable {
    #region Properties
    [Storable]
    protected TreeRequirements _treeRequirements;
    #endregion
    #region Clonning and Constructors
    [StorableConstructor]
    protected BaseAlg(StorableConstructorFlag _) : base(_) { }
    public BaseAlg(TreeRequirements treeRequirements) : base() {
      _treeRequirements = treeRequirements;
    }
    protected BaseAlg(BaseAlg original, Cloner cloner) : base(original, cloner) {
      _treeRequirements = cloner.Clone(original._treeRequirements);
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new BaseAlg(this, cloner);
    }
    #endregion

    // Common Methods 
    protected ISymbolicExpressionTree Crossover(ISymbolicExpressionTree parent1, ISymbolicExpressionTree parent2, IRandom random) {
      var crossOver = new SubtreeCrossover();
      var childTree = SubtreeCrossover.Cross(random, parent1, parent2, crossOver.CrossoverProbability, crossOver.InternalCrossoverPointProbability.Value, _treeRequirements.MaxTreeLength, _treeRequirements.MaxTreeDepth);
      return childTree;
    }
    protected double CalculateAverage(List<double> qualities) {
      double sum = 0.0;
      foreach (var val in qualities) {
        sum += val;
      }
      var avg = sum / qualities.Count;
      return avg;
    }
  }
}
