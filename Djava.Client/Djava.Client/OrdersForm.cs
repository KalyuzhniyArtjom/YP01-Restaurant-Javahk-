using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DjavaLib.Data;
using DjavaLib.Models;

namespace Djava.Client
{
    public partial class OrdersForm : Form
    {
        private readonly OrderListRepository _orders;
        private readonly string _adminName;
        private readonly OrderFilter _filter;
        private readonly OrderListModel _model;
        private readonly DbContext _dbContext;
        private StatisticsForm _statsForm;
        private string _sortColumn = "OrderId";
        private bool _ascending = true;

        public OrdersForm(string adminName)
        {
            InitializeComponent();
            _adminName = adminName;
            _dbContext = new DbContext(DbConfig.GetConnectionString());
            _orders = new OrderListRepository(_dbContext);
            _filter = new OrderFilter();
            _model = new OrderListModel();
            this.dgvOrders.DataError += new DataGridViewDataErrorEventHandler(this.dgvOrders_DataError);
            LoadStatusFilter();
            LoadOrders();
        }

        private void lblStatisticsLink_Click(object sender, EventArgs e)
        {
            if (_statsForm != null && !_statsForm.IsDisposed) { _statsForm.Activate(); return; }
            _statsForm = new StatisticsForm(_dbContext);
            lblStatisticsLink.ForeColor = Color.White;
            lblOrdersLink.ForeColor = Color.Gray;
            _statsForm.ShowDialog();
            _statsForm = null;
            lblOrdersLink.ForeColor = Color.White;
            lblStatisticsLink.ForeColor = Color.Gray;
        }

        private void lblOrdersLink_Click(object sender, EventArgs e)
        {
            if (_statsForm != null && !_statsForm.IsDisposed) { _statsForm.Close(); _statsForm = null; }
            lblOrdersLink.ForeColor = Color.White;
            lblStatisticsLink.ForeColor = Color.Gray;
        }

        private void LoadStatusFilter()
        {
            cmbStatusFilter.Items.Clear();
            cmbStatusFilter.Items.Add(new StatusFilterItem("(все)", null));
            cmbStatusFilter.Items.Add(new StatusFilterItem("Новый", OrderStatus.New));
            cmbStatusFilter.Items.Add(new StatusFilterItem("Принят", OrderStatus.Accepted));
            cmbStatusFilter.Items.Add(new StatusFilterItem("Готовится", OrderStatus.Cooking));
            cmbStatusFilter.Items.Add(new StatusFilterItem("Готов", OrderStatus.Ready));
            cmbStatusFilter.Items.Add(new StatusFilterItem("Доставляется", OrderStatus.Delivering));
            cmbStatusFilter.Items.Add(new StatusFilterItem("Выполнен", OrderStatus.Completed));
            cmbStatusFilter.Items.Add(new StatusFilterItem("Отменён", OrderStatus.Cancelled));
            cmbStatusFilter.DisplayMember = "DisplayName";
            cmbStatusFilter.SelectedIndex = 0;
            cmbStatusFilter.SelectedIndexChanged += cmbStatusFilter_SelectedIndexChanged;
        }

