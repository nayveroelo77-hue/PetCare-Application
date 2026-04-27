using PetCare.Model;

namespace PetCare.Service
{
    public class AuthService
    {
        private const string UserEmailKey = "UserEmail";
        private const string UserNameKey = "UserName";
        private const string UserRoleKey = "UserRole";
        private const string UserIdKey = "UserId";
        private const string IsLoggedInKey = "IsLoggedIn";

        public void SaveSession(UserAccount user)
        {
            Preferences.Default.Set(UserEmailKey, user.Email);
            Preferences.Default.Set(UserNameKey, user.FullName);
            Preferences.Default.Set(UserRoleKey, user.Role);
            Preferences.Default.Set(UserIdKey, user.Id);
            Preferences.Default.Set(IsLoggedInKey, true);
        }

        public void ClearSession()
        {
            Preferences.Default.Clear();
        }

        public bool IsLoggedIn => Preferences.Default.Get(IsLoggedInKey, false);
        public string UserRole => Preferences.Default.Get(UserRoleKey, string.Empty);
        public string UserName => Preferences.Default.Get(UserNameKey, string.Empty);
        public string UserEmail => Preferences.Default.Get(UserEmailKey, string.Empty);
        public int UserId => Preferences.Default.Get(UserIdKey, 0);
    }
}
