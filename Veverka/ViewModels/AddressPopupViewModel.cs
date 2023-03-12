using Android.App;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Veverka.ViewModels
{
    public class AddressPopupViewModel : PrimeViewModel
    {

        private ObservableCollection<string> _MemoryTypes = new ObservableCollection<string>() { "I", "O", "M", "DB" };
        public ObservableCollection<string> MemoryTypes { get => _MemoryTypes; }
        
        private string _SelectedMemoryType;
        public string SelectedMemoryType 
        { 
            get => _SelectedMemoryType; 
            set => SetProperty(ref _SelectedMemoryType, value); 
        }

        private ObservableCollection<string> _MemorySizes = new ObservableCollection<string>() 
        { 
            "Bit", 
            "Byte", 
            "Word", 
            "Real" 
        };
        public ObservableCollection<string> MemorySizes { get => _MemorySizes; }

        private string _SelectedMemorySize;
        public string SelectedMemorySize 
        { 
            get => _SelectedMemorySize; 
            set => SetProperty(ref _SelectedMemorySize, value); 
        }

        public ICommand SelectMemory { private set; get; }

        public AddressPopupViewModel()
        {
            SelectMemory = new Command(SelectMemoryHandler);
        }

        private void SelectMemoryHandler(object sender)
        {
            
            SelectedMemoryType = (string)sender;
        }
    }
}
