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
  [Item("RandomSelection", "A random selection operator")]
  [StorableType("0B6AFF23-B0F5-466D-96C6-E9B278A9123B")]
  public class RandomSelection : SelectionStrategy<double> {
    #region Constructors and Cloning
    [StorableConstructor]
    protected RandomSelection(StorableConstructorFlag _) : base(_) { }
    public RandomSelection() : base() { }
    protected RandomSelection(RandomSelection other, Cloner cloner) : base(other, cloner) {

    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new RandomSelection(this, cloner);
    }
    #endregion
    public override List<int> Select(int numSelectedIndividuals, IRandom rand, List<double> qualities, bool maximization) {
      Console.WriteLine("RandomSelection is performed");
      var selected = new List<int>();
      var count = qualities.Count;
      for (int i = 0; i < numSelectedIndividuals; i++) {
        selected.Add(rand.Next(count));
      }
      return selected;
    }
  }
}
