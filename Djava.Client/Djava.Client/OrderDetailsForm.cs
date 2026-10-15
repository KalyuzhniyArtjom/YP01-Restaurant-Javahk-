using System;
using System.Linq;
using System.Windows.Forms;
using DjavaLib.Data;

namespace Djava.Client
{
    public partial class OrderDetailsForm : Form
    {
        private readonly OrderListRepository _orders;
        private readonly int _orderId;

        public OrderDetailsForm(OrderListRepository orders, int orderId)
        {
            InitializeComponent();
            _orders = orders;
            _orderId = orderId;

            lblTitle.Text = "Состав заказа №" + orderId;
            LoadItems();
        }

        private void LoadItems()
        {
            try
            {
                var items = _orders.GetOrderItems(_orderId);

                dgvItems.DataSource = null;
                dgvItems.DataSource = items;

                // Настройка колонок
                if (dgvItems.Columns["DishId"] != null)
                    dgvItems.Columns["DishId"].Visible = false;

                if (dgvItems.Columns["DishName"] != null)
                    dgvItems.Columns["DishName"].HeaderText = "Блюдо";

                if (dgvItems.Columns["Quantity"] != null)
                    dgvItems.Columns["Quantity"].HeaderText = "Количество";

                if (dgvItems.Columns["Price"] != null)
                {
                    dgvItems.Columns["Price"].HeaderText = "Цена за ед.";
                    dgvItems.Columns["Price"].DefaultCellStyle.Format = "N2";
                }

                if (dgvItems.Columns["Sum"] != null)
                {
                    dgvItems.Columns["Sum"].HeaderText = "Сумма";
                    dgvItems.Columns["Sum"].DefaultCellStyle.Format = "N2";
                }

                decimal total = items != null ? items.Sum(x => x.Sum) : 0;
                lblTotal.Text = "Итого: " + total.ToString("N2") + " ₽";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки состава заказа: " + ex.Message,
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}