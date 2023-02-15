//using Android.Accounts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Veverka.Models;
using Veverka.Services;
using Veverka.Views;

namespace Veverka.ViewModels
{
    public class MainViewModel : PrimeViewModel
    {
        private ObservableCollection<S7Plc> _Plcs;
        public ObservableCollection<S7Plc> Plcs { get => _Plcs; set => SetProperty(ref _Plcs, value); }

        private S7Plc _SelectedPlc;
        public S7Plc SelectedPlc { get => _SelectedPlc; set => SetProperty(ref _SelectedPlc, value); }

        private ObservableCollection<PlcGroup> _Groups;
        public ObservableCollection<PlcGroup> Groups { get => _Groups; set => SetProperty(ref _Groups, value); }

        private S7Plc _SelectedGroup;
        public S7Plc SelectedGroup { get => _SelectedGroup; set => SetProperty(ref _SelectedGroup, value); }

        private bool _IsBusy;
        public bool IsBusy { get => _IsBusy; set => SetProperty(ref _IsBusy, value); }

        public ICommand Refresh { private set; get; }
        public ICommand CreatePlc { private set; get; }
        public ICommand CreateGroup { private set; get; }

        public MainViewModel()
        {
            
            
            
            

            CreatePlc = new Command(CreatePlcHandler);
            CreateGroup = new Command(CreateGroupHandler);
            Refresh = new Command(RefreshHandler);

            IsBusy = true;
        }

        private async void CreateGroupHandler()
        {
            string groupName = await Shell.Current.DisplayPromptAsync("Create PLC group",null, "Create", "Cancel", "Group name", 10, Keyboard.Text);
            
            if (!string.IsNullOrEmpty(groupName))
            {
                string cleanName = groupName.Trim();
                if (await DBV.GetGroupByName(cleanName) == null)
                {
                    await DBV.CreatePlcGroup(new PlcGroup() { Name = cleanName });
                }
                else
                {
                    await Shell.Current.DisplayAlert("Group already exists", "Try different name", "Cancel");
                }
            }
        }

        private async void CreatePlcHandler()
        {
            EditS7PlcPage edit = new EditS7PlcPage();
            await Shell.Current.Navigation.PushModalAsync(edit);
        }

        private async void RefreshHandler()
        {
            IsBusy = true;

            LoadGroups();

            Plcs = new ObservableCollection<S7Plc>();
            List<S7Plc> all = await DBV.GetAllPlcs();
            //all = all.OrderByDescending(x => x.LastUse).ToList();
            all.ForEach(x => Plcs.Add(x));

            IsBusy = false;
        }

        private async void LoadGroups()
        {
            Groups = new ObservableCollection<PlcGroup>();
            List<PlcGroup> allGroups = await DBV.GetAllPlcGroups();
            allGroups.ForEach(x => Groups.Add(x));
        }
    }
}
