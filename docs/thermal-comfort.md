# Thermal comfort

Methods on `BH.Engine.Psychrometrics.Compute` for issue #107. Temperatures on the public inputs and outputs are in kelvin.

- `PredictedMeanVote` calculates PMV and PPD to ISO 7730, and whether the inputs sit inside the standard's applicability limits. External work is zero.
- `PredictedMeanVoteRange` samples clothing insulation and metabolic rate at the ends and the midpoint, and returns the low and high PMV and PPD.
- `AdaptiveComfort` calculates the ASHRAE 55 comfort temperature and the 80% and 90% acceptability bands. The upper limit rises with air speed from 0.3 m/s to 1.2 m/s.
- `PredictedMeanVoteComfortZone` returns the operative-temperature and humidity-ratio outline where PMV is from −0.5 to +0.5.

## Tested

Built against BHoM 9.3.0.0 and against BHoM 10.0.0.0 (the 4 November sandbox and the 19 November installed assemblies). On each 10.0 build, 25 checks passed. Fifteen of those are pythermalcomfort 4.8.1 cases (`pmv_ppd_iso`, model `7730-2005`) and agree within 1e-6. Dataset rows for `PredictedMeanVote`, `PredictedMeanVoteRange`, and `AdaptiveComfort` are in `.ci/Datasets/Compute/`.

## Not tested

These methods have not been verified against actual project data. That check is still open on issue #107.

## Before a review into develop

Not run yet. Run the existing `.ci/Datasets` checks so the current methods still pass, then run the three new dataset files above. Leave issue #107 open until the project-data check is done.
