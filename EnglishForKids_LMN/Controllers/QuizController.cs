using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using EnglishForKids_LMN.Models;
using PagedList;
using PagedList.Mvc;

namespace EnglishForKids_LMN.Controllers
{
    public class QuizController : Controller
    {
        English_LearningEntities db = new English_LearningEntities();

        // GET: Quiz
        public ActionResult Choose_Image_Quiz()
        {
            List<Category_Vo> vocabulary_Type = db.Category_Vo.Where(s => s.ID_Category_Vo != 6).ToList();
            List<Quiz_Details> quiz_Details = db.Quiz_Details.ToList();
            Session["quiz_Details"] = quiz_Details;
            return View(vocabulary_Type);

        }
        static List<Vocabulary> vocabularies = new List<Vocabulary>();


        //Lấy ngẫu nhiên 5 từ vựng từ danh sách vocabulary (nếu danh sách có ít hơn 5 từ thì không bắt đầu làm bài kiểm tra mà chuyển sang trang chọn loại từ vựng).
        //Gán các từ vựng được chọn vào session và gọi view để hiển thị câu hỏi đầu tiên.
        //Nếu danh sách từ vựng đủ 5 từ, gọi view để hiển thị câu hỏi đầu tiên.
        public ActionResult Do_Image_Quiz(int id)
        {
            vocabularies.Clear();
            Session["Image_Quiz_List"] = 0;
            Session["Image_Quiz_Index"] = 1;
            Session["Image_Quiz_Score"] = 0;
            List<TempListAnswer> listAnss = new List<TempListAnswer>();
            TempListAnswer answer = new TempListAnswer();
            answer.IdAnswer = "0";
            answer.QuestionAnswer = "hello";
            listAnss.Add(answer);
            Session["Answer"] = listAnss;
            List<bool> save_Result = new List<bool>();
            Session["Image_Quiz_Result"] = save_Result;
            Session["Vocabulary_Index"] = null;
            List<Vocabulary> vocabulary = db.Vocabularies.Where(s => s.ID_Cate_Vo == id).ToList();
            Random random = new Random();
            int x;
            for (int i = 0; i < 5; i++)
            {
                if (vocabularies.Count() < 5)
                {
                    x = random.Next(0, vocabulary.Count());
                    if (vocabularies.Contains(vocabulary[x]))
                    {
                        i--;
                    }
                    else
                    {
                        vocabularies.Add(vocabulary[x]);
                    }
                }
                else
                {
                    break;
                }
            }
            Session["Count_Image_Quiz_List"] = vocabularies.Count();
            if (vocabularies == null || vocabularies.Count() < 5)
            {
                return RedirectToAction("Choose_Image_Quiz", "Quiz");
            }
            else
            {
                return View(vocabularies[int.Parse(Session["Image_Quiz_List"].ToString())]);
            }
        }

