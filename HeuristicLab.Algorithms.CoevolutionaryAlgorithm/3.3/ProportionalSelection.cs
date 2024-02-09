using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Problems.CooperativeProblem;
using HeuristicLab.Selection;

namespace HeuristicLab.Algorithms.CoevolutionaryAlgorithm {
  [Item("ProportionalSelection", "A quality proportional selection operator for symbolic expression trees in a basic algorithm.")]
  [StorableType("239BD90F-DA67-4BA1-9921-E65525880FE4")]

  public class ProportionalSelection : SelectionStrategy<double>{
    #region Properties
    [Storable]
    public bool ApplyWindowing { get; private set; }

    #endregion

    #region Constructors and Cloning
    [StorableConstructor]
    protected ProportionalSelection(StorableConstructorFlag _) : base(_) { }
    public ProportionalSelection(bool applyWindowing = true) : base() {
      
      ApplyWindowing = applyWindowing;
      
      
    }
    protected ProportionalSelection(ProportionalSelection other, Cloner cloner) : base(other, cloner) {
      ApplyWindowing = other.ApplyWindowing;
      

    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new ProportionalSelection(this, cloner);
    }

    #endregion
    public override List<int> Select(int numSelectedIndividuals, IRandom rand, List<double> qualities, bool maximization) {
      List<int> selected = new List<int>();
      double minQuality = double.MaxValue;
      double maxQuality = double.MinValue;
      //bool maximization = problem.Maximization[0];
      foreach (var quality in qualities) {
        if (!IsValidQuality(quality)) throw new ArgumentException("The qualities list contain invalid quality values (either infinity or double.NaN) on which the selector cannot operate.");
        if (quality < minQuality) minQuality = quality;
        if (quality > maxQuality) maxQuality = quality;
      }
      if (minQuality == maxQuality) {  // all quality values are equal
        qualities = qualities.Select(x => 1.0).ToList();
      } else {
        if (ApplyWindowing) {
          if (maximization)
            qualities = qualities.Select(x => x - minQuality).ToList();
          else
            qualities = qualities.Select(x => maxQuality - x).ToList();
        } else {
          if (minQuality < 0.0) throw new InvalidOperationException("Proportional selection without windowing does not work with quality values < 0.");
          if (!maximization) {
            double limit = Math.Min(maxQuality * 2, double.MaxValue);
            qualities = qualities.Select(x => limit - x).ToList();
          }
        }
      }
      
      double qualitySum = qualities.Sum();
      for (int i = 0; i < numSelectedIndividuals; i++) {
          double selectedQuality = rand.NextDouble() * qualitySum;
          int index = 0;
          double currentQuality = qualities[index];
          while (currentQuality < selectedQuality) {
            index++;
            currentQuality += qualities[index];
          }
          selected.Add(index);
      }
      return selected;
    }
  }
}
