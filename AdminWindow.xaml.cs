using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;

namespace MilkApp
{
    public partial class AdminWindow : Window
    {
        private readonly MilkEntities db = new MilkEntities();
        public ObservableCollection<User> Users { get; set; } = new ObservableCollection<User>();
        public ObservableCollection<Role> Roles { get; set; } = new ObservableCollection<Role>();

        public AdminWindow()
        {
            InitializeComponent();
            LoadData();
            DataContext = this;
        }

        private void LoadData()
        {
            db.User.Load();
            db.Role.Load();
            Users = db.User.Local;
            Roles = db.Role.Local;
            UserGrid.ItemsSource = Users;
        }

        private void AddUser_Click(object sender, RoutedEventArgs e)
        {
            var defaultRole = Roles.FirstOrDefault(r => r.Name == "Пользователь");
            if (defaultRole == null)
            {
                MessageBox.Show("Роль 'Пользователь' не найдена. Добавьте роли в БД.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var newUser = new User
            {
                Login = "new_user",
                Password = "123",
                Role_Id = 2,
                IsBlocked = false
            };
            db.User.Add(newUser);
            Users.Add(newUser);
            MessageBox.Show("Новый пользователь добавлен. Сохраните изменения.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ToggleBlock_Click(object sender, RoutedEventArgs e)
        {
            if (UserGrid.SelectedItem is User selectedUser)
            {
                selectedUser.IsBlocked = !selectedUser.IsBlocked;
                UserGrid.Items.Refresh();
                MessageBox.Show($"Пользователь {selectedUser.Login} теперь {(selectedUser.IsBlocked ? "заблокирован" : "разблокирован")}. Сохраните изменения.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Выберите пользователя из таблицы.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                db.SaveChanges();
                MessageBox.Show("Изменения сохранены успешно.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch
            {
                MessageBox.Show("Ошибка сохранения. Проверьте данные (логин уникален, поля заполнены).", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}