using DjavaLib.Core.Data;
using DjavaLib.Data;
using DjavaLib.Models;
using System;
using System.Windows.Forms;

namespace Djava.Client
{
    public partial class StatisticsForm : Form
    {
        private readonly IStatisticsRepository _statsRepo;

        // Конструктор ПРИНИМАЕТ DbContext
        public StatisticsForm(DbContext dbContext)
        {
            InitializeComponent();
            _statsRepo = new StatisticsRepository(dbContext);
        }

        private void StatisticsForm_Load(object sender, EventArgs e)
        {
            try
            {
                dgvStatistics.DataSource = null;
                var model = _statsRepo.GetGeneralStatistics();
                dgvStatistics.DataSource = model.Rows;

                if (dgvStatistics.Columns.Count > 0)
                {
                    dgvStatistics.Columns["IndicatorName"].HeaderText = "Показатель";
                    dgvStatistics.Columns["DisplayValue"].HeaderText = "Значение";
                    dgvStatistics.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    dgvStatistics.ReadOnly = true;
                    dgvStatistics.AllowUserToAddRows = false;
                    dgvStatistics.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                }
            }
            catch (Exception ex)
            {
                dgvStatistics.DataSource = null;
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}