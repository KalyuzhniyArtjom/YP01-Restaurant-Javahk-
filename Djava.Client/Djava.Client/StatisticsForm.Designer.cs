namespace Djava.Client
{
    partial class StatisticsForm
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
            this.panelStatsTop = new System.Windows.Forms.Panel();
            this.lblOrdersLinkStats = new System.Windows.Forms.Label();
            this.lblStatsLinkStats = new System.Windows.Forms.Label();
            this.dgvStatistics = new System.Windows.Forms.DataGridView();
            this.panelStatsBottom = new System.Windows.Forms.Panel();
            this.lblStatus = new System.Windows.Forms.Label();
            this.panelStatsTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStatistics)).BeginInit();
            this.panelStatsBottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelStatsTop
            // 
            this.panelStatsTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(58)))), ((int)(((byte)(58)))), ((int)(((byte)(58)))));
            this.panelStatsTop.Controls.Add(this.lblStatsLinkStats);
            this.panelStatsTop.Controls.Add(this.lblOrdersLinkStats);
            this.panelStatsTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelStatsTop.Location = new System.Drawing.Point(0, 0);
            this.panelStatsTop.Name = "panelStatsTop";
            this.panelStatsTop.Size = new System.Drawing.Size(800, 88);
            this.panelStatsTop.TabIndex = 0;
            // 
            // lblOrdersLinkStats
            // 
            this.lblOrdersLinkStats.AutoSize = true;
            this.lblOrdersLinkStats.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblOrdersLinkStats.Font = new System.Drawing.Font("Sitka Heading", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblOrdersLinkStats.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.lblOrdersLinkStats.Location = new System.Drawing.Point(12, 9);
            this.lblOrdersLinkStats.Name = "lblOrdersLinkStats";
            this.lblOrdersLinkStats.Size = new System.Drawing.Size(153, 30);
            this.lblOrdersLinkStats.TabIndex = 1;
            this.lblOrdersLinkStats.Text = "Список заказов";
            this.lblOrdersLinkStats.Click += new System.EventHandler(this.lblOrdersLinkStats_Click);
            // 
            // lblStatsLinkStats
            // 
            this.lblStatsLinkStats.AutoSize = true;
            this.lblStatsLinkStats.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblStatsLinkStats.Font = new System.Drawing.Font("Sitka Heading", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblStatsLinkStats.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.lblStatsLinkStats.Location = new System.Drawing.Point(171, 9);
            this.lblStatsLinkStats.Name = "lblStatsLinkStats";
            this.lblStatsLinkStats.Size = new System.Drawing.Size(224, 30);
            this.lblStatsLinkStats.TabIndex = 5;
            this.lblStatsLinkStats.Text = "Статистика по заказам";
            this.lblStatsLinkStats.Click += new System.EventHandler(this.lblStatsLinkStats_Click);
            // 
            // dgvStatistics
            // 
            this.dgvStatistics.AllowUserToAddRows = false;
            this.dgvStatistics.BackgroundColor = System.Drawing.Color.White;
            this.dgvStatistics.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvStatistics.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvStatistics.Location = new System.Drawing.Point(0, 88);
            this.dgvStatistics.Name = "dgvStatistics";
            this.dgvStatistics.ReadOnly = true;
            this.dgvStatistics.Size = new System.Drawing.Size(800, 362);
            this.dgvStatistics.TabIndex = 1;
            // 
            // panelStatsBottom
            // 
            this.panelStatsBottom.Controls.Add(this.lblStatus);
            this.panelStatsBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelStatsBottom.Location = new System.Drawing.Point(0, 403);
            this.panelStatsBottom.Name = "panelStatsBottom";
            this.panelStatsBottom.Size = new System.Drawing.Size(800, 47);
            this.panelStatsBottom.TabIndex = 2;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Sitka Heading", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblStatus.Location = new System.Drawing.Point(12, 17);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(46, 21);
            this.lblStatus.TabIndex = 0;
            this.lblStatus.Text = "label1";
            // 
            // StatisticsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.panelStatsBottom);
            this.Controls.Add(this.dgvStatistics);
            this.Controls.Add(this.panelStatsTop);
            this.Name = "StatisticsForm";
            this.Text = "StatisticsForm";
            this.Load += new System.EventHandler(this.StatisticsForm_Load);
            this.panelStatsTop.ResumeLayout(false);
            this.panelStatsTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStatistics)).EndInit();
            this.panelStatsBottom.ResumeLayout(false);
            this.panelStatsBottom.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelStatsTop;
        private System.Windows.Forms.Label lblOrdersLinkStats;
        private System.Windows.Forms.Label lblStatsLinkStats;
        private System.Windows.Forms.DataGridView dgvStatistics;
        private System.Windows.Forms.Panel panelStatsBottom;
        private System.Windows.Forms.Label lblStatus;
    }
}