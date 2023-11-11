using CommandLine;

class Program{
    class Options{
        [Option("days", Required = false, HelpText = "Numbers of days the simulation runs for. Defaults to 100.")]
        public string days { get; set; }

        [Option("infectionRate", Required = false, HelpText = "Transmission rate of disease. Defaults to 0.0.")]
        public string infectionRate { get; set; }

        [Option("incubationRate", Required = false, HelpText = "How fast exposed incubate to infected. Defaults to 0.0.")]
        public string incubationRate { get; set; }

        [Option("diseaseMortalityRate", Required = false, HelpText = "Rate at which infected die. Defaults to 0.0.")]
        public string diseaseMortalityRate { get; set; }

        [Option("recoveryRate", Required = false, HelpText = "Recovery rate of infected. Defaults to 0.0.")]
        public string recoveryRate { get; set; }

        [Option("vaccinationRate", Required = false, HelpText = "Rate of vaccination of Susceptible. Defaults to 0.0.")]
        public string vaccinationRate { get; set; }

        [Option("infectedQuarantineRate", Required = false, HelpText = "Rate at which infected are quarantined and not longer contribute to exposures. Defaults to 0.0.")]
        public string infectedQuarantineRate { get; set; }

        [Option("recoveredImmunityDecay", Required = false, HelpText = "Rate of decay for immunity gained from disease recovery. Defaults to 0.0.")]
        public string recoveredImmunityDecay { get; set; }

        [Option("vaccineImmunityDecay", Required = false, HelpText = "Rate of decay for immunity gained from vaccination. Defaults to 0.0.")]
        public string vaccineImmunityDecay { get; set; }

        [Option("vaccineEffectiveness", Required = false, HelpText = "Vaccine effectiveness. Defaults to 0.0.")]
        public string vaccineEffectiveness { get; set; }

        [Option('s', "susceptible", Required = false, HelpText = "Initial susceptible population. Defaults to 0.0.")]
        public string s { get; set; }

        [Option('e', "exposed", Required = false, HelpText = "Initial exposed population. Defaults to 0.0.")]
        public string e { get; set; }

        [Option('i', "infected", Required = false, HelpText = "Initial infected population. Defaults to 0.0.")]
        public string i { get; set; }

        [Option('r', "recovered", Required = false, HelpText = "Initial recovered population. Defaults to 0.0.")]
        public string r { get; set; }

        [Option('v', "vaccinated", Required = false, HelpText = "Initial vaccinated population. Defaults to 0.0.")]
        public string v { get; set; }

        [Option('d', "dead", Required = false, HelpText = "Initial dead population. Defaults to 0.0.")]
        public string d { get; set; }

        [Option('o', "outputPath", Required = false, HelpText = "Path to the simulation output file. Defaults to \"output.csv\"")]
        public string outputPath { get; set; }
    }

    public class State{
        public int Day;
        public double S; //susceptible
        public double E; //exposed
        public double I; //infected
        public double R; //recovered
        public double V; //vaccinated
        public double D; //dead

