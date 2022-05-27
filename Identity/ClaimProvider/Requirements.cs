using Microsoft.AspNetCore.Authorization;

namespace Identity.ClaimProvider;

public class Requirements : IAuthorizationRequirement
{
}

public class ExpireDateExchangeHandler : AuthorizationHandler<Requirements>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, Requirements requirement)
    {
        if (context.User is not null && context.User.Identity is not null)
        {
            var claim = context.User.Claims
                .FirstOrDefault(x => x.Type == "ExpireDateExchange");

            if (claim is not null)
            {
                if (DateTime.Now < Convert.ToDateTime(claim.Value))
                {
                    context.Succeed(requirement);
                }
                else
                {
                    context.Fail();
                }
            }
        }

        return Task.CompletedTask;
    }
}