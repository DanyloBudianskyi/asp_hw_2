using System.Text.Json;

namespace asp_hw_2.Services
{
    public class UserService
    {
        private List<User> Users { get; set; } = new List<User>();
        public UserService() 
        {
            Load();
        }
        private string UserDataFile = "Users.json";
        public void Load()
        {
            if (File.Exists(UserDataFile))
            {
                Users = JsonSerializer.Deserialize<List<User>>(File.ReadAllText(UserDataFile));
            }
        }
        public void AddUser(User user)
        {
            Users.Add(user);
            Save();
        }
        public User? FindByLogin(string login)
        {
            return Users.FirstOrDefault(x => x.Username == login);
        }
        public bool VerifyPassword(string login, string password)
        {
            return FindByLogin(login)?.Password == password; ;
        }
        private void Save()
        {
            File.WriteAllText(UserDataFile, JsonSerializer.Serialize(Users));
        }
    }
}
