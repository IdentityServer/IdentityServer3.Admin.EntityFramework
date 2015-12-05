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
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using IdentityAdmin.Core;
using IdentityAdmin.Core.Client;
using IdentityAdmin.Core.Metadata;
using IdentityAdmin.Core.Scope;
using IdentityAdmin.Extensions;
using IdentityServer3.Admin.EntityFramework.Entities;
using IdentityServer3.Admin.EntityFramework.Interfaces;
using IdentityServer3.EntityFramework;
using IdentityServer3.EntityFramework.Entities;

namespace IdentityServer3.Admin.EntityFramework
{
    public class IdentityAdminCoreManager<TCLient, TClientKey, TScope, TScopeKey> : IIdentityAdminService
        where TCLient : class, IClient<TClientKey>, new()
        where TClientKey : IEquatable<TClientKey>
        where TScope : class, IScope<TScopeKey>, new()
        where TScopeKey : IEquatable<TScopeKey>
    {
        private readonly string _connectionString;
        private readonly EntityFrameworkServiceOptions _entityFrameworkServiceOptions;
        
        public IdentityAdminCoreManager(string connectionString, bool createIfNotExist = false)
        {
            if (createIfNotExist)
            {
                
            }

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException("A connectionstring or name is needed to initialize the IdentityAdmin");
            }
            _connectionString = connectionString;
            _entityFrameworkServiceOptions = new EntityFrameworkServiceOptions();
            Mapper.CreateMap<IdentityClient, Client>();
            Mapper.CreateMap<Client, IdentityClient>();
            Mapper.CreateMap<ClientClaim, ClientClaimValue>();
            Mapper.CreateMap<ClientClaimValue, ClientClaim>();
            Mapper.CreateMap<ClientSecret, ClientSecretValue>();
            Mapper.CreateMap<ClientSecretValue, ClientSecret>();
            Mapper.CreateMap<ClientIdPRestriction, ClientIdPRestrictionValue>();
            Mapper.CreateMap<ClientIdPRestrictionValue, ClientIdPRestriction>();
            Mapper.CreateMap<ClientPostLogoutRedirectUri, ClientPostLogoutRedirectUriValue>();
            Mapper.CreateMap<ClientPostLogoutRedirectUriValue, ClientPostLogoutRedirectUri>();
            Mapper.CreateMap<ClientRedirectUri, ClientRedirectUriValue>();
            Mapper.CreateMap<ClientRedirectUriValue, ClientRedirectUri>();
            Mapper.CreateMap<ClientCorsOrigin, ClientCorsOriginValue>();
            Mapper.CreateMap<ClientCorsOriginValue, ClientCorsOrigin>();
            Mapper.CreateMap<ClientCustomGrantType, ClientCustomGrantTypeValue>();
            Mapper.CreateMap<ClientCustomGrantTypeValue, ClientCustomGrantType>();
            Mapper.CreateMap<ClientScope, ClientScopeValue>();
            Mapper.CreateMap<ClientScopeValue, ClientScope>();
            Mapper.CreateMap<ScopeClaim, ScopeClaimValue>();
            Mapper.CreateMap<ScopeClaimValue, ScopeClaim>();
            Mapper.CreateMap<IdentityScope, Scope>();
            Mapper.CreateMap<Scope, IdentityScope>();
        }
        public Task<IdentityAdminMetadata> GetMetadataAsync()
        {
            var updateClient = new List<PropertyMetadata>();
            updateClient.AddRange(PropertyMetadata.FromType<TCLient>());

            var createClient = new List<PropertyMetadata>
            {
                PropertyMetadata.FromProperty<TCLient>(x => x.ClientName,"ClientName", required: true),
                PropertyMetadata.FromProperty<TCLient>(x => x.ClientId,"ClientId", required: true),
            };

            var client = new ClientMetaData
            {
                SupportsCreate = true,
                SupportsDelete = true,
                CreateProperties = createClient,
                UpdateProperties = updateClient
            };


            var updateScope = new List<PropertyMetadata>();
            updateScope.AddRange(PropertyMetadata.FromType<TScope>());

            var createScope = new List<PropertyMetadata>
            {
                PropertyMetadata.FromProperty<TScope>(x => x.Name,"ScopeName", required: true),
            };

            var scope = new ScopeMetaData
            {
                SupportsCreate = true,
                SupportsDelete = true,
                CreateProperties = createScope,
                UpdateProperties = updateScope
            };


            var identityAdminMetadata = new IdentityAdminMetadata
            {
                ClientMetaData = client,
                ScopeMetaData = scope
            };

            return Task.FromResult(identityAdminMetadata);
        }

        #region Scope

