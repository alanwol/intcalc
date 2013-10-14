using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.ComponentModel;

namespace intcalc
{
    [Serializable()]
    class RentCalculator : IDataErrorInfo
    {
        public RentCalculator()
        {
            monthlyRent = 500m;
            rentInflation = 1.03m;
        }

        public static void Deserialize(String filename, ref RentCalculator rc)
        {
            if (File.Exists(filename))
            {
                Stream stream = File.OpenRead(filename);
                BinaryFormatter deserializer = new BinaryFormatter();
                rc = (RentCalculator)deserializer.Deserialize(stream);
                stream.Close();
            }
        }

        public static void Serialize(String filename, RentCalculator rc)
        {
            Stream stream = File.Create(filename);
            BinaryFormatter serializer = new BinaryFormatter();
            serializer.Serialize(stream, rc);
            stream.Close();
        }

        public decimal CalcTotalRent(int period)
        {
            totalRent = 0;
            decimal rent = monthlyRent;

            for (int i = 0; i < period; ++i)
            {
                totalRent += 12.0m * rent;
                rent *= rentInflation;
            }

            return totalRent;
        }

        public decimal Rent
        {
            get { return monthlyRent; }
            set { monthlyRent = value; }
        }

        public decimal RentInflation
        {
            get { return rentInflation; }
            set { rentInflation = value; }
        }

        public string Error { get { return null; } }

        public string this[string property]
        {
            get
            {
                if (property == "RentInflation")
                {
                    if (this.RentInflation < 1.0m || this.RentInflation >= 2.0m)
                    {
                        return "Rent inflation cannot be < 1.0 or >= 2.0";
                    }
                }

                return null;
            }
        }

        decimal totalRent;
        decimal monthlyRent;
        decimal rentInflation;
    }
}
