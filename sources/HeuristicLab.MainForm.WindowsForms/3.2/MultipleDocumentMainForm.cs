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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HeuristicLab.MainForm.WindowsForms {
  public partial class MultipleDocumentMainForm : MainForm {
    public MultipleDocumentMainForm()
      : base() {
      InitializeComponent();
    }

    public MultipleDocumentMainForm(Type userInterfaceType)
      : base(userInterfaceType) {
      InitializeComponent();
    }

    protected override void AdditionalCreationOfGUIElements() {
      base.AdditionalCreationOfGUIElements();
      ToolStripMenuItem window = new ToolStripMenuItem("Windows");
      this.menuStrip.MdiWindowListItem = window;
      this.menuStrip.Items.Add(window);
    }

    protected override void Show(IView view, bool firstTimeShown) {
      if (InvokeRequired) Invoke((Action<IView, bool>)Show, view, firstTimeShown);
      else {
        base.Show(view, firstTimeShown);
        if (firstTimeShown)
          this.GetForm(view).Show();
        else {
          this.GetForm(view).Visible = true;
          this.GetForm(view).Activate();
        }
      }
    }

    protected override void Hide(IView view) {
      if (InvokeRequired) Invoke((Action<IView>)Hide, view);
      else {
        base.Hide(view);
        this.GetForm(view).Hide();
      }
    }

    protected override Form CreateForm(IView view) {
      Form form = new DocumentForm(view);
      form.MdiParent = this;
      return form;
    }
  }
}
