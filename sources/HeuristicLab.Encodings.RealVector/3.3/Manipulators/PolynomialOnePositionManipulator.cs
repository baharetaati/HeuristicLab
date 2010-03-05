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
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.RealVector {
  /// <summary>
  /// Performs the polynomial manipulation on a randomly chosen single position in the real vector.
  /// </summary>
  /// <remarks>
  /// It is implemented as described in Deb, K. & Goyal, M. A. 1996. Combined Genetic Adaptive Search (GeneAS) for Engineering Design Computer Science and Informatics, 26, pp. 30-45.
  /// </remarks>
  [Item("PolynomialOnePositionManipulator", "The polynomial manipulation is implemented as described in Deb, K. & Goyal, M. A. 1996. Combined Genetic Adaptive Search (GeneAS) for Engineering Design Computer Science and Informatics, 26, pp. 30-45. In this operator it is performed on a single randomly chosen position of the real vector.")]
  [EmptyStorableClass]
  public class PolynomialOnePositionManipulator : RealVectorManipulator {
    /// <summary>
    /// The contiguity parameter specifies the shape of the probability density function that controls the mutation. Setting it to 0 is similar to a uniform distribution over the entire manipulation range (specified by <see cref="MaximumManipulationParameter"/>.
    /// A higher value will shape the density function such that values closer to 0 (little manipulation) are more likely than values closer to 1 or -1 (maximum manipulation).
    /// </summary>
    public ValueLookupParameter<DoubleData> ContiguityParameter {
      get { return (ValueLookupParameter<DoubleData>)Parameters["Contiguity"]; }
    }
    /// <summary>
    /// The maximum manipulation parameter specifies the range of the manipulation. The value specified here is the highest value the mutation will ever add to the current value.
    /// </summary>
    /// <remarks>
    /// The manipulated value is not restricted by the (possibly) specified lower and upper bounds. Use the <see cref="BoundsChecker"/> to correct the values after performing the mutation.
    /// </remarks>
    public ValueLookupParameter<DoubleData> MaximumManipulationParameter {
      get { return (ValueLookupParameter<DoubleData>)Parameters["MaximumManipulation"]; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="PolynomialOnePositionManipulator"/> with two parameters
    /// (<c>Contiguity</c> and <c>MaximumManipulation</c>).
    /// </summary>
    public PolynomialOnePositionManipulator()
      : base() {
      Parameters.Add(new ValueLookupParameter<DoubleData>("Contiguity", "Specifies whether the manipulation should produce far stretching (small value) or close (large value) manipulations with higher probability. Valid values must be greater or equal to 0.", new DoubleData(2)));
      Parameters.Add(new ValueLookupParameter<DoubleData>("MaximumManipulation", "Specifies the maximum value that should be added or subtracted by the manipulation. If this value is set to 0 no mutation will be performed.", new DoubleData(1)));
    }

    /// <summary>
    /// Performs the polynomial mutation on a single position in the real vector.
    /// </summary>
    /// <param name="random">The random number generator to use.</param>
    /// <param name="vector">The vector that should be manipulated.</param>
    /// <param name="contiguity">A parameter describing the shape of the probability density function which influences the strength of the manipulation.</param>
    /// <param name="maxManipulation">The maximum strength of the manipulation.</param>
    public static void Apply(IRandom random, DoubleArrayData vector, DoubleData contiguity, DoubleData maxManipulation) {
      if (contiguity.Value < 0) throw new ArgumentException("PolynomialOnePositionManipulator: Contiguity value is smaller than 0", "contiguity");
      int index = random.Next(vector.Length);
      double u = random.NextDouble(), delta = 0;

      if (u < 0.5) {
        delta = Math.Pow(2 * u, 1.0 / (contiguity.Value + 1)) - 1.0;
      } else if (u > 0.5) {
        delta = 1.0 - Math.Pow(2.0 - 2.0 * u, 1.0 / contiguity.Value + 1);
      } else if (u == 0.5) delta = 0;

      vector[index] += delta * maxManipulation.Value;
    }

    /// <summary>
    /// Checks the availability of the parameters and forwards the call to <see cref="Apply(IRandom, DoubleArrayData, DoubleData, DoubleData)"/>.
    /// </summary>
    /// <param name="random">The random number generator to use.</param>
    /// <param name="realVector">The vector of real values to manipulate.</param>
    protected override void Manipulate(IRandom random, DoubleArrayData realVector) {
      if (ContiguityParameter.ActualValue == null) throw new InvalidOperationException("PolynomialOnePositionManipulator: Parameter " + ContiguityParameter.ActualName + " could not be found.");
      if (MaximumManipulationParameter.ActualValue == null) throw new InvalidOperationException("PolynomialOnePositionManipulator: Parameter " + MaximumManipulationParameter.ActualName + " could not be found.");
      Apply(random, realVector, ContiguityParameter.ActualValue, MaximumManipulationParameter.ActualValue);
    }
  }
}
