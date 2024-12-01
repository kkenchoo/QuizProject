using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizModels
{
    public class Answer
    {
        public int Id { get; set; }
        public int QuestionID { get; set; }
        public string AnswerText { get; set; }
        public bool IsCorrect { get; set; }

        public Answer(int questionid, string answerText, bool isCorrect)
        {
            QuestionID = questionid;
            AnswerText = answerText;
            IsCorrect = isCorrect;
        }
    }
}