        // nếu đúng thì được 2 điểm và được lưu vào ds kết quả
        //Nếu chỉ mục câu hỏi vượt quá 5, điểm số của người dùng sẽ được lưu vào cơ sở dữ liệu và họ sẽ được chuyển hướng đến trang Kết quả. 
        [HttpPost]
        public ActionResult Check_Answer(Vocabulary vocabulary, string answer)
        {
            List<TempListAnswer> listAnss = new List<TempListAnswer>();
            listAnss = Session["Answer"] as List<TempListAnswer>;

            //
            List<bool> save_Result = Session["Image_Quiz_Result"] as List<bool>;
            int list = int.Parse(Session["Image_Quiz_List"].ToString());
            int index = int.Parse(Session["Image_Quiz_Index"].ToString());
            int quiz_Score = int.Parse(Session["Image_Quiz_Score"].ToString());
            Vocabulary vocabulary1 = db.Vocabularies.FirstOrDefault(s => s.ID_Vocabulary == vocabulary.ID_Vocabulary);
            //


            //
            if (vocabulary1 == null)
            {
                return RedirectToAction("Choose_Image_Quiz", "Quiz");
            }
            else
            {
                if (vocabulary1.EN_Meaning.ToLower().Trim() == answer.ToLower().Trim())
                {
                    quiz_Score = quiz_Score + 2;
                    save_Result.Add(true);

                    TempListAnswer aa = new TempListAnswer();
                    aa.IdAnswer = index.ToString();
                    aa.QuestionAnswer = answer.ToString();
                    aa.Answers = answer.ToString();
                    listAnss.Add(aa);
                    Session["Answer"] = listAnss;
                }
                else
                {
                    TempListAnswer aa = new TempListAnswer();
                    aa.IdAnswer = index.ToString();
                    aa.QuestionAnswer = answer.ToString();
                    aa.Answers = vocabulary1.EN_Meaning.ToString();
                    listAnss.Add(aa);
                    Session["Answer"] = listAnss;
                    save_Result.Add(false);
                }
                index++;
                Session["Image_Quiz_Index"] = index;
                list++;
                Session["Image_Quiz_List"] = list;
                Session["Image_Quiz_Result"] = save_Result;
                Session["Image_Quiz_Score"] = quiz_Score;

                if (int.Parse(Session["Image_Quiz_Index"].ToString()) > 5)
                {
                    int user_Code = int.Parse(Session["ID_User"].ToString());
                    if (db.Quiz_Details.FirstOrDefault(s => s.ID_User == user_Code && s.ID_Category_Vo == vocabulary1.ID_Cate_Vo) != null)
                    {
                        Quiz_Details quiz_Detail = db.Quiz_Details.FirstOrDefault(s => s.ID_User == user_Code && s.ID_Category_Vo == vocabulary1.ID_Cate_Vo);
                        if (quiz_Detail.Quiz_Score < quiz_Score)
                        {
                            quiz_Detail.Quiz_Score = quiz_Score;
                            db.Quiz_Details.AddOrUpdate(quiz_Detail);
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        Quiz_Details quiz_Detail = new Quiz_Details();
                        quiz_Detail.Quiz_Score = int.Parse(Session["Image_Quiz_Score"].ToString());
                        quiz_Detail.ID_Category_Vo = int.Parse(vocabulary1.ID_Cate_Vo.ToString());
                        quiz_Detail.ID_User = int.Parse(Session["ID_User"].ToString());
                        quiz_Detail.ID_Quiz = 6;
                        db.Quiz_Details.Add(quiz_Detail);
                        db.SaveChanges();
                    }
                    return RedirectToAction("Result", "Quiz");
                }
                else
                {
                    Session["Vocabulary"] = vocabularies[int.Parse(Session["Image_Quiz_List"].ToString())];
                    return RedirectToAction("Continue", "Quiz");
                }
            }
        }

        //phương thức Continue() để hiển thị câu hỏi tiếp theo của bài trắc nghiệm hình ảnh
        public ActionResult Continue()
        {
            if (Session["Vocabulary"] == null)
            {
                return RedirectToAction("Choose_Image_Quiz", "Quiz");
            }
            else
            {
                Vocabulary vocabulary = Session["Vocabulary"] as Vocabulary;
                return View(vocabulary);
            }
        }
        public ActionResult Result()
        {
            return View();
        }
        //phương thức kiểm tra xem câu trả lời của người dùng có khớp với nghĩa tiếng Anh của từ vựng hay không
        //Nếu có, điểm số của người dùng tăng lên 2, kết quả trả lời được lưu vào danh sách save_Result, và câu trả lời tạm thời được lưu vào danh sách listAnss.
        public ActionResult Choose_DragDrop_Quiz()
        {
            List<Quiz> quizzes = db.Quizs.Where(s => s.ID_Quiz != 6).ToList();
            List<Quiz_Details> quiz_Details = db.Quiz_Details.ToList();
            Session["quiz_Details"] = quiz_Details;
            Session["Choose_DragDrop_Quiz"] = null;
            return View(quizzes);
        }
        public ActionResult Do_DragDrop_Quiz(int id)
        {
            Quiz quizzes = db.Quizs.FirstOrDefault(s => s.ID_Quiz == id);
            Session["Choose_DragDrop_Quiz"] = id;
            return View(quizzes);
        }
        [HttpPost]
        public ActionResult Do_DragDrop_Quiz(string dropone, string droptwo, string dropthree, string dropfour, string dropfive)
        {
            try
            {
                int id = int.Parse(Session["Choose_DragDrop_Quiz"].ToString());
                Quiz quiz = db.Quizs.FirstOrDefault(s => s.ID_Quiz == id);
                int user_Code = int.Parse(Session["ID_User"].ToString());
                User userz = db.Users.FirstOrDefault(s => s.ID_User == user_Code);
                if (quiz != null && userz != null)
                {
                    int quiz_score = 0;
                    if (dropone == quiz.Answer_1)
                    {
                        quiz_score += 2;
                    }
                    if (droptwo == quiz.Answer_2)
                    {
                        quiz_score += 2;
                    }
                    if (dropthree == quiz.Answer_3)
                    {
                        quiz_score += 2;
                    }
                    if (dropfour == quiz.Answer_4)
                    {
                        quiz_score += 2;
                    }
                    if (dropfive == quiz.Answer_5)
                    {
                        quiz_score += 2;
                    }
                    Session["DragDrop_Quiz_Score"] = quiz_score;
                    Quiz_Details quiz_Detail = db.Quiz_Details.FirstOrDefault(s => s.ID_Quiz == quiz.ID_Quiz && s.ID_User == userz.ID_User);
                    if (quiz_Detail != null)
                    {
                        if (quiz_Detail.Quiz_Score < quiz_score)
                        {
                            quiz_Detail.Quiz_Score = quiz_score;
                            db.Quiz_Details.AddOrUpdate(quiz_Detail);
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        Quiz_Details quiz_Detail1 = new Quiz_Details();
                        quiz_Detail1.ID_User = userz.ID_User;
                        quiz_Detail1.ID_Quiz = quiz.ID_Quiz;
                        quiz_Detail1.Quiz_Score = quiz_score;
                        quiz_Detail1.ID_Category_Vo = 6;
                        db.Quiz_Details.AddOrUpdate(quiz_Detail1);
                        db.SaveChanges();
                    }
                    return RedirectToAction("Result_DragDrop_Quiz");
                }
                return this.Do_DragDrop_Quiz(id);
            }
            catch
            {
                //return HttpNotFound();
                return RedirectToAction("Error404", "Home");
            }
        }
        public ActionResult Result_DragDrop_Quiz()
        {
            return View();
        }
        public ActionResult ListQuiz(int? page, string searchQuiz, string sortQuiz)
        {
            List<Quiz> quizzes = new List<Quiz>();
            if (searchQuiz != null)
            {
                Session["searchQuiz"] = searchQuiz;
                quizzes = db.Quizs.Where(s => s.Name_Quiz.Contains(searchQuiz.Trim().ToLower()) && s.ID_Quiz != 6).ToList();
            }
            else
            {
                Session["searchQuiz"] = null;
                quizzes = db.Quizs.Where(s => s.ID_Quiz != 6).ToList();
            }
            List<Quiz> quizzes1;
            if (sortQuiz == null || sortQuiz == "None")
            {
                Session["sortQuiz"] = "None";
                quizzes1 = quizzes;
            }
            else if (sortQuiz == "AZ")
            {
                Session["sortQuiz"] = "A - Z";
                quizzes1 = quizzes.OrderBy(s => s.Name_Quiz).ToList();
            }
            else
            {
                Session["sortQuiz"] = "Z - A";
                quizzes1 = quizzes.OrderByDescending(s => s.Name_Quiz).ToList();
            }
            if (page == null)
            {
                page = 1;
            }
            int pageSize = 9;
            int pageNum = page ?? 1;
            return View(quizzes1.ToPagedList(pageNum, pageSize));
        }
        public ActionResult CreateQ()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateQ(Quiz quiz)
        {
            try
            {
                if (quiz != null)
                {
                    Quiz quiz1 = new Quiz();
                    quiz1.Name_Quiz = quiz.Name_Quiz;
                    quiz1.Content = quiz.Content;
                    quiz1.Answer_1 = quiz.Answer_1;
                    quiz1.Answer_2 = quiz.Answer_2;
                    quiz1.Answer_3 = quiz.Answer_3;
                    quiz1.Answer_4 = quiz.Answer_4;
                    quiz1.Answer_5 = quiz.Answer_5;
                    db.Quizs.Add(quiz1);
                    db.SaveChanges();
                }
                return RedirectToAction("ListQuiz", "Quiz");
            }
            catch
            {
                //return HttpNotFound();
                return RedirectToAction("Error404", "Home");
            }
        }
        public ActionResult EditQ(int id)
        {
            Quiz quiz = db.Quizs.FirstOrDefault(s => s.ID_Quiz == id);
            if (quiz != null)
            {
                return View(quiz);
            }
            return RedirectToAction("ListQuiz", "Quiz");
        }
        [HttpPost]
        public ActionResult EditQ(Quiz quiz)
        {
            try
            {
                if (quiz != null)
                {
                    if (db.Quizs.FirstOrDefault(s => s.ID_Quiz == quiz.ID_Quiz) != null)
                    {
                        Quiz quiz1 = db.Quizs.FirstOrDefault(s => s.ID_Quiz == quiz.ID_Quiz);
                        quiz1.Name_Quiz = quiz.Name_Quiz;
                        quiz1.Content = quiz.Content;
                        quiz1.Answer_1 = quiz.Answer_1;
                        quiz1.Answer_2 = quiz.Answer_2;
                        quiz1.Answer_3 = quiz.Answer_3;
                        quiz1.Answer_4 = quiz.Answer_4;
                        quiz1.Answer_5 = quiz.Answer_5;
                        db.Quizs.AddOrUpdate(quiz1);
                        db.SaveChanges();
                    }
                }
                return RedirectToAction("ListQuiz", "Quiz");
            }
            catch
            {
                //return HttpNotFound();
                return RedirectToAction("Error404", "Home");
            }
        }
        public ActionResult DeleteQ(int id)
        {
            Quiz quiz = db.Quizs.FirstOrDefault(s => s.ID_Quiz == id);
            if (quiz != null)
            {
                return View(quiz);
            }
            return RedirectToAction("ListQuiz", "Quiz");
        }
        [HttpPost]
        public ActionResult DeleteQ(Quiz quiz)
        {
            Quiz quiz1 = db.Quizs.FirstOrDefault(s => s.ID_Quiz == quiz.ID_Quiz);
            if (quiz != null)
            {
                db.Quizs.Remove(quiz1);
                db.SaveChanges();
            }
            return RedirectToAction("ListQuiz", "Quiz");
        }
    }
}