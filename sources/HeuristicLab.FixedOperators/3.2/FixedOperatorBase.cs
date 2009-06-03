﻿#region License Information
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
using HeuristicLab.Core;
using HeuristicLab.Operators;
using HeuristicLab.Data;
using System.Threading;
using System.Diagnostics;
using System.Text;

namespace HeuristicLab.FixedOperators {
  class FixedOperatorBase : CombinedOperator {
    protected ItemList<IOperation> persistedOperations;
    protected Stack<IOperation> executionStack;

    /// <summary>
    /// Execution pointer shows which command actually is executed
    /// </summary>
    protected int executionPointer;

    /// <summary>
    /// Execution pointer if execution was aborted previously
    /// </summary>
    protected IntData persistedExecutionPointer;

    protected int tempExePointer;
    protected int tempPersExePointer;

    /// <summary>
    /// Current operator in execution.
    /// </summary>
    protected IOperator currentOperator;

    public FixedOperatorBase()
      : base() {
      executionStack = new Stack<IOperation>();
    } // FixedOperatorBase

    private bool IsExecuted() {
      return persistedExecutionPointer.Data > executionPointer;
    } // AlreadyExecuted

    protected void ExecuteExitable(IOperator op, IScope scope) {

    } // ExecuteExitable

    protected void SetRegion(string region) {

    } // SetRegion

    protected virtual void Execute(IOperator op, IScope scope) {
      if (!IsExecuted()) {
        ExecuteOperation(op, scope);
        persistedExecutionPointer.Data++;
      } // if not executed
      executionPointer++;

      if (Canceled)
        throw new CancelException();
    } // Execute

    protected void ExecuteOperation(IOperator op, IScope scope) {
      IOperation operation;
       if (persistedOperations.Count == 0) {
        currentOperator = op;
        operation = op.Execute(scope);
        if (operation != null) {
          executionStack.Push(operation);
        }
      } else {
        executionStack = new Stack<IOperation>(persistedOperations);  
      }

      while (executionStack.Count > 0) {
        operation = executionStack.Pop();
        if (operation is AtomicOperation) {
          AtomicOperation atomicOperation = (AtomicOperation)operation;
          IOperation next = null;
          try {
            currentOperator = atomicOperation.Operator;
            next = currentOperator.Execute(atomicOperation.Scope);
          }
          catch (Exception) {
            throw new InvalidOperationException("Invalid Operation occured in FixedBase.Execute");
          }
          if (next != null)
            executionStack.Push(next);
        } else if (operation is CompositeOperation) {
          CompositeOperation compositeOperation = (CompositeOperation)operation;
          for (int i = compositeOperation.Operations.Count - 1; i >= 0; i--)
            executionStack.Push(compositeOperation.Operations[i]);
        } // else if

        if (Canceled && executionStack.Count > 0) {
          SaveExecutionStack(executionStack);
          throw new CancelException();
        }
      } // while
    } // ExecuteOperation

    private void SaveExecutionStack(Stack<IOperation> stack) {
      persistedOperations = new ItemList<IOperation>();
      persistedOperations.AddRange(stack.ToArray());
    } // SaveExecutionStack

    public override IOperation Apply(IScope scope) {
      base.Apply(scope);
      try {
        persistedExecutionPointer = scope.GetVariableValue<IntData>("ExecutionPointer", false);
      }
      catch (Exception) {
        persistedExecutionPointer = new IntData(0);
        scope.AddVariable(new Variable("ExecutionPointer", persistedExecutionPointer));
      }

      try {
        persistedOperations = scope.GetVariableValue<ItemList<IOperation>>("ExecutionStack", false);
      }
      catch (Exception) {
        persistedOperations = new ItemList<IOperation>();
        scope.AddVariable(new Variable("ExecutionStack", persistedOperations));
      }

      executionPointer = 0;

      for (int i = 0; i < SubOperators.Count; i++) {
        if (scope.GetVariable(SubOperators[i].Name) != null)
          scope.RemoveVariable(SubOperators[i].Name);
        scope.AddVariable(new Variable(SubOperators[i].Name, SubOperators[i]));
      }

      return null;
    } // Apply

    public override void Abort() {
      base.Abort();
      currentOperator.Abort();
    } // Abort

    /// <summary>
    /// Saves the value of the execution pointers into temp variables
    /// </summary>
    protected void SaveExecutionPointer() {
      tempExePointer = executionPointer;
      tempPersExePointer = persistedExecutionPointer.Data;
    } // SaveExecutionPointer

    protected void SetExecutionPointerToLastSaved() {
      if (executionPointer != persistedExecutionPointer.Data)
        persistedExecutionPointer.Data = tempPersExePointer;
      else
        persistedExecutionPointer.Data = tempExePointer;
      executionPointer = tempExePointer;
    } // SetExecutionPointerToLastSaved


    protected void ResetExecutionPointer() {
      executionPointer = 0;
      persistedExecutionPointer.Data = 0;
    } // ResetExecutionPointer
  } // class FixedOperatorBase

  class CancelException : Exception {

  } // class CancelException
} // namespace HeuristicLab.FixedOperators
