using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Speech.Synthesis;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

using EnglishForKids_LMN.Models;
using PagedList;

namespace EnglishForKids_LMN.Controllers
{
    public class VocabularyController : Controller
    {
        English_LearningEntities db = new English_LearningEntities();
        public ActionResult ListVocabularyType()
        {
            // Lấy danh sách thể loại từ vựng ra từ cái ID số 6 ra
            List<Category_Vo> vocabulary_Type = db.Category_Vo.Where(s => s.ID_Category_Vo != 6).ToList();
            Category_VocabularyBonus vocabular_TypeBonus = new Category_VocabularyBonus();
            // Danh sách thể loại từ vựng
            vocabular_TypeBonus.vocabulary_Types = vocabulary_Type;
            return View(vocabular_TypeBonus);
        }
        public ActionResult DeleteVT(int id)
        {
            try
            {
                // Tìm đối tượng để xóa
                Category_Vo vocabulary_Type = db.Category_Vo.FirstOrDefault(s => s.ID_Category_Vo == id);
                // nếu đối tượng đó tồn tại
                if (vocabulary_Type != null)
                {
                    // Dùng thuộc tính remove để xóa
                    db.Category_Vo.Remove(vocabulary_Type);
                    db.SaveChanges();
                    // Xóa trong trả lại danh sách
                    List<Category_Vo> vocabulary_Type1 = db.Category_Vo.ToList();
                }
                return RedirectToAction("ListVocabularyType");
            }
            catch
            {
                //return HttpNotFound();
                return RedirectToAction("Error404", "Home");
            }
        }
        public ActionResult EditVT(int id)
        {
            // Tìm đối tượng để cập nhật hiện lên ô input
            Category_Vo vocabulary_Type = db.Category_Vo.FirstOrDefault(s => s.ID_Category_Vo == id);
            Category_VocabularyBonus vocabular_TypeBonus = new Category_VocabularyBonus();
            // Gán giá trị vào để sửa
            vocabular_TypeBonus.ID_Category_Vo = vocabulary_Type.ID_Category_Vo;
            vocabular_TypeBonus.Name_Category_Vo = vocabulary_Type.Name_Category_Vo;
            return View(vocabular_TypeBonus);
        }
        [HttpPost]
        public ActionResult EditVT(VocabularyBonus vocabulary_TypeBonus)
        {
            // tìm đối tượng cập nhật
            Category_Vo vocabulary_Type2 = db.Category_Vo.FirstOrDefault(s => s.Name_Category_Vo == vocabulary_TypeBonus.Name_Category_Vo);
            Category_Vo vocabulary_Type1 = db.Category_Vo.FirstOrDefault(s => s.ID_Category_Vo == vocabulary_TypeBonus.ID_Category_Vo);
            try
            {
                // Nếu thể loại từ vựng là null
                // Cho phép đổi tên
                // Tức là cho mọi thể loại là null
                // Nói chung cái thể loại từ vựng đang bị lỗi
                if (vocabulary_Type2 == null)
                {
                    vocabulary_Type1.Name_Category_Vo = vocabulary_TypeBonus.Name_Category_Vo;
                    db.Category_Vo.AddOrUpdate(vocabulary_Type1);
                    db.SaveChanges();
                    return RedirectToAction("ListVocabularyType", "Vocabulary");
                }
                // Nếu đặt trùng tên thị sẽ như thế này
                else if (vocabulary_Type2 != null)
                {
                    db.Category_Vo.AddOrUpdate(vocabulary_Type1);
                    db.SaveChanges();
                    return RedirectToAction("ListVocabularyType", "Vocabulary");
                }
                else
                {
                    // Cái này không hoạt động ?? 
                    ViewData["existed"] = "Vocabulary type name existed";
                    return View(vocabulary_TypeBonus);
                }
            }
            catch
            {
                //return HttpNotFound();
                return RedirectToAction("Error404", "Home");
            }
        }
        [HttpPost]
        public ActionResult CreateVT(VocabularyBonus vocabular_TypeBonus)
        {
            Category_Vo vocabulary_Type2 = db.Category_Vo.FirstOrDefault(s => s.Name_Category_Vo == vocabular_TypeBonus.Name_Category_Vo);
            try
            {
                // nếu add tên từ vựng đó có rồi thì báo lỗi
                if (vocabular_TypeBonus != null)
                {
                    // Nếu tựng đó chưa có thì add vào
                    if (vocabulary_Type2 == null)
                    {
                        Category_Vo vocabulary_Type1 = new Category_Vo();
                        vocabulary_Type1.Name_Category_Vo = vocabular_TypeBonus.Name_Category_Vo;
                        db.Category_Vo.Add(vocabulary_Type1);
                        db.SaveChanges();
                        return RedirectToAction("ListVocabularyType", "Vocabulary");
                    }
                    else
                    {
                        return View(vocabular_TypeBonus);
                    }
                }
                else
                {
                    return View();
                }
            }
            catch
            {
                //return HttpNotFound();
                return RedirectToAction("Error404", "Home");
            }

        }









