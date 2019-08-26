using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BeltTest.Models
{
    public class Activity
    {
        [Key]
        public int id {get;set;}

        [Required]
        public string title {get;set;}

        [Required]
        [DataType(DataType.Date)]
        public DateTime date {get;set;}

        [Required]
        [DataType(DataType.Time)]
        public DateTime time {get;set;}

        [Required]
        public string duration {get;set;}

        [Required]
        public string desc {get;set;}

        [Required]
        public int Userid {get;set;}

        public User Creator {get;set;}

        public List<Post> PostedActivities {get;set;}
    }
}