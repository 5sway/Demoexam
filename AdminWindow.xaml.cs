using System.Data.Entity;
using System.Linq;
using System.Windows;

namespace TestApp
{
    public partial class AdminWindow : Window
    {
        private readonly TestEntities db = new TestEntities();

        public AdminWindow()
        {
            InitializeComponent();
            LoadUsers();
        }

        private void LoadUsers()
        {
            db.User.Load();
            dgUsers.ItemsSource = db.User.Local.ToBindingList();
        }

        private void AddUser_Click(object sender, RoutedEventArgs e)
        {
            var newUser = new User
            {
                Login = "new_user",
                Password = "123",
                Role_Id = 2,
                IsBlocked = false
            };

            db.User.Add(newUser);
            MessageBox.Show("Новый пользователь добавлен!\nОтредактируйте данные и нажмите Сохранить");
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            db.SaveChanges();
            MessageBox.Show("Изменения сохранены", "Успех");
        }
    }
}