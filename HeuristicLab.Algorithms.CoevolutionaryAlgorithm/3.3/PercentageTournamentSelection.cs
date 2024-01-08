using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;

namespace HeuristicLab.Algorithms.CoevolutionaryAlgorithm {
  [Item("PercentageTournamentSelection", "A tournament selection operator with GroupSize of ten percent of population size.")]
  [StorableType("1C182C4F-ED1C-49B5-9E72-14ADFCDB3D4B")]
  public class PercentageTournamentSelection : TournamentSelection {
    #region Constructors
    [StorableConstructor]
    protected PercentageTournamentSelection(StorableConstructorFlag _) : base(_) { }
    public PercentageTournamentSelection() : base(usePercentageGroupSize: true) { }
    protected PercentageTournamentSelection(PercentageTournamentSelection other, Cloner cloner) : base(other, cloner) { }
    #endregion

  }
}
