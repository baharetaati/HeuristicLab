using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;

namespace HeuristicLab.Algorithms.CoevolutionaryAlgorithm {
  [Item("AdaptiveEpsilonLexicaseSelection", "An adaptive selection strategy for multi-objective symbolic regression.")]
  [StorableType("B4CEBB36-C350-4A46-97DD-F495AC1018F0")]
  public class AdaptiveEpsilonLexicaseSelection : SelectionStrategy<double>{
    #region Constructors and Cloning
    [StorableConstructor]
    protected AdaptiveEpsilonLexicaseSelection(StorableConstructorFlag _) : base(_) { }
    public AdaptiveEpsilonLexicaseSelection() : base() {}
    protected AdaptiveEpsilonLexicaseSelection(AdaptiveEpsilonLexicaseSelection other, Cloner cloner) : base(other, cloner) {}
    public override IDeepCloneable Clone(Cloner cloner) {
      return new AdaptiveEpsilonLexicaseSelection(this, cloner);
    }

    #endregion
    public override List<int> Select(int numSelectedIndividuals, IRandom rand, List<double> qualities, bool maximization) {
      
      List<int> selected = new List<int>();
      List<double> fitnessCase = qualities;
      List<int> sortedIndices = new List<int>();

      if (maximization) {
        sortedIndices = fitnessCase
          .Select((val, index) => new { Index = index, Value = val })
          .OrderByDescending(item => item.Value)
          .Select(item => item.Index)
          .ToList();
      } else {
        sortedIndices = fitnessCase
         .Select((val, index) => new { Index = index, Value = val })
         .OrderBy(item => item.Value)
         .Select(item => item.Index)
         .ToList();
      }
      int best = sortedIndices[0];
      int count = sortedIndices.Count;
      double bestQuality = fitnessCase[best];
      selected.Add(best);
      if (count > 1) {
        var adaptiveEpsilon = CalculateMAD(fitnessCase.ToArray());
        //Console.WriteLine($"adaptiveEpsilon = {adaptiveEpsilon}");
        for (int i = 1; i < count; i++) {
          int curIndex = sortedIndices[i];
          if ((!maximization && (fitnessCase[curIndex] < bestQuality * (1 + adaptiveEpsilon))) ||
            (maximization && ((fitnessCase[curIndex] * (1 + adaptiveEpsilon)) > bestQuality))) {
            selected.Add(curIndex);
          } else {
            break;
          }

        }
      }

      return selected;
    }
    public static double CalculateMAD(double[] data) {
      var median = CalculateMedian(data);
      double[] absoluteDeviations = new double[data.Length];
      for (int d = 0; d < data.Length; d++) {
        absoluteDeviations[d] = Math.Abs(data[d] - median);
      }

      // Calculate the median of the absolute deviations
      double mad = CalculateMedian(absoluteDeviations);
      return mad;
    }
    public static double CalculateMedian(double[] values) {
      Array.Sort(values);
      int n = values.Length;
      if (n % 2 == 0) {
        // If the number of values is even, take the average of the two middle values
        return ((double)values[n / 2 - 1] + values[n / 2]) / 2.0;
      } else {
        // If the number of values is odd, take the middle value
        return values[n / 2];
      }

    }
  }
}
