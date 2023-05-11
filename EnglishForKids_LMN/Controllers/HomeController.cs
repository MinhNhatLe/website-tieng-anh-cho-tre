using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using EnglishForKids_LMN.Models;
using Newtonsoft.Json;

namespace EnglishForKids_LMN.Controllers
{

    public class HomeController : Controller
    {
        English_LearningEntities db = new English_LearningEntities();
        public ActionResult HomePage()
        {
            return View();
        }

        //chức năng xác định người dùng hiện tại và hiển thị thông tin tài khoản của họ trên trang "Account".
        public ActionResult Account()
        {
            if (Session["ID_User"] != null)
            {
                int check = int.Parse(Session["ID_User"].ToString());
                User user = db.Users.FirstOrDefault(s => s.ID_User == check);
                return View("Account", user);
            }
            else
            {
                int check = int.Parse(Session["Admin_Code"].ToString());
                User user = db.Users.FirstOrDefault(s => s.ID_User == check);
                return View("Account", user);
            }
        }
        // hàm kiểm tra tính hợp lệ của một số điện thoại
        //nhận đầu vào là một chuỗi phoneNumber kiểu string, và trả về giá trị kiểu bool.
        public bool IsValidPhoneNumber(string phoneNumber)
        {
            if (phoneNumber == null)
            {
                return false;
            }
            //Biểu thức chính quy này sử dụng ký tự "^" để bắt đầu chuỗi, và ký tự "$" để kết thúc chuỗi
            //Biểu thức chính quy này chỉ cho phép các số điện thoại có độ dài 10 chữ số, bắt đầu bằng các số sau
            string check = @"^((086(\d){7})|(096(\d){7})|(097(\d){7})|(098(\d){7})|(032(\d){7})|(033(\d){7})|(034(\d){7})|(035(\d){7})|(036(\d){7})|(037(\d){7})|(038(\d){7})|(039(\d){7})|(088(\d){7})|(091(\d){7})|(094(\d){7})|(083(\d){7})|(084(\d){7})|(085(\d){7})|(081(\d){7})|(082(\d){7})|(089(\d){7})|(090(\d){7})|(093(\d){7})|(070(\d){7})|(079(\d){7})|(077(\d){7})|(076(\d){7})|(078(\d){7})|(092(\d){7})|(056(\d){7})|(058(\d){7}))$";
            return Regex.IsMatch(phoneNumber.Trim(), check);
        }

        //Hàm này được gọi khi người dùng cập nhật thông tin tài khoản
        [HttpPost]
        public ActionResult Account(User userzBonus)
        {
            try
            {
                User users;
                if (Session["ID_User"] != null)
                {
                    int checkUser = int.Parse(Session["ID_User"].ToString());
                    users = db.Users.FirstOrDefault(s => s.ID_User == checkUser);
                }
                else if (Session["Admin_Code"] != null)
                {
                    int checkAdmin = int.Parse(Session["Admin_Code"].ToString());
                    users = db.Users.FirstOrDefault(s => s.ID_User == checkAdmin);
                }
                else
                {
                    return this.Account();
                }
                if (users != null)
                {
                    Session["Name_User"] = userzBonus.Name_User;
                    users.Name_User = userzBonus.Name_User;
                    users.Gender = userzBonus.Gender;
                    users.Birthday = DateTime.Parse(userzBonus.Birthday.ToString());
                    User user1 = db.Users.FirstOrDefault(s => s.Email == userzBonus.Email && s.ID_User != users.ID_User);
                    if (user1 == null)
                    {
                        users.Email = userzBonus.Email;
                    }
                    else
                    {
                        ViewData["GmailFound"] = "Gmail existed";
                        return this.Account();
                    }
                    if (IsValidPhoneNumber(userzBonus.Phone))
                    {
                        users.Phone = userzBonus.Phone;
                    }
                    else
                    {
                        ViewData["PhoneNumberNotValid"] = "Phone Number not valid";
                        return this.Account();
                    }
                    //Nếu người dùng tải lên hình ảnh mới, hình ảnh mới sẽ được lưu và đường dẫn đến hình ảnh sẽ được lưu trong Session để hiển thị cho người dùng sau đó.
                    //Sau khi cập nhật thông tin người dùng mới vào cơ sở dữ liệu, hàm sẽ chuyển hướng người dùng trở lại trang tài khoản của họ. Nếu có lỗi, hàm sẽ trả về trang 404.
                    if (userzBonus.Image_User != null)
                    {
                        users.Image_User = userzBonus.Image_User;
                        Session["Image_User"] = userzBonus.Image_User;
                    }
                    db.Users.AddOrUpdate(users);
                    db.SaveChanges();
                }
                return this.Account();
            }
            catch
            {
                //return HttpNotFound();
                return RedirectToAction("Error404", "Home");
            }
        }




