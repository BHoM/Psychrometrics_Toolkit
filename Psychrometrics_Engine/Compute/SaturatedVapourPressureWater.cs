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
using BH.oM.Quantities.Attributes;

namespace BH.Engine.Psychrometrics
{
    public static partial class Compute
    {
        [Description("Calculates water SaturatedVapourPressure from temperature.")]
        [Input("water temperature", "Water Temperature.", typeof(Temperature))]
        [Output("saturatedVapourPressure", "Saturated Vapour Pressure.", typeof(Pressure))]
        [PreviousVersion("6.3", "BH.Engine.Climate.Compute.SaturatedVapourPressureWater(System.Double)")]
        public static double SaturatedVapourPressureWater(double temperature)
        {
            BH.Engine.Base.Compute.RecordWarning("This method has not been thoroughly tested. The output may be incorrect. Use at own risk.");

            double t = Units.Convert.ToDegreeCelsius(temperature);
            if (t < 0 || t > 150)
            {
                BH.Engine.Base.Compute.RecordError("Water temperature must be greater than 273.15 and less than 423.15 K.");
                return double.NaN;
            }
            else if (Units.Convert.ToDegreeCelsius(temperature) < 21)
            {
                return 6.10830198582769e-03 + 3.69554702125838e-04 * t + 2.4671509929139e-05 * Math.Pow(t, 2);
            }
            else
            {
                return 2.76521518826485e-02 - 1.9984832033515e-03 * t + 1.26386221381836e-04 * Math.Pow(t, 2) - 2.43248314291122E-06 * Math.Pow(t, 3) + 3.97667179186101E-08 * Math.Pow(t, 4) - 2.63438493242063e-10 * Math.Pow(t, 5) + 1.23191485224688e-12 * Math.Pow(t, 6) - 2.20158156672378E-15 * Math.Pow(t, 7);
            }
        }
    }
}
