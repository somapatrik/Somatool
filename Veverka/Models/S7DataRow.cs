﻿using Sharp7;
using Veverka.Classes;
using Veverka.Services;
using Veverka.ViewModels;

namespace Veverka.Models
{
    public class S7DataRow : PrimeViewModel
    {
        private S7Client PlcClient;
        private byte[] Buffer;
        //private AddressFormatter addressFormatter;

        private S7Address _Address;
        public S7Address Address 
        { 
            get => _Address;
            set
            {
                SetProperty(ref _Address, value);
                //addressFormatter = new AddressFormatter() { Address = _Address.RawAddress };
                Buffer = new byte[_Address.MemorySize]; //new byte[addressFormatter.BufferSize];
                SetFormats();
                LoadSelectedFormat();
            }
        }

        private string _Data;
        public string Data { get => _Data; set => SetProperty(ref _Data, value); }

        private List<string> _Formats;
        public List<string> Formats { get => _Formats; set => SetProperty(ref _Formats, value); } 

        private string _SelectedFormat;
        public string SelectedFormat 
        { 
            get => _SelectedFormat;
            set
            {
                SetProperty(ref _SelectedFormat, value);
                UpdateAddress();
            }
        }

        public S7DataRow(S7Address address, ref S7Client client)
        {
            PlcClient = client;
            Address = address;            
        }

        private void LoadSelectedFormat()
        {
            if (Formats.Count > 0)
                SelectedFormat = Formats.Contains(Address.DataFormat) ? Address.DataFormat : Formats[0];
        }

        private async void UpdateAddress()
        {
            _Address.DataFormat = SelectedFormat;
            await DBV.UpdateAddress(Address);
        }

        private void SetFormats()
        {
            List<string> formats = new List<string>();
            
           // if (addressFormatter.IsValid)
           // {
                if (_Address.IsBit)
                {
                    formats.Add("BOOL");
                }
                else if (_Address.IsByte || _Address.IsWord)
                {
                    formats.Add("BINARY");
                    formats.Add("DECIMAL +/-");
                    formats.Add("DECIMAL");
                    //formats.Add("CHARACTER");
                }
                else if (_Address.IsDouble)
                {
                    formats.Add("BINARY");
                    formats.Add("DECIMAL +/-");
                    formats.Add("DECIMAL");
                    //formats.Add("CHARACTER");
                    formats.Add("FLOAT");
                }
                else if (_Address.IsString)
                {
                    formats.Add("STRING");
                }
           // }

            Formats = formats;

        }

        public void ReadFromPLC()
        {
            try 
            {  
                PlcClient.ReadArea(
                    _Address.Area,
                    _Address.DBNumber,
                    _Address.Start,
                    _Address.Amount,
                    _Address.WordLength,
                    Buffer);

                PresentData();

            }
            catch
            {
                Data = "<Error>";
            }
                            
        }

        public void PresentData()
        {
            switch (SelectedFormat)
            {
                case "BOOL":
                    GetBoolS();
                    break;
                case "BINARY":
                    GetBinS();
                    break;
                case "DECIMAL":
                    GetUDecS();
                    break;
                case "DECIMAL +/-":
                    GetSDecS();
                    break;
                case "FLOAT":
                    GetFloatS();
                    break;
                case "STRING":
                    GetString();
                    break;
            }
                
        }

        public void GetBoolS()
        {
            Data = S7.GetBitAt(Buffer, 0, 0).ToString();
        }

        public void GetBinS()
        {
            Data = string.Join(" ", Buffer.Select(x => Convert.ToString(x, 2).PadLeft(8, '0')));
        }

        public void GetSDecS()
        {
            if (_Address.IsByte) 
            { 
                Data = S7.GetSIntAt(Buffer, 0).ToString();
            }
            else if (_Address.IsWord)
            {
                Data = S7.GetIntAt(Buffer, 0).ToString();
            }
            else if (_Address.IsDouble)
            {
                Data = S7.GetDIntAt(Buffer, 0).ToString();
            }
        }

        public void GetUDecS()
        {
            if (_Address.IsByte)
            {
                Data = S7.GetUSIntAt(Buffer, 0).ToString();
            }
            else if (_Address.IsWord)
            {
                Data = S7.GetUIntAt(Buffer, 0).ToString();
            }
            else if (_Address.IsDouble)
            {
                Data = S7.GetUDIntAt(Buffer, 0).ToString();
            }

        }

        public void GetFloatS()
        {
            if (_Address.IsDouble)
                Data = S7.GetRealAt(Buffer, 0).ToString();
        }

        public void GetString()
        {
            Data = S7.GetStringAt(Buffer, 0);
        }

    }
}
