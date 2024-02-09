using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Optimization;
using HeuristicLab.Problems.CooperativeProblem;

namespace HeuristicLab.Algorithms.CoevolutionaryAlgorithm {
  [Item("BestSelection", "A selection operator which selects the best quality values")]
  [StorableType("E663EFA7-A7F9-4858-A407-AA15187ED083")]
  public class BestSelection : SelectionStrategy<double> {
    #region Constructors and Cloning
    [StorableConstructor]
    protected BestSelection(StorableConstructorFlag _) : base(_) { }
    public BestSelection() : base() { }
    protected BestSelection(BestSelection other, Cloner cloner) : base(other, cloner) {

    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new BestSelection(this, cloner);
    }
    #endregion
    public override List<int> Select(int numSelectedIndividuals, IRandom rand, List<double> qualities, bool maximization) {
      Console.WriteLine("BestSelection is performed");
      //bool maximization = problem.Maximization[0];
      var selected = new List<int>();

      // Check if all quality values are valid
      if (qualities.Any(q => !IsValidQuality(q))) {
        throw new ArgumentException("The qualities list contains invalid quality values (either infinity or double.NaN) on which the selector cannot operate.");
      }

      // Create a list of indices from 0 to qualities.Count - 1
      var indices = Enumerable.Range(0, qualities.Count).ToList();

      // Sort indices based on quality values and maximization flag
      if (maximization) {
        indices.Sort((i1, i2) => qualities[i2].CompareTo(qualities[i1]));
      } else {
        indices.Sort((i1, i2) => qualities[i1].CompareTo(qualities[i2]));
      }

      // Select the top numSelectedIndividuals indices
      int j = 0;
      for (int i = 0; i < numSelectedIndividuals; i++) {
        selected.Add(indices[j]);
        j++;
        if (j >= indices.Count) j = 0;
      }

      return selected;
    }
    

  }
}
