
using EnglishForKids_LMN.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Speech.Synthesis;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace EnglishForKids_LMN.Controllers
{
    public class GoogleTranslateController : Controller
    {

        // GET: GoogleTranslate
        // Nó tạo một đối tượng Google
        // đặt giá trị cho các Session "translate" và "checklanguage" và trả về View GoogleTranslate với đối tượng Google.
        public ActionResult GoogleTranslate()
        {
            Google gg = new Google();
            Session["translate"] = "";
            Session["checklanguage"] = "";
            return View(gg);
        }

        [HttpPost]
        public ActionResult GoogleTranslate(Google gg)
        {
            string url;
            if (gg.checklanguage == "english")
            {
                url = String.Format
                ("https://translate.googleapis.com/translate_a/single?client=gtx&sl={0}&tl={1}&dt=t&q={2}",
                "en", "vi", Uri.EscapeUriString(gg.content));
            }
            else
            {
                url = String.Format
                ("https://translate.googleapis.com/translate_a/single?client=gtx&sl={0}&tl={1}&dt=t&q={2}",
                "vi", "en", Uri.EscapeUriString(gg.content));
            }
            HttpClient httpClient = new HttpClient();
            string result = httpClient.GetStringAsync(url).Result;
            var jsonData = new JavaScriptSerializer().Deserialize<List<dynamic>>(result);
            var translationItems = jsonData[0];
            string translation = "";
            foreach (object item in translationItems)
            {
                IEnumerable translationLineObject = item as IEnumerable;
                IEnumerator translationLineString = translationLineObject.GetEnumerator();
                translationLineString.MoveNext();
                translation += string.Format(" {0}", Convert.ToString(translationLineString.Current));
            }
            if (translation.Length > 1) { translation = translation.Substring(1); };
            //lưu kết quả dịch vào Session và trả về View GoogleTranslate với đối tượng Google.
            Session["checklanguage"] = gg.checklanguage;
            Session["translate"] = translation.ToString();
            return View(gg);
        }

        public async Task<ActionResult> Speech()
        {
            if (Session["translate"] != null)
            {
                Task<ViewResult> viewResult = Task.Run(() =>
                {
                    using (SpeechSynthesizer sp = new SpeechSynthesizer())
                    {
                        // Chọn giọng đọc với giới tính là nữ và độ tuổi là trẻ
                        //sp.SelectVoiceByHints(VoiceGender.Female, VoiceAge.Child);
                        sp.SelectVoice("Microsoft Zira Desktop");
                        sp.SetOutputToDefaultAudioDevice();
                        //kiểm tra xem Session "translate" có giá trị hay không, sau đó sử dụng SpeechSynthesizer để phát âm văn bản
                        sp.Speak(Session["translate"].ToString());
                        Google google = new Google();
                        google.checklanguage = Session["checklanguage"].ToString();
                        google.content = Session["translate"].ToString();
                        return View("GoogleTranslate", google);
                    }
                });
                return await viewResult;
            }
            else
            {
                return this.GoogleTranslate();
            }
        }
    }
}