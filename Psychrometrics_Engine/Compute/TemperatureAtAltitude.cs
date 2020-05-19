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
        [Description("Calculates temperature as a function of altitude.")]
        [Input("altitude", "altitude (m)")]
        [Output("temperature", "temperature (C)")]
        public static double TemperatureAtAltitude(double altitude)
        {
            PsychroLib.Psychrometrics psy = new PsychroLib.Psychrometrics(PsychroLib.UnitSystem.SI);
            return psy.GetStandardAtmTemperature(altitude);
        }
    }
}