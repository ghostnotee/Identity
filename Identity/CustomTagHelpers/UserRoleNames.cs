using Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Identity.CustomTagHelpers;

[HtmlTargetElement("td", Attributes = "user-roles")]
public class UserRoleNames : TagHelper
{
    public UserManager<AppUser> UserManager { get; set; }
    
    public UserRoleNames(UserManager<AppUser> userManager)
    {
        UserManager = userManager;
    }

    [HtmlAttributeName("user-roles")] public string UserId { get; set; }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var user = await UserManager.FindByIdAsync(UserId);
        var roles = await UserManager.GetRolesAsync(user);

        var html = string.Empty;
        roles.ToList().ForEach(x => { html += $"<span class='badge bg-info text-dark'> {x}  </span>"; });

        output.Content.SetHtmlContent(html);
    }
}