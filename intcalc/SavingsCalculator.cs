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
    class SavingsCalculator : IDataErrorInfo
    {
        public SavingsCalculator()
        {
            savings = 50000m;
            interest = 1.02m;
        }

        public decimal CalcTotalSavings(int period)
        {
            totalSavings = savings;

            for (int i = 0; i < period; ++i)
            {
                totalSavings *= interest;
            }

            return totalSavings;
        }

        public decimal Savings
        {
            get { return savings; }
            set { savings = value; }
        }

        public decimal Interest
        {
            get { return interest; }
            set { interest = value; }
        }

        public static void Deserialize(String filename, ref SavingsCalculator sc)
        {
            if (File.Exists(filename))
            {
                Stream stream = File.OpenRead(filename);
                BinaryFormatter deserializer = new BinaryFormatter();
                sc = (SavingsCalculator)deserializer.Deserialize(stream);
                stream.Close();
            }
        }

        public static void Serialize(String filename, SavingsCalculator sc)
        {
            Stream stream = File.Create(filename);
            BinaryFormatter serializer = new BinaryFormatter();
            serializer.Serialize(stream, sc);
            stream.Close();
        }

        decimal totalSavings;
        decimal savings;
        decimal interest;

        public string Error { get { return null; } }

        public string this[string property]
        {
            get
            {
                if (property == "Interest")
                {
                    if (this.Interest >= 2.0m)
                    {
                        return "Interest cannot be >= 2.0";
                    }
                }

                return null;
            }
        }
    }
}
