using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

using EnglishForKids_LMN.Models;

namespace EnglishForKids_LMN.Controllers
{
    public class LoginController : Controller
    {
        English_LearningEntities db = new English_LearningEntities();
        public ActionResult SignIn()
        {
            // User tên lớp
            // users là tên biến tham chiếu đến đối tượng lớp User
            User users = new User();

            // Kiểm tra xem các cookies Login_User và Password_User có tồn tại trong request không?
            if (Request.Cookies["Login_User"] != null && Request.Cookies["Password_User"] != null)
            {
                // Gán giá trị của cookies Login_User Password_User cho đối tượng users
                users.Login_User = Request.Cookies["Login_User"].Value;
                users.Password_User = Request.Cookies["Password_User"].Value;

                // Truy vấn dữ liệu
                // Kiểm tra xem có khớp Email và Password không
                // FirstOrDefault lấy người dùng đầu tiên thõa mản điều kiện
                User users1 = db.Users.Where(s => s.Email == users.Email && s.Password_User == users.Password_User).FirstOrDefault();
                if (users1 != null)
                {
                    if (users1.IsAdmin == true)
                    {
                        // Nếu là admin thì lưu vào Session Admin_code
                        Session["Admin_Code"] = users1.ID_User;
                    }
                    else
                    {
                        // Nếu là user thì lưu vào Session User
                        Session["ID_User"] = users1.ID_User;
                    }
                    Session["Image_User"] = users1.Image_User;
                    Session["Name_User"] = users1.Name_User;
                    return RedirectToAction("HomePage", "Home");
                }
                else
                {
                    return View();
                }
            }
            else
            {
                return View();
            }
        }
        [HttpPost]
        public ActionResult SignIn(UserBonus userz)
        {
            // Kiểm tra xem có trùng dữ liệu không
            User users1 = db.Users.Where(s => s.Password_User == userz.User_Password && s.Email == userz.User_Mail).FirstOrDefault();
            
            // Nếu không nhập thì báo lỗi
            if (users1 == null)
            {
                ViewData["UserNotFound"] = " Please enter the correct account and password ";
                return this.SignIn();
            }
            else
            {
                // Kiểm tra nếu là Admin thì vào Admin
                if (users1.IsAdmin == true)
                {
                    Session["Admin_Code"] = users1.ID_User;
                }
                // Nếu không thì vào user
                else
                {
                    Session["ID_User"] = users1.ID_User;
                }
                // Gán Name và Image vào session
                Session["Name_User"] = users1.Name_User;
                Session["Image_User"] = users1.Image_User;


                if (userz.RememberMe)
                {
                    // Khởi tạo HttpCookie để lưu giá trị Login_User cho người dùng
                    // Expires lưu giá trị 1 ngày
                    // Gán giá trị vào http cookie
                    // Cuối cùng là add vào
                    HttpCookie httpCookie = new HttpCookie("Login_User");                    
                    httpCookie.Expires = DateTime.Now.AddDays(1d);
                    httpCookie.Value = userz.User_Mail;
                    Response.Cookies.Add(httpCookie);

                    HttpCookie httpCookie1 = new HttpCookie("User_Password");
                    httpCookie1.Expires = DateTime.Now.AddDays(1d);
                    httpCookie1.Value = userz.User_Password;
                    Response.Cookies.Add(httpCookie1);
                }
                return RedirectToAction("HomePage", "Home");
            }
        }






