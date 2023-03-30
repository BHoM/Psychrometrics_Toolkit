using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using BH.oM.Base.Attributes;


namespace BH.Engine.Psychrometrics
{
    public static partial class Compute
    {
        [Description("Calculates Water SpecificHeatCapacity from temperature")]
        [Input("temperature", "temperature [degC]")]
        [Output("specificHeatCapacity", "Specific Heat Capacity [kJ/kgK]")]
        [PreviousVersion("6.2", "BH.Engine.Climate.Compute.SpecificHeatCapacityWater(System.Double)")]
        public static double SpecificHeatCapacityWater(double temperature)
        {
            double t = temperature;
            if (t < 0 || t > 150)
            {
                BH.Engine.Base.Compute.RecordError("Temperature must be greater than 0 and less than 150 degC");
                return -1;
            }
            else
            {
                return 4.21704317315562 - 3.38336617551249e-03 * t + 1.15883761850455e-04 * Math.Pow(t, 2) - 2.08959843561729e-06 * Math.Pow(t, 3) + 2.27639253003176e-08 * Math.Pow(t, 4) - 1.42019051900533e-10 * Math.Pow(t, 5) + 4.7949488211343e-13 * Math.Pow(t, 6) - 6.7653778811732e-16 * Math.Pow(t, 7);
            }
        }
    }
}