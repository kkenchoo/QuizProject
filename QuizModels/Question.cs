using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizModels
{
    public class Question
    {
        public int Id { get; set; }
        public int QuizID { get; set; }
        public string QuestionText { get; set; }
        public List<Answer> Answers { get; set; }

        public Question(string questionText, int quizid)
        {
            QuizID = quizid;
            QuestionText = questionText;
            Answers = new List<Answer>();
        }
    }
}
