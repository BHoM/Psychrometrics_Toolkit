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
        [Description("Calculates enthalpy from dry-bulb temperature and humidity ratio.")]
        [Input("dryBulbTemperature", "dry-bulb temperature (C)")]
        [Input("humidityRatio", "humidity ratio (kg_water/kg_dryair)")]
        [Output("enthalpy", "enthalpy (J/kg)")]
        public static double EnthalpyHumidityRatio(double dryBulbTemperature, double humidityRatio)
        {
            PsychroLib.Psychrometrics psy = new PsychroLib.Psychrometrics(PsychroLib.UnitSystem.SI);
            return psy.GetMoistAirEnthalpy(dryBulbTemperature, humidityRatio);
        }
    }
}