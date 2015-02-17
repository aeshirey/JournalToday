using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SQLite;

namespace JournalToday
{
    public partial class formMain : Form
    {
        private const string JOURNAL_DB = "journalToday.sqlite";
        private SQLiteConnection dbConnection;
        
        private Func<DateTime, UInt32> formatDate = date => Convert.ToUInt32(date.Year * 10000 + date.Month * 100 + date.Day);

        public formMain()
        {
            InitializeComponent();
            this.KeyPreview = true;

            try
            {
                dbConnection = new SQLiteConnection(JOURNAL_DB);

                if (!System.IO.File.Exists(JOURNAL_DB))
                    dbConnection.CreateTable<JournalEntry>();
            }
            catch
            {
                MessageBox.Show("Unable to open or create journal database file '" + JOURNAL_DB + "'");

                dateTimePicker1.Enabled = false;
                tbJournalText.Enabled = false;
            }
        }

        /// <summary>
        /// Load the entry (if one exists) for the selected date
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            var selectedDate = ((DateTimePicker)sender).Value.Date;

            var journalEntry = dbConnection.Table<JournalEntry>().FirstOrDefault(je => je.JournalDate == formatDate(selectedDate));
            tbJournalText.Text = journalEntry == null ? string.Empty : journalEntry.JournalText;


            tbJournalText.Enabled = selectedDate == DateTime.Now.Date;
        }

        private void tbJournalText_Leave(object sender, EventArgs e)
        {
            var text = ((TextBox)sender).Text;

            var journalEntry = new JournalEntry()
            {
                JournalDate = formatDate(DateTime.Now),
                JournalText = tbJournalText.Text
            };
            dbConnection.InsertOrReplace(journalEntry);
        }

        private void formMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            dbConnection.Commit();
            dbConnection.Close();
        }

        private void tbJournalText_TextChanged(object sender, EventArgs e)
        {
            int charCount = ((TextBox)sender).Text.Count(c => !char.IsWhiteSpace(c));
            statusLabelCharacterCount.Text = charCount.ToString() + (charCount == 1 ? " character" : " characters");
        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/aeshirey/JournalToday");
        }

        private void formMain_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.F11)
            {
                if (this.FormBorderStyle == System.Windows.Forms.FormBorderStyle.None)
                {
                    // un-fullscreen
                    this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
                    this.TopMost = false;
                    this.WindowState = FormWindowState.Normal;
                }
                else
                {
                    // make fullscreen
                    this.WindowState = FormWindowState.Maximized;
                    this.TopMost = true;
                    this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                }
            }
        }
    }
}
