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

using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;
using System.ComponentModel;

namespace BH.Engine.Psychrometrics
{
    public static partial class Compute
    {
        [Description("Calculates operative temperature from air temperature, mean radiant temperature and relative air speed (ASHRAE 55 Appendix A). Air temperature is weighted 0.5 below 0.2 m/s, 0.6 from 0.2 m/s to 0.6 m/s and 0.7 from 0.6 m/s to 1.0 m/s.")]
        [Input("airTemperature", "Air temperature.", typeof(Temperature))]
        [Input("meanRadiantTemperature", "Mean radiant temperature.", typeof(Temperature))]
        [Input("relativeAirSpeed", "Relative air speed.", typeof(Velocity))]
        [Output("operativeTemperature", "Operative temperature.", typeof(Temperature))]
        public static double OperativeTemperature(double airTemperature, double meanRadiantTemperature, double relativeAirSpeed)
        {
            WarnIfNotKelvin(airTemperature, meanRadiantTemperature);
            if (relativeAirSpeed < 0)
            {
                Base.Compute.RecordError("Relative air speed must be zero or positive.");
                return double.NaN;
            }

            if (relativeAirSpeed > 1.0)
                Base.Compute.RecordWarning("Relative air speed is above 1.0 m/s. The 0.7 weighting is used, which is outside the range given in ASHRAE 55.");

            double weight = relativeAirSpeed < 0.2 ? 0.5 : relativeAirSpeed < 0.6 ? 0.6 : 0.7;
            return weight * airTemperature + (1d - weight) * meanRadiantTemperature;
        }
    }
}
