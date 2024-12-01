using QuizModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace QuizRepository
{
   
    public class UserJsonRepository
    {
        private readonly string _userfilePath;
        private List<User> _users;

        
        public UserJsonRepository(string filePath)
        {
            _userfilePath = filePath;
            _users = LoadData();
        }

       

       

        public User GetUserAccount(int id)
        {
            var user = _users.FirstOrDefault(user => user.Id == id);
         
            return user;
        }

        public int GetUserAccByID(string username)
        {
            var user = _users.FirstOrDefault(user => user.UserName.Equals(username, StringComparison.OrdinalIgnoreCase));
        

            return user.Id;
        }
        
        


        public List<User> GetScoreLeaderboard()
        {
            var sortedUsers = _users
                .OrderByDescending(user => user.Score) 
                .ToList(); 

            return sortedUsers;
            
            
        }

        public void CreateUser(User user)
        {
            user.Id = _users.Any() ? _users.Max(a => a.Id) + 1 : 1;
            _users.Add(user);
            SaveData();

        }
        public void UpdateUser(User user)
        {
            var index = _users.FindIndex(a => a.Id == user.Id);
            if (index >= 0)
            {
                _users[index] = user;
                SaveData();
            }
        }
        public void Delete(int id)
        {
            var account = _users.FirstOrDefault(a => a.Id == id);
            if (account != null)
            {
                _users.Remove(account);
                SaveData();
            }
        }
        public void SaveData()
        {

            using (StreamWriter sw = new StreamWriter(_userfilePath))
            {
                var json = JsonSerializer.Serialize(_users, new JsonSerializerOptions() { WriteIndented = true });
                sw.WriteLine(json);

            }
        }

        private List<User> LoadData()
        {
            if (!File.Exists(_userfilePath))
                return new List<User>();



            using (StreamReader sr = new StreamReader(_userfilePath))
            {
                string json = sr.ReadToEnd();

                if (string.IsNullOrWhiteSpace(json))
                    return new List<User>();


    
                return JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();


            }
        }
    }
}
