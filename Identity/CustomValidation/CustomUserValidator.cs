using Identity.Models;
using Microsoft.AspNetCore.Identity;

namespace Identity.CustomValidation;

public class CustomUserValidator : IUserValidator<AppUser>
{
    public Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user)
    {
        List<IdentityError> errors = new();
        string[] Digits = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
        foreach (var digit in Digits)
        {
            if (user.UserName[0].ToString() == digit)
            {
                errors.Add(new IdentityError()
                {
                    Code = "UserNameContainsFirstLetterDigitContains",
                    Description = "Kullanıcı adının ilk karakteri sayısal karakter olamaz."
                });
            }
            
            
        }
        
        if (errors.Count == 0)
        {
            return Task.FromResult(IdentityResult.Success);
        }
        else
        {
            return Task.FromResult(IdentityResult.Failed(errors.ToArray()));
        }
        
    }
}