//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EnglishForKids_LMN.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Vocabulary
    {
        public int ID_Vocabulary { get; set; }
        public string Pronunciation { get; set; }
        public string VN_Meaning { get; set; }
        public string EN_Meaning { get; set; }
        public string Image_Vocabulary { get; set; }
        public Nullable<int> ID_Cate_Vo { get; set; }
        public Nullable<int> ID_Category { get; set; }
    
        public virtual Category Category { get; set; }
        public virtual Category_Vo Category_Vo { get; set; }
        public bool ShowRecordButton { get; internal set; }
    }
}
