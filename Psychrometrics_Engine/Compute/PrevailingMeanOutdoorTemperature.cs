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

using System.Collections.Generic;
using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;
using System.ComponentModel;

namespace BH.Engine.Psychrometrics
{
    public static partial class Compute
    {
        [Description("Calculates the ASHRAE 55 prevailing mean outdoor temperature for each hour of an hourly outdoor temperature series, as the arithmetic mean of the daily mean outdoor temperatures over the previous days. The series is treated as a repeating year, so the first days use the days at the end of the series.")]
        [Input("hourlyOutdoorTemperature", "Hourly outdoor dry-bulb temperature, for example from a weather file. The series starts at the first hour of a day and covers whole days.", typeof(Temperature))]
        [Input("days", "Number of previous days averaged, from 7 to 30.")]
        [Output("prevailingMeanOutdoorTemperature", "Prevailing mean outdoor temperature for each hour of the input.", typeof(Temperature))]
        public static List<double> PrevailingMeanOutdoorTemperature(List<double> hourlyOutdoorTemperature, int days = 7)
        {
            if (hourlyOutdoorTemperature == null || hourlyOutdoorTemperature.Count == 0 || hourlyOutdoorTemperature.Count % 24 != 0)
            {
                Base.Compute.RecordError("The hourly outdoor temperature must cover whole days, 24 values per day.");
                return new List<double>();
            }

            int dayCount = hourlyOutdoorTemperature.Count / 24;
            if (days < 7 || days > 30 || days >= dayCount)
            {
                Base.Compute.RecordError("The number of previous days must be from 7 to 30, and less than the number of days in the series.");
                return new List<double>();
            }

            double[] dailyMean = new double[dayCount];
            bool celsius = false;
            for (int day = 0; day < dayCount; day++)
            {
                double sum = 0;
                for (int hour = 0; hour < 24; hour++)
                {
                    double value = hourlyOutdoorTemperature[day * 24 + hour];
                    if (value < 100)
                        celsius = true;
                    sum += value;
                }

                dailyMean[day] = sum / 24d;
            }

            if (celsius)
                Base.Compute.RecordWarning("It looks like you have entered a temperature in Celsius/Fahrenheit instead of Kelvin. Check your inputs.");

            List<double> prevailing = new List<double>(hourlyOutdoorTemperature.Count);
            for (int day = 0; day < dayCount; day++)
            {
                double sum = 0;
                for (int previous = 1; previous <= days; previous++)
                    sum += dailyMean[(day - previous + dayCount) % dayCount];

                double mean = sum / days;
                for (int hour = 0; hour < 24; hour++)
                    prevailing.Add(mean);
            }

            return prevailing;
        }
    }
}
