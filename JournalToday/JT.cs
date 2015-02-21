using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace JournalToday
{
    static class JT
    {
        public static SQLiteConnection db;
        public const string JOURNAL_DB = "journalToday.sqlite";

        public static bool LoadDatabase()
        {
            try
            {
                db = new SQLiteConnection(JOURNAL_DB);

                if (!System.IO.File.Exists(JOURNAL_DB))
                    db.CreateTable<JournalEntry>();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static void CloseDatabase()
        {
            db.Commit();
            db.Close();
        }

        public static int LongestStreak()
        {
            throw new NotImplementedException("LongestStreak");
        }

        public static Func<DateTime, UInt32> formatDate = date => Convert.ToUInt32(date.Year * 10000 + date.Month * 100 + date.Day);

    }
}
