﻿using System.Collections.Generic;
using HeuristicLab.Core;

namespace HeuristicLab.Visualization {
  public interface IChartDataRowsModel : IItem {
    string Title { get; set; }
    string XAxisLabel { get; set; }
    List<string> XLabels { get; }
    List<IDataRow> Rows { get; }

    void AddDataRow(IDataRow row);
    void RemoveDataRow(IDataRow row);

    void AddLabel(string label);
    void AddLabel(string label, int index);
    void AddLabels(string[] labels);
    void AddLabels(string[] labels, int index);
    void ModifyLabel(string label, int index);
    void ModifyLabels(string[] labels, int index);
    void RemoveLabel(int index);
    void RemoveLabels(int index, int count);

    event ModelChangedHandler ModelChanged;
    event DataRowAddedHandler DataRowAdded;
    event DataRowRemovedHandler DataRowRemoved;
  }
}