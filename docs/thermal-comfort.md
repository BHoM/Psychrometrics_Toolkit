# Thermal comfort

Methods on `BH.Engine.Psychrometrics.Compute` for issue #107. Temperatures on the public inputs and outputs are in kelvin.

- `PredictedMeanVote` calculates PMV and PPD to ISO 7730, and whether the inputs sit inside the standard's applicability limits. External work is zero.
- `PredictedMeanVoteRange` samples clothing insulation and metabolic rate at the ends and the midpoint, and returns the low and high PMV and PPD.
- `AdaptiveComfort` calculates the ASHRAE 55 comfort temperature and the 80% and 90% acceptability bands. The upper limit rises with air speed from 0.3 m/s to 1.2 m/s.
- `PredictedMeanVoteComfortZone` returns the operative-temperature and humidity-ratio outline where PMV is from −0.5 to +0.5, on a 0.1 °C and 1% relative humidity grid.
- `PredictedMeanVoteCategories` returns the percentage of PMV values within ISO 7730 categories A, B and C, and the percentage colder or warmer than category C.
- `PrevailingMeanOutdoorTemperature` calculates the ASHRAE 55 prevailing mean outdoor temperature for each hour of an hourly series, from the mean of the previous 7 to 30 days. The series is treated as a repeating year.
- `OperativeTemperature` weights air temperature and mean radiant temperature by relative air speed, to ASHRAE 55 Appendix A.

## Tested

Built against BHoM 9.3.0.0 and against BHoM 10.0.0.0 (the 4 November sandbox and the 19 November installed assemblies). On each 10.0 build, 25 checks passed. Fifteen of those are pythermalcomfort 4.8.1 cases (`pmv_ppd_iso`, model `7730-2005`) and agree within 1e-6. Dataset rows for `PredictedMeanVote`, `PredictedMeanVoteRange`, and `AdaptiveComfort` are in `.ci/Datasets/Compute/`.

Run on a four-room IES VE annual sample with a London weather file: 12,276 occupied room-hours through `PredictedMeanVote`, `PredictedMeanVoteRange`, `AdaptiveComfort`, `PredictedMeanVoteComfortZone`, `PredictedMeanVoteCategories` and `PrevailingMeanOutdoorTemperature`. PMV and PPD on 300 random hours agree with pythermalcomfort 4.8.1 within output rounding.

## Not tested

`PredictedMeanVoteCategories`, `PrevailingMeanOutdoorTemperature`, `OperativeTemperature` and `PredictedMeanVoteComfortZone` have no dataset files yet.

## Before a review into develop

Not run yet. Run the existing `.ci/Datasets` checks so the current methods still pass, then run the three new dataset files above. Leave issue #107 open until the project-data check is done.
