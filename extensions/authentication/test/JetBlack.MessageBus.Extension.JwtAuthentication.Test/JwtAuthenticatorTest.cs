using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security;
using System.Security.Claims;
using System.Text;

using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using JetBlack.MessageBus.Common.IO;
using JetBlack.MessageBus.Common.Security.Authentication;

namespace JetBlack.MessageBus.Extension.JwtAuthentication
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void ShouldAuthenticate()
        {
            // Create a valid token.
            const string secret = "a secret that needs to be at least 16 characters long";
            const string issuer = "example.com";
            const string audience = "example.com";
            const string username = "jim@example.com";

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256, SecurityAlgorithms.Sha256Digest);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                notBefore: DateTime.UtcNow,
                signingCredentials: signingCredentials);

            var tokenHandler = new JwtSecurityTokenHandler();
            var encodedToken = tokenHandler.WriteToken(token);

            using (var stream = new MemoryStream())
            {
                var writer = new DataWriter(stream);
                writer.Write($"Token={encodedToken}");

                stream.Seek(0, SeekOrigin.Begin);
                var authenticator = new JwtAuthenticator(new[] { secret });
                var identity = authenticator.Authenticate(stream);
                Assert.AreEqual(identity.User, username);
                Assert.AreEqual(identity.Method, "JWT");
            }
        }

        [TestMethod]
        public void ShouldNotAuthenticate()
        {
            // Create a valid token.
            const string validSecret = "a valid secret that needs to be at least 16 characters long";
            const string invalidSecret = "an invalid secret that needs to be at least 16 characters long";
            const string issuer = "example.com";
            const string audience = "example.com";
            const string username = "jim@example.com";

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(validSecret));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256, SecurityAlgorithms.Sha256Digest);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddSeconds(1), // Expired!
                notBefore: DateTime.UtcNow,
                signingCredentials: signingCredentials);

            var tokenHandler = new JwtSecurityTokenHandler();
            var encodedToken = tokenHandler.WriteToken(token);

            using (var stream = new MemoryStream())
            {
                var writer = new DataWriter(stream);
                writer.Write($"Token={encodedToken}");

                stream.Seek(0, SeekOrigin.Begin);
                var authenticator = new JwtAuthenticator(new[] { invalidSecret });
                Assert.ThrowsException<SecurityException>(() =>
                {
                    authenticator.Authenticate(stream);
                });
            }
        }
    }
}
