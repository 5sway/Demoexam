using MilkApp.Properties;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
namespace MilkApp
{
    public partial class MainWindow : Window
    {
        private readonly MilkEntities db = new MilkEntities();
        private readonly List<int> captchaOrder = new List<int>();

        public MainWindow()
        {
            InitializeComponent();
            InitializeDatabaseIfEmpty();
        }

        private void InitializeDatabaseIfEmpty()
        {
            if (!db.Role.Any())
            {
                db.Role.Add(new Role { Name = "Пользователь" });
                db.Role.Add(new Role { Name = "Администратор" });
                db.SaveChanges();
            }

            if (!db.User.Any())
            {
                var userRole = db.Role.First(r => r.Name == "Пользователь");
                var adminRole = db.Role.First(r => r.Name == "Администратор");

                db.User.Add(new User { Login = "user", Password = "123", Role_Id = 2, IsBlocked = false });
                db.User.Add(new User { Login = "admin", Password = "123", Role_Id = 1, IsBlocked = false });
                db.SaveChanges();
            }
        }

        private void CaptchaImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is System.Windows.Controls.Image img)
            {
                if (int.TryParse(img.Tag?.ToString(), out int number))
                {
                    if (!captchaOrder.Contains(number))
                    {
                        captchaOrder.Add(number);
                        txtCaptchaStatus.Text = $"Выбранный порядок: {string.Join(" → ", captchaOrder)}";

                        // Визуальная обратная связь (опционально, но очень помогает)
                        img.Opacity = 0.7;
                    }
                }
            }
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            if (captchaOrder.Count != 4 || !captchaOrder.SequenceEqual(new[] { 1, 2, 3, 4 }))
            {
                MessageBox.Show("Неверный порядок капчи. Попробуйте снова.", "Ошибка капчи", MessageBoxButton.OK, MessageBoxImage.Warning);
                captchaOrder.Clear();
                CaptchaStatusText.Text = "Выбранный порядок: (пусто)";
                return;
            }

            string login = LoginTextBox.Text.Trim();
            string password = PasswordBox.Password;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Заполните все поля. Логин и пароль не могут быть пустыми.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (login.Length < 3)
            {
                MessageBox.Show("Логин должен быть минимум 3 символа. Исправьте и попробуйте снова.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var user = db.User.FirstOrDefault(u => u.Login == login && u.Password == password);

                if (user == null)
                {
                    MessageBox.Show("Неверный логин или пароль. Проверьте данные и попробуйте снова.", "Ошибка авторизации", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (user.IsBlocked)
                {
                    MessageBox.Show("Ваш аккаунт заблокирован. Обратитесь к администратору.", "Доступ запрещен", MessageBoxButton.OK, MessageBoxImage.Stop);
                    return;
                }

                MessageBox.Show("Авторизация успешна!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                // Разграничение прав через switch (первый вариант)
                switch (user.Role.Name)
                {
                    case "Администратор":
                        new AdminWindow().Show();
                        Close();
                        break;
                    case "Пользователь":
                        new UserWindow().Show();
                        Close();
                        break;
                    default:
                        MessageBox.Show("Неизвестная роль. Обратитесь к администратору.", "Ошибка роли", MessageBoxButton.OK, MessageBoxImage.Error);
                        break;
                }
            }
            catch
            {
                MessageBox.Show("Ошибка подключения к базе данных. Проверьте соединение и попробуйте снова.", "Критическая ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            captchaOrder.Clear();
            CaptchaStatusText.Text = "Выбранный порядок: (пусто)";
        }
    }
}