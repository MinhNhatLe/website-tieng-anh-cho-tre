using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishForKids_LMN.Models
{
    public class TempListAnswer
    {
        private String idAnswer;
        private String questionAnswer;
        private String Answer;


        public string IdAnswer { get => idAnswer; set => idAnswer = value; }
        public string QuestionAnswer { get => questionAnswer; set => questionAnswer = value; }
        public string Answers { get => Answer; set => Answer = value; }
    }
}
