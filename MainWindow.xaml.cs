using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TestApp;

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

        private void Captcha_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Image img && int.TryParse(img.Tag?.ToString(), out int num))
            {
                if (!captchaOrder.Contains(num))
                {
                    captchaOrder.Add(num);
                    txtCaptcha.Text = $"Выбранный порядок: {string.Join(" → ", captchaOrder)}";
                    img.Opacity = 0.55;
                }
            }
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            if (captchaOrder.Count != 4 || !captchaOrder.SequenceEqual(new[] { 1, 2, 3, 4 }))
            {
                MessageBox.Show("Неверный порядок капчи!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                ResetCaptcha();
                return;
            }

            string login = LoginTextBox.Text.Trim();
            string pass = PasswordBox.Password;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(pass))
            {
                MessageBox.Show("Введите логин и пароль");
                return;
            }

            var user = db.User.FirstOrDefault(u => u.Login == login && u.Password == pass);

            if (user == null)
            {
                MessageBox.Show("Неверный логин или пароль");
                ResetCaptcha();
                return;
            }

            if (user.IsBlocked == true)
            {
                MessageBox.Show("Аккаунт заблокирован!", "Доступ запрещён");
                ResetCaptcha();
                return;
            }

            MessageBox.Show($"Добро пожаловать, {user.Login}!");

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

            var grid = VisualTreeHelper.GetChild(this.Content as DependencyObject, 0) as Grid;
        }
    }
}