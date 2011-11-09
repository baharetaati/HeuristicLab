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
using HeuristicLab.Clients.Common;

namespace HeuristicLab.Clients.Hive {
  public class ServiceLocator : IServiceLocator {
    private static IServiceLocator instance = null;
    public static IServiceLocator Instance {
      get {
        if (instance == null) {
          instance = new ServiceLocator();
        }
        return instance;
      }
      set {
        instance = value;
      }
    }

    private string username;
    public string Username {
      get { return username; }
      set { username = value; }
    }

    private string password;
    public string Password {
      get { return password; }
      set { password = value; }
    }

    private HiveServiceClient NewServiceClient() {
      HiveServiceClient cl;
      if (string.IsNullOrEmpty(username) && string.IsNullOrEmpty(password))
        cl = ClientFactory.CreateClient<HiveServiceClient, IHiveService>();
      else
        cl = ClientFactory.CreateClient<HiveServiceClient, IHiveService>(null, null, username, password);
      
      return cl;
    }

    public T CallHiveService<T>(Func<IHiveService, T> call) {
      HiveServiceClient client = NewServiceClient();
      try {
        return call(client);
      }
      finally {
        try {
          client.Close();
        }
        catch (Exception) {
          client.Abort();
        }
      }
    }

    public void CallHiveService(Action<IHiveService> call) {
      HiveServiceClient client = NewServiceClient();
      try {
        call(client);
      }
      finally {
        try {
          client.Close();
        }
        catch (Exception) {
          client.Abort();
        }
      }
    }
  }
}
