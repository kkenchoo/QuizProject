using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using QuizModels;



namespace QuizRepository
{
    public class QuizJsonRepository
    {
        private readonly string _quizfilePath;
        private readonly string _userfilepath;
        private List<Quiz> _quizzes;

        public QuizJsonRepository(string filePath)
        {
            _quizfilePath = filePath;
            _quizzes = LoadData();
        }

        
       

        public List<string> GetAllQuizzesNames()
        {
            if (_quizzes == null)
                Console.WriteLine("No quizzes available"); 

            return _quizzes.Select(x => x.QuizName).ToList();


        }

        
        
        public void CreateQuiz(Quiz quiz)
        {
            quiz.Id = _quizzes.Any() ? _quizzes.Max(x => x.Id) + 1 : 1;
            _quizzes.Add(quiz);

            SaveData();
        }

       

        public void DeleteQuiz(int id)
        {
            var quiz = _quizzes.FirstOrDefault(a => a.Id == id);
            
                
                _quizzes.Remove(quiz);
                SaveData();
            
        }
       
        public Quiz GetQuizzByName(string quizName)
        {
            var quiz = _quizzes.FirstOrDefault(quiz => quiz.QuizName.Equals(quizName, StringComparison.OrdinalIgnoreCase));

            return quiz;
        }

        
        
        public void SaveData()
        {

            using (StreamWriter sw = new StreamWriter(_quizfilePath))
            {
                var json = JsonSerializer.Serialize(_quizzes, new JsonSerializerOptions() { WriteIndented = true });
                sw.WriteLine(json);

            }
        }

       

        private List<Quiz> LoadData()
        {
            if (!File.Exists(_quizfilePath))
                return new List<Quiz>();

            using (StreamReader sr = new StreamReader(_quizfilePath))
            {
                string json = sr.ReadToEnd();

                if (string.IsNullOrWhiteSpace(json))
                    return new List<Quiz>();

                
                    return JsonSerializer.Deserialize<List<Quiz>>(json) ?? new List<Quiz>();
                
                
            }
        }


    }
}
