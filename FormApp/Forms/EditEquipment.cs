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
    public partial class EditEquipment : Form
    {

        DBContext context;
        private Equipment selectedEquipment;
        public EditEquipment(Equipment equipment)
        {
            InitializeComponent();
            context = new DBContext();
            selectedEquipment = equipment;


            lblUserName.Text = UserSession.FullName;

            RoleHelper.ApplyRolePermissions(
            UserSession.RoleID,
            lblPosition
            );
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            FormHelper.NavigateTo<EquipmentManagement>(this);
        }

        private void btnSaveChanges_Click(object sender, EventArgs e)
        {
            try
            {
                // Validation
                if (string.IsNullOrWhiteSpace(txtName.Text) ||
                    string.IsNullOrWhiteSpace(txtDescription.Text) ||
                    string.IsNullOrWhiteSpace(txtPrice.Text) ||
                    cmbCategory.SelectedItem == null ||
                    cmbAvailability.SelectedItem == null ||
                    cmbCondition.SelectedItem == null)
                {
                    MessageBox.Show("Please fill all fields!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!decimal.TryParse(txtPrice.Text, out decimal price))
                {
                    MessageBox.Show("Invalid Price entered!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Updating the values
                selectedEquipment.Name = txtName.Text.Trim();
                selectedEquipment.Description = txtDescription.Text.Trim();
                selectedEquipment.Price = price;
                selectedEquipment.CategoryId = Convert.ToInt32(cmbCategory.SelectedValue);
                selectedEquipment.AvailableId = Convert.ToInt32(cmbAvailability.SelectedValue);
                selectedEquipment.ConditionId = Convert.ToInt32(cmbCondition.SelectedValue);

                // Update only entity state
                context.Entry(selectedEquipment).State = EntityState.Modified;
                context.SaveChanges();

                // Log equipment update
                Log log = new Log
                {
                    UserId = UserSession.UserID,
                    Action = "Edit Equipment",
                    TimeStamp = DateTime.Now,
                    AffectedData = $"Edited Equipment: {selectedEquipment.Name}, ID: {selectedEquipment.Id}, New Price: {selectedEquipment.Price}",
                    Source = "EditEquipment Form"
                };

                context.Logs.Add(log);
                context.SaveChanges(); // Save log entry

                MessageBox.Show("Equipment Updated Successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EditEquipment_Load(object sender, EventArgs e)
        {
            LoadCategories();
            LoadAvailability();
            LoadCondition();
            LoadEquipmentData();
        }

        private void LoadCategories()
        {
            cmbCategory.DataSource = context.Categories.ToList();
            cmbCategory.DisplayMember = "Name";
            cmbCategory.ValueMember = "Id";
        }

        private void LoadAvailability()
        {
            cmbAvailability.DataSource = context.AvailableStatuses.ToList();
            cmbAvailability.DisplayMember = "Status";
            cmbAvailability.ValueMember = "Id";
        }

        private void LoadCondition()
        {
            cmbCondition.DataSource = context.ConditionStatuses.ToList();
            cmbCondition.DisplayMember = "Status";
            cmbCondition.ValueMember = "Id";
        }

        private void LoadEquipmentData()
        {
            txtEquipmentID.Text = selectedEquipment.Id.ToString();
            txtName.Text = selectedEquipment.Name;
            txtDescription.Text = selectedEquipment.Description;
            txtPrice.Text = selectedEquipment.Price.ToString();
            cmbCategory.SelectedValue = selectedEquipment.CategoryId;
            cmbAvailability.SelectedValue = selectedEquipment.AvailableId;
            cmbCondition.SelectedValue = selectedEquipment.ConditionId;
        }
    }
}
