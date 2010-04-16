#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Xml;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Data;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  public abstract class SymbolicExpressionTreeTerminalNode : SymbolicExpressionTreeNode {
    private static List<SymbolicExpressionTreeNode> emptyList = new List<SymbolicExpressionTreeNode>();

    protected SymbolicExpressionTreeTerminalNode(Symbol symbol) : base(symbol) { }
    protected SymbolicExpressionTreeTerminalNode(SymbolicExpressionTreeTerminalNode original)
      : base(original) {
    }

    public override void AddSubTree(SymbolicExpressionTreeNode tree) {
      throw new NotSupportedException();
    }
    public override void InsertSubTree(int index, SymbolicExpressionTreeNode tree) {
      throw new NotSupportedException();
    }
    public override void RemoveSubTree(int index) {
      throw new NotSupportedException();
    }
    public override IList<SymbolicExpressionTreeNode> SubTrees {
      get {
        return SymbolicExpressionTreeTerminalNode.emptyList;
      }
    }
  }
}
