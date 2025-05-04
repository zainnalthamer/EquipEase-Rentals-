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
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FormApp
{
    public partial class RentalTransactions : Form
    {

        DBContext context;
        public RentalTransactions()
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

        private void RentalTransactions_Load(object sender, EventArgs e)
        {
            transactionGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            transactionGrid.MultiSelect = false;
            transactionGrid.ReadOnly = true;
            LoadRentalTransactions();
        }

        private void LoadRentalTransactions()
        {
            try
            {
                var transactions = context.RentalTransactions
                    .Include(t => t.User)
                    .Include(t => t.PaymentStatusNavigation)
                    .Include(t => t.RentalStatusNavigation)
                    .Select(t => new
                    {
                        t.Id,
                        FullName = t.User.Fname,
                        PaymentStatus = t.PaymentStatusNavigation.Status,
                        RentalStatus = t.RentalStatusNavigation.Status,
                        Fee = t.Fee,
                        PickupDate = t.Pickup,
                        ReturnDate = t.ReturnDate
                    }).ToList();

                transactionGrid.DataSource = transactions;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching data: " + ex.Message);
            }
        }


        private void lblTransactions_Click(object sender, EventArgs e)
        {

        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void flowLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (transactionGrid.SelectedRows.Count > 0)
                {
                    int id = Convert.ToInt32(transactionGrid.SelectedRows[0].Cells["Id"].Value);

                    var transaction = context.RentalTransactions.Find(id);

                    if (transaction != null)
                    {
                        var confirm = MessageBox.Show("Are you sure to delete this transaction?", "Confirm Delete", MessageBoxButtons.YesNo);
                        if (confirm == DialogResult.Yes)
                        {
                            context.RentalTransactions.Remove(transaction);
                            context.SaveChanges();

                            // Log the deletion
                            Log log = new Log
                            {
                                UserId = UserSession.UserID,
                                Action = "Delete Rental Transaction",
                                TimeStamp = DateTime.Now,
                                AffectedData = $"Deleted Rental Transaction ID {transaction.Id}",
                                Source = "RentalTransactions Form"
                            };
                            context.Logs.Add(log);
                            context.SaveChanges(); // Save the log entry

                            MessageBox.Show("Transaction deleted successfully.");
                            LoadRentalTransactions();
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
                MessageBox.Show("Error deleting transaction: " + ex.Message);
            }

        }


        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                string search = txtTransactionID.Text.ToLower();

                if (string.IsNullOrWhiteSpace(search))
                {
                    MessageBox.Show("Please enter search value.");
                    return;
                }

                IQueryable<RentalTransaction> query = context.RentalTransactions
                    .Include(t => t.User)
                    .Include(t => t.PaymentStatusNavigation)
                    .Include(t => t.RentalStatusNavigation);

                if (int.TryParse(search, out int id))
                {
                    query = query.Where(t => t.Id == id);
                }
                else
                {
                    query = query.Where(t =>
                        t.User.Fname.ToLower().Contains(search) ||
                        t.PaymentStatusNavigation.Status.ToLower().Contains(search) ||
                        t.RentalStatusNavigation.Status.ToLower().Contains(search)
                    );
                }

                var result = query.Select(t => new
                {
                    t.Id,
                    FullName = t.User.Fname,
                    PaymentStatus = t.PaymentStatusNavigation.Status,
                    RentalStatus = t.RentalStatusNavigation.Status,
                    Fee = t.Fee,
                    PickupDate = t.Pickup,
                    ReturnDate = t.ReturnDate
                }).ToList();

                if (result.Count == 0)
                    MessageBox.Show("No matching record found.");

                transactionGrid.DataSource = result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error during search: " + ex.Message);
            }

        }


        //private void btnFilter_Click(object sender, EventArgs e)
        //{
        //    string searchText = txtTransactionID.Text.ToLower();

        //    var filteredData = context.RentalTransactions
        //        .Include(t => t.PaymentStatusNavigation)
        //        .Include(t => t.RentalStatusNavigation)
        //        .Include(t => t.User)
        //        .Where(t =>
        //            t.Id.ToString().Contains(searchText) ||
        //            t.User.Fname.ToLower().Contains(searchText) ||
        //            t.PaymentStatusNavigation.Status.ToLower().Contains(searchText) ||
        //            t.RentalStatusNavigation.Status.ToLower().Contains(searchText) ||
        //            t.Fee.ToString().Contains(searchText) ||
        //            t.Pickup.ToString().Contains(searchText) ||
        //            t.ReturnDate.ToString().Contains(searchText)
        //        )
        //        .Select(t => new
        //        {
        //            t.Id,
        //            FullName = t.User.Fname,
        //            PaymentStatus = t.PaymentStatusNavigation.Status,
        //            RentalStatus = t.RentalStatusNavigation.Status,
        //            Fee = t.Fee,
        //            PickupDate = t.Pickup,
        //            ReturnDate = t.ReturnDate
        //        }).ToList();

        //    transactionGrid.DataSource = filteredData;
        //}

        private void lblReturnRecords_Click(object sender, EventArgs e)
        {
            FormHelper.NavigateTo<ReturnRecords>(this);
        }

        private void lblEquipmentManagement_Click(object sender, EventArgs e)
        {
            FormHelper.NavigateTo<EquipmentManagement>(this);
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                LoadRentalTransactions();
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
                if (transactionGrid.SelectedRows.Count > 0)
                {
                    int id = Convert.ToInt32(transactionGrid.SelectedRows[0].Cells["Id"].Value);

                    // display update transaction form
                    this.Hide();
                    UpdateTransaction updateTransaction = new UpdateTransaction(id);
                    updateTransaction.Show();
                    LoadRentalTransactions();
                }
                else
                {
                    MessageBox.Show("Please select a row to update.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating transaction: " + ex.Message);
            }
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            // display create transaction form
            FormHelper.NavigateTo<CreateTransaction>(this);
        }

        private void label16_Click(object sender, EventArgs e)
        {
            FormHelper.NavigateTo<Dashboard>(this);
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

        private void lblRequest_Click(object sender, EventArgs e)
        {
            FormHelper.NavigateTo<RentalRequests>(this);
        }
    }

}

