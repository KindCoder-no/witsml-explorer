import {
  AddServerAction,
  AddWellAction,
  AddWellboreAction,
  RemoveWellAction,
  RemoveWellboreAction,
  RemoveWitsmlServerAction,
  UpdateServerAction,
  UpdateServerListAction,
  UpdateWellAction,
  UpdateWellboreAction,
  UpdateWellboreBhaRunsAction,
  UpdateWellboreLogAction,
  UpdateWellboreLogsAction,
  UpdateWellboreMessageAction,
  UpdateWellboreMessagesAction,
  UpdateWellboreRigsAction,
  UpdateWellboreRisksAction,
  UpdateWellboreTrajectoriesAction,
  UpdateWellboreTrajectoryAction,
  UpdateWellboreTubularAction,
  UpdateWellboreTubularsAction,
  UpdateWellboreWbGeometryAction,
  UpdateWellboreWbGeometrysAction,
  UpdateWellsAction
} from "./modificationActions";
import {
  SelectBhaRunGroupAction,
  SelectJobsAction,
  SelectLogCurveInfoAction,
  SelectLogGroupAction,
  SelectLogObjectAction,
  SelectLogTypeAction,
  SelectMessageGroupAction,
  SelectMudLogGroupAction,
  SelectRigGroupAction,
  SelectRiskGroupAction,
  SelectServerAction,
  SelectServerManagerAction,
  SelectTrajectoryAction,
  SelectTrajectoryGroupAction,
  SelectTubularAction,
  SelectTubularGroupAction,
  SelectWbGeometryAction,
  SelectWbGeometryGroupAction,
  SelectWellAction,
  SelectWellboreAction,
  SetCurveThresholdAction,
  SetFilterAction,
  ToggleTreeNodeAction
} from "./navigationActions";

export type NavigationAction =
  | AddServerAction
  | AddWellAction
  | AddWellboreAction
  | RemoveWellAction
  | RemoveWellboreAction
  | RemoveWitsmlServerAction
  | UpdateServerAction
  | UpdateServerListAction
  | UpdateWellAction
  | UpdateWellsAction
  | UpdateWellboreAction
  | UpdateWellboreBhaRunsAction
  | UpdateWellboreLogAction
  | UpdateWellboreLogsAction
  | UpdateWellboreMessagesAction
  | UpdateWellboreMessageAction
  | UpdateWellboreRigsAction
  | UpdateWellboreRisksAction
  | UpdateWellboreTrajectoryAction
  | UpdateWellboreTrajectoriesAction
  | UpdateWellboreTubularAction
  | UpdateWellboreTubularsAction
  | UpdateWellboreWbGeometryAction
  | UpdateWellboreWbGeometrysAction
  | ToggleTreeNodeAction
  | SelectJobsAction
  | SelectBhaRunGroupAction
  | SelectLogTypeAction
  | SelectLogGroupAction
  | SelectLogCurveInfoAction
  | SelectLogObjectAction
  | SelectWellAction
  | SelectWellboreAction
  | SelectRigGroupAction
  | SelectMessageGroupAction
  | SelectMudLogGroupAction
  | SelectRiskGroupAction
  | SelectServerAction
  | SelectTrajectoryAction
  | SelectTrajectoryGroupAction
  | SelectTubularAction
  | SelectTubularGroupAction
  | SelectWbGeometryGroupAction
  | SelectWbGeometryAction
  | SetFilterAction
  | SetCurveThresholdAction
  | SelectServerManagerAction;
