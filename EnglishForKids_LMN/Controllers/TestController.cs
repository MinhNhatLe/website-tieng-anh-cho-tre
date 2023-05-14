using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using EnglishForKids_LMN.Models;
using PagedList;

namespace EnglishForKids_LMN.Controllers
{
    public class TestController : Controller
    {
        // GET: Test
        English_LearningEntities db = new English_LearningEntities();
        public ActionResult Choose_Test()
        {
            List<Test> tests = db.Tests.ToList();
            List<Test_Details> test_Details = db.Test_Details.ToList();
            Session["test_Details"] = test_Details;
            return View(tests);
        }
        static List<Question> questions = new List<Question>();
        public ActionResult Do_Test(int id)
        {
            questions.Clear();
            Session["Test_Score"] = 0;
            Test test = db.Tests.FirstOrDefault(s => s.ID_Test == id);
            List<Question> questions1 = db.Questions.Where(s => s.ID_Test == test.ID_Test).ToList();
            Session["video"] = test.Video;
            Session["paragraph"] = test.Paragraph;
            Random random = new Random();
            int x;
            for (int i = 0; i < 10; i++)
            {
                if (questions.Count() < 10)
                {
                    x = random.Next(0, questions1.Count());
                    if (questions.Contains(questions1[x]))
                    {
                        i--;
                    }
                    else
                    {
                        questions.Add(questions1[x]);
                    }
                }
                else
                {
                    break;
                }
            }



            List<Question> questions2 = new List<Question>();
            List<Question> questions3 = new List<Question>();
            List<Question> questions4 = new List<Question>();
            List<Question> questions5 = new List<Question>();
            foreach (Question item in questions)
            {
                if (item.Type_Question == "multipe-choice")
                {
                    questions2.Add(item);
                }
                else if (item.Type_Question == "text")
                {
                    questions3.Add(item);
                }
                else if (item.Type_Question == "paragraph")
                {
                    questions4.Add(item);
                }
                else
                {
                    questions5.Add(item);
                }
            }
            Session["questions2"] = questions2;
            Session["questions3"] = questions3;
            Session["questions4"] = questions4;
            Session["questions5"] = questions5;
            questions.Clear();
            questions.AddRange(questions2);
            questions.AddRange(questions3);
            questions.AddRange(questions4);
            questions.AddRange(questions5);
            if (questions == null || questions.Count() < 10)
            {
                return RedirectToAction("Choose_Test", "Test");
            }
            else
            {
                return View(questions);
            }
        }
        [HttpPost]
        public ActionResult Do_Test(string choice1, string choice2, string choice3, string choice4, string choice5, string choice6, string choice7, string text1, string text2, string text3)
        {
            int test_Score = int.Parse(Session["Test_Score"].ToString());
            if (questions[0].Answer_Correct == choice1)
            {
                test_Score++;
            }
            if (questions[1].Answer_Correct == choice2)
            {
                test_Score++;
            }
            if (questions[2].Answer_Correct == choice3)
            {
                test_Score++;
            }
            if (questions[3].Answer_Correct == text1)
            {
                test_Score++;
            }
            if (questions[4].Answer_Correct == text2)
            {
                test_Score++;
            }
            if (questions[5].Answer_Correct == text3)
            {
                test_Score++;
            }
            if (questions[6].Answer_Correct == choice4)
            {
                test_Score++;
            }
            if (questions[7].Answer_Correct == choice5)
            {
                test_Score++;
            }
            if (questions[8].Answer_Correct == choice6)
            {
                test_Score++;
            }
            if (questions[9].Answer_Correct == choice7)
            {
                test_Score++;
            }
            Session["choice1"] = choice1;
            Session["choice2"] = choice2;
            Session["choice3"] = choice3;
            Session["choice4"] = choice4;
            Session["choice5"] = choice5;
            Session["choice6"] = choice6;
            Session["choice7"] = choice7;
            Session["text1"] = text1;
            Session["text2"] = text2;
            Session["text3"] = text3;



            Session["Test_Score"] = test_Score;
            // Lấy thông tin user
            int user_Code = int.Parse(Session["ID_User"].ToString());
            User userz = db.Users.FirstOrDefault(s => s.ID_User == user_Code);
            // Lưu kết quả bài kiểm tra vào CSDL
            int id = int.Parse(questions[0].ID_Test.ToString());
            if (db.Test_Details.FirstOrDefault(s => s.ID_Test == id && s.ID_User == user_Code) != null)
            {
                Test_Details test_Detail = db.Test_Details.FirstOrDefault(s => s.ID_Test == id && s.ID_User == user_Code);
                if (test_Detail.Test_Score < test_Score)
                {
                    test_Detail.Test_Score = test_Score;
                    db.Test_Details.AddOrUpdate(test_Detail);
                    db.SaveChanges();
                }
            }
            else
            {
                Test_Details test_Detail = new Test_Details();
                test_Detail.Test_Score = int.Parse(Session["Test_Score"].ToString());
                test_Detail.ID_User = int.Parse(Session["ID_User"].ToString());
                test_Detail.ID_Test = int.Parse(questions[0].ID_Test.ToString());
                db.Test_Details.Add(test_Detail);
                db.SaveChanges();
            }
            //MailAddress fromGMail = new MailAddress("garena281215@gmail.com", "StartEndSchools");
            //MailAddress toGMail = new MailAddress(userz.Email, "Me");
            MailMessage Message = new MailMessage();
            SmtpClient smtp = new SmtpClient();
            Message.From = new MailAddress("lmnhat.englishforkids@gmail.com");
            Message.To.Add(new MailAddress(userz.Email));
            {
                Message.IsBodyHtml = true;
                Message.Subject = "Test result";
                Message.Body = "Hello " + userz.Name_User.ToString() + ",\n"
                + "Result : \n\n"
                + "---------------------------------------------------"
                + "Question 1 : " + questions[0].Content + "\n"
                + "Your answer : " + choice1 + "\n\n"
                + "Question 2 : " + questions[1].Content + "\n"
                + "Your answer : " + choice2 + "\n\n"
                + "Question 3 : " + questions[2].Content + "\n"
                + "Your answer : " + choice3 + "\n\n"
                + "Question 4 : " + questions[3].Content + "\n"
                + "Your answer : " + text1 + "\n\n"
                + "Question 5 : " + questions[4].Content + "\n"
                + "Your answer : " + text2 + "\n\n"
                + "Question 6 : " + questions[5].Content + "\n"
                + "Your answer : " + text3 + "\n\n"
                + "Question 7 : " + questions[6].Content + "\n"
                + "Your answer : " + choice4 + "\n\n"
                + "Question 8 : " + questions[7].Content + "\n"
                + "Your answer : " + choice5 + "\n\n"
                + "Question 9 : " + questions[8].Content + "\n"
                + "Your answer : " + choice6 + "\n\n"
                + "Question 10 : " + questions[9].Content + "\n"
                + "Your answer : " + choice7 + "\n\n"
                + "-----------------------------------------------------"
                + "Total : " + test_Score + "\n\n"
                + "Wish you have a useful and fun learning session,\n"
                + "English For Kids";
                Message.Priority = MailPriority.High;
                Message.IsBodyHtml = false;
            };

            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential("lmnhat.englishforkids@gmail.com", "p");
            //smtp.Send(Message);
            return RedirectToAction("Result_Test", "Test");
        }