        public ActionResult ListVocabulary(int? page, string searchVocabulary1, string sortVocabulary1)
        {
            // Lấy danh sách từ vựng ra
            List<Vocabulary> vocabularies = new List<Vocabulary>();
            // Nếu tìm kiếm không để trống 
            if (searchVocabulary1 != null)
            {
                // lưu cái tìm kiếm đó vào session
                Session["searchVocabulary1"] = searchVocabulary1;
                // Lấy dữ liệu ra 
                // Tìm kiếm theo EN_Meaning
                // ToLower tất cả là chữ nhỏ
                vocabularies = db.Vocabularies.Where(s => s.EN_Meaning.Contains(searchVocabulary1.Trim().ToLower())).ToList();
            }
            else
            {
                Session["searchVocabulary1"] = null;
                vocabularies = db.Vocabularies.ToList();
            }
            

            // Sắp xếp nè
            List<Vocabulary> vocabularies1;
            // Sắp xếp không có gì
            if (sortVocabulary1 == null || sortVocabulary1 == "None")
            {
                Session["Vocabulary_Sort1"] = "None";
                vocabularies1 = vocabularies;
            }
            // Sắp xếp theo từ a -> z
            else if (sortVocabulary1 == "AZ")
            {
                // Lưu vào session
                Session["Vocabulary_Sort1"] = "A - Z";
                // Tìm kiếm theo En_Meaning
                // Dùng OderBy sắp xếp theo a -> z
                vocabularies1 = vocabularies.OrderBy(s => s.EN_Meaning).ToList();
            }
            else
            {
                Session["Vocabulary_Sort1"] = "Z - A";
                vocabularies1 = vocabularies.OrderByDescending(s => s.EN_Meaning).ToList();
            }
            // Nếu tìm kiếm không có gì thì xuất hiện 1 trang
            if (page == null)
            {
                page = 1;
            }
            // 1 trang chưa 9 items
            int pageSize = 9;
            int pageNum = page ?? 1;
            return View(vocabularies1.ToPagedList(pageNum, pageSize));
        }
        public ActionResult DetailV(int id)
        {
            Vocabulary vocabulary = db.Vocabularies.FirstOrDefault(s => s.ID_Vocabulary == id);
            // Nếu từ vựng đó tồn tại
            if (vocabulary != null)
            {
                // tăng view lên nè
                vocabulary.View_Vocabulary += 1;
                db.SaveChanges();
                return View(vocabulary);
            }

            return RedirectToAction("User_View", "Vocabulary");
        }

