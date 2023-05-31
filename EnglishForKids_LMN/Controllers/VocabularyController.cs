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
        // GET: Vocabulary
        //chức năng lấy danh sách các loại từ vựng từ cơ sở dữ liệu và hiển thị lên trang web.
        public ActionResult ListVocabularyType()
        {
            List<Category_Vo> vocabulary_Type = db.Category_Vo.Where(s => s.ID_Category_Vo != 6).ToList();
            Category_VocabularyBonus vocabular_TypeBonus = new Category_VocabularyBonus();
            vocabular_TypeBonus.vocabulary_Types = vocabulary_Type;




            return View(vocabular_TypeBonus);
        }
        // sử dụng id chuyền vào tìm kiếm Category_Vo trong cơ sở dữ liệu
        //Nếu đối tượng được tìm thấy, nó sẽ bị xóa khỏi cơ sở dữ liệu và sau đó danh sách Category_Vo được cập nhật
        public ActionResult DeleteVT(int id)
        {
            try
            {
                Category_Vo vocabulary_Type = db.Category_Vo.FirstOrDefault(s => s.ID_Category_Vo == id);
                if (vocabulary_Type != null)
                {
                    db.Category_Vo.Remove(vocabulary_Type);
                    db.SaveChanges();
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
        // lấy ra thông tin loại từ vựng (vocabulary type) có ID tương ứng từ cơ sở dữ liệu, và trả về một view cho phép người dùng chỉnh sửa thông tin này.
        public ActionResult EditVT(int id)
        {
            Category_Vo vocabulary_Type = db.Category_Vo.FirstOrDefault(s => s.ID_Category_Vo == id);
            Category_VocabularyBonus vocabular_TypeBonus = new Category_VocabularyBonus();
            vocabular_TypeBonus.ID_Category_Vo = vocabulary_Type.ID_Category_Vo;
            vocabular_TypeBonus.Name_Category_Vo = vocabulary_Type.Name_Category_Vo;
            return View(vocabular_TypeBonus);
        }
        [HttpPost]
        public ActionResult EditVT(VocabularyBonus vocabulary_TypeBonus)
        {
            Category_Vo vocabulary_Type2 = db.Category_Vo.FirstOrDefault(s => s.Name_Category_Vo == vocabulary_TypeBonus.Name_Category_Vo);
            Category_Vo vocabulary_Type1 = db.Category_Vo.FirstOrDefault(s => s.ID_Category_Vo == vocabulary_TypeBonus.ID_Category_Vo);
            try
            {
                if (vocabulary_Type2 == null)
                {
                    vocabulary_Type1.Name_Category_Vo = vocabulary_TypeBonus.Name_Category_Vo;
                    db.Category_Vo.AddOrUpdate(vocabulary_Type1);
                    db.SaveChanges();
                    return RedirectToAction("ListVocabularyType", "Vocabulary");
                }
                else
                {
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
                if (vocabular_TypeBonus != null)
                {
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
            List<Vocabulary> vocabularies = new List<Vocabulary>();
            if (searchVocabulary1 != null)
            {
                Session["searchVocabulary1"] = searchVocabulary1;
                vocabularies = db.Vocabularies.Where(s => s.EN_Meaning.Contains(searchVocabulary1.Trim().ToLower())).ToList();
            }
            else
            {
                Session["searchVocabulary1"] = null;
                vocabularies = db.Vocabularies.ToList();
            }
            List<Vocabulary> vocabularies1;
            if (sortVocabulary1 == null || sortVocabulary1 == "None")
            {
                Session["Vocabulary_Sort1"] = "None";
                vocabularies1 = vocabularies;
            }
            else if (sortVocabulary1 == "AZ")
            {
                Session["Vocabulary_Sort1"] = "A - Z";
                vocabularies1 = vocabularies.OrderBy(s => s.EN_Meaning).ToList();
            }
            else
            {
                Session["Vocabulary_Sort1"] = "Z - A";
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
        public ActionResult DetailV(int id)
        {
            Vocabulary vocabulary = db.Vocabularies.FirstOrDefault(s => s.ID_Vocabulary == id);
            return View(vocabulary);
        }
        //chức năng để tổng hợp giọng nói của từ vựng tiếng Anh. 
        //chương trình sử dụng đối tượng 'SpeechSynthesizer' để tổng hợp giọng nói
        //Cụ thể, nó lấy nghĩa tiếng Anh của từ vựng đó từ cơ sở dữ liệu và sử dụng phương thức 'Speak' để phát âm.
        // Chức năng này được viết dưới dạng phương thức bất đồng bộ ('async') và trả về một đối tượng 'Task<ActionResult>'.
        public async Task<ActionResult> Speech(int id)
        {
            Vocabulary vocabulary = db.Vocabularies.FirstOrDefault(s => s.ID_Vocabulary == id);
            Task<RedirectToRouteResult> viewResult = Task.Run(() =>
            {
                using (SpeechSynthesizer sp = new SpeechSynthesizer())
                {
                    // Chọn giọng đọc với giới tính là nữ và độ tuổi là trẻ
                    //sp.SelectVoiceByHints(VoiceGender.Male, VoiceAge.Child);
                    sp.SelectVoice("Microsoft Zira Desktop");
                    sp.SetOutputToDefaultAudioDevice();
                    sp.Speak(vocabulary.EN_Meaning);
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
                Vocabulary vocabulary = db.Vocabularies.FirstOrDefault(s => s.ID_Vocabulary == id);
                if (vocabulary != null)
                {
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
                    if (vocabularyBonus.Image_Vocabulary != null)
                    {
                        vocabulary.Image_Vocabulary = vocabularyBonus.Image_Vocabulary;
                    }
                    else
                    {
                        //vocabulary.Image_Vocabulary = "1478594.png";
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
        //phương thức (method) có tên là ProcessUpload và nhận đối số đầu vào là một đối tượng HttpPostedFileBase.
        //kiểm tra xem đối tượng file có tồn tại hay không. Nếu không tồn tại, phương thức sẽ trả về một chuỗi rỗng.
        //Nếu đối tượng file tồn tại, phương thức sẽ lưu trữ tệp tin đó lên server bằng cách sử dụng phương thức SaveAs của đối tượng file. Sau đó, phương thức trả về tên tệp tin đã được tải lên.
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
        public ActionResult EditV(int id)
        {
            Vocabulary vocabulary = db.Vocabularies.FirstOrDefault(s => s.ID_Vocabulary == id);
            if (vocabulary != null)
            {
                VocabularyBonus vocabularyBonus = new VocabularyBonus();
                vocabularyBonus.ID_Vocabulary = vocabulary.ID_Vocabulary;
                vocabularyBonus.Image_Vocabulary = vocabulary.Image_Vocabulary;
                vocabularyBonus.EN_Meaning = vocabulary.EN_Meaning;
                vocabularyBonus.VN_Meaning = vocabulary.VN_Meaning;
                vocabularyBonus.Pronunciation = vocabulary.Pronunciation;
                vocabularyBonus.ID_Category_Vo = vocabulary.Category_Vo.ID_Category_Vo;
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
            
            List<Category_Vo> vocabulary_Types = db.Category_Vo.Where(s => s.ID_Category_Vo != 6).ToList();
            Session["Index"] = vocabulary_Types;
            List<Vocabulary> vocabularies = new List<Vocabulary>();
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
            //sử dụng thư viện PagedList để phân trang
            //pageSize được gán giá trị là 9, đây là số lượng từ vựng sẽ được hiển thị trên mỗi trang.
            int pageSize = 9;
            int pageNum = page ?? 1;
            return View(vocabularies1.ToPagedList(pageNum, pageSize));
        }

    }
}