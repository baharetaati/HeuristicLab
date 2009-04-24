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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Xml;
using System.Windows.Forms;

namespace HeuristicLab.Core {
  /// <summary>
  /// Base class for all visual representations.
  /// </summary>
  public partial class ViewBase : UserControl, IView {
    private IItem myItem;
    /// <summary>
    /// Gets or sets the item to represent visually.
    /// </summary>
    /// <remarks>Calls <see cref="OnItemChanged"/>, <see cref="Refresh"/>, 
    /// <see cref="RemoveItemEvents"/> (if the current item is not null) and 
    /// <see cref="AddItemEvents"/> (if the new item is not null) in the setter.</remarks>
    public IItem Item {
      get { return myItem; }
      protected set {
        if (value != myItem) {
          if (myItem != null)
            RemoveItemEvents();
          myItem = value;
          if (myItem != null)
            AddItemEvents();
          OnItemChanged();
          Refresh();
        }
      }
    }
    private string myCaption;
    /// <summary>
    /// Gets or sets the caption of the current instance.
    /// </summary>
    /// <remarks>Call <see cref="OnCaptionChanged"/> in the setter if a new item is set.</remarks>
    public string Caption {
      get { return myCaption; }
      set {
        if (value != myCaption) {
          myCaption = value;
          OnCaptionChanged();
        }
      }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ViewBase"/> with the caption "View".
    /// </summary>
    public ViewBase() {
      InitializeComponent();
      Caption = "View";
    }

    /// <summary>
    /// Removes the eventhandlers from the current instance.
    /// </summary>
    protected virtual void RemoveItemEvents() { }
    /// <summary>
    /// Adds eventhandlers to the current instance.
    /// </summary>
    protected virtual void AddItemEvents() { }

    /// <summary>
    /// Refreshes the current view.
    /// </summary>
    /// <remarks>Creates a new <see cref="MethodInvoker"/> if an invoke is required 
    /// (see <see cref="Control.InvokeRequired"/>.<br/>
    /// Otherwise calls <see cref="UpdateControls"/> and <see cref="Control.Refresh"/> of base class 
    /// <see cref="System.Windows.Forms.UserControl"/>.</remarks>
    public override void Refresh() {
      if (InvokeRequired) {
        Invoke(new MethodInvoker(Refresh));
      } else {
        UpdateControls();
        base.Refresh();
      }
    }
    /// <summary>
    /// Updates the controls with the latest values of the model.
    /// </summary>
    protected virtual void UpdateControls() {
      if (Item == null)
        Caption = "View";
      else
        Caption = "View (" + Item.GetType().Name + ")";
      
    }

    /// <summary>
    /// Occurs when the current item was changed.
    /// </summary>
    public event EventHandler ItemChanged;
    /// <summary>
    /// Fires a new <c>ItemChanged</c> event.
    /// </summary>
    protected virtual void OnItemChanged() {
      if (ItemChanged != null)
        ItemChanged(this, new EventArgs());
    }
    /// <summary>
    /// Occurs when the current caption was changed.
    /// </summary>
    public event EventHandler CaptionChanged;
    /// <summary>
    /// Fires a new <c>CaptionChanged</c> event.
    /// </summary>
    protected virtual void OnCaptionChanged() {
      if (CaptionChanged != null)
        CaptionChanged(this, new EventArgs());
    }

    /// <summary>
    /// Asynchron call of GUI updating.
    /// </summary>
    /// <param name="method">The delegate to invoke.</param>
    protected new void Invoke(Delegate method) {
      // enforce context switch to improve GUI response time
      System.Threading.Thread.Sleep(0);

      // prevent blocking of worker thread in Invoke, if the control is disposed
      IAsyncResult result = BeginInvoke(method);
      while ((!result.AsyncWaitHandle.WaitOne(100, false)) && (!IsDisposed)) { }
      if (!IsDisposed) EndInvoke(result);
    }
    /// <summary>
    /// Asynchron call of GUI updating.
    /// </summary>
    /// <param name="method">The delegate to invoke.</param>
    /// <param name="args">The invoke arguments.</param>
    protected new void Invoke(Delegate method, params object[] args) {
      // enforce context switch to improve GUI response time
      System.Threading.Thread.Sleep(0);

      // prevent blocking of worker thread in Invoke, if the control is disposed
      IAsyncResult result = BeginInvoke(method, args);
      while ((!result.AsyncWaitHandle.WaitOne(100, false)) && (!IsDisposed)) { }
      if (!IsDisposed) EndInvoke(result);
    }
  }
}
