using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HobbiesExam.Models
{
    public class Login 
        {
            [Required]
            public string UserName {get; set;}

            [Required]
            [DataType(DataType.Password)]
            public string Password {get; set;}

        }
}