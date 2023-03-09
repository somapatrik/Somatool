using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Veverka.Classes;
using Veverka.Models;

namespace Veverka.ViewModels
{
    public class S7ProfileViewModel : PrimeViewModel
    {
        private S7Plc _plc;
        public S7Plc PLC { get => _plc; set => SetProperty(ref _plc, value); }


        public ICommand NewAddress { private set; get; }

        public S7ProfileViewModel(S7Plc plc)
        {
            PLC = plc;
            NewAddress = new Command(NewAddressHandler);
        }


        public async void NewAddressHandler()
        {
            string newAddress = await Shell.Current.DisplayPromptAsync("New address", "");
            AddressFormatter formatter = new AddressFormatter() { Address = newAddress};

            if (formatter.IsValid)
            {
                S7Address s7Address = new S7Address();

            }
        }
    }
}
