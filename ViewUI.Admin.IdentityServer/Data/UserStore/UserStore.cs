using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using IdentityModel;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ViewUI.Admin.IdentityServer.Data.UserStore
{
    public class UserStore
    {
        //private readonly List<User> _users;
        private readonly UserContext _context;

        //
        // 摘要:
        //     Initializes a new instance of the IdentityServer4.Test.TestUserStore class.
        //
        // 参数:
        //   users:
        //     The users.
        public UserStore(UserContext context)
        {
            //_users = users;
            _context = context;
        }

        //
        // 摘要:
        //     Validates the credentials.
        //
        // 参数:
        //   username:
        //     The username.
        //
        //   password:
        //     The password.
        public bool ValidateCredentials(string username, string password)
        {
            User User = FindByUsername(username);
            if (User != null)
            {
                if (string.IsNullOrWhiteSpace(User.Password) && string.IsNullOrWhiteSpace(password))
                {
                    return true;
                }

                return User.Password.Equals(password);
            }

            return false;
        }

        //
        // 摘要:
        //     Finds the user by subject identifier.
        //
        // 参数:
        //   subjectId:
        //     The subject identifier.
        public User FindBySubjectId(string subjectId)
        {
            return _context.Users.Where(u => u.SubjectId == subjectId).Include(u => u.Claims).SingleOrDefault();
            //return ((IEnumerable<User>)_users).FirstOrDefault((Func<User, bool>)((User x) => x.SubjectId == subjectId));
        }

        //
        // 摘要:
        //     Finds the user by username.
        //
        // 参数:
        //   username:
        //     The username.
        public User FindByUsername(string username)
        {
            return _context.Users.Where(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)).Include(u => u.Claims).FirstOrDefault();
            //return ((IEnumerable<User>)_users).FirstOrDefault((Func<User, bool>)((User x) => x.Username.Equals(username, StringComparison.OrdinalIgnoreCase)));
        }

        //
        // 摘要:
        //     Finds the user by external provider.
        //
        // 参数:
        //   provider:
        //     The provider.
        //
        //   userId:
        //     The user identifier.
        public User FindByExternalProvider(string provider, string userId)
        {
            return _context.Users.Where(x => x.ProviderName == provider && x.ProviderSubjectId == userId).Include(u => u.Claims).FirstOrDefault();
        }

        //
        // 摘要:
        //     Automatically provisions a user.
        //
        // 参数:
        //   provider:
        //     The provider.
        //
        //   userId:
        //     The user identifier.
        //
        //   claims:
        //     The claims.
        public User AutoProvisionUser(string provider, string userId, List<Claim> claims)
        {
            List<Claim> list = new List<Claim>();
            foreach (Claim claim in claims)
            {
                if (claim.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")
                {
                    //list.Add(new Claim("name", claim.Value));
                    list.Add(new Claim { Type = "name", Value = claim.Value });
                }
                else if (JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.ContainsKey(claim.Type))
                {
                    list.Add(new Claim { Type = JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap[claim.Type], Value = claim.Value });
                }
                else
                {
                    list.Add(claim);
                }
            }

            if (!((IEnumerable<Claim>)list).Any((Func<Claim, bool>)((Claim x) => x.Type == "name")))
            {
                string text = ((IEnumerable<Claim>)list).FirstOrDefault((Func<Claim, bool>)((Claim x) => x.Type == "given_name"))?.Value;
                string text2 = ((IEnumerable<Claim>)list).FirstOrDefault((Func<Claim, bool>)((Claim x) => x.Type == "family_name"))?.Value;
                if (text != null && text2 != null)
                {
                    list.Add(new Claim { Type = "name", Value = text + " " + text2 });
                }
                else if (text != null)
                {
                    list.Add(new Claim { Type = "name", Value = text });
                }
                else if (text2 != null)
                {
                    list.Add(new Claim{ Type = "name", Value = text2 });
                }
            }

            string text3 = CryptoRandom.CreateUniqueId();
            string username = ((IEnumerable<Claim>)list).FirstOrDefault((Func<Claim, bool>)((Claim c) => c.Type == "name"))?.Value ?? text3;
            User User = new User
            {
                SubjectId = text3,
                Username = username,
                ProviderName = provider,
                ProviderSubjectId = userId,
                Claims = list
            };
            //_users.Add(User);
            _context.Users.Add(User);
            _context.SaveChanges();
            return User;
        }
    }
}
