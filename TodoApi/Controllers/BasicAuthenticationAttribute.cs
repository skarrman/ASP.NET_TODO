using System;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TodoApi.Models;

namespace TodoApi.Controllers
{
    public class BasicAuthenticationAttribute: Attribute, IAuthorizationFilter
    {
        public const string AuthScheme = "Basic";
        private const string AuthHeaderKey = "Authorization";
        private const string EncodingId = "iso-8859-1";
        private const char CredentialsSeparator = ':';

        public void OnAuthorization(AuthorizationFilterContext context)
        {

            var authHeaderValue = context.HttpContext
                .Request.Headers[AuthHeaderKey];

            if(AuthenticationHeaderValue.TryParse(authHeaderValue,
                out var authHeader) && authHeader.Scheme == AuthScheme)
            {
                Encoding encoding = Encoding.GetEncoding(EncodingId);
                var usrPwd = encoding.GetString(
                    Convert.FromBase64String(authHeader.Parameter));
                var credentials = usrPwd.Split(CredentialsSeparator);
                if (credentials.Length == 2 && credentials[0] == "usr" &&
                    credentials[1] == "pwd")
                {
                    return;
                }
            }
            context.Result = new UnauthorizedResult();
        }
    }
}
