
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veverka.Models;

namespace Veverka.Services
{
    public static class DBV
    {
        #region DB settings

        static SQLiteAsyncConnection Database;

        public static string DatabaseFilename = "veverka.db3";

        public static SQLite.SQLiteOpenFlags Flags =
            // open the database in read/write mode
            SQLite.SQLiteOpenFlags.ReadWrite |
            // create the database if it doesn't exist
            SQLite.SQLiteOpenFlags.Create |
            // enable multi-threaded database access
            SQLite.SQLiteOpenFlags.SharedCache;

        public static string DatabasePath => Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);

        public static async Task InitDB()
        {
            if (Database != null)
                return;

            Database = new SQLiteAsyncConnection(DatabasePath, Flags);

            await Database.CreateTableAsync<PlcGroup>();
            await Database.CreateTableAsync<S7Plc>();
            await Database.CreateTableAsync<S7Address>();
        }


        #endregion

        #region PLC

        public static async Task CreatePlc(S7Plc plc)
        {
            await Database.InsertAsync(plc);
        }

        public static async Task<List<S7Plc>> GetAllPlcs()
        {
            return await Database.Table<S7Plc>().ToListAsync();
        }

        public static async Task<List<S7Plc>> GetAllPlcs(int Group_ID)
        {
            await InitDB();
            return await Database.Table<S7Plc>().Where(p => p.Group_ID == Group_ID).ToListAsync();
        }

        public static async Task<S7Plc> GetPlcByIP(string IP)
        {
            return await Database.Table<S7Plc>().Where(x => x.IP == IP).FirstOrDefaultAsync();
        }

        public static async Task<S7Plc> GetPlcByName(string Name)
        {
            return await Database.Table<S7Plc>().Where(x => x.Name == Name).FirstOrDefaultAsync();
        }

        public static async Task<int> DeletePlc(S7Plc plc)
        {
           (await GetAddresses(plc.ID)).ForEach(async r => await DeleteAddress(r));
           return await Database.DeleteAsync(plc);
        }

        public static async Task<int> EditPlc(S7Plc plc)
        {
           return await Database.UpdateAsync(plc);
        }

        #endregion

        #region PLC Group
        public static async Task<PlcGroup> GetGroupByName(string Name)
        {
            await InitDB();
            return await Database.Table<PlcGroup>().Where(g => g.Name == Name).FirstOrDefaultAsync();
        }

        public static async Task<PlcGroup> GetGroupByID(int ID)
        {
            await InitDB();
            return await Database.Table<PlcGroup>().Where(g => g.ID == ID).FirstOrDefaultAsync();
        }

        public static async Task CreatePlcGroup(PlcGroup group)
        {
            await Database.InsertAsync(group);
        }

        public static async Task<List<PlcGroup>> GetAllPlcGroups()
        {
            return await Database.Table<PlcGroup>().ToListAsync();
        }

        public static async Task<int> DeleteGroup(PlcGroup group)
        {
            return await Database.DeleteAsync(group);
        }

        public static async Task RemoveGroupFromPlc(PlcGroup group)
        {
            (await GetAllPlcs(group.ID)).ForEach(async plc => { plc.Group_ID = 0; await EditPlc(plc); });
        }

        public static async Task MoveToAnotherGroup(int idFrom, int idTo)
        {
            (await Database.Table<S7Plc>().Where(plc => plc.Group_ID == idFrom).ToListAsync())
                .ForEach(async x => 
                {
                    x.Group_ID = idTo;
                    await EditPlc(x);
                });
        }

        public static async Task<int> CountInGroup(PlcGroup group)
        {
            return await Database.Table<S7Plc>().Where(plc => plc.Group_ID == group.ID).CountAsync();
        }

        #endregion

        #region S7 Address

        public static async Task CreateAddress(S7Address address)
        {
            await Database.InsertAsync(address);
        }

        public static async Task<List<S7Address>> GetAddresses(int PLC_ID)
        {
            await InitDB();

            return await Database.Table<S7Address>().Where(a => a.PLC_ID == PLC_ID).ToListAsync();
        }

        public static async Task<int> UpdateAddress(S7Address address)
        {
            return await Database.UpdateAsync(address);
        }

        public static async Task<int> DeleteAddress(S7Address address)
        {
            return await Database.DeleteAsync(address);
        }

        #endregion)

    }
}
