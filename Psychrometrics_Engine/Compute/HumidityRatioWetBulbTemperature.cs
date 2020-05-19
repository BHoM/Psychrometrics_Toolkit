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
        [Description("Calculates humidity ratio from dry-bulb temperature and wet-bulb temperature.")]
        [Input("dryBulbTemperature", "dry-bulb temperature (C)")]
        [Input("wetBulbTemperature", "wet-bulb temperature (C)")]
        [Input("pressure", "pressure (Pa)")]
        [Output("humidityRatio", "humidity ratio (kg_water/kg_dryair)")]
        public static double HumidityRatioWetBulbTemperature(double dryBulbTemperature, double wetBulbTemperature, double pressure)
        {
            PsychroLib.Psychrometrics psy = new PsychroLib.Psychrometrics(PsychroLib.UnitSystem.SI);
            return psy.GetHumRatioFromTWetBulb(dryBulbTemperature, wetBulbTemperature, pressure);
        }
    }
}