using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Authentication;
using sysClaims = System.Security.Claims;

namespace ViewUI.Admin.IdentityServer.Data.UserStore
{
    //
    // 摘要:
    //     Resource owner password validator for test users
    public class UserResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly UserStore _users;

        private readonly ISystemClock _clock;

        //
        // 摘要:
        //     Initializes a new instance of the IdentityServer4.Test.TestUserResourceOwnerPasswordValidator
        //     class.
        //
        // 参数:
        //   users:
        //     The users.
        //
        //   clock:
        //     The clock.
        public UserResourceOwnerPasswordValidator(UserStore users, ISystemClock clock)
        {
            _users = users;
            _clock = clock;
        }

        //
        // 摘要:
        //     Validates the resource owner password credential
        //
        // 参数:
        //   context:
        //     The context.
        public Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            if (_users.ValidateCredentials(context.UserName, context.Password))
            {
                User user = _users.FindByUsername(context.UserName);
                List<sysClaims.Claim> claims = new List<sysClaims.Claim>();
                foreach (var item in user.Claims)
                {
                    claims.Add(new sysClaims.Claim(item.Type, item.Value));
                }
                context.Result = new GrantValidationResult(user.SubjectId ?? throw new ArgumentException("Subject ID not set", "SubjectId"), "pwd", _clock.UtcNow.UtcDateTime, claims);
            }

            return Task.CompletedTask;
        }
    }
}
