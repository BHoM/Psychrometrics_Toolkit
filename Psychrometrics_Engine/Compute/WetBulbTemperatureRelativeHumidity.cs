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
        [Description("Calculates wet-bulb temperature from dry-bulb temperature and relative humidity.")]
        [Input("dryBulbTemperature", "dry-bulb temperature (C)")]
        [Input("relativeHumidity", "relative humidity (%)")]
        [Input("pressure", "pressure (Pa)")]
        [Output("wetBulbTemperature", "wet-bulb temperature (C)")]
        public static double WetBulbTemperatureRelativeHumidity(double dryBulbTemperature, double relativeHumidity, double pressure)
        {
            relativeHumidity = relativeHumidity / 100;
            PsychroLib.Psychrometrics psy = new PsychroLib.Psychrometrics(PsychroLib.UnitSystem.SI);
            return psy.GetTWetBulbFromRelHum(dryBulbTemperature, relativeHumidity, pressure);
        }
    }
}