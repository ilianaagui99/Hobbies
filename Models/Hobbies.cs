using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace HobbiesExam.Models
{
    public class Hobby
        {
            [Key]
            public int HobbyId { get; set; }

            [Required]
            [MinLength(2)]
            public string Name {get; set;}

            [Required]
            [MinLength(2)]
            public string Description {get; set;}
            public DateTime CreatedAt {get;set;} = DateTime.Now;
            public DateTime UpdatedAt {get;set;} = DateTime.Now; 
           // public List<Association> ProductsCategories {get; set;}

           public int UserId{get;set;}
           public User User{get;set;}
           public List<Association> Enthusiasts{get;set;}

        }
}