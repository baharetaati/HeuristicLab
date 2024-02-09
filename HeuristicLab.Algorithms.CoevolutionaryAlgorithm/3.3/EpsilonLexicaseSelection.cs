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
  [Item("EpsilonLexicaseSelection", "A selection strategy for multi-objective symbolic regression.")]
  [StorableType("772084A9-BD1F-40BB-838D-228A83DC7C7D")]
  public class EpsilonLexicaseSelection : SelectionStrategy<double>{
    #region Properties
    [Storable]
    public double Epsilon { get; set; }
    #endregion
    #region Constructors and Cloning
    [StorableConstructor]
    protected EpsilonLexicaseSelection(StorableConstructorFlag _) : base(_) { }
    public EpsilonLexicaseSelection(double epsilon) : base() {

      Epsilon = epsilon;
    }
    protected EpsilonLexicaseSelection(EpsilonLexicaseSelection other, Cloner cloner) : base(other, cloner) {
      Epsilon = other.Epsilon;
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new EpsilonLexicaseSelection(this, cloner);
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
        for (int i = 1; i < count; i++) {
          int curIndex = sortedIndices[i];
          if ((!maximization && (fitnessCase[curIndex] < bestQuality * (1 + Epsilon) )) ||
            (maximization && ((fitnessCase[curIndex] * (1 + Epsilon)) > bestQuality) )) {
            selected.Add(curIndex);
          } else {
            break;
          }
        
        }
      }
      
      
      return selected;
    }
  }
}
