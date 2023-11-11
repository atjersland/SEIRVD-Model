# Susceptible Exposed Infected Recovered Vaccinated Dead (SEIRVD) Model Simulator

## Description

Simulation of a SEIRVD model written in C#. Writes per day outputs to CSV for easy ingestion into a visualizer.

## Compartments
- **Susceptible**: The population of people that are suceptible to infection.

- **Exposed**: The population of people that have been exposed to disease and are infectious without showing symptoms

- **Infected**: The population of people that are actively infected and infectious.

- **Recovered**: The population of people that have recovered from the infection.

- **Vaccinated**: The population of otherwise susceptible people that are vaccinated against the disease

- **Dead**: The population of people that have died from infection

## Simulation Math
### Formulas for Constituent Terms
	infectionFromVTerm = previousState.V * (previousState.E + (previousState.I * (1-infectedQuarantineRate))) * infectionRate * (1-vaccineEffectiveness);
	infectionFromSTerm = previousState.S * (previousState.E + (previousState.I * (1-infectedQuarantineRate))) * infectionRate;
	diseaseMortalityTerm = previousState.I * diseaseMortalityRate;
	vaccineImmunityDecayTerm = previousState.V * vaccineImmunityDecay;
	recoveredImmuneDecayTerm = previousState.R * recoveredImmunityDecay;
	vaccinationRateTerm = previousState.S * vaccinationRate;
	incubationRateTerm = previousState.E * incubationRate;
	recoveryRateTerm = previousState.I * recoveryRate;

### New Populaton Delta Equations
	S = previousState.S + (vaccineImmunityDecayTerm + recoveredImmuneDecayTerm - vaccinationRateTerm - infectionFromSTerm);
	E = previousState.E + (infectionFromVTerm + infectionFromSTerm - incubationRateTerm);
	I = previousState.I + (incubationRateTerm - recoveryRateTerm - diseaseMortalityTerm);
	R = previousState.R + (recoveryRateTerm - recoveredImmuneDecayTerm);
	V = previousState.V + (vaccinationRateTerm - vaccineImmunityDecayTerm - infectionFromVTerm);
	D = previousState.D + (diseaseMortalityTerm);

### Values are normalized to force values to sum to ~99% at the end of each iteration. This prevents compounding floating point multiplication errors.
	double sum = newS + newE + newI + newR + newV + newD;
    newS /= sum;
    newE /= sum;
    newI /= sum;
    newR /= sum;
    newV /= sum;
    newD /= sum;

## Usage
	seirvd.exe --days 100 --infectionRate 0.3 --incubationRate 0.07 --diseaseMortalityRate 0.05 --recoveryRate 0.03 --vaccinationRate 0.08 --infectedQuarantineRate 0.2 --recoveredImmunityDecay 0.15 --vaccineImmunityDecay 0.05 --vaccineEffectiveness 0.3 -s 0.98 -e 0.02 -i 0.0 -r 0.0 -v 0.0 -d 0.0 -o results.csv

## Model Inputs
| Input Parameter | Description | Default Value |
| :--- | :--- | :--- |
| --days | Duration of the simulation | 100 |
| --infectionRate | Transmission rate of disease | 0.0 |
| --incubationRate | how fast exposed incubate to infected | 0.0 |
| --diseaseMortalityRate | rate at which infected die | 0.0 |
| --recoveryRate | Recovery rate of infected | 0.0 |
| --vaccinationRate | Rate of vaccination of Susceptible | 0.0 |
| --infectedQuarantineRate | rate at which infected are quarantined and not longer contribute to exposures | 0.0 |
| --recoveredImmunityDecay |  rate of decay for immunity gained from disease recovery | 0.0 |
| --vaccineImmunityDecay | rate of decay for immunity gained from vaccination | 0.0 |
| --vaccineEffectiveness | Vaccine effectiveness | 0.0 |
| -s, --susceptible | Initial susceptible population | 0.0 |
| -e, --exposed | Initial exposed population | 0.0 |
| -i, --infected | Initial infectious population | 0.0 |
| -r, --recovered | Initial recovered population | 0.0 |
| -v, --vaccinated | Initial vaccinated population | 0.0 |
| -d, --dead | Initial dead population | 0.0 |
| -o, --outputPath | Path to the simulation output file. | "output.csv" |

# Model Limitations
- The model does not consider natural birth and death rates within the population.
- Similar to the typical SIR model, it assumes a homogeneously mixed population.