using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Problems.CooperativeProblem;

namespace HeuristicLab.Algorithms.CoevolutionaryAlgorithm {
  public class SingleObjectiveIndividual : DeepCloneable {
    #region Properties
    [Storable]
    public double Quality;
    [Storable]
    private readonly TreeRequirements _treeRequirements;
    [Storable]
    public ISymbolicExpressionTree Solution { get; set; }
    #endregion

    #region Constructors and Cloning

    [StorableConstructor]
    protected SingleObjectiveIndividual(StorableConstructorFlag _) { }
    public SingleObjectiveIndividual(TreeRequirements treeRequirements, ISymbolicExpressionTree solution) {
      Quality = -1.0;
      _treeRequirements = treeRequirements;
      Solution = solution;
    }
    public SingleObjectiveIndividual(TreeRequirements treeRequirements, CooperativeProblem problem, IRandom random) {
      Quality = -1.0;
      _treeRequirements = treeRequirements;
      Initialize(random, problem);
    }
    protected SingleObjectiveIndividual(SingleObjectiveIndividual original, Cloner cloner) {
      Quality= original.Quality;
      _treeRequirements = cloner.Clone(original._treeRequirements);
      Solution = cloner.Clone(original.Solution);
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SingleObjectiveIndividual(this, cloner);
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
    }
    public double Evaluate(IRandom random, CooperativeProblem problem) {
      bool maximization = problem.Maximization[0];
      if (maximization) {
        Quality = problem.EvaluateSingleObjectivePearsonRsquaredError(Solution, random);
      } else {
        //Quality = problem.EvaluateSingleObjectiveNormalizedMeanSquaredError(Solution, random);
        Quality = problem.EvaluateSingleObjectivePearsonRsquaredError(Solution, random);

      }

      return Quality;
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
  }
}
