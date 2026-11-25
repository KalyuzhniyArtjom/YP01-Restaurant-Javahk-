using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DjavaLib.Data;
using DjavaLib.Models;

namespace Djava.Client
{
    public partial class OrderDetailsForm : Form
    {
        private readonly OrderListRepository _orders;
        private readonly int _orderId;
        private List<OrderItem> _currentItems;

        public OrderDetailsForm(OrderListRepository orders, int orderId)
        {
            InitializeComponent();
            _orders = orders;
            _orderId = orderId;

            lblTitle.Text = "Состав заказа №" + orderId;

            SetupGrid();
            LoadItems();
        }

        private void SetupGrid()
        {
            dgvItems.Columns.Clear();
            dgvItems.AutoGenerateColumns = false;
            dgvItems.AllowUserToAddRows = false;
            dgvItems.ReadOnly = false;

            dgvItems.Columns.Add(new DataGridViewTextBoxColumn { Name = "DishName", HeaderText = "Блюдо", ReadOnly = true, Width = 200 });
            dgvItems.Columns.Add(new DataGridViewTextBoxColumn { Name = "Quantity", HeaderText = "Количество", ReadOnly = false, Width = 100 });
            dgvItems.Columns.Add(new DataGridViewTextBoxColumn { Name = "Price", HeaderText = "Цена за ед.", ReadOnly = true, Width = 100 });
            dgvItems.Columns.Add(new DataGridViewTextBoxColumn { Name = "Sum", HeaderText = "Сумма", ReadOnly = true, Width = 100 });

            // ПОДПИСЫВАЕМСЯ НА СОБЫТИЕ ИЗМЕНЕНИЯ ЯЧЕЙКИ
            dgvItems.CellValueChanged += dgvItems_CellValueChanged;
        }

        private void LoadItems()
        {
            try
            {
                _currentItems = _orders.GetOrderItems(_orderId);
                RefreshGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки состава заказа: " + ex.Message,
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RefreshGrid()
        {
            dgvItems.Rows.Clear();

            foreach (var item in _currentItems)
            {
                dgvItems.Rows.Add(
                    item.DishName,
                    item.Quantity,
                    item.Price.ToString("N2"),
                    (item.Price * item.Quantity).ToString("N2")
                );
            }

            UpdateTotal();
        }

        // Пересчет общей суммы
        private void UpdateTotal()
        {
            decimal total = _currentItems.Sum(i => i.Price * i.Quantity);
            lblTotal.Text = $"Итого: {total:N2} ₽";
        }

        // СОБЫТИЕ: Когда меняется количество, пересчитываем сумму в строке и итог
        private void dgvItems_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvItems.Columns[e.ColumnIndex].Name == "Quantity")
            {
                var row = dgvItems.Rows[e.RowIndex];

                // Получаем цену из ячейки (там строка, поэтому парсим)
                if (decimal.TryParse(row.Cells["Price"].Value?.ToString(), out decimal price) &&
                    int.TryParse(row.Cells["Quantity"].Value?.ToString(), out int qty))
                {
                    // Обновляем сумму в этой строке
                    row.Cells["Sum"].Value = (price * qty).ToString("N2");

                    // Обновляем локальный список, чтобы итог считался правильно
                    if (e.RowIndex < _currentItems.Count)
                    {
                        _currentItems[e.RowIndex].Quantity = qty;
                    }

                    UpdateTotal();
                }
            }
        }

        // --- КНОПКА "ДОБАВИТЬ БЛЮДО" ---
        private void btnAddDish_Click(object sender, EventArgs e)
        {
            var availableDishes = _orders.GetAvailableDishes();

            if (availableDishes.Count == 0)
            {
                MessageBox.Show("Нет доступных блюд в меню.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var dialog = new Form())
            {
                dialog.Text = "Добавить блюдо";
                dialog.Size = new Size(300, 180);
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                dialog.MaximizeBox = false;
                dialog.MinimizeBox = false;

                var cmbDishes = new ComboBox { Left = 20, Top = 20, Width = 240, DropDownStyle = ComboBoxStyle.DropDownList };
                cmbDishes.DataSource = availableDishes;
                cmbDishes.DisplayMember = "Name";
                cmbDishes.ValueMember = "Id";

                var numQty = new NumericUpDown { Left = 20, Top = 60, Width = 100, Minimum = 1, Maximum = 100, Value = 1 };

                var btnOk = new Button { Text = "Добавить", Left = 20, Top = 100, Width = 100, DialogResult = DialogResult.OK };
                var btnCancel = new Button { Text = "Отмена", Left = 140, Top = 100, Width = 100, DialogResult = DialogResult.Cancel };

                dialog.Controls.Add(cmbDishes);
                dialog.Controls.Add(numQty);
                dialog.Controls.Add(btnOk);
                dialog.Controls.Add(btnCancel);
                dialog.AcceptButton = btnOk;
                dialog.CancelButton = btnCancel;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    var selectedDish = (Dish)cmbDishes.SelectedItem;
                    int qty = (int)numQty.Value;

                    var existingItem = _currentItems.FirstOrDefault(i => i.DishId == selectedDish.Id);
                    if (existingItem != null)
                    {
                        existingItem.Quantity += qty;
                    }
                    else
                    {
                        _currentItems.Add(new OrderItem
                        {
                            DishId = selectedDish.Id,
                            DishName = selectedDish.Name,
                            Price = selectedDish.Price,
                            Quantity = qty
                        });
                    }
                    RefreshGrid();
                }
            }
        }

        // --- КНОПКА "УДАЛИТЬ БЛЮДО" ---
        private void btnRemoveDish_Click(object sender, EventArgs e)
        {
            if (dgvItems.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите блюдо для удаления.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int rowIndex = dgvItems.SelectedRows[0].Index;
            if (rowIndex >= 0 && rowIndex < _currentItems.Count)
            {
                _currentItems.RemoveAt(rowIndex);
                RefreshGrid();
            }
        }

        // --- КНОПКА "СОХРАНИТЬ" ---
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // Синхронизируем данные из таблицы перед сохранением
                for (int i = 0; i < dgvItems.Rows.Count; i++)
                {
                    var qtyCell = dgvItems.Rows[i].Cells["Quantity"].Value;
                    if (qtyCell != null && int.TryParse(qtyCell.ToString(), out int newQty) && newQty > 0)
                    {
                        _currentItems[i].Quantity = newQty;
                    }
                    else
                    {
                        MessageBox.Show($"Некорректное количество в строке {i + 1}.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                decimal totalAmount = _currentItems.Sum(item => item.Price * item.Quantity);

                if (_orders.UpdateOrderItems(_orderId, _currentItems, totalAmount))
                {
                    MessageBox.Show("Состав заказа успешно обновлен!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Не удалось сохранить изменения.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}