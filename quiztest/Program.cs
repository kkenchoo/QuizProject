using QuizRepository;
using QuizModels;
using System.Runtime.CompilerServices;
using System.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System;

namespace quiztest
{
    internal class Program
    {
        static void Main(string[] args)
        {
          
            var userrepository = new UserJsonRepository(@"C:\Users\User\source\repos\Quiz\QuizRepository\Data\user.json");
            var quizrepository = new QuizJsonRepository(@"C:\Users\User\source\repos\Quiz\QuizRepository\Data\quiz.json");
            var questionrepository = new QuestionJsonRepository(@"C:\Users\User\source\repos\Quiz\QuizRepository\Data\question.json");
            var answerrespository = new AnswerJsonRepository(@"C:\Users\User\source\repos\Quiz\QuizRepository\Data\answer.json");
            
            Console.WriteLine("\t\t\t\t\tWelcome");

            var leaderUsers = userrepository.GetScoreLeaderboard();
            foreach (var user in leaderUsers)
                Console.WriteLine($"User: {user.UserName}, Score: {user.Score}");
            

            Console.WriteLine("\nRegister (type 1)\nLogin (type 2)");

            User currentLoggedUser = null;
            bool isUserLogged = false;


            char choice1 = Convert.ToChar(Console.ReadLine());

            switch (choice1)
            {
                case '1':
                    Console.Write("Type Your New Username: ");
                    string newuserName = Console.ReadLine();

                    User user = new User(newuserName, 0);

                    userrepository.CreateUser(user);

                    Console.WriteLine("\nYour Account Was Saved!");
                    break;

                case '2':

                    Console.Write("\nEnter Your UserName: ");
                    string username = Console.ReadLine();

                    int userid = userrepository.GetUserAccByID(username);

                    User useracc = userrepository.GetUserAccount(userid);
                    isUserLogged = true;
                    currentLoggedUser = useracc;

                    Console.WriteLine($"Id: {useracc.Id}\nUserName: {useracc.UserName}\nTotal Score: {useracc.Score}");

                    break;

            }


            Console.WriteLine("\nChoose\nCREATE QUIZ (1)\nEDIT QUIZ(2)\nSTART QUIZ(3)\nDELETE QUIZ(4)\nEXIT(5)");
            char choice2 = Convert.ToChar(Console.ReadLine());

            switch (choice2)
            {
                case '1':
                    #region CREATE QUIZ
                    //create quiz
                    Console.Write("\nEnter The Name of Your Quiz: ");
                    string quizname = Console.ReadLine();

                    Quiz quiz = new Quiz(currentLoggedUser.Id, quizname);

                    quizrepository.CreateQuiz(quiz);

                    for(int i = 0; i < 5; i++)
                    {
                        Console.Write("Enter Question Text: ");
                        string questionText = Console.ReadLine();

                        Question question = new Question(questionText, quiz.Id);

                        questionrepository.CreateQuestion(question);

                        for(int j = 0; j < 4; j++)
                        {
                            Console.Write("\nEnter Answer Text: ");
                            string answerText = Console.ReadLine();

                            Console.Write("Is this the correct answer? (yes or no): ");
                            string isAnswerCorrectStr = Console.ReadLine();
                            bool isAnswerCorrect = isAnswerCorrectStr.Equals("yes", StringComparison.OrdinalIgnoreCase) ? true : false;


                            Answer answer = new Answer(question.Id, answerText, isAnswerCorrect);

                            answerrespository.CreateAnswer(answer);
                            
                        }
                    }


                    Console.WriteLine("Your Quiz Was Saved!");
                    break;
                #endregion
                case '2':
                    //edit quiz
                    bool hasAccessToEditQuiz = false;
                    Quiz toEditQuiz = null;

                    while (!hasAccessToEditQuiz)
                    {
                        var quizzes = quizrepository.GetAllQuizzesNames();

                        Console.WriteLine("Available Quizzes:");
                        foreach (var quizName in quizzes)
                            Console.WriteLine($"- {quizName}");

                        Console.Write("Choose the Quiz You Want to Edit: ");
                        string toEditQuizStr = Console.ReadLine();

                        toEditQuiz = quizrepository.GetQuizzByName(toEditQuizStr);

                        if (toEditQuiz == null)
                        {
                            Console.WriteLine("Quiz not found. Please try again.");
                            continue;
                        }

                        if (toEditQuiz.UserID != currentLoggedUser.Id)
                        {
                            Console.WriteLine("You don't have permission to edit this quiz.");
                            continue;
                        }

                        hasAccessToEditQuiz = true;
                    }

                    var quizToEditQuestions = questionrepository.GetQuizQuestions(toEditQuiz.Id);

                    for (int i = 0; i < quizToEditQuestions.Count(); i++)
                        Console.WriteLine($"{i + 1}. {quizToEditQuestions[i].QuestionText}");
                    

                    while (true)
                    {
                        Console.Write("Choose The Question You Want To Edit: ");
                        var inputNum = Console.ReadLine();

                        if (!int.TryParse(Console.ReadLine(), out int chosenQuestionIndex) || chosenQuestionIndex < 0 || chosenQuestionIndex > quizToEditQuestions.Count())
                        {
                            Console.WriteLine("Invalid choice. Please try again.");
                            continue;
                        }

                        if (chosenQuestionIndex == 0)
                            break;


                        var selectedQuestion = quizToEditQuestions[chosenQuestionIndex - 1];

                        Console.Write("Enter The New Question Text: ");
                        string newQuestionText = Console.ReadLine();



                        selectedQuestion.QuestionText = newQuestionText;

                        questionrepository.UpdateQuestion(selectedQuestion);

                        var oldAnswers = answerrespository.GetQuestionAnswers(selectedQuestion.Id);

                        for (int j = 0; j < oldAnswers.Count(); j++)
                        {
                            var currentAnswer = oldAnswers[j];
                            Console.WriteLine($"{j + 1}. {currentAnswer.AnswerText} (Correct: {currentAnswer.IsCorrect})");

                              
                            Console.Write($"Enter new text for answer {j + 1} (or press Enter to keep unchanged): ");
                            string newAnswerText = Console.ReadLine();
                            if (!string.IsNullOrWhiteSpace(newAnswerText))
                                currentAnswer.AnswerText = newAnswerText;

                      
                            Console.Write($"Is this the correct answer? (yes or no, or press Enter to keep unchanged): ");
                            string isCorrectInput = Console.ReadLine();
                            if (!string.IsNullOrWhiteSpace(isCorrectInput))
                                currentAnswer.IsCorrect = isCorrectInput.Equals("yes", StringComparison.OrdinalIgnoreCase);

                                answerrespository.UpdateAnswer(currentAnswer);

                        }

                        Console.Write("Are You Done Editing (yes or no): ");
                        string doneEditingInput = Console.ReadLine();
                        if (doneEditingInput == "yes")
                            break;
                        
                    }

                    Console.WriteLine("Quiz Updated Successfully");
                    
                    break;
                case '3':
                    //start quiz 
                    #region DO QUIZ
                    bool hasAccessToQuiz = false;
                    Quiz chosenQuiz = null;
                    while (!hasAccessToQuiz)
                    {
                        var quizzes = quizrepository.GetAllQuizzesNames();
                       
                        
                        Console.WriteLine("Available Quizzes:");

                        foreach (var quizName in quizzes)
                            Console.WriteLine($"- {quizName}");
                        
                        Console.Write("Choose Quiz: ");
                        string chosenQuizstr = Console.ReadLine();


                    
                        chosenQuiz = quizrepository.GetQuizzByName(chosenQuizstr);

                        if (chosenQuiz.UserID == currentLoggedUser.Id)
                            Console.WriteLine("You can't acces this quiz");
                        else
                            hasAccessToQuiz |= true;
                    }

                    var quizQuestions = questionrepository.GetQuizQuestions(chosenQuiz.Id);

                    foreach (var question in quizQuestions)
                    {
                        Console.WriteLine(question.QuestionText);
                        var questionAnswers = answerrespository.GetQuestionAnswers(question.Id);

                        for (int i = 0; i < questionAnswers.Count(); i++)
                            Console.WriteLine($"{i + 1}. {questionAnswers[i].AnswerText}");
                        

                        int chosenAnswerIndex = -1;

                        while (true)
                        {
                            Console.Write("Choose Your Answer (enter the number): ");
                            var input = Console.ReadLine();

                            if (int.TryParse(input, out chosenAnswerIndex) && chosenAnswerIndex > 0 && chosenAnswerIndex <= questionAnswers.Count())
                                break;
                            
                            else
                                Console.WriteLine("Invalid choice. Please try again.");
                            
                        }
                        var selectedAnswer = questionAnswers[chosenAnswerIndex - 1];
                        

                        if (selectedAnswer.IsCorrect)
                        {
                            currentLoggedUser.Score += 20;
                            userrepository.UpdateUser(currentLoggedUser);
                            Console.WriteLine("Correct! +20 points.");
                        }
                        else
                        {
                            currentLoggedUser.Score -= 20;
                            userrepository.UpdateUser(currentLoggedUser);

                            Console.WriteLine("Wrong! -20 points.");
                        }

                    }


                    break;
                #endregion
                case '4':
                    //delete quiz 

                    bool hasAccessToDeleteQuiz = false;

                    while (!hasAccessToDeleteQuiz)
                    {
                        
                        var quizzes = quizrepository.GetAllQuizzesNames();

                        foreach (var quizName in quizzes)
                            Console.WriteLine(quizName);

                        Console.WriteLine("Enter the name of the quiz you want to delete: ");
                        string toDeleteQuizStr = Console.ReadLine();

                        
                        var chosenQuizToDel = quizrepository.GetQuizzByName(toDeleteQuizStr);

                        if (chosenQuizToDel == null)
                        {
                            Console.WriteLine("Quiz not found. Please try again.");
                            continue;
                        }

                    
                        if (chosenQuizToDel.UserID != currentLoggedUser.Id)
                        {
                            Console.WriteLine("You do not have access to delete this quiz.");
                            continue;
                        }

                        hasAccessToDeleteQuiz = true;

                        

                       
                        var questions = questionrepository.GetQuizQuestions(chosenQuizToDel.Id);
                        foreach (var question in questions)
                        {
                            var answers = answerrespository.GetQuestionAnswers(question.Id);
                            foreach (var answer in answers)
                            {
                                answerrespository.DeleteAnswers(answer.Id);
                                
                            }
                            questionrepository.DeleteQuestions(question.Id); 
                        }

                    
                        quizrepository.DeleteQuiz(chosenQuizToDel.Id);
                        Console.WriteLine("Quiz deleted successfully.");

                       
                    }
                    break;
                case '5':
                    break;
            }
        }
    }
}
