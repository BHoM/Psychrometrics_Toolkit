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

namespace BH.Engine.Psychrometrics
{
    public static partial class Compute
    {
        [Description("Calculates Water SpecificHeatCapacity from temperature.")]
        [Input("temperature", "temperature (K).")]
        [Output("specificHeatCapacity", "Specific Heat Capacity (kJ/kgK).")]
        [PreviousVersion("6.3", "BH.Engine.Climate.Compute.SpecificHeatCapacityWater(System.Double)")]
        public static double SpecificHeatCapacityWater(double temperature)
        {
            // add temperature conversion here
            double t = temperature;
            if (t < 0 || t > 150)
            {
                BH.Engine.Base.Compute.RecordError("Temperature must be greater than 0 and less than 150 degC.");
                return double.NaN;
            }
            else
            {
                return 4.21704317315562 - 3.38336617551249e-03 * t + 1.15883761850455e-04 * Math.Pow(t, 2) - 2.08959843561729e-06 * Math.Pow(t, 3) + 2.27639253003176e-08 * Math.Pow(t, 4) - 1.42019051900533e-10 * Math.Pow(t, 5) + 4.7949488211343e-13 * Math.Pow(t, 6) - 6.7653778811732e-16 * Math.Pow(t, 7);
            }
        }
    }
}
