using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Problems.TestFunctions.MultiObjective;

namespace HeuristicLab.Algorithms.CoevolutionaryAlgorithm {
  public static class HypervolumeCalculation {
    public static double Calculate(IEnumerable<double[]> front, double[] referencePoint, bool[] maximization, bool adjacentPointsConsideration = true) {
      front = NonDominatedSelect.GetDominatingVectors(front, referencePoint, maximization, false);
      if (!front.Any()) throw new ArgumentException("No point in the front dominates the referencePoint");

      if (maximization.Length == 2)
        return Calculate2D(front, referencePoint, maximization, adjacentPointsConsideration);
      else
        throw new NotImplementedException("Hypervolume calculation for more than two dimensions is not supported.");

    }

    private static double Calculate2D(IEnumerable<double[]> front, double[] referencePoint, bool[] maximization, bool adjacentPointsConsideration=true) {
      if (front == null) throw new ArgumentNullException("Front must not be null.");
      if (!front.Any()) throw new ArgumentException("Front must not be empty.");

      if (referencePoint == null) throw new ArgumentNullException("ReferencePoint must not be null.");
      if (referencePoint.Length != 2) throw new ArgumentException("ReferencePoint must have exactly two dimensions.");

      double[][] set = front.ToArray();
      if (set.Any(s => s.Length != 2)) throw new ArgumentException("Points in front must have exactly two dimensions.");

      bool descending = maximization[0];
      if (descending)
        set = set.OrderByDescending(point => point[0]).ToArray();
      else
        set = set.OrderBy(point => point[0]).ToArray();

      double sum = 0.0;
      if (adjacentPointsConsideration) {
        for (int i = 0; i < set.Length - 1; i++) {
          sum += Math.Abs((set[i][0] - set[i + 1][0])) * Math.Abs((set[i][1] - referencePoint[1]));
        }

        double[] lastPoint = set[set.Length - 1];
        sum += Math.Abs(lastPoint[0] - referencePoint[0]) * Math.Abs(lastPoint[1] - referencePoint[1]);
      } else {
        for (int i = 0; i < set.Length; i++) {
          sum += Math.Abs((set[i][0] - referencePoint[0])) * Math.Abs((set[i][1] - referencePoint[1]));
        }
      }


      return sum;
    }
  }
}
