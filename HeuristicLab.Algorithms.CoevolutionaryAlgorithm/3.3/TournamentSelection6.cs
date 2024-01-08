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
  [Item("TournamentSelection6", "A tournament selection operator with GroupSize of six.")]
  [StorableType("9A32B5BA-D566-42B7-85D3-8DAEF8561929")]
  public class TournamentSelection6 : TournamentSelection {
    [StorableConstructor]
    protected TournamentSelection6(StorableConstructorFlag _) : base(_) { }
    public TournamentSelection6() : base(groupSize: 6) { }
    protected TournamentSelection6(TournamentSelection6 other, Cloner cloner) : base(other, cloner) { }
  }
}