        public State(int day, double s, double e, double i, double r, double v, double d){
            Day = day;
            S = s;
            E = e;
            I = i;
            R = r;
            V = v;
            D = d;
        }
    }
    public static void Main(string[] args){
        int days = 100; // Duration of the simulation

        string outputPath = "output.csv";

        double infectionRate = 0.0;          // Transmission rate of disease
        double incubationRate = 0.0;         // how fast exposed incubate to infected
        double diseaseMortalityRate = 0.0;   //rate at which infected die
        double recoveryRate = 0.0;           // Recovery rate of infected
        double vaccinationRate = 0.0;        // Rate of vaccination of Susceptible
        double infectedQuarantineRate = 0.0; //rate at which infected are quarantined and don't contribute to exposures
        double recoveredImmunityDecay = 0.0; //rate of decay for immunity gained from disease recovery
        double vaccineImmunityDecay = 0.0;   //rate of decay for immunity gained from vaccination
        double vaccineEffectiveness = 0.0;   //Vaccine effectiveness

        double S = 0.0; // Initial susceptible population
        double E = 0.0; // Initial exposed population
        double I = 0.0; // Initial infectious population
        double R = 0.0; // Initial recovered population
        double V = 0.0; // Initial vaccinated population
        double D = 0.0; //Initial dead population

        //assign parsed args to variables
        var result = Parser.Default.ParseArguments<Options>(args)
            .WithParsed(options =>
            {
                days = Int32.Parse(options.days);

                outputPath = options.outputPath;

                infectionRate = Convert.ToDouble(options.infectionRate);
                incubationRate = Convert.ToDouble(options.incubationRate);
                diseaseMortalityRate = Convert.ToDouble(options.diseaseMortalityRate);
                recoveryRate = Convert.ToDouble(options.recoveryRate);
                vaccinationRate = Convert.ToDouble(options.vaccinationRate);
                infectedQuarantineRate = Convert.ToDouble(options.infectedQuarantineRate);
                recoveredImmunityDecay = Convert.ToDouble(options.recoveredImmunityDecay);
                vaccineImmunityDecay = Convert.ToDouble(options.vaccineImmunityDecay);
                vaccineEffectiveness = Convert.ToDouble(options.vaccineEffectiveness);

                S = Convert.ToDouble(options.s);
                E = Convert.ToDouble(options.e);
                I = Convert.ToDouble(options.i);
                R = Convert.ToDouble(options.r);
                V = Convert.ToDouble(options.v);
                D = Convert.ToDouble(options.d);
            });

        if (result.Tag == ParserResultType.NotParsed){
            // Help text requested, or parsing failed. Exit.
            return;
        }

        State previousState = new State(0, S, E, I, R, V, D);

        using(StreamWriter sw = new StreamWriter(outputPath)){
            //write csv column headers
            sw.WriteLine("Day,Susceptible,Exposed,Infected,Recovered,Vaccinated,Dead");

            //record initial state as day 0
            sw.WriteLine($"{previousState.Day},{previousState.S},{previousState.E},{previousState.I},{previousState.R},{previousState.V},{previousState.D}");
            for (int day = 1; day <= days; day++){

                //define terms used in population deltas separately to improve readability
                double infectionFromVTerm = previousState.V * (previousState.E + (previousState.I * (1-infectedQuarantineRate))) * infectionRate * (1-vaccineEffectiveness);
                double infectionFromSTerm = previousState.S * (previousState.E + (previousState.I * (1-infectedQuarantineRate))) * infectionRate;
                double diseaseMortalityTerm = previousState.I * diseaseMortalityRate;
                double vaccineImmunityDecayTerm = previousState.V * vaccineImmunityDecay;
                double recoveredImmuneDecayTerm = previousState.R * recoveredImmunityDecay;
                double vaccinationRateTerm = previousState.S * vaccinationRate;
                double incubationRateTerm = previousState.E * incubationRate;
                double recoveryRateTerm = previousState.I * recoveryRate;
                
                //calculate new population deltas
                S = previousState.S + (vaccineImmunityDecayTerm + recoveredImmuneDecayTerm - vaccinationRateTerm - infectionFromSTerm);
                E = previousState.E + (infectionFromVTerm + infectionFromSTerm - incubationRateTerm);
                I = previousState.I + (incubationRateTerm - recoveryRateTerm - diseaseMortalityTerm);
                R = previousState.R + (recoveryRateTerm - recoveredImmuneDecayTerm);
                V = previousState.V + (vaccinationRateTerm - vaccineImmunityDecayTerm - infectionFromVTerm);
                D = previousState.D + (diseaseMortalityTerm);

                //normalize the values to attempt to sum to 100%
                double sum = S + E + I + R + V + D;
                S /= sum;
                E /= sum;
                I /= sum;
                R /= sum;
                V /= sum;
                D /= sum;

                previousState = new State(day, S, E, I, R, V, D);
                sw.WriteLine($"{previousState.Day},{previousState.S},{previousState.E},{previousState.I},{previousState.R},{previousState.V},{previousState.D}");;
            }
        }
        Console.WriteLine("Simulation Finished. Written to " + outputPath);  
    }
}
