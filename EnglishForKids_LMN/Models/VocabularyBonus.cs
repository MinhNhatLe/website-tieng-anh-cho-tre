using EnglishForKids_LMN.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EnglishForKids_LMN.Models
{
    public class VocabularyBonus
    {
        [Required(ErrorMessage = " Please choose vocabulary type ")]
        public List<Category_Vo> category_vos { get; set; }
        public List<Vocabulary> vocabularies { get; set; }
        [DisplayName("English Name : ")]
        [Required(ErrorMessage = " Please enter english name ")]
        [MaxLength(30)]
        public string EN_Meaning { get; set; }
        [DisplayName("Vietnamese Name : ")]
        [Required(ErrorMessage = " Please enter vietnamese name ")]
        [MaxLength(30)]
        public string VN_Meaning { get; set; }
        [DisplayName("Pronunciation : ")]
        [Required(ErrorMessage = " Please enter pronunciation ")]
        [MaxLength(20)]
        public string Pronunciation { get; set; }
        [DisplayName("Vocabulary Image : ")]
        [Required(ErrorMessage = " Please enter vocabulary image ")]
        [MaxLength(50)]
        public string Image_Vocabulary { get; set; }
        public int ID_Category_Vo { get; set; }
        [Required(ErrorMessage = "Please enter vocabulary type name")]
        [MaxLength(20, ErrorMessage = "Vocabulary Type Name can't longer than 20 character")]
        public string Name_Category_Vo { get; set; }
        public int ID_Vocabulary { get; set; }

    }
}