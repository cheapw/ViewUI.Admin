using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.Extensions.Logging;
using sysClaims = System.Security.Claims;

namespace ViewUI.Admin.IdentityServer.Data.UserStore
{
    //
    // 摘要:
    //     Profile service for test users
    public class UserProfileService : IProfileService
    {
        ////
        //// 摘要:
        ////     The logger
        //protected readonly ILogger Logger;

        //
        // 摘要:
        //     The users
        protected readonly UserStore Users;

        //
        // 摘要:
        //     Initializes a new instance of the IdentityServer4.Test.TestUserProfileService
        //     class.
        //
        // 参数:
        //   users:
        //     The users.
        //
        //   logger:
        //     The logger.
        public UserProfileService(UserStore users)
        {
            Users = users;
            //Logger = logger;
        }

        //
        // 摘要:
        //     This method is called whenever claims about the user are requested (e.g. during
        //     token creation or via the userinfo endpoint)
        //
        // 参数:
        //   context:
        //     The context.
        public virtual Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            //context.LogProfileRequest(Logger);
            if (context.RequestedClaimTypes.Any())
            {
                User User = Users.FindBySubjectId(context.Subject.GetSubjectId());
                if (User != null)
                {
                    List<sysClaims.Claim> claims = new List<sysClaims.Claim>();
                    foreach (var item in User.Claims)
                    {
                        claims.Add(new sysClaims.Claim(item.Type, item.Value));
                    }
                    context.AddRequestedClaims(claims);
                }
            }

            //context..(Logger);
            return Task.CompletedTask;
        }

        //
        // 摘要:
        //     This method gets called whenever identity server needs to determine if the user
        //     is valid or active (e.g. if the user's account has been deactivated since they
        //     logged in). (e.g. during token issuance or validation).
        //
        // 参数:
        //   context:
        //     The context.
        public virtual Task IsActiveAsync(IsActiveContext context)
        {
            //Logger.LogDebug("IsActive called from: {caller}", context.Caller);
            context.IsActive = (Users.FindBySubjectId(context.Subject.GetSubjectId())?.IsActive ?? false);
            return Task.CompletedTask;
        }
    }
}

