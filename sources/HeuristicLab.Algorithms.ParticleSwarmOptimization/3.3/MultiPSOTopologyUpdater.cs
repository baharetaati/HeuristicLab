﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Operators;
using HeuristicLab.Common;
using HeuristicLab.Parameters;
using HeuristicLab.Encodings.IntegerVectorEncoding;
using HeuristicLab.Data;

namespace HeuristicLab.Algorithms.ParticleSwarmOptimization {
  [Item("Multi PSO Topology Initializer/Updater", "Splits swarm into swarmsize / (nrOfConnections + 1) non-overlapping sub-swarms. Swarms are re-grouped every regroupingPeriod iteration. The operator is implemented as described in Liang, J.J. and Suganthan, P.N 2005. Dynamic multi-swarm particle swarm optimizer. IEEE Swarm Intelligence Symposium, pp. 124-129")]
  [StorableClass]
  public class MultiPSOTopologyUpdater : SingleSuccessorOperator, ITopologyUpdater, ITopologyInitializer {
    #region Parameters
    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters["Random"]; }
    }
    public IValueLookupParameter<IntValue> NrOfConnectionsParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["NrOfConnections"]; }
    }
    public ILookupParameter<IntValue> SwarmSizeParameter {
      get { return (ILookupParameter<IntValue>)Parameters["SwarmSize"]; }
    }
    public IScopeTreeLookupParameter<IntegerVector> NeighborsParameter {
      get { return (IScopeTreeLookupParameter<IntegerVector>)Parameters["Neighbors"]; }
    }
    public ILookupParameter<IntValue> CurrentIterationParameter {
      get { return (ILookupParameter<IntValue>)Parameters["CurrentIteration"]; }
    }
    public IValueLookupParameter<IntValue> RegroupingPeriodParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["RegroupingPeriod"]; }
    }
    #endregion

    #region Parameter Values
    public IRandom Random {
      get { return RandomParameter.ActualValue; }
    }
    public int NrOfConnections {
      get { return NrOfConnectionsParameter.ActualValue.Value; }
    }
    public int SwarmSize {
      get { return SwarmSizeParameter.ActualValue.Value; }
    }
    public ItemArray<IntegerVector> Neighbors {
      get { return NeighborsParameter.ActualValue; }
      set { NeighborsParameter.ActualValue = value; }
    }
    public int CurrentIteration {
      get { return CurrentIterationParameter.ActualValue.Value; }
    }
    public int RegroupingPeriod {
      get { return RegroupingPeriodParameter.ActualValue.Value; }
    }
    #endregion

    public MultiPSOTopologyUpdater()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "A random number generator."));
      Parameters.Add(new ValueLookupParameter<IntValue>("NrOfConnections", "Nr of connected neighbors.", new IntValue(3)));
      Parameters.Add(new LookupParameter<IntValue>("SwarmSize", "Number of particles in the swarm."));
      Parameters.Add(new ScopeTreeLookupParameter<IntegerVector>("Neighbors", "The list of neighbors for each particle."));
      Parameters.Add(new LookupParameter<IntValue>("CurrentIteration", "The current iteration of the algorithm."));
      Parameters.Add(new ValueLookupParameter<IntValue>("RegroupingPeriod", "Update interval (=iterations) for regrouping of neighborhoods.", new IntValue(5)));
    }

    // Splits the swarm into non-overlapping sub swarms
    public override IOperation Apply() {
      if (CurrentIteration % RegroupingPeriod == 0) {
        ItemArray<IntegerVector> neighbors = new ItemArray<IntegerVector>(SwarmSize);
        Dictionary<int, List<int>> neighborsPerParticle = new Dictionary<int, List<int>>();
        for (int i = 0; i < SwarmSize; i++) {
          neighborsPerParticle.Add(i, new List<int>());
        }

        // partition swarm into groups
        Dictionary<int, List<int>> groups = new Dictionary<int, List<int>>();
        int groupId = 0;
        var numbers = Enumerable.Range(0, SwarmSize).ToList();
        for (int i = 0; i < SwarmSize; i++) {
          int nextParticle = numbers[Random.Next(0, numbers.Count)];
          if (!groups.ContainsKey(groupId)) {
            groups.Add(groupId, new List<int>()); 
          }
          groups[groupId].Add(nextParticle);
          if (groups[groupId].Count - 1 == NrOfConnections) {
            groupId++;
          }
          numbers.Remove(nextParticle); 
        }

        // add neighbors to each particle
        foreach (List<int> group in groups.Values) {
          foreach (int sib1 in group) {
            foreach (int sib2 in group) {
              if (sib1 != sib2 && !neighborsPerParticle[sib1].Contains(sib2)) {
                neighborsPerParticle[sib1].Add(sib2);
              }
            }
          }
        }

        for (int particle = 0; particle < neighborsPerParticle.Count; particle++) {
          neighbors[particle] = new IntegerVector(neighborsPerParticle[particle].ToArray());
        }
        Neighbors = neighbors;
      }
      return base.Apply();
    }

    [StorableConstructor]
    protected MultiPSOTopologyUpdater(bool deserializing) : base(deserializing) { }

    #region Cloning
    public override IDeepCloneable Clone(Cloner cloner) {
      return new MultiPSOTopologyUpdater(this, cloner);
    }

    protected MultiPSOTopologyUpdater(MultiPSOTopologyUpdater original, Cloner cloner)
      : base(original, cloner) {
    }
    #endregion
  }
}
