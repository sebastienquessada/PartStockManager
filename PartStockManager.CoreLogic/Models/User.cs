namespace PartStockManager.CoreLogic.Models
{   
    public class User
    {
        public string Username { get; private set; }
        public string PasswordHash { get; private set; }
        public UserProfile Profile { get; private set; }

        public User(string username, string passwordHash, UserProfile profile)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentNullException(nameof(username), "Username cannot be null or empty.");
            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentNullException(nameof(passwordHash), "PasswordHash cannot be null or empty.");

            Username = username;
            PasswordHash = passwordHash;
            Profile = profile;
        }
    }
}
