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

        public OrdersForm(string adminName)
        {
            InitializeComponent();
            _adminName = adminName;

            var context = new DbContext(DbConfig.GetConnectionString());
            _orders = new OrderListRepository(context);

            lblAdmin.Text = "👤 " + adminName;

            LoadOrders();
            LoadStatuses();
        }

        private void LoadOrders()
        {
            try
            {
                var orders = _orders.GetOrders();

                dgvOrders.DataSource = null;
                dgvOrders.DataSource = orders;

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

                lblCount.Text = "Всего заказов: " + orders.Count;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки заказов: " + ex.Message,
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                dgvOrders.DataSource = null;
                lblCount.Text = "Всего заказов: 0";
            }
        }

        private void LoadStatuses()
        {
            cmbStatus.Items.Clear();
            cmbStatus.Items.Add("Новый");
            cmbStatus.Items.Add("Принят");
            cmbStatus.Items.Add("Готовится");
            cmbStatus.Items.Add("Готов");
            cmbStatus.Items.Add("Доставляется");
            cmbStatus.Items.Add("Выполнен");
            cmbStatus.Items.Add("Отменён");

            if (cmbStatus.Items.Count > 0)
                cmbStatus.SelectedIndex = 0;
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

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadOrders();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnChangeStatus_Click(object sender, EventArgs e)
        {
            if (dgvOrders.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите заказ из списка", "Ошибка",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cmbStatus.SelectedItem == null)
            {
                MessageBox.Show("Выберите новый статус заказа", "Ошибка",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                int orderId = Convert.ToInt32(dgvOrders.SelectedRows[0].Cells["OrderId"].Value);
                OrderStatus status = ParseStatus(cmbStatus.SelectedItem.ToString());

                if (_orders.UpdateOrderStatus(orderId, status))
                {
                    MessageBox.Show("Статус заказа обновлён", "Успех",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadOrders();
                }
                else
                {
                    MessageBox.Show("Не удалось обновить статус", "Ошибка",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message, "Ошибка",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
    }
}