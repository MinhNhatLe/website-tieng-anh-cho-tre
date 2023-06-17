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

        public ActionResult Choose_Image_Quiz(int? page, string searchImgQuiz, string sortImgQuiz)
        {
            //// lấy ra danh sách thể loại từ vựng và bỏ cái thứ 6 đi
            //List<Category_Vo> vocabulary_Type = db.Category_Vo.Where(s => s.ID_Category_Vo != 6).ToList();
            //// lấy ra danh sách câu đố 
            List<Quiz_Details> quiz_Details = db.Quiz_Details.ToList();
            //// lưu vào session để đẩy ra view
            Session["quiz_Details"] = quiz_Details;
            //return View(vocabulary_Type);



            List<Category_Vo> vocabulary_Type = db.Category_Vo.Where(s => s.ID_Category_Vo != 6).ToList();
            if (searchImgQuiz != null)
            {
                Session["searchImgQuiz"] = searchImgQuiz;
                vocabulary_Type = db.Category_Vo.Where(s => s.Name_Category_Vo.ToString().Contains(searchImgQuiz.Trim().ToLower())).Where(s => s.ID_Category_Vo != 6).ToList();
            }
            else
            {
                Session["searchImgQuiz"] = null;
                vocabulary_Type = db.Category_Vo.Where(s => s.ID_Category_Vo != 6).ToList();
            }
            List<Category_Vo> category_Vos = db.Category_Vo.Where(s => s.ID_Category_Vo != 6).ToList();
            //List<Category_Vo> category_Vos;

            if (sortImgQuiz == null || sortImgQuiz == "None")
            {
                Session["sortImgQuiz"] = "None";
                category_Vos = vocabulary_Type;
            }
            else if (sortImgQuiz == "AZ")
            {
                Session["sortImgQuiz"] = "A - Z";
                category_Vos = vocabulary_Type.OrderBy(s => s.Name_Category_Vo).ToList();
            }
            else
            {
                Session["sortImgQuiz"] = "Z - A";
                category_Vos = vocabulary_Type.OrderByDescending(s => s.Name_Category_Vo).ToList();
            }
            if (page == null)
            {
                page = 1;
            }
            int pageSize = 6;
            int pageNum = page ?? 1;
            return View(category_Vos.ToPagedList(pageNum, pageSize));

        }
        // lấy ra danh sách từ vựng lưu vào static
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
            // lấy danh sách câu hỏi và câu trả lời
            List<TempListAnswer> listAnss = new List<TempListAnswer>();
            TempListAnswer answer = new TempListAnswer();
            answer.IdAnswer = "0";
            answer.QuestionAnswer = "hello";
            listAnss.Add(answer);
            Session["Answer"] = listAnss;
            List<bool> save_Result = new List<bool>();
            // lưu kết quả vào session
            Session["Image_Quiz_Result"] = save_Result;
            Session["Vocabulary_Index"] = null;

            // lấy danh danh thể loại từ vựng ra
            List<Vocabulary> vocabulary = db.Vocabularies.Where(s => s.ID_Cate_Vo == id).ToList();
            // random từ vựng
            Random random = new Random();
            int x;
            // random ngầu nhiên lấy ra 5 từ vựng
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
            // nếu từ vựng đó không có hoặc dưới 5
            // thì quay lại trang chọn câu đố
            Session["Count_Image_Quiz_List"] = vocabularies.Count();
            if (vocabularies == null || vocabularies.Count() < 5)
            {
                return RedirectToAction("Choose_Image_Quiz", "Quiz");
                //return RedirectToAction("Error404", "Home");
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
            // Tạo một danh sách mới có kiểu TempListAnswer và gán nó bằng giá trị của Session "Answer" được ép kiểu thành List<TempListAnswer>.
            //Session "Answer" được truy xuất và lưu trữ từ trước.
            List<TempListAnswer> listAnss = new List<TempListAnswer>();
            listAnss = Session["Answer"] as List<TempListAnswer>;
            //Tạo một danh sách mới có kiểu bool và gán nó bằng giá trị của Session "Image_Quiz_Result" được ép kiểu thành List<bool>.
            //Session "Image_Quiz_Result" được truy xuất và lưu trữ từ trước.
            List<bool> save_Result = Session["Image_Quiz_Result"] as List<bool>;
            //Tạo các biến list, index, và quiz_Score và gán chúng bằng giá trị của các Session tương ứng.
            //Giá trị của các Session này được ép kiểu thành int từ kiểu dữ liệu gốc (thường là string).
            int list = int.Parse(Session["Image_Quiz_List"].ToString());
            int index = int.Parse(Session["Image_Quiz_Index"].ToString());
            int quiz_Score = int.Parse(Session["Image_Quiz_Score"].ToString());

            // lấy ra id của từ vựng
            Vocabulary vocabulary1 = db.Vocabularies.FirstOrDefault(s => s.ID_Vocabulary == vocabulary.ID_Vocabulary);

            // nếu kh có từ vựng thì chuyển hướng sang sang chọn từ vựng
            if (vocabulary1 == null)
            {
                return RedirectToAction("Choose_Image_Quiz", "Quiz");
            }
            else
            // nếu có từ vựng
            {
                if (vocabulary1.EN_Meaning.ToLower().Trim() == answer.ToLower().Trim())
                {
                    // nếu trả lời đúng thì +2 điểm
                    quiz_Score = quiz_Score + 2;
                    save_Result.Add(true);

                    // lấy ra danh sách rồi add vào
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
                // từ vựng tăng lên lưu vào Session["Image_Quiz_Index"]
                index++;
                Session["Image_Quiz_Index"] = index;
                // danh sách thì cũng lưu vào
                list++;
                Session["Image_Quiz_List"] = list;
                Session["Image_Quiz_Result"] = save_Result;
                Session["Image_Quiz_Score"] = quiz_Score;

                if (int.Parse(Session["Image_Quiz_Index"].ToString()) > 5)
                {
                    // này nếu là user thì như này
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
        public ActionResult Choose_DragDrop_Quiz(int? page, string searchDragDropQuiz, string sortDragDropQuiz)
        {
            // lấy danh sách câu đố ra trừ cái thứ 6
            //List<Quiz> quizzes = db.Quizs.Where(s => s.ID_Quiz != 6).ToList();
            List<Quiz_Details> quiz_Details = db.Quiz_Details.ToList();
            //// lưu vào session
            Session["quiz_Details"] = quiz_Details;
            Session["Choose_DragDrop_Quiz"] = null;
            //return View(quizzes);

            List<Quiz> quizzes = db.Quizs.Where(s => s.ID_Quiz != 6).ToList();
            if (searchDragDropQuiz != null)
            {
                Session["searchDragDropQuiz"] = searchDragDropQuiz;
                quizzes = db.Quizs.Where(s => s.Name_Quiz.ToString().Contains(searchDragDropQuiz.Trim().ToLower())).Where(s => s.ID_Quiz != 6).ToList();
            }
            else
            {
                Session["searchDragDropQuiz"] = null;
                quizzes = db.Quizs.Where(s => s.ID_Quiz != 6).ToList();
            }
            List<Quiz> quizzes1 = db.Quizs.Where(s => s.ID_Quiz != 6).ToList();

            if (sortDragDropQuiz == null || sortDragDropQuiz == "None")
            {
                Session["sortDragDropQuiz"] = "None";
                quizzes1 = quizzes;
            }
            else if (sortDragDropQuiz == "AZ")
            {
                Session["sortDragDropQuiz"] = "A - Z";
                quizzes1 = quizzes.OrderBy(s => s.Name_Quiz).ToList();
            }
            else
            {
                Session["sortDragDropQuiz"] = "Z - A";
                quizzes1 = quizzes.OrderByDescending(s => s.Name_Quiz).ToList();
            }
            if (page == null)
            {
                page = 1;
            }
            int pageSize = 6;
            int pageNum = page ?? 1;
            return View(quizzes1.ToPagedList(pageNum, pageSize));

        }
        public ActionResult Do_DragDrop_Quiz(int id)
        {
            // lấy id câu đố ra
            Quiz quizzes = db.Quizs.FirstOrDefault(s => s.ID_Quiz == id);
            // lưu vào session
            Session["Choose_DragDrop_Quiz"] = id;
            return View(quizzes);
        }
        [HttpPost]
        public ActionResult Do_DragDrop_Quiz(string dropone, string droptwo, string dropthree, string dropfour, string dropfive)
        {
            try
            {
                // gán id
                int id = int.Parse(Session["Choose_DragDrop_Quiz"].ToString());
                // lấy id câu đố ra
                Quiz quiz = db.Quizs.FirstOrDefault(s => s.ID_Quiz == id);
                // gán user
                int user_Code = int.Parse(Session["ID_User"].ToString());
                // lấy id user = tài khoản user
                User userz = db.Users.FirstOrDefault(s => s.ID_User == user_Code);
                // nếu câu đố có và tài khoản có
                if (quiz != null && userz != null)
                {
                    // khai báo ban đầu là 0 điểm
                    int quiz_score = 0;
                    // nếu dropone giống với câu trả lời của quiz trong db thì +2 và tương tự cho 5 cái
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
                    // lưu điểm vào session
                    Session["DragDrop_Quiz_Score"] = quiz_score;
                    // lấy danh sách câu đố và id người dùng
                    Quiz_Details quiz_Detail = db.Quiz_Details.FirstOrDefault(s => s.ID_Quiz == quiz.ID_Quiz && s.ID_User == userz.ID_User);
                    // update điểm câu đố vào
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
            // lấy danh sách câu đô
            List<Quiz> quizzes = new List<Quiz>();
            // tìm kiếm theo tên câu đô
            if (searchQuiz != null)
            {
                Session["searchQuiz"] = searchQuiz;
                quizzes = db.Quizs.Where(s => s.Name_Quiz.Contains(searchQuiz.Trim().ToLower()) && s.ID_Quiz != 6).ToList();
            }
            else
            // nếu không tìm kiếm gì thì đổ ra danh sách
            {
                Session["searchQuiz"] = null;
                quizzes = db.Quizs.Where(s => s.ID_Quiz != 6).ToList();
            }

            // lấy danh sách câu đố ra
            List<Quiz> quizzes1;
            // nếu kh sắp xếp gì thì đổ ra hết danh sách
            if (sortQuiz == null || sortQuiz == "None")
            {
                Session["sortQuiz"] = "None";
                quizzes1 = quizzes;
            }
            else if (sortQuiz == "AZ")
                // nếu sắp xếp từ AZ thì dùng OderBy
            {
                Session["sortQuiz"] = "A - Z";
                quizzes1 = quizzes.OrderBy(s => s.Name_Quiz).ToList();
            }
            else
            // nếu sắp xếp từ ZA thì dùng OderByDescending
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
                    // lấy danh sách câu đố ra 
                    // add vào thôi
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
            // lấy id của câu đố muốn cập nhật ra
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
                    // lấy id câu đố muốn cập nhật
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
            // lấy id câu đố muỗn xóa
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
            // lấy id câu đố muỗn xóa
            Quiz quiz1 = db.Quizs.FirstOrDefault(s => s.ID_Quiz == quiz.ID_Quiz);
            if (quiz != null)
            {
                // dùng remove để xóa
                db.Quizs.Remove(quiz1);
                db.SaveChanges();
            }
            return RedirectToAction("ListQuiz", "Quiz");
        }
    }
}