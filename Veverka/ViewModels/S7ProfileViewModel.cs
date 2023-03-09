using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private bool _IsBusy;
        public bool IsBusy { get => _IsBusy; set => SetProperty(ref _IsBusy, value); }

        private S7Plc _plc;
        public S7Plc PLC { get => _plc; set => SetProperty(ref _plc, value); }

        private ObservableCollection<S7Address> _Addresses;
        public ObservableCollection<S7Address> Addresses { get => _Addresses; set => SetProperty(ref _Addresses, value); }

        public ICommand LoadAddresses { private set; get; }
        public ICommand NewAddress { private set; get; }

        public S7ProfileViewModel(S7Plc plc)
        {
            PLC = plc;
            
            NewAddress = new Command(NewAddressHandler);
            LoadAddresses = new Command(LoadAddressesHandler);
        }

        public async void LoadAddressesHandler()
        {
            
        }

        public async void NewAddressHandler()
        {
            string newAddress = await Shell.Current.DisplayPromptAsync("New address", "");
            AddressFormatter formatter = new AddressFormatter() { Address = newAddress};

            if (formatter.IsValid)
            {
                S7Address s7Address = new S7Address()
                {
                    PLC_ID = PLC.ID,
                     RawAddress = formatter.Address
                };



            }
        }
    }
}