            public ActionResult Result_Test()
        {
            return View();
        }
        public ActionResult ListTest(int? page, string searchTest, string sortTest)
        {
            List<Test> tests = new List<Test>();
            if (searchTest != null)
            {
                Session["searchTest"] = searchTest;
                tests = db.Tests.Where(s => s.Name_Test.Contains(searchTest.Trim().ToLower())).ToList();
            }
            else
            {
                Session["searchTest"] = null;
                tests = db.Tests.ToList();
            }
            List<Test> tests1;
            if (sortTest == null || sortTest == "None")
            {
                Session["sortTest"] = "None";
                tests1 = tests;
            }
            else if (sortTest == "AZ")
            {
                Session["sortTest"] = "A - Z";
                tests1 = tests.OrderBy(s => s.Name_Test).ToList();
            }
            else
            {
                Session["sortTest"] = "Z - A";
                tests1 = tests.OrderByDescending(s => s.Name_Test).ToList();
            }
            if (page == null)
            {
                page = 1;
            }
            int pageSize = 9;
            int pageNum = page ?? 1;
            return View(tests1.ToPagedList(pageNum, pageSize));
        }


        public ActionResult ListQuestion(int? page, string searchQuestion, string sortQuestion)
        {
            List<Question> questions = new List<Question>();
            if (searchQuestion != null)
            {
                Session["searchQuestion"] = searchQuestion;
                questions = db.Questions.Where(s => s.Content.Contains(searchQuestion.Trim().ToLower())).ToList();
            }
            else
            {
                Session["searchQuestion"] = null;
                questions = db.Questions.ToList();
            }
            List<Question> questions1;
            if (sortQuestion == null || sortQuestion == "None")
            {
                Session["sortQuestion"] = "None";
                questions1 = questions;
            }
            else if (sortQuestion == "AZ")
            {
                Session["sortQuestion"] = "A - Z";
                questions1 = questions.OrderBy(s => s.Content).ToList();
            }
            else
            {
                Session["sortQuestion"] = "Z - A";
                questions1 = questions.OrderByDescending(s => s.Content).ToList();
            }
            if (page == null)
            {
                page = 1;
            }
            int pageSize = 9;
            int pageNum = page ?? 1;
            return View(questions1.ToPagedList(pageNum, pageSize));
        }



