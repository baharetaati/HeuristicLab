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
  [Item("TournamentSelection3", "A tournament selection operator with GroupSize of three.")]
  [StorableType("8A928171-B2CF-4F08-89C0-9E135F11C5B6")]
  public class TournamentSelection3 : TournamentSelection{
    [StorableConstructor]
    protected TournamentSelection3(StorableConstructorFlag _) : base(_) { }
    public TournamentSelection3() : base(groupSize: 3) { }
    protected TournamentSelection3(TournamentSelection3 other, Cloner cloner) : base(other, cloner) { }
  }
}
