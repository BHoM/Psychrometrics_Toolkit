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
        [Description("Calculates temperature as a function of enthalpy.")]
        [Input("enthalpy", "enthalpy (J/kg)")]
        [Input("humidityRatio", "humidity ratio (g/g)")]
        [Output("temperature", "temperature (C)")]
        public static double TemperatureEnthalpyHumidityRatio(double enthalpy, double humidityRatio)
        {
            PsychroLib.Psychrometrics psy = new PsychroLib.Psychrometrics(PsychroLib.UnitSystem.SI);
            return psy.GetTDryBulbFromEnthalpyAndHumRatio(enthalpy, humidityRatio);
        }
    }
}