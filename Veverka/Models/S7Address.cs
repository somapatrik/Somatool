using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veverka.Models
{
    public class S7Address
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public int PLC_ID { get; set; }
        public string DisplayName { get; set; }
        public string RawAddress { get; set; }
        public string DataFormat { get; set; }
    }
}
