/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
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
using BH.oM.Reflection.Attributes;

namespace BH.Engine.Psychrometrics
{
    public static partial class Compute
    {
        [Description("Calculates wet-bulb temperature from dry-bulb temperature and relative humidity.")]
        [Input("unitSystem", "SI [IP]")]
        [Input("dryBulbTemperature", "dry-bulb temperature (C) [(F)]")]
        [Input("relativeHumidity", "relative humidity (%)")]
        [Input("pressure", "pressure (Pa) [(Psi)]")]
        [Output("wetBulbTemperature", "wet-bulb temperature (C) [(F)]")]
        public static double WetBulbTemperatureRelativeHumidity(string unitSystem, double dryBulbTemperature, double relativeHumidity, double pressure)
        {
            relativeHumidity = relativeHumidity / 100;

            if (unitSystem == "SI" || unitSystem == string.Empty)
            {
                PsychroLib.Psychrometrics psy = new PsychroLib.Psychrometrics(PsychroLib.UnitSystem.SI);
                return psy.GetTWetBulbFromRelHum(dryBulbTemperature, relativeHumidity, pressure);
            }
            else
            {
                PsychroLib.Psychrometrics psy = new PsychroLib.Psychrometrics(PsychroLib.UnitSystem.IP);
                return psy.GetTWetBulbFromRelHum(dryBulbTemperature, relativeHumidity, pressure);
            }

        }
    }
}
