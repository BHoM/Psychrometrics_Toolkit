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
        [Description("Calculates specific volume from dry-bulb temperature and humidity ratio.")]
        [Input("dryBulbTemperature", "dry-bulb temperature (C)")]
        [Input("humidityRatio", "humidity ratio (kg_water/kg_dryair)")]
        [Input("pressure", "pressure (Pa)")]
        [Output("specificVolume", "specificVolume(m3/kg)")]
        public static double SpecificVolumeHumidityRatio(double dryBulbTemperature, double humidityRatio, double pressure)
        {
            PsychroLib.Psychrometrics psy = new PsychroLib.Psychrometrics(PsychroLib.UnitSystem.SI);
            return psy.GetMoistAirVolume(dryBulbTemperature, humidityRatio, pressure);
        }
    }
}
