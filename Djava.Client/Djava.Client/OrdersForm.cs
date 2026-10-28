using System;
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

        private string _sortColumn = "OrderDate";
        private bool _ascending = false;

        public OrdersForm(string adminName)
        {
            InitializeComponent();
            _adminName = adminName;

            var context = new DbContext(DbConfig.GetConnectionString());
            _orders = new OrderListRepository(context);
            _filter = new OrderFilter();
            _model = new OrderListModel();

            lblAdmin.Text = "👤 " + adminName;

            LoadStatusFilter();
            LoadOrders();
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
                dgvOrders.DataSource = result;

                if (dgvOrders.Columns["OrderId"] != null)
                    dgvOrders.Columns["OrderId"].HeaderText = "№";

                if (dgvOrders.Columns["OrderDate"] != null)
                {
                    dgvOrders.Columns["OrderDate"].HeaderText = "Дата";
                    dgvOrders.Columns["OrderDate"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
                }

                if (dgvOrders.Columns["ClientName"] != null)
                    dgvOrders.Columns["ClientName"].HeaderText = "Покупатель";

                if (dgvOrders.Columns["ClientContact"] != null)
                    dgvOrders.Columns["ClientContact"].HeaderText = "Контакты";

                if (dgvOrders.Columns["DeliveryMethod"] != null)
                    dgvOrders.Columns["DeliveryMethod"].HeaderText = "Способ получения";

                if (dgvOrders.Columns["Status"] != null)
                    dgvOrders.Columns["Status"].HeaderText = "Статус";

                if (dgvOrders.Columns["TotalAmount"] != null)
                {
                    dgvOrders.Columns["TotalAmount"].HeaderText = "Сумма";
                    dgvOrders.Columns["TotalAmount"].DefaultCellStyle.Format = "N2";
                }

                lblStatus.Text = "Всего заказов: " + result.Count;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки данных: " + ex.Message, "Ошибка",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                dgvOrders.DataSource = null;
                lblStatus.Text = "Ошибка загрузки данных";
            }
        }

        private void dgvOrders_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var row = dgvOrders.Rows[e.RowIndex];
            if (row.DataBoundItem == null) return;

            var order = row.DataBoundItem as Order;
            if (order == null) return;

            string columnName = dgvOrders.Columns[e.ColumnIndex].Name;

            if (columnName == "DeliveryMethod")
            {
                e.Value = GetDeliveryText(order.DeliveryMethod);
                e.FormattingApplied = true;
            }
            else if (columnName == "Status")
            {
                e.Value = GetStatusText(order.Status);
                e.FormattingApplied = true;
            }
        }

        private void cmbStatusFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            var item = cmbStatusFilter.SelectedItem as StatusFilterItem;
            if (item != null)
            {
                _filter.Status = item.Value;
                LoadOrders();
            }
        }

        private void btnResetFilter_Click(object sender, EventArgs e)
        {
            _filter.Reset();
            cmbStatusFilter.SelectedIndex = 0;
            _sortColumn = "OrderDate";
            _ascending = false;
            LoadOrders();
        }

        private void dgvOrders_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
            {
                string columnName = dgvOrders.Columns[e.ColumnIndex].Name;

                if (_sortColumn == columnName)
                {
                    if (!_ascending) _ascending = true;
                    else
                    {
                        _sortColumn = "OrderDate";
                        _ascending = false;
                    }
                }
                else
                {
                    _sortColumn = columnName;
                    _ascending = true;
                }

                LoadOrders();
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadOrders();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgvOrders_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            try
            {
                int orderId = Convert.ToInt32(dgvOrders.Rows[e.RowIndex].Cells["OrderId"].Value);
                var detailsForm = new OrderDetailsForm(_orders, orderId);
                detailsForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка открытия состава заказа: " + ex.Message,
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetDeliveryText(DeliveryType type)
        {
            switch (type)
            {
                case DeliveryType.Pickup: return "Самовывоз";
                case DeliveryType.Delivery: return "Доставка";
                default: return "";
            }
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

        private string _lastColumn = null;
        private int _clickCount = 0;

        private void dgvOrders_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string columnName = dgvOrders.Columns[e.ColumnIndex].Name;

            if (_lastColumn == columnName)
            {
                _clickCount++;
            }
            else
            {
                _lastColumn = columnName;
                _clickCount = 1;
            }

            switch (_clickCount)
            {
                case 1:
                    _sortColumn = columnName;
                    _ascending = true;
                    break;
                case 2:
                    _sortColumn = columnName;
                    _ascending = false;
                    break;
                default:
                    _sortColumn = "OrderDate";
                    _ascending = false;
                    _lastColumn = null;
                    _clickCount = 0;
                    break;
            }

            LoadOrders();
        }
    }
}