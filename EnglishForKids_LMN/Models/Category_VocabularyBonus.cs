using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EnglishForKids_LMN.Models;

namespace EnglishForKids_LMN.Models
{
    public class Category_VocabularyBonus
    {
        [Required(ErrorMessage = "Please enter vocabulary type name")]
        [MaxLength(20, ErrorMessage = "Vocabulary type name no longer than 20 character")]
        public string Name_Category_Vo { get; set; }
        public int ID_Category_Vo { get; set; }

        public List<Category_Vo> vocabulary_Types { get; set; }

        internal object ToPagedList(int pageNum, int pageSize)
        {
            throw new NotImplementedException();
        }
    }
}