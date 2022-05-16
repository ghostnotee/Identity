using System.ComponentModel.DataAnnotations;

namespace Identity.Models.ViewModels;

public class PasswordResetViewModel
{
    [Required(ErrorMessage = "Email alanı gereklidir")]
    [EmailAddress]
    [Display(Name = "Email adresiniz")]
    public string Email { get; set; }
    
    [Display(Name = "Yeni Şifreniz")]
    [Required(ErrorMessage = "Şifre alanı gereklidir")]
    [DataType(DataType.Password)]
    [MinLength(4, ErrorMessage = "En az 4 karakter olmalıdır")]
    public string PasswordNew { get; set; }
}