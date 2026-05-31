using System;
using System.Collections.Generic;
using System.Data.Entity;
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
using System.Windows.Shapes;

namespace DemoExam
{
    /// <summary>
    /// Панель администратора для управления пользователями системы.
    /// </summary>
    public partial class AdminWindow : Window
    {
        private readonly TestEntities db = new TestEntities();

        /// <summary>
        /// Конструктор окна администратора.
        /// Загружает данные пользователей при открытии окна.
        /// </summary>
        public AdminWindow()
        {
            InitializeComponent();
            LoadData();
        }

        /// <summary>
        /// Загружает список всех пользователей из базы данных 
        /// и привязывает его к DataGrid.
        /// </summary>
        private void LoadData()
        {
            db.User.Load();
            dgUsers.ItemsSource = db.User.Local.ToBindingList();
        }

        /// <summary>
        /// Сохраняет все изменения (добавление, редактирование, удаление) 
        /// в базу данных.
        /// </summary>
        private void Savebtn_Click(object sender, RoutedEventArgs e)
        {
            db.SaveChanges();
        }

        /// <summary>
        /// Обработчик выхода из панели администратора.
        /// Возвращает пользователя на форму авторизации.
        /// </summary>
        private void Exitbtn_Click(object sender, RoutedEventArgs e)
        {
            new LoginWindow().Show();
            Close();
        }

        /// <summary>
        /// Добавляет нового пользователя с предустановленными значениями.
        /// </summary>
        private void Addbtn_Click(object sender, RoutedEventArgs e)
        {
            var newUser = new User
            {
                Login = "NewUser",
                Password = "123",
                Role_Id = 2,
                IsBlocked = false
            };
            db.User.Add(newUser);
            db.SaveChanges();
        }
    }
}
