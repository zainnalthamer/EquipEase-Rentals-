using ClassLibrary.Persistence;
using FormApp.Classes;
using FormApp.Forms;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph.Drives.Item.Items.Item.GetActivitiesByIntervalWithStartDateTimeWithEndDateTimeWithInterval;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClassLibrary.Models;

namespace FormApp
{
    public partial class EquipmentManagement : Form
    {
        DBContext context;
        public EquipmentManagement()
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

        private void EquipmentManagement_Load(object sender, EventArgs e)
        {
            eqGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            eqGrid.MultiSelect = false;
            eqGrid.ReadOnly = true;
            LoadEquipment();
        }

        private void LoadEquipment()
        {

            try
            {
                var equipmentData = context.Equipment
                    .Include(e => e.Category)
                    .Include(e => e.Available)
                    .Include(e => e.Condition)
                    .Select(e => new
                    {
                        e.Id,
                        e.Name,
                        e.Description,
                        Category = e.Category.Name,
                        Availability = e.Available.Status,
                        Condition = e.Condition.Status,
                        e.Price
                    }).ToList();

                eqGrid.DataSource = equipmentData;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message);
            }


        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    MessageBox.Show("Please enter a value to search.");
                    return;
                }

                var search = txtSearch.Text.ToLower();

                var query = from ee in context.Equipment
                            select ee;

                if (int.TryParse(search, out int id))
                {
                    query = query.Where(ee => ee.Id == id);
                }
                else
                {
                    query = query.Where(ee =>
                        ee.Name.ToLower().Contains(search) ||
                        ee.Description.ToLower().Contains(search) ||
                        ee.Category.Name.ToLower().Contains(search) ||
                        ee.Available.Status.ToLower().Contains(search) ||
                        ee.Condition.Status.ToLower().Contains(search)
                    );
                }

                var result = from ee in query
                             select new
                             {
                                 ee.Id,
                                 ee.Name,
                                 ee.Description,
                                 Category = ee.Category.Name,
                                 Availability = ee.Available.Status,
                                 Condition = ee.Condition.Status,
                                 ee.Price
                             };

                if (!result.Any())
                {
                    MessageBox.Show("No matching record found.");
                }

                eqGrid.DataSource = result.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching data: " + ex.Message);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadEquipment();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (eqGrid.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Please select a record to delete.");
                    return;
                }

                int id = Convert.ToInt32(eqGrid.SelectedRows[0].Cells["Id"].Value);

                var equipment = context.Equipment.Find(id);

                if (equipment != null)
                {
                    string equipmentName = equipment.Name;

                    context.Equipment.Remove(equipment);
                    context.SaveChanges();

                    // Log the deletion
                    Log log = new Log
                    {
                        UserId = UserSession.UserID,
                        Action = "Delete Equipment",
                        TimeStamp = DateTime.Now,
                        AffectedData = $"Deleted Equipment: {equipmentName}, ID: {id}",
                        Source = "EquipmentManagement Form"
                    };

                    context.Logs.Add(log);
                    context.SaveChanges(); // Save log

                    MessageBox.Show("Record deleted successfully.");
                    LoadEquipment();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting record: " + ex.Message);
            }
        }

        private void label16_Click(object sender, EventArgs e)
        {
            FormHelper.NavigateTo<Dashboard>(this);
        }

        private void lblRequest_Click(object sender, EventArgs e)
        {
            FormHelper.NavigateTo<RentalRequests>(this);
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

        private void btnAdd_Click(object sender, EventArgs e)
        {
           

            FormHelper.NavigateTo<AddEquipment>(this);
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                if (eqGrid.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Please select a record to edit.");
                    return;
                }

                int id = Convert.ToInt32(eqGrid.SelectedRows[0].Cells["Id"].Value);

                // Find Equipment from DB
                var equipment = context.Equipment
                    .Include(e => e.Category)
                    .Include(e => e.Available)
                    .Include(e => e.Condition)
                    .FirstOrDefault(e => e.Id == id);

                if (equipment != null)
                {
                    EditEquipment editForm = new EditEquipment(equipment);
                    editForm.ShowDialog();

                    //After editing → refresh data
                    LoadEquipment();
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

        private void lblTransactions_Click(object sender, EventArgs e)
        {
            FormHelper.NavigateTo<RentalTransactions>(this);
        }

        private void lblReturnRecords_Click(object sender, EventArgs e)
        {
            // display return records form
            FormHelper.NavigateTo<ReturnRecords>(this);
        }
    }
}
