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
using BH.Engine.Base;

namespace BH.Engine.Psychrometrics
{
    public static partial class Compute
    {
        [Description("Calculates relative humidity from dry-bulb temperature, pressure and one of: humidityRatio, relativeHumidity or wetBulbTemperature.")]
        [Input("dryBulbTemperature", "Dry-bulb temperature (C)")]
        [Input("pressure", "Pressure (Pa)")]
        [Input("humidityRatio", "Humidity ratio (kg_water/kg_dryair)")]
        [Input("wetBulbTemperature", "Wet-bulb temperature (C)")]
        [Output("relativeHumidity", "Relative humidity (%)")]
        [PreviousVersion("6.3", "BH.Engine.Psychrometrics.Compute.RelativeHumidityHumidityRatio(System.Double, System.Double, System.Double)")]
        [PreviousVersion("6.3", "BH.Engine.Psychrometrics.Compute.RelativeHumidityWetBulbTemperature(System.Double, System.Double, System.Double)")]
        public static double RelativeHumidity(double dryBulbTemperature, double pressure, double humidityRatio = double.MinValue, double wetBulbTemperature = double.MinValue)
        {
            if (humidityRatio != double.MinValue)
            {
                if (wetBulbTemperature != double.MinValue)
                {
                    BH.Engine.Base.Compute.RecordWarning($"Warning: This function is prioritising humidityRatio for calculating relativeHumidity. If you want to use a wetBulbTemperature, remove relativeHumidity.");
                }
                PsychroLib.Psychrometrics psy = new PsychroLib.Psychrometrics(PsychroLib.UnitSystem.SI);
                return psy.GetRelHumFromHumRatio(dryBulbTemperature, humidityRatio, pressure) * 100;
            }
            if (wetBulbTemperature != double.MinValue)
            {
                PsychroLib.Psychrometrics psy = new PsychroLib.Psychrometrics(PsychroLib.UnitSystem.SI);
                return psy.GetRelHumFromTWetBulb(dryBulbTemperature, wetBulbTemperature, pressure) * 100;
            }
            else
            {
                BH.Engine.Base.Compute.RecordError($"one of the following parameters must have a value: humidityRatio, relativeHumidity, wetBulbTemperature.");
                return 0;
            }
        }
    }
}