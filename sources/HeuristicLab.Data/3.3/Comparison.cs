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

using System;
using System.Drawing;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Data {
  [Item("Comparison", "Represents a comparison.")]
  [StorableClass]
  public class Comparison : ValueTypeValue<ComparisonType>, IComparable {
    public override Image ItemImage {
      get { return HeuristicLab.Common.Resources.VS2008ImageLibrary.Enum; }
    }

    public Comparison() : base() { }
    public Comparison(ComparisonType value) : base(value) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      Comparison clone = new Comparison(value);
      cloner.RegisterClonedObject(this, clone);
      clone.ReadOnlyView = ReadOnlyView;
      return clone;
    }

    public virtual int CompareTo(object obj) {
      Comparison other = obj as Comparison;
      if (other != null)
        return Value.CompareTo(other.Value);
      else
        return Value.CompareTo(obj);
    }
  }
}
