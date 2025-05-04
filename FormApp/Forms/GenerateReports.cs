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
using ClassLibrary.Persistence;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using ClassLibrary.Models;

namespace FormApp.Forms
{
    public partial class GenerateReports : Form
    {
        private readonly DBContext _context;
        private string currentReport = "";

        public GenerateReports()
        {
            InitializeComponent();

            _context = new DBContext();

            lblName.Text = UserSession.FullName;

            RoleHelper.ApplyRolePermissions(
            UserSession.RoleID,
            lblRole,
            lblAuditLogs,
            lblPerformDBBackup,
            lblGenerateReports
            );

            gridReports.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void GenerateReports_Load(object sender, EventArgs e)
        {

        }

        private void btnActiveCustomers_Click(object sender, EventArgs e)
        {
            try
            {
                var customers = _context.Users
                    .Where(u => u.RoleId == 3)
                    .Select(u => new Dictionary<string, object>
                    {
                        { "Full Name", u.Fname + " " + u.Lname },
                        { "Email", u.Email },
                        { "Total Rentals", u.RentalTransactions.Count },
                        { "Total Spent", u.RentalTransactions.Sum(t => (decimal?)t.Fee) ?? 0 }
                    })
                    .ToList();

                gridReports.DataSource = new BindingSource { DataSource = ToDataTable(customers) };
                currentReport = "ActiveCustomers";

                // log report generation
                Log log = new Log
                {
                    UserId = UserSession.UserID,
                    Action = "Generate Report",
                    TimeStamp = DateTime.Now,
                    AffectedData = "Generated Active Customers Report",
                    Source = "GenerateReports Form"
                };
                _context.Logs.Add(log);
                _context.SaveChanges(); 
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load customer report:\n" + ex.Message);
            }
        }

        private void btnMostRentedEquipment_Click(object sender, EventArgs e)
        {
            try
            {
                var equipmentData = _context.RentalRequests
                    .Include(r => r.Equipment)
                    .ThenInclude(e => e.Category)
                    .AsEnumerable()
                    .Where(r => r.Equipment != null && r.Equipment.Category != null)
                    .GroupBy(r => new
                    {
                        r.Equipment.Name,
                        Category = r.Equipment.Category.Name
                    })
                    .Select(g => new Dictionary<string, object>
                    {
                { "Equipment Name", g.Key.Name },
                { "Category", g.Key.Category },
                { "Rental Count", g.Count() }
                    })
                    .OrderByDescending(e => (int)e["Rental Count"])
                    .ToList();

                gridReports.DataSource = new BindingSource { DataSource = ToDataTable(equipmentData) };
                currentReport = "MostRentedEquipment";

                Log log = new Log
                {
                    UserId = UserSession.UserID,
                    Action = "Generate Report",
                    TimeStamp = DateTime.Now,
                    AffectedData = "Generated Most Rented Equipment Report",
                    Source = "GenerateReports Form"
                };
                _context.Logs.Add(log);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load equipment report:\n" + ex.Message);
            }
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            if (gridReports.DataSource == null)
            {
                MessageBox.Show("No data to download.");
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "CSV files (*.csv)|*.csv";
                sfd.FileName = currentReport + "_Report.csv";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var dt = (DataTable)((BindingSource)gridReports.DataSource).DataSource;
                        ExportDataTableToCSV(dt, sfd.FileName);
                        MessageBox.Show("Report downloaded successfully!");

                        Log log = new Log
                        {
                            UserId = UserSession.UserID,
                            Action = "Download Report",
                            TimeStamp = DateTime.Now,
                            AffectedData = $"Downloaded Report: {currentReport}_Report.csv",
                            Source = "GenerateReports Form"
                        };
                        _context.Logs.Add(log);
                        _context.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error downloading file: " + ex.Message);
                    }
                }
            }
        }

        private void ExportDataTableToCSV(DataTable dt, string filePath)
        {
            var lines = new List<string>();

            string[] columnNames = dt.Columns.Cast<DataColumn>()
                                     .Select(column => column.ColumnName)
                                     .ToArray();

            var header = string.Join(",", columnNames);
            lines.Add(header);

            foreach (DataRow row in dt.Rows)
            {
                string[] fields = row.ItemArray.Select(field => field.ToString()).ToArray();
                lines.Add(string.Join(",", fields));
            }

            File.WriteAllLines(filePath, lines);
        }

        private DataTable ToDataTable(List<Dictionary<string, object>> data)
        {
            DataTable table = new DataTable();

            if (data == null || data.Count == 0)
                return table;

            foreach (var key in data[0].Keys)
            {
                table.Columns.Add(key);
            }

            foreach (var dict in data)
            {
                DataRow row = table.NewRow();
                foreach (var key in dict.Keys)
                {
                    row[key] = dict[key] ?? DBNull.Value;
                }
                table.Rows.Add(row);
            }

            return table;
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

        private void lblAuditLogs_Click(object sender, EventArgs e)
        {
            // display audit logs form
            FormHelper.NavigateTo<AuditLogs>(this);
        }

        private void lblPerformDBBackup_Click(object sender, EventArgs e)
        {
            // display database backup form
            FormHelper.NavigateTo<DatabaseBackup>(this);
        }

        private void lblLogOut_Click(object sender, EventArgs e)
        {
            // return to login page
            FormHelper.ConfirmAndLogout(this);
        }

        private void lblExit_Click(object sender, EventArgs e)
        {
            FormHelper.ExitApp();
        }
    }
}
