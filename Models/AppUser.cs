using System;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Pronia.Models;

public class AppUser : IdentityUser
{
    [Required]
    public string Fullname { get; set; }
    public DateTime BirthDate { get; set; }
}

