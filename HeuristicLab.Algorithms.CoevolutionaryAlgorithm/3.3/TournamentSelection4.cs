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
  [Item("TournamentSelection4", "A tournament selection operator with GroupSize of four.")]
  [StorableType("CE0B4F41-C0B0-40E8-813B-B6760EC3E7E4")]
  public class TournamentSelection4 : TournamentSelection{
    [StorableConstructor]
    protected TournamentSelection4(StorableConstructorFlag _) : base(_) { }
    public TournamentSelection4() : base(groupSize: 4) { }
    protected TournamentSelection4(TournamentSelection4 other, Cloner cloner) : base(other, cloner) { }
  }
}
