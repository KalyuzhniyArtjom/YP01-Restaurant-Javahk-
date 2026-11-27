using System;
using System.Drawing;
using System.Windows.Forms;
using DjavaLib.Data;
using Xceed.Words.NET;
using Xceed.Document.NET; // Добавляем это для работы с цветами

namespace Djava.Client
{
    public partial class StatisticsForm : Form
    {
        private readonly DbContext _dbContext;
        private readonly OrderListRepository _orders;

        public StatisticsForm(DbContext dbContext)
        {
            InitializeComponent();
            _dbContext = dbContext;
            _orders = new OrderListRepository(_dbContext);

            // Настройка дат по умолчанию (с 1 числа текущего месяца по сегодня)
            dtpStart.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            dtpEnd.Value = DateTime.Now;

            LoadStatistics();
        }

        private void LoadStatistics()
        {
            try
            {
                var stats = _orders.GetStatistics(dtpStart.Value, dtpEnd.Value);

                // Имя таблицы изменено на dgvStatistics
                dgvStatistics.DataSource = null;
                dgvStatistics.DataSource = stats.Rows;

                if (dgvStatistics.Columns["IndicatorName"] != null) dgvStatistics.Columns["IndicatorName"].HeaderText = "Показатель";
                if (dgvStatistics.Columns["DisplayValue"] != null) dgvStatistics.Columns["DisplayValue"].HeaderText = "Значение";
                if (dgvStatistics.Columns["IndicatorName"] != null) dgvStatistics.Columns["IndicatorName"].Width = 300;

                lblStatus.Text = $"Статистика за период с {dtpStart.Value.ToShortDateString()} по {dtpEnd.Value.ToShortDateString()}";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки статистики: " + ex.Message);
                dgvStatistics.DataSource = null;
                lblStatus.Text = "Ошибка загрузки данных";
            }
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            LoadStatistics();
        }

        // --- КОД ДЛЯ WORD-ОТЧЕТА ---
        private void btnReport_Click(object sender, EventArgs e)
        {
            try
            {
                var stats = _orders.GetStatistics(dtpStart.Value, dtpEnd.Value);

                string fileName = $"Отчет_Джавахк_{DateTime.Now:dd_MM_yyyy_HHmm}.docx";
                string filePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName);

                using (var doc = DocX.Create(filePath))
                {
                    // Заголовок
                    var title = doc.InsertParagraph("Отчёт по статистике ресторана «Джавахк»");
                    title.FontSize(22);
                    title.Bold();
                    title.Alignment = Alignment.center;
                    title.SpacingAfter(10);

                    // Период
                    var period = doc.InsertParagraph($"Период: с {dtpStart.Value.ToShortDateString()} по {dtpEnd.Value.ToShortDateString()}");
                    period.FontSize(14);
                    period.Italic();
                    period.Alignment = Alignment.center;
                    period.SpacingAfter(30);

                    // Таблица
                    var table = doc.InsertTable(stats.Rows.Count + 1, 2);
                    table.Alignment = Alignment.center;

                    // Заголовки таблицы
                    table.Rows[0].Cells[0].Paragraphs[0].Append("Показатель").Bold().FontSize(12);
                    table.Rows[0].Cells[1].Paragraphs[0].Append("Значение").Bold().FontSize(12);

                    // ИСПОЛЬЗУЕМ Xceed.Drawing.Color для заливки
                    table.Rows[0].Cells[0].FillColor = Xceed.Drawing.Color.LightGray;
                    table.Rows[0].Cells[1].FillColor = Xceed.Drawing.Color.LightGray;

                    // Данные
                    for (int i = 0; i < stats.Rows.Count; i++)
                    {
                        table.Rows[i + 1].Cells[0].Paragraphs[0].Append(stats.Rows[i].IndicatorName).FontSize(12);
                        table.Rows[i + 1].Cells[1].Paragraphs[0].Append(stats.Rows[i].DisplayValue).FontSize(12);
                    }

                    // Подвал
                    var footer = doc.InsertParagraph($"\nДокумент сформирован: {DateTime.Now:dd.MM.yyyy HH:mm}");
                    footer.FontSize(10);
                    footer.Color(Xceed.Drawing.Color.Gray); // И здесь используем Xceed
                    footer.Alignment = Alignment.right;

                    doc.Save();
                }

                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(filePath) { UseShellExecute = true });

                MessageBox.Show($"Отчёт сохранён и открыт!\nПуть: {filePath}", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при создании отчёта: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
