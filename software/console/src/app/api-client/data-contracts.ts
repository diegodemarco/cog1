/* eslint-disable */
/* tslint:disable */
/*
 * ---------------------------------------------------------------
 * ## THIS FILE WAS GENERATED VIA SWAGGER-TYPESCRIPT-API        ##
 * ##                                                           ##
 * ## AUTHOR: acacode                                           ##
 * ## SOURCE: https://github.com/acacode/swagger-typescript-api ##
 * ---------------------------------------------------------------
 */

export interface CPUReport {
  usage?: CPUUsage;
  architecture?: string | null;
  vendor?: string | null;
  model?: string | null;
  /** @format int32 */
  cpuCount?: number;
}

export interface CPUUsage {
  lastSecond?: CPUUsageInterval;
  last5Seconds?: CPUUsageInterval;
  lastMinute?: CPUUsageInterval;
  last5Minutes?: CPUUsageInterval;
}

export interface CPUUsageInterval {
  /** @format double */
  idlePercentage?: number | null;
  /** @format double */
  ioWaitPercentage?: number | null;
}

export interface CommonLiteralsContainer {
  login?: string | null;
  logout?: string | null;
  email?: string | null;
  name?: string | null;
  password?: string | null;
  changePassword?: string | null;
  currentPassword?: string | null;
  newPassword?: string | null;
  reEnterPassword?: string | null;
  rememberMe?: string | null;
  iLostMyPassword?: string | null;
  next?: string | null;
  previous?: string | null;
  reportExecute?: string | null;
  filter?: string | null;
  commands?: string | null;
  buttonOk?: string | null;
  buttonConfirm?: string | null;
  buttonCancel?: string | null;
  buttonClose?: string | null;
  buttonYes?: string | null;
  buttonNo?: string | null;
  buttonRetry?: string | null;
  buttonUpdate?: string | null;
  buttonSearch?: string | null;
  buttonClean?: string | null;
  buttonSend?: string | null;
  sureYouWantToContinue?: string | null;
  sureYouWantToExit?: string | null;
  changesSavedSuccessfully?: string | null;
  description?: string | null;
  endpointType?: string | null;
  endpointTypes?: string | null;
  model?: string | null;
  serialNumber?: string | null;
  firmwareVersion?: string | null;
  application?: string | null;
  applications?: string | null;
  application_CoreManager?: string | null;
  year?: string | null;
  month?: string | null;
  firstName?: string | null;
  lastName?: string | null;
  language?: string | null;
  uploadImage?: string | null;
  iconUrl?: string | null;
  profile?: string | null;
  newLoginNeeded?: string | null;
  pleaseEnterFirstName?: string | null;
  pleaseEnterLastName?: string | null;
  pleaseEnterEmail?: string | null;
  pleaseEnterPassword?: string | null;
  pleaseEnterDescription?: string | null;
  pleaseEnterValue?: string | null;
  pleaseEnterCoordinates?: string | null;
  pleaseCompleteMandatoryFields?: string | null;
  passwordMismatch?: string | null;
  generalPermissions?: string | null;
  reports?: string | null;
  add?: string | null;
  edit?: string | null;
  delete?: string | null;
  wordYes?: string | null;
  wordNo?: string | null;
  latitude?: string | null;
  longitude?: string | null;
  notification?: string | null;
  notifications?: string | null;
  day?: string | null;
  days?: string | null;
  time?: string | null;
  parameters?: string | null;
  enabled_Female?: string | null;
  enabled_Male?: string | null;
  suspend?: string | null;
  unsuspend?: string | null;
  username?: string | null;
  multiSelect_All_Male?: string | null;
  multiSelect_All_Female?: string | null;
  multiSelect_X_Selected_Male?: string | null;
  multiSelect_X_Selected_Female?: string | null;
  multiSelect_X_No_Selected_Male?: string | null;
  multiSelect_X_No_Selected_Female?: string | null;
  legend?: string | null;
  multiSelect_Find?: string | null;
  none_Female?: string | null;
  select?: string | null;
  currentInformation?: string | null;
  dashboard?: string | null;
  dashboards?: string | null;
  descargas?: string | null;
  totalRowCount?: string | null;
  details?: string | null;
  category?: string | null;
  categories?: string | null;
  weekdayNames?: string[] | null;
  weekdayNames_Sunday?: string | null;
  weekdayNames_Monday?: string | null;
  weekdayNames_Tuesday?: string | null;
  weekdayNames_Wednesday?: string | null;
  weekdayNames_Thursday?: string | null;
  weekdayNames_Friday?: string | null;
  weekdayNames_Saturday?: string | null;
  weekdayNames_Short?: string[] | null;
  monthNames?: string[] | null;
  monthNames_Short?: string[] | null;
  geoCodingAdminArea2?: string | null;
  date?: string | null;
  selectFile?: string | null;
  commandExecutedSuccessfully?: string | null;
  noData?: string | null;
  turnedOn?: string | null;
  turnedOff?: string | null;
  turnOn?: string | null;
  turnOff?: string | null;
  stop?: string | null;
  moveUp?: string | null;
  moveDown?: string | null;
  dim?: string | null;
  today?: string | null;
  tomorrow?: string | null;
  yesterday?: string | null;
  now?: string | null;
  areaCode?: string | null;
  phoneNumber?: string | null;
  noResultsFound?: string | null;
  loading?: string | null;
  goBack?: string | null;
  active_Female?: string | null;
  active_Male?: string | null;
  activate?: string | null;
  event?: string | null;
  events?: string | null;
  refresh?: string | null;
  refreshAll?: string | null;
  languageEnglishUS?: string | null;
  dimLevel?: string | null;
  device?: string | null;
  devices?: string | null;
  apply?: string | null;
  normal?: string | null;
  warning?: string | null;
  error?: string | null;
  errors?: string | null;
  n_Errors?: string | null;
  total?: string | null;
  groupName_Administrators?: string | null;
  groupName_Everyone?: string | null;
  alert?: string | null;
  alerts?: string | null;
  comments?: string | null;
  wordToken?: string | null;
  price?: string | null;
  unsubscribe?: string | null;
  result?: string | null;
}

