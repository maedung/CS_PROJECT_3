using System;
using System.ComponentModel.DataAnnotations;

namespace BeltTest.Models
{
    public class Post
    {
        [Key]
        public int id {get;set;}
        
        public int userid {get;set;}

        public int activityid {get;set;}

        public User User {get;set;}

        public Activity Activity {get;set;}
    }
}