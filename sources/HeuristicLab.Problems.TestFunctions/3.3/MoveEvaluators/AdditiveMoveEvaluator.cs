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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Encodings.RealVectorEncoding;

namespace HeuristicLab.Problems.TestFunctions {
  [Item("AdditiveMoveEvaluator", "Base class for evaluating an additive move.")]
  [StorableClass]
  public abstract class AdditiveMoveEvaluator : SingleSuccessorOperator, ISingleObjectiveTestFunctionAdditiveMoveEvaluator {
    public abstract Type EvaluatorType { get; }
    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Quality"]; }
    }
    public ILookupParameter<DoubleValue> MoveQualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["MoveQuality"]; }
    }
    public ILookupParameter<RealVector> RealVectorParameter {
      get { return (ILookupParameter<RealVector>)Parameters["Point"]; }
    }
    public ILookupParameter<AdditiveMove> AdditiveMoveParameter {
      get { return (ILookupParameter<AdditiveMove>)Parameters["AdditiveMove"]; }
    }

    protected AdditiveMoveEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<DoubleValue>("Quality", "The quality of a TSP solution."));
      Parameters.Add(new LookupParameter<DoubleValue>("MoveQuality", "The evaluated quality of a move on a TSP solution."));
      Parameters.Add(new LookupParameter<RealVector>("Point", "The point to evaluate the move on."));
      Parameters.Add(new LookupParameter<AdditiveMove>("AdditiveMove", "The move to evaluate."));
    }

    public override IOperation Apply() {
      double mq = Evaluate(QualityParameter.ActualValue.Value, RealVectorParameter.ActualValue, AdditiveMoveParameter.ActualValue);
      DoubleValue moveQuality = MoveQualityParameter.ActualValue;
      if (moveQuality == null) {
        MoveQualityParameter.ActualValue = new DoubleValue(mq);
      } else moveQuality.Value = mq;
      return base.Apply();
    }

    protected abstract double Evaluate(double quality, RealVector point, AdditiveMove move);
  }
}
