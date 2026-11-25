/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2026, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *
 *
 * The BHoM is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation, either version 3.0 of the License, or
 * (at your option) any later version.
 *
 * The BHoM is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.
 */

using System;
using System.Collections.Generic;
using BH.oM.Base;
using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.Psychrometrics
{
    public static partial class Compute
    {
        [Description("Summarises a series of predicted mean vote values against the ISO 7730 categories: the percentage within category A (-0.2 to 0.2), category B (-0.5 to 0.5) and category C (-0.7 to 0.7), and the percentage colder or warmer than category C. Values that are not numbers are left out.")]
        [Input("predictedMeanVote", "Predicted mean vote values, for example one per occupied hour.")]
        [MultiOutput(0, "percentageWithinA", "Percentage of values from -0.2 to 0.2 (%).")]
        [MultiOutput(1, "percentageWithinB", "Percentage of values from -0.5 to 0.5 (%).")]
        [MultiOutput(2, "percentageWithinC", "Percentage of values from -0.7 to 0.7 (%).")]
        [MultiOutput(3, "percentageCold", "Percentage of values below -0.7 (%).")]
        [MultiOutput(4, "percentageWarm", "Percentage of values above 0.7 (%).")]
        public static Output<double, double, double, double, double> PredictedMeanVoteCategories(List<double> predictedMeanVote)
        {
            int count = 0, withinA = 0, withinB = 0, withinC = 0, cold = 0, warm = 0;
            if (predictedMeanVote != null)
            {
                foreach (double value in predictedMeanVote)
                {
                    if (double.IsNaN(value) || double.IsInfinity(value))
                        continue;

                    count++;
                    double size = Math.Abs(value);
                    if (size <= 0.2)
                        withinA++;
                    if (size <= 0.5)
                        withinB++;
                    if (size <= 0.7)
                        withinC++;
                    else if (value < 0)
                        cold++;
                    else
                        warm++;
                }
            }

            if (count == 0)
            {
                Base.Compute.RecordError("No predicted mean vote values to summarise.");
                return new Output<double, double, double, double, double> { Item1 = double.NaN, Item2 = double.NaN, Item3 = double.NaN, Item4 = double.NaN, Item5 = double.NaN };
            }

            if (count < predictedMeanVote.Count)
                Base.Compute.RecordWarning("Some predicted mean vote values are not numbers and were left out.");

            double percent = 100d / count;
            return new Output<double, double, double, double, double>
            {
                Item1 = withinA * percent,
                Item2 = withinB * percent,
                Item3 = withinC * percent,
                Item4 = cold * percent,
                Item5 = warm * percent
            };
        }
    }
}
