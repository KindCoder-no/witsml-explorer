using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Witsml;
using Witsml.Data;
using Witsml.Extensions;
using Witsml.Query;
using Witsml.ServiceReference;

using WitsmlExplorer.Api.Jobs;
using WitsmlExplorer.Api.Models;
using WitsmlExplorer.Api.Services;

namespace WitsmlExplorer.Api.Workers.Delete
{
    public class DeleteMudLogWorker : BaseWorker<DeleteMudLogJob>, IWorker
    {
        public JobType JobType => JobType.DeleteMudLog;

        public DeleteMudLogWorker(ILogger<DeleteMudLogJob> logger, IWitsmlClientProvider witsmlClientProvider) : base(witsmlClientProvider, logger) { }

        public override async Task<(WorkerResult, RefreshAction)> Execute(DeleteMudLogJob job)
        {
            string wellUid = job.ToDelete.WellUid;
            string wellboreUid = job.ToDelete.WellboreUid;
            string uid = job.ToDelete.Uid;

            WitsmlMudLogs deleteRequest = DeleteRequest(wellUid, wellboreUid, uid);

            QueryResult result = await GetTargetWitsmlClientOrThrow().DeleteFromStoreAsync(deleteRequest);

            if (result.IsSuccessful)
            {
                Logger.LogInformation("Deleted mudLog. WellUid: {WellUid}, WellboreUid: {WellboreUid}, Uid: {Uid}",
                        wellUid,
                        wellboreUid,
                        uid);
                RefreshWell refreshAction = new(GetTargetWitsmlClientOrThrow().GetServerHostname(), wellUid, RefreshType.Remove);
                WorkerResult workerResult = new(GetTargetWitsmlClientOrThrow().GetServerHostname(), true, $"Deleted mudLog with uid ${wellUid}");
                return (workerResult, refreshAction);
            }

            Logger.LogError("Failed to delete mudLog. WellUid: {WellUid}, WellboreUid: {WellboreUid}, Uid: {Uid}",
                        wellUid,
                        wellboreUid,
                        uid);
            WitsmlMudLogs query = MudLogQueries.QueryById(wellUid, wellboreUid, uid);
            WitsmlMudLogs queryResult = await GetTargetWitsmlClientOrThrow().GetFromStoreAsync(query, new OptionsIn(ReturnElements.IdOnly));
            EntityDescription description = null;
            WitsmlMudLog mudLog = queryResult.MudLogs.FirstOrDefault();
            if (mudLog != null)
            {
                description = new EntityDescription
                {
                    ObjectName = mudLog.Name
                };
            }
            return (new WorkerResult(GetTargetWitsmlClientOrThrow().GetServerHostname(), false, "Failed to delete mudLog", result.Reason, description), null);

        }

        private static WitsmlMudLogs DeleteRequest(string wellUid, string wellboreUid, string uid)
        {
            return new WitsmlMudLogs
            {
                MudLogs = new WitsmlMudLog
                {
                    UidWell = wellUid,
                    UidWellbore = wellboreUid,
                    Uid = uid
                }.AsSingletonList()
            };
        }
    }
}
