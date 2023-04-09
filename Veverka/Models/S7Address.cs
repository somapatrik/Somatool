
using Sharp7;
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
        [Indexed]
        public int PLC_ID { get; set; }
        /// <summary>
        /// Displayname for user
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// String representation as in Step7
        /// </summary>
        public string RawAddress { get; set; }
        /// <summary>
        /// Users choosen format to convert bytes to
        /// </summary>
        public string DataFormat { get; set; }
        /// <summary>
        /// I, Q, M, DB
        /// </summary>
        public string MemoryType { get; set; }
        /// <summary>
        /// BIT, BYTE, WORD, DOUBLE, STRING
        /// </summary>
        public string SizeType { get; set; }
        /// <summary>
        /// Number of bytes to read
        /// </summary>
        public int MemorySize { get; set; }
        /// <summary>
        /// Byte
        /// </summary>
        public int Offset { get; set; }
        /// <summary>
        /// Bit 0 - 6
        /// </summary>
        public int Bit { get; set; }
        /// <summary>
        /// Datablock address
        /// </summary>
        public int DBNumber { get; set; }

        [Ignore]
        public S7Area Area
        {
            get 
            { 
                if (IsInput)
                    return S7Area.PE;
                else if (IsOutput)
                    return S7Area.PA;
                else if (IsMerker)
                    return S7Area.MK;
                else
                    return S7Area.DB;
                
            }
        }

        [Ignore]
        public int Start
        {
            get
            {
                if (IsBit)
                    return (Offset * 8) + Bit;
                else
                    return Offset;
            }
        }

        [Ignore]
        public S7WordLength WordLength
        {
            get 
            { 
                if (IsBit)
                {
                    return S7WordLength.Bit;
                }
                else if (IsByte)
                {
                    return S7WordLength.Byte;
                }
                else if (IsWord)
                {
                    return S7WordLength.Word;
                }
                else if (IsDouble)
                {
                    return S7WordLength.DWord;
                }
                else
                {
                    return S7WordLength.Byte;
                }
            }
        }

        [Ignore]
        public int Amount
        {
            get
            {
                if (IsString)
                    return MemorySize;
                else
                    return 1;
            }
        }

        [Ignore]
        public bool IsInput => !string.IsNullOrEmpty(MemoryType) &&  MemoryType == "I"; 
        [Ignore]
        public bool IsOutput => !string.IsNullOrEmpty(MemoryType) && MemoryType == "Q"; 
        [Ignore]
        public bool IsMerker => !string.IsNullOrEmpty(MemoryType) && MemoryType == "M"; 
        [Ignore]
        public bool IsDB => !string.IsNullOrEmpty(MemoryType) && MemoryType == "DB";


        [Ignore]
        public bool IsBit => !string.IsNullOrEmpty(SizeType) && SizeType == "BIT";
        [Ignore]
        public bool IsByte => !string.IsNullOrEmpty(SizeType) && SizeType == "BYTE";
        [Ignore]
        public bool IsWord => !string.IsNullOrEmpty(SizeType) && SizeType == "WORD";
        [Ignore]
        public bool IsDouble => !string.IsNullOrEmpty(SizeType) && SizeType == "DOUBLE";
        [Ignore]
        public bool IsString => !string.IsNullOrEmpty(SizeType) && SizeType == "STRING";

    }
}
