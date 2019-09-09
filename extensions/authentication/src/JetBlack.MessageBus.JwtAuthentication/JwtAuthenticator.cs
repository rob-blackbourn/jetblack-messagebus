#nullable enable

using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security;
using System.Security.Principal;
using System.Text;

using Microsoft.IdentityModel.Tokens;

using JetBlack.MessageBus.Common.IO;

namespace JetBlack.MessageBus.Common.Security.Authentication
{
    public class JwtAuthenticator : IAuthenticator
    {
        public JwtAuthenticator(string[] args)
        {
            if (args.Length != 1)
                throw new ArgumentException("usage: <secret>");

            var secret = Environment.ExpandEnvironmentVariables(args[0]);

            SecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        }

        public SymmetricSecurityKey SecurityKey { get; }

        public GenericIdentity Authenticate(Stream stream)
        {
            var reader = new DataReader(stream);
            var encodedToken = reader.ReadString();

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.ReadJwtToken(encodedToken);
                var parameters = new TokenValidationParameters
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = SecurityKey
                };
                tokenHandler.ValidateToken(encodedToken, parameters, out var securityToken);
                return new GenericIdentity(((JwtSecurityToken)securityToken).Subject, "JWT");
            }
            catch
            {
                throw new SecurityException();
            }
        }
    }
}