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
using HeuristicLab.Core;
using System.Threading;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.SequentialEngine {
  /// <summary>
  /// Represents an engine that executes its steps sequentially, also if they could be executed 
  /// in parallel.
  /// </summary>
  [EmptyStorableClass]
  public class SequentialEngine : EngineBase, IEditable {
    private IOperator currentOperator;

    /// <summary>
    /// Aborts the engine execution.
    /// </summary>
    /// <remarks>Calls <see cref="EngineBase.Abort"/> of base class <see cref="EngineBase"/> and
    /// <see cref="IOperator.Abort"/> of the current <see cref="IOperator"/>.</remarks>
    public override void Abort() {
      base.Abort();
      if (currentOperator != null)
        currentOperator.Abort();
    }

    /// <summary>
    /// Deals with the next operation, if it is an <see cref="AtomicOperation"/> it is executed,
    /// if it is a <see cref="CompositeOperation"/> its single operations are pushed on the execution stack.
    /// </summary>
    /// <remarks>If an error occurs during the execution the operation is aborted and the operation
    /// is pushed on the stack again.<br/>
    /// If the execution was successful <see cref="EngineBase.OnOperationExecuted"/> is called.</remarks>
    protected override void ProcessNextOperation() {
      IOperation operation = myExecutionStack.Pop();
      if (operation is AtomicOperation) {
        AtomicOperation atomicOperation = (AtomicOperation)operation;
        IOperation next = null;
        try {
          currentOperator = atomicOperation.Operator;
          next = atomicOperation.Operator.Execute(atomicOperation.Scope);
        }
        catch (Exception ex) {
          // push operation on stack again
          myExecutionStack.Push(atomicOperation);
          Abort();
          ThreadPool.QueueUserWorkItem(delegate(object state) { OnExceptionOccurred(ex);});
        }
        if (next != null)
          myExecutionStack.Push(next);
        OnOperationExecuted(atomicOperation);
        if (atomicOperation.Operator.Breakpoint) Abort();
      } else if (operation is CompositeOperation) {
        CompositeOperation compositeOperation = (CompositeOperation)operation;
        for (int i = compositeOperation.Operations.Count - 1; i >= 0; i--)
          myExecutionStack.Push(compositeOperation.Operations[i]);
      }
    }
  }
}