        public ActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SignUp(UserBonus userz)
        {
            try
            {
                // Truy vấn CSDL và lấy ra 
                User users2 = db.Users.FirstOrDefault(s => s.Email == userz.User_Mail);

                // Tạo account mới
                if (users2 == null)
                {
                    // Kiểm tra Password và Check_Password phải giống nhau
                    if (userz.User_Password == userz.Check_Password)
                    {
                        // Tạo đối tượng user mới
                        User userz1 = new User();
                        userz1.Name_User = userz.User_FullName;
                        userz1.Password_User = userz.User_Password;
                        // Quyết định xem account nào là admin hay user
                        userz1.IsAdmin = false;
                        userz1.Email = userz.User_Mail;
                        // Cho img mặc định vừa tạo là ảnh 404
                        userz1.Image_User = "404.jpg";
                        db.Users.AddOrUpdate(userz1);
                        db.SaveChanges();


                        // Chức năng thông báo email vừa tạo xong
                        MailAddress fromGMail = new MailAddress("lmnnhat.work@gmail.com", "English For Kids - CEO Lê Minh Nhật");
                        MailAddress toGMail = new MailAddress(userz.User_Mail, "Me");
                        MailMessage Message = new MailMessage()
                        {
                            From = fromGMail,
                            Subject = "Create account successful",
                            Body = "Dear " + userz.User_FullName.ToString() + ",\n" + "You have successfully registered\n" + "Now you can learn vocabulary, read stories and listen to stories as you please, take tests to improve your intelligence. In addition, there are other extremely cool and interesting functions.\n" + "Wish you have a meaningful lesson,\n" + "English For Kids - CEO Lê Minh Nhật",
                            Priority = MailPriority.High,
                            IsBodyHtml = false
                        };
                        Message.To.Add(toGMail);
                        SmtpClient smtp = new SmtpClient()
                        {
                            Host = "smtp.gmail.com",
                            Port = 587,
                            EnableSsl = true,
                            DeliveryMethod = SmtpDeliveryMethod.Network,
                            UseDefaultCredentials = false,
                            Credentials = new NetworkCredential()
                            {
                                UserName = "lmnhat.work@gmail.com",
                                Password = "drhvuzjzhkoizsch"
                            }
                        };
                        smtp.Send(Message);
                        return RedirectToAction("SignIn", "Login");
                    }
                    // Nếu nhập sai thì nó sẽ báo lỗi
                    else
                    {
                        ViewData["WrongPassword"] = " Please enter the correct password ";
                    }
                }
                else
                // nếu user đó đã có rồi thì tồn tại
                {
                    ViewData["UserExisted"] = " Gmail existed ";
                }
                return this.SignUp();
            }
            catch (Exception)
            {
                //return HttpNotFound();
                return RedirectToAction("Error404", "Home");
            }
        }


        public ActionResult SignUpAdmin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SignUpAdmin(UserBonus userz)
        {
            try
            {
                User users2 = db.Users.FirstOrDefault(s => s.Email == userz.User_Mail);
                if (users2 == null)
                {
                    if (userz.User_Password == userz.Check_Password)
                    {
                        User userz1 = new User();
                        userz1.Name_User = userz.User_FullName;
                        userz1.Password_User = userz.User_Password;
                        userz1.IsAdmin = true;
                        userz1.Email = userz.User_Mail;
                        userz1.Image_User = "404.jpg";
                        db.Users.AddOrUpdate(userz1);
                        db.SaveChanges();
                        MailAddress fromGMail = new MailAddress("lmnnhat.work@gmail.com", "English For Kids - CEO Lê Minh Nhật");
                        MailAddress toGMail = new MailAddress(userz.User_Mail, "Me");
                        MailMessage Message = new MailMessage()
                        {
                            From = fromGMail,
                            Subject = "Create Account Successful",
                            Body = "Dear " + userz.User_FullName.ToString() + ",\n" + "You have successfully registered\n" + "You are now the admin of the English For Kids website! Let's run the website and manage students well together with us!\n" + "Have a nice day,\n" + "English For Kids - CEO Lê Minh Nhật",
                            Priority = MailPriority.High,
                            IsBodyHtml = false
                        };
                        Message.To.Add(toGMail);
                        SmtpClient smtp = new SmtpClient()
                        {
                            Host = "smtp.gmail.com",
                            Port = 587,
                            EnableSsl = true,
                            DeliveryMethod = SmtpDeliveryMethod.Network,
                            UseDefaultCredentials = false,
                            Credentials = new NetworkCredential()
                            {
                                UserName = "lmnhat.work@gmail.com",
                                Password = "drhvuzjzhkoizsch"
                            }
                        };
                        smtp.Send(Message);
                        return RedirectToAction("SignIn", "Login");
                    }
                    else
                    {
                        ViewData["WrongPassword"] = " Please enter the correct password ";
                    }
                }
                else
                {
                    ViewData["UserExisted"] = " Gmail existed ";
                }
                return this.SignUpAdmin();
            }
            catch (Exception)
            {
                //return HttpNotFound();
                return RedirectToAction("Error404", "Home");
            }
        }




