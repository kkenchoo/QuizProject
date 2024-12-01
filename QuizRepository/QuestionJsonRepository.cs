using QuizModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace QuizRepository
{
    public class QuestionJsonRepository
    {
        private readonly string _questionfilePath;
        private List<Question> _questions;


        public QuestionJsonRepository(string filePath)
        {
            _questionfilePath = filePath;
            _questions = LoadData();
        }

        public void CreateQuestion(Question question)
        {
            question.Id = _questions.Any() ? _questions.Max(x => x.Id) + 1 : 1;
            _questions.Add(question);

            SaveData();
        }

        public List<Question> GetQuizQuestions(int quizId)
        {
            var questions = _questions.Where(x => x.QuizID == quizId).ToList();

            return questions;
        }

        public void DeleteQuestions(int quizId) 
        {
            var question = _questions.FirstOrDefault(a => a.QuizID == quizId);
            
                _questions.Remove(question);
                SaveData();
            
        }

        public void UpdateQuestion(Question question)
        {
            var index = _questions.FindIndex(a => a.Id == question.Id);
            if (index >= 0)
            {
                _questions[index] = question;
                SaveData();
            }
        }
        public void SaveData()
        {

            using (StreamWriter sw = new StreamWriter(_questionfilePath))
            {
                var json = JsonSerializer.Serialize(_questions, new JsonSerializerOptions() { WriteIndented = true });
                sw.WriteLine(json);

            }
        }

        private List<Question> LoadData()
        {
          
            if (!File.Exists(_questionfilePath))
                return new List<Question>();

           
            using (StreamReader sr = new StreamReader(_questionfilePath))
            {
                string json = sr.ReadToEnd();

               
                if (string.IsNullOrWhiteSpace(json))
                    return new List<Question>();

                
                    
                    return JsonSerializer.Deserialize<List<Question>>(json) ?? new List<Question>();
                
                
            }
        }


    }
}
