﻿using Microsoft.Maui.Graphics.Text;
using Sharp7;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Veverka.Models;
using Veverka.Services;

namespace Veverka.ViewModels
{
    public class EditS7PlcViewModel : PrimeViewModel
    {
        private S7Plc _plc;
        public S7Plc Plc { get => _plc; set => SetProperty(ref _plc, value); }

        private string _Name;
        public string Name { get => _Name; set { SetProperty(ref _Name, value); RefreshCans(); } }

        private bool _pingIP;
        public bool pingIP 
        { 
            get => _pingIP;
            set
            {
                SetProperty(ref _pingIP, value);
            }
        }

        private string _IP;
        public string IP 
        { 
            get => _IP;
            set
            {
                SetProperty(ref _IP, value);
                if (!string.IsNullOrEmpty(_IP) && TryIP())
                    tryPing();
                else
                    pingIP = false;

                RefreshCans();
            }
        }

        private bool _isIP;
        public bool isIP
        {
            get => _isIP; set => SetProperty(ref _isIP, value);
        }

        private string _Description;
        public string Description { get => _Description; set => SetProperty(ref _Description, value); }

        private int _Rack;
        public int Rack { get => _Rack; set => SetProperty(ref _Rack, value); }

        private int _Slot;
        public int Slot { get => _Slot; set => SetProperty(ref _Slot, value); }

        private bool _IsS7Connection;
        public bool IsS7Connection { get => _IsS7Connection; set => SetProperty(ref _IsS7Connection, value); }

        private ObservableCollection<PlcGroup> _Groups;
        public ObservableCollection<PlcGroup> Groups { get => _Groups; set => SetProperty(ref _Groups, value); }

        private PlcGroup _SelectedGroup;
        public PlcGroup SelectedGroup { get => _SelectedGroup; set => SetProperty(ref _SelectedGroup, value); }


        public ICommand SavePlc { private set; get; }

        private List<string> UsedIP;

        public EditS7PlcViewModel()
        {
            BindCommands();
            LoadGroups();
            LoadUsedIPs();
        }

        private async void LoadUsedIPs()
        {
            UsedIP = (await DBV.GetAllPlcs()).Select(p => p.IP).ToList();
        }

        private async void LoadGroups()
        {
            Groups = new ObservableCollection<PlcGroup>
            {
                new PlcGroup() { ID = 0, Name = "<None>" }
            };

            (await DBV.GetAllPlcGroups()).ForEach(g => Groups.Add(g));

        }

        private void BindCommands()
        {
            SavePlc = new Command(SavePlcHandler, CanSave);
        }

        private bool CanSave()
        {
            bool checkName = !string.IsNullOrEmpty(Name);
            bool checkIP = isIP;
            bool checkRackSlot = Rack >= 0 && Slot >= 0;

            bool checkIPName = false;
            if (checkIP)
                checkIPName = !UsedIP.Contains(IP);


            return checkIP && checkName && checkRackSlot && checkIPName ;
        }

        private void RefreshCans()
        {
            ((Command)SavePlc).ChangeCanExecute();
        }

        private async void SavePlcHandler()
        {
            S7Plc savePlc = new S7Plc()
            {
                Name = Name,
                IP = IP,
                Description = Description,
                Group_ID = SelectedGroup == null ? 0 : SelectedGroup.ID
            };

            await DBV.CreatePlc(savePlc);
            await Shell.Current.GoToAsync("..");
        }

        private bool TryIP()
        {
            isIP = false;

            if (!string.IsNullOrEmpty(IP))
            {
                isIP = IPAddress.TryParse(IP, out _);
            }
            return isIP;
        }

        private async void tryPing()
        {
            Ping pingSender = new Ping();
            PingOptions options = new PingOptions();
            options.DontFragment = true;

            IPAddress pingAdr = IPAddress.Parse(IP);
            PingReply pingReply = await pingSender.SendPingAsync(pingAdr,200);
            pingIP = pingReply.Status == IPStatus.Success;

        }
    
        private async Task<bool> TestS7()
        {
            bool testConnect = false;

            try
            {
                S7Client client = new S7Client();
                testConnect = client.ConnectTo(IP, Rack, Slot) == 1;
            }
            catch
            {

            }

            //IsS7Connection = testConnect;
            return testConnect;
        }

    }
}
