using System.Windows;

namespace SmartWarehouse
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string login = txtLogin.Text;
            string password = txtPassword.Password;

            Admin testAdmin = new Admin("admin", "admin123");
            User testUser = new User("user", "user123", UserRole.User);

            User foundUser = null;

            if (login == testAdmin.Login && testAdmin.Authenticate(password))
            {
                foundUser = testAdmin;
            }
            else if (login == testUser.Login && testUser.Authenticate(password))
            {
                foundUser = testUser;
            }

            if (foundUser != null)
            {
                MainWindow main = new MainWindow();
                main.CurrentUser = foundUser;
                main.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Невірний логін або пароль! Спробуйте admin/admin123 або user/user123");
            }
        }
    }
}