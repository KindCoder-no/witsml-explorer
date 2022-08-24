using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Witsml.Data;
using Witsml.ServiceReference;

using WitsmlExplorer.Api.Models;
using WitsmlExplorer.Api.Models.Measure;
using WitsmlExplorer.Api.Query;

namespace WitsmlExplorer.Api.Services
{
    public interface IRigService
    {
        Task<IEnumerable<Rig>> GetRigs(string wellUid, string wellboreUid);
        Task<Rig> GetRig(string wellUid, string wellboreUid, string rigUid);
    }

    // ReSharper disable once UnusedMember.Global
    public class RigService : WitsmlService, IRigService
    {
        public RigService(IWitsmlClientProvider witsmlClientProvider) : base(witsmlClientProvider) { }

        public async Task<IEnumerable<Rig>> GetRigs(string wellUid, string wellboreUid)
        {
            WitsmlRigs witsmlRigs = RigQueries.GetWitsmlRigByWellbore(wellUid, wellboreUid);
            WitsmlRigs result = await _witsmlClient.GetFromStoreAsync(witsmlRigs, new OptionsIn(ReturnElements.All));
            return result.Rigs.Select(rig =>
                new Rig
                {
                    AirGap = rig.AirGap == null ? null : new LengthMeasure { Uom = rig.AirGap.Uom, Value = StringHelpers.ToDecimal(rig.AirGap.Value) },
                    Approvals = rig.Approvals,
                    ClassRig = rig.ClassRig,
                    DTimStartOp = StringHelpers.ToDateTime(rig.DTimStartOp),
                    DTimEndOp = StringHelpers.ToDateTime(rig.DTimEndOp),
                    EmailAddress = rig.EmailAddress,
                    FaxNumber = rig.FaxNumber,
                    IsOffshore = rig.IsOffshore,
                    Manufacturer = rig.Manufacturer,
                    Name = rig.Name,
                    NameContact = rig.NameContact,
                    WellName = rig.NameWell,
                    WellboreName = rig.NameWellbore,
                    Owner = rig.Owner,
                    Uid = rig.Uid,
                    WellUid = rig.UidWell,
                    WellboreUid = rig.UidWellbore,
                    RatingDrillDepth = rig.RatingDrillDepth == null ? null : new LengthMeasure { Uom = rig.RatingDrillDepth.Uom, Value = StringHelpers.ToDecimal(rig.RatingDrillDepth.Value) },
                    RatingWaterDepth = rig.RatingWaterDepth == null ? null : new LengthMeasure { Uom = rig.RatingWaterDepth.Uom, Value = StringHelpers.ToDecimal(rig.RatingWaterDepth.Value) },
                    Registration = rig.Registration,
                    TelNumber = rig.TelNumber,
                    TypeRig = rig.TypeRig,
                    YearEntService = rig.YearEntService,
                    CommonData = new CommonData()
                    {
                        ItemState = rig.CommonData.ItemState,
                        SourceName = rig.CommonData.SourceName,
                        DTimLastChange = StringHelpers.ToDateTime(rig.CommonData.DTimLastChange),
                        DTimCreation = StringHelpers.ToDateTime(rig.CommonData.DTimCreation),
                    }

                }).OrderBy(rig => rig.Name);
        }

        public async Task<Rig> GetRig(string wellUid, string wellboreUid, string rigUid)
        {
            WitsmlRigs query = RigQueries.GetWitsmlRigById(wellUid, wellboreUid, rigUid);
            WitsmlRigs result = await _witsmlClient.GetFromStoreAsync(query, new OptionsIn(ReturnElements.All));
            WitsmlRig witsmlRig = result.Rigs.FirstOrDefault();

            return (witsmlRig == null) ? null : new Rig
            {
                AirGap = witsmlRig.AirGap == null ? null : new LengthMeasure { Uom = witsmlRig.AirGap.Uom, Value = StringHelpers.ToDecimal(witsmlRig.AirGap.Value) },
                Approvals = witsmlRig.Approvals,
                ClassRig = witsmlRig.ClassRig,
                DTimStartOp = StringHelpers.ToDateTime(witsmlRig.DTimStartOp),
                DTimEndOp = StringHelpers.ToDateTime(witsmlRig.DTimEndOp),
                EmailAddress = witsmlRig.EmailAddress,
                FaxNumber = witsmlRig.FaxNumber,
                IsOffshore = witsmlRig.IsOffshore,
                Owner = witsmlRig.Owner,
                Manufacturer = witsmlRig.Manufacturer,
                Name = witsmlRig.Name,
                NameContact = witsmlRig.NameContact,
                WellName = witsmlRig.NameWell,
                WellboreName = witsmlRig.NameWellbore,
                Registration = witsmlRig.Registration,
                RatingDrillDepth = witsmlRig.RatingDrillDepth == null ? null : new LengthMeasure { Uom = witsmlRig.RatingDrillDepth.Uom, Value = StringHelpers.ToDecimal(witsmlRig.RatingDrillDepth.Value) },
                RatingWaterDepth = witsmlRig.RatingWaterDepth == null ? null : new LengthMeasure { Uom = witsmlRig.RatingWaterDepth.Uom, Value = StringHelpers.ToDecimal(witsmlRig.RatingWaterDepth.Value) },
                TelNumber = witsmlRig.TelNumber,
                TypeRig = witsmlRig.TypeRig,
                Uid = witsmlRig.Uid,
                WellUid = witsmlRig.UidWell,
                WellboreUid = witsmlRig.UidWellbore,
                YearEntService = witsmlRig.YearEntService,
                CommonData = new CommonData()
                {
                    ItemState = witsmlRig.CommonData.ItemState,
                    SourceName = witsmlRig.CommonData.SourceName,
                    DTimLastChange = StringHelpers.ToDateTime(witsmlRig.CommonData.DTimLastChange),
                    DTimCreation = StringHelpers.ToDateTime(witsmlRig.CommonData.DTimCreation),
                }

            };
        }
    }
}
