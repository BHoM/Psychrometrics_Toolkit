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

using BH.Engine.Units;
using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;
using System.ComponentModel;

namespace BH.Engine.Psychrometrics
{
    public static partial class Compute
    {
        [Description("Calculates the low and high predicted mean vote and predicted percentage of dissatisfied to ISO 7730 across a range of metabolic rate and clothing insulation at fixed room conditions. The range is sampled at its ends and its midpoint.")]
        [Input("airTemperature", "Air temperature.", typeof(Temperature))]
        [Input("meanRadiantTemperature", "Mean radiant temperature.", typeof(Temperature))]
        [Input("relativeAirSpeed", "Relative air speed.", typeof(Velocity))]
        [Input("relativeHumidity", "Relative humidity (%).")]
        [Input("metabolicRateLow", "Low metabolic rate (met).")]
        [Input("metabolicRateHigh", "High metabolic rate (met).")]
        [Input("clothingInsulationLow", "Low clothing insulation (clo).")]
        [Input("clothingInsulationHigh", "High clothing insulation (clo).")]
        [MultiOutput(0, "predictedMeanVoteLow", "Lowest predicted mean vote in the sample.")]
        [MultiOutput(1, "predictedMeanVoteHigh", "Highest predicted mean vote in the sample.")]
        [MultiOutput(2, "predictedPercentageDissatisfiedLow", "Lowest predicted percentage of dissatisfied in the sample (%).")]
        [MultiOutput(3, "predictedPercentageDissatisfiedHigh", "Highest predicted percentage of dissatisfied in the sample (%).")]
        [MultiOutput(4, "withinApplicabilityLimits", "True when every solved sample sits inside the ISO 7730 applicability limits.")]
        public static Output<double, double, double, double, bool> PredictedMeanVoteRange(double airTemperature, double meanRadiantTemperature, double relativeAirSpeed, double relativeHumidity, double metabolicRateLow, double metabolicRateHigh, double clothingInsulationLow, double clothingInsulationHigh)
        {
            WarnIfNotKelvin(airTemperature, meanRadiantTemperature);
            if (relativeAirSpeed < 0 || relativeHumidity < 0)
            {
                Base.Compute.RecordError("Relative air speed and relative humidity must be zero or positive.");
                return UnsolvedMeanVoteRange();
            }

            double airTemperatureC = airTemperature.ToDegreeCelsius();
            double radiantTemperatureC = meanRadiantTemperature.ToDegreeCelsius();
            double[] clothing = Samples(clothingInsulationLow, clothingInsulationHigh);
            double[] metabolic = Samples(metabolicRateLow, metabolicRateHigh);
            int total = 0;
            double pmvLow = 0;
            double pmvHigh = 0;
            double ppdLow = 0;
            double ppdHigh = 0;
            bool withinLimits = true;
            for (int m = 0; m < metabolic.Length; m++)
            {
                for (int c = 0; c < clothing.Length; c++)
                {
                    MeanVoteResult sample = PredictedMeanVoteFromCelsius(airTemperatureC, radiantTemperatureC, relativeAirSpeed, relativeHumidity, metabolic[m], clothing[c]);
                    if (!sample.Solved)
                        continue;
                    if (total == 0)
                    {
                        pmvLow = sample.PredictedMeanVote;
                        pmvHigh = sample.PredictedMeanVote;
                        ppdLow = sample.PredictedPercentageDissatisfied;
                        ppdHigh = sample.PredictedPercentageDissatisfied;
                    }
                    else
                    {
                        if (sample.PredictedMeanVote < pmvLow)
                            pmvLow = sample.PredictedMeanVote;
                        if (sample.PredictedMeanVote > pmvHigh)
                            pmvHigh = sample.PredictedMeanVote;
                        if (sample.PredictedPercentageDissatisfied < ppdLow)
                            ppdLow = sample.PredictedPercentageDissatisfied;
                        if (sample.PredictedPercentageDissatisfied > ppdHigh)
                            ppdHigh = sample.PredictedPercentageDissatisfied;
                    }

                    total++;
                    if (!sample.WithinApplicabilityLimits)
                        withinLimits = false;
                }
            }

            if (total == 0)
            {
                Base.Compute.RecordError("Predicted mean vote did not converge for these inputs.");
                return UnsolvedMeanVoteRange();
            }

            if (!withinLimits)
                Base.Compute.RecordWarning("Outside the ISO 7730 applicability limits. The predicted mean vote is less reliable for these inputs.");

            return new Output<double, double, double, double, bool>
            {
                Item1 = pmvLow,
                Item2 = pmvHigh,
                Item3 = ppdLow,
                Item4 = ppdHigh,
                Item5 = withinLimits
            };
        }

        private static Output<double, double, double, double, bool> UnsolvedMeanVoteRange()
        {
            return new Output<double, double, double, double, bool>
            {
                Item1 = double.NaN,
                Item2 = double.NaN,
                Item3 = double.NaN,
                Item4 = double.NaN,
                Item5 = false
            };
        }
    }
}
