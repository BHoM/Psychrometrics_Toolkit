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
using BH.Engine.Units;
using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;
using System.ComponentModel;

namespace BH.Engine.Psychrometrics
{
    public static partial class Compute
    {
        [Description("Calculates the predicted mean vote and predicted percentage of dissatisfied to ISO 7730 from air temperature, mean radiant temperature, relative air speed, relative humidity, metabolic rate and clothing insulation. External work is zero.")]
        [Input("airTemperature", "Air temperature.", typeof(Temperature))]
        [Input("meanRadiantTemperature", "Mean radiant temperature.", typeof(Temperature))]
        [Input("relativeAirSpeed", "Relative air speed.", typeof(Velocity))]
        [Input("relativeHumidity", "Relative humidity (%).")]
        [Input("metabolicRate", "Metabolic rate (met).")]
        [Input("clothingInsulation", "Clothing insulation (clo).")]
        [MultiOutput(0, "predictedMeanVote", "Predicted mean vote.")]
        [MultiOutput(1, "predictedPercentageDissatisfied", "Predicted percentage of dissatisfied (%).")]
        [MultiOutput(2, "withinApplicabilityLimits", "True when the inputs sit inside the ISO 7730 applicability limits.")]
        public static Output<double, double, bool> PredictedMeanVote(double airTemperature, double meanRadiantTemperature, double relativeAirSpeed, double relativeHumidity, double metabolicRate, double clothingInsulation)
        {
            WarnIfNotKelvin(airTemperature, meanRadiantTemperature);
            if (relativeAirSpeed < 0 || clothingInsulation < 0 || metabolicRate <= 0 || relativeHumidity < 0)
            {
                Base.Compute.RecordError("Metabolic rate must be greater than zero. Relative air speed, clothing insulation and relative humidity must be zero or positive.");
                return UnsolvedMeanVote();
            }

            MeanVoteResult result = PredictedMeanVoteFromCelsius(airTemperature.ToDegreeCelsius(), meanRadiantTemperature.ToDegreeCelsius(), relativeAirSpeed, relativeHumidity, metabolicRate, clothingInsulation);
            if (!result.Solved)
            {
                Base.Compute.RecordError("Predicted mean vote did not converge for these inputs.");
                return UnsolvedMeanVote();
            }

            if (!result.WithinApplicabilityLimits)
                Base.Compute.RecordWarning("Outside the ISO 7730 applicability limits. The predicted mean vote is less reliable for these inputs.");

            return new Output<double, double, bool>
            {
                Item1 = result.PredictedMeanVote,
                Item2 = result.PredictedPercentageDissatisfied,
                Item3 = result.WithinApplicabilityLimits
            };
        }

        private static void WarnIfNotKelvin(double firstTemperature, double secondTemperature)
        {
            if (firstTemperature < 100 || secondTemperature < 100)
                Base.Compute.RecordWarning("It looks like you have entered a temperature in Celsius/Fahrenheit instead of Kelvin. Check your inputs.");
        }

        private static Output<double, double, bool> UnsolvedMeanVote()
        {
            return new Output<double, double, bool> { Item1 = double.NaN, Item2 = double.NaN, Item3 = false };
        }

        private static double[] Samples(double low, double high)
        {
            if (high < low)
            {
                double swap = low;
                low = high;
                high = swap;
            }

            if (high - low < 0.000001)
                return new double[] { low };

            double[] values = new double[3];
            for (int i = 0; i < values.Length; i++)
                values[i] = low + (high - low) * i / (values.Length - 1);
            return values;
        }

        private struct MeanVoteResult
        {
            public bool Solved;
            public double PredictedMeanVote;
            public double PredictedPercentageDissatisfied;
            public bool WithinApplicabilityLimits;
        }

        private static MeanVoteResult PredictedMeanVoteFromCelsius(double airTemperatureC, double radiantTemperatureC, double relativeAirSpeed, double relativeHumidity, double metabolicRate, double clothingInsulation)
        {
            const double wattsPerMet = 58.15;
            MeanVoteResult result = new MeanVoteResult();
            if (relativeAirSpeed < 0 || clothingInsulation < 0 || metabolicRate <= 0 || relativeHumidity < 0)
                return result;

            double vapourPressure = relativeHumidity * 10d * Math.Exp(16.6536 - 4030.183 / (airTemperatureC + 235d));
            double clothing = 0.155 * clothingInsulation;
            double metabolic = metabolicRate * wattsPerMet;
            double area = clothing <= 0.078 ? 1d + 1.29 * clothing : 1.05 + 0.645 * clothing;
            double forced = 12.1 * Math.Sqrt(relativeAirSpeed);
            double airKelvin = airTemperatureC + 273d;
            double radiantKelvin = radiantTemperatureC + 273d;
            double clothingGuess = airKelvin + (35.5 - airTemperatureC) / (3.5 * (6.45 * clothing + 0.1));
            double p1 = clothing * area;
            double p2 = p1 * 3.96;
            double p3 = p1 * 100d;
            double p4 = p1 * airKelvin;
            double p5 = (308.7 - 0.028 * metabolic) + (p2 * Math.Pow(radiantKelvin / 100d, 4));
            double xn = clothingGuess / 100d;
            double xf = clothingGuess / 50d;
            double convection = forced;
            int step = 0;
            while (Math.Abs(xn - xf) > 0.00015)
            {
                xf = (xf + xn) / 2d;
                double natural = 2.38 * Math.Pow(Math.Abs(100d * xf - airKelvin), 0.25);
                convection = Math.Max(natural, forced);
                xn = (p5 + p4 * convection - p2 * Math.Pow(xf, 4)) / (100d + p3 * convection);
                step++;
                if (step > 150 || double.IsNaN(xn) || double.IsInfinity(xn))
                    return result;
            }

            double clothingTemperature = 100d * xn - 273d;
            double skin = 3.05 * 0.001 * (5733d - (6.99 * metabolic) - vapourPressure);
            double sweat = metabolic > wattsPerMet ? 0.42 * (metabolic - wattsPerMet) : 0d;
            double latentBreath = 1.7 * 0.00001 * metabolic * (5867d - vapourPressure);
            double dryBreath = 0.0014 * metabolic * (34d - airTemperatureC);
            double radiation = 3.96 * area * (Math.Pow(xn, 4) - Math.Pow(radiantKelvin / 100d, 4));
            double surface = area * convection * (clothingTemperature - airTemperatureC);
            double sensation = 0.303 * Math.Exp(-0.036 * metabolic) + 0.028;
            double pmv = sensation * (metabolic - skin - sweat - latentBreath - dryBreath - radiation - surface);
            if (double.IsNaN(pmv) || double.IsInfinity(pmv))
                return result;

            result.Solved = true;
            result.PredictedMeanVote = pmv;
            result.PredictedPercentageDissatisfied = 100d - 95d * Math.Exp(-0.03353 * Math.Pow(pmv, 4) - 0.2179 * Math.Pow(pmv, 2));
            result.WithinApplicabilityLimits = airTemperatureC >= 10d && airTemperatureC <= 30d
                && radiantTemperatureC >= 10d && radiantTemperatureC <= 40d
                && relativeAirSpeed >= 0d && relativeAirSpeed <= 1d
                && metabolicRate >= 0.8 && metabolicRate <= 4d
                && clothingInsulation >= 0d && clothingInsulation <= 2d
                && vapourPressure >= 0d && vapourPressure <= 2700d
                && pmv >= -2d && pmv <= 2d;
            return result;
        }
    }
}
