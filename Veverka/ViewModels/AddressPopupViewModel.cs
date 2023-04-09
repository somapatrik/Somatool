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
            "Double",
            "String"
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
                IsString = _SelectedMemorySize.ToUpper() == "STRING";
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

        private int _Length;
        public int Length
        {
            get => _Length;
            set
            {
                SetProperty(ref _Length, value);
                RefreshCan();
            }
        }

        #endregion

        private bool _IsBit;
        public bool IsBit { get => _IsBit; set => SetProperty(ref _IsBit, value); }

        private bool _IsDB;
        public bool IsDB { get => _IsDB; set => SetProperty(ref _IsDB, value); }

        private bool _IsString;
        public bool IsString { get => _IsString; set => SetProperty(ref _IsString, value); }

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

            DB = 1;
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
            bool dbCheck = IsDB ? DB >= 1 : true;
            bool stringCheck = IsString ? Length > 2 && Length <= 50 : true; 

            return selectedAll && bitCheck && dbCheck && stringCheck;
                
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
                case "STRING":
                    addressSizeShort += "S";
                    break;
            }

            if (IsDB) 
            { 
                // DB5.DBW5
                address += DB.ToString() + ".DB" + addressSizeShort + Offset.ToString();
                
                //DB5.DBX5.1
                if (IsBit)
                    address += "." + Bit.ToString();

                if (IsString)
                    address += "," + Length.ToString();
            }
            else
            {

                // IW5 | I5.1
                if (IsBit) 
                { 
                    address += Offset.ToString() + "." + Bit.ToString();
                }
                else
                {
                    address += addressSizeShort + Offset.ToString();

                    // IS5,3
                    if (IsString)
                        address += "," + Length.ToString();
                }
            }

            outAddress = address;

        }
    }
}
