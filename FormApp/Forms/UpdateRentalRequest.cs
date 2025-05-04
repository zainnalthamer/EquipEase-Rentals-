using ClassLibrary.Persistence;
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
using ClassLibrary.Models;
using FormApp.Forms;
using FormApp.Classes;

namespace FormApp.Forms
{
    public partial class UpdateRentalRequest : Form
    {
        private RentalRequest currentRequest;
        DBContext context;
        public UpdateRentalRequest(RentalRequest request)
        {
            InitializeComponent();
            context = new DBContext();
            currentRequest = request;


            // display user info
            lblUserName.Text = UserSession.FullName;

            RoleHelper.ApplyRolePermissions(
            UserSession.RoleID,
            lblPosition
            );

            //Display the selected rental request record on the form.

            txtReqId.Text = request.Id.ToString();
            txtEqName.Text = request.Equipment?.Name ?? "Unknown";
            dptStartDate.Value = request.StartDate;
            dptReturnDate.Value = request.ReturnDate;
            txtCost.Text = request.Cost.ToString("0.00");

            LoadRentalStatuses();
            cmbRentalStatus.SelectedValue = request.RentalStatus;

        }

        private void LoadRentalStatuses()
        {
            using (var context = new DBContext())
            {
                var statuses = context.RentalStatuses.ToList();
                cmbRentalStatus.DataSource = statuses;
                cmbRentalStatus.DisplayMember = "Status";
                cmbRentalStatus.ValueMember = "Id";
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            FormHelper.NavigateTo<RentalRequests>(this);
        }

        private void UpdateRentalRequest_Load(object sender, EventArgs e)
        {

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (dptReturnDate.Value < dptStartDate.Value)
            {
                MessageBox.Show("Return date cannot be before start date.");
                return;
            }

            if (!decimal.TryParse(txtCost.Text.Trim(), out decimal cost))
            {
                MessageBox.Show("Please enter a valid numeric cost.");
                return;
            }

            if (cmbRentalStatus.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a rental status.");
                return;
            }

            try
            {
                using (var context = new DBContext())
                {
                    var requestToUpdate = context.RentalRequests.FirstOrDefault(r => r.Id == currentRequest.Id);

                    if (requestToUpdate != null)
                    {
                        requestToUpdate.StartDate = dptStartDate.Value;
                        requestToUpdate.ReturnDate = dptReturnDate.Value;
                        requestToUpdate.Cost = cost;
                        requestToUpdate.RentalStatus = Convert.ToInt32(cmbRentalStatus.SelectedValue);

                        context.SaveChanges();
                        MessageBox.Show("Rental request updated successfully.");
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Request not found.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void txtEqName_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
