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
using System.Linq;
using System.Text;
using HeuristicLab.Hive.Server.DataAccess;
using HeuristicLab.Hive.Contracts.BusinessObjects;
using System.Linq.Expressions;
using HeuristicLab.DataAccess.Interfaces;
using HeuristicLab.DataAccess.ADOHelper;
using HeuristicLab.Hive.Server.ADODataAccess.dsHiveServerTableAdapters;
using System.Data.Common;
using System.Data.SqlClient;

namespace HeuristicLab.Hive.Server.ADODataAccess {
  class ClientAdapterWrapper :
    DataAdapterWrapperBase<
        dsHiveServerTableAdapters.ClientTableAdapter,
    ClientInfo,
    dsHiveServer.ClientRow> {
    public override void UpdateRow(dsHiveServer.ClientRow row) {
      TransactionalAdapter.Update(row);
    }

    public override dsHiveServer.ClientRow
      InsertNewRow(ClientInfo client) {
      dsHiveServer.ClientDataTable data =
        new dsHiveServer.ClientDataTable();

      dsHiveServer.ClientRow row = data.NewClientRow();
      row.ResourceId = client.Id;
      data.AddClientRow(row);

      return row;
    }

    public override IEnumerable<dsHiveServer.ClientRow>
      FindById(Guid id) {
      return TransactionalAdapter.GetDataById(id);
    }

    public override IEnumerable<dsHiveServer.ClientRow>
      FindAll() {
      return TransactionalAdapter.GetData();
    }

    protected override void SetConnection(DbConnection connection) {
      adapter.Connection = connection as SqlConnection;
    }

    protected override void SetTransaction(DbTransaction transaction) {
      adapter.Transaction = transaction as SqlTransaction;
    }
  }

  class ClientAdapter: 
    DataAdapterBase<
      dsHiveServerTableAdapters.ClientTableAdapter, 
      ClientInfo, 
      dsHiveServer.ClientRow>,
    IClientAdapter {
    #region Fields
    private IResourceAdapter resAdapter = null;

    private IResourceAdapter ResAdapter {
      get {
        if (resAdapter == null)
          resAdapter =
            this.Session.GetDataAdapter<Resource, IResourceAdapter>();

        return resAdapter;
      }
    }

    private IClientGroupAdapter clientGroupAdapter = null;

    private IClientGroupAdapter ClientGroupAdapter {
      get {
        if (clientGroupAdapter == null) {
          clientGroupAdapter =
            this.Session.GetDataAdapter<ClientGroup, IClientGroupAdapter>();
        }

        return clientGroupAdapter;
      }
    }

    private IJobAdapter jobAdapter = null;

    private IJobAdapter JobAdapter {
      get {
        if (jobAdapter == null) {
          this.Session.GetDataAdapter<Job, IJobAdapter>();
        }

        return jobAdapter;
      }
    }
    #endregion

    public ClientAdapter(): 
      base(new ClientAdapterWrapper()) {
    }

    #region Overrides
    protected override ClientInfo ConvertRow(dsHiveServer.ClientRow row, 
      ClientInfo client) {
      if(row != null && client != null) {      
        /*Parent - resource*/
        client.Id = row.ResourceId;
        ResAdapter.GetById(client);

        /*ClientInfo*/       
        if (!row.IsCPUSpeedNull())
          client.CpuSpeedPerCore = row.CPUSpeed;
        else
          client.CpuSpeedPerCore = 0;

        if (!row.IsMemoryNull())
          client.Memory = row.Memory;
        else
          client.Memory = 0;

        if (!row.IsLoginNull())
          client.Login = row.Login;
        else
          client.Login = DateTime.MinValue;

        if (!row.IsStatusNull())
          client.State = (State)Enum.Parse(typeof(State), row.Status, true);
        else
          client.State = State.nullState;

        if (!row.IsNumberOfCoresNull())
          client.NrOfCores = row.NumberOfCores;
        else
          client.NrOfCores = 0;

        if (!row.IsNumberOfFreeCoresNull())
          client.NrOfFreeCores = row.NumberOfFreeCores;
        else
          client.NrOfFreeCores = 0;

        if (!row.IsFreeMemoryNull())
          client.FreeMemory = row.FreeMemory;
        else
          client.FreeMemory = 0;

        //todo: config adapter (client.config)

        return client;
      }
      else
        return null;
    }

    protected override dsHiveServer.ClientRow ConvertObj(ClientInfo client,
      dsHiveServer.ClientRow row) {
      if (client != null && row != null) {
        row.ResourceId = client.Id;
        row.CPUSpeed = client.CpuSpeedPerCore;
        row.Memory = client.Memory;
        row.Login = client.Login;
        if (client.State != State.nullState)
          row.Status = client.State.ToString();
        else
          row.SetStatusNull();
        row.NumberOfCores = client.NrOfCores;
        row.NumberOfFreeCores = client.NrOfFreeCores;
        row.FreeMemory = client.FreeMemory;

        //todo: config adapter
        /*if (client.Config != null)
          row.ClientConfigId = client.Config.ClientConfigId;
         else
          row.ClientConfigId = null;*/
      }

      return row;
    }

    #endregion

    #region IClientAdapter Members
    protected override void doUpdate(ClientInfo client) {
      if (client != null) {
        ResAdapter.Update(client);

        base.doUpdate(client);
      }
    }

    public ClientInfo GetByName(string name) {
      ClientInfo client = new ClientInfo();
      Resource res =
        ResAdapter.GetByName(name);

      return GetById(res.Id);
    }

    protected override bool doDelete(ClientInfo client) {
      bool success = false;
      
      if (client != null) {
        dsHiveServer.ClientRow row =
          GetRowById(client.Id);

        if (row != null) {
          success = base.doDelete(client) && 
            ResAdapter.Delete(client);
        }
      }

      return success;
    }

    #endregion
  }
}
