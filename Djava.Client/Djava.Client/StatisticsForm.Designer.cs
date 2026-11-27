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
            this.btnFilter = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.dtpEnd = new System.Windows.Forms.DateTimePicker();
            this.dtpStart = new System.Windows.Forms.DateTimePicker();
            this.lblStatsLinkStats = new System.Windows.Forms.Label();
            this.lblOrdersLinkStats = new System.Windows.Forms.Label();
            this.dgvStatistics = new System.Windows.Forms.DataGridView();
            this.panelStatsBottom = new System.Windows.Forms.Panel();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnReport = new System.Windows.Forms.Button();
            this.panelStatsTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStatistics)).BeginInit();
            this.panelStatsBottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelStatsTop
            // 
            this.panelStatsTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(58)))), ((int)(((byte)(58)))), ((int)(((byte)(58)))));
            this.panelStatsTop.Controls.Add(this.btnFilter);
            this.panelStatsTop.Controls.Add(this.label2);
            this.panelStatsTop.Controls.Add(this.label1);
            this.panelStatsTop.Controls.Add(this.dtpEnd);
            this.panelStatsTop.Controls.Add(this.dtpStart);
            this.panelStatsTop.Controls.Add(this.lblStatsLinkStats);
            this.panelStatsTop.Controls.Add(this.lblOrdersLinkStats);
            this.panelStatsTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelStatsTop.Location = new System.Drawing.Point(0, 0);
            this.panelStatsTop.Name = "panelStatsTop";
            this.panelStatsTop.Size = new System.Drawing.Size(800, 88);
            this.panelStatsTop.TabIndex = 0;
            // 
            // btnFilter
            // 
            this.btnFilter.BackColor = System.Drawing.Color.MistyRose;
            this.btnFilter.Font = new System.Drawing.Font("Palatino Linotype", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnFilter.Location = new System.Drawing.Point(626, 62);
            this.btnFilter.Name = "btnFilter";
            this.btnFilter.Size = new System.Drawing.Size(135, 23);
            this.btnFilter.TabIndex = 10;
            this.btnFilter.Text = "Расчитать";
            this.btnFilter.UseVisualStyleBackColor = false;
            this.btnFilter.Click += new System.EventHandler(this.btnFilter_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(481, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Дата конца:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(481, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Дата начала:";
            // 
            // dtpEnd
            // 
            this.dtpEnd.Location = new System.Drawing.Point(561, 38);
            this.dtpEnd.Name = "dtpEnd";
            this.dtpEnd.Size = new System.Drawing.Size(200, 20);
            this.dtpEnd.TabIndex = 7;
            // 
            // dtpStart
            // 
            this.dtpStart.Location = new System.Drawing.Point(561, 12);
            this.dtpStart.Name = "dtpStart";
            this.dtpStart.Size = new System.Drawing.Size(200, 20);
            this.dtpStart.TabIndex = 6;
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
            this.panelStatsBottom.Controls.Add(this.btnReport);
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
            // btnReport
            // 
            this.btnReport.BackColor = System.Drawing.Color.MistyRose;
            this.btnReport.Font = new System.Drawing.Font("Palatino Linotype", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnReport.Location = new System.Drawing.Point(636, 12);
            this.btnReport.Name = "btnReport";
            this.btnReport.Size = new System.Drawing.Size(152, 23);
            this.btnReport.TabIndex = 1;
            this.btnReport.Text = "Сформировать отчёт";
            this.btnReport.UseVisualStyleBackColor = false;
            this.btnReport.Click += new System.EventHandler(this.btnReport_Click);
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
        private System.Windows.Forms.Button btnFilter;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dtpEnd;
        private System.Windows.Forms.DateTimePicker dtpStart;
        private System.Windows.Forms.Button btnReport;
    }
}