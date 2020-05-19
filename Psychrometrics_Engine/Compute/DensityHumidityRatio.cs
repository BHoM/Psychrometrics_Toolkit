﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using BH.oM.Reflection.Attributes;

using PsychroLib;


namespace BH.Engine.Psychrometrics
{
    public static partial class Compute
    {
        [Description("Calculates density from dry-bulb temperature and humidity ratio.")]
        [Input("dryBulbTemperature", "dry-bulb temperature (C)")]
        [Input("humidityRatio", "humidity ratio (kg_water/kg_dryair)")]
        [Input("pressure", "pressure (Pa)")]
        [Output("density", "density(kg/m3)")]
        public static double DensityHumidityRatio(double dryBulbTemperature, double humidityRatio, double pressure)
        {
            PsychroLib.Psychrometrics psy = new PsychroLib.Psychrometrics(PsychroLib.UnitSystem.SI);
            return psy.GetMoistAirDensity(dryBulbTemperature, humidityRatio, pressure);
        }
    }
}