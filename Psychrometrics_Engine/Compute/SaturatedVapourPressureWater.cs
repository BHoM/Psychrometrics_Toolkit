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
        [Description("Calculates water SaturatedVapourPressure from temperature")]
        [Input("temperature", "temperature [degC]")]
        [Output("saturatedVapourPressure", "Saturated Vapour Pressure [Pa]")]
        [PreviousVersion("6.2", "BH.Engine.Climate.Compute.SaturatedVapourPressureWater(System.Double)")]
        public static double SaturatedVapourPressureWater(double temperature)
        {
            BH.Engine.Base.Compute.RecordWarning("This method has not been thoroughly tested. The output may be incorrect. Use at own risk.");

            double t = temperature;
            if (t < 0 || t > 150)
            {
                BH.Engine.Base.Compute.RecordError("Temperature must be greater than 0 and less than 150 degC");
                return -1;
            }
            else if (temperature < 21)
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
