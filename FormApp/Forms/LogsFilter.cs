using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClassLibrary.Persistence;

namespace FormApp.Forms
{
    public partial class LogsFilter : Form
    {
        private readonly DBContext _context;
        public string LogId { get; private set; }
        public string UserId { get; private set; }
        public string ActionSelected { get; private set; }
        public string Date { get; private set; }


        public LogsFilter()
        {
            _context = new DBContext();
            InitializeComponent();
            LoadActions();
        }

        private void LogsFilter_Load(object sender, EventArgs e)
        {
        }

        private void LoadActions()
        {
            try
            {
                using (var context = new DBContext())
                {
                    var actions = context.Logs
                        .Select(l => l.Action)
                        .Distinct()
                        .OrderBy(a => a)
                        .ToList();

                    actions.Insert(0, "Select Action"); 

                    cmbAction.DataSource = actions;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load actions: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnApplyFilters_Click(object sender, EventArgs e)
        {
            LogId = txtLogId.Text.Trim();
            UserId = txtUserId.Text.Trim();
            ActionSelected = cmbAction.SelectedItem?.ToString();
            Date = dtpDate.Value.Date.ToString("yyyy-MM-dd");

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
