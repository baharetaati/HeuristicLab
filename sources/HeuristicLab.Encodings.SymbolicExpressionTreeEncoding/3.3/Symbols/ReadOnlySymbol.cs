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
using System.Collections.Generic;
using System.Text;
using HeuristicLab.Common;
using HeuristicLab.Core;
using System.Linq;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Data;
using HeuristicLab.Parameters;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Symbols {
  [StorableClass]
  [Item("ReadOnlySymbol", "Represents a symbol in a symbolic function tree that cannot be modified.")]
  public abstract class ReadOnlySymbol : Symbol {
    #region Properties
    [Storable]
    private double initialFrequency;
    public double InitialFrequency {
      get { return initialFrequency; }
      set { throw new NotSupportedException(); }
    }
    #endregion

    public override bool CanChangeName {
      get { return false; }
    }
    public override bool CanChangeDescription {
      get { return false; }
    }

    protected ReadOnlySymbol() : base() { }
    protected ReadOnlySymbol(string name, string description) : base(name, description) { }
    [StorableConstructor]
    protected ReadOnlySymbol(bool deserializing) : base(deserializing) { }
  }
}
