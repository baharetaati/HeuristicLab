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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Optimization.Operators {
  /// <summary>
  /// Modifies the value by exponential fall (steep fall initially, slow fall to the end) or rise (slow rise initially, fast rise to the end).
  /// </summary>
  [Item("ExponentialDiscreteDoubleValueModifier", "Modifies the value by exponential fall (steep fall initially, slow fall to the end) or rise (slow rise initially, fast rise to the end).")]
  [StorableClass]
  public class ExponentialDiscreteDoubleValueModifier : DiscreteDoubleValueModifier {
    protected override double Modify(double value, double startValue, double endValue, int index, int startIndex, int endIndex) {
      double b = Math.Pow(endValue / startValue, 1.0 / (endIndex - startIndex));
      return startValue * Math.Pow(b, index - startIndex);
    }
  }
}
