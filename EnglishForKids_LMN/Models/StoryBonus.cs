using EnglishForKids_LMN.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EnglishForKids_LMN.Models
{
    public class StoryBonus
    {
        [DisplayName("Tên câu chuyện : ")]
        [Required(ErrorMessage = " Hãy nhập thông tin tên câu chuyện ")]
        [MaxLength(50)]
        public string Name_Story { get; set; }
        [DisplayName("Nội dung tiếng anh : ")]
        [Required(ErrorMessage = " Hãy nhập nội dụng tiếng anh")]
        [MaxLength(50)]
        public string EN_Content { get; set; }
        [DisplayName("Nội dung tiếng việt : ")]
        [Required(ErrorMessage = " Hãy nhập nội dụng tiếng việt ")]
        [MaxLength(50)]
        public string VN_Content { get; set; }
        [DisplayName("Âm thanh câu chuyện : ")]
        [Required(ErrorMessage = " Hãy nhập âm thanh câu chuyện ")]
        [MaxLength(50)]
        public string Audio { get; set; }
        [DisplayName("Story banner : ")]
        [Required(ErrorMessage = " Please enter story banner ")]
        [MaxLength(50)]
        public string Banner { get; set; }
        [DisplayName("Story image : ")]
        [Required(ErrorMessage = " Please enter story image ")]
        [MaxLength(50)]
        public string Image_Story { get; set; }
        public int ID_Story { get; set; }
        public string Name_User { get; set; }
        public Nullable<int> ID_User { get; set; }

        public virtual User Users { get; set; }
    }
}