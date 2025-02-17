using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;

using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using Moq;

using Witsml.Data;

using WitsmlExplorer.Api.Configuration;
using WitsmlExplorer.Api.HttpHandlers;
using WitsmlExplorer.Api.Models;
using WitsmlExplorer.Api.Repositories;
using WitsmlExplorer.Api.Services;

using Xunit;
namespace WitsmlExplorer.Api.Tests.Services
{
    public class CredentialsServiceTests
    {
        private readonly CredentialsService _credentialsService;

        public CredentialsServiceTests()
        {
            Mock<IDataProtectionProvider> dataProtectorProvider = new();
            Mock<ITimeLimitedDataProtector> dataProtector = new();
            Mock<ILogger<CredentialsService>> logger = new();
            Mock<IOptions<WitsmlClientCapabilities>> clientCapabilities = new();
            Mock<IWitsmlSystemCredentials> witsmlServerCredentials = new();
            Mock<IDocumentRepository<Server, Guid>> witsmlServerRepository = new();
            CredentialsCache credentialsCache = new(new Mock<ILogger<CredentialsCache>>().Object);

            dataProtector.Setup(p => p.Protect(It.IsAny<byte[]>())).Returns((byte[] a) => a);
            dataProtector.Setup(p => p.Unprotect(It.IsAny<byte[]>())).Returns((byte[] a) => a);
            dataProtectorProvider.Setup(pp => pp.CreateProtector("WitsmlServerPassword")).Returns(dataProtector.Object);
            clientCapabilities.Setup(cc => cc.Value).Returns(new WitsmlClientCapabilities());

            // Keyvault secrets
            witsmlServerCredentials.Setup(w => w.WitsmlCreds).Returns(new ServerCredentials[] {
                new ServerCredentials() {
                    Host = new Uri("http://some.url.com"),
                    UserId = "systemuser",
                    Password = "systempassword"
                }
            });
            // Configuration DB
            witsmlServerRepository.Setup(wsr => wsr.GetDocumentsAsync()).ReturnsAsync(new List<Server>() {
                new Server()
                {
                    Name = "Test Server",
                    Url = new Uri("http://some.url.com"),
                    Description = "Testserver for SystemCreds testing",
                    SecurityScheme = "OAuth2",
                    Roles = new List<string>() {"validrole","developer"}
                }
            });

            _credentialsService = new(
                dataProtectorProvider.Object,
                clientCapabilities.Object,
                witsmlServerCredentials.Object,
                witsmlServerRepository.Object,
                credentialsCache,
                logger.Object
            );
        }

        [Fact]
        public void GetCredentials_ValidTokenValidRolesValidURLValidUsername_ReturnSystemCreds()
        {
            // 1. CONFIG:   There is a server config in DB with URL: "http://some.url.com" and role: ["validrole"]
            // 2. CONFIG:   There exist system credentials in keyvault for server with URL: "http://some.url.com"
            // 3. REQUEST:  User provide token and valid roles in token
            // 4. REQUEST:  Header WitsmlTargetServer Header with URL: "http://some.url.com"
            // 5. RESPONSE: System creds should be returned because server-roles and user-roles overlap
            string server = "http://some.url.com";
            EssentialHeaders eh = CreateEhWithAuthorization(new string[] { "validrole" }, false, "tokenuser@arpa.net");

            ServerCredentials creds = _credentialsService.GetCredentials(true, eh, server, "systemuser");
            Assert.True(creds.UserId == "systemuser" && creds.Password == "systempassword");
            Cleanup();
        }

        [Fact]
        public void GetCredentials_ValidTokenValidRolesInvalidURL_ReturnNull()
        {
            string server = "http://some.invalidurl.com";
            EssentialHeaders eh = CreateEhWithAuthorization(new string[] { "validrole" }, false, "tokenuser@arpa.net");

            ServerCredentials creds = _credentialsService.GetCredentials(true, eh, server, "systemuser");
            Assert.Null(creds);
            Cleanup();
        }

        [Fact]
        public void GetCredentials_InvalidTokenRolesURLOnlyBasicHeader_ReturnNull()
        {
            string server = "http://some.url.com";
            EssentialHeaders eh = CreateEhWithAuthorization(new string[] { "invalidrole" }, false, "tokenuser@arpa.net");

            ServerCredentials creds = _credentialsService.GetCredentials(true, eh, server, "systemuser");
            Assert.Null(creds);
            Cleanup();
        }

        [Fact]
        public void GetCredentials_CredentialsInCache_ReturnCorrectly()
        {
            string userId = "username";
            string clientId = Guid.NewGuid().ToString();
            ServerCredentials sc = new() { UserId = userId, Password = "dummypassword", Host = new Uri("https://somehost.url") };
            string b64Creds = Convert.ToBase64String(Encoding.ASCII.GetBytes(sc.UserId + ":" + sc.Password));
            string headerValue = b64Creds + "@" + sc.Host;

            Mock<IEssentialHeaders> headersMock = new();
            headersMock.Setup(x => x.GetCookieValue()).Returns(clientId);
            headersMock.SetupGet(x => x.TargetServer).Returns(sc.Host.ToString());

            _credentialsService.CacheCredentials(clientId, sc, 1.0, n => n);
            ServerCredentials fromCache = _credentialsService.GetCredentials(false, headersMock.Object, headersMock.Object.TargetServer, userId);
            Assert.Equal(sc, fromCache);
            _credentialsService.RemoveAllCachedCredentials();
            Cleanup();
        }

        private void Cleanup()
        {
            _credentialsService.RemoveAllCachedCredentials();
        }

        private static EssentialHeaders CreateEhWithAuthorization(string[] appRoles, bool signed, string upn)
        {
            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Expires = DateTime.UtcNow.AddSeconds(60),
                Claims = new Dictionary<string, object>() {
                    { "roles", new List<string>(appRoles) },
                    { "upn", upn },
                    { "sub", Guid.NewGuid().ToString() }
                }
            };
            if (signed)
            {
                byte[] secret = new byte[64];
                RandomNumberGenerator.Create().GetBytes(secret);
                tokenDescriptor.SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(secret),
                    SecurityAlgorithms.HmacSha256Signature,
                    SecurityAlgorithms.Sha512Digest
                );
            }
            return new()
            {
                Authorization = "Bearer " + new JwtSecurityTokenHandler().WriteToken(new JwtSecurityTokenHandler().CreateJwtSecurityToken(tokenDescriptor))
            };
        }
    }
}
