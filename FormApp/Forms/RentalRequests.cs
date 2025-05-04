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
    public partial class RentalRequests : Form
    {
        DBContext context;
        public RentalRequests()
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

        private void lblDashboard_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void RentalRequest_Load(object sender, EventArgs e)
        {
            RentalRequestGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            RentalRequestGrid.MultiSelect = false;
            RentalRequestGrid.ReadOnly = true;
            LoadRentalRequest();
        }

        private void LoadRentalRequest()
        {
            try
            {
                var requests = context.RentalRequests
                    .Include(r => r.Equipment)
                    .Include(r => r.RentalStatus1)
                    .Select(r => new
                    {
                        r.Id,
                        EquipmentName = r.Equipment.Name,
                        r.StartDate,
                        r.ReturnDate,
                        r.Cost,
                        RentalStatus = r.RentalStatus1.Status
                    })
                    .ToList();

                RentalRequestGrid.DataSource = requests;
                RentalRequestGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                RentalRequestGrid.MultiSelect = false;
                RentalRequestGrid.ReadOnly = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                string search = requestId.Text.Trim();

                if (string.IsNullOrWhiteSpace(search))
                {
                    MessageBox.Show("Please enter a value to search.");
                    return;
                }

                using (DBContext context = new DBContext())
                {
                    var query = context.RentalRequests
                        .Include(r => r.Equipment)
                        .Include(r => r.RentalStatus1)
                        .AsQueryable();

                    if (int.TryParse(search, out int id))
                    {
                        query = query.Where(r => r.Id == id);
                    }
                    else
                    {
                        query = query.Where(r => r.Equipment.Name.Contains(search) ||
                                                 r.RentalStatus1.Status.Contains(search));
                    }

                    var result = query.Select(r => new
                    {
                        r.Id,
                        EquipmentName = r.Equipment.Name,
                        r.StartDate,
                        r.ReturnDate,
                        r.Cost,
                        RentalStatus = r.RentalStatus1.Status
                    }).ToList();

                    if (result.Count == 0)
                    {
                        MessageBox.Show("No matching record found.");
                        return;
                    }

                    RentalRequestGrid.DataSource = result;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadRentalRequest();
        }

        private void btnReject_Click(object sender, EventArgs e)
        {
            if (RentalRequestGrid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a record to reject.");
                return;
            }

            try
            {
                int selectedId = Convert.ToInt32(RentalRequestGrid.SelectedRows[0].Cells["Id"].Value);

                var request = context.RentalRequests
                    .Include(r => r.Equipment)
                    .Include(r => r.RentalStatus1)
                    .FirstOrDefault(r => r.Id == selectedId);

                if (request != null)
                {
                    if (request.RentalStatus1.Status != "Pending")
                    {
                        MessageBox.Show("Only requests with status 'Pending' can be rejected.");
                        return;
                    }

                    var rejectedStatus = context.RentalStatuses.FirstOrDefault(s => s.Status == "Rejected");

                    if (rejectedStatus == null)
                    {
                        MessageBox.Show("Rejected status not found in database.");
                        return;
                    }

                    request.RentalStatus = rejectedStatus.Id;
                    context.SaveChanges();

                    // Log the rejection
                    Log log = new Log
                    {
                        UserId = UserSession.UserID,
                        Action = "Reject Rental Request",
                        TimeStamp = DateTime.Now,
                        AffectedData = $"Rental Request ID {request.Id} rejected for Equipment: {request.Equipment?.Name ?? "Unknown"}",
                        Source = "RentalRequests Form"
                    };
                    context.Logs.Add(log);
                    context.SaveChanges(); // Save log

                    MessageBox.Show("Request rejected.");
                    LoadRentalRequest();
                }
                else
                {
                    MessageBox.Show("Request not found.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            try
            {
                if (RentalRequestGrid.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Please select a rental request to update.");
                    return;
                }

                int selectedId = Convert.ToInt32(RentalRequestGrid.SelectedRows[0].Cells["Id"].Value);
                var request = context.RentalRequests
                    .Include(r => r.Equipment)
                    .Include(r => r.RentalStatus1)
                    .FirstOrDefault(r => r.Id == selectedId);

                if (request != null)
                {
                    UpdateRentalRequest updateForm = new UpdateRentalRequest(request);
                    updateForm.ShowDialog();

                    LoadRentalRequest(); // Refresh after update
                }
                else
                {
                    MessageBox.Show("Selected rental request not found.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }


        private void label16_Click(object sender, EventArgs e)
        {
            FormHelper.NavigateTo<Dashboard>(this);

        }

        private void lblTransactions_Click(object sender, EventArgs e)
        {
            FormHelper.NavigateTo<RentalTransactions>(this);
        }

        private void lblReturnRecords_Click(object sender, EventArgs e)
        {
            FormHelper.NavigateTo<ReturnRecords>(this);

        }

        private void lblEquipmentManagement_Click(object sender, EventArgs e)
        {
            FormHelper.NavigateTo<EquipmentManagement>(this);

        }

        private void lblViewAuditLogs_Click(object sender, EventArgs e)
        {
            // display audit logs form
            FormHelper.NavigateTo<AuditLogs>(this);
        }

        private void lblDBbackup_Click(object sender, EventArgs e)
        {
            // display database backup form
            FormHelper.NavigateTo<DatabaseBackup>(this);
        }

        private void lblGenerateReport_Click(object sender, EventArgs e)
        {
            // display generate reports form
            FormHelper.NavigateTo<GenerateReports>(this);
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

        private void flowLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
