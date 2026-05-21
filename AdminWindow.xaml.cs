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
            dgUsers.Items.Refresh();
            MessageBox.Show("Новый пользователь добавлен.\nОтредактируйте данные и нажмите Сохранить.");
        }

        private void ToggleBlock_Click(object sender, RoutedEventArgs e)
        {
            if (dgUsers.SelectedItem is User user)
            {
                user.IsBlocked = !user.IsBlocked;
                dgUsers.Items.Refresh();
            }
            else
            {
                MessageBox.Show("Выберите пользователя в таблице");
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                db.SaveChanges();
                MessageBox.Show("Изменения успешно сохранены в базу данных!", "Успех");
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                MessageBox.Show("Ошибка валидации данных:\n" + ex.Message, "Ошибка");
            }
            catch
            {
                MessageBox.Show("Ошибка сохранения.\nВозможно, логин уже занят или данные некорректны.", "Ошибка");
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            db.User.Load();
            dgUsers.ItemsSource = db.User.Local.ToBindingList();
            MessageBox.Show("Таблица обновлена");
        }
    }
}