﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Hive.Server.LINQDataAccess {
  [Plugin("HeuristicLab.Hive.Server.LINQDataAccess-3.2")]
  [PluginFile("HeuristicLab.Hive.Server.LINQDataAccess-3.2.dll", PluginFileType.Assembly)]
  [PluginDependency("HeuristicLab.Core-3.2")]
  [PluginDependency("HeuristicLab.Hive.Contracts-3.2")]
  [PluginDependency("HeuristicLab.Hive.Server.DataAccess-3.2")]
  [PluginDependency("HeuristicLab.Tracing", "3.2.0")]
  public class LINQDataAccessPlugin: PluginBase {
  }
}