        // sử dụng bất động bộ async await
        public async Task<ActionResult> Speech(int id)
        {
            // Tím kiếm ID nó ra
            Vocabulary vocabulary = db.Vocabularies.FirstOrDefault(s => s.ID_Vocabulary == id);
            Task<RedirectToRouteResult> viewResult = Task.Run(() =>
            {
                // Sử dụng SpeechSynthesizer
                using (SpeechSynthesizer sp = new SpeechSynthesizer())
                {
                    // Chọn giọng đọc với giới tính là nữ và độ tuổi là trẻ
                    //sp.SelectVoiceByHints(VoiceGender.Male, VoiceAge.Child);
                    sp.SelectVoice("Microsoft Zira Desktop");
                    sp.SetOutputToDefaultAudioDevice();
                    // phát âm cái từ vựng của thuộc tính EN_Meaning
                    sp.Speak(vocabulary.EN_Meaning);
                    // sau khi phát âm xong redirection tại trang cũ
                    return RedirectToAction("DetailV/" + id);
                }
            });
            return await viewResult;
        }


        public ActionResult DeleteV(int id)
        {
            Vocabulary vocabulary = db.Vocabularies.FirstOrDefault(s => s.ID_Vocabulary == id);
            return View(vocabulary);
        }
        public ActionResult DeleteVC(int id)
        {
            try
            {
                // tìm ID muốn xóa
                Vocabulary vocabulary = db.Vocabularies.FirstOrDefault(s => s.ID_Vocabulary == id);
                // Xóa những từ vựng tồn tại
                if (vocabulary != null)
                {
                    // sử dụng Remove để xóa
                    db.Vocabularies.Remove(vocabulary);
                    db.SaveChanges();
                }
                return RedirectToAction("ListVocabulary");
            }
            catch
            {
                //return HttpNotFound();
                return RedirectToAction("Error404", "Home");
            }
        }
        public ActionResult CreateV()
        {
            // Lấy thể loại từ vựng ra
            List<Category_Vo> vocabulary_Types = db.Category_Vo.ToList();
            VocabularyBonus vocabularyBonus = new VocabularyBonus();
            vocabularyBonus.category_vos = vocabulary_Types;
            return View(vocabularyBonus);
        }
        [HttpPost]
        public ActionResult CreateV(VocabularyBonus vocabularyBonus)
        {
            try
            {
                if (vocabularyBonus != null)
                {
                    Vocabulary vocabulary = new Vocabulary();
                    vocabulary.ID_Cate_Vo = vocabularyBonus.ID_Category_Vo;
                    vocabulary.EN_Meaning = vocabularyBonus.EN_Meaning;
                    vocabulary.VN_Meaning = vocabularyBonus.VN_Meaning;
                    vocabulary.Pronunciation = vocabularyBonus.Pronunciation;
                    vocabulary.View_Vocabulary = 0;
                    // Nếu không mốn null thì add Ảnh vào
                    if (vocabularyBonus.Image_Vocabulary != null)
                    {
                        vocabulary.Image_Vocabulary = vocabularyBonus.Image_Vocabulary;
                    }
                    else
                    // Không muốn add ảnh vào thì ra ảnh 404
                    {
                        vocabulary.Image_Vocabulary = "404.jpg";

                    }
                    db.Vocabularies.Add(vocabulary);
                    db.SaveChanges();
                    return RedirectToAction("ListVocabulary", "Vocabulary");
                }
                return this.CreateV();
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
                // Lấy hình ở đâu cũng được
                // Nhưng lấy xong nó sẽ lưu vào Server theo đường dẫn này
                file.SaveAs(Server.MapPath("~/Content/images/" + file.FileName));
            }
            return file.FileName;
        }
        public ActionResult EditV(int id)
        {
            // Lấy cái ID muốn cập nhật ra
            Vocabulary vocabulary = db.Vocabularies.FirstOrDefault(s => s.ID_Vocabulary == id);
            // Lấy đối tượng không phải null
            if (vocabulary != null)
            {
                VocabularyBonus vocabularyBonus = new VocabularyBonus();
                vocabularyBonus.ID_Vocabulary = vocabulary.ID_Vocabulary;
                vocabularyBonus.Image_Vocabulary = vocabulary.Image_Vocabulary;
                vocabularyBonus.EN_Meaning = vocabulary.EN_Meaning;
                vocabularyBonus.VN_Meaning = vocabulary.VN_Meaning;
                vocabularyBonus.Pronunciation = vocabulary.Pronunciation;
                vocabularyBonus.ID_Category_Vo = vocabulary.Category_Vo.ID_Category_Vo;

                // Lấy danh sách thể loại từ vựng đố ra
                List<Category_Vo> vocabulary_Types = db.Category_Vo.ToList();
                vocabularyBonus.category_vos = vocabulary_Types;
                return View(vocabularyBonus);
            }
            else
            {
                return RedirectToAction("ListVocabulary", "Vocabulary");
            }
        }
        [HttpPost]
        public ActionResult EditV(VocabularyBonus vocabularyBonus)
        {
            try
            {
                if (vocabularyBonus != null)
                {
                    Vocabulary vocabulary = db.Vocabularies.FirstOrDefault(s => s.ID_Vocabulary == vocabularyBonus.ID_Vocabulary);
                    vocabulary.ID_Cate_Vo = vocabularyBonus.ID_Category_Vo;
                    vocabulary.EN_Meaning = vocabularyBonus.EN_Meaning;
                    vocabulary.VN_Meaning = vocabularyBonus.VN_Meaning;
                    vocabulary.Pronunciation = vocabularyBonus.Pronunciation;
                    vocabulary.Image_Vocabulary = vocabularyBonus.Image_Vocabulary;
                    db.Vocabularies.AddOrUpdate(vocabulary);
                    db.SaveChanges();
                    return RedirectToAction("ListVocabulary", "Vocabulary");
                }
                return View(vocabularyBonus);
            }
            catch
            {
                //return HttpNotFound();
                return RedirectToAction("Error404", "Home");
            }
        }
        public ActionResult User_View(int? id, int? page, string searchVocabulary, string sortVocabulary)
        {
            // Lấy danh sách thể loại ra trừ cái thể loại thứ 6
            List<Category_Vo> vocabulary_Types = db.Category_Vo.Where(s => s.ID_Category_Vo != 6).ToList();
            // Lưu vào session
            Session["Index"] = vocabulary_Types;

            // Lấy danh sách từ vựng ra
            List<Vocabulary> vocabularies = new List<Vocabulary>();
            // tìm kiếm 
            if (searchVocabulary != null)
            {
                Session["searchVocabulary"] = searchVocabulary;
                vocabularies = db.Vocabularies.Where(s => s.EN_Meaning.Contains(searchVocabulary.Trim().ToLower())).ToList();
            }
            else
            {
                Session["searchVocabulary"] = null;
                if (id == null)
                {
                    vocabularies = db.Vocabularies.ToList();
                }
                if (id != null)
                {
                    Category_Vo vocabulary_Type = db.Category_Vo.FirstOrDefault(s => s.ID_Category_Vo == id);
                    vocabularies = db.Vocabularies.Where(s => s.ID_Cate_Vo == vocabulary_Type.ID_Category_Vo).ToList();
                }
            }

            // Lấy danh sách ra
            List<Vocabulary> vocabularies1;
            if (sortVocabulary == null || sortVocabulary == "None")
            {
                Session["Vocabulary_Sort"] = "None";
                vocabularies1 = vocabularies;
            }
            else if (sortVocabulary == "AZ")
            {
                Session["Vocabulary_Sort"] = "A - Z";
                vocabularies1 = vocabularies.OrderBy(s => s.EN_Meaning).ToList();
            }
            else
            {
                Session["Vocabulary_Sort"] = "Z - A";
                vocabularies1 = vocabularies.OrderByDescending(s => s.EN_Meaning).ToList();
            }
            if (page == null)
            {
                page = 1;
            }
            int pageSize = 9;
            int pageNum = page ?? 1;
            return View(vocabularies1.ToPagedList(pageNum, pageSize));
        }


    }
}