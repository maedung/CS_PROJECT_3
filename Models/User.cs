using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeltTest.Models
{
    public class User
    {
        [Key]
        public int id {get;set;}

        [Required]
        [MinLength(2)]
        public string name {get;set;}

        [Required]
        [EmailAddress]
        public string email {get;set;}

        [Required]
        [DataType(DataType.Password)]
        [MinLength(8)]
        [RegularExpression(@"(?=.*\d)(?=.*[A-Za-z])(?=.*[!@#$%^&*]).{8,}", ErrorMessage = "Password should contain 1 number, 1 letter, 1 special character")]
        public string password {get;set;}

        [Required]
        [DataType(DataType.Password)]
        [NotMapped]
        [Compare("password")]
        public string confirm {get;set;}

        public List<Post> Posts {get;set;}
        public List<Activity> CreatedActivities {get;set;}
    }

    public class LoginUser
    {
        [Required]
        public string login_email {get;set;}

        [Required]
        [DataType(DataType.Password)]
        public string login_password {get;set;}
    }
}