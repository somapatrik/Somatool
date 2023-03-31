using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Veverka.ViewModels
{
    public class AddressPopupViewModel : PrimeViewModel
    {

        public string outAddress;

        #region Collections

        private ObservableCollection<string> _MemoryTypes = new ObservableCollection<string>() { "I", "Q", "M", "DB" };
        public ObservableCollection<string> MemoryTypes { get => _MemoryTypes; }
        
        private ObservableCollection<string> _MemorySizes = new ObservableCollection<string>() 
        { 
            "Bit", 
            "Byte", 
            "Word", 
            "Double" 
        };
        public ObservableCollection<string> MemorySizes { get => _MemorySizes; }

        #endregion

        #region Input field

        private string _Name;
        public string Name
        {
            get => _Name;
            set
            {
                SetProperty(ref _Name, value);
                RefreshCan();
            }
        }

        private string _SelectedMemoryType;
        public string SelectedMemoryType
        {
            get => _SelectedMemoryType;
            set
            {
                SetProperty(ref _SelectedMemoryType, value);
                IsDB = _SelectedMemoryType.ToUpper() == "DB";
                RefreshCan();
            }
        }

        private string _SelectedMemorySize;
        public string SelectedMemorySize 
        { 
            get => _SelectedMemorySize;
            set
            {
                SetProperty(ref _SelectedMemorySize, value);
                IsBit = _SelectedMemorySize.ToUpper() == "BIT";
                RefreshCan();
            }
        }

        private int _Offset;
        public int Offset 
        { 
            get => _Offset;
            set 
            {
                SetProperty(ref _Offset, value); 
                RefreshCan();
            }
        }

        private int _Bit;
        public int Bit
        {
            get => _Bit;
            set
            {
                SetProperty(ref _Bit, value);
                RefreshCan();
            }
        }

        private int _DB;
        public int DB
        {
            get => _DB;
            set
            {
                SetProperty(ref _DB, value);
                RefreshCan();
            }
        }

        #endregion

        private bool _IsBit;
        public bool IsBit { get => _IsBit; set => SetProperty(ref _IsBit, value); }

        private bool _IsDB;
        public bool IsDB { get => _IsDB; set => SetProperty(ref _IsDB, value); }

        #region Commands

        public ICommand SelectMemory { private set; get; }
        public ICommand SelectSize { private set; get; }
        public ICommand Confirm { private set; get; }

        #endregion

        public AddressPopupViewModel()
        {
            SelectMemory = new Command(SelectMemoryHandler);
            SelectSize = new Command(SelectSizeHandler);
            Confirm = new Command(ConfirmHandler, CanOk);
        }
        private void SelectSizeHandler(object sender)
        {
            SelectedMemorySize = (string)sender;
        }

        private void SelectMemoryHandler(object sender)
        {
            
            SelectedMemoryType = (string)sender;
        }

        private void RefreshCan()
        {
            ((Command)Confirm).ChangeCanExecute();
        }
        private bool CanOk()
        {
            bool selectedAll = !string.IsNullOrEmpty(SelectedMemoryType)
                && !string.IsNullOrEmpty(SelectedMemorySize)
                && Offset >= 0;

            bool bitCheck = IsBit ? Bit >= 0 && Bit <= 7 : true;
            bool dbCheck = IsDB ? DB >= 0 : true;

            return selectedAll && bitCheck && dbCheck;
                
        }

        private void ConfirmHandler()
        {
            CompleteOutputAddress();
        }

        private void CompleteOutputAddress()
        {
            string addressType = SelectedMemoryType.ToUpper();
            string addressSize = SelectedMemorySize.ToUpper();
            string addressSizeShort = "";

            string address = addressType;

            switch (addressSize)
            {
                case "BIT":
                    addressSizeShort += "X";
                    break;
                case "BYTE":
                    addressSizeShort += "B";
                    break;
                case "WORD":
                    addressSizeShort += "W";
                    break;
                case "DOUBLE":
                    addressSizeShort += "D";
                    break;
            }

            if (IsDB) 
            { 
                address += DB.ToString() + ".DB" + addressSizeShort + Offset.ToString();
                if (IsBit)
                    address += "." + Bit.ToString();
            }
            else
            {
                //address += addressSizeShort + Offset.ToString();
                if (IsBit)
                    address += Offset.ToString() + "." + Bit.ToString();
                else
                    address += addressSizeShort + Offset.ToString(); ;
            }

            outAddress = address;

        }
    }
}
