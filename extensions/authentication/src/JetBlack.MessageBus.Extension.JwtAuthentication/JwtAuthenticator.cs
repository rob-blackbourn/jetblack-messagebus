using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security;
using System.Text;

using Microsoft.IdentityModel.Tokens;

using JetBlack.MessageBus.Common.IO;
using JetBlack.MessageBus.Common.Security.Authentication;

namespace JetBlack.MessageBus.Extension.JwtAuthentication
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

        public string Method => "JWT";
        public SymmetricSecurityKey SecurityKey { get; }

        public AuthenticationResponse Authenticate(Stream stream)
        {
            var reader = new DataReader(stream);
            var connectionString = reader.ReadString();
            var connectionDetails = JwtConnectionDetails.Parse(connectionString);

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.ReadJwtToken(connectionDetails.Token);
                var parameters = new TokenValidationParameters
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = SecurityKey
                };
                tokenHandler.ValidateToken(connectionDetails.Token, parameters, out var securityToken);
                return new AuthenticationResponse(
                    ((JwtSecurityToken)securityToken).Subject,
                    Method,
                    connectionDetails.Impersonating,
                    connectionDetails.ForwardedFor,
                    connectionDetails.Application,
                    null);
            }
            catch
            {
                throw new SecurityException();
            }
        }
    }
}