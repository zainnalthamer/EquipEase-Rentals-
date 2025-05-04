using ClassLibrary.Models;
using ClassLibrary.Persistence;
using FormApp.Classes;
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
using System.Xml.Linq;

namespace FormApp.Forms
{
    public partial class CreateRecord : Form
    {

        DBContext context;
        public CreateRecord()
        {
            InitializeComponent();
            context = new DBContext();


            lblUserName.Text = UserSession.FullName;

            RoleHelper.ApplyRolePermissions(
            UserSession.RoleID,
            lblPosition
            );

            // setting placeholders
            PlaceholderService.SetPlaceholder(txtEquipmentId, "Equipment ID");
           PlaceholderService.SetPlaceholder(txtLateFees, "Late Fee Amount");
         
        }

        private void CreateRecord_Load(object sender, EventArgs e)
        {
            LoadConditions();
        }

        private void LoadConditions()
        {
            var conditions = context.ConditionStatuses
         .Select(cs => new { cs.Id, cs.Status })
         .ToList();

            conditions.Insert(0, new { Id = -1, Status = "Select Condition" });

            cmbCondition.DataSource = conditions;
            cmbCondition.DisplayMember = "Status";
            cmbCondition.ValueMember = "Id";
            cmbCondition.SelectedIndex = 0;
        }


        //private void btnCreate_Click(object sender, EventArgs e)
        //{


        //}

        private void ClearControls()
        {
            txtEquipmentId.Clear();
            cmbCondition.SelectedIndex = 0;  
            txtLateFees.Clear();
            dtpReturnDate.Value = DateTime.Today;
        }

        private void btnCreate_Click_1(object sender, EventArgs e)
        {

            try
            {
                // Validating Required Fields
                if (string.IsNullOrWhiteSpace(txtEquipmentId.Text) ||
                    Convert.ToInt32(cmbCondition.SelectedValue) == -1 ||
                    string.IsNullOrWhiteSpace(txtLateFees.Text))
                {
                    MessageBox.Show("Please fill all fields!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Validate Equipment ID
                if (!int.TryParse(txtEquipmentId.Text.Trim(), out int equipmentId))
                {
                    MessageBox.Show("Invalid Equipment ID entered!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Validate Late Fees
                if (!decimal.TryParse(txtLateFees.Text.Trim(), out decimal lateFees))
                {
                    MessageBox.Show("Invalid Late Fee entered!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Validate Return Date
                if (dtpReturnDate.Value.Date < DateTime.Today)
                {
                    MessageBox.Show("Return Date cannot be before today's date!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Check if Equipment Exists
                bool exists = context.Equipment.Any(e => e.Id == equipmentId);
                if (!exists)
                {
                    MessageBox.Show("Equipment ID not found in the database!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Create Return Record
                ReturnRecord record = new ReturnRecord
                {
                    Equipment = equipmentId,
                    Condition = Convert.ToInt32(cmbCondition.SelectedValue),
                    ReturnDate = dtpReturnDate.Value,
                    LateFees = lateFees
                };

                context.ReturnRecords.Add(record);
                context.SaveChanges();

                // Log the action
                Log log = new Log
                {
                    UserId = UserSession.UserID,
                    Action = "Create Return Record",
                    TimeStamp = DateTime.Now,
                    AffectedData = $"Return Record for Equipment ID: {record.Equipment}, Return Date: {record.ReturnDate.ToShortDateString()}",
                    Source = "CreateRecord Form"
                };

                context.Logs.Add(log);
                context.SaveChanges(); // save log entry

                MessageBox.Show("Record created successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearControls();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            FormHelper.NavigateTo<ReturnRecords>(this);
        }
    }
}