        public async Task<IdentityAdminResult<ScopeDetail>> GetScopeAsync(string subject)
        {
            using (var db = new ScopeConfigurationDbContext(_connectionString, _entityFrameworkServiceOptions.Schema))
            {
                int parsedId;
                if (int.TryParse(subject, out parsedId))
                {
                    var efScope = await db.Scopes.FirstOrDefaultAsync(p => p.Id == parsedId);
                    if (efScope == null)
                    {
                        return new IdentityAdminResult<ScopeDetail>((ScopeDetail)null);
                    }
                    var coreScope = new TScope();
                    Mapper.Map(efScope, coreScope);
                    var result = new ScopeDetail
                    {
                        Subject = subject,
                        Name = efScope.Name,
                        Description = efScope.Description,
                    };

                    var metadata = await GetMetadataAsync();
                    var props = from prop in metadata.ScopeMetaData.UpdateProperties
                                select new PropertyValue
                                {
                                    Type = prop.Type,
                                    Value = GetScopeProperty(prop, coreScope),
                                };

                    result.Properties = props.ToArray();
                    result.ScopeClaimValues = new List<ScopeClaimValue>();
                    Mapper.Map(efScope.ScopeClaims.ToList(), result.ScopeClaimValues);

                    return new IdentityAdminResult<ScopeDetail>(result);
                }
                return new IdentityAdminResult<ScopeDetail>((ScopeDetail)null);
            }
        }

        public Task<IdentityAdminResult<QueryResult<ScopeSummary>>> QueryScopesAsync(string filter, int start, int count)
        {
            using (var db = new ScopeConfigurationDbContext(_connectionString, _entityFrameworkServiceOptions.Schema))
            {
                var query =
                    from scope in db.Scopes
                    orderby scope.Name
                    select scope;

                if (!String.IsNullOrWhiteSpace(filter))
                {
                    query =
                        from scope in query
                        where scope.Name.Contains(filter)
                        orderby scope.Name
                        select scope;
                }

                int total = query.Count();
                var scopes = query.Skip(start).Take(count).ToArray();

                var result = new QueryResult<ScopeSummary>
                {
                    Start = start,
                    Count = count,
                    Total = total,
                    Filter = filter,
                    Items = scopes.Select(x =>
                    {
                        var scope = new ScopeSummary
                        {
                            Subject = x.Id.ToString(),
                            Name = x.Name,
                            Description = x.Name
                        };

                        return scope;
                    }).ToArray()
                };

                return Task.FromResult(new IdentityAdminResult<QueryResult<ScopeSummary>>(result));
            }
        }

        public async Task<IdentityAdminResult<CreateResult>> CreateScopeAsync(IEnumerable<PropertyValue> properties)
        {
            var scopeName = properties.Single(x => x.Type == "ScopeName");
            var scopeNameValue = scopeName.Value;
            string[] exclude = { "ScopeName" };
            var otherProperties = properties.Where(x => !exclude.Contains(x.Type)).ToArray();
            var metadata = await GetMetadataAsync();
            var createProps = metadata.ScopeMetaData.CreateProperties;
            var scope = new TScope { Name = scopeNameValue };

            foreach (var prop in otherProperties)
            {
                var propertyResult = SetScopeProperty(createProps, scope, prop.Type, prop.Value);
                if (!propertyResult.IsSuccess)
                {
                    return new IdentityAdminResult<CreateResult>(propertyResult.Errors.ToArray());
                }
            }
            var efSCope = new Scope();
            using (var db = new ScopeConfigurationDbContext(_connectionString, _entityFrameworkServiceOptions.Schema))
            {
                try
                {
                    Mapper.Map(scope, efSCope);
                    db.Scopes.Add(efSCope);
                    db.SaveChanges();
                }
                catch (SqlException ex)
                {
                    return new IdentityAdminResult<CreateResult>(ex.Message);
                }
            }

            return new IdentityAdminResult<CreateResult>(new CreateResult { Subject = efSCope.Id.ToString() });
        }

        public async Task<IdentityAdminResult> SetScopePropertyAsync(string subject, string type, string value)
        {
            int parsedSubject;
            if (int.TryParse(subject, out  parsedSubject))
            {
                using (var db = new ScopeConfigurationDbContext(_connectionString, _entityFrameworkServiceOptions.Schema))
                {
                    try
                    {
                        var efScope = await db.Scopes.FirstOrDefaultAsync(p => p.Id == parsedSubject);
                        if (efScope == null)
                        {
                            return new IdentityAdminResult("Invalid subject");
                        }
                        var meta = await GetMetadataAsync();
                        var coreScope = new TScope();
                        Mapper.Map(efScope, coreScope);
                        var propResult = SetScopeProperty(meta.ScopeMetaData.UpdateProperties, coreScope, type, value);
                        if (!propResult.IsSuccess)
                        {
                            return propResult;
                        }
                        Mapper.Map(coreScope, efScope);

                        await db.SaveChangesAsync();

                        return IdentityAdminResult.Success;
                    }
                    catch (SqlException ex)
                    {
                        return new IdentityAdminResult(ex.Message);
                    }
                }
            }
            return new IdentityAdminResult("Invalid subject");

        }

