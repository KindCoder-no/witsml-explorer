using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

using WitsmlExplorer.Api.Configuration;
using WitsmlExplorer.Api.Extensions;
using WitsmlExplorer.Api.Models;
using WitsmlExplorer.Api.Repositories;
using WitsmlExplorer.Api.Services;

namespace WitsmlExplorer.Api.HttpHandlers
{
    public static class WitsmlServerHandler
    {
        [Produces(typeof(IEnumerable<Connection>))]
        public static async Task<IResult> GetWitsmlServers([FromServices] IDocumentRepository<Server, Guid> witsmlServerRepository, IConfiguration configuration, HttpContext httpContext, ICredentialsService credentialsService)
        {
            EssentialHeaders httpHeaders = new(httpContext?.Request);
            bool useOAuth = StringHelpers.ToBoolean(configuration[ConfigConstants.OAuth2Enabled]);
            if (!useOAuth)
            {
                httpContext.GetOrCreateWitsmlExplorerCookie();
            }
            IEnumerable<Server> servers = await witsmlServerRepository.GetDocumentsAsync();
            IEnumerable<Connection> credentials = await Task.WhenAll(servers.Select(async (server) =>
                new Connection(server)
                {
                    Usernames = await credentialsService.GetLoggedInUsernames(useOAuth, httpHeaders, server.Url)
                }));
            return TypedResults.Ok(credentials);
        }

        [Produces(typeof(Server))]
        public static async Task<IResult> CreateWitsmlServer(Server witsmlServer, [FromServices] IDocumentRepository<Server, Guid> witsmlServerRepository)
        {
            Server inserted = await witsmlServerRepository.CreateDocumentAsync(witsmlServer);
            return TypedResults.Ok(inserted);
        }

        [Produces(typeof(Server))]
        public static async Task<IResult> UpdateWitsmlServer(Guid witsmlServerId, Server witsmlServer, [FromServices] IDocumentRepository<Server, Guid> witsmlServerRepository)
        {
            Server updatedServer = await witsmlServerRepository.UpdateDocumentAsync(witsmlServerId, witsmlServer);
            return TypedResults.Ok(updatedServer);
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public static async Task<IResult> DeleteWitsmlServer(Guid witsmlServerId, [FromServices] IDocumentRepository<Server, Guid> witsmlServerRepository)
        {
            await witsmlServerRepository.DeleteDocumentAsync(witsmlServerId);
            return TypedResults.NoContent();
        }
    }
}
