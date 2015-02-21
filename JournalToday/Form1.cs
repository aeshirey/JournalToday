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
        public formMain()
        {
            InitializeComponent();
            this.KeyPreview = true; // permits F11 check

            if (!JT.LoadDatabase())
            {
                MessageBox.Show("Unable to open or create journal database file '" + JT.JOURNAL_DB + "'");

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

            var journalEntry = JT.db.Table<JournalEntry>().FirstOrDefault(je => je.JournalDate == JT.formatDate(selectedDate));
            tbJournalText.Text = journalEntry == null ? string.Empty : journalEntry.JournalText;


            tbJournalText.Enabled = selectedDate == DateTime.Now.Date;
        }

        private void tbJournalText_Leave(object sender, EventArgs e)
        {
            var text = ((TextBox)sender).Text;

            var journalEntry = new JournalEntry()
            {
                JournalDate = JT.formatDate(DateTime.Now),
                JournalText = tbJournalText.Text
            };
            JT.db.InsertOrReplace(journalEntry);
        }

        private void formMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            JT.CloseDatabase();
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
