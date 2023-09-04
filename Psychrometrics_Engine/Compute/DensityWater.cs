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

namespace BH.Engine.Psychrometrics
{
    public static partial class Compute
    {
        [Description("Calculates water density from temperature.")]
        [Input("temperature", "temperature (K).")]
        [Output("density", "Density (kg/m3).")]
        [PreviousVersion("6.3", "BH.Engine.Climate.Compute.DensityWater(System.Double)")]
        public static double DensityWater(double temperature)
        {
            double t = Units.Convert.ToDegreeCelsius(temperature);
            if (t < 0 || t > 150)
            {
                BH.Engine.Base.Compute.RecordError("Temperature must be greater than 0 and less than 150 degC.");
                return double.NaN;
            }
            else
            {
                return 999.792764532729 + 0.07544354069978 * t - 8.88812233461067e-03 * Math.Pow(t, 2) + 7.43496157156187e-05 * Math.Pow(t, 3) - 5.05819372165206e-07 * Math.Pow(t, 4) + 1.71286251848812e-09 * Math.Pow(t, 5) - 3.41712769191815e-13 * Math.Pow(t, 6) - 8.9940922954777e-15 * Math.Pow(t, 7);
            }
        }
    }
}
