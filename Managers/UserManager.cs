using GetOutAdminV2.Models;

namespace GetOutAdminV2.Managers
{
    public interface IUserManager
    {
        List<User> Users { get; set; }
        User CurrentUser { get; set; }
    }
    public class UserManager : IUserManager
    {
        public List<User> Users { get; set; }
        public User CurrentUser { get; set; }
    }
}
