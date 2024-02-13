using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using HEAL.Attic;
using System.Numerics;
using HeuristicLab.Core;

namespace HeuristicLab.Algorithms.CoevolutionaryAlgorithm {
  [StorableType("C2B8E6B1-0385-4AB4-9A52-44714268B530")]
  public static class DasAndDennisTechnique {
        
    private static int Factorial(int n) {
      if (n == 0 || n == 1)
        return 1;
      else
        return n * Factorial(n - 1);
    }
    private static int BinomialCoefficient(int n, int k) {
      return (int)Factorial(n) / (Factorial(k) * Factorial(n - k));
    }
    public static int GetNumberOfPoints(int number_of_partitions, int dimension) {
      return (int)BinomialCoefficient(number_of_partitions + dimension - 1, dimension - 1);
    }
    private static double[] GetFirstLevel(int number_of_partitions) {
      var first_level = new double[number_of_partitions + 1];
      double step_size = 1.0 / number_of_partitions;
      for (int i = 0; i <= number_of_partitions; i++) {
        first_level[i] = (double)(i * step_size);
      }
      return first_level;
    }
    public static List<double[]> GetWeightVectors(int number_of_partitions, int dimension) {
      var weightVec = new List<double[]>();
      var firstLevel = GetFirstLevel(number_of_partitions);
      for (int i = 0; i < firstLevel.Length; i++) {
        var weight = new double[dimension];
        weight[0] = firstLevel[i];
        weight[1] = 1.0 - firstLevel[i];
        weightVec.Add(weight);
      }
      return weightVec;
    }
    //private List<List<double>> GetGenericLevel(List<List<double>> first_level, List<List<List<double>>> previous_level) {
    //  var next_level = new List<List<double>>();
    //  foreach (var i in previous_level) {
    //    foreach (var j in i[1]) {
    //      var values = new List<double>();
    //      for (int k = 0; k < first_level.Count - j.Count - i[0].Count; k++) {
    //        values.Add(first_level[k][0]);
    //      }
    //      next_level.Add(new List<double>(i[0]));
    //      next_level[next_level.Count - 1].AddRange(j);
    //    }
    //  }
    //  return next_level;
    //}
    //private List<List<double>> GetLastLevel(List<List<double>> previous_level) {
    //  var last_level = new List<List<double>>();
    //  foreach (var i in previous_level) {
    //    foreach (var j in i) {
    //      var new_vector = new List<double>(j);
    //      new_vector.Add(1.0 - j.Sum());
    //      last_level.Add(new_vector);
    //    }
    //  }
    //  return last_level;
    //}



  }
}
