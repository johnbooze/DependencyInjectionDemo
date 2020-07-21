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
            lamp.TurnOn();
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

    class FloorLamp
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
            MainPowerSource powerSource = new MainPowerSource();
            Electricity power = powerSource.GenerateElectricty(this.AmpsNeeded);

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

    // A main power source intended for a house
    class MainPowerSource
    {
        private const double Voltage = 120;
        private const double MaximumAmperage = 1000;
        private bool isCircuitBlown = false;

        public MainPowerSource()
        {
            Console.WriteLine("Constructing an expensive MainPowerSource...");
            System.Threading.Thread.Sleep(5000);
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

}