        public async Task<IdentityAdminResult> DeleteScopeAsync(string subject)
        {
            int parsedSubject;
            if (int.TryParse(subject, out  parsedSubject))
            {
                using (var db = new ScopeConfigurationDbContext(_connectionString, _entityFrameworkServiceOptions.Schema))
                {
                    try
                    {
                        var scope = await db.Scopes.FirstOrDefaultAsync(p => p.Id == parsedSubject);
                        if (scope == null)
                        {
                            return new IdentityAdminResult("Invalid subject");
                        }
                        db.Scopes.Remove(scope);
                        db.SaveChanges();
                        return IdentityAdminResult.Success;
                    }
                    catch (SqlException ex)
                    {
                        return new IdentityAdminResult(ex.Message);
                    }
                }
            }

            return new IdentityAdminResult("Invalid subject");
        }

        #region Scope claim

        public async Task<IdentityAdminResult> AddScopeClaimAsync(string subject, string name, string description, bool alwaysIncludeInIdToken)
        {
            int parsedSubject;
            if (int.TryParse(subject, out parsedSubject))
            {
                using (var db = new ScopeConfigurationDbContext(_connectionString, _entityFrameworkServiceOptions.Schema))
                {
                    try
                    {
                        var scope = await db.Scopes.FirstOrDefaultAsync(p => p.Id == parsedSubject);
                        if (scope == null)
                        {
                            return new IdentityAdminResult("Invalid subject");
                        }
                        var existingClaims = scope.ScopeClaims;
                        if (!existingClaims.Any(x => x.Name == name && x.Description == description))
                        {
                            scope.ScopeClaims.Add(new ScopeClaim
                            {
                                Name = name,
                                Description = description,
                                AlwaysIncludeInIdToken = alwaysIncludeInIdToken
                            });
                            db.SaveChanges();
                        }
                        return IdentityAdminResult.Success;
                    }
                    catch (SqlException ex)
                    {
                        return new IdentityAdminResult<CreateResult>(ex.Message);
                    }
                }
            }
            return new IdentityAdminResult("Invalid subject");
        }

        public async Task<IdentityAdminResult> RemoveScopeClaimAsync(string subject, string id)
        {
            int parsedSubject;
            int parsedScopeId;
            if (int.TryParse(subject, out parsedSubject) && int.TryParse(id, out parsedScopeId))
            {
                using (var db = new ScopeConfigurationDbContext(_connectionString, _entityFrameworkServiceOptions.Schema))
                {
                    try
                    {
                        var scope = await db.Scopes.FirstOrDefaultAsync(p => p.Id == parsedSubject);
                        if (scope == null)
                        {
                            return new IdentityAdminResult("Invalid subject");
                        }
                        var existingClaim = scope.ScopeClaims.FirstOrDefault(p => p.Id == parsedScopeId);
                        if (existingClaim != null)
                        {
                            scope.ScopeClaims.Remove(existingClaim);
                            await db.SaveChangesAsync();
                        }
                        return IdentityAdminResult.Success;
                    }
                    catch (SqlException ex)
                    {
                        return new IdentityAdminResult<CreateResult>(ex.Message);
                    }
                }
            }
            return new IdentityAdminResult("Invalid subject or clientId");
        }

        #endregion
        #endregion

        #region Client
        public async Task<IdentityAdminResult<ClientDetail>> GetClientAsync(string subject)
        {
            using (var db = new ClientConfigurationDbContext(_connectionString, _entityFrameworkServiceOptions.Schema))
            {
                int parsedId;
                if (int.TryParse(subject, out parsedId))
                {
                    var eFclient = await db.Clients.FirstOrDefaultAsync(p => p.Id == parsedId);
                    if (eFclient == null)
                    {
                        return new IdentityAdminResult<ClientDetail>((ClientDetail)null);
                    }
                    var coreClient = new TCLient();
                    Mapper.Map(eFclient, coreClient);
                    var result = new ClientDetail
                    {
                        Subject = subject,
                        ClientId = eFclient.ClientId,
                        ClientName = eFclient.ClientName,
                    };


                    var metadata = await GetMetadataAsync();
                    var props = from prop in metadata.ClientMetaData.UpdateProperties
                                select new PropertyValue
                                {
                                    Type = prop.Type,
                                    Value = GetClientProperty(prop, coreClient),
                                };

                    result.Properties = props.ToArray();

                    result.AllowedCorsOrigins = new List<ClientCorsOriginValue>();
                    Mapper.Map(eFclient.AllowedCorsOrigins.ToList(), result.AllowedCorsOrigins);
                    result.AllowedCustomGrantTypes = new List<ClientCustomGrantTypeValue>();
                    Mapper.Map(eFclient.AllowedCustomGrantTypes.ToList(), result.AllowedCustomGrantTypes);
                    result.AllowedScopes = new List<ClientScopeValue>();
                    Mapper.Map(eFclient.AllowedScopes.ToList(), result.AllowedScopes);
                    result.Claims = new List<ClientClaimValue>();
                    Mapper.Map(eFclient.Claims.ToList(), result.Claims);
                    result.ClientSecrets = new List<ClientSecretValue>();
                    Mapper.Map(eFclient.ClientSecrets.ToList(), result.ClientSecrets);
                    result.IdentityProviderRestrictions = new List<ClientIdPRestrictionValue>();
                    Mapper.Map(eFclient.IdentityProviderRestrictions.ToList(), result.IdentityProviderRestrictions);
                    result.PostLogoutRedirectUris = new List<ClientPostLogoutRedirectUriValue>();
                    Mapper.Map(eFclient.PostLogoutRedirectUris.ToList(), result.PostLogoutRedirectUris);
                    result.RedirectUris = new List<ClientRedirectUriValue>();
                    Mapper.Map(eFclient.RedirectUris.ToList(), result.RedirectUris);

                    return new IdentityAdminResult<ClientDetail>(result);
                }
                return new IdentityAdminResult<ClientDetail>((ClientDetail)null);
            }
        }

