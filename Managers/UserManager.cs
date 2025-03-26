using GetOutAdminV2.Models;
using GetOutAdminV2.Providers;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace GetOutAdminV2.Managers
{
    public interface IUserManager
    {
        void GetAllUsers();
        User GetUserById(long id);
        User GetUserByEmail(string email);
        User GetUserByLogin(string email, string password);
        void AddUser(User user);
        void UpdateUser(User user);
        void DeleteUser(long id);
        User CurrentUser { get; set; }
        ObservableCollection<User> ListOfUsers { get; set; }

        event Action UsersUpdated;
    }
    public class UserManager : IUserManager
    {
        readonly IUserProvider _userProvider;
        public event Action UsersUpdated;
        public User CurrentUser { get; set; }
        public ObservableCollection<User> ListOfUsers { get; set; }
        public UserManager(IUserProvider userProvider)
        {
            _userProvider = userProvider;
            ListOfUsers = new ObservableCollection<User>();
        }

        public void GetAllUsers()
        {
            var users = _userProvider.GetUsers().OrderBy(u => u.Id);
            ListOfUsers.Clear();
            foreach (var user in users)
            {
                ListOfUsers.Add(user);
            }
            UsersUpdated?.Invoke();
        }

        public User GetUserById(long id)
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
            UsersUpdated?.Invoke();
        }

        public void UpdateUser(User user)
        {
            _userProvider.UpdateUser(user);
            UsersUpdated?.Invoke();
        }

        public void DeleteUser(long id)
        {
            _userProvider.DeleteUser(id);
            var userToRemove = ListOfUsers.FirstOrDefault(_ => _.Id == id);
            if (userToRemove != null)
            {
                ListOfUsers.Remove(userToRemove);
            }
        }
    }
}
