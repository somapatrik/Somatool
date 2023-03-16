using Sharp7;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veverka.Classes;
using Veverka.ViewModels;

namespace Veverka.Models
{
    public class S7DataRow : PrimeViewModel
    {
        private S7Client PlcClient;
        private byte[] Buffer;
        private AddressFormatter addressFormatter;

        private S7Address _Address;
        public S7Address Address 
        { 
            get => _Address;
            set
            {
                SetProperty(ref _Address, value);
                addressFormatter = new AddressFormatter() { Address = _Address.RawAddress };
                Buffer = new byte[addressFormatter.BufferSize];
            }
        }

        private string _Data;
        public string Data
        {
            get => _Data;
            set
            {
                SetProperty(ref _Data, value);
            }
        }

        public S7DataRow(S7Address address, ref S7Client client)
        {
            PlcClient = client;
            Address = address;            
        }

        public void ReadFromPLC()
        {
            if (addressFormatter.IsValid)
                PlcClient.ReadArea(addressFormatter.Area,
                    addressFormatter.DBNumber,
                    addressFormatter.Start,
                    addressFormatter.Amount,
                    addressFormatter.WordLen,
                    Buffer);
        }

       


    }
}
