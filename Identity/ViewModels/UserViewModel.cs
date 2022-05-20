using System.ComponentModel.DataAnnotations;
using Identity.Enums;

namespace Identity.Models.ViewModels;

public class UserViewModel
{
    [Required(ErrorMessage = "Kullanıcı ismi gereklidir")]
    [Display(Name = "Kullanıcı Adı")]
    public string UserName { get; set; }

    [Display(Name = "Tel No:")] public string PhoneNumber { get; set; }

    [Required(ErrorMessage = "Email adresi gereklidir")]
    [Display(Name = "Email Adresiniz")]
    [EmailAddress(ErrorMessage = "Email adresiniz doğru formatta değil")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Şifre gereklidir")]
    [Display(Name = "Şifre alanı")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    public DateTime BirthDay { get; set; }
    public string Picture { get; set; }
    public string City { get; set; }
    public Gender Gender { get; set; }
}