using Android.Accounts;
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

        private bool _IsBusy;
        public bool IsBusy { get => _IsBusy; set => SetProperty(ref _IsBusy, value); }

        public ICommand Refresh { private set; get; }
        public ICommand CreatePlc { private set; get; }

        public MainViewModel()
        {
            Plcs = new ObservableCollection<S7Plc>();

            CreatePlc = new Command(CreatePlcHandler);
            Refresh = new Command(RefreshHandler);

            IsBusy = true;
        }

        private async void CreatePlcHandler()
        {
            EditS7PlcPage edit = new EditS7PlcPage();
            await Shell.Current.Navigation.PushModalAsync(edit);
        }

        private async void RefreshHandler()
        {
            IsBusy = true;

            Plcs.Clear();
            List<S7Plc> all = await DBV.GetAllPlcs();
            //all = all.OrderByDescending(x => x.LastUse).ToList();
            all.ForEach(x => Plcs.Add(x));

            IsBusy = false;
        }

    }
}
