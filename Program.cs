using System;
using System.Linq;

namespace DependencyInjectionDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Run();
            Console.WriteLine("----");
            Tests();
        }

        // Simulation of the production service
        static void Run()
        {
            Console.WriteLine("Running the service");
            Lamp lamp = new Lamp("The lamp");
            lamp.TurnOn();
        }

        // For simplicity of demonstration we'll write the tests here instead of a dedicated unit test project
        static void Tests()
        {
            Console.WriteLine("Running tests");

            Lamp lamp = new Lamp("First lamp");
            Console.WriteLine("Test - Turn on");
            Assert(lamp.TurnOn().Lumens > 0, "Turning on lamp produced light");

            Lamp secondLamp = new Lamp("Second lamp");
            Console.WriteLine("Test - second lamp can turn on for 4 hours producing at least 100 lumens");
            foreach (var iteration in Enumerable.Range(1, 4))
            {
                Light lightProduced = lamp.TurnOn();
                Assert(lightProduced.Lumens > 100, $"Iteration {iteration} Produced at least 100 lumens");
            }
        }

        static void Assert(bool isTrue, string message)
        {
            string result = isTrue ? "PASS" : "FAIL";
            Console.WriteLine("  " + result + ": " + message);
        }
    }

    class Lamp
    {
        public string Name { get; }
        public double AmpsNeeded { get; }
        public double MaximumVoltage { get; }
        private bool isOperational = true;

        public Lamp(string name)
        {
            this.Name = name;
            this.AmpsNeeded = 1;
            this.MaximumVoltage = 120;
        }

        // Turns on the lamp for one hour
        public Light TurnOn()
        {
            Electricity power = null; // TODO: Need electricity to turn on the lamp

            if (power.Volts > this.MaximumVoltage)
            {
                isOperational = false; // Too much voltage burns out the lamp :(
            }

            bool turnedOn = isOperational && (power.Amps >= this.AmpsNeeded);
            Light light = new Light(power.Volts * power.Amps);
            if (turnedOn)
            {
                Console.WriteLine($"{this.Name} turned on and produced {light.Lumens} lumens.");
            }
            else
            {
                Console.WriteLine($"Not enough power to turn on {this.Name} :(");
            }

            return light;
        }
    }

    class PowerGenerator
    {
        private readonly double Voltage = 120;
        private readonly double MaximumAmperage = 10;
        private double wattHoursRemaining;
        private bool isCircuitBlown = false;

        public PowerGenerator(double wattHourCapacity)
        {
            this.wattHoursRemaining = wattHourCapacity;

            int cost = Convert.ToInt32(Voltage * MaximumAmperage * wattHourCapacity / 1000) + 1;
            Console.WriteLine($"Constructing a PowerGenerator(cost:{cost})...");
            System.Threading.Thread.Sleep(cost);
        }

        // Generate electricity for one hour
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

            Electricity requestedElectricty =  new Electricity(Voltage, ampsRequested);
            if (this.wattHoursRemaining >= requestedElectricty.Watts)
            {
                this.wattHoursRemaining -= requestedElectricty.Watts;
                return requestedElectricty;
            }
            else
            {
                this.wattHoursRemaining = 0;
                double remainingAmps = this.wattHoursRemaining / this.Voltage;
                return new Electricity(this.Voltage, remainingAmps);
            }
        }
    }

    class Electricity
    {
        public double Volts { get; }
        public double Amps { get; }
        public double Watts
        {
            get
            {
                return this.Volts * this.Amps;
            }
        }
        public Electricity(double volts, double amps)
        {
            this.Volts = volts;
            this.Amps = amps;
        }

        public override string ToString()
        {
            return $"Volts: {this.Volts} Amps: {this.Amps}";
        }
    }

    class Light
    {
        public double Lumens { get; }
        public Light(double lumens)
        {
            this.Lumens = lumens;
        }

        public override string ToString()
        {
            return $"Lumens: {this.Lumens}";
        }
    }
}
