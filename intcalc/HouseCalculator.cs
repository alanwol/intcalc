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
    class HouseCalculator : IDataErrorInfo
    {
        public HouseCalculator()
        {
            mortgageRate = 1.05m;
            maintenanceMonth = 150.0m;
            housePrice = 100000m;
        }

        private decimal CalcMaintenance(int period)
        {
            decimal totalSum = 0m;
            decimal main = maintenanceMonth;

            for (int i = 0; i < period; ++i)
            {
                totalSum += 12.0m * main;
                main *= 1.02m;
            }

            return totalSum;
        }

        public decimal CalcTotalHousePrice(int period)
        {
            if (period == 0 || mortgageRate < 0.001m)
            {
                return 0m;
            }

            int numMonths = 12 * period;

            if (Math.Abs(mortgageRate - 1.0m) < 0.001m)
            {
                mortgageMonth = housePrice / numMonths;
                totalHousePrice = housePrice + CalcMaintenance(period);
                return totalHousePrice;
            }

            decimal mortgageRateMonth = (decimal)Math.Pow((double)mortgageRate, 1.0 / 12.0) -1.0m;

            // http://nl.wikipedia.org/wiki/Annu%C3%AFteit Annuiteiten formule
            mortgageMonth = housePrice * (mortgageRateMonth)/(1.0m - (decimal)Math.Pow((double)(mortgageRateMonth+1.0m),-numMonths));

            totalHousePrice = mortgageMonth * numMonths + CalcMaintenance(period);

            return totalHousePrice;
        }

        public static void Deserialize(String filename, ref HouseCalculator hc)
        {
            if (File.Exists(filename))
            {
                Stream stream = File.OpenRead(filename);
                BinaryFormatter deserializer = new BinaryFormatter();
                hc = (HouseCalculator)deserializer.Deserialize(stream);
                stream.Close();
            }
        }

        public static void Serialize(String filename, HouseCalculator hc)
        {
            Stream stream = File.Create(filename);
            BinaryFormatter serializer = new BinaryFormatter();
            serializer.Serialize(stream, hc);
            stream.Close();
        }

        public decimal HousePrice
        {
            get { return housePrice; }
            set { housePrice = value; }
        }

        public decimal MortgageRate
        {
            get { return mortgageRate; }
            set { mortgageRate = value; }
        }

        public decimal MortgageMonth
        {
            get { return mortgageMonth; }
            set { mortgageMonth = value; }
        }

        public decimal MaintenanceMonth
        {
            get { return maintenanceMonth; }
            set { maintenanceMonth = value; }
        }

        public string Error { get { return null; } }

        public string this[string property]
        {
            get
            {
                if (property == "MortgageRate")
                {
                    if (this.MortgageRate < 1.0m || this.MortgageRate >= 2.0m)
                    {
                        return "Mortgage rate cannot be < 1.0 or >= 2.0";
                    }
                }

                return null;
            }
        }

        decimal totalHousePrice;
        decimal housePrice;
        decimal mortgageRate;
        decimal mortgageMonth;
        decimal maintenanceMonth;
    }
}
