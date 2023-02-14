using System;
using System.Collections.Generic;
using System.Linq;
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

        private string _IP;
        public string IP { get => _IP; set => SetProperty(ref _IP, value); }

        private string _Description;
        public string Description { get => _Description; set => SetProperty(ref _Description, value); }


        public ICommand SavePlc { private set; get; }

        public EditS7PlcViewModel()
        {
            BindCommands();
        }

        private void BindCommands()
        {
            SavePlc = new Command(SavePlcHandler);
        }

        private async void SavePlcHandler()
        {
            S7Plc savePlc = new S7Plc()
            {
                Name = Name,
                IP = IP,
                Description = Description
            };

            await DBV.CreatePlc(savePlc);
            await Shell.Current.GoToAsync("..");
        }
    }
}
