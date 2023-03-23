//using Android.Accounts;
//using Android.OS;
using CommunityToolkit.Maui.Alerts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
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
        public PlcGroup SelectedGroup 
        { 
            get => _SelectedGroup;
            set
            {
                SetProperty(ref _SelectedGroup, value);
            }
        }

        private bool _IsBusy;
        public bool IsBusy { get => _IsBusy; set => SetProperty(ref _IsBusy, value); }

        private string _MyIP;
        public string MyIP { get => _MyIP; set => SetProperty(ref _MyIP, value); }


        public ICommand Refresh { private set; get; }
        public ICommand CreatePlc { private set; get; }
        public ICommand CreateGroup { private set; get; }
        public ICommand SelectGroup { private set; get; }
        public ICommand DeleteGroup { private set; get; }
        public ICommand PlcOpen { private set; get; }
        public ICommand PlcDelete { private set; get; }
        public ICommand PlcEdit { private set; get; }

        public MainViewModel()
        {           
            CreatePlc = new Command(CreatePlcHandler);
            CreateGroup = new Command(CreateGroupHandler);
            DeleteGroup = new Command(DeleteGroupHandler);
            Refresh = new Command(RefreshHandler);
            SelectGroup = new Command(SelectGroupHandler);
            PlcOpen = new Command(PlcOpenHandler);
            PlcDelete = new Command(PlcDeleteHandler);
            PlcEdit = new Command(PlcEditHandler);
        }

        private async void PlcOpenHandler(object sender)
        {
            S7ProfilePage plcPage = new S7ProfilePage((S7Plc)sender);

            await Shell.Current.Navigation.PushAsync(plcPage);
        }

        private async void PlcDeleteHandler(object sender)
        {
            if (await Shell.Current.DisplayAlert("Delete PLC", "All data related to this PLC will be deleted.", "OK", "Cancel")) { 
                await DBV.DeletePlc((S7Plc)sender);
                await RefreshPlcs();
            }
        }

        private async void PlcEditHandler(object sender)
        {
            S7Plc plc = (S7Plc)sender;
            EditS7PlcPage editPage = new EditS7PlcPage();
            await Shell.Current.Navigation.PushAsync(editPage);
            
            editPage.SetEdit(plc);
        }

        private async void DeleteGroupHandler()
        {
            if (SelectedGroup !=null && 
                await Shell.Current.DisplayAlert("Delete " + SelectedGroup.Name, "Delete group and remove it from all respective PLCs?", "Delete", "Cancel"))
            {
                await DBV.RemoveGroupFromPlc(SelectedGroup);
                await DBV.DeleteGroup(SelectedGroup);
                
                SelectedGroup = null;
                await LoadGroups();
                await RefreshPlcs();
            }
        }

        private async void SelectGroupHandler(object sender)
        {
            PlcGroup clicked = (PlcGroup)sender;

            if (SelectedGroup == clicked)
                SelectedGroup = null;
            else
                SelectedGroup = clicked;

            await RefreshPlcs();
        }

        public void OnAppearing()
        {
            IsBusy = true;
           // await LoadGroups();
           // await RefreshPlcs();
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
            await Shell.Current.Navigation.PushAsync(edit);
        }

        private async void RefreshHandler()
        {
            IsBusy = true;

            await LoadGroups();
            await RefreshPlcs();

            IsBusy = false;
        }

        private async Task RefreshPlcs()
        {
            Plcs = new ObservableCollection<S7Plc>();
            List<S7Plc> all = new List<S7Plc>();

            if (SelectedGroup == null)
                all = await DBV.GetAllPlcs();
            else
                all = await DBV.GetAllPlcs(SelectedGroup.ID);

            all = all.OrderBy(x => x.Name).ToList();
            all.ForEach(x => Plcs.Add(x));
        }

        // TODO: Maybe load only if different?
        private async Task LoadGroups()
        {
            int preSelected = 0;
            if (SelectedGroup != null)
                preSelected = SelectedGroup.ID;

            Groups = new ObservableCollection<PlcGroup>();
            List<PlcGroup> allGroups = await DBV.GetAllPlcGroups();

            allGroups = allGroups.OrderByDescending(x => x.Name).ToList();
            allGroups.ForEach(x => Groups.Add(x));

            if (preSelected != 0)
                SelectedGroup = Groups.FirstOrDefault(g => g.ID == preSelected);
        }
    }
}
