using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using VOffice.API.Models;
using System.Configuration;

namespace VOffice.API.Providers
{
    /// <summary>
    /// OAuth
    /// </summary>
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {
        private readonly string _publicClientId;

        public ApplicationOAuthProvider(string publicClientId)
        {
            if (publicClientId == null)
            {
                throw new ArgumentNullException("publicClientId");
            }

            _publicClientId = publicClientId;
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            //var allowedOrigin = context.OwinContext.Get<string>("as:clientAllowedOrigin");

            //if (allowedOrigin == null) allowedOrigin = "*";

            //context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });

            var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();

            ApplicationUser user = await userManager.FindAsync(context.UserName, context.Password);
            if (user == null)
            {
                context.SetError("invalid_grant", "Tên đăng nhập hoặc mật khẩu không đúng");
                return;
            }
            else if (user.LockoutEnabled == false && user.Deleted == true)
            {
                context.SetError("deleted", "Failed");
                return;
            }
            else if (user.LockoutEnabled == true)
            {
                context.SetError("locked_out", "Failed");
                return;
            }
            else 
            {
                #region Customize

                VOffice.Model.UserDepartment userInfo = new Model.UserDepartment();
                VOffice.Repository.DepartmentRepository departmentRepository = new Repository.DepartmentRepository();
                userInfo = departmentRepository.GetUserMainOrganization(user.Id);
                #endregion Customize
                ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(userManager,
                OAuthDefaults.AuthenticationType);

                string email = string.IsNullOrEmpty(user.Email) ? "" : user.Email;


                oAuthIdentity.AddClaim(new Claim("userId", user.Id));
                oAuthIdentity.AddClaim(new Claim("fullName", userInfo.FullName));
                oAuthIdentity.AddClaim(new Claim("firstName", userInfo.FirstName));
                oAuthIdentity.AddClaim(new Claim("email", email));
                oAuthIdentity.AddClaim(new Claim("username", user.UserName));
                oAuthIdentity.AddClaim(new Claim("position", userInfo.Position));
                oAuthIdentity.AddClaim(new Claim("staffCode", userInfo.StaffCode));

                oAuthIdentity.AddClaim(new Claim("phoneNumber", userInfo.PhoneNumber != null ? userInfo.PhoneNumber.ToString() : string.Empty));
                oAuthIdentity.AddClaim(new Claim("dateOfBirth", userInfo.DateOfBirth != null ? userInfo.DateOfBirth.Value.ToString("dd/MM/yyyy") : ""));
                oAuthIdentity.AddClaim(new Claim("gender", userInfo.Gender != null ? userInfo.Gender.Value.ToString() : "1"));
                oAuthIdentity.AddClaim(new Claim("avatar", userInfo.Avatar != null ? userInfo.Avatar.ToString() : string.Empty));
                oAuthIdentity.AddClaim(new Claim("leader", userInfo.Leader != null ? userInfo.Leader.Value.ToString() : "false"));
                oAuthIdentity.AddClaim(new Claim("seniorLeader", userInfo.SeniorLeader != null ? userInfo.SeniorLeader.Value.ToString() : "false"));
                oAuthIdentity.AddClaim(new Claim("superLeader", userInfo.SuperLeader != null ? userInfo.SuperLeader.Value.ToString() : "false"));
                oAuthIdentity.AddClaim(new Claim("signedBy", userInfo.SignedBy != null ? userInfo.SignedBy.Value.ToString() : "false"));
                oAuthIdentity.AddClaim(new Claim("googleAccount", userInfo.GoogleAccount != null ? userInfo.GoogleAccount.ToString() : string.Empty));

                oAuthIdentity.AddClaim(new Claim("departmentId", userInfo.DepartmentId.ToString()));
                oAuthIdentity.AddClaim(new Claim("parentDepartmentId", userInfo.RootDepartmentId.ToString()));
                oAuthIdentity.AddClaim(new Claim("office", userInfo.Office.ToString()));
                oAuthIdentity.AddClaim(new Claim("subDepartmentId", userInfo.SubDepartmentId.ToString()));
                oAuthIdentity.AddClaim(new Claim("ListSubDepartmentId", userInfo.ListSubDepartmentId));
                oAuthIdentity.AddClaim(new Claim("departmentShortName", userInfo.DepartmentShortName != null ? userInfo.DepartmentShortName.ToString() : string.Empty));
                oAuthIdentity.AddClaim(new Claim("departmentName", userInfo.DepartmentName != null ? userInfo.DepartmentName.ToString() : string.Empty));


                //remove rolesClaim
                //var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>());
                //var roles = roleManager.Roles.ToList();
                //foreach (var role in roles)
                //{
                //    try
                //    {
                //        oAuthIdentity.RemoveClaim(new Claim(ClaimTypes.Role, role.Name));
                //    }
                //    catch (Exception ex)
                //    { }
                //}

                var currentRoles = await userManager.GetRolesAsync(user.Id);
                foreach (var item in currentRoles)
                {
                    oAuthIdentity.AddClaim(new Claim(ClaimTypes.Role, item));
                }


                ClaimsIdentity cookiesIdentity = await user.GenerateUserIdentityAsync(userManager,
                    CookieAuthenticationDefaults.AuthenticationType);

                //AuthenticationProperties properties = CreateProperties(user.UserName);

                var properties = new AuthenticationProperties(new Dictionary<string, string>
                    {
                     {"userId", user.Id},
                        {"fullName", userInfo.FullName},
                        {"firstName", userInfo.FirstName},
                        {"email", email},
                        {"username", user.UserName},
                        {"position", userInfo.Position},
                        {"staffCode", userInfo.StaffCode},
                        {"phoneNumber", userInfo.PhoneNumber != null ? userInfo.PhoneNumber.ToString() : string.Empty},
                        {"dateOfBirth", userInfo.DateOfBirth != null ? userInfo.DateOfBirth.Value.ToString("dd/MM/yyyy") : ""},
                        {"gender", userInfo.Gender != null ? userInfo.Gender.Value.ToString() : "1"},
                        {"avatar",userInfo.Avatar != null ? userInfo.Avatar.ToString() : string.Empty},
                        {"leader", userInfo.Leader != null ? userInfo.Leader.Value.ToString() : "false"},
                        {"seniorLeader", userInfo.SeniorLeader != null ? userInfo.SeniorLeader.Value.ToString() : "false"},
                        {"superLeader", userInfo.SuperLeader != null ? userInfo.SuperLeader.Value.ToString() : "false"},
                        {"signedBy", userInfo.SignedBy != null ? userInfo.SignedBy.Value.ToString() : "false"},
                        {"googleAccount", userInfo.GoogleAccount != null ? userInfo.GoogleAccount.ToString() : string.Empty},
                        {"departmentId", userInfo.DepartmentId.ToString()},
                        {"parentDepartmentId", userInfo.RootDepartmentId.ToString()},
                        {"office", userInfo.Office.ToString()},
                        {"subDepartmentId", userInfo.SubDepartmentId.ToString()},
                        {"ListSubDepartmentId", userInfo.ListSubDepartmentId},
                        {"departmentShortName", userInfo.DepartmentShortName!=null?userInfo.DepartmentShortName.ToString():string.Empty},
                     {"departmentName", userInfo.DepartmentName!=null?userInfo.DepartmentName.ToString():string.Empty}
                    });
                AuthenticationTicket ticket = new AuthenticationTicket(oAuthIdentity, properties);
                context.Validated(ticket);
                context.Request.Context.Authentication.SignIn(cookiesIdentity);
            }


        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            // Resource owner password credentials does not provide a client ID.
            if (context.ClientId == null)
            {
                context.Validated();
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            if (context.ClientId == _publicClientId)
            {
                Uri expectedRootUri = new Uri(context.Request.Uri, "/");

                if (expectedRootUri.AbsoluteUri == context.RedirectUri)
                {
                    context.Validated();
                }
            }

            return Task.FromResult<object>(null);
        }

        public static AuthenticationProperties CreateProperties(string userName)
        {
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "userName", userName }
            };
            return new AuthenticationProperties(data);
        }
    }
}