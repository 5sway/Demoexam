using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace TestApp
{
    public partial class MainWindow : Window
    {
        private readonly TestEntities db = new TestEntities();
        private readonly List<int> captchaOrder = new List<int>();

        public MainWindow()
        {
            InitializeComponent();
        }

        // ==================== КАПЧА ====================
        private void Captcha1_Click(object sender, System.Windows.Input.MouseButtonEventArgs e) => AddToCaptcha(1);
        private void Captcha2_Click(object sender, System.Windows.Input.MouseButtonEventArgs e) => AddToCaptcha(2);
        private void Captcha3_Click(object sender, System.Windows.Input.MouseButtonEventArgs e) => AddToCaptcha(3);
        private void Captcha4_Click(object sender, System.Windows.Input.MouseButtonEventArgs e) => AddToCaptcha(4);

        private void AddToCaptcha(int number)
        {
            if (!captchaOrder.Contains(number))
            {
                captchaOrder.Add(number);
                txtCaptcha.Text = $"Выбранный порядок: {string.Join(" → ", captchaOrder)}";
            }
        }

        // ==================== АВТОРИЗАЦИЯ ====================
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            if (captchaOrder.Count != 4 || !captchaOrder.SequenceEqual(new[] { 1, 2, 3, 4 }))
            {
                MessageBox.Show("Неверный порядок капчи!\nКликните по картинкам строго 1 → 2 → 3 → 4",
                                "Ошибка капчи", MessageBoxButton.OK, MessageBoxImage.Warning);
                ResetCaptcha();
                return;
            }

            string login = LoginTextBox.Text.Trim();
            string password = PasswordBox.Password;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Введите логин и пароль", "Ошибка");
                return;
            }

            var user = db.User.FirstOrDefault(u => u.Login == login && u.Password == password);

            if (user == null)
            {
                MessageBox.Show("Неверный логин или пароль", "Ошибка авторизации");
                ResetCaptcha();
                return;
            }

            if (user.IsBlocked == true)
            {
                MessageBox.Show("Аккаунт заблокирован!", "Доступ запрещён");
                ResetCaptcha();
                return;
            }

            MessageBox.Show($"Добро пожаловать, {user.Login}!", "Успех");

            if (user.Role?.Name == "Администратор")
                new AdminWindow().Show();
            else
                new UserWindow().Show();

            Close();
        }

        private void ResetCaptcha()
        {
            captchaOrder.Clear();
            txtCaptcha.Text = "Выбранный порядок: —";
        }
    }
}