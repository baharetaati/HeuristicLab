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
  [Item("GeneralizedRankSelection3", "The generalized rank selection operator with SelectionPressure of three.")]
  [StorableType("C6314C34-CFE6-4143-9B54-E1193E97548D")]
  public class GeneralizedRankSelection3 : GeneralizedRankSelection{
    [StorableConstructor]
    protected GeneralizedRankSelection3(StorableConstructorFlag _) : base(_) { }
    public GeneralizedRankSelection3() : base(selectionPressure: 3) { }
    protected GeneralizedRankSelection3(GeneralizedRankSelection3 other, Cloner cloner) : base(other, cloner) { }
  }
}
