using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BH.Engine.Psychrometrics
{
    public static partial class Compute
    {
        [Description("Calculates enthalpy from dry-bulb temperature, pressure and one of: humidityRatio, relativeHumidity or wetBulbTemperature.")]
        [Input("dryBulbTemperature", "Dry-bulb temperature (C)")]
        [Input("pressure", "Pressure (Pa)")]
        [Input("humidityRatio", "Humidity ratio (kg_water/kg_dryair)")]
        [Input("relativeHumidity", "Relative humidity (%)")]
        [Input("wetBulbTemperature", "Wet-bulb temperature (C)")]
        [Output("enthalpy", "Enthalpy(J/kg)")]
        public static double Enthalpy(double dryBulbTemperature, double pressure, double humidityRatio = double.MinValue, double relativeHumidity = double.MinValue, double wetBulbTemperature = double.MinValue)
        {
            if (humidityRatio != double.MinValue)
            {
                if (relativeHumidity != double.MinValue || wetBulbTemperature != double.MinValue)
                {
                    BH.Engine.Base.Compute.RecordWarning($"Warning: This function is prioritising humidityRatio for calculating Enthalpy. If you want to use a different parameter, remove humidityRatio.");
                }
                PsychroLib.Psychrometrics psy = new PsychroLib.Psychrometrics(PsychroLib.UnitSystem.SI);
                return psy.GetMoistAirEnthalpy(dryBulbTemperature, humidityRatio);
            }
            if (relativeHumidity != double.MinValue)
            {
                if (wetBulbTemperature != double.MinValue)
                {
                    BH.Engine.Base.Compute.RecordWarning($"Warning: This function is prioritising relativeHumidity for calculating Enthalpy. If you want to use a wetBulbTemperature, remove relativeHumidity.");
                }
                relativeHumidity = relativeHumidity / 100;
                PsychroLib.Psychrometrics psy = new PsychroLib.Psychrometrics(PsychroLib.UnitSystem.SI);
                humidityRatio = psy.GetHumRatioFromRelHum(dryBulbTemperature, relativeHumidity, pressure);
                return psy.GetMoistAirEnthalpy(dryBulbTemperature, humidityRatio);
            }
            if (wetBulbTemperature != double.MinValue)
            {
                PsychroLib.Psychrometrics psy = new PsychroLib.Psychrometrics(PsychroLib.UnitSystem.SI);
                humidityRatio = psy.GetHumRatioFromTWetBulb(dryBulbTemperature, wetBulbTemperature, pressure);
                return psy.GetMoistAirEnthalpy(dryBulbTemperature, humidityRatio);
            }
            else
            {
                BH.Engine.Base.Compute.RecordError($"one of the following parameters must have a value: humidityRatio, relativeHumidity, wetBulbTemperature.");
                return 0;
            }
        }
    }
}
