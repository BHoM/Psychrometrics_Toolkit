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
        [Description("Calculates humidity ratio from dry-bulb temperature and relative humidity.")]
        [Input("dryBulbTemperature", "dry-bulb temperature (C)")]
        [Input("relativeHumidity", "relative humidity (%)")]
        [Input("pressure", "pressure (Pa)")]
        [Output("humidityRatio", "humidity ratio (kg_water/kg_dryair)")]
        public static double HumidityRatioRelativeHumidity(double dryBulbTemperature, double relativeHumidity, double pressure)
        {
            relativeHumidity = relativeHumidity / 100;
            PsychroLib.Psychrometrics psy = new PsychroLib.Psychrometrics(PsychroLib.UnitSystem.SI);
            return psy.GetHumRatioFromRelHum(dryBulbTemperature, relativeHumidity, pressure);
        }
    }
}
