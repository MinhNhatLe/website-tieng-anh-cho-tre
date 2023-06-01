using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using EnglishForKids_LMN.Models;
using PagedList;

namespace EnglishForKids_LMN.Controllers
{
    public class StoryController : Controller
    {
        // GET: Story
        English_LearningEntities db = new English_LearningEntities();
        public ActionResult ListStory(int? page, string searchStory1, string sortStory1)
        {
            List<Story> stories = new List<Story>();
            if (searchStory1 != null)
            {
                Session["searchStory1"] = searchStory1;
                stories = db.Stories.Where(s => s.Name_Story.Contains(searchStory1.Trim().ToLower())).ToList();
            }
            else
            {
                Session["searchStory1"] = null;
                stories = db.Stories.ToList();
            }
            List<Story> stories1;
            if (sortStory1 == null || sortStory1 == "None")
            {
                Session["sortStory1"] = "None";
                stories1 = stories;
            }
            else if (sortStory1 == "AZ")
            {
                Session["sortStory1"] = "A - Z";
                stories1 = stories.OrderBy(s => s.Name_Story).ToList();
            }
            else if (sortStory1 == "ZA")
            {
                Session["sortStory"] = "Z - A";
                stories1 = stories.OrderByDescending(s => s.Name_Story).ToList();
            }
            else
            {
                Session["sortStory1"] = "View";
                stories1 = stories.OrderByDescending(s => s.View_Story).ToList();
            }
            
               List <StoryBonus> storyBonus = new List<StoryBonus>();
            foreach (Story item in stories1)
            {
                StoryBonus storyBonus1 = new StoryBonus();
                storyBonus1.ID_Story = item.ID_Story;
                storyBonus1.Image_Story = item.Image_Story;
                storyBonus1.Name_Story = item.Name_Story;
                

                User users = db.Users.FirstOrDefault(s => s.ID_User == item.ID_User);
                //storyBonus1.Name_User = users.Name_User;



                // fix lỗi 500
                if (users != null)
                {
                    storyBonus1.Name_User = users.Name_User;
                }
                else
                {
                    // Xử lý khi đối tượng users là null
                }

                /////////////////
                ///



                storyBonus.Add(storyBonus1);
            }
            if (page == null)
            {
                page = 1;
            }
            int pageSize = 9;
            int pageNum = page ?? 1;
            return View(storyBonus.ToPagedList(pageNum, pageSize));
        }
        public ActionResult CreateS()
        {
            StoryBonus story = new StoryBonus();
            return View(story);
        }
        [HttpPost]
        public ActionResult CreateS(StoryBonus story)
        {
            try
            {
                if (story != null)
                {
                    Story story1 = new Story();
                    if (story.Banner != null)
                    {
                        story1.Banner = story.Banner;
                    }
                    else
                    {
                        story1.Banner = "404.jpg";
                    }
                    if (story.Image_Story != null)
                    {
                        story1.Image_Story = story.Image_Story;
                    }
                    else
                    {
                        story1.Image_Story = "404.jpg";
                    }
                    story1.Name_Story = story.Name_Story;
                    story1.EN_Content = story.EN_Content;
                    story1.VN_Content = story.VN_Content;
                    story1.Audio = story.Audio;
                    story1.ID_User = story.ID_User;
                    story1.View_Story = 0;
                    db.Stories.Add(story1);
                    db.SaveChanges();
                }
                return RedirectToAction("ListStory", "Story");
            }
            catch
            {
                //return HttpNotFound();
                return RedirectToAction("Error404", "Home");
            }
        }
        public ActionResult EditS(int id)
        {
            Story story = db.Stories.FirstOrDefault(s => s.ID_Story == id);
            if (story != null)
            {
                StoryBonus storyBonus = new StoryBonus();
                storyBonus.ID_Story = story.ID_Story;
                storyBonus.Banner = story.Banner;
                storyBonus.Image_Story = story.Image_Story;
                storyBonus.Name_Story = story.Name_Story;
                storyBonus.EN_Content = story.EN_Content;
                storyBonus.VN_Content = story.VN_Content;
                storyBonus.Audio = story.Audio;
                storyBonus.ID_User = story.User.ID_User;
                return View(storyBonus);
            }
            return RedirectToAction("ListStory", "Story");
        }
        [HttpPost]
        public ActionResult EditS(StoryBonus storyBonus)
        {
            try
            {
                Story story = db.Stories.FirstOrDefault(s => s.ID_Story == storyBonus.ID_Story);
                if (story != null)
                {
                    story.Banner = storyBonus.Banner;
                    story.Image_Story = storyBonus.Image_Story;
                    story.Name_Story = storyBonus.Name_Story;
                    story.EN_Content = storyBonus.EN_Content;
                    story.VN_Content = storyBonus.VN_Content;
                    story.Audio = storyBonus.Audio;
                    db.Stories.AddOrUpdate(story);
                    db.SaveChanges();
                }
                return RedirectToAction("ListStory", "Story");
            }
            catch
            {
                //return HttpNotFound();
                return RedirectToAction("Error404", "Home");
            }
        }
        public ActionResult DetailS(int id)
        {
            Story story = db.Stories.FirstOrDefault(s => s.ID_Story == id);
            if (story != null)
            {
                story.View_Story += 1;
                //story.View_Story = story.View_Story;
                //db.Stories.AddOrUpdate(story);

                // Lấy lượt xem cao nhất
                //Story topStory = db.Stories.OrderByDescending(v => v.View_Story).FirstOrDefault();

                //ViewBag.TopVocabulary = topStory; // Truyền vào ViewBag để sử dụng trong View
                db.SaveChanges();
                return View(story);
            }
            return RedirectToAction("User_View", "Story");
        }
        public ActionResult DeleteS(int id)
        {
            Story story = db.Stories.FirstOrDefault(s => s.ID_Story == id);
            if (story != null)
            {
                return View(story);
            }
            return RedirectToAction("ListStory", "Story");
        }
        public ActionResult DeleteSS(int id)
        {
            try
            {
                Story story = db.Stories.FirstOrDefault(s => s.ID_Story == id);
                if (story != null)
                {
                    db.Stories.Remove(story);
                    db.SaveChanges();
                }
                return RedirectToAction("ListStory", "Story");
            }
            catch
            {
                //return HttpNotFound();
                return RedirectToAction("Error404", "Home");
            }
        }
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
        public ActionResult User_View(int? page, string searchStory, string sortStory)
        {
            List<Story> stories = new List<Story>();
            if (searchStory != null)
            {
                Session["searchStory"] = searchStory;
                stories = db.Stories.Where(s => s.Name_Story.Contains(searchStory.Trim().ToLower())).ToList();
            }
            else
            {
                Session["searchStory"] = null;
                stories = db.Stories.ToList();
            }
            List<Story> stories1;
            if (sortStory == null || sortStory == "None")
            {
                Session["sortStory"] = "None";
                stories1 = stories;
            }
            else if (sortStory == "AZ")
            {
                Session["sortStory"] = "A - Z";
                stories1 = stories.OrderBy(s => s.Name_Story).ToList();
            }
            else if (sortStory == "ZA")
            {
                Session["sortStory"] = "Z - A";
                stories1 = stories.OrderByDescending(s => s.Name_Story).ToList();
            }
            else
            {
                Session["sortStory"] = "View";
                stories1 = stories.OrderByDescending(s => s.View_Story).ToList();
            }
            if (page == null)
            {
                page = 1;
            }
            int pageSize = 9;
            int pageNum = page ?? 1;
            return View(stories1.ToPagedList(pageNum, pageSize));
        }
    }
}