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
  [StorableType("32F2D4F7-C065-4053-B973-3FEA3022A21F")]
  public interface ISelectionStrategy : IItem {
    List<int> Select(int numSelectedIndividuals, IRandom rand, List<double> qualities, CooperativeProblem problem);
  }
}
