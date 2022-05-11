using Microsoft.AspNetCore.Identity;

namespace Identity.CustomValidation;

public class CustomIdentityErrorDescriber : IdentityErrorDescriber
{
    public override IdentityError InvalidUserName(string userName)
    {
        return new IdentityError()
        {
            Code = "InvalidUSerName",
            Description = $"Bu {userName} geçersizdir."
        };
    }

    public override IdentityError DuplicateEmail(string email)
    {
        return new IdentityError()
        {
            Code = "DublicateEmail",
            Description = $"Bu mail adresi '{email}' kullanılmaktadır."
        };
    }

    public override IdentityError DuplicateUserName(string userName)
    {
        return new IdentityError()
        {
            Code = "DublicateUserName",
            Description = $"Bu username '{userName}' kullanılmaktadır."
        };
    }
}