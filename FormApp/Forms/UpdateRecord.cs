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

namespace FormApp.Forms
{
    public partial class UpdateRecord : Form
    {

        DBContext context;
        private ReturnRecord selectedRecord; // hold incoming object
        public UpdateRecord(ReturnRecord record)
        {
            InitializeComponent();
            context = new DBContext();
            selectedRecord = record;


            lblUserName.Text = UserSession.FullName;

            RoleHelper.ApplyRolePermissions(
            UserSession.RoleID,
            lblPosition
            );
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            FormHelper.NavigateTo<ReturnRecords>(this);
        }



        private void UpdateRecord_Load(object sender, EventArgs e)
        {
            LoadCondition();
            LoadData();

        }

        private void LoadCondition()
        {
            cmbCondition.DataSource = context.ConditionStatuses.ToList();
            cmbCondition.DisplayMember = "Status";
            cmbCondition.ValueMember = "Id";
        }

        private void LoadData()
        {
            txtRecordID.Text = selectedRecord.Id.ToString();
            txtEquipmentID.Text = selectedRecord.Equipment.ToString();
            txtLateFees.Text = selectedRecord.LateFees.ToString();
            cmbCondition.SelectedValue = selectedRecord.Condition;
            dtpReturnDate.Value = selectedRecord.ReturnDate;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {

            try
            {
                // Validate Empty Fields
                if (cmbCondition.SelectedItem == null ||
                    string.IsNullOrWhiteSpace(txtLateFees.Text))
                {
                    MessageBox.Show("Please fill all fields!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Validate LateFees
                if (!decimal.TryParse(txtLateFees.Text, out decimal lateFees))
                {
                    MessageBox.Show("Invalid Late Fees!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Validate Date
                if (dtpReturnDate.Value.Date < DateTime.Today)
                {
                    MessageBox.Show("Return Date cannot be before today's date!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Update values
                selectedRecord.Condition = Convert.ToInt32(cmbCondition.SelectedValue);
                selectedRecord.LateFees = lateFees;
                selectedRecord.ReturnDate = dtpReturnDate.Value;

                // Update DB
                context.Entry(selectedRecord).State = EntityState.Modified;
                context.SaveChanges();

                // Log the update
                Log log = new Log
                {
                    UserId = UserSession.UserID,
                    Action = "Update Return Record",
                    TimeStamp = DateTime.Now,
                    AffectedData = $"Updated Return Record ID {selectedRecord.Id} – Late Fees: {selectedRecord.LateFees}, Return Date: {selectedRecord.ReturnDate.ToShortDateString()}",
                    Source = "UpdateRecord Form"
                };
                context.Logs.Add(log);
                context.SaveChanges(); // Save log

                MessageBox.Show("Record Updated Successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
