using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EnglishForKids_LMN.Models
{
    public class UserBonus
    {
        [DisplayName("Full name: ")]
        [Required(ErrorMessage = " Please enter your full name ")]
        [MaxLength(20, ErrorMessage = " Please enter your name with no more than 20 characters ")]
        public string User_FullName { get; set; }
        [DisplayName("Password: ")]
        [Required(ErrorMessage = " Please enter your password ")]
        [MaxLength(10, ErrorMessage = " Please enter a password between 5 and 10 characters ")]
        [MinLength(5, ErrorMessage = " Please enter a password between 5 and 10 characters ")]
        [DataType(DataType.Password)]
        public string User_Password { get; set; }
        [DisplayName("Confirm password: ")]
        //[Required(ErrorMessage = " Please re-enter your password ")]
        [DataType(DataType.Password)]
        public string Check_Password { get; set; }

        [DisplayName("Remember me ")]
        public bool RememberMe { get; set; }
        [DisplayName("Verification Code: ")]
        [Required(ErrorMessage = " Please enter your verification code ")]
        public int CheckMail { get; set; }
        [DisplayName("Gender: ")]
        [Required(ErrorMessage = " Please enter your gender ")]
        public string User_Gender { get; set; }
        [DisplayName("Phone number: ")]
        [Required(ErrorMessage = " Please enter your phone number ")]
        [MaxLength(10, ErrorMessage = " Phone number must have 10 number ")]
        [MinLength(10, ErrorMessage = " Phone number must have 10 number ")]
        public string User_PhoneNumber { get; set; }
        [DisplayName("Gmail: ")]
        [Required(ErrorMessage = " Please enter your gmail ")]
        [DataType(DataType.EmailAddress)]
        public string User_Mail { get; set; }

    }
}