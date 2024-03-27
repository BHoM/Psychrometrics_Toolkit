/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.Base;
using BH.Engine.Base;
using BH.Engine.Units;
using BH.oM.Quantities.Attributes;

namespace BH.Engine.Psychrometrics
{
    public static partial class Compute
    {
        [Description("Calculates density, enthalpy, dew-point temperature, humidity ratio, specific volume and wet-bulb temperature from dry-bulb temperature, pressure and relative humidity.")]
        [Input("dryBulbTemperature", "Dry-bulb temperature.", typeof(Temperature))]
        [Input("relativeHumidity", "Relative humidity (%).")]
        [Input("pressure", "Air pressure, defaults to sea level air pressure (101,325).", typeof(Pressure))]
        [MultiOutput(0, "density", "Density.", typeof(Density))]
        [MultiOutput(1, "enthalpy", "Enthalpy.", typeof(SpecificEnergy))]
        [MultiOutput(2, "dewPoint", "Dew-point temperature.", typeof(Temperature))]
        [MultiOutput(3, "humidityRatio", "Humidity ratio (kg_water/kg_dryair).", typeof(Ratio))]
        [MultiOutput(4, "specificVolume", "Specific Volume.", typeof(VolumePerQuantity))]
        [MultiOutput(5, "wetBulbTemperature", "Wet-bulb temperature.", typeof(Temperature))]
        public static Output<double, double, double, double, double, double> FromRelativeHumidity(
            double dryBulbTemperature,
            double relativeHumidity,
            double pressure = 101325)
        {
            dryBulbTemperature = BH.Engine.Units.Convert.ToDegreeCelsius(dryBulbTemperature);

            double Density = DensityRelativeHumidity(dryBulbTemperature, relativeHumidity, pressure);
            double Enthalpy = EnthalpyRelativeHumidity(dryBulbTemperature, relativeHumidity, pressure);
            double DewPoint = DewPointRelativeHumidity(dryBulbTemperature, relativeHumidity, pressure);
            double HumidityRatio = HumidityRatioRelativeHumidity(dryBulbTemperature, relativeHumidity, pressure);
            double SpecificVolume = SpecificVolumeRelativeHumidity(dryBulbTemperature, relativeHumidity, pressure);
            double WetBulbTemperature = WetBulbTemperatureRelativeHumidity(dryBulbTemperature, relativeHumidity, pressure);

            DewPoint = BH.Engine.Units.Convert.FromDegreeCelsius(DewPoint);
            WetBulbTemperature = BH.Engine.Units.Convert.FromDegreeCelsius(WetBulbTemperature);

            return new Output<double, double, double, double, double, double>
            {
                Item1 = Density,
                Item2 = Enthalpy,
                Item3 = DewPoint,
                Item4 = HumidityRatio,
                Item5 = SpecificVolume,
                Item6 = WetBulbTemperature
            };
        }
    }
}

