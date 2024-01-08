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
  [Item("GeneralizedRankSelection5", "The generalized rank selection operator with SelectionPressure of five.")]
  [StorableType("2CF5FCBA-3FE9-40E5-B0A9-3F84D44B1471")]
  public class GeneralizedRankSelection5 : GeneralizedRankSelection {
    [StorableConstructor]
    protected GeneralizedRankSelection5(StorableConstructorFlag _) : base(_) { }
    public GeneralizedRankSelection5() : base(selectionPressure: 5) { }
    protected GeneralizedRankSelection5(GeneralizedRankSelection5 other, Cloner cloner) : base(other, cloner) { }
  }
}
