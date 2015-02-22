using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace JournalToday
{
    public class JournalEntry
    {
        [PrimaryKey]
        public DateTime JournalDate { get; set; }

        public string JournalText { get; set; }
    }
}
