using System;
using System.Dynamic;

namespace DependencyInjectionDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting Main.");
            TestFloorLamps();
        }

        static void TestFloorLamps()
        {
            IPowerSource lightweightPowerSource = new LightweightPowerSource();
            FloorLamp lamp = new FloorLamp("The lamp", lightweightPowerSource);
            lamp.TurnOn();
            lamp.TurnOn();

            // Now that powerSource is exposed as a constructor argument I have the flexibility to use an existing IPowerSource
            // or use a different one for isolation
            FloorLamp secondLamp = new FloorLamp("Second lamp", lightweightPowerSource);
            secondLamp.TurnOn();
            secondLamp.TurnOn();

            IPowerSource isolatedLightweightPowerSource = new LightweightPowerSource();
            FloorLamp thirdLamp = new FloorLamp("Third lamp", isolatedLightweightPowerSource);
            thirdLamp.TurnOn();
            thirdLamp.TurnOn();
        }

        static void TestSuperSaiyanLamp()
        {
            IPowerSource lightweightPowerSource = new SuperPowerSource();
            SuperSaiyanLamp superSaiyanLamp = new SuperSaiyanLamp("Super Saiyan lamp", lightweightPowerSource);
            superSaiyanLamp.TurnOn();
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

    class FloorLamp : ILamp
    {
        public string Name { get; }
        public double AmpsNeeded { get; }
        public double MaximumVoltage { get; }
        public double Lumens { get; }
        private bool isOperational = true;

        private IPowerSource powerSource;

        // "Don't look for things; ask for things"
        // Simply take our dependency (powerSource) as a constructor argument
        public FloorLamp(string name, IPowerSource powerSource)
        {
            this.Name = name;
            this.AmpsNeeded = 15;
            this.MaximumVoltage = 120;
            this.Lumens = 30;

            this.powerSource = powerSource;
        }
        public void TurnOn()
        {
            Electricity power = this.powerSource.GenerateElectricty(this.AmpsNeeded);

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

        private IPowerSource powerSource;

        public SuperSaiyanLamp(string name, IPowerSource powerSource)
        {
            this.Name = name;
            this.AmpsNeeded = 1500;
            this.MaximumVoltage = 120;
            this.Lumens = 9001; // It's over 9000!

            this.powerSource = powerSource;
        }
        public void TurnOn()
        {
            Electricity power = this.powerSource.GenerateElectricty(this.AmpsNeeded);

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