        public ActionResult CreateT()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateT(Test test)
        {
            if (test != null)
            {


                Test test1 = new Test();
                test1.Name_Test = test.Name_Test;
                test1.Paragraph = test.Paragraph;
                test1.Video = test.Video;
                db.Tests.Add(test1);
                db.SaveChanges();

            }
            return RedirectToAction("ListTest", "Test");
        }




        

        public ActionResult Create_Question()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create_Question(Question question)
        {
            if (question != null)
            {
                Question question1 = new Question();
                question1.ID_Question = question.ID_Question;
                question1.Content = question.Content;
                question1.Type_Question = question.Type_Question;
                question1.Answer_Content1 = question.Answer_Content1;
                question1.Answer_Content2 = question.Answer_Content2;
                question1.Answer_Content3 = question.Answer_Content3;
                question1.Answer_Content4 = question.Answer_Content4;
                question1.Answer_Correct = question.Answer_Correct;
                question1.ID_Test = question.ID_Test;
                db.Questions.Add(question1);
                db.SaveChanges();

            }
            return RedirectToAction("ListQuestion", "Test");
        }


        public ActionResult EditQ(int id)
        {
            Test test = db.Tests.FirstOrDefault(s => s.ID_Test == id);
            if (test != null)
            {
                return View(test);
            }
            return RedirectToAction("ListTest", "Test");
        }
        [HttpPost]
        public ActionResult EditQ(Test test)
        {
            Test test1 = db.Tests.FirstOrDefault(s => s.ID_Test == test.ID_Test);
            if (test != null)
            {
                test1.Name_Test = test.Name_Test;
                test1.Paragraph = test.Paragraph;
                test1.Video = test.Video;
                db.Tests.AddOrUpdate(test1);
                db.SaveChanges();
            }
            return RedirectToAction("ListTest", "Test");
        }


        public ActionResult EditQuestion(int id)
        {
            Question question = db.Questions.FirstOrDefault(s => s.ID_Question == id);
            if (question != null)
            {
                return View(question);
            }
            return RedirectToAction("ListQuestion", "Test");
        }
        [HttpPost]
        public ActionResult EditQuestion(Question question)
        {
            Question question1 = db.Questions.FirstOrDefault(s => s.ID_Question == question.ID_Question);
            if (question != null)
            {
                //question1.ID_Question = question.ID_Question;
                question1.Content = question.Content;
                question1.Type_Question = question.Type_Question;
                question1.Answer_Content1 = question.Answer_Content1;
                question1.Answer_Content2 = question.Answer_Content2;
                question1.Answer_Content3 = question.Answer_Content3;
                question1.Answer_Content4 = question.Answer_Content4;
                question1.Answer_Correct = question.Answer_Correct;
                question1.ID_Test = question.ID_Test;

                db.Questions.AddOrUpdate(question1);
                db.SaveChanges();
            }
            return RedirectToAction("ListQuestion", "Test");
        }



        public ActionResult DeleteQ(int id)
        {
            try
            {
                Test test = db.Tests.FirstOrDefault(s => s.ID_Test == id);
                if (test != null)
                {
                    db.Tests.Remove(test);
                    db.SaveChanges();
                    List<Test> tests = db.Tests.ToList();
                }
                return RedirectToAction("ListTest");
            }
            catch
            {
                //return HttpNotFound();
                return RedirectToAction("Error404", "Home");
            }
        }


        public ActionResult DeleteQuestion(int id)
        {
            try
            {
                Question questions = db.Questions.FirstOrDefault(s => s.ID_Question == id);
                if (questions != null)
                {
                    db.Questions.Remove(questions);
                    db.SaveChanges();
                    List<Question> question = db.Questions.ToList();
                }
                return RedirectToAction("ListQuestion");
            }
            catch
            {
                //return HttpNotFound();
                return RedirectToAction("Error404", "Home");
            }
        }
    }
}