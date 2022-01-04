using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace HobbiesExam.Models
{
    public class Association
    {
        [Key]
        public int AssociationId { get; set; }
        [Required]
        public int UserId { get; set; }
        public User user { get; set; }

        [Required]
        public int HobbyId {get;set;}
        public Hobby hobby{get;set;}

        //[Required]
        //public string Level{get;set;}


    }   
        
}