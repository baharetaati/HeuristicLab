using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HEAL.Attic;

namespace HeuristicLab.Algorithms.CoevolutionaryAlgorithm {
  [StorableType("6E1E53E5-4B84-4AFD-B687-70B4A45003B8")]
  public enum OffspringParentsComparisonTypes {
    DominationBaseComparison,
    RandomIndexComparison,
    AccuracyBasedComparison,
    WeightedBasedComparison,
    EpsilonLexicaseBasedComparison
  }
}
