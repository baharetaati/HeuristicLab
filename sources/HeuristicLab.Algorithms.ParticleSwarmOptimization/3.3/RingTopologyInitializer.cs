﻿#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.IntegerVectorEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
namespace HeuristicLab.Algorithms.ParticleSwarmOptimization {

  [Item("Ring Topology Initializer", "Connected every particle with its preceeding and its following particle.")]
  [StorableClass]
  public class RingTopologyInitializer : TopologyInitializer {

    #region Construction & Cloning
    public RingTopologyInitializer() { }
    [StorableConstructor]
    protected RingTopologyInitializer(bool deserializing) : base(deserializing) { }
    protected RingTopologyInitializer(RingTopologyInitializer original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new RingTopologyInitializer(this, cloner);
    }
    #endregion

    public override IOperation Apply() {
      ItemArray<IntegerVector> neighbors = new ItemArray<IntegerVector>(SwarmSize);
      for (int i = 0; i<SwarmSize; i++) {
        neighbors[i] = new IntegerVector(new[] { (SwarmSize + i-1) % SwarmSize, (i+1) % SwarmSize });
      }
      Neighbors = neighbors;
      return base.Apply();
    }

  }
}
