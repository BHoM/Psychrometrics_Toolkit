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
        [Description("Calculates specific volume from dry-bulb temperature and wet-bulb temperature.")]
        [Input("dryBulbTemperature", "dry-bulb temperature (C)")]
        [Input("wetBulbTemperature", "wet-bulb temperature (C)")]
        [Input("pressure", "pressure (Pa)")]
        [Output("specificVolume", "specific volume(m3/kg)")]
        public static double SpecificVolumeWetBulbTemperature(double dryBulbTemperature, double wetBulbTemperature, double pressure)
        {
            PsychroLib.Psychrometrics psy = new PsychroLib.Psychrometrics(PsychroLib.UnitSystem.SI);
            double humidityRatio = psy.GetHumRatioFromTWetBulb(dryBulbTemperature, wetBulbTemperature, pressure);
            return psy.GetMoistAirVolume(dryBulbTemperature, humidityRatio, pressure);
        }
    }
}