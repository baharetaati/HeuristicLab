using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Problems.CooperativeProblem.Interfaces;
using HeuristicLab.Problems.CooperativeProblem;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using HeuristicLab.Random;

namespace HeuristicLab.Algorithms.CoevolutionaryAlgorithm {
  [StorableType("A3C4170D-CB7D-4E31-957A-FFC7078DA739")]
  public abstract class Individual : DeepCloneable {
    #region Properties
    [Storable]
    private readonly TreeRequirements _treeRequirements ;
    [Storable]
    public double[] Quality { get; set; }
    [Storable]
    public bool BeingCommunicated { get; set; }
    [Storable]
    public bool BeingEffective { get; set; }
    [Storable]
    public ISymbolicExpressionTree Solution{ get; set;}
    #endregion

    #region Constructors and Cloning
    
    [StorableConstructor]
    protected Individual(StorableConstructorFlag _) { }
    //Recreating the individual of NSGA2
    public Individual(TreeRequirements treeRequirements, ISymbolicExpressionTree solution, double[] quality) {
      _treeRequirements = treeRequirements;
      Solution = solution;
      Quality = quality;
      BeingCommunicated = false;
      BeingEffective = false;

    }
    //Offspring creation
    public Individual(TreeRequirements treeRequirements, ISymbolicExpressionTree solution, int qualityLength) {
      _treeRequirements = treeRequirements;
      Solution = solution;
      Quality = new double[qualityLength];
      BeingCommunicated = false;
      BeingEffective = false;
    }
    //Initializing solutions
    public Individual(TreeRequirements treeRequirements, int qualityLength, CooperativeProblem problem, IRandom random) {
      _treeRequirements = treeRequirements;
      Quality = new double[qualityLength];
      BeingCommunicated = false;
      BeingEffective = false;
      Initialize(random, problem);
    }
    protected Individual(Individual original, Cloner cloner):base(original, cloner) { 
      _treeRequirements = cloner.Clone(original._treeRequirements);
      Solution = cloner.Clone(original.Solution);
      Quality = original.Quality.ToArray();
      BeingCommunicated = original.BeingCommunicated;
      BeingEffective = original.BeingEffective;
    }
    #endregion
    public void Initialize(IRandom random, CooperativeProblem problem) {
      if (random == null) {
        throw new Exception("random is null");

      }
      if (problem == null) {
        throw new Exception("problem is null");
      }
      Console.WriteLine("Initialize is running without any problems");
      Solution = problem.SymbolicExpressionTreeCreator.CreateTree(random, problem.SymbolicExpressionTreeGrammar, _treeRequirements.MaxTreeLength, _treeRequirements.MaxTreeDepth);
      //var Sltn = problem.SolutionCreator
    }
    public void Mutate(ISymbolicExpressionTreeManipulator chosenManipulator, IRandom random) {
      if (chosenManipulator is ChangeNodeTypeManipulation) {
        ChangeNodeTypeManipulation.ChangeNodeType(random, Solution);
      } else if (chosenManipulator is FullTreeShaker) {
        var m = (FullTreeShaker)chosenManipulator;
        Solution.Root.ForEachNodePostfix(node => {
          if (node.HasLocalParameters) {
            node.ShakeLocalParameters(random, m.ShakingFactor);
          }
        });
      } else if (chosenManipulator is OnePointShaker m) {
        OnePointShaker.Shake(random, Solution, m.ShakingFactor);
      } else if (chosenManipulator is RemoveBranchManipulation) {
        RemoveBranchManipulation.RemoveRandomBranch(random, Solution, _treeRequirements.MaxTreeLength, _treeRequirements.MaxTreeDepth);
      } else if (chosenManipulator is ReplaceBranchManipulation) {
        ReplaceBranchManipulation.ReplaceRandomBranch(random, Solution, _treeRequirements.MaxTreeLength, _treeRequirements.MaxTreeDepth);
      } else if (chosenManipulator is MultiSymbolicExpressionTreeManipulator) {
        //  Console.WriteLine("chosenManipulator: MultiSymbolicExpressionTreeManipulator");
        //  var m1 = ((MultiSymbolicExpressionTreeManipulator)chosenManipulator).Operators.CheckedItems.Shuffle(random).FirstOrDefault();
        //  chosenManipulator = m1.Value;
      } else
        throw new NotImplementedException("The specific manipulator " + chosenManipulator.GetType() + "can not be used in BasicAlgorithms yet");
    }
    public abstract double[] Evaluate(IRandom random, CooperativeProblem problem);
  }
}
