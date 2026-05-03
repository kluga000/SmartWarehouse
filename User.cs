using System;
using System.Security.Cryptography;
using System.Text;

namespace SmartWarehouse
{
    // Представляє користувача системи з механізмом безпечної автентифікації
    public class User
    {
        public string Login { get; set; }
        public string PasswordHash { get; private set; }
        public UserRole Role { get; set; } 

        public User(string login, string password, UserRole role)
        {
            Login = login;
            PasswordHash = HashPassword(password);
            Role = role;
        }

        // Перевіряє відповідність введеного пароля збереженому хешу
        public bool Authenticate(string password)
        {
            return PasswordHash == HashPassword(password);
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }
    }
}