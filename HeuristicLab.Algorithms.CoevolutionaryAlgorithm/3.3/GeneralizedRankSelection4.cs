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
  [Item("GeneralizedRankSelection4", "The generalized rank selection operator with SelectionPressure of four.")]
  [StorableType("03BAA321-C35F-4A28-B6AF-E99A313358C2")]
  public class GeneralizedRankSelection4 : GeneralizedRankSelection {
    [StorableConstructor]
    protected GeneralizedRankSelection4(StorableConstructorFlag _) : base(_) { }
    public GeneralizedRankSelection4() : base(selectionPressure: 4) { }
    protected GeneralizedRankSelection4(GeneralizedRankSelection4 other, Cloner cloner) : base(other, cloner) { }
  }
}
