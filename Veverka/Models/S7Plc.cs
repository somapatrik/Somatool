using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veverka.Models
{
    public class S7Plc
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Name { get; set; }
        public string IP { get; set; }
        public int Rack { get; set; }
        public int Slot { get; set; }
        public string Description { get; set; }
        public int Group_ID { get; set; }


    }
}
