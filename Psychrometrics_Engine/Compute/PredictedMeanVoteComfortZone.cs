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
using BH.Engine.Units;
using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;
using System.ComponentModel;

namespace BH.Engine.Psychrometrics
{
    public static partial class Compute
    {
        [Description("Calculates the operative-temperature and humidity-ratio outline of the ISO 7730 comfort zone, where predicted mean vote is from -0.5 to 0.5, for a range of metabolic rate and clothing insulation. Operative temperature stands in for both air temperature and mean radiant temperature.")]
        [Input("relativeAirSpeed", "Relative air speed.", typeof(Velocity))]
        [Input("metabolicRateLow", "Low metabolic rate (met).")]
        [Input("metabolicRateHigh", "High metabolic rate (met).")]
        [Input("clothingInsulationLow", "Low clothing insulation (clo).")]
        [Input("clothingInsulationHigh", "High clothing insulation (clo).")]
        [Input("pressure", "Air pressure, defaults to sea level air pressure (101,325).", typeof(Pressure))]
        [Output("comfortZone", "Comfort-zone bands. Each band is operative temperature, low humidity ratio and high humidity ratio.")]
        public static List<Output<List<double>, List<double>, List<double>>> PredictedMeanVoteComfortZone(double relativeAirSpeed, double metabolicRateLow, double metabolicRateHigh, double clothingInsulationLow, double clothingInsulationHigh, double pressure = 101325)
        {
            if (relativeAirSpeed < 0 || pressure <= 0)
            {
                Base.Compute.RecordError("Relative air speed must be zero or positive, and air pressure must be greater than zero.");
                return new List<Output<List<double>, List<double>, List<double>>>();
            }

            double[] clothing = Samples(clothingInsulationLow, clothingInsulationHigh);
            double[] metabolic = Samples(metabolicRateLow, metabolicRateHigh);
            List<List<double>> openTemperature = new List<List<double>>();
            List<List<double>> openLow = new List<List<double>>();
            List<List<double>> openHigh = new List<List<double>>();
            List<Output<List<double>, List<double>, List<double>>> finished = new List<Output<List<double>, List<double>, List<double>>>();
            for (int i = 0; i <= 44; i++)
            {
                double temperature = 14d + i * 0.5;
                List<double> spanLow = new List<double>();
                List<double> spanHigh = new List<double>();
                bool open = false;
                double low = 0;
                double high = 0;
                for (int humidity = 0; humidity <= 100; humidity += 2)
                {
                    if (!ComfortZoneContains(temperature, humidity, relativeAirSpeed, metabolic, clothing, pressure, out double grams))
                    {
                        if (open)
                        {
                            spanLow.Add(low);
                            spanHigh.Add(high);
                            open = false;
                        }

                        continue;
                    }

                    if (!open)
                    {
                        open = true;
                        low = grams;
                        high = grams;
                    }
                    else
                    {
                        if (grams < low)
                            low = grams;
                        if (grams > high)
                            high = grams;
                    }
                }

                if (open)
                {
                    spanLow.Add(low);
                    spanHigh.Add(high);
                }

                bool[] taken = new bool[spanLow.Count];
                List<List<double>> nextTemperature = new List<List<double>>();
                List<List<double>> nextLow = new List<List<double>>();
                List<List<double>> nextHigh = new List<List<double>>();
                for (int run = 0; run < openTemperature.Count; run++)
                {
                    int last = openLow[run].Count - 1;
                    int best = -1;
                    double bestOverlap = -1;
                    for (int span = 0; span < spanLow.Count; span++)
                    {
                        if (taken[span])
                            continue;
                        double overlap = Math.Min(openHigh[run][last], spanHigh[span]) - Math.Max(openLow[run][last], spanLow[span]);
                        if (overlap < -0.05 || overlap <= bestOverlap)
                            continue;
                        best = span;
                        bestOverlap = overlap;
                    }

                    if (best < 0)
                    {
                        finished.Add(ComfortZoneBand(openTemperature[run], openLow[run], openHigh[run]));
                        continue;
                    }

                    taken[best] = true;
                    openTemperature[run].Add(temperature);
                    openLow[run].Add(spanLow[best]);
                    openHigh[run].Add(spanHigh[best]);
                    nextTemperature.Add(openTemperature[run]);
                    nextLow.Add(openLow[run]);
                    nextHigh.Add(openHigh[run]);
                }

                for (int span = 0; span < spanLow.Count; span++)
                {
                    if (taken[span])
                        continue;
                    List<double> temperatures = new List<double>();
                    List<double> lows = new List<double>();
                    List<double> highs = new List<double>();
                    temperatures.Add(temperature);
                    lows.Add(spanLow[span]);
                    highs.Add(spanHigh[span]);
                    nextTemperature.Add(temperatures);
                    nextLow.Add(lows);
                    nextHigh.Add(highs);
                }

                openTemperature = nextTemperature;
                openLow = nextLow;
                openHigh = nextHigh;
            }

            for (int run = 0; run < openTemperature.Count; run++)
                finished.Add(ComfortZoneBand(openTemperature[run], openLow[run], openHigh[run]));
            return finished;
        }

        private static bool ComfortZoneContains(double temperatureC, double relativeHumidity, double relativeAirSpeed, double[] metabolicRate, double[] clothingInsulation, double pressure, out double gramsPerKilogram)
        {
            gramsPerKilogram = double.NaN;
            for (int m = 0; m < metabolicRate.Length; m++)
            {
                for (int c = 0; c < clothingInsulation.Length; c++)
                {
                    MeanVoteResult sample = PredictedMeanVoteFromCelsius(temperatureC, temperatureC, relativeAirSpeed, relativeHumidity, metabolicRate[m], clothingInsulation[c]);
                    if (!sample.Solved || sample.PredictedMeanVote < -0.5 || sample.PredictedMeanVote > 0.5)
                        continue;

                    gramsPerKilogram = HumidityRatioGramsPerKilogram(temperatureC, relativeHumidity, pressure);
                    if (double.IsNaN(gramsPerKilogram) || gramsPerKilogram < 0 || gramsPerKilogram > 40)
                        continue;
                    return true;
                }
            }

            return false;
        }

        private static double HumidityRatioGramsPerKilogram(double temperatureC, double relativeHumidity, double pressure)
        {
            double vapourPressure = relativeHumidity * 10d * Math.Exp(16.6536 - 4030.183 / (temperatureC + 235d));
            if (vapourPressure <= 0 || vapourPressure >= 100000d)
                return double.NaN;

            return 0.62198 * vapourPressure / (pressure - vapourPressure) * 1000d;
        }

        private static Output<List<double>, List<double>, List<double>> ComfortZoneBand(List<double> temperatureC, List<double> lowGramsPerKilogram, List<double> highGramsPerKilogram)
        {
            List<double> temperature = new List<double>(temperatureC.Count);
            List<double> low = new List<double>(lowGramsPerKilogram.Count);
            List<double> high = new List<double>(highGramsPerKilogram.Count);
            for (int i = 0; i < temperatureC.Count; i++)
            {
                temperature.Add(temperatureC[i].FromDegreeCelsius());
                low.Add(lowGramsPerKilogram[i] / 1000d);
                high.Add(highGramsPerKilogram[i] / 1000d);
            }

            return new Output<List<double>, List<double>, List<double>>
            {
                Item1 = temperature,
                Item2 = low,
                Item3 = high
            };
        }
    }
}
