using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Problems.CooperativeProblem;

namespace HeuristicLab.Algorithms.CoevolutionaryAlgorithm {
  [Item("TournamentSelection", "A tournament selection operator which considers a single double quality value for selection (GroupSize of two).")]
  [StorableType("6ADD4CA5-DC43-42FA-A808-0FED2FBEF0FA")]
  public class TournamentSelection : SelectionStrategy {
    #region Properties
    [Storable]
    public int GroupSize { get; private set; }
    [Storable]
    public bool UsePercentageGroupSize { get; set; }
    #endregion

    #region Constructors and Cloning
    [StorableConstructor]
    protected TournamentSelection(StorableConstructorFlag _) : base(_) { }
    public TournamentSelection(int groupSize = 2, bool usePercentageGroupSize = false) : base() {

      GroupSize = groupSize;
      UsePercentageGroupSize = usePercentageGroupSize;


    }
    protected TournamentSelection(TournamentSelection other, Cloner cloner) : base(other, cloner) {
      GroupSize = other.GroupSize;


    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new TournamentSelection(this, cloner);
    }
    #endregion
    private void SetGroupSize(int populationSize) {
      int newGroupSize = (int)(0.1 * populationSize);
      GroupSize = newGroupSize;
    }
    public override List<int> Select(int numSelectedIndividuals, IRandom rand, List<double> qualities, CooperativeProblem problem) {
      
      int count = qualities.Count;
      if (UsePercentageGroupSize) {
        if (count >= 200) {
          SetGroupSize(count);
        }
      }

      bool maximization = problem.Maximization[0];
      List<int> selected = new List<int>();
      if (qualities.Any(q => !IsValidQuality(q))) {
        throw new ArgumentException("The qualities list contains invalid quality values (either infinity or double.NaN) on which the selector cannot operate.");
      }

      for (int i = 0; i < numSelectedIndividuals; i++) {
        int best = rand.Next(count);
        int index;
        for (int j = 1; j < GroupSize; j++) {
          index = rand.Next(count);
          if (((maximization) && (qualities[index] > qualities[best])) ||
              ((!maximization) && (qualities[index] < qualities[best]))) {
            best = index;
          }
        }
        selected.Add(best);
      }
      return selected;
    }
   
  }
}
