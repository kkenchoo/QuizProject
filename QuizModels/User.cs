using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace QuizModels
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public int Score { get; set; }

        public List<Quiz> Quizzes { get; set; } 

        public User(string userName, int score)
        {
            UserName = userName;
            Score = score;
            Quizzes = new List<Quiz>();
        }

        
    }


   
    

   

    
}
        
