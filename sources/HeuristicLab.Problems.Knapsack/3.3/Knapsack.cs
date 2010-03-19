#region License Information
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
using System.Drawing;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.BinaryVectorEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Problems.Knapsack {
  [Item("Knapsack", "Represents a Knapsack Problem.")]
  [Creatable("Problems")]
  [StorableClass]
  public sealed class Knapsack : ParameterizedNamedItem, ISingleObjectiveProblem {
    public override Image ItemImage {
      get { return HeuristicLab.Common.Resources.VS2008ImageLibrary.Type; }
    }

    #region Parameter Properties
    public ValueParameter<BoolValue> MaximizationParameter {
      get { return (ValueParameter<BoolValue>)Parameters["Maximization"]; }
    }
    IParameter ISingleObjectiveProblem.MaximizationParameter {
      get { return MaximizationParameter; }
    }
    public ValueParameter<IntValue> KnapsackCapacityParameter {
      get { return (ValueParameter<IntValue>)Parameters["KnapsackCapacity"]; }
    }
    public ValueParameter<IntArray> WeightsParameter {
      get { return (ValueParameter<IntArray>)Parameters["Weights"]; }
    }
    public ValueParameter<IntArray> ValuesParameter {
      get { return (ValueParameter<IntArray>)Parameters["Values"]; }
    }
    public ValueParameter<DoubleValue> PenaltyParameter {
      get { return (ValueParameter<DoubleValue>)Parameters["Penalty"]; }
    }
    public ValueParameter<IBinaryVectorCreator> SolutionCreatorParameter {
      get { return (ValueParameter<IBinaryVectorCreator>)Parameters["SolutionCreator"]; }
    }
    IParameter IProblem.SolutionCreatorParameter {
      get { return SolutionCreatorParameter; }
    }
    public ValueParameter<IKnapsackEvaluator> EvaluatorParameter {
      get { return (ValueParameter<IKnapsackEvaluator>)Parameters["Evaluator"]; }
    }
    IParameter IProblem.EvaluatorParameter {
      get { return EvaluatorParameter; }
    }
    public OptionalValueParameter<IKnapsackSolutionsVisualizer> VisualizerParameter {
      get { return (OptionalValueParameter<IKnapsackSolutionsVisualizer>)Parameters["Visualizer"]; }
    }
    IParameter IProblem.VisualizerParameter {
      get { return VisualizerParameter; }
    }  
    public OptionalValueParameter<DoubleValue> BestKnownQualityParameter {
      get { return (OptionalValueParameter<DoubleValue>)Parameters["BestKnownQuality"]; }
    }
    IParameter ISingleObjectiveProblem.BestKnownQualityParameter {
      get { return BestKnownQualityParameter; }
    }
    #endregion

    #region Properties
    public IntValue KnapsackCapacity {
      get { return KnapsackCapacityParameter.Value; }
      set { KnapsackCapacityParameter.Value = value; }
    }
    public IntArray Weights {
      get { return WeightsParameter.Value; }
      set { WeightsParameter.Value = value; }
    }
    public IntArray Values {
      get { return ValuesParameter.Value; }
      set { ValuesParameter.Value = value; }
    }
    public DoubleValue Penalty {
      get { return PenaltyParameter.Value; }
      set { PenaltyParameter.Value = value; }
    }
    public IBinaryVectorCreator SolutionCreator {
      get { return SolutionCreatorParameter.Value; }
      set { SolutionCreatorParameter.Value = value; }
    }
    ISolutionCreator IProblem.SolutionCreator {
      get { return SolutionCreatorParameter.Value; }
    }
    public IKnapsackEvaluator Evaluator {
      get { return EvaluatorParameter.Value; }
      set { EvaluatorParameter.Value = value; }
    }
    ISingleObjectiveEvaluator ISingleObjectiveProblem.Evaluator {
      get { return EvaluatorParameter.Value; }
    }
    IEvaluator IProblem.Evaluator {
      get { return EvaluatorParameter.Value; }
    }
    public IKnapsackSolutionsVisualizer Visualizer {
      get { return VisualizerParameter.Value; }
      set { VisualizerParameter.Value = value; }
    }
    ISolutionsVisualizer IProblem.Visualizer {
      get { return VisualizerParameter.Value; }
    } 
    public DoubleValue BestKnownQuality {
      get { return BestKnownQualityParameter.Value; }
      set { BestKnownQualityParameter.Value = value; }
    }
    private List<IBinaryVectorOperator> operators;
    public IEnumerable<IOperator> Operators {
      get { return operators.Cast<IOperator>(); }
    }
    #endregion

    public Knapsack()
      : base() {
      RandomBinaryVectorCreator creator = new RandomBinaryVectorCreator();
      KnapsackEvaluator evaluator = new KnapsackEvaluator();

      Parameters.Add(new ValueParameter<BoolValue>("Maximization", "Set to true as the Knapsack Problem is a maximization problem.", new BoolValue(true)));
      Parameters.Add(new ValueParameter<IntValue>("KnapsackCapacity", "Capacity of the Knapsack.", new IntValue(0)));
      Parameters.Add(new ValueParameter<IntArray>("Weights", "The weights of the items.", new IntArray(5)));
      Parameters.Add(new ValueParameter<IntArray>("Values", "The values of the items.", new IntArray(5)));
      Parameters.Add(new ValueParameter<DoubleValue>("Penalty", "The penalty value for each unit of overweight.", new DoubleValue(1)));
      Parameters.Add(new ValueParameter<IBinaryVectorCreator>("SolutionCreator", "The operator which should be used to create new Knapsack solutions.", creator));
      Parameters.Add(new ValueParameter<IKnapsackEvaluator>("Evaluator", "The operator which should be used to evaluate Knapsack solutions.", evaluator));
      Parameters.Add(new OptionalValueParameter<DoubleValue>("BestKnownQuality", "The quality of the best known solution of this Knapsack instance."));
      Parameters.Add(new ValueParameter<IKnapsackSolutionsVisualizer>("Visualizer", "The operator which should be used to visualize Knapsack solutions.", null));

      creator.BinaryVectorParameter.ActualName = "KnapsackSolution";
      evaluator.QualityParameter.ActualName = "NumberOfOnes";
      ParameterizeSolutionCreator();
      ParameterizeEvaluator();

      Initialize();
    }

    [StorableConstructor]
    private Knapsack(bool deserializing) : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      Knapsack clone = (Knapsack)base.Clone(cloner);
      clone.Initialize();
      return clone;
    }

    #region Events
    public event EventHandler SolutionCreatorChanged;
    private void OnSolutionCreatorChanged() {
      if (SolutionCreatorChanged != null)
        SolutionCreatorChanged(this, EventArgs.Empty);
    }
    public event EventHandler EvaluatorChanged;
    private void OnEvaluatorChanged() {
      if (EvaluatorChanged != null)
        EvaluatorChanged(this, EventArgs.Empty);
    }
    public event EventHandler VisualizerChanged;
    private void OnVisualizerChanged() {
      if (VisualizerChanged != null)
        VisualizerChanged(this, EventArgs.Empty);
    }

    public event EventHandler OperatorsChanged;
    private void OnOperatorsChanged() {
      if (OperatorsChanged != null)
        OperatorsChanged(this, EventArgs.Empty);
    }

    private void SolutionCreatorParameter_ValueChanged(object sender, EventArgs e) {
      SolutionCreator.BinaryVectorParameter.ActualNameChanged += new EventHandler(SolutionCreator_PermutationParameter_ActualNameChanged);
      ParameterizeSolutionCreator();
      ParameterizeEvaluator();
      ParameterizeOperators();
      OnSolutionCreatorChanged();
    }
    private void SolutionCreator_PermutationParameter_ActualNameChanged(object sender, EventArgs e) {
      ParameterizeEvaluator();
      ParameterizeOperators();
    }
    private void EvaluatorParameter_ValueChanged(object sender, EventArgs e) {
      ParameterizeEvaluator();
      OnEvaluatorChanged();
    }
    void KnapsackCapacityParameter_ValueChanged(object sender, EventArgs e) {
      ParameterizeEvaluator();
    }
    void WeightsParameter_ValueChanged(object sender, EventArgs e) {
      ParameterizeEvaluator();
      ParameterizeSolutionCreator();

      WeightsParameter.Value.Reset += new EventHandler(WeightsValue_Reset);
    }
    void WeightsValue_Reset(object sender, EventArgs e) {
      ParameterizeSolutionCreator();
    }
    void ValuesParameter_ValueChanged(object sender, EventArgs e) {
      ParameterizeEvaluator();
      ParameterizeSolutionCreator();

      ValuesParameter.Value.Reset += new EventHandler(ValuesValue_Reset);
    }
    void ValuesValue_Reset(object sender, EventArgs e) {
      ParameterizeSolutionCreator();
    }
    void PenaltyParameter_ValueChanged(object sender, EventArgs e) {
      ParameterizeEvaluator();
    }
    void VisualizerParameter_ValueChanged(object sender, EventArgs e) {
      OnVisualizerChanged();
    }
    void OneBitflipMoveParameter_ActualNameChanged(object sender, EventArgs e) {
      string name = ((ILookupParameter<OneBitflipMove>)sender).ActualName;
      foreach (IOneBitflipMoveOperator op in Operators.OfType<IOneBitflipMoveOperator>()) {
        op.OneBitflipMoveParameter.ActualName = name;
      }
    }
    #endregion

    #region Helpers
    [StorableHook(HookType.AfterDeserialization)]
    private void Initialize() {
      InitializeOperators();
      SolutionCreatorParameter.ValueChanged += new EventHandler(SolutionCreatorParameter_ValueChanged);
      SolutionCreator.BinaryVectorParameter.ActualNameChanged += new EventHandler(SolutionCreator_PermutationParameter_ActualNameChanged);
      EvaluatorParameter.ValueChanged += new EventHandler(EvaluatorParameter_ValueChanged);
      KnapsackCapacityParameter.ValueChanged += new EventHandler(KnapsackCapacityParameter_ValueChanged);
      WeightsParameter.ValueChanged += new EventHandler(WeightsParameter_ValueChanged);
      WeightsParameter.Value.Reset += new EventHandler(WeightsValue_Reset);
      ValuesParameter.ValueChanged += new EventHandler(ValuesParameter_ValueChanged);
      ValuesParameter.Value.Reset += new EventHandler(ValuesValue_Reset);
      PenaltyParameter.ValueChanged += new EventHandler(PenaltyParameter_ValueChanged);
      VisualizerParameter.ValueChanged += new EventHandler(VisualizerParameter_ValueChanged);
    }
    private void ParameterizeSolutionCreator() {
      if(SolutionCreator.LengthParameter.Value == null ||
        SolutionCreator.LengthParameter.Value.Value != WeightsParameter.Value.Length)
        SolutionCreator.LengthParameter.Value = new IntValue(WeightsParameter.Value.Length);
    }
    private void ParameterizeEvaluator() {
      if (Evaluator is KnapsackEvaluator) {
        KnapsackEvaluator knapsackEvaluator =
          (KnapsackEvaluator)Evaluator;
        knapsackEvaluator.BinaryVectorParameter.ActualName = SolutionCreator.BinaryVectorParameter.ActualName;
        knapsackEvaluator.KnapsackCapacityParameter.ActualName = KnapsackCapacityParameter.Name;
        knapsackEvaluator.WeightsParameter.ActualName = WeightsParameter.Name;
        knapsackEvaluator.ValuesParameter.ActualName = ValuesParameter.Name;
        knapsackEvaluator.PenaltyParameter.ActualName = PenaltyParameter.Name;
      }
    }
    private void InitializeOperators() {
      operators = new List<IBinaryVectorOperator>();
      if (ApplicationManager.Manager != null) {
        operators.AddRange(ApplicationManager.Manager.GetInstances<IBinaryVectorOperator>());
        ParameterizeOperators();
      }

      InitializeMoveGenerators();
    }
    private void InitializeMoveGenerators() {
      foreach (IOneBitflipMoveOperator op in Operators.OfType<IOneBitflipMoveOperator>()) {
        if (op is IMoveGenerator) {
          op.OneBitflipMoveParameter.ActualNameChanged += new EventHandler(OneBitflipMoveParameter_ActualNameChanged);
        }
      }
    }
    private void ParameterizeOperators() {
      foreach (IBinaryVectorCrossover op in Operators.OfType<IBinaryVectorCrossover>()) {
        op.ParentsParameter.ActualName = SolutionCreator.BinaryVectorParameter.ActualName;
        op.ChildParameter.ActualName = SolutionCreator.BinaryVectorParameter.ActualName;
      }
      foreach (IBinaryVectorManipulator op in Operators.OfType<IBinaryVectorManipulator>()) {
        op.BinaryVectorParameter.ActualName = SolutionCreator.BinaryVectorParameter.ActualName;
      }
      foreach (IBinaryVectorMoveOperator op in Operators.OfType<IBinaryVectorMoveOperator>()) {
        op.BinaryVectorParameter.ActualName = SolutionCreator.BinaryVectorParameter.ActualName;
      }
      foreach (IKnapsackMoveEvaluator op in Operators.OfType<IKnapsackMoveEvaluator>()) {
        op.KnapsackCapacityParameter.ActualName = KnapsackCapacityParameter.Name;
        op.PenaltyParameter.ActualName = PenaltyParameter.Name;
        op.WeightsParameter.ActualName = WeightsParameter.Name;
        op.ValuesParameter.ActualName = ValuesParameter.Name;
      }
    }
    #endregion
  }
}
