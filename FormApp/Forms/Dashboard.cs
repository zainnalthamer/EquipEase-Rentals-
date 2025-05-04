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
using FormApp.Classes;
using FormApp.Forms;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ClassLibrary.Models;


namespace FormApp
{
    public partial class Dashboard : Form

    {
        private DBContext context;

        public Dashboard()
        {
            InitializeComponent();
            context = new DBContext();



            lblName.Text = UserSession.FullName;

            RoleHelper.ApplyRolePermissions(
            UserSession.RoleID,
            lblPosition,
            lblViewAuditLogs,
            lblDBbackup,
            lblGenerateReport
            );


        }



        private void button10_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            RentalRequests r = new RentalRequests();
            r.Show();

        }

        private void btnRentalTransactions_Click(object sender, EventArgs e)
        {
            this.Hide();
            RentalTransactions t = new RentalTransactions();
            t.Show();
        }

        private void btnRentalRecords_Click(object sender, EventArgs e)
        {
            this.Hide();
            ReturnRecords ret = new ReturnRecords();
            ret.Show();
        }

        private void btnEquipmentManagement_Click(object sender, EventArgs e)
        {
            this.Hide();
            EquipmentManagement eq = new EquipmentManagement();
            eq.Show();
        }

        private void btnAuditLogs_Click(object sender, EventArgs e)
        {
            this.Hide();
            AuditLogs auditLogs = new AuditLogs();
            auditLogs.Show();

        }

        private void btnDbBackup_Click(object sender, EventArgs e)
        {
            this.Hide();

        }

        private void btnGenerateReport_Click(object sender, EventArgs e)
        {
            this.Hide();

        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void flowLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void lblRecentEq_Click(object sender, EventArgs e)
        {

        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label15_Click(object sender, EventArgs e)
        {

        }

        private void Dashboard_Load(object sender, EventArgs e)
        {
            recentlyAddedEquipmentGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            recentlyAddedEquipmentGrid.MultiSelect = false;
            recentlyAddedEquipmentGrid.ReadOnly = true;

            latestRentalRequestGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            latestRentalRequestGrid.MultiSelect = false;
            latestRentalRequestGrid.ReadOnly = true;

            LoadDashboardData();
        }



        private void lblOverdue_Click(object sender, EventArgs e)
        {

        }

        private void lblTransactions_Click(object sender, EventArgs e)
        {
            FormHelper.NavigateTo<RentalTransactions>(this);
        }

        private void lblReturnRecords_Click(object sender, EventArgs e)
        {
            FormHelper.NavigateTo<ReturnRecords>(this);
        }

        private void lblRequest_Click(object sender, EventArgs e)
        {
            FormHelper.NavigateTo<RentalRequests>(this);
        }



        private void LoadDashboardData()
        {
            try
            {
                // Equipment Summary
                lbleqTotal.Text = context.Equipment.Count().ToString();

                lblavEq.Text = context.Equipment
                    .Where(e => e.AvailableId == 1)
                    .Count().ToString();

                lblrentedEquipment.Text = context.Equipment
                    .Where(e => e.AvailableId == 2)
                    .Count().ToString();

                // Rental Summary
                lblActiveRentals.Text = context.RentalTransactions.Count().ToString();

                lblPendingRequests.Text = context.RentalRequests
                    .Where(r => r.RentalStatus1.Status == "Pending")
                    .Count().ToString();

                lblOverdueRequests.Text = context.RentalTransactions
                    .Where(r => r.ReturnDate < DateTime.Today)
                    .Count().ToString();

                // Recently Added Equipment - Top 3
                var recentEquipment = context.Equipment
                    .OrderByDescending(e => e.Id)
                    .Take(3)
                    .Select(e => new
                    {
                        e.Id,
                        e.Name,
                        e.Description,
                        Category = e.Category.Name,
                        Availability = e.Available.Status,
                        Condition = e.Condition.Status
                    }).ToList();

                recentlyAddedEquipmentGrid.DataSource = recentEquipment;

                // Latest Rental Requests - Top 3
                var latestRequests = context.RentalRequests
                    .OrderByDescending(r => r.Id)
                    .Take(3)
                    .Select(r => new
                    {
                        r.Id,
                        EquipmentName = r.Equipment.Name,
                        r.StartDate,
                        r.ReturnDate,
                        Status = r.RentalStatus1.Status
                    }).ToList();

                latestRentalRequestGrid.DataSource = latestRequests;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading dashboard data: " + ex.Message);
            }
        }

        private void lblEquipmentManagement_Click(object sender, EventArgs e)
        {
            FormHelper.NavigateTo<EquipmentManagement>(this);
        }

        private void lblExit_Click(object sender, EventArgs e)
        {
            FormHelper.ExitApp();
        }

        private void lblViewAuditLogs_Click(object sender, EventArgs e)
        {
            FormHelper.NavigateTo<AuditLogs>(this);
        }

        private void lblDBbackup_Click(object sender, EventArgs e)
        {
            FormHelper.NavigateTo<DatabaseBackup>(this);
        }

        private void lblGenerateReport_Click(object sender, EventArgs e)
        {
            FormHelper.NavigateTo<GenerateReports>(this);
        }

        private void lblLogOut_Click(object sender, EventArgs e)
        {
            FormHelper.ConfirmAndLogout(this);
        }
    }
}
