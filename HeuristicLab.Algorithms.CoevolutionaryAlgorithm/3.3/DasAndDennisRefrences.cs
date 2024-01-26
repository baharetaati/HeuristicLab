using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;

namespace HeuristicLab.Algorithms.CoevolutionaryAlgorithm {
  public static class DasAndDennisRefrences {
    static HashSet<List<double>> hull = new HashSet<List<double>>();
    private static int findSide(List<double> p1, List<double> p2, List<double> p) {
      double val = (p[1] - p1[1]) * (p2[0] - p1[0]) - (p2[1] - p1[1]) * (p[0] - p1[0]);

      if (val > 0) return 1;
      if (val < 0) return -1;
      return 0;
    }

    private static double lineDist(List<double> p1, List<double> p2, List<double> p) {
      return Math.Abs((p[1] - p1[1]) * (p2[0] - p1[0]) - (p2[1] - p1[1]) * (p[0] - p1[0]));
    }
    public static void quickHull(List<List<double>> a, int n, List<double> p1, List<double> p2, int side) {
      int ind = -1;
      double max_dist = 0;

      for (int i = 0; i < n; i++) {
        double temp = lineDist(p1, p2, a[i]);
        if (findSide(p1, p2, a[i]) == side && temp > max_dist) {
          ind = i;
          max_dist = temp;
        }
      }

      if (ind == -1) {
        hull.Add(p1);
        hull.Add(p2);
        return;
      }

      quickHull(a, n, a[ind], p1, -findSide(a[ind], p1, p2));
      quickHull(a, n, a[ind], p2, -findSide(a[ind], p2, p1));
    }
    public static HashSet<List<double>> InitializeReferenceVectors(int numObjectives, int H, IRandom random) {
      if (numObjectives < 1 || H < 1) {
        Console.WriteLine("Initialization not possible");
        return null;
      }
      int numInitialVectors = CalculateCombination(H + numObjectives - 1, numObjectives - 1);

      // Randomly choose the first set of reference vectors
      List<List<double>> initialVectors = new List<List<double>>();
      for (int i = 0; i < numInitialVectors; i++) {
        List<double> randomVector = new List<double>
    {
                random.NextDouble(), // Random value between 0 and 1
                random.NextDouble()  // Random value between 0 and 1
            };
        initialVectors.Add(randomVector);
      }
      // Find the convex hull of the initial vectors using the QuickHull algorithm
      foreach (var vector in initialVectors) {
        hull.Add(vector);
      }
      int min_x = 0, max_x = 0;
      for (int i = 1; i < numInitialVectors; i++) {
        if (initialVectors[i][0] < initialVectors[min_x][0]) min_x = i;
        if (initialVectors[i][0] > initialVectors[max_x][0]) max_x = i;
      }
      quickHull(initialVectors, numInitialVectors, initialVectors[min_x], initialVectors[max_x], 1);
      quickHull(initialVectors, numInitialVectors, initialVectors[min_x], initialVectors[max_x], -1);
      Console.WriteLine("quickHull is performed");
      return hull;
    }
    static int CalculateCombination(int n, int k) {
      if (k > n || k < 0) {
        return 0;
      }

      int result = 1;
      for (int i = 1; i <= k; i++) {
        result *= n - i + 1;
        result /= i;
      }

      return result;
    }
  }
}
