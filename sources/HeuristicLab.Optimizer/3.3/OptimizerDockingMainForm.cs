﻿#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Optimizer {
  internal partial class OptimizerDockingMainForm : DockingMainForm {

    private Clipboard<IItem> clipboard;
    public Clipboard<IItem> Clipboard {
      get { return clipboard; }
    }

    public OptimizerDockingMainForm()
      : base() {
      InitializeComponent();
    }
    public OptimizerDockingMainForm(Type userInterfaceItemType)
      : base(userInterfaceItemType) {
      InitializeComponent();
    }
    public OptimizerDockingMainForm(Type userInterfaceItemType, bool showContentInViewHost)
      : this(userInterfaceItemType) {
      this.ShowContentInViewHost = showContentInViewHost;
    }

    protected override void OnInitialized(EventArgs e) {
      base.OnInitialized(e);

      ContentManager.Initialize(new PersistenceContentManager());

      clipboard = new Clipboard<IItem>();
      clipboard.Dock = DockStyle.Left;
      clipboard.Collapsed = Properties.Settings.Default.CollapseClipboard;
      if (Properties.Settings.Default.ShowClipboard) {
        clipboard.Show();
      }
      if (Properties.Settings.Default.ShowOperatorsSidebar) {
        OperatorsSidebar operatorsSidebar = new OperatorsSidebar();
        operatorsSidebar.Dock = DockStyle.Left;
        operatorsSidebar.Show();
        operatorsSidebar.Collapsed = Properties.Settings.Default.CollapseOperatorsSidebar;
      }
      if (Properties.Settings.Default.ShowStartPage) {
        StartPage startPage = new StartPage();
        startPage.Show();
      }

      WindowState = Properties.Settings.Default.ShowMaximized ? FormWindowState.Maximized : FormWindowState.Normal;
    }

    protected override void OnClosing(CancelEventArgs e) {
      base.OnClosing(e);
      if (MainFormManager.MainForm.Views.OfType<IContentView>().FirstOrDefault() != null) {
        if (MessageBox.Show(this, "Some views are still opened. If their content has not been saved, it will be lost after closing. Do you really want to close HeuristicLab Optimizer?", "Close Optimizer", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.No)
          e.Cancel = true;
      }
    }
    protected override void OnClosed(EventArgs e) {
      base.OnClosed(e);
      Properties.Settings.Default.ShowMaximized = WindowState == FormWindowState.Maximized;
      Properties.Settings.Default.CollapseClipboard = clipboard.Collapsed;
      OperatorsSidebar operatorsSidebar = MainFormManager.MainForm.Views.OfType<OperatorsSidebar>().FirstOrDefault();
      if (operatorsSidebar != null) Properties.Settings.Default.CollapseOperatorsSidebar = operatorsSidebar.Collapsed;
      Properties.Settings.Default.Save();
    }

    protected override void OnActiveViewChanged() {
      base.OnActiveViewChanged();
      UpdateTitle();
    }
  }
}
