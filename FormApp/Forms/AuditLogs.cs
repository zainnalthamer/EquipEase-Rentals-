using FormApp.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;
using ClassLibrary.Persistence;
using System.Reflection.Metadata.Ecma335;

namespace FormApp.Forms
{
    public partial class AuditLogs : Form
    {
        private readonly DBContext _context;

        public AuditLogs()
        {
            InitializeComponent();

            // instantiate DBContext
            _context = new DBContext();

            // apply placeholder to the search bar
            PlaceholderService.SetPlaceholder(txtSearchBar, "Log ID");

            // displaying user info
            lblName.Text = UserSession.FullName;

            RoleHelper.ApplyRolePermissions(
            UserSession.RoleID,
            lblRole,
            lblViewAuditLogs,
            lblPerformDBBackup,
            lblGenerateReports
            );

            // centering the form
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void AuditLogs_Load(object sender, EventArgs e)
        {
            LoadLogs(); // load all logs on form load
        }

        // load the grid view and filter logs by id
        private void LoadLogs(string searchId = "")
        {
            var logsQuery = _context.Logs.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchId) && int.TryParse(searchId, out int id))
            {
                logsQuery = logsQuery.Where(log => log.Id == id);
            }

            var logList = logsQuery
                .Select(log => new
                {
                    log.Id,
                    log.UserId,
                    log.Action,
                    log.TimeStamp,
                    log.AffectedData,
                    log.Source
                })
                .ToList();

            gridLogs.DataSource = logList;
        }

        // search button click event
        private void btnSearch_Click(object sender, EventArgs e)
        {
            LoadLogs(txtSearchBar.Text.Trim());
        }

        private void lblDashboard_Click(object sender, EventArgs e)
        {
            // display dashboard
            FormHelper.NavigateTo<Dashboard>(this);
        }

        private void lblRentalRequests_Click(object sender, EventArgs e)
        {
            // display rental requests form
            FormHelper.NavigateTo<RentalRequests>(this);
        }

        private void lblRentalTransactions_Click(object sender, EventArgs e)
        {
            // display rental transactions form
            FormHelper.NavigateTo<RentalTransactions>(this);
        }

        private void lblReturnRecords_Click(object sender, EventArgs e)
        {
            // display return records form
            FormHelper.NavigateTo<ReturnRecords>(this);
        }

        private void lblEquipmentManagement_Click(object sender, EventArgs e)
        {
            // display equipment management form
            FormHelper.NavigateTo<EquipmentManagement>(this);
        }

        private void lblPerformDBBackup_Click(object sender, EventArgs e)
        {
            // display database backup form
            FormHelper.NavigateTo<DatabaseBackup>(this);
        }

        private void lblGenerateReports_Click(object sender, EventArgs e)
        {
            // display generate reports form
            FormHelper.NavigateTo<GenerateReports>(this);
        }

        private void lblLogOut_Click(object sender, EventArgs e)
        {
            FormHelper.ConfirmAndLogout(this);
        }

        private void lblExit_Click(object sender, EventArgs e)
        {
            FormHelper.ExitApp();
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            // open the filter form as a dialog
            LogsFilter filter = new LogsFilter();
            if (filter.ShowDialog() == DialogResult.OK)
            {
                // retrieve the values entered by the user
                string logId = filter.LogId;
                string userId = filter.UserId;
                string action = filter.ActionSelected;
                string date = filter.Date;

                // apply the filters to the grid
                ApplyFilters(logId, userId, action, date);
            }
        }

        private void ApplyFilters(string logId, string userId, string action, string date)
        {
            using (var context = new DBContext())
            {
                // start with the full logs table as a queryable object
                var query = context.Logs.AsQueryable();

                // filter by log id if provided
                if (!string.IsNullOrEmpty(logId))
                {
                    if (int.TryParse(logId, out int logIdInt))
                    {
                        query = query.Where(l => l.Id == logIdInt);
                    }
                }

                // filter by user id if provided
                if (!string.IsNullOrEmpty(userId))
                {
                    if (int.TryParse(userId, out int userIdInt))
                    {
                        query = query.Where(l => l.UserId == userIdInt);
                    }
                }

                // filter by action if its selected
                if (!string.IsNullOrEmpty(action) && action != "Select Action")
                {
                    query = query.Where(l => l.Action == action);
                }

                // filter by date
                if (!string.IsNullOrEmpty(date))
                {
                    if (DateTime.TryParse(date, out DateTime selectedDate))
                    {
                        query = query.Where(l => l.TimeStamp.Date == selectedDate.Date);
                    }
                }

                // project and order by most recent logs first
                var result = query
                    .Select(l => new
                    {
                        l.Id,
                        l.UserId,
                        l.Action,
                        l.TimeStamp,
                        l.AffectedData,
                        l.Source
                    })
                    .OrderByDescending(l => l.TimeStamp)
                    .ToList();

                // bind the filtered results to the grid view
                gridLogs.DataSource = result;
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            // clear the search bar
            txtSearchBar.Text = "";

            // reapply the placeholder
            PlaceholderService.SetPlaceholder(txtSearchBar, "Log ID");

            // reload all logs from the database
            LoadLogs();
        }
    }
}
