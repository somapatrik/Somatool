using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veverka.Models
{
    public class PlcGroup
    {
        [PrimaryKey,AutoIncrement]
        public int ID { get; set; }

        public string Name { get; set; }
    }
}
