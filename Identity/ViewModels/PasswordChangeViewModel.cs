using System.ComponentModel.DataAnnotations;

namespace Identity.Models.ViewModels;

public class PasswordChangeViewModel
{
    [Display(Name = "Eski Şifreniz")]
    [Required(ErrorMessage = "Eski Şifre gereklidir")]
    [DataType(DataType.Password)]
    [MinLength(4, ErrorMessage = "En az 4 karakterli olmalıdır.")]
    public string PasswordOld { get; set; }

    [Display(Name = "Yeni Şifreniz")]
    [Required(ErrorMessage = "Yeni Şifre gereklidir")]
    [DataType(DataType.Password)]
    [MinLength(4, ErrorMessage = "En az 4 karakterli olmalıdır.")]
    public string PasswordNew { get; set; }
    
    [Display(Name = "Doğrulama Şifreniz")]
    [Required(ErrorMessage = "Yeni Şifre gereklidir")]
    [DataType(DataType.Password)]
    [MinLength(4, ErrorMessage = "En az 4 karakterli olmalıdır.")]
    [Compare("PasswordNew",ErrorMessage = "Doğrulama aynı olmalıdır")]
    public string PasswordConfirm { get; set; }
}