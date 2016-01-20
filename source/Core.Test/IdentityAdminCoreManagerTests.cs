/*
 * Copyright 2015 Bert Hoorne,Dominick Baier, Brock Allen
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Test.Config;
using IdentityAdmin.Core;
using IdentityServer3.Core.Models;
using IdentityServer3.EntityFramework;
using IdentityServer3.EntityFramework.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Client = IdentityServer3.EntityFramework.Entities.Client;
using Scope = IdentityServer3.EntityFramework.Entities.Scope;
using ScopeClaim = IdentityServer3.EntityFramework.Entities.ScopeClaim;

namespace Core.Test
{
    [TestClass]
    public class IdentityAdminCoreManagerTests
    {
        private const string ConnectionString = "IdSvr3ConfigAdmin";
        private static string _clientSubject = "1";
        private static string _scopeSubject = "2";
        private static string _clientName = "TestClient";
        private static string _scopeName = "TestScope";
        private static IdentityAdminManagerService _identityAdminManagerService;

        public IdentityAdminCoreManagerTests()
        {
            _identityAdminManagerService = new IdentityAdminManagerService("IdSvr3ConfigAdmin");
            using (var db = new ClientConfigurationDbContext(ConnectionString))
            {
                var allClients = db.Clients.Where(p => true);
                foreach (var c in allClients  )
                {
                    db.Clients.Remove(c);
                }
                db.SaveChanges();
                var testClient = new Client
                {
                    ClientId = "IdToTest",
                    ClientName = _clientName,
                    Enabled = true,
                    Flow = Flows.Implicit,
                    RequireConsent = true,
                    AllowRememberConsent = true,
                    RedirectUris =new List<ClientRedirectUri>() {new ClientRedirectUri {Id = 1, Uri = "www.redirect.com"}},
                    PostLogoutRedirectUris = new List<ClientPostLogoutRedirectUri>(){new ClientPostLogoutRedirectUri{Id = 1, Uri = "www.postRedirectUri.com"}},
                    AllowedScopes = new List<ClientScope>() { new ClientScope { Scope = "read" ,Id = 1} },
                    AccessTokenType = AccessTokenType.Jwt,
                    ClientSecrets = new List<ClientSecret>{new ClientSecret{Id = 1,Description = "removeMe",Type = "ssssshhh", Value = "nothing to see here"}},
                    IdentityProviderRestrictions = new List<ClientIdPRestriction>(){new ClientIdPRestriction{Id = 1,Provider = "www.provideme.com"}},
                    AllowedCustomGrantTypes = new List<ClientCustomGrantType>{new ClientCustomGrantType{Id = 1, GrantType = "Authorization Grant"}},
                    Claims = new List<ClientClaim>{new ClientClaim{Id = 1,Value = "tester", Type = "role"}},
                    AllowedCorsOrigins = new List<ClientCorsOrigin> { new ClientCorsOrigin { Id = 1,Origin = "www.CrossOriginMe.com"} }
                };
                db.Clients.Add(testClient);
                db.SaveChanges();
                _clientSubject = testClient.Id.ToString();
            }

            using (var db = new ScopeConfigurationDbContext(ConnectionString))
            {
                var allScopes = db.Scopes.Where(p => true);
                foreach (var c in allScopes)
                {
                    db.Scopes.Remove(c);
                }
                db.SaveChanges();
                var testScope = new Scope { Name = _scopeName,ScopeClaims = new List<ScopeClaim>{new ScopeClaim{Id = 1,Description = "To Test", Name = "testScope"}}};
                db.Scopes.Add(testScope);
                db.SaveChanges();
                _scopeSubject = testScope.Id.ToString();
            }
        }

        [TestMethod]
        public async Task CanAddClientClaim()
        {
            var clientClaim =  await _identityAdminManagerService.AddClientClaimAsync(_clientSubject, "TestClaim", "TestValue");
            Assert.IsNotNull(clientClaim);
            Assert.IsTrue(clientClaim.IsSuccess);
        }

        [TestMethod]
        public async Task CanAddClientCorsOrigin()
        {
            var clientCorsOrigin = await _identityAdminManagerService.AddClientCorsOriginAsync(_clientSubject, "http://www.test.Be");
            Assert.IsNotNull(clientCorsOrigin);
            Assert.IsTrue(clientCorsOrigin.IsSuccess);
        }

        [TestMethod]
        public async Task CanAddClientCustomGrantType()
        {
            var clientGrantType = await _identityAdminManagerService.AddClientCustomGrantTypeAsync(_clientSubject, "Authorization Grant");
            Assert.IsNotNull(clientGrantType);
            Assert.IsTrue(clientGrantType.IsSuccess);
        }

        [TestMethod]
        public async Task CanAddClientIdPRestriction()
        {
            var clientIdPRestriction = await _identityAdminManagerService.AddClientIdPRestrictionAsync(_clientSubject, "Test Provider");
            Assert.IsNotNull(clientIdPRestriction);
            Assert.IsTrue(clientIdPRestriction.IsSuccess);
        }

        [TestMethod]
        public async Task CanAddClientRedirectUri()
        {
            var clientRedirectUri = await _identityAdminManagerService.AddClientRedirectUriAsync(_clientSubject, "http://www.redirect.com");
            Assert.IsNotNull(clientRedirectUri);
            Assert.IsTrue(clientRedirectUri.IsSuccess);
        }

        [TestMethod]
        public async Task CanAddClientScope()
        {
            var clientScope = await _identityAdminManagerService.AddClientScopeAsync(_clientSubject, "read");
            Assert.IsNotNull(clientScope);
            Assert.IsTrue(clientScope.IsSuccess);
        }

        [TestMethod]
        public async Task CanAddClientSecret()
        {
            var clientSecret = await _identityAdminManagerService.AddClientSecretAsync(_clientSubject, "secretType", "secretValue");
            Assert.IsNotNull(clientSecret);
            Assert.IsTrue(clientSecret.IsSuccess);
        }

        [TestMethod]
        public async Task CanAddPostLogoutRedirectUri()
        {
            var clientPostLogoutRedirectUri = await _identityAdminManagerService.AddPostLogoutRedirectUriAsync(_clientSubject, "http://www.getmeoutofhere.com");
            Assert.IsNotNull(clientPostLogoutRedirectUri);
            Assert.IsTrue(clientPostLogoutRedirectUri.IsSuccess);
        }

        [TestMethod]
        public async Task CanAddScopeClaim()
        {
            var scopeClaim = await _identityAdminManagerService.AddScopeClaimAsync(_scopeSubject, "role", "the role", true);
            Assert.IsNotNull(scopeClaim);
            Assert.IsTrue(scopeClaim.IsSuccess);
        }

        [TestMethod]
        public async Task CanCreateClient()
        {
            var client = await _identityAdminManagerService.CreateClientAsync(new List<PropertyValue>
            {
                new PropertyValue
                {
                    Type = "ClientId",
                    Value = "Test",
                },
                new PropertyValue
                {
                    Type = "ClientName",
                    Value = "Test",
                },
            });
            Assert.IsTrue(client.IsSuccess);
            Assert.IsNotNull(client.Result);
        }

        [TestMethod]
        public async Task CanCreateScope()
        {
            var scope = await _identityAdminManagerService.CreateScopeAsync(new List<PropertyValue>
            {
                new PropertyValue
                {
                    Type = "ScopeName",
                    Value = "UnitScope",
                },
            });
            Assert.IsTrue(scope.IsSuccess);
            Assert.IsNotNull(scope.Result);
        }

        [TestMethod]
        public async Task CanDeleteClient()
        {
            var client = await _identityAdminManagerService.CreateClientAsync(new List<PropertyValue>
            {
                new PropertyValue
                {
                    Type = "ClientId",
                    Value = "TestId",
                },
                new PropertyValue
                {
                    Type = "ClientName",
                    Value = "TestName",
                },
            });
            Assert.IsTrue(client.IsSuccess);
            Assert.IsNotNull(client.Result);
            var deleted = await _identityAdminManagerService.DeleteClientAsync(client.Result.Subject);
            Assert.IsTrue(deleted.IsSuccess);
        }

        [TestMethod]
        public async Task CanDeleteScope()
        {
          var scope = await _identityAdminManagerService.CreateScopeAsync(new List<PropertyValue>
            {
                new PropertyValue
                {
                    Type = "ScopeName",
                    Value = "DeleteMe",
                },
            });
            Assert.IsTrue(scope.IsSuccess);
            Assert.IsNotNull(scope.Result);
            var deletedScope = await _identityAdminManagerService.DeleteScopeAsync(scope.Result.Subject);
            Assert.IsTrue(deletedScope.IsSuccess);
        }

        [TestMethod]
        public async Task CanGetClient()
        {
            var foundClient = await _identityAdminManagerService.GetClientAsync(_clientSubject);
            Assert.IsNotNull(foundClient);
            Assert.IsNotNull(foundClient.Result);
        }

        [TestMethod]
        public async Task CanGetMetadata()
        {
            var meta = await _identityAdminManagerService.GetMetadataAsync();
            Assert.IsNotNull(meta);
            Assert.IsNotNull(meta.ClientMetaData);
            Assert.IsNotNull(meta.ScopeMetaData);
        }

        [TestMethod]
        public async Task CanGetScope()
        {
            var foundScope = await _identityAdminManagerService.GetScopeAsync(_scopeSubject);
            Assert.IsNotNull(foundScope);
            Assert.IsNotNull(foundScope.Result);
        }

        [TestMethod]
        public async Task CanQueryClients()
        {
            var foundClients = await _identityAdminManagerService.QueryClientsAsync(_clientName, 0, 10);
            Assert.IsNotNull(foundClients);
            Assert.IsTrue(foundClients.IsSuccess);
            Assert.IsNotNull(foundClients.Result.Items.FirstOrDefault(p => p.ClientName == _clientName));
        }

        [TestMethod]
        public async Task CanQueryScopes()
        {
            var foundScopes = await _identityAdminManagerService.QueryScopesAsync(_scopeName, 0, 10);
            Assert.IsNotNull(foundScopes);
            Assert.IsTrue(foundScopes.IsSuccess);
            Assert.IsNotNull(foundScopes.Result.Items.FirstOrDefault(p => p.Name == _scopeName));
        }

        [TestMethod]
        public async Task CanRemoveClientClaim()
        {
            var client = await _identityAdminManagerService.GetClientAsync(_clientSubject);
            Assert.IsNotNull(client);
            var claim = client.Result.Claims.FirstOrDefault();	
            Assert.IsNotNull(claim);
            var removeClaim = await _identityAdminManagerService.RemoveClientClaimAsync(_clientSubject, claim.Id);
            Assert.IsTrue(removeClaim.IsSuccess);
        }

        [TestMethod]
        public async Task CanRemoveClientCorsOrigin()
        {
            var client = await _identityAdminManagerService.GetClientAsync(_clientSubject);
            Assert.IsNotNull(client);
            var corsOrigin = client.Result.AllowedCorsOrigins.FirstOrDefault();
            Assert.IsNotNull(corsOrigin);
            var removeCorsOrigin = await _identityAdminManagerService.RemoveClientCorsOriginAsync(_clientSubject, corsOrigin.Id);
            Assert.IsTrue(removeCorsOrigin.IsSuccess);
        }

        [TestMethod]
        public async Task CanRemoveClientCustomGrantType(string subject, string id)
        {
            var client = await _identityAdminManagerService.GetClientAsync(_clientSubject);
            Assert.IsNotNull(client);
            var grant = client.Result.AllowedCustomGrantTypes.FirstOrDefault();
            Assert.IsNotNull(grant);
            var removeGrant = await _identityAdminManagerService.RemoveClientCustomGrantTypeAsync(_clientSubject, grant.Id);
            Assert.IsTrue(removeGrant.IsSuccess);
        }

        [TestMethod]
        public async Task CanRemoveClientIdPRestriction(string subject, string id)
        {
            var client = await _identityAdminManagerService.GetClientAsync(_clientSubject);
            Assert.IsNotNull(client);
            var identityProviderRestrictions = client.Result.IdentityProviderRestrictions.FirstOrDefault();
            Assert.IsNotNull(identityProviderRestrictions);
            var removeIdpvrRestr = await _identityAdminManagerService.RemoveClientIdPRestrictionAsync(_clientSubject, identityProviderRestrictions.Id);
            Assert.IsTrue(removeIdpvrRestr.IsSuccess);
        }

        [TestMethod]
        public async Task CanRemoveClientRedirectUri(string subject, string id)
        {
            var client = await _identityAdminManagerService.GetClientAsync(_clientSubject);
            Assert.IsNotNull(client);
            var redirectUri = client.Result.RedirectUris.FirstOrDefault();
            Assert.IsNotNull(redirectUri);
            var removeRedirectUri = await _identityAdminManagerService.RemoveClientRedirectUriAsync(_clientSubject, redirectUri.Id);
            Assert.IsTrue(removeRedirectUri.IsSuccess);
        }

        [TestMethod]
        public async Task CanRemoveClientScope(string subject, string id)
        {
            var client = await _identityAdminManagerService.GetClientAsync(_clientSubject);
            Assert.IsNotNull(client);
            var foundClientScope = client.Result.AllowedScopes.FirstOrDefault();
            Assert.IsNotNull(foundClientScope);
            var removeClientScope = await _identityAdminManagerService.RemoveClientScopeAsync(_clientSubject, foundClientScope.Id);
            Assert.IsTrue(removeClientScope.IsSuccess);
        }

        [TestMethod]
        public async Task CanRemoveClientSecret(string subject, string id)
        {
            var client = await _identityAdminManagerService.GetClientAsync(_clientSubject);
            Assert.IsNotNull(client);
            var secret = client.Result.ClientSecrets.FirstOrDefault();
            Assert.IsNotNull(secret);
            var removeSecret = await _identityAdminManagerService.RemoveClientSecretAsync(_clientSubject, secret.Id);
            Assert.IsTrue(removeSecret.IsSuccess);
        }

        [TestMethod]
        public async Task CanRemovePostLogoutRedirectUri(string subject, string id)
        {
            var client = await _identityAdminManagerService.GetClientAsync(_clientSubject);
            Assert.IsNotNull(client);
            var postLogoutRedirectUri = client.Result.PostLogoutRedirectUris.FirstOrDefault();
            Assert.IsNotNull(postLogoutRedirectUri);
            var removePostLogoutRedirectUri = await _identityAdminManagerService.RemovePostLogoutRedirectUriAsync(_clientSubject, postLogoutRedirectUri.Id);
            Assert.IsTrue(removePostLogoutRedirectUri.IsSuccess);
        }

        [TestMethod]
        public async Task CanRemoveScopeClaim(string subject, string id)
        {
            var scope = await _identityAdminManagerService.GetScopeAsync(_scopeSubject);
            Assert.IsNotNull(scope);
            var scopeClaim = scope.Result.ScopeClaimValues.FirstOrDefault();
            Assert.IsNotNull(scopeClaim);
            var removeScopeClaim = await _identityAdminManagerService.RemoveScopeClaimAsync(_scopeSubject, scopeClaim.Id);
            Assert.IsTrue(removeScopeClaim.IsSuccess);
        }

        [TestMethod]
        public async Task CanSetClientProperty(string subject, string type, string value)
        {
            var clientProp =  await _identityAdminManagerService.SetClientPropertyAsync(_clientSubject, "RequireConsent", "false");
            Assert.IsNotNull(clientProp);
            Assert.IsTrue(clientProp.IsSuccess);
            var client = await _identityAdminManagerService.GetClientAsync(_clientSubject);
            Assert.IsNotNull(client);
            var consent = client.Result.Properties.FirstOrDefault(p => p.Value == "RequireConsent");
            Assert.AreEqual(consent, "true");
        }

        [TestMethod]
        public async Task CanSetScopeProperty(string subject, string type, string value)
        {
            var scopeProperty = await _identityAdminManagerService.SetScopePropertyAsync(_scopeSubject, "Description", "toTest");
            Assert.IsNotNull(scopeProperty);
            Assert.IsTrue(scopeProperty.IsSuccess);
            var scope = await _identityAdminManagerService.GetScopeAsync(_scopeSubject);
            Assert.IsNotNull(scope);
            var desc = scope.Result.Properties.FirstOrDefault(p => p.Value == "Description");
            Assert.AreEqual(desc, "toTest");
        }
    }
}