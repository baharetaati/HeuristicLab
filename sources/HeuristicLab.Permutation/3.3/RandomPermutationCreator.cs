#region License Information
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

using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Permutation {
  /// <summary>
  /// An operator which creates a new random permutation of integer values.
  /// </summary>
  [Item("RandomPermutationCreator", "An operator which creates a new random permutation of integer values.")]
  [EmptyStorableClass]
  [Creatable("Test")]
  public sealed class RandomPermutationCreator : SingleSuccessorOperator {
    public LookupParameter<IRandom> RandomParameter {
      get { return (LookupParameter<IRandom>)Parameters["Random"]; }
    }
    public ValueLookupParameter<IntData> LengthParameter {
      get { return (ValueLookupParameter<IntData>)Parameters["Length"]; }
    }
    public LookupParameter<Permutation> PermutationParameter {
      get { return (LookupParameter<Permutation>)Parameters["Permutation"]; }
    }

    public RandomPermutationCreator()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The pseudo random number generator which should be used to initialize the new random permutation."));
      Parameters.Add(new ValueLookupParameter<IntData>("Length", "The length of the new random permutation."));
      Parameters.Add(new LookupParameter<Permutation>("Permutation", "The new random permutation."));
    }

    public override IExecutionSequence Apply() {
      PermutationParameter.ActualValue = new Permutation(LengthParameter.ActualValue.Value, RandomParameter.ActualValue);
      return base.Apply();
    }
  }
}
