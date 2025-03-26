using GetOutAdminV2.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetOutAdminV2.Providers
{
    public interface IUserProvider
    {
        ObservableCollection<User> GetUsers();
        User GetUserById(long id);
        User GetUserByEmail(string email);
        User GetUserByLogin(string email, string password);
        void AddUser(User user);
        void UpdateUser(User user);
        void DeleteUser(long id);
    }

    public class UserProvider : IUserProvider
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public UserProvider(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public ObservableCollection<User> GetUsers()
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                var users = context.Users.ToList();
                return new ObservableCollection<User>(users);
            }
            catch (Exception ex)
            {
                // Log l'erreur ou gérez-la selon vos besoins
                throw new Exception("Erreur lors de la récupération des utilisateurs", ex);
            }
        }

        public User GetUserById(long id)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                return context.Users.FirstOrDefault(u => u.Id == id);
            }
            catch (Exception ex)
            {
                throw new Exception("Erreur lors de la récupération de l'utilisateur par ID", ex);
            }
        }

        public User GetUserByEmail(string email)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                return context.Users.FirstOrDefault(u => u.Email == email);
            }
            catch (Exception ex)
            {
                throw new Exception("Erreur lors de la récupération de l'utilisateur par email", ex);
            }
        }

        public User GetUserByLogin(string email, string password)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                return context.Users.FirstOrDefault(u => u.Email == email && u.Password == password);
            }
            catch (Exception ex)
            {
                throw new Exception("Erreur lors de la récupération de l'utilisateur par login", ex);
            }
        }

        public void AddUser(User user)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                context.Users.Add(user);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Erreur lors de l'ajout de l'utilisateur", ex);
            }
        }

        public void UpdateUser(User user)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                context.Attach(user).State = EntityState.Modified;
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Erreur lors de la mise à jour de l'utilisateur", ex);
            }
        }

        public void DeleteUser(long id)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                var userToDelete = context.Users.FirstOrDefault(u => u.Id == id);
                if (userToDelete != null)
                {
                    context.Users.Remove(userToDelete);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erreur lors de la suppression de l'utilisateur", ex);
            }
        }
    }
}
