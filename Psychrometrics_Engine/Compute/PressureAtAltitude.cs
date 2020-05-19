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
        [Description("Calculates atmospheric pressure as a function of altitude.")]
        [Input("altitude", "altitude (m)")]
        [Output("atmosphericPressure", "atmospheric pressure (Pa)")]
        public static double PressureAtAltitude(double altitude)
        {
            PsychroLib.Psychrometrics psy = new PsychroLib.Psychrometrics(PsychroLib.UnitSystem.SI);
            return psy.GetStandardAtmPressure(altitude);
        }
    }
}