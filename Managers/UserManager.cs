using GetOutAdminV2.Models;
using GetOutAdminV2.Providers;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace GetOutAdminV2.Managers
{
    public interface IUserManager
    {
        void GetAllUsers();
        User GetUserById(int id);
        User GetUserByEmail(string email);
        User GetUserByLogin(string email, string password);
        void AddUser(User user);
        void UpdateUser(User user);
        void DeleteUser(int id);
        User CurrentUser { get; set; }
        ObservableCollection<User> ListOfUsers { get; set; }
    }
    public class UserManager : IUserManager
    {
        readonly IUserProvider _userProvider;

        public User CurrentUser { get; set; }
        public ObservableCollection<User> ListOfUsers { get; set; }
        public UserManager(IUserProvider userProvider)
        {
            _userProvider = userProvider;
            ListOfUsers = new ObservableCollection<User>();
        }

        public void GetAllUsers()
        {
            var users = _userProvider.GetUsers();
            ListOfUsers.Clear();
            foreach (var user in users)
            {
                ListOfUsers.Add(user);
            }
        }

        public User GetUserById(int id)
        {
            return _userProvider.GetUserById(id);
        }

        public User GetUserByEmail(string email)
        {
            return _userProvider.GetUserByEmail(email);
        }

        public User GetUserByLogin(string email, string password)
        {
            return _userProvider.GetUserByLogin(email, password);
        }

        public void AddUser(User user)
        {
            _userProvider.AddUser(user);
        }

        public void UpdateUser(User user)
        {
            _userProvider.UpdateUser(user);
        }

        public void DeleteUser(int id)
        {
            _userProvider.DeleteUser(id);
        }
    }
}
