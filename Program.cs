using System;
using System.Dynamic;

namespace DependencyInjectionDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting Main.");
            FloorLamp lamp = new FloorLamp("The lamp");
            lamp.TurnOn();

            SuperSaiyanLamp superSaiyanLamp = new SuperSaiyanLamp("Super Saiyan lamp");
            superSaiyanLamp.TurnOn();

            lamp.TurnOn();

            FloorLamp secondLamp = new FloorLamp("Second lamp");
            secondLamp.TurnOn();
            secondLamp.TurnOn();
        }
    }

    class Electricity
    {
        public double Volts { get; }
        public double Amps { get; }
        public Electricity(double volts, double amps)
        {
            this.Volts = volts;
            this.Amps = amps;
        }
    }

    interface ILamp
    {
        void TurnOn();
    }

    class Lamp
    {
        // Power source can be expensive to make, so lets share one amongst all lamps
        // Beware that static variables are effectively global state!
        public static SuperPowerSource PowerSource = new SuperPowerSource();
    }

    class FloorLamp : ILamp
    {
        public string Name { get; }
        public double AmpsNeeded { get; }
        public double MaximumVoltage { get; }
        public double Lumens { get; }
        private bool isOperational = true;

        public FloorLamp(string name)
        {
            this.Name = name;
            this.AmpsNeeded = 15;
            this.MaximumVoltage = 120;
            this.Lumens = 30;

        }
        public void TurnOn()
        {
            Electricity power = Lamp.PowerSource.GenerateElectricty(this.AmpsNeeded);

            if (power.Volts > this.MaximumVoltage)
            {
                isOperational = false; // Too much voltage burns out the lamp :(
            }

            bool turnedOn = isOperational && (power.Amps >= this.AmpsNeeded);
            if (turnedOn)
            {
                Console.WriteLine($"{this.Name} turned on and produced {this.Lumens} lumens.");
            }
            else
            {
                Console.WriteLine($"Not enough power to turn on {this.Name} :(");
            }
        }
    }

    // New kind of lamp but now we have the Code smell: duplication of code
    class SuperSaiyanLamp : ILamp
    {
        public string Name { get; }
        public double AmpsNeeded { get; }
        public double MaximumVoltage { get; }
        public double Lumens { get; }
        private bool isOperational = true;

        public SuperSaiyanLamp(string name)
        {
            this.Name = name;
            this.AmpsNeeded = 1500;
            this.MaximumVoltage = 120;
            this.Lumens = 9001; // It's over 9000!

        }
        public void TurnOn()
        {
            Electricity power = Lamp.PowerSource.GenerateElectricty(this.AmpsNeeded);

            if (power.Volts > this.MaximumVoltage)
            {
                isOperational = false; // Too much voltage burns out the lamp :(
            }

            bool turnedOn = isOperational && (power.Amps >= this.AmpsNeeded);
            if (turnedOn)
            {
                Console.WriteLine($"{this.Name} turned on and produced {this.Lumens} lumens.");
            }
            else
            {
                Console.WriteLine($"Not enough power to turn on {this.Name} :(");
            }
        }
    }

    interface IPowerSource
    {
        public Electricity GenerateElectricty(double ampsRequested);
    }

    class VariablePowerSource
    {
        private const double Voltage = 120;
        private readonly double MaximumAmperage = 1000;
        private bool isCircuitBlown = false;

        public VariablePowerSource(double maximumAmperage)
        {
            this.MaximumAmperage = maximumAmperage;
            int constructionCost = (int)(15 * maximumAmperage);
            Console.WriteLine($"Constructing a VariablePowerSource ({constructionCost})...");
            System.Threading.Thread.Sleep(constructionCost);
        }

        public Electricity GenerateElectricty(double ampsRequested)
        {
            if (ampsRequested > MaximumAmperage)
            {
                this.isCircuitBlown = true;
            }

            if (this.isCircuitBlown)
            {
                return new Electricity(0, 0);
            }
            else
            {
                return new Electricity(Voltage, ampsRequested);
            }
        }
    }

    // A main power source intended for a house
    class MainPowerSource : IPowerSource
    {
        private readonly VariablePowerSource powerSource;

        public MainPowerSource()
        {
            Console.WriteLine($"Constructing an expensive MainPowerSource ...");
            this.powerSource = new VariablePowerSource(1000);
        }

        public Electricity GenerateElectricty(double ampsRequested)
        {
            return this.powerSource.GenerateElectricty(ampsRequested);
        }
    }

    // A super power source intended for an office building
    class SuperPowerSource : IPowerSource
    {
        private readonly VariablePowerSource powerSource;

        public SuperPowerSource()
        {
            Console.WriteLine($"Constructing a super expensive SuperPowerSource ...");
            this.powerSource = new VariablePowerSource(1500);
        }

        public Electricity GenerateElectricty(double ampsRequested)
        {
            return this.powerSource.GenerateElectricty(ampsRequested);
        }
    }

    class LightweightPowerSource : IPowerSource
    {
        private readonly VariablePowerSource powerSource;

        public LightweightPowerSource()
        {
            Console.WriteLine($"Constructing a LightweightPowerSource ...");
            this.powerSource = new VariablePowerSource(15);
        }

        public Electricity GenerateElectricty(double ampsRequested)
        {
            return this.powerSource.GenerateElectricty(ampsRequested);
        }
    }
}
