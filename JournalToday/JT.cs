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

        public static void LoadDatabase()
        {
            try
            {
                db = new SQLiteConnection(JOURNAL_DB, storeDateTimeAsTicks: true);
                db.CreateTable<JournalEntry>();
            }
            catch (Exception e)
            {
                db = null;
                throw e;
            }
        }

        public static void CloseDatabase()
        {
            if (db != null)
            {
                db.Commit();
                db.Close();
            }
        }


        private static IEnumerable<IList<JournalEntry>> GetAllStreaks()
        {
            var entries = db.Table<JournalEntry>().OrderByDescending(je => je.JournalDate);
            var streak = new List<JournalEntry>();
            
            JournalEntry previous = null;
            foreach (var entry in entries)
            {
                if (previous == null || previous.JournalDate - entry.JournalDate == TimeSpan.FromDays(1))
                {
                    streak.Add(entry);
                }
                else
                {
                    yield return streak;
                    streak = new List<JournalEntry>();
                }

                previous = entry;
            }

            if (streak.Count > 0)
                yield return streak;
        }

        public static int LongestStreak()
        {
            return GetAllStreaks().Max(streak => streak.Count);
        }

        public static int CurrentStreak()
        {            
            IList<JournalEntry> mostRecentStreak = GetAllStreaks().FirstOrDefault();

            var today = DateTime.Now.Date;
            var yesterday = today.AddDays(-1);

            if (mostRecentStreak != null 
                && mostRecentStreak.Any(entry => entry.JournalDate == today || entry.JournalDate == yesterday))
            {
                return mostRecentStreak.Count;
            }

            return 0;
        }

    }
}