        public Task<IdentityAdminResult<QueryResult<ClientSummary>>> QueryClientsAsync(string filter, int start, int count)
        {
            using (var db = new ClientConfigurationDbContext(_connectionString, _entityFrameworkServiceOptions.Schema))
            {
                var query =
                    from client in db.Clients
                    orderby client.ClientName
                    select client;

                if (!String.IsNullOrWhiteSpace(filter))
                {
                    query =
                        from client in query
                        where client.ClientName.Contains(filter)
                        orderby client.ClientName
                        select client;
                }

                int total = query.Count();
                var clients = query.Skip(start).Take(count).ToArray();

                var result = new QueryResult<ClientSummary>
                {
                    Start = start,
                    Count = count,
                    Total = total,
                    Filter = filter,
                    Items = clients.Select(x =>
                    {
                        var client = new ClientSummary
                        {
                            Subject = x.Id.ToString(),
                            ClientName = x.ClientName,
                            ClientId = x.ClientId
                        };

                        return client;
                    }).ToArray()
                };

                return Task.FromResult(new IdentityAdminResult<QueryResult<ClientSummary>>(result));
            }
        }

        public async Task<IdentityAdminResult<CreateResult>> CreateClientAsync(IEnumerable<PropertyValue> properties)
        {
            var clientNameClaim = properties.Single(x => x.Type == "ClientName");
            var clientIdClaim = properties.Single(x => x.Type == "ClientId");

            var clientId = clientNameClaim.Value;
            var clientName = clientIdClaim.Value;

            string[] exclude = new string[] { "ClientName", "ClientId" };
            var otherProperties = properties.Where(x => !exclude.Contains(x.Type)).ToArray();

            var metadata = await GetMetadataAsync();
            var createProps = metadata.ClientMetaData.CreateProperties;

            var client = new TCLient { ClientId = clientId, ClientName = clientName };
            foreach (var prop in otherProperties)
            {
                var propertyResult = SetClientProperty(createProps, client, prop.Type, prop.Value);
                if (!propertyResult.IsSuccess)
                {
                    return new IdentityAdminResult<CreateResult>(propertyResult.Errors.ToArray());
                }
            }

            var efClient = new Client();
            using (var db = new ClientConfigurationDbContext(_connectionString, _entityFrameworkServiceOptions.Schema))
            {
                try
                {
                    Mapper.Map(client, efClient);
                    db.Clients.Add(efClient);
                    db.SaveChanges();
                }
                catch (SqlException ex)
                {
                    return new IdentityAdminResult<CreateResult>(ex.Message);
                }
            }

            return new IdentityAdminResult<CreateResult>(new CreateResult { Subject = efClient.Id.ToString() });
        }

        public async Task<IdentityAdminResult> SetClientPropertyAsync(string subject, string type, string value)
        {
            int parsedSubject;
            if (int.TryParse(subject, out  parsedSubject))
            {
                using (var db = new ClientConfigurationDbContext(_connectionString, _entityFrameworkServiceOptions.Schema))
                {
                    try
                    {
                        var eFclient = await db.Clients.FirstOrDefaultAsync(p => p.Id == parsedSubject);
                        if (eFclient == null)
                        {
                            return new IdentityAdminResult("Invalid subject");
                        }
                        var meta = await GetMetadataAsync();
                        var coreClient = new TCLient();
                        Mapper.Map(eFclient, coreClient);
                        var propResult = SetClientProperty(meta.ClientMetaData.UpdateProperties, coreClient, type, value);
                        if (!propResult.IsSuccess)
                        {
                            return propResult;
                        }
                        Mapper.Map(coreClient, eFclient);

                        await db.SaveChangesAsync();

                        return IdentityAdminResult.Success;
                    }
                    catch (SqlException ex)
                    {
                        return new IdentityAdminResult(ex.Message);
                    }
                }
            }
            return new IdentityAdminResult("Invalid subject");

        }

