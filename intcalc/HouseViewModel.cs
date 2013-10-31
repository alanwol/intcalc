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

// http://joshsmithonwpf.wordpress.com/2008/11/14/using-a-viewmodel-to-provide-meaningful-validation-error-messages/

namespace intcalc
{
    class HouseViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
        public HouseViewModel()
        {
            period         = 10;
            mortgagePeriod = 30;
            houseSellPrice = 0m;

            hc = new HouseCalculator();
            rc = new RentCalculator();
            sc = new SavingsCalculator();

            ReadInputData();

            this.PropertyChanged += HandlePropertyChanged;
        }

        private void ReadInputData()
        {
            /*
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
            */
            SavingsCalculator.Deserialize(scFile, ref sc);
            HouseCalculator.Deserialize(hcFile, ref hc);
            RentCalculator.Deserialize(rcFile, ref rc);

            Interest      = sc.Interest;
            MortgageRate  = hc.MortgageRate;
            RentInflation = rc.RentInflation;
        }

        public void Save()
        {
            SavingsCalculator.Serialize(scFile, sc);
            HouseCalculator.Serialize(hcFile, hc);
            RentCalculator.Serialize(rcFile, rc);
        }

        public void CalcHousePrice()
        {
            TotalHousePrice = hc.CalcTotalHousePrice(mortgagePeriod);
            MortgageMonth   = hc.MortgageMonth;
        }

        public void CalcRent()
        {
            TotalRent = rc.CalcTotalRent(period);
        }

        public void CalcSavings()
        {
            TotalSavings = sc.CalcTotalSavings(period);
        }

        public void CalcMoneySaved()
        {
            decimal saved = TotalRent - TotalHousePrice - TotalSavings + HouseSellPrice;
            MoneySaved = saved;
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
            get { return mortgageRate; }
            set
            {
                mortgageRate = value;
                RaisePropertyChanged("MortgageRate");
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

        public decimal MortgageMonth
        {
            get { return mortgageMonth; }
            set
            {
                mortgageMonth = value;
                RaisePropertyChanged("MortgageMonth");
            }
        }

        public decimal TotalHousePrice
        {
            get { return totalHousePrice; }
            set
            {
                totalHousePrice = value;
                RaisePropertyChanged("TotalHousePrice");
            }
        }

        public decimal TotalRent
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
                CalcMoneySaved();
            }
        }

        public decimal RentInflation
        {
            get { return rentInflation; }
            set
            {
                //Debug.Assert(value < 2.0m);
                rentInflation = value;
                RaisePropertyChanged("RentInflation");
                //CalcRent();
                //CalcMoneySaved();
            }
        }

        public decimal TotalSavings
        {
            get { return totalSavings; }
            set
            {
                totalSavings = value;
                RaisePropertyChanged("TotalSavings");
            }
        }

        public decimal MoneySaved
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
            get { return interest; }
            set
            {
                interest = value; 
                RaisePropertyChanged("Interest");
            }
        }

        public int Period
        {
            get { return period; }
            set 
            {
                period = value;
                RaisePropertyChanged("Period");
            }
        }

        public int MortgagePeriod
        {
            get { return mortgagePeriod; }
            set
            {
                mortgagePeriod = value;
                RaisePropertyChanged("MortgagePeriod");
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

        void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Interest":
                    Console.WriteLine("Interest changed.");
                    break;
                case "RentInflation":
                    Console.WriteLine("Rent inflation changed.");
                    break;
                case "MortgagePeriod":
                    Console.WriteLine("Mortgage period changed.");
                    CalcHousePrice();
                    CalcMoneySaved();
                    break;
                case "Period":
                    CalcRent();
                    //CalcHousePrice(); Should be replaced with calculation of paid so far and debt remaining
                    CalcSavings();
                    CalcMoneySaved();
                    break;
            }
        }

        public string Error
        {
            get { return null; }
        }

        public string this[string property]
        {
            get
            {
                switch (property)
                {
                    case "Interest":
                        {
                            decimal it;
                            string val = ValidateInterest(interest, out it);

                            if (!String.IsNullOrEmpty(val)) return val;

                            sc.Interest = it;
                            CalcSavings();
                            CalcMoneySaved();
                        }

                        return sc[property];
                    case "MortgageRate":
                       {
                           decimal rate;
                           string val = ValidateInterest(mortgageRate, out rate);

                           if (!String.IsNullOrEmpty(val)) return val;

                           hc.MortgageRate = rate;
                           CalcHousePrice();
                           CalcMoneySaved();
                       }
                       return hc[property];
                    case "RentInflation":
                       {
                           decimal ri;
                           string val = ValidateInterest(rentInflation, out ri);

                           if (!String.IsNullOrEmpty(val)) return val;

                           rc.RentInflation = ri;
                           CalcRent();
                           CalcMoneySaved();
                       }
                        return rc[property];
                    default:
                        return "Error not a valid property";
                }
            }
        }

        string ValidateInterest(decimal rate, out decimal it)
        {
            it = 1.0m;

            string val = null;

            if (rate >= 1.0m && rate < 2.0m)
            {
                it = rate;
            }
            else val = "Invalid decimal!";

            return val;
        }

        HouseCalculator hc;
        RentCalculator rc;
        SavingsCalculator sc;
        decimal totalHousePrice;
        decimal totalRent;
        decimal totalSavings;
        decimal mortgageMonth;
        decimal moneySaved;
        String scFile = "sc.dat";
        String hcFile = "hc.dat";
        String rcFile = "rc.dat";
        decimal interest;
        decimal mortgageRate;
        decimal rentInflation;
        int period;
        int mortgagePeriod;
        decimal houseSellPrice;
    }
}
