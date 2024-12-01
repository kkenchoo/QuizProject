using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizModels
{
    public class Quiz
    {
        public int Id { get; set; }
        public int UserID { get; set; }
        public string QuizName { get; set; }
        public List<Question> Questions { get; set; }

        public Quiz(int userid, string quizName)
        {
            UserID = userid;
            QuizName = quizName;
            Questions = new List<Question>();
        }

        

        

    }
}
