using DjavaLib.Data;
using DjavaLib.Validation;
using System;
using System.Windows.Forms;

namespace Djava.Client
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            this.Load += MainForm_Load;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Пароль скрыт при запуске
            txtPassword.UseSystemPasswordChar = true;
            btnTogglePassword.Text = "👁";
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

        private void btnTogglePassword_Click(object sender, EventArgs e)
        {
            if (txtPassword.UseSystemPasswordChar)
            {
                txtPassword.UseSystemPasswordChar = false;
                btnTogglePassword.Text = "🙈";
            }
            else
            {
                txtPassword.UseSystemPasswordChar = true;
                btnTogglePassword.Text = "👁";
            }
        }
    }
}