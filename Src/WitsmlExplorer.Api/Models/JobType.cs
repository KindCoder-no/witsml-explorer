namespace WitsmlExplorer.Api.Models
{
    public enum JobType
    {
        CopyBhaRun = 1,
        CopyLog,
        CopyLogData,
        CopyTrajectory,
        CopyTubular,
        CopyTubularComponents,
        ModifyBhaRun,
        TrimLogObject,
        ModifyLogObject,
        DeleteMessageObjects,
        ModifyMessageObject,
        DeleteBhaRuns,
        DeleteCurveValues,
        DeleteLogObjects,
        DeleteMnemonics,
        DeleteTrajectory,
        DeleteTrajectoryStations,
        DeleteTubular,
        DeleteTubularComponents,
        DeleteWbGeometrys,
        DeleteWell,
        DeleteWellbore,
        DeleteRigs,
        DeleteRisks,
        DeleteMudLog,
        RenameMnemonic,
        ModifyTrajectoryStation,
        ModifyTubular,
        ModifyTubularComponent,
        ModifyWbGeometry,
        ModifyWell,
        ModifyWellbore,
        ModifyMudLog,
        ModifyRig,
        ModifyRisk,
        CreateLogObject,
        CreateWell,
        CreateWellbore,
        CreateRisk,
        CreateMudLog,
        CreateWbGeometry,
        BatchModifyWell,
        ImportLogData
    }
}
