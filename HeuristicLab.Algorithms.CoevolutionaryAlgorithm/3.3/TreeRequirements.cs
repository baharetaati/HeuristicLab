using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Problems.CooperativeProblem;
using HeuristicLab.Problems.DataAnalysis.Symbolic;

namespace HeuristicLab.Algorithms.CoevolutionaryAlgorithm {
  [StorableType("DD3CD807-9D89-4EC4-8865-A9E513930603")]
  public class TreeRequirements : DeepCloneable{
    #region Properties
    //[Storable]
    public double MinTreeLength { get; }
    //[Storable]
    //public ProbabilisticTreeCreator TreeCreator { get;}
    //[Storable]
    public int MaxTreeLength { get;}
    //[Storable]
    public int MaxTreeDepth { get;}
    [Storable]
    public List<ISymbolicExpressionTreeManipulator> Manipulators { get; private set; }
    //[Storable]
    public double MutationProbability { get;}

    #endregion

    #region Constructors
    [StorableConstructor]
    protected TreeRequirements(StorableConstructorFlag _) { }
    public TreeRequirements(int maxTreeLength, double minTreeLength, int maxTreeDepth, double mutationProbability) {
      //Grammar = new TypeCoherentExpressionGrammar();
      //Grammar.ConfigureAsDefaultRegressionGrammar();
      //Grammar.ConfigureVariableSymbols(problem.ProblemData);
      //TreeCreator = new ProbabilisticTreeCreator();
      MaxTreeLength = maxTreeLength;
      MinTreeLength = minTreeLength;
      MaxTreeDepth = maxTreeDepth;
      MutationProbability = mutationProbability;
      Manipulators = new List<ISymbolicExpressionTreeManipulator>();
      InitializeManipulators();
    }
    protected TreeRequirements(TreeRequirements original, Cloner cloner) : base(original, cloner) {
      //Grammar = cloner.Clone(original.Grammar);
      //TreeCreator = cloner.Clone(original.TreeCreator);
      MaxTreeLength = original.MaxTreeLength;
      MinTreeLength = original.MinTreeLength;
      MaxTreeDepth = original.MaxTreeDepth;
      MutationProbability = original.MutationProbability;
      Manipulators = original.Manipulators.Select(cloner.Clone).ToList();
    }
    #endregion
    public void InitializeManipulators() {
      Manipulators.Add(new ChangeNodeTypeManipulation());
      Manipulators.Add(new FullTreeShaker());
      Manipulators.Add(new OnePointShaker());
      Manipulators.Add(new RemoveBranchManipulation());
      Manipulators.Add(new ReplaceBranchManipulation());
      //manipulator.Add(new MultiSymbolicExpressionTreeArchitectureManipulator());
      //MultiSymbolicExpressionTreeManipulator
    }
    #region Cloning
    public override IDeepCloneable Clone(Cloner cloner) {
      return new TreeRequirements(this, cloner);
    }
    #endregion

  }
}
