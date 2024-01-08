using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using HEAL.Attic;
using HeuristicLab.Core;

namespace HeuristicLab.Problems.CooperativeProblem {
 
  [StorableType("C3AEC845-387F-4B15-88BF-0CFD30C89027")]
  public enum CooperativeProblemType {
    UnconstrainedMultiObjectiveProblem,
    ConstrainedMultiObjectiveProblem,
    UnconstrainedSingleObjectiveProblem,
    WeightedSumConstrainedSingleObjectiveProblem,
  }
}