export interface DateTimeReport {
  /** @format date-time */
  utc?: string;
  /** @format date-time */
  local?: string;
  timeZone?: string | null;
  /** @format int64 */
  uptime?: number;
}

export interface DiskReport {
  /** @format int64 */
  bytesTotal?: number;
  /** @format int64 */
  bytesUsed?: number;
  /** @format int64 */
  bytesAvailable?: number;
  /** @format double */
  freePercentage?: number;
}

export interface LiteralsContainerDTO {
  common?: CommonLiteralsContainer;
}

export interface LoginRequestDTO {
  userName?: string | null;
  password?: string | null;
}

export interface LoginResponseDTO {
  token?: string | null;
}

export interface MemoryReport {
  /** @format int64 */
  totalBytes?: number;
  /** @format int64 */
  usedBytes?: number;
  /** @format int64 */
  freeBytes?: number;
  /** @format double */
  freePercentage?: number;
}

export interface SystemStatsReport {
  dateTime?: DateTimeReport;
  cpuReport?: CPUReport;
  memory?: MemoryReport;
  disk?: DiskReport;
  temperature?: TemperatureReport;
  wiFi?: WiFiReport;
}

export interface TemperatureReport {
  /** @format double */
  maxTemperatureC?: number | null;
  isCritical?: boolean | null;
  details?: TemperatureReportEntry[] | null;
}

export interface TemperatureReportEntry {
  source?: string | null;
  /** @format double */
  temperatureC?: number;
  /** @format double */
  criticalTemperatureC?: number | null;
  isCritical?: boolean;
}

export interface VariableDTO {
  /** @format int32 */
  variableId?: number;
  variableCode?: string | null;
  description?: string | null;
  units?: string | null;
  type?: VariableType;
  direction?: VariableDirection;
}

/** @format int32 */
export enum VariableDirection {
  Unknown = 0,
  Input = 1,
  Output = 2,
}

/** @format int32 */
export enum VariableType {
  Unknown = 0,
  Binary = 1,
  Integer = 2,
  FloatingPoint = 3,
}

export interface VariableValueDTO {
  /** @format int32 */
  variableId?: number;
  /** @format double */
  value?: number | null;
  /** @format date-time */
  lastUpdateUtc?: string | null;
}

export interface WiFiReport {
  ssid?: string | null;
  /** @format int32 */
  connectionState?: number;
  isConnected?: boolean;
  ipv4?: string | null;
  /** @format int32 */
  maskBits?: number;
  /** @format int32 */
  rssi?: number;
  /** @format int32 */
  noise?: number;
  /** @format int32 */
  frequency?: number;
  savedConnections?: string[] | null;
}
