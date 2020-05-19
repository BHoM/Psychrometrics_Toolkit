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
        [Description("Calculates relative humidity from dry-bulb temperature and wet-bulb temperature.")]
        [Input("dryBulbTemperature", "dry-bulb temperature (C)")]
        [Input("wetBulbTemperature", "wet-bulb temperature (C)")]
        [Input("pressure", "pressure (Pa)")]
        [Output("relativeHumidity", "relative humidity (%)")]
        public static double RelativeHumidityWetBulbTemperature(double dryBulbTemperature, double wetBulbTemperature, double pressure)
        {
            PsychroLib.Psychrometrics psy = new PsychroLib.Psychrometrics(PsychroLib.UnitSystem.SI);
            return psy.GetRelHumFromTWetBulb(dryBulbTemperature, wetBulbTemperature, pressure) * 100;
        }
    }
}