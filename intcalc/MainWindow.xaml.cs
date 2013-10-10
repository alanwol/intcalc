using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Globalization;


namespace intcalc
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        HouseViewModel _viewModel;

        public MainWindow()
        {
            this.Language = XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag);

            InitializeComponent();

            _viewModel = new HouseViewModel();

            base.DataContext = _viewModel;

            sysInfo = (CultureInfo)Thread.CurrentThread.CurrentCulture.Clone();
            sysUIInfo = (CultureInfo)Thread.CurrentThread.CurrentUICulture.Clone();
        }

        private void window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _viewModel.Save();
            Console.WriteLine("Window closing.");
        }

        // TODO rewrite this to IDataError or something

        private bool validateIntegerInput(string input)
        {
            bool validNum = true;

            for (int i = 0; i < input.Length; ++i)
            {
                if (!Char.IsDigit(input[i]))
                {
                    validNum = false;
                    break;
                }
            }

            return validNum;
        }

        private int getIntInput(string input)
        {
            int value = 0;

            if (validateIntegerInput(input))
            {
                try
                {
                    value = Convert.ToInt32(input);
                }
                catch (OverflowException)
                {
                    Console.WriteLine("Number is outside the range of the Int32 type.");
                }
                catch (FormatException)
                {
                    Console.WriteLine("The value is not in a recognizable format.");
                }
            }

            return value;
        }

        private bool validateDecimalInput(string input)
        {
            bool validNum = true;

            bool dotFound = false;

            for (int i = 0; i < input.Length; ++i)
            {
                if (!Char.IsDigit(input[i]))
                {
                    if (!dotFound && ( input[i] == '.' || input[i] == ',') )
                    {
                        dotFound = true;
                        continue;
                    }
                    validNum = false;
                    break;
                }
            }

            return validNum;
        }

        private decimal getDecimalInput(string input)
        {
            decimal value = 0.0m;

            if (validateDecimalInput(input))
            {
                try
                {
                    value = decimal.Parse(input);
                    //value = decimal.Parse(input, CultureInfo.InvariantCulture);
                    //value = Convert.ToDecimal(input);
                }
                catch (OverflowException)
                {
                    Console.WriteLine("Number is outside the range of the Int32 type.");
                }
                catch (FormatException)
                {
                    Console.WriteLine("The value is not in a recognizable format.");
                }
            }

            return value;
        }

        private void numericInt_TextPreview(object sender, TextCompositionEventArgs e)
        {
            if(!validateIntegerInput(e.Text))
            {
                e.Handled = true;
            }
        }

        private void numericReal_TextPreview(object sender, TextCompositionEventArgs e)
        {
            if (!validateDecimalInput(e.Text))
            {
                e.Handled = true;
            }
        }

        private void rentHouseMonth_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (rentHouseMonth.Text.Length != 0)
            {
                _viewModel.Rent = getDecimalInput(rentHouseMonth.Text);
            }
        }

        private void periodYears_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (periodYears.Text.Length != 0)
            {
                _viewModel.Period = getIntInput(periodYears.Text);
            }
        }

        private void rentInflation_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (rentInflation.Text.Length != 0)
            {
                _viewModel.RentInflation = getDecimalInput(rentInflation.Text);
            }
        }

        private void housePrice_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (housePrice.Text.Length != 0)
            {
                _viewModel.HousePrice = getDecimalInput(housePrice.Text);
            }
        }

        private void mortgageRate_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (mortgageRate.Text.Length != 0)
            {
                _viewModel.MortgageRate = getDecimalInput(mortgageRate.Text);
            }
        }

        private void savingsBank_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (savingsBank.Text.Length != 0)
            {
                _viewModel.Savings = getDecimalInput(savingsBank.Text);
            }
        }

        private void interestBank_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (interestBank.Text.Length != 0)
            {
                _viewModel.Interest = getDecimalInput(interestBank.Text);
            }
        }

        private void maintenanceMonth_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (maintenanceMonth.Text.Length != 0)
            {
                _viewModel.Maintenance = getDecimalInput(maintenanceMonth.Text);
            }
        }

        private void houseSellPrice_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (houseSellPrice.Text.Length != 0)
            {
                _viewModel.HouseSellPrice = getDecimalInput(houseSellPrice.Text);
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("nl-NL");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("nl-NL");

            this.Language = XmlLanguage.GetLanguage(Thread.CurrentThread.CurrentCulture.Name);
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Thread.CurrentThread.CurrentCulture = sysInfo;
            Thread.CurrentThread.CurrentUICulture = sysUIInfo;

            this.Language = XmlLanguage.GetLanguage(Thread.CurrentThread.CurrentCulture.Name);
        }

        private CultureInfo sysInfo;
        private CultureInfo sysUIInfo;

        private void mortgagePeriod_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (mortgagePeriod.Text.Length != 0)
            {
                _viewModel.MortgagePeriod = getIntInput(mortgagePeriod.Text);
            }
        }
    }
}
