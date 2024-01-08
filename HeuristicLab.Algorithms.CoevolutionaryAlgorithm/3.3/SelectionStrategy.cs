using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Problems.CooperativeProblem;

namespace HeuristicLab.Algorithms.CoevolutionaryAlgorithm {
  [StorableType("8CE5FDC1-E9C9-4532-8D08-93794430A385")]
  public abstract class SelectionStrategy : Item, ISelectionStrategy {
    #region Constructors and Cloning
    [StorableConstructor]
    protected SelectionStrategy(StorableConstructorFlag _) { }
    public SelectionStrategy() : base() { }
    protected SelectionStrategy(SelectionStrategy other, Cloner cloner) : base(other, cloner) {

    }
    #endregion
    public bool IsValidQuality(double quality) {
      return !double.IsNaN(quality) && !double.IsInfinity(quality);
    }
    public abstract List<int> Select(int numSelectedIndividuals, IRandom rand, List<double> qualities, CooperativeProblem problem);
  }
}
