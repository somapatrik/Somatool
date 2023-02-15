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

        public static string DatabaseFilename = "TodoSQLite.db3";

        public static SQLite.SQLiteOpenFlags Flags =
            // open the database in read/write mode
            SQLite.SQLiteOpenFlags.ReadWrite |
            // create the database if it doesn't exist
            SQLite.SQLiteOpenFlags.Create |
            // enable multi-threaded database access
            SQLite.SQLiteOpenFlags.SharedCache;

        public static string DatabasePath =>  Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);

        public static async Task InitDB()
        {
            if (Database != null)
                return;

            Database = new SQLiteAsyncConnection(DatabasePath, Flags);

            await Database.CreateTableAsync<S7Plc>();
            await Database.CreateTableAsync<PlcGroup>();
        }


        #endregion

        public static async Task CreatePlc(S7Plc plc)
        {
            await Database.InsertAsync(plc);
        }

        public static async Task<List<S7Plc>> GetAllPlcs()
        {
            return await Database.Table<S7Plc>().ToListAsync();
        }


        public static async Task<PlcGroup> GetGroupByName(string Name)
        {
            return await Database.Table<PlcGroup>().Where(g => g.Name == Name).FirstOrDefaultAsync();
        }

        public static async Task<PlcGroup> GetGroupByID(int ID)
        {
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

    }
}
