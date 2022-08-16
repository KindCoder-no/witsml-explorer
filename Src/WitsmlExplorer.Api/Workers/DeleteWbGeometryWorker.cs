using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Serilog;

using Witsml;

using WitsmlExplorer.Api.Jobs;
using WitsmlExplorer.Api.Models;
using WitsmlExplorer.Api.Query;
using WitsmlExplorer.Api.Services;



namespace WitsmlExplorer.Api.Workers
{
    public class DeleteWbGeometryWorker : BaseWorker<DeleteWbGeometryJob>, IWorker
    {
        private readonly IWitsmlClient _witsmlClient;
        public JobType JobType => JobType.DeleteWbGeometrys;

        public DeleteWbGeometryWorker(IWitsmlClientProvider witsmlClientProvider)
        {
            _witsmlClient = witsmlClientProvider.GetClient();
        }

        public override async Task<(WorkerResult, RefreshAction)> Execute(DeleteWbGeometryJob job)
        {
            Verify(job);

            var wellUid = job.ToDelete.WellUid;
            var wellboreUid = job.ToDelete.WellboreUid;
            var wbGeometryUids = job.ToDelete.WbGeometryUids;
            var queries = WbGeometryQueries.DeleteWbGeometryQuery(wellUid, wellboreUid, wbGeometryUids);
            bool error = false;
            var successUids = new List<string>();
            var errorReasons = new List<string>();
            var errorEnitities = new List<EntityDescription>();

            var results = await Task.WhenAll(queries.Select(async (query) =>
            {
                var result = await _witsmlClient.DeleteFromStoreAsync(query);
                var wbGeometry = query.WbGeometrys.First();
                if (result.IsSuccessful)
                {
                    Log.Information("{JobType} - Job successful", GetType().Name);
                    successUids.Add(wbGeometry.Uid);
                }
                else
                {
                    Log.Error("Failed to delete wbGeometry. WellUid: {WellUid}, WellboreUid: {WellboreUid}, Uid: {wbGeometryUid}, Reason: {Reason}",
                    wellUid,
                    wellboreUid,
                    query.WbGeometrys.First().Uid,
                    result.Reason);
                    error = true;
                    errorReasons.Add(result.Reason);
                    errorEnitities.Add(new EntityDescription
                    {
                        WellName = wbGeometry.NameWell,
                        WellboreName = wbGeometry.NameWellbore,
                        ObjectName = wbGeometry.Name
                    });
                }
                return result;
            }));

            var refreshAction = new RefreshWbGeometryObjects(_witsmlClient.GetServerHostname(), wellUid, wellboreUid, RefreshType.Update);
            var successString = successUids.Count > 0 ? $"Deleted WbGeometrys: {string.Join(", ", successUids)}." : "";
            if (!error)
            {
                return (new WorkerResult(_witsmlClient.GetServerHostname(), true, successString), refreshAction);
            }

            return (new WorkerResult(_witsmlClient.GetServerHostname(), false, $"{successString} Failed to delete some WbGeometrys", errorReasons.First(), errorEnitities.First()), successUids.Count > 0 ? refreshAction : null);
        }

        private static void Verify(DeleteWbGeometryJob job)
        {
            if (!job.ToDelete.WbGeometryUids.Any()) throw new ArgumentException("A minimum of one WbGeometry UID is required");
            if (string.IsNullOrEmpty(job.ToDelete.WellUid)) throw new ArgumentException("WellUid is required");
            if (string.IsNullOrEmpty(job.ToDelete.WellboreUid)) throw new ArgumentException("WellboreUid is required");
        }
    }
}
