using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;

namespace HeuristicLab.Algorithms.CoevolutionaryAlgorithm {
  [StorableType("B4C9101C-C728-421E-8D64-E91D73E79306")]
  public class CrowdedTournamentSelection : DeepCloneable {
    #region Properties
    [Storable]
    public int GroupSize { get; private set; }
    #endregion

    #region Constructors and Cloning
    [StorableConstructor]
    protected CrowdedTournamentSelection(StorableConstructorFlag _) { }
    public CrowdedTournamentSelection(int groupSize = 2) {
      
      GroupSize = groupSize;
    }
    protected CrowdedTournamentSelection(CrowdedTournamentSelection other, Cloner cloner) { 
      
      GroupSize = other.GroupSize;  
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new CrowdedTournamentSelection(this, cloner);
    }
    #endregion

    public List<int> Select(IRandom random, int numSelectedIndividuals, List<int> ranks, List<double> crowdingDistances) {
      var selected = new List<int>();
      for (int i = 0; i < numSelectedIndividuals; i++) {
        int best = random.Next(ranks.Count);
        int index;
        for (int j = 1; j < GroupSize; j++) {
          index = random.Next(ranks.Count);
          if (ranks[best] > ranks[index]
            || ranks[best] == ranks[index]
              && crowdingDistances[best] < crowdingDistances[index]) {
            best = index;
          }
        }
        selected.Add(best);
        
      }
      return selected;
    }


  }
}
