namespace FormApp.Forms
{
    partial class LogsFilter
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            pictureBox1 = new PictureBox();
            txtLogId = new TextBox();
            txtUserId = new TextBox();
            btnApplyFilters = new Button();
            cmbAction = new ComboBox();
            lblLogId = new Label();
            lblUserID = new Label();
            lblDate = new Label();
            lblAction = new Label();
            dtpDate = new DateTimePicker();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Image = Properties.Resources.Logo;
            pictureBox1.Location = new Point(329, 60);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(296, 312);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 1;
            pictureBox1.TabStop = false;
            // 
            // txtLogId
            // 
            txtLogId.BackColor = Color.WhiteSmoke;
            txtLogId.Location = new Point(162, 444);
            txtLogId.Name = "txtLogId";
            txtLogId.Size = new Size(312, 39);
            txtLogId.TabIndex = 2;
            // 
            // txtUserId
            // 
            txtUserId.BackColor = Color.WhiteSmoke;
            txtUserId.Location = new Point(497, 444);
            txtUserId.Name = "txtUserId";
            txtUserId.Size = new Size(312, 39);
            txtUserId.TabIndex = 3;
            // 
            // btnApplyFilters
            // 
            btnApplyFilters.BackColor = Color.FromArgb(249, 115, 22);
            btnApplyFilters.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnApplyFilters.ForeColor = Color.White;
            btnApplyFilters.Location = new Point(264, 633);
            btnApplyFilters.Name = "btnApplyFilters";
            btnApplyFilters.Size = new Size(463, 56);
            btnApplyFilters.TabIndex = 6;
            btnApplyFilters.Text = "Apply Filters";
            btnApplyFilters.UseVisualStyleBackColor = false;
            btnApplyFilters.Click += btnApplyFilters_Click;
            // 
            // cmbAction
            // 
            cmbAction.BackColor = Color.WhiteSmoke;
            cmbAction.FormattingEnabled = true;
            cmbAction.Location = new Point(162, 554);
            cmbAction.Name = "cmbAction";
            cmbAction.Size = new Size(312, 40);
            cmbAction.TabIndex = 7;
            // 
            // lblLogId
            // 
            lblLogId.AutoSize = true;
            lblLogId.Location = new Point(162, 400);
            lblLogId.Margin = new Padding(5, 0, 5, 0);
            lblLogId.Name = "lblLogId";
            lblLogId.Size = new Size(88, 32);
            lblLogId.TabIndex = 76;
            lblLogId.Text = "Log ID:";
            // 
            // lblUserID
            // 
            lblUserID.AutoSize = true;
            lblUserID.Location = new Point(497, 400);
            lblUserID.Margin = new Padding(5, 0, 5, 0);
            lblUserID.Name = "lblUserID";
            lblUserID.Size = new Size(96, 32);
            lblUserID.TabIndex = 77;
            lblUserID.Text = "User ID:";
            // 
            // lblDate
            // 
            lblDate.AutoSize = true;
            lblDate.Location = new Point(497, 510);
            lblDate.Margin = new Padding(5, 0, 5, 0);
            lblDate.Name = "lblDate";
            lblDate.Size = new Size(69, 32);
            lblDate.TabIndex = 79;
            lblDate.Text = "Date:";
            // 
            // lblAction
            // 
            lblAction.AutoSize = true;
            lblAction.Location = new Point(162, 510);
            lblAction.Margin = new Padding(5, 0, 5, 0);
            lblAction.Name = "lblAction";
            lblAction.Size = new Size(87, 32);
            lblAction.TabIndex = 78;
            lblAction.Text = "Action:";
            // 
            // dtpDate
            // 
            dtpDate.CalendarMonthBackground = Color.WhiteSmoke;
            dtpDate.Location = new Point(497, 555);
            dtpDate.Name = "dtpDate";
            dtpDate.Size = new Size(312, 39);
            dtpDate.TabIndex = 80;
            // 
            // LogsFilter
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(979, 743);
            Controls.Add(dtpDate);
            Controls.Add(lblDate);
            Controls.Add(lblAction);
            Controls.Add(lblUserID);
            Controls.Add(lblLogId);
            Controls.Add(cmbAction);
            Controls.Add(btnApplyFilters);
            Controls.Add(txtUserId);
            Controls.Add(txtLogId);
            Controls.Add(pictureBox1);
            Name = "LogsFilter";
            Text = "Filter";
            Load += LogsFilter_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private PictureBox pictureBox1;
        private TextBox txtLogId;
        private TextBox txtUserId;
        private Button btnApplyFilters;
        private ComboBox cmbAction;
        private Label lblLogId;
        private Label lblUserID;
        private Label lblDate;
        private Label lblAction;
        private DateTimePicker dtpDate;
    }
}