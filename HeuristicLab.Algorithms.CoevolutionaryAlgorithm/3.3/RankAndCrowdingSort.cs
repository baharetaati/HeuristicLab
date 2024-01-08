using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Problems.Instances;

namespace HeuristicLab.Algorithms.CoevolutionaryAlgorithm {
  [StorableType("7C04D7BB-4272-4C8A-91F9-041A7144635B")]
  public class RankAndCrowdingSort : DeepCloneable {


    #region Constructors and Cloning
    [StorableConstructor]
    protected RankAndCrowdingSort(StorableConstructorFlag _) { }
    public RankAndCrowdingSort() {}
    protected RankAndCrowdingSort(RankAndCrowdingSort other, Cloner cloner) : base(other, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new RankAndCrowdingSort(this, cloner);
    }
    #endregion

    public List<List<Tuple<ISymbolicExpressionTree, double[]>>> FastNonDominatedSorting(List<ISymbolicExpressionTree> solutions, List<double[]> qualities, bool[] maximization, out int[] rank) {
      //var qualitiesArray = qualities.ToArray();
      var qualitiesArray = qualities.Select(x => x.ToArray()).ToArray();
      var paretoFronts = DominationCalculator<ISymbolicExpressionTree>.CalculateAllParetoFronts(solutions.ToArray(), qualitiesArray, maximization, out rank);
      return paretoFronts;
    }
    public List<double> CalculateCrowdingDistance(List<double[]> qualities, bool[] maximization) {
      double epsilon = 1e-6;
      int frontSize = qualities.Count;
      int objectiveCount = qualities[0].Length;

      List<double> crowdingDistances = new List<double>(frontSize);
      for (int i = 0; i < frontSize; i++) {
        crowdingDistances.Add(0.0);
      }

      for (int m = 0; m < objectiveCount; m++) {
        
        List<int> indexes = Enumerable.Range(0, frontSize).ToList();
        indexes.Sort((i, j) => qualities[i][m].CompareTo(qualities[j][m]));
        //if (maximization[m]) {
        //  indexes.Sort((i, j) => qualities[j][m].CompareTo(qualities[i][m]));
        //}
        //else {
        //  indexes.Sort((i, j) => qualities[i][m].CompareTo(qualities[j][m]));
        //}

        double minQuality = qualities[indexes[0]][m];
        double maxQuality = qualities[indexes[frontSize - 1]][m];
        crowdingDistances[indexes[0]] = double.MaxValue;
        crowdingDistances[indexes[frontSize - 1]] = double.MaxValue;

        for (int i = 1; i < frontSize - 1; i++) {
          crowdingDistances[indexes[i]] += (qualities[indexes[i + 1]][m] - qualities[indexes[i - 1]][m]) / (maxQuality - minQuality + epsilon);
        }
      }
      return crowdingDistances;

    }
    public List<int> RankBasedCrowdedComparisonSorter(List<double> distances) {
      List<int> indexes = Enumerable.Range(0, distances.Count).ToList();
      indexes.Sort((i, j) => -distances[i].CompareTo(distances[j]));
      return indexes;
    }
    
  }
}
