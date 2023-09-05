/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
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
        [Description("Calculates density, enthalpy, dew-point temperature, humidity ratio, relative humidity and specific volume from dry-bulb temperature, pressure and wet-bulb temperature.")]
        [Input("dryBulbTemperature", "Dry-bulb temperature.", typeof(Temperature))]
        [Input("wetBulbTemperature", "Wet-bulb temperature.", typeof(Temperature))]
        [Input("pressure", "Air pressure, defaults to sea level air pressure (101,325).", typeof(Pressure))]
        [MultiOutput(0, "density", "Density.", typeof(Density))]
        [MultiOutput(1, "enthalpy", "Enthalpy.", typeof(SpecificEnergy))]
        [MultiOutput(2, "dewPoint", "Dew-point temperature.", typeof(Temperature))]
        [MultiOutput(3, "humidityRatio", "Humidity ratio (kg_water/kg_dryair).", typeof(Ratio))]
        [MultiOutput(4, "relativeHumidity", "Relative humidity (%).")]
        [MultiOutput(5, "specificVolume", "Specific Volume.", typeof(VolumePerQuantity))]
        [PreviousVersion("6.3", "BH.Engine.Psychrometrics.Compute.DensityWetBulbTemperature(System.Double, System.Double, System.Double)")]
        [PreviousVersion("6.3", "BH.Engine.Psychrometrics.Compute.EnthalpyWetBulbTemperature(System.Double, System.Double, System.Double)")]
        [PreviousVersion("6.3", "BH.Engine.Psychrometrics.Compute.DewPointWetBulbTemperature(System.Double, System.Double, System.Double)")]
        [PreviousVersion("6.3", "BH.Engine.Psychrometrics.Compute.HumidityRatioWetBulbTemperature(System.Double, System.Double, System.Double)")]
        [PreviousVersion("6.3", "BH.Engine.Psychrometrics.Compute.RelativeHumidityWetBulbTemperature(System.Double, System.Double, System.Double)")]
        [PreviousVersion("6.3", "BH.Engine.Psychrometrics.Compute.SpecificVolumeWetBulbTemperature(System.Double, System.Double, System.Double)")]
        public static Output<double, double, double, double, double, double> FromWetBulbTemperature(
            double dryBulbTemperature,
            double wetBulbTemperature,
            double pressure = 101325)
        {
            dryBulbTemperature = BH.Engine.Units.Convert.ToDegreeCelsius(dryBulbTemperature);
            wetBulbTemperature = BH.Engine.Units.Convert.ToDegreeCelsius(wetBulbTemperature);

            double Density = DensityWetBulbTemperature(dryBulbTemperature, wetBulbTemperature, pressure);
            double Enthalpy = EnthalpyWetBulbTemperature(dryBulbTemperature, wetBulbTemperature, pressure);
            double DewPoint = DewPointWetBulbTemperature(dryBulbTemperature, wetBulbTemperature, pressure);
            double HumidityRatio = HumidityRatioWetBulbTemperature(dryBulbTemperature, wetBulbTemperature, pressure);
            double RelativeHumidity = RelativeHumidityWetBulbTemperature(dryBulbTemperature, wetBulbTemperature, pressure);
            double SpecificVolume = SpecificVolumeWetBulbTemperature(dryBulbTemperature, wetBulbTemperature, pressure);

            DewPoint = BH.Engine.Units.Convert.FromDegreeCelsius(DewPoint);

            return new Output<double, double, double, double, double, double>
            {
                Item1 = Density,
                Item2 = Enthalpy,
                Item3 = DewPoint,
                Item4 = HumidityRatio,
                Item5 = RelativeHumidity,
                Item6 = SpecificVolume
            };
        }
    }
}
