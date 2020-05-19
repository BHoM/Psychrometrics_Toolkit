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
        [Description("Calculates saturation pressure over liquid water for the temperature range -100C to 200C.")]
        [Input("dryBulbTemperature", "dry-bulb temperature (C)")]
        [Output("saturationPressure", "saturation pressure (Pa)")]
        public static double PartialVapourPressure(double dryBulbTemperature)
        {
            PsychroLib.Psychrometrics psy = new PsychroLib.Psychrometrics(PsychroLib.UnitSystem.SI);
            return psy.GetSatVapPres(dryBulbTemperature);
        }
    }
}