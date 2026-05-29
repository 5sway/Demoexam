using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure.Interception;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DemoExam
{
    /// <summary>
    /// Главное окно приложения. Реализует авторизацию с графической капчей.
    /// </summary>
    public partial class LoginWindow : Window
    {
        private readonly TestEntities db = new TestEntities();
        private readonly List<int> captchaorder = new List<int>();
        public LoginWindow()
        {
            InitializeComponent();
            ResetCaptcha();
        }

        private void CaptchaImg2_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            AddToCaptcha(2);
        }

        private void CaptchaImg1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            AddToCaptcha(1);
        }

        private void CaptchaImg3_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            AddToCaptcha(3);
        }

        private void CaptchaImg4_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            AddToCaptcha(4);
        }

        /// <summary>
        /// Сбрасывает последовательность капчи.
        /// </summary>
        private void ResetCaptcha()
        {
            captchaorder.Clear();
            CaptchaTxt.Text = "Выбранный порядок капчи:- ";
        }

        /// <summary>
        /// Добавляет номер капчи в последовательность, если его ещё нет.
        /// </summary>
        private void AddToCaptcha(int number)
        {
            if (!captchaorder.Contains(number))
            {
                captchaorder.Add(number);
                CaptchaTxt.Text = $"Выбранный порядок капчи:- {String.Join(", ", captchaorder)}";
            }
        }

        /// <summary>
        /// Обработчик нажатия кнопки "Войти".
        /// Выполняет полную проверку авторизации и открывает нужное окно.
        /// </summary>
        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            if (captchaorder.Count != 4 || !captchaorder.SequenceEqual(new[] { 1, 2, 3, 4 }))
            {
                MessageBox.Show("Неверный порядок капчи!");
                ResetCaptcha();
                return;
            }
            
            var login = LoginTxtBox.Text.Trim();
            
            var password = PasswordTxtBox.Password;
            
            
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Введите логин или пароль!");
                ResetCaptcha();
                return;
            }

            var user = db.User.FirstOrDefault(u => u.Login == login && u.Password == password);

            if (user == null)
            {
                MessageBox.Show("Неверный логин или пароль!");
                ResetCaptcha();
                return;
            }
            
            if (user.IsBlocked == true)
            {
                MessageBox.Show("Ваша учетная запись заблокирована!");
                ResetCaptcha();
                return;
            }

            // Открытие соответствующего окна в зависимости от роли
            if (user.Role?.Name == "Администратор")
                new AdminWindow().Show();
            else
                new UserWindow().Show();
            
            Close();
        }
    }
}
