using QuizModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace QuizRepository
{
    public class AnswerJsonRepository
    {
        private readonly string _answersfilePath;
        private List<Answer> _answers;


        public AnswerJsonRepository(string filePath)
        {
            _answersfilePath = filePath;
            _answers = LoadData();
        }
        public List<Answer> GetQuestionAnswers(int questionId)
        {
           

            return _answers.Where(x => x.QuestionID == questionId).ToList(); 
        }

        public void CreateAnswer(Answer answer) 
        {
            answer.Id = _answers.Any() ? _answers.Max(x => x.Id) + 1 : 1;
            _answers.Add(answer);

            SaveData();
        }

        public void DeleteAnswers(int questionId)
        {
            var answers = _answers.FirstOrDefault(a => a.QuestionID == questionId);
            
                _answers.Remove(answers);
                SaveData();
            
        }

        public void UpdateAnswer(Answer answer)
        {
            var index = _answers.FindIndex(a => a.Id == answer.Id);
            if (index >= 0)
            {
                _answers[index] = answer;
                SaveData();
            }
        }
        public void SaveData()
        {

            using (StreamWriter sw = new StreamWriter(_answersfilePath))
            {
                var json = JsonSerializer.Serialize(_answers, new JsonSerializerOptions() { WriteIndented = true });
                sw.WriteLine(json);

            }
        }

        private List<Answer> LoadData()
        {
            if (!File.Exists(_answersfilePath))
                return new List<Answer>();

            using (StreamReader sr = new StreamReader(_answersfilePath))
            {
                string json = sr.ReadToEnd();

             
                if (string.IsNullOrWhiteSpace(json))
                    return new List<Answer>();

                
                   
                    return JsonSerializer.Deserialize<List<Answer>>(json) ?? new List<Answer>();
                
               
            }
        }


    }
}
