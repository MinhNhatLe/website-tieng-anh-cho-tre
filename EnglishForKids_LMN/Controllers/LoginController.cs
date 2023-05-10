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
        // GET: Login

        // Hàm SignIn(): Nếu đã lưu cookie của tài khoản đăng nhập trước đó, nó sẽ tự động đăng nhập và chuyển hướng trang đến trang chủ.
        // Nếu không, nó sẽ trả về View để hiển thị form đăng nhập.
        public ActionResult SignIn()
        {
            User users = new User();
            if (Request.Cookies["Login_User"] != null && Request.Cookies["Password_User"] != null)
            {
                users.Login_User = Request.Cookies["Login_User"].Value;
                users.Password_User = Request.Cookies["Password_User"].Value;
                User users1 = db.Users.Where(s => s.Email == users.Email && s.Password_User == users.Password_User).FirstOrDefault();
                if (users1 != null)
                {
                    if (users1.IsAdmin == true)
                    {
                        Session["Admin_Code"] = users1.ID_User;
                    }
                    else
                    {
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



        //Hàm SignIn(UserBonus userz): Đây là phương thức POST được gọi khi người dùng submit form đăng nhập.
        //Nó tìm kiếm trong CSDL xem tài khoản người dùng có tồn tại hay không.
        //Nếu có, nó lưu các thông tin tài khoản vào Session và cookie (nếu người dùng chọn "Ghi nhớ tài khoản"), sau đó chuyển hướng đến trang chủ.
        //Nếu không, nó trả về View đăng nhập với thông báo lỗi "Wrong user or password".
        [HttpPost]
        public ActionResult SignIn(UserBonus userz)
        {
            User users1 = db.Users.Where(s => s.Password_User == userz.User_Password && s.Email == userz.User_Mail).FirstOrDefault();
            if (users1 == null)
            {
                ViewData["UserNotFound"] = " Wrong user or password ";
                return this.SignIn();
            }
            else
            {
                if (users1.IsAdmin == true)
                {
                    Session["Admin_Code"] = users1.ID_User;
                }
                else
                {
                    Session["ID_User"] = users1.ID_User;
                }
                Session["Name_User"] = users1.Name_User;
                Session["Image_User"] = users1.Image_User;
                if (userz.RememberMe)
                {
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





        //Sign Up
        //Hàm SignUp(): Hàm này trả về View hiển thị form đăng ký.
        public ActionResult SignUp()
        {
            return View();
        }

        //Hàm SignUp(UserBonus userz): Đây là phương thức POST được gọi khi người dùng submit form đăng ký.
        //Nó kiểm tra xem email đã tồn tại hay chưa.
        //Nếu chưa, nó tạo một đối tượng User mới trong CSDL với các thông tin người dùng đã nhập vào, sau đó chuyển hướng đến trang đăng nhập.
        //Nếu email đã tồn tại hoặc password không đúng, nó trả về View đăng ký với thông báo lỗi tương ứng.
        [HttpPost]
        public ActionResult SignUp(UserBonus userz)
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
                        userz1.IsAdmin = false;
                        userz1.Email = userz.User_Mail;
                        userz1.Image_User = "404.jpg";
                        db.Users.AddOrUpdate(userz1);
                        db.SaveChanges();
                        /*MailAddress fromGMail = new MailAddress("garena281215@gmail.com", "StartEndSchools");
                        MailAddress toGMail = new MailAddress(userz.User_Mail, "Me");
                        MailMessage Message = new MailMessage()
                        {
                            From = fromGMail,
                            Subject = "Create Account Successful",
                            Body = "Dear " + userz.User_FullName.ToString() + ",\n" + "Bạn đã đăng ký thành công\n" + "Bây giờ bạn có thể học từ vựng, xem truyện hoặc làm bài kiểm tra trên web\n" + "Chúc bạn có 1 buổi học thật ý nghĩa,\n" + "StartEnd's school",
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
                                UserName = "garena281215@gmail.com",
                                Password = "lcxehypdkkvzyzqh"
                            }
                        };
                        smtp.Send(Message);*/
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
                return this.SignUp();
            }
            catch (Exception)
            {
                //return HttpNotFound();
                return RedirectToAction("Error404", "Home");
            }
        }
        static int reCode;
        Random rd = new Random();
        public ActionResult SendVC()
        {
            return View();
        }
        static string user_mail;




        //Hàm SendVC(): Hàm này trả về View để hiển thị form gửi mã xác nhận.

        [HttpPost]
        public ActionResult SendVC(UserBonus userBonus)
        {
            User userz = db.Users.FirstOrDefault(s => s.Email == userBonus.User_Mail);
            if (userz != null)
            {
                user_mail = userBonus.User_Mail;
                MailAddress fromGMail = new MailAddress("lmnhat.englishforkids@gmail.com", "English For Kids");
                MailAddress toGMail = new MailAddress(userBonus.User_Mail, "Me");
                reCode = rd.Next(100000, 999999);
                MailMessage Message = new MailMessage()
                {
                    From = fromGMail,
                    Subject = "Đổi mật khẩu",
                    Body = "Mã của bạn là : " + reCode.ToString(),
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
                        UserName = "lmnhat.englishforkids@gmail.com",
                        Password = "Leminhnhat2001"
                    }
                };
                smtp.Send(Message);
            }
            else
            {
                ViewData["MailNotFound"] = "Gmail not found";
            }
            return this.SendVC();
        }


        //Hàm SendVC(string user_mail): Đây là phương thức POST được gọi khi người dùng submit form gửi mã xác nhận.
        //Nó sinh ra một mã xác nhận ngẫu nhiên và gửi nó đến email của người dùng, sau đó lưu mã này vào biến reCode để sử dụng trong các phương thức khác.
        //Cuối cùng, nó trả về View để hiển thị form nhập mã xác nhận.

        [HttpPost]
        public ActionResult CheckMail(UserBonus userBonus)
        {
            if (userBonus.CheckMail == reCode)
            {
                return RedirectToAction("ChangePassword", "Login");
            }
            else
            {
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
            if (userzBonus.Check_Password == userzBonus.User_Password)
            {
                if (userz != null)
                {
                    userz.Password_User = userzBonus.User_Password;
                    db.Users.AddOrUpdate(userz);
                    db.SaveChanges();
                    return RedirectToAction("SignIn", "Login");
                }
                else
                {
                    ViewData["Null"] = " Account not found ";
                }
            }
            else
            {
                ViewData["NotEqual"] = " Please enter correct infomation ";

            }
            return this.ChangePassword();
        }
    }
}