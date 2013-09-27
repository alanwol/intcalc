using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.IO;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;
using System.Globalization;
using System.Threading;

namespace intcalc
{
    class HouseViewModel : INotifyPropertyChanged
    {
        public HouseViewModel()
        {
            period    = 10;
            hc = new HouseCalculator();
            rc = new RentCalculator();
            sc = new SavingsCalculator();

            ReadInputData();

            CultureInfo nl = new CultureInfo("nl-NL");
            numf = nl.NumberFormat;
        }

        private void ReadInputData()
        {
            try
            {
                using (StreamReader sr = new StreamReader("input.txt"))
                {
                    List<String> lines = new List<string>();
                    String line;
                    while((line = sr.ReadLine()) != null)
                    {
                        Console.WriteLine(line);
                        lines.Add(line);
                    }

                    hc.HousePrice   = Convert.ToDecimal(lines[0]);
                    period          = Convert.ToInt32(lines[1]);
                    hc.MortgageRate = Convert.ToDecimal(lines[2]);

                    rc.Rent          = Convert.ToDecimal(lines[3]);
                    rc.RentInflation = Convert.ToDecimal(lines[4]);
                    sc.Savings       = Convert.ToDecimal(lines[5]);
                    sc.Interest      = Convert.ToDecimal(lines[6]);

                    Debug.Assert(lines.Count == 7);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }

            SavingsCalculator.Deserialize(scFile, ref sc);
            HouseCalculator.Deserialize(hcFile, ref hc);
            RentCalculator.Deserialize(rcFile, ref rc);
        }

        public void Save()
        {
            SavingsCalculator.Serialize(scFile, sc);
            HouseCalculator.Serialize(hcFile, hc);
            RentCalculator.Serialize(rcFile, rc);
        }

        public void CalcHousePrice()
        {
            TotalHousePrice = hc.CalcTotalHousePrice(period).ToString("c", numf);
            MortgageMonth = hc.MortgageMonth.ToString( "c", numf );
        }

        public void CalcRent()
        {
            TotalRent = rc.CalcTotalRent(period).ToString("c", numf );
        }

        public void CalcSavings()
        {
            TotalSavings = sc.CalcTotalSavings(period).ToString( "c", numf );
        }

        public decimal HousePrice
        {
            get { return hc.HousePrice; }
            set
            {
                hc.HousePrice = value;
                CalcHousePrice();
            }
        }

        public decimal MortgageRate
        {
            get { return hc.MortgageRate; }
            set
            {
                hc.MortgageRate = value;
                CalcHousePrice();
            }
        }

        public decimal Maintenance
        {
            get { return hc.MaintenanceMonth; }
            set
            {
                hc.MaintenanceMonth = value;
                CalcHousePrice();
            }
        }

        public String MortgageMonth
        {
            get { return mortgageMonth; }
            set
            {
                mortgageMonth = value;
                RaisePropertyChanged("MortgageMonth");
            }
        }

        public String TotalHousePrice
        {
            get { return totalHousePrice; }
            set
            {
                totalHousePrice = value;
                RaisePropertyChanged("TotalHousePrice");
            }
        }

        public String TotalRent
        {
            get { return totalRent; }
            set 
            { 
                totalRent = value;
                RaisePropertyChanged("TotalRent");
            }
        }

        public decimal Rent
        {
            get { return rc.Rent; }
            set
            {
                rc.Rent = value;
                CalcRent();
            }
        }

        public decimal RentInflation
        {
            get { return rc.RentInflation; }
            set
            {
                rc.RentInflation = value;
                CalcRent();
            }
        }

        public String TotalSavings
        {
            get { return totalSavings; }
            set
            {
                totalSavings = value;
                RaisePropertyChanged("TotalSavings");
            }
        }

        public decimal Savings
        {
            get { return sc.Savings; }
            set
            {
                sc.Savings = value;
                CalcSavings();
            }
        }

        public decimal Interest
        {
            get { return sc.Interest; }
            set
            {
                sc.Interest = value;
                CalcSavings();
            }
        }

        public int Period
        {
            get { return period; }
            set 
            {
                period = value;
                CalcRent();
                CalcHousePrice();
                CalcSavings();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            // take a copy to prevent thread issues
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        HouseCalculator hc;
        RentCalculator rc;
        SavingsCalculator sc;
        String totalHousePrice;
        String totalRent;
        String totalSavings;
        String mortgageMonth;
        String scFile = "sc.dat";
        String hcFile = "hc.dat";
        String rcFile = "rc.dat";
        int period;

        NumberFormatInfo numf;
    }
}