        public async Task<IdentityAdminResult> DeleteClientAsync(string subject)
        {
            int parsedSubject;
            if (int.TryParse(subject, out  parsedSubject))
            {
                using (var db = new ClientConfigurationDbContext(_connectionString, _entityFrameworkServiceOptions.Schema))
                {
                    try
                    {
                        var client = await db.Clients.FirstOrDefaultAsync(p => p.Id == parsedSubject);
                        if (client == null)
                        {
                            return new IdentityAdminResult("Invalid subject");
                        }
                        db.Clients.Remove(client);
                        db.SaveChanges();
                        return IdentityAdminResult.Success;
                    }
                    catch (SqlException ex)
                    {
                        return new IdentityAdminResult(ex.Message);
                    }
                }
            }

            return new IdentityAdminResult("Invalid subject");
        }

        #region Client claim

        public async Task<IdentityAdminResult> AddClientClaimAsync(string subject, string type, string value)
        {
            int parsedSubject;
            if (int.TryParse(subject, out parsedSubject))
            {
                using (var db = new ClientConfigurationDbContext(_connectionString, _entityFrameworkServiceOptions.Schema))
                {
                    try
                    {
                        var client = await db.Clients.FirstOrDefaultAsync(p => p.Id == parsedSubject);
                        if (client == null)
                        {
                            return new IdentityAdminResult("Invalid subject");
                        }
                        var existingClaims = client.Claims;
                        if (!existingClaims.Any(x => x.Type == type && x.Value == value))
                        {
                            client.Claims.Add(new ClientClaim
                            {
                                Type = type,
                                Value = value
                            });
                            db.SaveChanges();
                        }
                        return IdentityAdminResult.Success;
                    }
                    catch (SqlException ex)
                    {
                        return new IdentityAdminResult<CreateResult>(ex.Message);
                    }
                }
            }
            return new IdentityAdminResult("Invalid subject");
        }

        public async Task<IdentityAdminResult> RemoveClientClaimAsync(string subject, string id)
        {
            int parsedSubject;
            int parsedClientId;
            if (int.TryParse(subject, out parsedSubject) && int.TryParse(id, out parsedClientId))
            {
                using (var db = new ClientConfigurationDbContext(_connectionString, _entityFrameworkServiceOptions.Schema))
                {
                    try
                    {
                        var client = await db.Clients.FirstOrDefaultAsync(p => p.Id == parsedSubject);
                        if (client == null)
                        {
                            return new IdentityAdminResult("Invalid subject");
                        }
                        var existingClaim = client.Claims.FirstOrDefault(p => p.Id == parsedClientId);
                        if (existingClaim != null)
                        {
                            client.Claims.Remove(existingClaim);
                            await db.SaveChangesAsync();
                        }
                        return IdentityAdminResult.Success;
                    }
                    catch (SqlException ex)
                    {
                        return new IdentityAdminResult<CreateResult>(ex.Message);
                    }
                }
            }
            return new IdentityAdminResult("Invalid subject or clientId");
        }

        #endregion

        #region Client Secret

        public async Task<IdentityAdminResult> AddClientSecretAsync(string subject, string type, string value)
        {
            int parsedSubject;
            if (int.TryParse(subject, out parsedSubject))
            {
                using (var db = new ClientConfigurationDbContext(_connectionString, _entityFrameworkServiceOptions.Schema))
                {
                    try
                    {
                        var client = await db.Clients.FirstOrDefaultAsync(p => p.Id == parsedSubject);
                        if (client == null)
                        {
                            return new IdentityAdminResult("Invalid subject");
                        }
                        var existingSecrets = client.ClientSecrets;
                        if (!existingSecrets.Any(x => x.Type == type && x.Value == value))
                        {
                            client.ClientSecrets.Add(new ClientSecret
                            {
                                Type = type,
                                Value = value
                            });
                            db.SaveChanges();
                        }
                        return IdentityAdminResult.Success;
                    }
                    catch (SqlException ex)
                    {
                        return new IdentityAdminResult<CreateResult>(ex.Message);
                    }
                }
            }
            return new IdentityAdminResult("Invalid subject");
        }

        public async Task<IdentityAdminResult> RemoveClientSecretAsync(string subject, string id)
        {
            int parsedSubject;
            int parsedObjectId;
            if (int.TryParse(subject, out parsedSubject) && int.TryParse(id, out parsedObjectId))
            {
                using (var db = new ClientConfigurationDbContext(_connectionString, _entityFrameworkServiceOptions.Schema))
                {
                    try
                    {
                        var client = await db.Clients.FirstOrDefaultAsync(p => p.Id == parsedSubject);
                        if (client == null)
                        {
                            return new IdentityAdminResult("Invalid subject");
                        }
                        var existingClientSecret = client.ClientSecrets.FirstOrDefault(p => p.Id == parsedObjectId);
                        if (existingClientSecret != null)
                        {
                            client.ClientSecrets.Remove(existingClientSecret);
                            await db.SaveChangesAsync();
                        }
                        return IdentityAdminResult.Success;
                    }
                    catch (SqlException ex)
                    {
                        return new IdentityAdminResult<CreateResult>(ex.Message);
                    }
                }
            }
            return new IdentityAdminResult("Invalid subject or secretId");
        }

        #endregion

        #region ClientIdPRestriction

