using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HobbiesExam.Models
{
    public class User
        {
                [Key]
                public int UserId {get;set;}

               [Required]
                [MinLength(2)]
                public string FirstName {get; set;}

                [Required]
                [MinLength(2)]
                public string LastName {get; set;}

                [Required]
                [MinLength(3,  ErrorMessage = "Username must be 3-15 characters")]
                [MaxLength(15,  ErrorMessage = "Username must be 3-15 characters")]    
                public string UserName {get; set;}

                [Required]
                [DataType(DataType.Password)]
                [MinLength(8, ErrorMessage="Password must be 8 characters or longer!")]
                public string Password {get; set;}
                    // Will not be mapped to your users table!
                [NotMapped]
                [Compare("Password")]
                [DataType(DataType.Password)]
                public string Confirm {get;set;} 

                public List<Association> LikedHobbies {get; set;} // all the hobbies this user has

        }
}