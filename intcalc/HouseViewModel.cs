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
            houseSellPrice = 0m;

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
            totalHousePrice = hc.CalcTotalHousePrice(period);
            TotalHousePrice = totalHousePrice.ToString("c", numf);
            MortgageMonth = hc.MortgageMonth.ToString( "c", numf );
        }

        public void CalcRent()
        {
            totalRent = rc.CalcTotalRent(period);
            TotalRent = totalRent.ToString("c", numf );
        }

        public void CalcSavings()
        {
            totalSavings = sc.CalcTotalSavings(period);
            TotalSavings = totalSavings.ToString( "c", numf );
        }

        public void CalcMoneySaved()
        {
            decimal saved = totalRent - totalHousePrice - totalSavings + houseSellPrice;
            MoneySaved = saved.ToString("c", numf);
        }

        public decimal HousePrice
        {
            get { return hc.HousePrice; }
            set
            {
                hc.HousePrice = value;
                CalcHousePrice();
                CalcMoneySaved();
            }
        }

        public decimal HouseSellPrice
        {
            get { return houseSellPrice; }
            set
            {
                houseSellPrice = value;
                CalcMoneySaved();
            }
        }

        public decimal MortgageRate
        {
            get { return hc.MortgageRate; }
            set
            {
                hc.MortgageRate = value;
                CalcHousePrice();
                CalcMoneySaved();
            }
        }

        public decimal Maintenance
        {
            get { return hc.MaintenanceMonth; }
            set
            {
                hc.MaintenanceMonth = value;
                CalcHousePrice();
                CalcMoneySaved();
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
            get { return totalHousePriceString; }
            set
            {
                totalHousePriceString = value;
                RaisePropertyChanged("TotalHousePrice");
            }
        }

        public String TotalRent
        {
            get { return totalRentString; }
            set 
            { 
                totalRentString = value;
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
                CalcMoneySaved();
            }
        }

        public decimal RentInflation
        {
            get { return rc.RentInflation; }
            set
            {
                rc.RentInflation = value;
                CalcRent();
                CalcMoneySaved();
            }
        }

        public String TotalSavings
        {
            get { return totalSavingsString; }
            set
            {
                totalSavingsString = value;
                RaisePropertyChanged("TotalSavings");
            }
        }

        public String MoneySaved
        {
            get { return moneySaved; }
            set
            {
                moneySaved = value;
                RaisePropertyChanged("MoneySaved");
            }
        }

        public decimal Savings
        {
            get { return sc.Savings; }
            set
            {
                sc.Savings = value;
                CalcSavings();
                CalcMoneySaved();
            }
        }

        public decimal Interest
        {
            get { return sc.Interest; }
            set
            {
                sc.Interest = value;
                CalcSavings();
                CalcMoneySaved();
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
                CalcMoneySaved();
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
        decimal totalHousePrice;
        decimal totalRent;
        decimal totalSavings;
        String totalHousePriceString;
        String totalRentString;
        String totalSavingsString;
        String mortgageMonth;
        String moneySaved;
        String scFile = "sc.dat";
        String hcFile = "hc.dat";
        String rcFile = "rc.dat";
        int period;
        decimal houseSellPrice;

        NumberFormatInfo numf;
    }
}
