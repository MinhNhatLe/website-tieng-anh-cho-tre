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
        [DisplayName("Story name: ")]
        [Required(ErrorMessage = " Please enter the name of the story ")]
        [MaxLength(50)]
        public string Name_Story { get; set; }
        [DisplayName("English content: ")]
        [Required(ErrorMessage = " Please enter English content")]
        [MaxLength(50)]
        public string EN_Content { get; set; }
        [DisplayName("Vietnamese content: ")]
        [Required(ErrorMessage = " Please enter the content in Vietnamese ")]
        [MaxLength(50)]
        public string VN_Content { get; set; }
        [DisplayName("Story audio: ")]
        [Required(ErrorMessage = " Please enter the story audio ")]
        [MaxLength(50)]
        public string Audio { get; set; }
        [DisplayName("Story banner: ")]
        [Required(ErrorMessage = " Please enter story banner ")]
        [MaxLength(50)]
        public string Banner { get; set; }
        [DisplayName("Story image: ")]
        [Required(ErrorMessage = " Please enter story image ")]
        [MaxLength(50)]
        public string Image_Story { get; set; }
        public int ID_Story { get; set; }
        public string Name_User { get; set; }
        public Nullable<int> ID_User { get; set; }

        public virtual User Users { get; set; }
    }
}