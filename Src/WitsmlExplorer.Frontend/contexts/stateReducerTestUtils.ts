import BhaRun from "../models/bhaRun";
import LogObject from "../models/logObject";
import { Server } from "../models/server";
import Trajectory from "../models/trajectory";
import Well from "../models/well";
import Wellbore from "../models/wellbore";
import Filter, { EMPTY_FILTER } from "./filter";
import { EMPTY_NAVIGATION_STATE, NavigationState } from "./navigationContext";

export const SERVER_1: Server = { id: "1", name: "WITSML server", url: "http://example.com", description: "Witsml server", securityscheme: "", roles: [] };
export const SERVER_2: Server = { id: "2", name: "WITSML server 2", url: "http://example2.com", description: "Witsml server 2", securityscheme: "", roles: [] };
export const WELLBORE_1: Wellbore = {
  uid: "wellbore1",
  wellUid: "well1",
  name: "Wellbore 1",
  bhaRuns: [],
  logs: [],
  rigs: [],
  trajectories: [],
  messages: [],
  mudLogs: [],
  risks: [],
  tubulars: [],
  wbGeometrys: [],
  wellStatus: "",
  wellType: "",
  isActive: false
};
export const WELLBORE_2: Wellbore = {
  uid: "wellbore2",
  wellUid: "well2",
  name: "Wellbore 2",
  bhaRuns: [],
  logs: [],
  rigs: [],
  trajectories: [],
  messages: [],
  mudLogs: [],
  risks: [],
  tubulars: [],
  wbGeometrys: [],
  wellStatus: "",
  wellType: "",
  isActive: false
};
export const WELLBORE_3: Wellbore = {
  uid: "wellbore3",
  wellUid: "well3",
  name: "Wellbore 3",
  bhaRuns: [],
  logs: [],
  rigs: [],
  trajectories: [],
  messages: [],
  mudLogs: [],
  risks: [],
  tubulars: [],
  wbGeometrys: [],
  wellStatus: "",
  wellType: "",
  isActive: false
};
export const WELL_1: Well = { uid: "well1", name: "Well 1", wellbores: [WELLBORE_1], field: "", operator: "", country: "" };
export const WELL_2: Well = { uid: "well2", name: "Well 2", wellbores: [WELLBORE_2], field: "", operator: "", country: "" };
export const WELL_3: Well = { uid: "well3", name: "Well 3", wellbores: [WELLBORE_3], field: "", operator: "", country: "" };
export const WELLS = [WELL_1, WELL_2, WELL_3];
export const BHARUN_1: BhaRun = {
  uid: "bharun",
  name: "bharun 1",
  wellUid: WELL_1.uid,
  wellboreUid: WELLBORE_1.uid,
  wellboreName: "",
  wellName: "",
  numStringRun: "",
  tubular: "",
  dTimStart: null,
  dTimStop: null,
  dTimStartDrilling: null,
  dTimStopDrilling: null,
  planDogleg: null,
  actDogleg: null,
  actDoglegMx: null,
  statusBha: "",
  numBitRun: "",
  reasonTrip: "",
  objectiveBha: "",
  commonData: null,
  tubularUidRef: ""
};
export const LOG_1: LogObject = { uid: "log1", name: "Log 1", wellUid: WELL_1.uid, wellboreUid: WELLBORE_1.uid, wellboreName: "", wellName: "" };
export const RIG_1 = { uid: "rig1", name: "Rig 1" };
export const TRAJECTORY_1: Trajectory = {
  uid: "trajectory1",
  name: "Trajectory 1",
  wellUid: "",
  wellboreUid: "",
  wellboreName: "",
  wellName: "",
  aziRef: "",
  mdMax: 0,
  mdMin: 0,
  trajectoryStations: [],
  dTimTrajEnd: null,
  dTimTrajStart: null
};
export const MESSAGE_1 = {
  dateTimeLastChange: "2021-03-03T18:00:24.439+01:00",
  messageText: "Fill Brine Storage 2 with drillwater",
  name: "Surface Logging Data - Message - MSG1",
  uid: "MSG1",
  wellName: "",
  wellUid: "",
  wellboreName: "",
  wellboreUid: ""
};
export const MUDLOG_1 = {
  uid: "123"
};
export const RISK_1 = {
  dateTimeLastChange: "2021-03-03T18:00:24.439+01:00",
  name: "Dangerous risk",
  uid: "MSG1",
  wellName: "",
  wellUid: "",
  wellboreName: "",
  wellboreUid: ""
};
export const TUBULAR_1 = {
  uid: "TUB1",
  wellUid: "",
  wellboreUid: "",
  name: "tubby",
  typeTubularAssy: "drilling"
};
export const WBGEOMETRY_1 = {
  dateTimeLastChange: "2021-03-03T18:00:24.439+01:00",
  uid: "WBG1",
  wellName: "",
  wellUid: "",
  wellboreName: "",
  wellboreUid: ""
};
export const FILTER_1: Filter = { ...EMPTY_FILTER, wellName: WELL_1.name };
export const TRAJECTORY_GROUP_1 = "TrajectoryGroup";

export const getInitialState = (): NavigationState => {
  const well1 = { ...WELL_1, wellbores: [{ ...WELLBORE_1 }] };
  const well2 = { ...WELL_2, wellbores: [{ ...WELLBORE_2 }] };
  const well3 = { ...WELL_3, wellbores: [{ ...WELLBORE_3 }] };
  const wells = [well1, well2, well3];
  const servers = [SERVER_1];
  return {
    ...EMPTY_NAVIGATION_STATE,
    selectedServer: SERVER_1,
    wells,
    filteredWells: wells,
    servers
  };
};