        // reCode là một biến static kiểu số nguyên. Biến này được sử dụng để lưu trữ mã xác nhận.
        static int reCode;
        // rd là một đối tượng của lớp Random, được sử dụng để tạo số ngẫu nhiên.
        Random rd = new Random();
        public ActionResult SendVC()
        {
            return View();
        }
        //user_mail là một biến static kiểu chuỗi. Biến này được sử dụng để lưu trữ địa chỉ email của người dùng.
        static string user_mail;

        [HttpPost]
        public ActionResult SendVC(UserBonus userBonus)
        {
            User userz = db.Users.FirstOrDefault(s => s.Email == userBonus.User_Mail);
            // Nếu user đó không phải null
            if (userz != null)
            {
                // Lấy cái User_mail của userBonus lưu vào user_mail của cái static string
                user_mail = userBonus.User_Mail;
                MailAddress fromGMail = new MailAddress("lmnhat.work@gmail.com", "English For Kids - CEO Lê Minh Nhật");
                MailAddress toGMail = new MailAddress(userBonus.User_Mail, "Me");
                // Ramdom 6 số ngẫu nhiên
                reCode = rd.Next(100000, 999999);
                MailMessage Message = new MailMessage()
                {
                    From = fromGMail,
                    Subject = "Change password",
                    Body = "Your code is: " + reCode.ToString(),
                    Priority = MailPriority.High,
                    IsBodyHtml = false
                };
                Message.To.Add(toGMail);
                SmtpClient smtp = new SmtpClient()
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential()
                    {
                        UserName = "lmnhat.work@gmail.com",
                        Password = "drhvuzjzhkoizsch"
                    }
                };
                smtp.Send(Message);
            }
            else
            // Nếu usez đó không có thì báo lỗi
            {
                ViewData["MailNotFound"] = "Gmail not found";
            }
            return this.SendVC();
        }

        // CheckMail của SendVC
        [HttpPost]
        public ActionResult CheckMail(UserBonus userBonus)
        {
            if (userBonus.CheckMail == reCode)
            {
                // Nếu đúng thì chuyển hướng sang trang ChangePassword
                return RedirectToAction("ChangePassword", "Login");
            }
            else
            {
                // Nếu sai thì thông báo
                ViewData["WrongVC"] = " Wrong verification code";
                return this.SendVC();
            }
        }


        public ActionResult ChangePassword()
        {
            UserBonus userBonus = new UserBonus();
            userBonus.User_Mail = user_mail;
            return View(userBonus);
        }

        [HttpPost]
        public ActionResult ChangePassword(UserBonus userzBonus)
        {
            User userz = db.Users.FirstOrDefault(s => s.Email == userzBonus.User_Mail);

            // Kiểm tra User_Password và Check_Password phải giống nhau
            if (userzBonus.Check_Password == userzBonus.User_Password)
            {
                // Nếu user tồn tại
                if (userz != null)
                {
                    // cập nhật lại password
                    userz.Password_User = userzBonus.User_Password;
                    db.Users.AddOrUpdate(userz);
                    db.SaveChanges();
                    return RedirectToAction("SignIn", "Login");
                }
                else
                // nếu user không có
                {
                    ViewData["Null"] = " Account not found ";
                }
            }
            // Kiểm tra User_Password và Check_Password không phải giống nhau thì báo lỗi

            else
            {
                ViewData["NotEqual"] = " Please enter correct infomation ";

            }
            return this.ChangePassword();
        }
    }
}