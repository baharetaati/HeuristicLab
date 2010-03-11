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

using System;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.Permutation {
  [Item("TwoOptMoveMaker", "Peforms a 2-opt move on a given permutation and updates the quality.")]
  [StorableClass(StorableClassType.Empty)]
  public class TwoOptMoveMaker : SingleSuccessorOperator {
    public LookupParameter<DoubleData> QualityParameter {
      get { return (LookupParameter<DoubleData>)Parameters["Quality"]; }
    }
    public LookupParameter<TwoOptMove> MoveParameter {
      get { return (LookupParameter<TwoOptMove>)Parameters["Move"]; }
    }
    public LookupParameter<DoubleData> MoveQualityParameter {
      get { return (LookupParameter<DoubleData>)Parameters["MoveQuality"]; }
    }
    public ILookupParameter<Permutation> PermutationParameter {
      get { return (ILookupParameter<Permutation>)Parameters["Permutation"]; }
    }

    public TwoOptMoveMaker()
      : base() {
      Parameters.Add(new LookupParameter<DoubleData>("Quality", "The quality of the solution."));
      Parameters.Add(new LookupParameter<TwoOptMove>("Move", "The move to evaluate."));
      Parameters.Add(new LookupParameter<DoubleData>("MoveQuality", "The relative quality of the move."));
      Parameters.Add(new LookupParameter<Permutation>("Permutation", "The solution as permutation."));
    }

    public override IOperation Apply() {
      TwoOptMove move = MoveParameter.ActualValue;
      Permutation permutation = PermutationParameter.ActualValue;
      DoubleData moveQuality = MoveQualityParameter.ActualValue;
      DoubleData quality = QualityParameter.ActualValue;

      InversionManipulator.Apply(permutation, move.Index1, move.Index2);
      quality.Value = moveQuality.Value;

      return base.Apply();
    }
  }
}
