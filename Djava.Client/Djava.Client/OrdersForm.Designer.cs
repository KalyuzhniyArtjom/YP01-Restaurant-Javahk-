namespace Djava.Client
{
    partial class OrdersForm
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
            this.panelTop = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.lblAdmin = new System.Windows.Forms.Label();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnLogout = new System.Windows.Forms.Button();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.cmbStatus = new System.Windows.Forms.ComboBox();
            this.btnChangeStatus = new System.Windows.Forms.Button();
            this.lblCount = new System.Windows.Forms.Label();
            this.dgvOrders = new System.Windows.Forms.DataGridView();
            this.panelTop.SuspendLayout();
            this.panelBottom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvOrders)).BeginInit();
            this.SuspendLayout();
            // 
            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(58)))), ((int)(((byte)(58)))), ((int)(((byte)(58)))));
            this.panelTop.Controls.Add(this.btnLogout);
            this.panelTop.Controls.Add(this.lblAdmin);
            this.panelTop.Controls.Add(this.label1);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(984, 100);
            this.panelTop.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label1.Location = new System.Drawing.Point(15, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(151, 24);
            this.label1.TabIndex = 0;
            this.label1.Text = "Список заказов";
            // 
            // lblAdmin
            // 
            this.lblAdmin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblAdmin.AutoSize = true;
            this.lblAdmin.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblAdmin.ForeColor = System.Drawing.Color.Snow;
            this.lblAdmin.Location = new System.Drawing.Point(380, 20);
            this.lblAdmin.Name = "lblAdmin";
            this.lblAdmin.Size = new System.Drawing.Size(154, 24);
            this.lblAdmin.TabIndex = 1;
            this.lblAdmin.Text = "Администратор";
            this.lblAdmin.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefresh.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(184)))), ((int)(((byte)(160)))), ((int)(((byte)(165)))));
            this.btnRefresh.Location = new System.Drawing.Point(640, 15);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(100, 30);
            this.btnRefresh.TabIndex = 1;
            this.btnRefresh.Text = "Обновить";
            this.btnRefresh.UseVisualStyleBackColor = false;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnLogout
            // 
            this.btnLogout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLogout.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.btnLogout.Location = new System.Drawing.Point(750, 15);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(80, 30);
            this.btnLogout.TabIndex = 2;
            this.btnLogout.Text = "Выйти";
            this.btnLogout.UseVisualStyleBackColor = false;
            this.btnLogout.Click += new System.EventHandler(this.btnLogout_Click);
            // 
            // panelBottom
            // 
            this.panelBottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(236)))), ((int)(((byte)(236)))));
            this.panelBottom.Controls.Add(this.lblCount);
            this.panelBottom.Controls.Add(this.cmbStatus);
            this.panelBottom.Controls.Add(this.btnChangeStatus);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(0, 461);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(984, 100);
            this.panelBottom.TabIndex = 2;
            // 
            // cmbStatus
            // 
            this.cmbStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbStatus.FormattingEnabled = true;
            this.cmbStatus.Location = new System.Drawing.Point(12, 21);
            this.cmbStatus.Name = "cmbStatus";
            this.cmbStatus.Size = new System.Drawing.Size(200, 21);
            this.cmbStatus.TabIndex = 3;
            // 
            // btnChangeStatus
            // 
            this.btnChangeStatus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(184)))), ((int)(((byte)(160)))), ((int)(((byte)(165)))));
            this.btnChangeStatus.Location = new System.Drawing.Point(230, 15);
            this.btnChangeStatus.Name = "btnChangeStatus";
            this.btnChangeStatus.Size = new System.Drawing.Size(160, 30);
            this.btnChangeStatus.TabIndex = 0;
            this.btnChangeStatus.Text = "Сменить статус";
            this.btnChangeStatus.UseVisualStyleBackColor = false;
            this.btnChangeStatus.Click += new System.EventHandler(this.btnChangeStatus_Click);
            // 
            // lblCount
            // 
            this.lblCount.AutoSize = true;
            this.lblCount.BackColor = System.Drawing.Color.Silver;
            this.lblCount.Location = new System.Drawing.Point(800, 18);
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(94, 13);
            this.lblCount.TabIndex = 3;
            this.lblCount.Text = "Всего заказов: 0";
            // 
            // dgvOrders
            // 
            this.dgvOrders.AllowUserToAddRows = false;
            this.dgvOrders.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvOrders.BackgroundColor = System.Drawing.Color.White;
            this.dgvOrders.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvOrders.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvOrders.Location = new System.Drawing.Point(0, 100);
            this.dgvOrders.MultiSelect = false;
            this.dgvOrders.Name = "dgvOrders";
            this.dgvOrders.ReadOnly = true;
            this.dgvOrders.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvOrders.Size = new System.Drawing.Size(984, 361);
            this.dgvOrders.TabIndex = 3;
            this.dgvOrders.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvOrders_CellDoubleClick);
            this.dgvOrders.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvOrders_CellFormatting);
            // 
            // OrdersForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 561);
            this.Controls.Add(this.dgvOrders);
            this.Controls.Add(this.panelBottom);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.panelTop);
            this.Name = "OrdersForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Список заказов — Ресторан «Джавахк»";
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.panelBottom.ResumeLayout(false);
            this.panelBottom.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvOrders)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Button btnLogout;
        private System.Windows.Forms.Label lblAdmin;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.Label lblCount;
        private System.Windows.Forms.ComboBox cmbStatus;
        private System.Windows.Forms.Button btnChangeStatus;
        private System.Windows.Forms.DataGridView dgvOrders;
    }
}