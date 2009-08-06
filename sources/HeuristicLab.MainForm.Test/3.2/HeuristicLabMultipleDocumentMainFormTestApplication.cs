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
using System.Windows.Forms;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.MainForm;

namespace HeuristicLab.MainForm.Test {
  [ClassInfo(Name = "MultipleDocumentMainForm Test", Description="Test application for new mainform development.")]
  public class HeuristicLabMultipleDocumentMainFormTestApplication : ApplicationBase {
    public override void Run() {
      MultipleDocumentMainForm mainForm = new MultipleDocumentMainForm(typeof(ITestUserInterfaceItemProvider));
      mainForm.Title = "Test new MAINFORM concept";
      Application.Run(mainForm);
    }
  }
}