        public async Task<IdentityAdminResult> AddClientIdPRestrictionAsync(string subject, string provider)
        {
            int parsedSubject;
            if (int.TryParse(subject, out parsedSubject))
            {
                using (var db = new ClientConfigurationDbContext(_connectionString, _entityFrameworkServiceOptions.Schema))
                {
                    try
                    {
                        var client = await db.Clients.FirstOrDefaultAsync(p => p.Id == parsedSubject);
                        if (client == null)
                        {
                            return new IdentityAdminResult("Invalid subject");
                        }
                        var existingIdentityProviderRestrictions = client.IdentityProviderRestrictions;
                        if (existingIdentityProviderRestrictions.All(x => x.Provider != provider))
                        {
                            client.IdentityProviderRestrictions.Add(new ClientIdPRestriction
                            {
                                Provider = provider,
                            });
                            db.SaveChanges();
                        }
                        return IdentityAdminResult.Success;
                    }
                    catch (SqlException ex)
                    {
                        return new IdentityAdminResult<CreateResult>(ex.Message);
                    }
                }
            }
            return new IdentityAdminResult("Invalid subject");
        }

        public async Task<IdentityAdminResult> RemoveClientIdPRestrictionAsync(string subject, string id)
        {
            int parsedSubject;
            int parsedObjectId;
            if (int.TryParse(subject, out parsedSubject) && int.TryParse(id, out parsedObjectId))
            {
                using (var db = new ClientConfigurationDbContext(_connectionString, _entityFrameworkServiceOptions.Schema))
                {
                    try
                    {
                        var client = await db.Clients.FirstOrDefaultAsync(p => p.Id == parsedSubject);
                        if (client == null)
                        {
                            return new IdentityAdminResult("Invalid subject");
                        }
                        var existingIdentityProviderRestrictions = client.IdentityProviderRestrictions.FirstOrDefault(p => p.Id == parsedObjectId);
                        if (existingIdentityProviderRestrictions != null)
                        {
                            client.IdentityProviderRestrictions.Remove(existingIdentityProviderRestrictions);
                            await db.SaveChangesAsync();
                        }
                        return IdentityAdminResult.Success;
                    }
                    catch (SqlException ex)
                    {
                        return new IdentityAdminResult<CreateResult>(ex.Message);
                    }
                }
            }
            return new IdentityAdminResult("Invalid subject or secretId");
        }

        #endregion

        #region PostLogoutRedirectUri

        public async Task<IdentityAdminResult> AddPostLogoutRedirectUriAsync(string subject, string uri)
        {
            int parsedSubject;
            if (int.TryParse(subject, out parsedSubject))
            {
                using (var db = new ClientConfigurationDbContext(_connectionString, _entityFrameworkServiceOptions.Schema))
                {
                    try
                    {
                        var client = await db.Clients.FirstOrDefaultAsync(p => p.Id == parsedSubject);
                        if (client == null)
                        {
                            return new IdentityAdminResult("Invalid subject");
                        }
                        var existingPostLogoutRedirectUris = client.PostLogoutRedirectUris;
                        if (existingPostLogoutRedirectUris.All(x => x.Uri != uri))
                        {
                            client.PostLogoutRedirectUris.Add(new ClientPostLogoutRedirectUri
                            {
                                Uri = uri,
                            });
                            db.SaveChanges();
                        }
                        return IdentityAdminResult.Success;
                    }
                    catch (SqlException ex)
                    {
                        return new IdentityAdminResult<CreateResult>(ex.Message);
                    }
                }
            }
            return new IdentityAdminResult("Invalid subject");
        }

        public async Task<IdentityAdminResult> RemovePostLogoutRedirectUriAsync(string subject, string id)
        {
            int parsedSubject;
            int parsedObjectId;
            if (int.TryParse(subject, out parsedSubject) && int.TryParse(id, out parsedObjectId))
            {
                using (var db = new ClientConfigurationDbContext(_connectionString, _entityFrameworkServiceOptions.Schema))
                {
                    try
                    {
                        var client = await db.Clients.FirstOrDefaultAsync(p => p.Id == parsedSubject);
                        if (client == null)
                        {
                            return new IdentityAdminResult("Invalid subject");
                        }

                        var existingPostLogoutRedirectUris = client.PostLogoutRedirectUris.FirstOrDefault(p => p.Id == parsedObjectId);
                        if (existingPostLogoutRedirectUris != null)
                        {
                            client.PostLogoutRedirectUris.Remove(existingPostLogoutRedirectUris);
                            await db.SaveChangesAsync();
                        }
                        return IdentityAdminResult.Success;
                    }
                    catch (SqlException ex)
                    {
                        return new IdentityAdminResult<CreateResult>(ex.Message);
                    }
                }
            }
            return new IdentityAdminResult("Invalid subject or secretId");
        }

        #endregion

        #region ClientRedirectUri

