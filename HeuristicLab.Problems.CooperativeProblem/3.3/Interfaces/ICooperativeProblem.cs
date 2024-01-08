using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;
using HeuristicLab.Optimization;

namespace HeuristicLab.Problems.CooperativeProblem.Interfaces {
  public interface ICooperativeProblem {
    //bool Maximization { get; }
    //double Evaluate(Individual individual, IRandom random);
    //void Analyze(Individual[] individuals, double[] qualities, ResultCollection results, IRandom random);
    int NumObjectives { get; }
    int NumConstraints { get; }
  }
}
