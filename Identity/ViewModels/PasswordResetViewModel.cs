using System.ComponentModel.DataAnnotations;

namespace Identity.Models.ViewModels;

public class PasswordResetViewModel
{
    [Required(ErrorMessage = "Email alanı gereklidir")]
    [EmailAddress]
    [Display(Name = "Email adresiniz")]
    public string Email { get; set; }
}