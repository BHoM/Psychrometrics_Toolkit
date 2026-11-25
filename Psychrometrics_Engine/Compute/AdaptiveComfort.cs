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
        [Description("Calculates the ASHRAE 55 adaptive comfort temperature and the 80% and 90% acceptability bands from the prevailing mean outdoor temperature and air speed. The upper limit rises as air speed increases from 0.3 m/s to 1.2 m/s.")]
        [Input("prevailingMeanOutdoorTemperature", "Prevailing mean outdoor temperature.", typeof(Temperature))]
        [Input("airSpeed", "Air speed.", typeof(Velocity))]
        [MultiOutput(0, "comfortTemperature", "Adaptive comfort temperature.", typeof(Temperature))]
        [MultiOutput(1, "acceptability80Low", "Lower 80% acceptability temperature.", typeof(Temperature))]
        [MultiOutput(2, "acceptability80High", "Upper 80% acceptability temperature.", typeof(Temperature))]
        [MultiOutput(3, "acceptability90Low", "Lower 90% acceptability temperature.", typeof(Temperature))]
        [MultiOutput(4, "acceptability90High", "Upper 90% acceptability temperature.", typeof(Temperature))]
        [MultiOutput(5, "withinApplicabilityLimits", "True when the prevailing mean outdoor temperature is inside the ASHRAE 55 applicability range.")]
        public static Output<double, double, double, double, double, bool> AdaptiveComfort(double prevailingMeanOutdoorTemperature, double airSpeed)
        {
            if (prevailingMeanOutdoorTemperature < 100)
                Base.Compute.RecordWarning("It looks like you have entered a temperature in Celsius/Fahrenheit instead of Kelvin. Check your inputs.");

            double outdoorC = prevailingMeanOutdoorTemperature.ToDegreeCelsius();
            double comfortC = 0.31 * outdoorC + 17.8;
            double rise = AdaptiveUpperRise(airSpeed);
            bool withinLimits = outdoorC >= 10d && outdoorC <= 33.5;
            if (!withinLimits)
                Base.Compute.RecordWarning("Outside the ASHRAE 55 applicability limits. Adaptive comfort is less reliable for these inputs.");

            return new Output<double, double, double, double, double, bool>
            {
                Item1 = comfortC.FromDegreeCelsius(),
                Item2 = (comfortC - 3.5).FromDegreeCelsius(),
                Item3 = (comfortC + 3.5 + rise).FromDegreeCelsius(),
                Item4 = (comfortC - 2.5).FromDegreeCelsius(),
                Item5 = (comfortC + 2.5 + rise).FromDegreeCelsius(),
                Item6 = withinLimits
            };
        }

        private static double AdaptiveUpperRise(double airSpeed)
        {
            if (airSpeed <= 0.3)
                return 0d;
            if (airSpeed >= 1.2)
                return 2.2;
            if (airSpeed <= 0.6)
                return (airSpeed - 0.3) / 0.3 * 1.2;
            if (airSpeed <= 0.9)
                return 1.2 + (airSpeed - 0.6) / 0.3 * 0.6;
            return 1.8 + (airSpeed - 0.9) / 0.3 * 0.4;
        }
    }
}