        public async Task<IdentityAdminResult> AddClientRedirectUriAsync(string subject, string uri)
        {
            int parsedSubject;
            if (int.TryParse(subject, out parsedSubject))
            {
                using (var db = new ClientConfigurationDbContext(_connectionString, _entityFrameworkServiceOptions.Schema))
                {
                    try
                    {
                        var client = await db.Clients.FirstOrDefaultAsync(p => p.Id == parsedSubject);
                        if (client == null)
                        {
                            return new IdentityAdminResult("Invalid subject");
                        }
                        var existingRedirectUris = client.RedirectUris;
                        if (existingRedirectUris.All(x => x.Uri != uri))
                        {
                            client.RedirectUris.Add(new ClientRedirectUri
                            {
                                Uri = uri,
                            });
                            db.SaveChanges();
                        }
                        return IdentityAdminResult.Success;
                    }
                    catch (SqlException ex)
                    {
                        return new IdentityAdminResult<CreateResult>(ex.Message);
                    }
                }
            }
            return new IdentityAdminResult("Invalid subject");
        }

        public async Task<IdentityAdminResult> RemoveClientRedirectUriAsync(string subject, string id)
        {
            int parsedSubject;
            int parsedObjectId;
            if (int.TryParse(subject, out parsedSubject) && int.TryParse(id, out parsedObjectId))
            {
                using (var db = new ClientConfigurationDbContext(_connectionString, _entityFrameworkServiceOptions.Schema))
                {
                    try
                    {
                        var client = await db.Clients.FirstOrDefaultAsync(p => p.Id == parsedSubject);
                        if (client == null)
                        {
                            return new IdentityAdminResult("Invalid subject");
                        }
                        var existingRedirectUris = client.RedirectUris.FirstOrDefault(p => p.Id == parsedObjectId);
                        if (existingRedirectUris != null)
                        {
                            client.RedirectUris.Remove(existingRedirectUris);
                            await db.SaveChangesAsync();
                        }
                        return IdentityAdminResult.Success;
                    }
                    catch (SqlException ex)
                    {
                        return new IdentityAdminResult<CreateResult>(ex.Message);
                    }
                }
            }
            return new IdentityAdminResult("Invalid subject or secretId");
        }

        #endregion

        #region ClientCorsOrigin

        public async Task<IdentityAdminResult> AddClientCorsOriginAsync(string subject, string origin)
        {
            int parsedSubject;
            if (int.TryParse(subject, out parsedSubject))
            {
                using (var db = new ClientConfigurationDbContext(_connectionString, _entityFrameworkServiceOptions.Schema))
                {
                    try
                    {
                        var client = await db.Clients.FirstOrDefaultAsync(p => p.Id == parsedSubject);
                        if (client == null)
                        {
                            return new IdentityAdminResult("Invalid subject");
                        }
                        var existingCorsOrigins = client.AllowedCorsOrigins;
                        if (existingCorsOrigins.All(x => x.Origin != origin))
                        {
                            client.AllowedCorsOrigins.Add(new ClientCorsOrigin
                            {
                                Origin = origin,
                            });
                            db.SaveChanges();
                        }
                        return IdentityAdminResult.Success;
                    }
                    catch (SqlException ex)
                    {
                        return new IdentityAdminResult<CreateResult>(ex.Message);
                    }
                }
            }
            return new IdentityAdminResult("Invalid subject");
        }

        public async Task<IdentityAdminResult> RemoveClientCorsOriginAsync(string subject, string id)
        {
            int parsedSubject;
            int parsedObjectId;
            if (int.TryParse(subject, out parsedSubject) && int.TryParse(id, out parsedObjectId))
            {
                using (var db = new ClientConfigurationDbContext(_connectionString, _entityFrameworkServiceOptions.Schema))
                {
                    try
                    {
                        var client = await db.Clients.FirstOrDefaultAsync(p => p.Id == parsedSubject);
                        if (client == null)
                        {
                            return new IdentityAdminResult("Invalid subject");
                        }
                        var existingCorsOrigins = client.AllowedCorsOrigins.FirstOrDefault(p => p.Id == parsedObjectId);
                        if (existingCorsOrigins != null)
                        {
                            client.AllowedCorsOrigins.Remove(existingCorsOrigins);
                            await db.SaveChangesAsync();
                        }
                        return IdentityAdminResult.Success;
                    }
                    catch (SqlException ex)
                    {
                        return new IdentityAdminResult<CreateResult>(ex.Message);
                    }
                }
            }
            return new IdentityAdminResult("Invalid subject or secretId");
        }

        #endregion

        #region ClientCustomGrantType

        public async Task<IdentityAdminResult> AddClientCustomGrantTypeAsync(string subject, string grantType)
        {
            int parsedSubject;
            if (int.TryParse(subject, out parsedSubject))
            {
                using (var db = new ClientConfigurationDbContext(_connectionString, _entityFrameworkServiceOptions.Schema))
                {
                    try
                    {
                        var client = await db.Clients.FirstOrDefaultAsync(p => p.Id == parsedSubject);
                        if (client == null)
                        {
                            return new IdentityAdminResult("Invalid subject");
                        }
                        var existingGrantTypes = client.AllowedCustomGrantTypes;
                        if (existingGrantTypes.All(x => x.GrantType != grantType))
                        {
                            client.AllowedCustomGrantTypes.Add(new ClientCustomGrantType
                            {
                                GrantType = grantType,
                            });
                            db.SaveChanges();
                        }
                        return IdentityAdminResult.Success;
                    }
                    catch (SqlException ex)
                    {
                        return new IdentityAdminResult<CreateResult>(ex.Message);
                    }
                }
            }
            return new IdentityAdminResult("Invalid subject");
        }

