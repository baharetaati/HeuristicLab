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
  [Item("GeneralizedRankSelection", "The generalized rank selection operator selects qualities by rank with a varying focus on better qualities.")]
  [StorableType("C828B056-DCCC-4F9A-B062-CE1F666E6235")]
  public class GeneralizedRankSelection : SelectionStrategy<double> {
    #region Properties
    [Storable]
    public double SelectionPressure { get; private set; }
    #endregion

    #region Constructors and Cloning
    [StorableConstructor]
    protected GeneralizedRankSelection(StorableConstructorFlag _) : base(_) { }
    public GeneralizedRankSelection(double selectionPressure = 2) : base() {

      SelectionPressure = selectionPressure;
    }
    protected GeneralizedRankSelection(GeneralizedRankSelection other, Cloner cloner):base(other, cloner) {
      SelectionPressure = other.SelectionPressure;
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new GeneralizedRankSelection(this, cloner);
    }
    #endregion
    public override List<int> Select(int numSelectedIndividuals, IRandom rand, List<double> qualities, bool maximization) {
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

      var m = qualities.Count;
      for (int i = 0; i < numSelectedIndividuals; i++) {
        double rnd = 1 + rand.NextDouble() * (Math.Pow(m, 1.0 / SelectionPressure) - 1);
        int selIdx = (int)Math.Floor(Math.Pow(rnd, SelectionPressure) - 1);  
        selected.Add(selIdx);
      }
      return selected;
    }
    
  }
}
