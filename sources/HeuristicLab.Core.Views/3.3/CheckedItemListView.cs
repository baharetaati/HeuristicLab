﻿#region License Information
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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.Collections;
using HeuristicLab.MainForm;

namespace HeuristicLab.Core.Views {
  /// <summary>
  /// The visual representation of a list of checked items.
  /// </summary>
  [View("CheckedItemList View")]
  [Content(typeof(CheckedItemList<>), true)]
  [Content(typeof(ICheckedItemList<>), true)]
  public partial class CheckedItemListView<T> : ItemListView<T> where T : class, IItem {
    public new ICheckedItemList<T> Content {
      get { return (ICheckedItemList<T>)base.Content; }
      set { base.Content = value; }
    }

    public CheckedItemListView()
      : base() {
      InitializeComponent();
    }

    protected override void RegisterContentEvents() {
      Content.CheckedItemsChanged += new CollectionItemsChangedEventHandler<IndexedItem<T>>(Content_CheckedItemsChanged);
      base.RegisterContentEvents();
    }
    protected override void DeregisterContentEvents() {
      Content.CheckedItemsChanged -= new CollectionItemsChangedEventHandler<IndexedItem<T>>(Content_CheckedItemsChanged);
      base.DeregisterContentEvents();
    }

    protected override void OnContentChanged() {
      Caption = "Checked Item List";
      base.OnContentChanged();
    }

    protected override ListViewItem CreateListViewItem(T item) {
      ListViewItem listViewItem = base.CreateListViewItem(item);
      listViewItem.Checked = Content.ItemChecked(item);
      return listViewItem;
    }

    #region ListView Events
    protected virtual void itemsListView_ItemChecked(object sender, ItemCheckedEventArgs e) {
      var checkedItem = (T)e.Item.Tag;
      if (Content.ItemChecked(checkedItem) != e.Item.Checked) {
        Content.SetItemCheckedState(checkedItem, e.Item.Checked);
      }
    }
    #endregion

    #region Content Events
    protected virtual void Content_CheckedItemsChanged(object sender, CollectionItemsChangedEventArgs<IndexedItem<T>> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<IndexedItem<T>>(Content_CheckedItemsChanged), sender, e);
      else {
        foreach (var item in e.Items) {
          if (itemsListView.Items[item.Index].Checked != Content.ItemChecked(item.Value))
            itemsListView.Items[item.Index].Checked = Content.ItemChecked(item.Value);
        }
      }
    }
    #endregion
  }
}