        public async Task<IdentityAdminResult> RemoveClientCustomGrantTypeAsync(string subject, string id)
        {
            int parsedSubject;
            int parsedObjectId;
            if (int.TryParse(subject, out parsedSubject) && int.TryParse(id, out parsedObjectId))
            {
                using (var db = new ClientConfigurationDbContext(_connectionString, _entityFrameworkServiceOptions.Schema))
                {
                    try
                    {
                        var client = await db.Clients.FirstOrDefaultAsync(p => p.Id == parsedSubject);
                        if (client == null)
                        {
                            return new IdentityAdminResult("Invalid subject");
                        }
                        var existingGrantTypes = client.AllowedCustomGrantTypes.FirstOrDefault(p => p.Id == parsedObjectId);
                        if (existingGrantTypes != null)
                        {
                            client.AllowedCustomGrantTypes.Remove(existingGrantTypes);
                            await db.SaveChangesAsync();
                        }
                        return IdentityAdminResult.Success;
                    }
                    catch (SqlException ex)
                    {
                        return new IdentityAdminResult<CreateResult>(ex.Message);
                    }
                }
            }
            return new IdentityAdminResult("Invalid subject or secretId");
        }

        #endregion

        #region ClientScope

        public async Task<IdentityAdminResult> AddClientScopeAsync(string subject, string scope)
        {
            int parsedSubject;
            if (int.TryParse(subject, out parsedSubject))
            {
                using (var db = new ClientConfigurationDbContext(_connectionString, _entityFrameworkServiceOptions.Schema))
                {
                    try
                    {
                        var client = await db.Clients.FirstOrDefaultAsync(p => p.Id == parsedSubject);
                        if (client == null)
                        {
                            return new IdentityAdminResult("Invalid subject");
                        }
                        var existingScopes = client.AllowedScopes;
                        if (existingScopes.All(x => x.Scope != scope))
                        {
                            client.AllowedScopes.Add(new ClientScope
                            {
                                Scope = scope,
                            });
                            db.SaveChanges();
                        }
                        return IdentityAdminResult.Success;
                    }
                    catch (SqlException ex)
                    {
                        return new IdentityAdminResult<CreateResult>(ex.Message);
                    }
                }
            }
            return new IdentityAdminResult("Invalid subject");
        }

        public async Task<IdentityAdminResult> RemoveClientScopeAsync(string subject, string id)
        {
            int parsedSubject;
            int parsedObjectId;
            if (int.TryParse(subject, out parsedSubject) && int.TryParse(id, out parsedObjectId))
            {
                using (var db = new ClientConfigurationDbContext(_connectionString, _entityFrameworkServiceOptions.Schema))
                {
                    try
                    {
                        var client = await db.Clients.FirstOrDefaultAsync(p => p.Id == parsedSubject);
                        if (client == null)
                        {
                            return new IdentityAdminResult("Invalid subject");
                        }
                        var existingScopes = client.AllowedScopes.FirstOrDefault(p => p.Id == parsedObjectId);
                        if (existingScopes != null)
                        {
                            client.AllowedScopes.Remove(existingScopes);
                            await db.SaveChangesAsync();
                        }
                        return IdentityAdminResult.Success;
                    }
                    catch (SqlException ex)
                    {
                        return new IdentityAdminResult<CreateResult>(ex.Message);
                    }
                }
            }
            return new IdentityAdminResult("Invalid subject or secretId");
        }

        #endregion

        #endregion

        #region helperMethods
        protected IdentityAdminResult SetClientProperty(IEnumerable<PropertyMetadata> propsMeta, TCLient client, string type, string value)
        {
            IdentityAdminResult result;
            if (propsMeta.TrySet(client, type, value, out result))
            {
                return result;
            }

            throw new Exception("Invalid property type " + type);
        }

        protected string GetClientProperty(PropertyMetadata propMetadata, TCLient client)
        {
            string val;
            if (propMetadata.TryGet(client, out val))
            {
                return val;
            }
            throw new Exception("Invalid property type " + propMetadata.Type);
        }

        protected IdentityAdminResult SetScopeProperty(IEnumerable<PropertyMetadata> propsMeta, TScope scope, string type, string value)
        {
            IdentityAdminResult result;
            if (propsMeta.TrySet(scope, type, value, out result))
            {
                return result;
            }

            throw new Exception("Invalid property type " + type);
        }

        protected string GetScopeProperty(PropertyMetadata propMetadata, TScope scope)
        {
            string val;
            if (propMetadata.TryGet(scope, out val))
            {
                return val;
            }
            throw new Exception("Invalid property type " + propMetadata.Type);
        }

        #endregion
    }
}