        private void LoadOrders()
        {
            try
            {
                _model.Orders = _orders.GetOrders(_filter);
                var result = _model.ApplyFilterAndSort(_filter, _sortColumn, _ascending);

                dgvOrders.DataSource = null;
                dgvOrders.Columns.Clear();
                dgvOrders.AutoGenerateColumns = false;
                dgvOrders.AllowUserToAddRows = false;
                dgvOrders.ReadOnly = false;

                dgvOrders.Columns.Add(new DataGridViewTextBoxColumn { Name = "OrderId", HeaderText = "№", DataPropertyName = "OrderId", ReadOnly = true });
                dgvOrders.Columns.Add(new DataGridViewTextBoxColumn { Name = "OrderDate", HeaderText = "Дата", DataPropertyName = "OrderDate", ReadOnly = true, DefaultCellStyle = new DataGridViewCellStyle { Format = "dd.MM.yyyy HH:mm" } });
                dgvOrders.Columns.Add(new DataGridViewTextBoxColumn { Name = "ClientName", HeaderText = "Покупатель", DataPropertyName = "ClientName", ReadOnly = false });
                dgvOrders.Columns.Add(new DataGridViewTextBoxColumn { Name = "ClientContact", HeaderText = "Контакты", DataPropertyName = "ClientContact", ReadOnly = false });

                var deliveryColumn = new DataGridViewComboBoxColumn
                {
                    Name = "DeliveryMethod",
                    HeaderText = "Способ получения",
                    ReadOnly = false,
                    DataSource = new List<string> { "Доставка", "Самовывоз" },
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton
                };
                dgvOrders.Columns.Add(deliveryColumn);

                var statusColumn = new DataGridViewComboBoxColumn
                {
                    Name = "Status",
                    HeaderText = "Статус",
                    ReadOnly = false,
                    DataSource = new List<string> { "Новый", "Принят", "Готовится", "Готов", "Доставляется", "Выполнен", "Отменён" },
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton
                };
                dgvOrders.Columns.Add(statusColumn);

                dgvOrders.Columns.Add(new DataGridViewTextBoxColumn { Name = "TotalAmount", HeaderText = "Сумма", DataPropertyName = "TotalAmount", ReadOnly = true, DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" } });

                dgvOrders.DataSource = result;

                for (int i = 0; i < dgvOrders.Rows.Count; i++)
                {
                    if (i < result.Count)
                    {
                        dgvOrders.Rows[i].Cells["DeliveryMethod"].Value = result[i].DeliveryMethod == DeliveryType.Delivery ? "Доставка" : "Самовывоз";
                        dgvOrders.Rows[i].Cells["Status"].Value = GetStatusText(result[i].Status);
                    }
                }
                lblStatus.Text = "Всего заказов: " + result.Count;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки данных: " + ex.Message);
            }
        }

        private void dgvOrders_DataError(object sender, DataGridViewDataErrorEventArgs e) { e.ThrowException = false; }

        private void cmbStatusFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            var item = cmbStatusFilter.SelectedItem as StatusFilterItem;
            if (item != null) { _filter.Status = item.Value; LoadOrders(); }
        }

        private void btnResetFilter_Click(object sender, EventArgs e)
        {
            _filter.Reset(); cmbStatusFilter.SelectedIndex = 0; _sortColumn = "OrderId"; _ascending = true; LoadOrders();
        }

        private void dgvOrders_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
            {
                string columnName = dgvOrders.Columns[e.ColumnIndex].Name;
                if (_sortColumn == columnName) { if (!_ascending) _ascending = true; else { _sortColumn = "OrderId"; _ascending = true; } }
                else { _sortColumn = columnName; _ascending = true; }
                LoadOrders();
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e) { LoadOrders(); }
        private void btnLogout_Click(object sender, EventArgs e) { this.Close(); }

        private void dgvOrders_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            try
            {
                int orderId = Convert.ToInt32(dgvOrders.Rows[e.RowIndex].Cells["OrderId"].Value);
                var detailsForm = new OrderDetailsForm(_orders, orderId);
                detailsForm.ShowDialog();
                LoadOrders();
            }
            catch (Exception ex) { MessageBox.Show("Ошибка: " + ex.Message); }
        }

        private string GetStatusText(OrderStatus status)
        {
            switch (status)
            {
                case OrderStatus.New: return "Новый";
                case OrderStatus.Accepted: return "Принят";
                case OrderStatus.Cooking: return "Готовится";
                case OrderStatus.Ready: return "Готов";
                case OrderStatus.Delivering: return "Доставляется";
                case OrderStatus.Completed: return "Выполнен";
                case OrderStatus.Cancelled: return "Отменён";
                default: return "";
            }
        }

        private OrderStatus ParseStatus(string value)
        {
            switch (value)
            {
                case "Новый": return OrderStatus.New;
                case "Принят": return OrderStatus.Accepted;
                case "Готовится": return OrderStatus.Cooking;
                case "Готов": return OrderStatus.Ready;
                case "Доставляется": return OrderStatus.Delivering;
                case "Выполнен": return OrderStatus.Completed;
                case "Отменён": return OrderStatus.Cancelled;
                default: return OrderStatus.New;
            }
        }

        private DeliveryType ParseDelivery(string value) => value == "Доставка" ? DeliveryType.Delivery : DeliveryType.Pickup;

        private string _lastColumn = null;
        private int _clickCount = 0;

        private void dgvOrders_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string columnName = dgvOrders.Columns[e.ColumnIndex].Name;
            if (_lastColumn == columnName) _clickCount++; else { _lastColumn = columnName; _clickCount = 1; }
            switch (_clickCount)
            {
                case 1: _sortColumn = columnName; _ascending = true; break;
                case 2: _sortColumn = columnName; _ascending = false; break;
                default: _sortColumn = "OrderId"; _ascending = true; _lastColumn = null; _clickCount = 0; break;
            }
            LoadOrders();
        }

        private void btnSaveChanges_Click(object sender, EventArgs e)
        {
            if (dgvOrders.Rows.Count == 0) return;

            dgvOrders.EndEdit();

            int updatedCount = 0;
            try
            {
                foreach (DataGridViewRow row in dgvOrders.Rows)
                {
                    if (row.IsNewRow) continue;

                    int orderId = Convert.ToInt32(row.Cells["OrderId"].Value);
                    string name = row.Cells["ClientName"].Value?.ToString() ?? "";
                    string phone = row.Cells["ClientContact"].Value?.ToString() ?? "";

                    string deliveryStr = row.Cells["DeliveryMethod"].Value?.ToString() ?? "Самовывоз";
                    DeliveryType delivery = ParseDelivery(deliveryStr);

                    string statusStr = row.Cells["Status"].Value?.ToString() ?? "Новый";
                    OrderStatus status = ParseStatus(statusStr);

                    if (_orders.UpdateOrderDetails(orderId, name, phone, delivery, status))
                    {
                        updatedCount++;
                    }
                }

                MessageBox.Show($"Успешно обновлено: {updatedCount} заказов!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadOrders();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
