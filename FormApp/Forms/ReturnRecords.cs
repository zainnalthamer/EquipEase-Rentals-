using ClassLibrary.Models;
using ClassLibrary.Persistence;
using FormApp.Classes;
using FormApp.Forms;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FormApp
{
    public partial class ReturnRecords : Form
    {
        DBContext context;
        public ReturnRecords()
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

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void ReturnRecords_Load(object sender, EventArgs e)
        {
            ReturnRecordGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            ReturnRecordGrid.MultiSelect = false;
            ReturnRecordGrid.ReadOnly = true;
            LoadReturnRecord();
        }

        private void ReturnRecords_Load_1(object sender, EventArgs e)
        {
            LoadReturnRecord();
        }

        private void LoadReturnRecord()
        {

            try
            {
                var records = context.ReturnRecords
                    .Include(r => r.EquipmentNavigation)
                    .Include(r => r.ConditionNavigation)
                    .Select(r => new
                    {
                        r.Id,
                        EquipmentName = r.EquipmentNavigation.Name,
                        Condition = r.ConditionNavigation.Status,
                        r.ReturnDate,
                        r.LateFees
                    }).ToList();

                ReturnRecordGrid.DataSource = records;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching records: " + ex.Message);
            }

        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                string search = txtReturnRecordId.Text.ToLower();

                if (string.IsNullOrWhiteSpace(search))
                {
                    MessageBox.Show("Please enter a value to search.");
                    return;
                }

                IQueryable<ReturnRecord> query = context.ReturnRecords
                    .Include(r => r.EquipmentNavigation)
                    .Include(r => r.ConditionNavigation);

                if (int.TryParse(search, out int id))
                {
                    query = query.Where(r => r.Id == id);
                }
                else
                {
                    query = query.Where(r =>
                        r.EquipmentNavigation.Name.ToLower().Contains(search) ||
                        r.ConditionNavigation.Status.ToLower().Contains(search)
                    );
                }

                var result = query.Select(r => new
                {
                    r.Id,
                    EquipmentName = r.EquipmentNavigation.Name,
                    Condition = r.ConditionNavigation.Status,
                    r.ReturnDate,
                    r.LateFees
                }).ToList();

                if (result.Count == 0)
                    MessageBox.Show("No matching record found.");

                ReturnRecordGrid.DataSource = result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error during search: " + ex.Message);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (ReturnRecordGrid.SelectedRows.Count > 0)
                {
                    int id = Convert.ToInt32(ReturnRecordGrid.SelectedRows[0].Cells["Id"].Value);

                    var record = context.ReturnRecords.Find(id);

                    if (record != null)
                    {
                        var confirm = MessageBox.Show("Are you sure to delete this record?", "Confirm Delete", MessageBoxButtons.YesNo);
                        if (confirm == DialogResult.Yes)
                        {
                            // Get equipment name before deleting
                            var equipmentName = context.Equipment.FirstOrDefault(e => e.Id == record.Equipment)?.Name ?? "Unknown";

                            context.ReturnRecords.Remove(record);
                            context.SaveChanges();

                            // Log the deletion
                            Log log = new Log
                            {
                                UserId = UserSession.UserID,
                                Action = "Delete Return Record",
                                TimeStamp = DateTime.Now,
                                AffectedData = $"Deleted Return Record ID {record.Id} for Equipment: {equipmentName}",
                                Source = "ReturnRecords Form"
                            };

                            context.Logs.Add(log);
                            context.SaveChanges(); // Save log

                            MessageBox.Show("Record deleted successfully.");
                            LoadReturnRecord();
                        }
                    }
                    }
                    else
                {
                    MessageBox.Show("Please select a row to delete.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting record: " + ex.Message);
            }

        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                LoadReturnRecord();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error refreshing data: " + ex.Message);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (ReturnRecordGrid.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Please select a record to edit.");
                    return;
                }

                int id = Convert.ToInt32(ReturnRecordGrid.SelectedRows[0].Cells["Id"].Value);

                // Find ReturnRecord from DB
                var record = context.ReturnRecords
                    .Include(r => r.EquipmentNavigation)
                    .Include(r => r.ConditionNavigation)
                    .FirstOrDefault(r => r.Id == id);

                if (record != null)
                {
                    UpdateRecord updateForm = new UpdateRecord(record);
                    updateForm.ShowDialog();

                    //After editing → refresh data
                    LoadReturnRecord();
                }
                else
                {
                    MessageBox.Show("Record not found.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error opening edit form: " + ex.Message);
            }
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            FormHelper.NavigateTo<CreateRecord>(this);
        }

        private void label16_Click(object sender, EventArgs e)
        {
            FormHelper.NavigateTo<Dashboard>(this);
        }

        private void lblRequest_Click(object sender, EventArgs e)
        {
            FormHelper.NavigateTo<RentalRequests>(this);
        }

        private void lblEquipmentManagement_Click(object sender, EventArgs e)
        {
            FormHelper.NavigateTo<EquipmentManagement>(this);
        }

        private void lblViewAuditLogs_Click(object sender, EventArgs e)
        {
            // display view audit form
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

        private void lblTransactions_Click(object sender, EventArgs e)
        {
            // display rental transactions form
            FormHelper.NavigateTo<RentalTransactions>(this);
        }
    }

}


