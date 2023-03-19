using CommunityToolkit.Maui.Views;
using Java.Net;
using Javax.Security.Auth;
using Sharp7;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Veverka.Classes;
using Veverka.Models;
using Veverka.Services;
using Veverka.Views;

namespace Veverka.ViewModels
{
    public class S7ProfileViewModel : PrimeViewModel
    {
        public S7Client PlcClient;
        
        private bool _IsBusy;
        public bool IsBusy { get => _IsBusy; set => SetProperty(ref _IsBusy, value); }

        private S7Plc _plc;
        public S7Plc PLC { get => _plc; set => SetProperty(ref _plc, value); }

        private ObservableCollection<S7DataRow> _Addresses;
        public ObservableCollection<S7DataRow> Addresses { get => _Addresses; set => SetProperty(ref _Addresses, value); }

        #region Commands

        public ICommand LoadAddresses { private set; get; }
        public ICommand AddAddress { private set; get; }
        public ICommand Read { private set; get; }

        #endregion

        Timer TimeUpdate;

        public S7ProfileViewModel(S7Plc plc)
        {
            PLC = plc;

            PlcClient = new S7Client();
            LoadAddresses = new Command(LoadAddressesHandler);
            AddAddress = new Command(AddressPopup);
            Read = new Command(ReadHandler);

            TimeUpdate = new Timer(TimerTick, null, 0, 1000);
        }

        private void TimerTick(object state)
        {

        }

        public void OnAppearing()
        {
            // LoadAddressesHandler();
            IsBusy = true;
        }

        public async void AddressPopup()
        {
            var popup = new AddressPopup();
            AddressPopupViewModel addressViewModel = (AddressPopupViewModel) await Shell.Current.ShowPopupAsync(popup);
            
            if (addressViewModel != null) 
            { 
                AddressFormatter formatter = new AddressFormatter() { Address = addressViewModel.outAddress };
                
                if (formatter.IsValid)
                {
                    S7Address s7Address = new S7Address()
                    {
                        PLC_ID = PLC.ID,
                        DisplayName = addressViewModel.Name,
                        RawAddress = formatter.Address
                    };

                    await DBV.CreateAddress(s7Address);
                }
            }
        }

        public async void LoadAddressesHandler()
        {
            IsBusy = true;

            //ObservableCollection<S7DataRow> loadAdrresses = new ObservableCollection<S7DataRow>();
            Addresses = new ObservableCollection<S7DataRow>();
            (await DBV.GetAddresses(PLC.ID)).ForEach(a => Addresses.Add(new S7DataRow(a, ref PlcClient)));
            //Addresses = loadAdrresses;

            IsBusy = false;
        }

        private void ConnectPlc()
        {
            PlcClient.ConnectTo(PLC.IP, 0, 2);
        }

        private void ReadHandler()
        {
            ConnectPlc();
            foreach (S7DataRow addressRow in Addresses)
            {
                addressRow.ReadFromPLC();
            }

        }

    }
}
