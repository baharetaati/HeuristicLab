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
  [Item("TournamentSelection7", "A tournament selection operator with GroupSize of seven.")]
  [StorableType("75B18950-AEE9-4B16-BB28-5D048A181CD9")]
  public class TournamentSelection7 : TournamentSelection {
    [StorableConstructor]
    protected TournamentSelection7(StorableConstructorFlag _) : base(_) { }
    public TournamentSelection7() : base(groupSize: 7) { }
    protected TournamentSelection7(TournamentSelection7 other, Cloner cloner) : base(other, cloner) { }
  }
}
