using System.Collections.Generic;

namespace SmartWarehouse
{
    public class Admin : User
    {
        public Admin(string login, string password)
            : base(login, password, UserRole.Admin) { }

        public void CreateUser(List<User> userList, User newUser) => userList.Add(newUser);
        public void DeleteUser(List<User> userList, User user) => userList.Remove(user);
    }
}