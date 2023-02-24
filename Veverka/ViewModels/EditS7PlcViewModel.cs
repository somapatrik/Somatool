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
        public string Name { get => _Name; set => SetProperty(ref _Name, value); }

        private bool _pingIP;
        public bool pingIP 
        { 
            get => _pingIP;
            set
            {
                SetProperty(ref _pingIP, value);
                tryPing();
            }
        }


        private string _IP;
        public string IP 
        { 
            get => _IP;
            set
            {
                SetProperty(ref _IP, value);
            }
        }

        private string _Description;
        public string Description { get => _Description; set => SetProperty(ref _Description, value); }

        private ObservableCollection<PlcGroup> _Groups;
        public ObservableCollection<PlcGroup> Groups { get => _Groups; set => SetProperty(ref _Groups, value); }


        private PlcGroup _SelectedGroup;
        public PlcGroup SelectedGroup { get => _SelectedGroup; set => SetProperty(ref _SelectedGroup, value); }


        public ICommand SavePlc { private set; get; }

        public EditS7PlcViewModel()
        {
            BindCommands();
            LoadGroups();
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
            //Name check
            bool checkName = !string.IsNullOrEmpty(Name);

            return true;
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

        private async void tryPing()
        {
            IPAddress verifyIP;

            if (!string.IsNullOrEmpty(IP) && IPAddress.TryParse(IP,out verifyIP))
            {
                Ping pingSender = new Ping();
                PingOptions options = new PingOptions();
                options.DontFragment = true;

                PingReply pingReply = await pingSender.SendPingAsync(verifyIP,200);
                pingIP = pingReply.Status == IPStatus.Success;
            }
        }
    }
}
