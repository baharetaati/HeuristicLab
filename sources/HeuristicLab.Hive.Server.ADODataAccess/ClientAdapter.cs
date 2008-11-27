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
using HeuristicLab.Hive.Server.Core.InternalInterfaces.DataAccess;
using HeuristicLab.Hive.Contracts.BusinessObjects;

namespace HeuristicLab.Hive.Server.ADODataAccess {
  class ClientAdapter: IClientAdapter {
    private dsHiveServerTableAdapters.ClientTableAdapter adapter =
        new dsHiveServerTableAdapters.ClientTableAdapter();

    private ResourceAdapter resAdapter = 
      new ResourceAdapter();
    
    #region IClientAdapter Members
    private ClientInfo Convert(dsHiveServer.ClientRow row) {
      if(row != null) {
        ClientInfo client = new ClientInfo();
       
        /*Parent - resource*/
        Resource resource =
          resAdapter.GetResourceById(row.ResourceId);
        client.ResourceId = resource.ResourceId;
        client.Name = resource.Name;

        /*ClientInfo*/
        client.ClientId = row.GUID;
        client.CpuSpeedPerCore = row.CPUSpeed;
        client.Memory = row.Memory;
        client.Login = row.Login;
        if (row.Status != null)
          client.State = (State)Enum.Parse(typeof(State), row.Status, true);
        client.NrOfCores = row.NumberOfCores;

        //todo: config adapter (client.config)

        return client;
      }
      else
        return null;
    }

    private dsHiveServer.ClientRow Convert(ClientInfo client,
      dsHiveServer.ClientRow row) {
      if (client != null && row != null) {      
        row.ResourceId = client.ResourceId;
        row.GUID = client.ClientId;
        row.CPUSpeed = client.CpuSpeedPerCore;
        row.Memory = client.Memory;
        row.Login = client.Login;
        row.Status = client.State.ToString();
        row.NumberOfCores = client.NrOfCores;

        //todo: config adapter
        /*if (client.Config != null)
          row.ClientConfigId = client.Config.ClientConfigId;
         else
          row.ClientConfigId = null;*/
      }

      return row;
    }

    public void UpdateClient(ClientInfo client) {
      if (client != null) {
        resAdapter.UpdateResource(client);

        dsHiveServer.ClientDataTable data =
          adapter.GetDataById(client.ClientId);

        dsHiveServer.ClientRow row;
        if (data.Count == 0) {
          row = data.NewClientRow();
          row.ResourceId = client.ResourceId;
          data.AddClientRow(row);
        } else {
          row = data[0];
        }

        Convert(client, row);

        adapter.Update(data);
      }
    }

    public ClientInfo GetClientById(Guid clientId) {
      dsHiveServer.ClientDataTable data =
          adapter.GetDataById(clientId);
      if (data.Count == 1) {
        dsHiveServer.ClientRow row = 
          data[0];
        return Convert(row);
      } else {
        return null;
      }
    }

    public ICollection<ClientInfo> GetAllClients() {
      ICollection<ClientInfo> allClients =
        new List<ClientInfo>();

      dsHiveServer.ClientDataTable data =
          adapter.GetData();

      foreach (dsHiveServer.ClientRow row in data) {
        allClients.Add(Convert(row));
      }

      return allClients;
    }

    public bool DeleteClient(ClientInfo client) {
      //referential integrity will delete the client object
      return resAdapter.DeleteResource(client);
    }

    #endregion
  }
}
