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
using System.Windows.Forms;
using HeuristicLab.MainForm;

namespace HeuristicLab.Data {
  /// <summary>
  /// The visual representation of the class <see cref="DoubleMatrixData"/>, symbolizing a two-dimensional
  /// matrix of double values.
  /// </summary>
  [Content(typeof(DoubleMatrixData), true)]
  public partial class DoubleMatrixDataView : MatrixDataBaseView {
    /// <summary>
    /// Gets or sets the double matrix to represent visually.
    /// </summary>
    /// <remarks>Uses property <see cref="ArrayDataBase"/> of base class 
    /// <see cref="MatrixDataBaseView"/>. No own data storage present.</remarks>
    public DoubleMatrixData DoubleMatrixData {
      get { return (DoubleMatrixData)base.ArrayDataBase; }
      set { base.ArrayDataBase = value; }
    }

    /// <summary>
    /// Initializes a new instance of the class <see cref="DoubleMatrixDataView"/>.
    /// </summary>
    public DoubleMatrixDataView() {
      InitializeComponent();
      // round-trip format for all cells
      dataGridView.DefaultCellStyle.Format = "r";
    }   
    /// <summary>
    /// Initializes a new instance of the class <see cref="DoubleMatrixDataView"/> with the given
    /// <paramref name="doubleMatrixData"/>.
    /// <note type="caution"> No CopyConstructor! <paramref name="doubleMatrixData"/> is not copied!</note>
    /// </summary>
    /// <param name="doubleMatrixData">The matrix of doubles to represent visually.</param>
    public DoubleMatrixDataView(DoubleMatrixData doubleMatrixData)
      : this() {
      DoubleMatrixData = doubleMatrixData;
    }

    /// <summary>
    /// Subsitutes an element in the given <paramref name="row"/> and the given 
    /// <paramref name="column"/> with the given <paramref name="element"/>.
    /// </summary>
    /// <param name="row">The row of the element to substitute.</param>
    /// <param name="column">The column of the element to substitute.</param>
    /// <param name="element">The element to insert.</param>
    protected override void SetArrayElement(int row, int column, string element) {
      double result;
      double.TryParse(element, out result);
      if(result != DoubleMatrixData.Data[row, column]) {
        DoubleMatrixData.Data[row, column] = result;
        DoubleMatrixData.FireChanged();
      }
    }

    /// <summary>
    /// Checks whether the given <paramref name="element"/> can be converted to a double value.
    /// </summary>
    /// <param name="element">The element to check.</param>
    /// <returns><c>true</c> if the <paramref name="element"/> could be converted,
    /// <c>false</c> otherwise.</returns>
    protected override bool ValidateData(string element) {
      double result;
      return element != null && double.TryParse(element, out result);
    }
  }
}
