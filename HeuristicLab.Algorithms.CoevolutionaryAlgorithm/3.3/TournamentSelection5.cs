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
  [Item("TournamentSelection5", "A tournament selection operator with GroupSize of five.")]
  [StorableType("D8ECF1CB-A5C7-4763-82F5-16EED2990B62")]
  public class TournamentSelection5 : TournamentSelection {
    [StorableConstructor]
    protected TournamentSelection5(StorableConstructorFlag _) : base(_) { }
    public TournamentSelection5() : base(groupSize: 5) { }
    protected TournamentSelection5(TournamentSelection5 other, Cloner cloner) : base(other, cloner) { }
  }
}