        public ActionResult Error404()
        {
            return View();
        }
        public ActionResult Index()
        {            
            // Tổng số từ vựng
            int totalVocabularies = db.Vocabularies.Count();
            ViewBag.TotalVocabularies = totalVocabularies;

            // Tổng số thể loại từ vựng
            int totalCategory_Vo = db.Category_Vo.Count();
            ViewBag.TotalCategory_Vo = totalCategory_Vo;

            // Tổng số câu truyện
            int totalStories = db.Stories.Count();
            ViewBag.TotalStories = totalStories;

            // Tổng số câu đố
            int totalQuizs = db.Quizs.Count();
            ViewBag.TotalQuizs = totalQuizs;

            // Tổng số câu hỏi
            int totalQuestions = db.Questions.Count();
            ViewBag.TotalQuestions = totalQuestions;
            
            // Tổng số bài kiểm tra
            int totalTests = db.Tests.Count();
            ViewBag.TotalTests = totalTests;
            

            // Tổng số người chơi quiz
            int totalQuiz_Details = db.Quiz_Details.Count();
            ViewBag.TotalQuiz_Details = totalQuiz_Details;
            
            // Tổng số người test
            int totalTest_Details = db.Test_Details.Count();
            ViewBag.TotalTest_Details = totalTest_Details;

            // Tổng số người dùng
            int totalUsers = db.Users.Count();
            ViewBag.TotalUsers = totalUsers;


            // Từng thể loại
            ViewBag.UserData = JsonConvert.SerializeObject(new
            {
                labels = new[] { "Vocabulary", "Category", "Story", "Quiz", "Question", "Test"},
                data = new[] { ViewBag.TotalVocabularies, ViewBag.TotalCategory_Vo, ViewBag.TotalStories, ViewBag.TotalQuizs, ViewBag.TotalQuestions, ViewBag.TotalTests}
            });

            // Người dùng
            ViewBag.UserData1 = JsonConvert.SerializeObject(new
            {
                labels = new[] { "Quiz", "Test", "Account" },
                data = new[] { ViewBag.TotalQuiz_Details, ViewBag.TotalTest_Details, ViewBag.TotalUsers}
            });

            //Tất cả
            ViewBag.UserData2 = JsonConvert.SerializeObject(new
            {
                labels = new[] { "Vocabulary", "Category", "Story", "Quiz", "Question", "Test", "Quiz", "Test", "Account" },
                data = new[] { ViewBag.TotalVocabularies, ViewBag.TotalCategory_Vo, ViewBag.TotalStories, ViewBag.TotalQuizs, ViewBag.TotalQuestions, ViewBag.TotalTests, ViewBag.TotalQuiz_Details, ViewBag.TotalTest_Details, ViewBag.TotalUsers }
            });

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        //Đoạn code này thực hiện việc đăng xuất người dùng khỏi hệ thống.
        //Nó xóa tất cả các phiên đăng nhập và cài đặt lại các cookie "Login_User" và "Password_User" với thời gian hết hạn là -1 tháng.
        //Sau đó, nó chuyển hướng người dùng đến trang đăng nhập.
        public ActionResult Logout()
        {
            Session.RemoveAll();
            if (Response.Cookies["Login_User"] != null && Response.Cookies["Password_User"] != null)
            {
                HttpCookie httpCookie = new HttpCookie("Login_User");
                httpCookie.Expires = DateTime.Now.AddMonths(-1);
                Response.Cookies.Add(httpCookie);
                HttpCookie httpCookie1 = new HttpCookie("Password_User");
                httpCookie1.Expires = DateTime.Now.AddMonths(-1);
                Response.Cookies.Add(httpCookie1);
            }
            return RedirectToAction("SignIn", "Login");
        }
        //phương thức dùng để xử lý upload file ảnh lên server.
        //, phương thức kiểm tra xem tham số đầu vào có phải là một file hợp lệ hay không bằng cách kiểm tra xem giá trị của biến file có phải là null hay không
        //Nếu file không phải là null, phương thức sẽ lưu file ảnh lên server bằng cách sử dụng phương thức SaveAs của đối tượng HttpPostedFileBase.
        //đường dẫn để lưu file ảnh được xác định bằng cách sử dụng phương thức MapPath của đối tượng Server.
        public string ProcessUpload(HttpPostedFileBase file)
        {
            if (file == null)
            {
                return "";
            }
            else
            {
                file.SaveAs(Server.MapPath("~/Content/images/" + file.FileName));
            }
            return file.FileName;
        }

    }
}