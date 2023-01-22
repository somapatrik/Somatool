using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veverka.Services
{
    public static class DBV
    {
        #region DB settings

        public static string DatabaseFilename = "TodoSQLite.db3";

        public static SQLite.SQLiteOpenFlags Flags =
            // open the database in read/write mode
            SQLite.SQLiteOpenFlags.ReadWrite |
            // create the database if it doesn't exist
            SQLite.SQLiteOpenFlags.Create |
            // enable multi-threaded database access
            SQLite.SQLiteOpenFlags.SharedCache;

        public static string DatabasePath =>  Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);

        #endregion



    }
}
