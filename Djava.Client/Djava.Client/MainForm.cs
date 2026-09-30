using DjavaLib.Data;
using DjavaLib.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Djava.Client
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string login = txtLogin.Text;
            string password = txtPassword.Text;

            var validator = new AuthValidator();
            var result = validator.Validate(login, password);

            if (!result.IsValid)
            {
                MessageBox.Show(result.ErrorMessage, "Ошибка валидации");
                return;
            }

            try
            {
                string connStr = DbConfig.GetConnectionString();
                var repo = new UserPgRepository(connStr);
                bool ok = repo.Authenticate(login, password);

                if (ok)
                {
                    var user = repo.GetUserByLogin(login);
                    MessageBox.Show("Добро пожаловать, " + user.FullName +
                                    "!\nРоль: " + user.Role,
                        "Успешный вход");
                }
                else
                {
                    MessageBox.Show("Неверный логин или пароль", "Ошибка");
                    txtPassword.Clear();
                    txtPassword.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка подключения к БД: " + ex.Message, "Ошибка");
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Вы действительно хотите выйти?",
                "Подтверждение выхода",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }
    }
}
