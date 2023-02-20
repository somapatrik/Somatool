//using Android.Accounts;
//using Android.OS;
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

        private PlcGroup _SelectedGroup;
        public PlcGroup SelectedGroup { get => _SelectedGroup; set => SetProperty(ref _SelectedGroup, value); }

        //private List<PlcGroup> _SelectedGroups;
        //public List<PlcGroup> SelectedGroups 
        //{ 
        //    get => _SelectedGroups; 
        //    set => SetProperty(ref _SelectedGroups, value); 
        //}

        private bool _IsBusy;
        public bool IsBusy { get => _IsBusy; set => SetProperty(ref _IsBusy, value); }

        public ICommand Refresh { private set; get; }
        public ICommand CreatePlc { private set; get; }
        public ICommand CreateGroup { private set; get; }
       // public ICommand SelectGroup { private set; get; }

        public ICommand SelectItem { private set; get; }

        public MainViewModel()
        {
            //SelectedGroups = new List<PlcGroup>();
           
            CreatePlc = new Command(CreatePlcHandler);
            CreateGroup = new Command(CreateGroupHandler);
            Refresh = new Command(RefreshHandler);

           // SelectGroup = new Command(SelectGroupHandler);

            SelectItem = new Command(SelectItemHandler);

            // IsBusy = true;
        }

        private void SelectItemHandler(object sender)
        {
            PlcGroup clicked = (PlcGroup)sender;

            if (SelectedGroup == clicked)
                SelectedGroup = null;
            else
                SelectedGroup = clicked;
        }

        public async void OnAppearing()
        {
            await LoadPlcs();
            await LoadGroups();
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
                    await LoadGroups();
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

            await LoadGroups();
            await LoadPlcs();

            IsBusy = false;
        }

        private async Task LoadPlcs()
        {
            Plcs = new ObservableCollection<S7Plc>();
            List<S7Plc> all = await DBV.GetAllPlcs();
            //all = all.OrderByDescending(x => x.LastUse).ToList();
            all.ForEach(x => Plcs.Add(x));
        }

        private async Task LoadGroups()
        {
            Groups = new ObservableCollection<PlcGroup>();
            List<PlcGroup> allGroups = await DBV.GetAllPlcGroups();
            allGroups = allGroups.OrderByDescending(x => x.Name).ToList();
            allGroups.ForEach(x => Groups.Add(x));
        }
    }
}
