using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;

namespace kursova
{
    //Enum for engine types
    enum Engines
    {
        [Description("Diesel")]
        Diesel,

        [Description("Gasoline")]
        Gasoline,

        [Description("Gas")]
        Gas,

        [Description("Electric")]
        Electric,

        [Description("Hybrid")]
        Hybrid
    }

    //abstract class Transport: Brand, EngineType, NumberOfAxels
    abstract class CTransportVehicle
    {
        private string brand;
        private Engines engineType;
        private int numberOfAxles;

        //Getters&Setters
        public string Brand
        {
            get { return brand; }
            set { brand = value; }
        }

        public Engines EngineType
        {
            get { return engineType; }
            set { engineType = value; }
        }

        public int NumberOfAxles
        {
            get { return numberOfAxles; }
            set { numberOfAxles = value; }
        }

        public CTransportVehicle()
        {
        }

        public CTransportVehicle(string brand, 
                                Engines engineType, 
                                int numberOfAxles)
        {
            Brand = brand;
            EngineType = engineType;
            NumberOfAxles = numberOfAxles;
        }

        public CTransportVehicle(CTransportVehicle other)
        {
            Brand = other.Brand;
            EngineType = other.EngineType;
            NumberOfAxles = other.NumberOfAxles;
        }
    }
    //class inherited from TransportVehicle: Number, PassengerCapacity, SeatingCapacity, NumberOfDoors, EnginePower, LowFloor
    class CPublicTransport : CTransportVehicle, IEquatable<CPublicTransport>
    {
        private int number;
        private int passengerCapacity;
        private int seatingCapacity;
        private int numberOfDoors;
        private int enginePower;
        private string lowFloor;


        //Getter&Setters
        public int Number
        {
            get { return number; }
            set { number = value; }
        }
        public int PassengerCapacity
        {
            get { return passengerCapacity; }
            set { passengerCapacity = value; }
        }

        public int SeatingCapacity
        {
            get { return seatingCapacity; }
            set { seatingCapacity = value; }
        }

        public int NumberOfDoors
        {
            get { return numberOfDoors; }
            set { numberOfDoors = value; }
        }

        public int EnginePower
        {
            get { return enginePower; }
            set { enginePower = value; }
        }

        public string LowFloor
        {
            get { return lowFloor; }
            set { lowFloor = value; }
        }

        public CPublicTransport()
        {
        }

        public CPublicTransport(string brand, 
                               Engines engineType, 
                               int numberOfAxles, 
                               int passengerCapacity, 
                               int seatingCapacity, 
                               int numberOfDoors, 
                               int enginePower, 
                               string lowFloor)
            : base(brand, engineType, numberOfAxles)
        {
            PassengerCapacity = passengerCapacity;
            SeatingCapacity = seatingCapacity;
            NumberOfDoors = numberOfDoors;
            EnginePower = enginePower;
            LowFloor = lowFloor;
        }

        public CPublicTransport(CPublicTransport other)
            : base(other)
        {
            Number = other.Number;
            PassengerCapacity = other.PassengerCapacity;
            SeatingCapacity = other.SeatingCapacity;
            NumberOfDoors = other.NumberOfDoors;
            EnginePower = other.EnginePower;
            LowFloor = other.LowFloor;
        }

        //Read one transport from file. Format: Brand Engine Power Axels Places Seating Doors Low \n
        public void ReadFromFile(StreamReader reader)
        {
            try
            {
                string line = reader.ReadLine();

                if (line.Length >= 8 * 2)
                {
                    string[] data = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    string brand = data[0].Trim();
                    string engine = data[1].Trim();
                    int power = int.Parse(data[2].Trim());
                    int axles = int.Parse(data[3].Trim());
                    int places = int.Parse(data[4].Trim());
                    int seating = int.Parse(data[5].Trim());
                    int doors = int.Parse(data[6].Trim());
                    string low = data[7].Trim();

                    if (power < 0 || axles < 0 || places < 0 || seating < 0 || doors < 0)
                    {
                        throw new ArgumentException("Negative values are not allowed.");
                    }

                    Brand = brand;

                    if (Enum.TryParse(engine, out Engines engineTypeEnum))
                    {
                        EngineType = engineTypeEnum;
                    }

                    EnginePower = power;
                    NumberOfAxles = axles;
                    PassengerCapacity = places;
                    SeatingCapacity = seating;
                    NumberOfDoors = doors;
                    LowFloor = low;
                }
                else
                {
                    throw new FormatException($"Invalid line format:\n {line}");
                }
            }
            catch (FormatException ex)
            {
                throw ex;
            }
        }

        //Write one transport to file
        public void WriteToFile(StreamWriter writer)
        {
            //Width between "columns"
            const int columnWidth = 16;
            try
            {
                writer.WriteLine(string.Format("{0,-" + columnWidth + "} {1,-" + columnWidth + "} {2,-" + columnWidth + "} {3,-" + columnWidth + "} {4,-" + columnWidth + "} {5,-" + columnWidth + "} {6,-" + columnWidth + "} {7,-" + columnWidth + "}",
                                Brand, EngineType, EnginePower, NumberOfAxles, PassengerCapacity, SeatingCapacity, NumberOfDoors, LowFloor));
            }
            catch (FormatException ex) {
                throw new FormatException($"Error writing data to file.");
            }
            
        }        
        
        

        //----------Table Managmment---------------------
        public bool Equals(CPublicTransport other)
        {
            if (other == null)
                return false;

            return Number == other.Number;
        }

        public override bool Equals(object obj)
        {
            if (obj is CPublicTransport other)
                return Equals(other);
            return false;
        }

        public override int GetHashCode()
        {
            return Number.GetHashCode();
        }
    }

}

