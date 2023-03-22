using Sharp7;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Veverka.Classes
{
    class AddressFormatter
    {

        #region Private properties

        private string RawAddress;
        private bool _Valid;

        private int _Byte;
        private int _Bit;
        private int _DBNumber;

        private int _StringLen;

        private bool ibit;
        private bool ibyte;
        private bool iword;
        private bool idouble;

        private bool qbit;
        private bool qbyte;
        private bool qword;
        private bool qdouble;

        private bool mbit;
        private bool mbyte;
        private bool mword;
        private bool mdouble;

        private bool dbbit;
        private bool dbbyte;
        private bool dbword;
        private bool dbdouble;

        private bool dbs7string;

        private bool _IsInput;
        private bool _IsOutput;
        private bool _IsMerker;
        private bool _IsDB;

        #endregion

        #region Public properties

        public bool IsValid => _Valid;
        public bool IsBit => ibit || qbit || mbit || dbbit;
        public bool IsByte => ibyte || qbyte || mbyte || dbbyte;
        public bool IsWord => iword || qword || mword || dbword; 
        public bool IsDouble => idouble || qdouble || mdouble || dbdouble;
        public bool IsString => dbs7string; 
        public bool IsInput => _IsInput;
        public bool IsOutput => _IsOutput; 
        public bool IsMerker => _IsMerker;
        public bool IsDB => _IsDB;
        public int Byte => _Byte;
        public int Bit => _Bit;
        public int StringLen => _StringLen;
        public int DBNumber => _DBNumber; 
        

        public S7Area Area;
        public int Start;
        public int Amount;
        public S7WordLength WordLen;
        public int BufferSize;

        #region Regex

        Regex InputBit = new Regex(@"[I]\d+[.][0-7]$", RegexOptions.IgnoreCase);
        Regex InputByte = new Regex(@"[I][B]\d+$", RegexOptions.IgnoreCase);
        Regex InputWord = new Regex(@"[I][W]\d+$", RegexOptions.IgnoreCase);
        Regex InputDouble = new Regex(@"[I][D]\d+$", RegexOptions.IgnoreCase);

        Regex OutputBit = new Regex(@"[Q]\d+[.][0-7]$", RegexOptions.IgnoreCase);
        Regex OutputByte = new Regex(@"[Q][B]\d+$", RegexOptions.IgnoreCase);
        Regex OutputWord = new Regex(@"[Q][W]\d+$", RegexOptions.IgnoreCase);
        Regex OutputDouble = new Regex(@"[Q][D]\d+$", RegexOptions.IgnoreCase);

        Regex MerkerBit = new Regex(@"[M]\d+[.][0-7]$", RegexOptions.IgnoreCase);
        Regex MerkerByte = new Regex(@"[M][B]\d+$", RegexOptions.IgnoreCase);
        Regex MerkerWord = new Regex(@"[M][W]\d+$", RegexOptions.IgnoreCase);
        Regex MerkerDouble = new Regex(@"[M][D]\d+$", RegexOptions.IgnoreCase);

        Regex DBBit = new Regex(@"\DB\d+.DBX\d+[.][0-7]$", RegexOptions.IgnoreCase);
        Regex DBByte = new Regex(@"\DB\d+.DB[B]\d+$", RegexOptions.IgnoreCase);
        Regex DBWord = new Regex(@"\DB\d+.DB[W]\d+$", RegexOptions.IgnoreCase);
        Regex DBDouble = new Regex(@"\DB\d+.DB[D]\d+$", RegexOptions.IgnoreCase);

        Regex DBS7String = new Regex(@"\DB\d+.DB[S]\d+[:]\d+$", RegexOptions.IgnoreCase);

        #endregion

        public string Address
        {
            set
            {
                RawAddress = value.ToUpper().Trim().Replace(" ", "");
                CheckAddress();

                if (IsValid)
                {
                    GetByte();

                    if (IsBit)
                        GetBit();
                    else
                        _Bit = 0;

                    if (_IsDB)
                        GetDBNumber();
                    else
                        _DBNumber = 0;

                    if (IsString)
                        GetStringLen();
                    

                    FormaterToInfo();
                }
                else
                {
                    _Byte = 0;
                    _Bit = 0;
                    _DBNumber = 0;
                }
            }
            get
            {
                return RawAddress;
            }
        }

        #endregion


        private void GetDBNumber()
        {
            int.TryParse(RawAddress.Split('.')[0].Substring(2), out _DBNumber);
        }

        private void GetByte()
        {
            int Byte;

            if (IsString)
            {
                string split1 = RawAddress.Split('.')[1];
                string split2 = split1.Split(':')[0];
                int.TryParse(split2.Substring(3), out Byte);
            }
            else
            {
                if (_IsInput || _IsOutput || _IsMerker)
                    int.TryParse(RawAddress.Substring(2), out Byte);
                else
                    int.TryParse(RawAddress.Split('.')[1].Substring(3), out Byte);
            }

            _Byte = Byte;
        }

        private void GetStringLen()
        {
            int len = 0;

            if (IsDB)
                int.TryParse(RawAddress.Split(':')[1].Substring(0), out len);

            _StringLen = len;
        }

        private void GetBit()
        {
            int Bit;

            if (_IsInput || _IsOutput || _IsMerker)
                int.TryParse(RawAddress.Split('.')[1], out Bit);
            else
                int.TryParse(RawAddress.Split('.')[2], out Bit);

            _Bit = Bit;
        }

        // TODO: Only one is needed to be true
        private void CheckAddress()
        {
            ibit = InputBit.IsMatch(RawAddress);
            ibyte = InputByte.IsMatch(RawAddress);
            iword = InputWord.IsMatch(RawAddress);
            idouble = InputDouble.IsMatch(RawAddress);

            qbit = OutputBit.IsMatch(RawAddress);
            qbyte = OutputByte.IsMatch(RawAddress);
            qword = OutputWord.IsMatch(RawAddress);
            qdouble = OutputDouble.IsMatch(RawAddress);

            mbit = MerkerBit.IsMatch(RawAddress);
            mbyte = MerkerByte.IsMatch(RawAddress);
            mword = MerkerWord.IsMatch(RawAddress);
            mdouble = MerkerDouble.IsMatch(RawAddress);

            dbbit = DBBit.IsMatch(RawAddress);
            dbbyte = DBByte.IsMatch(RawAddress);
            dbword = DBWord.IsMatch(RawAddress);
            dbdouble = DBDouble.IsMatch(RawAddress);
            dbs7string = DBS7String.IsMatch(RawAddress);

            _IsInput = ibit || ibyte || iword || idouble;
            _IsOutput = qbit || qbyte || qword || qdouble;
            _IsMerker = mbit || mbyte || mword || mdouble;
            _IsDB = dbbit || dbbyte || dbword || dbdouble || dbs7string;

            _Valid = _IsInput ^ _IsOutput ^ _IsMerker ^ _IsDB;

        }

        private void FormaterToInfo()
        {
            if (IsInput)
                Area = S7Area.PE;
            else if (IsOutput)
                Area = S7Area.PA;
            else if (IsMerker)
                Area = S7Area.MK;
            else if (IsDB)
                Area = S7Area.DB;

            if (IsBit)
            {
                WordLen = S7WordLength.Bit;
                BufferSize = 1;
            }
            else if (IsByte)
            {
                WordLen = S7WordLength.Byte;
                BufferSize = 1;
            }
            else if (IsWord)
            {
                WordLen = S7WordLength.Word;
                BufferSize = 2;
            }
            else if (IsDouble)
            {
                WordLen = S7WordLength.DWord;
                BufferSize = 4;
            }
            else if (IsString)
            {
                WordLen = S7WordLength.Byte;
                BufferSize = StringLen;
            }

            if (IsString)
                Amount = StringLen;
            else
                Amount = 1;

            if (IsBit)
                Start = (Byte * 8) + Bit;
            else
                Start = Byte;
        }

    }
}
