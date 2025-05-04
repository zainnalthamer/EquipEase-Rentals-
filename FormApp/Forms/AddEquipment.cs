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
using FormApp.Classes;

namespace FormApp.Forms
{
    public partial class AddEquipment : Form
    {
        DBContext context;
        public AddEquipment()
        {
            InitializeComponent();
            context = new DBContext();

            lblUserName.Text = UserSession.FullName;

            RoleHelper.ApplyRolePermissions(
            UserSession.RoleID,
            lblPosition
            );


            this.StartPosition = FormStartPosition.CenterScreen;

            // setting placeholders
            PlaceholderService.SetPlaceholder(txtName, "Name");
            PlaceholderService.SetPlaceholder(txtDescription, "Description");
            PlaceholderService.SetPlaceholder(txtPrice, "Price");
        }

        private void AddEquipment_Load(object sender, EventArgs e)
        {
            LoadCategories();
            LoadAvailability();
            LoadCondition();
        }

        private void LoadCategories()
        {
            var categories = context.Categories
        .Select(c => new { c.Id, c.Name })
        .ToList();

            categories.Insert(0, new { Id = -1, Name = "Category" });

            cmbCategory.DataSource = categories;
            cmbCategory.DisplayMember = "Name";
            cmbCategory.ValueMember = "Id";
            cmbCategory.SelectedIndex = 0;
        }

        private void LoadCondition()
        {
            var conditions = context.ConditionStatuses
       .Select(cs => new { cs.Id, cs.Status })
       .ToList();

            conditions.Insert(0, new { Id = -1, Status = "Condition" });

            cmbCondition.DataSource = conditions;
            cmbCondition.DisplayMember = "Status";
            cmbCondition.ValueMember = "Id";
            cmbCondition.SelectedIndex = 0;
        }

        private void LoadAvailability()
        {
            var availabilities = context.AvailableStatuses
        .Select(av => new { av.Id, av.Status })
        .ToList();

            availabilities.Insert(0, new { Id = -1, Status = "Availability" });

            cmbAvailability.DataSource = availabilities;
            cmbAvailability.DisplayMember = "Status";
            cmbAvailability.ValueMember = "Id";
            cmbAvailability.SelectedIndex = 0;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {

            try
            {
                // Validating Required Fields
                if (string.IsNullOrWhiteSpace(txtName.Text) ||
                    string.IsNullOrWhiteSpace(txtDescription.Text) ||
                    string.IsNullOrWhiteSpace(txtPrice.Text) ||
                    Convert.ToInt32(cmbCategory.SelectedValue) == -1 ||
                    Convert.ToInt32(cmbAvailability.SelectedValue) == -1 ||
                    Convert.ToInt32(cmbCondition.SelectedValue) == -1)
                {
                    MessageBox.Show("Please fill all fields!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Validate Price
                if (!decimal.TryParse(txtPrice.Text.Trim(), out decimal price))
                {
                    MessageBox.Show("Invalid Price entered!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Create Equipment Object
                Equipment newEquipment = new Equipment
                {
                    Name = txtName.Text.Trim(),
                    Description = txtDescription.Text.Trim(),
                    Price = price,
                    CategoryId = Convert.ToInt32(cmbCategory.SelectedValue),
                    AvailableId = Convert.ToInt32(cmbAvailability.SelectedValue),
                    ConditionId = Convert.ToInt32(cmbCondition.SelectedValue),
                    Image = new byte[0] // Optional, can be updated later
                };

                // Add to Database
                context.Equipment.Add(newEquipment);
                context.SaveChanges();

                // logging equipment addition
                Log log = new Log
                {
                    UserId = UserSession.UserID,
                    Action = "Add Equipment",
                    TimeStamp = DateTime.Now,
                    AffectedData = $"Equipment Added: {newEquipment.Name}, ID: {newEquipment.Id}",
                    Source = "AddEquipment Form"
                };

                context.Logs.Add(log);
                context.SaveChanges(); // save log entry

                MessageBox.Show("Equipment Added Successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                ClearControls(); // reset fields
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearControls()
        {
            txtName.Text = "";
            txtDescription.Text = "";
            txtPrice.Text = "";
            cmbCategory.SelectedIndex = -1;
            cmbAvailability.SelectedIndex = -1;
            cmbCondition.SelectedIndex = -1;
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            FormHelper.NavigateTo<EquipmentManagement>(this);
        }

        private void cmbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
