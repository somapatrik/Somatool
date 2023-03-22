using CommunityToolkit.Maui.Views;
using Java.Net;
using Javax.Security.Auth;
using Sharp7;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net;
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

        AutoResetEvent autoEvent = new AutoResetEvent(false);

        private bool _IsBusy;
        public bool IsBusy { get => _IsBusy; set => SetProperty(ref _IsBusy, value); }

        private S7Plc _plc;
        public S7Plc PLC { get => _plc; set => SetProperty(ref _plc, value); }

        private bool _IsConnected;
        public bool IsConnected 
        { 
            get => _IsConnected; 
            set 
            {
                SetProperty(ref _IsConnected, value);
                RefreshCans();
            } 
        }

        private ObservableCollection<S7DataRow> _Addresses;
        public ObservableCollection<S7DataRow> Addresses { get => _Addresses; set => SetProperty(ref _Addresses, value); }

        private bool _pingIP;
        public bool pingIP
        {
            get => _pingIP;
            set
            {
                SetProperty(ref _pingIP, value);
            }
        }

        private int _CpuStatus;
        public int CpuStatus { get => _CpuStatus; set { SetProperty(ref _CpuStatus, value); } }

        #region Commands

        public ICommand LoadAddresses { private set; get; }
        public ICommand AddAddress { private set; get; }
        public ICommand Read { private set; get; }
        public ICommand ConnectToPlc { private set; get; }
        public ICommand DisconnectFromPlc { private set; get; }

        #endregion

        Timer TimeUpdate;

        int UpdateTime = 1000;

        public S7ProfileViewModel(S7Plc plc)
        {
            PLC = plc;

            PlcClient = new S7Client();
            LoadAddresses = new Command(LoadAddressesHandler);
            AddAddress = new Command(AddressPopup);
            Read = new Command(ReadHandler, CanRead);
            DisconnectFromPlc = new Command(DisconnectPlc);
            ConnectToPlc = new Command(ConnectPlc, CanConnect);


            TimeUpdate = new Timer(TimerTick, autoEvent, 500, UpdateTime);

        }

        #region Override

        public void OnAppearing()
        {
            IsBusy = true;
        }

        public void OnDisappearing()
        {
            StopTimer();
        }

        #endregion

        #region Timer

        private void StartTimer()
        {
            TimeUpdate.Change(0, UpdateTime);
        }

        private void StopTimer()
        {
            TimeUpdate.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private void TimerTick(object state)
        {
            try 
            {
                CPUStatus();
                IsS7Connected();
            }
            catch
            {
                CpuStatus = 0;
                IsConnected = false;
            }
        }

        #endregion

        private void IsS7Connected()
        {
            IsConnected = CpuStatus != 0;
        }

        private void CPUStatus()
        {
            int status = 0;
            PlcClient.PlcGetStatus(ref status);
            CpuStatus = status;
        }

        private async Task tryPing()
        {
            Ping pingSender = new Ping();
            PingOptions options = new PingOptions();
            options.DontFragment = true;

            IPAddress pingAdr = IPAddress.Parse(PLC.IP);
            PingReply pingReply = await pingSender.SendPingAsync(pingAdr, 500);
            pingIP = pingReply.Status == IPStatus.Success;

        }

        private void RefreshCans()
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                ((Command)ConnectToPlc).ChangeCanExecute();
                ((Command)Read).ChangeCanExecute();
            });
            //((Command)DisconnectFromPlc).ChangeCanExecute();
        }

        #region Popup

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
                    IsBusy = true;
                }
            }
        }

        #endregion

        #region Addresses
        public async void LoadAddressesHandler()
        {
            IsBusy = true;

            await RefreshAddresses();

            IsBusy = false;
        }

        public async Task RefreshAddresses()
        {
            Addresses = new ObservableCollection<S7DataRow>();
            (await DBV.GetAddresses(PLC.ID)).ForEach(a => Addresses.Add(new S7DataRow(a, ref PlcClient)));
        }

        #endregion

        private bool CanConnect()
        {
            return !IsConnected;
        }
        private bool CanDisconnect()
        {
            return IsConnected;
        }


        private void ConnectPlc()
        {
            PlcClient.ConnectTo(PLC.IP, PLC.Rack, PLC.Slot);
            //StartTimer();
        }
        private void DisconnectPlc()
        {
            PlcClient.Disconnect();
        }

        private bool CanRead()
        {
            return IsConnected;
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
