using DjavaLib.Core.Data;
using DjavaLib.Data;
using DjavaLib.Models;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Djava.Client
{
    public partial class StatisticsForm : Form
    {
        private readonly IStatisticsRepository _statsRepo;

        public StatisticsForm(DbContext context)
        {
            InitializeComponent();
            _statsRepo = new StatisticsRepository(context);

            // Выделяем активную вкладку
            lblStatsLinkStats.ForeColor = Color.White;
            lblOrdersLinkStats.ForeColor = Color.Gray;
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
                }

                lblStatus.Text = model.TotalOrders == 0
                    ? "Всего заказов: 0. Статистика успешно загружена."
                    : $"Всего заказов: {model.TotalOrders}. Статистика успешно загружена.";
            }
            catch (Exception ex)
            {
                dgvStatistics.DataSource = null;
                lblStatus.Text = $"Ошибка загрузки данных: {ex.Message}";
            }
        }

        private void lblOrdersLinkStats_Click(object sender, EventArgs e)
        {
            this.Close(); // Возврат 
        }

        private void lblStatsLinkStats_Click(object sender, EventArgs e)
        {
            // Обновляем данные при клике на активную вкладку
            StatisticsForm_Load(sender, e);
        }
    }
}