using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Identity.Models;

public class AppUser : IdentityUser
{
    [StringLength(50)] public string City { get; set; }
    public string Picture { get; set; }
    public DateTime BirthDay { get; set; }
    public ushort Gender { get; set; }
}