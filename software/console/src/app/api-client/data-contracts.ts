/* eslint-disable */
/* tslint:disable */
// @ts-nocheck
/*
 * ---------------------------------------------------------------
 * ## THIS FILE WAS GENERATED VIA SWAGGER-TYPESCRIPT-API        ##
 * ##                                                           ##
 * ## AUTHOR: acacode                                           ##
 * ## SOURCE: https://github.com/acacode/swagger-typescript-api ##
 * ---------------------------------------------------------------
 */

/** @format int32 */
export enum VariableType {
  Unknown = 0,
  Binary = 1,
  Integer = 2,
  FloatingPoint = 3,
}

/** @format int32 */
export enum VariableSource {
  Unknown = 0,
  BuiltIn = 1,
  Calculated = 2,
  External = 3,
  Modbus = 4,
}

/** @format int32 */
export enum VariableAccessType {
  Unknown = 0,
  Readonly = 1,
  ReadWrite = 2,
  ReadWriteAction = 3,
}

/** @format int32 */
export enum ModbusRegisterType {
  Unknown = 0,
  Coil = 1,
  DiscreteInput = 2,
  HoldingRegister = 3,
  InputRegister = 4,
}

/** @format int32 */
export enum ModbusDataType {
  Unknown = 0,
  Boolean = 1,
  UInt16 = 2,
  UInt32 = 3,
  Int16 = 4,
  Int32 = 5,
  Float32 = 6,
}

export interface AccessTokenInfoDTO {
  user?: UserDTO;
}

export interface BasicEntitiesContainerDTO {
  literals?: LiteralsContainerDTO;
  locales?: LocaleDTO[] | null;
  variableTypes?: VariableTypeDTO[] | null;
  variableAccessTypes?: VariableAccessTypeDTO[] | null;
  variableSources?: VariableSourceDTO[] | null;
  modbusRegisterTypes?: ModbusRegisterTypeDTO[] | null;
  modbusDataTypes?: ModbusDataTypeDTO[] | null;
}

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
  results?: string | null;
  success?: string | null;
  localeCode?: string | null;
}

export interface DashboardLiteralsContainer {
  dashboard?: string | null;
  cpu?: string | null;
  ram?: string | null;
  storage?: string | null;
  temperature?: string | null;
  last5Minutes?: string | null;
  cpuLast5Minutes?: string | null;
  localeCode?: string | null;
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

export interface EthernetLinkConfigurationDTO {
  /** @format int32 */
  speed?: number;
}

export interface EthernetReport {
  macAddress?: string | null;
  /** @format int32 */
  connectionState?: number;
  isConnected?: boolean;
  /** @format int32 */
  speed?: number;
  fullDuplex?: boolean;
  autoNegotiate?: boolean;
  ipConfiguration?: IpConfigurationDTO;
}

export interface IpConfigurationDTO {
  dhcp?: boolean;
  ipv4?: string | null;
  /** @format int32 */
  netMask?: number;
  gateway?: string | null;
  dns?: string | null;
}

export interface JsonControllerException {
  message?: string | null;
  className?: string | null;
  faultCode?: string | null;
  faultData?: string | null;
}

export interface LiteralsContainerDTO {
  common?: CommonLiteralsContainer;
  dashboard?: DashboardLiteralsContainer;
  security?: SecurityLiteralsContainer;
  network?: NetworkLiteralsContainer;
  variables?: VariablesLiteralsContainer;
  modbus?: ModbusLiteralsContainer;
  localeCode?: string | null;
}

export interface LocaleDTO {
  localeCode?: string | null;
  description?: string | null;
}

export interface LoginRequestDTO {
  userName?: string | null;
  password?: string | null;
}

export interface LoginResponseDTO {
  token?: string | null;
}

export interface MemoryReportDTO {
  /** @format int64 */
  totalBytes?: number;
  /** @format int64 */
  usedBytes?: number;
  /** @format int64 */
  freeBytes?: number;
  /** @format int64 */
  availableBytes?: number;
  /** @format double */
  freePercentage?: number;
  /** @format double */
  availablePercentage?: number;
}

export interface ModbusDataTypeDTO {
  modbusDataType?: ModbusDataType;
  description?: string | null;
}

export interface ModbusLiteralsContainer {
  coil?: string | null;
  discreteInput?: string | null;
  holdingRegister?: string | null;
  inputRegister?: string | null;
  dataTypeBoolean?: string | null;
  tcpHost?: string | null;
  slaveId?: string | null;
  registerAddress?: string | null;
  registerType?: string | null;
  dataType?: string | null;
  registers?: string | null;
  newRegister?: string | null;
  editRegister?: string | null;
  deleteRegister?: string | null;
  deleteRegisterConfirmation?: string | null;
  registerCreated?: string | null;
  registerUpdated?: string | null;
  registerDeleted?: string | null;
  noRegistersToDisplay?: string | null;
  localeCode?: string | null;
}

export interface ModbusRegisterDTO {
  tcpHost?: string | null;
  /** @format int32 */
  slaveId?: number;
  /** @format int32 */
  registerAddress?: number;
  registerType?: ModbusRegisterType;
  dataType?: ModbusDataType;
}

export interface ModbusRegisterTypeDTO {
  modbusRegisterType?: ModbusRegisterType;
  description?: string | null;
}

export interface NetworkLiteralsContainer {
  network?: string | null;
  summary?: string | null;
  networkSummary?: string | null;
  connection?: string | null;
  status?: string | null;
  connected?: string | null;
  disconnected?: string | null;
  ipMethod?: string | null;
  ipFixed?: string | null;
  ipAddress?: string | null;
  gateway?: string | null;
  frequency?: string | null;
  speed?: string | null;
  fullDuplex?: string | null;
  halfDuplex?: string | null;
  macAddress?: string | null;
  connect?: string | null;
  disconnect?: string | null;
  forget?: string | null;
  scanning?: string | null;
  scanningPleaseWait?: string | null;
  connectingPleaseWait?: string | null;
  disconnectingPleaseWait?: string | null;
  forgettingPleaseWait?: string | null;
  configuringPleaseWait?: string | null;
  ipConfiguration?: string | null;
  linkConfiguration?: string | null;
  wiFiNetworks?: string | null;
  confirmChanges?: string | null;
  confirmForget?: string | null;
  configurationAppliedSuccessfully?: string | null;
  localeCode?: string | null;
}

export interface SecurityLiteralsContainer {
  basicUser?: string | null;
  operator?: string | null;
  administrator?: string | null;
  userRole?: string | null;
  security?: string | null;
  users?: string | null;
  newUser?: string | null;
  editUser?: string | null;
  deleteUser?: string | null;
  deleteUserConfirmation?: string | null;
  changePassword?: string | null;
  noUsersToDisplay?: string | null;
  userCreated?: string | null;
  userUpdated?: string | null;
  userDeleted?: string | null;
  localeCode?: string | null;
}

export interface SystemStatsReport {
  dateTime?: DateTimeReport;
  cpuReport?: CPUReport;
  memory?: MemoryReportDTO;
  disk?: DiskReport;
  temperature?: TemperatureReport;
  wiFi?: WiFiReport;
  ethernet?: EthernetReport;
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

export interface UpdateProfileRequestDTO {
  localeCode?: string | null;
}

export interface UserDTO {
  /** @format int32 */
  userId?: number;
  userName?: string | null;
  isOperator?: boolean;
  isAdmin?: boolean;
  localeCode?: string | null;
}

export interface UserWithPasswordDTO {
  user?: UserDTO;
  password?: string | null;
}

export interface VariableAccessTypeDTO {
  accessType?: VariableAccessType;
  description?: string | null;
}

export interface VariableDTO {
  /** @format int32 */
  variableId?: number;
  variableCode?: string | null;
  description?: string | null;
  units?: string | null;
  type?: VariableType;
  accessType?: VariableAccessType;
  source?: VariableSource;
  /** @format int32 */
  pollIntervalMs?: number;
  modbusRegister?: ModbusRegisterDTO;
}

export interface VariableSourceDTO {
  variableSource?: VariableSource;
  description?: string | null;
}

export interface VariableTypeDTO {
  variableType?: VariableType;
  description?: string | null;
}

export interface VariableValueDTO {
  /** @format int32 */
  variableId?: number;
  /** @format double */
  value?: number | null;
  /** @format date-time */
  lastUpdateUtc?: string | null;
}

export interface VariablesLiteralsContainer {
  variable?: string | null;
  variables?: string | null;
  newVariable?: string | null;
  editVariable?: string | null;
  deleteVariable?: string | null;
  deleteVariableConfirmation?: string | null;
  variableId?: string | null;
  variableType?: string | null;
  variableSource?: string | null;
  variableAccessType?: string | null;
  variableCode?: string | null;
  variableUnits?: string | null;
  pollInterval?: string | null;
  binary?: string | null;
  integer?: string | null;
  fLoatingPoint?: string | null;
  readonly?: string | null;
  readWrite?: string | null;
  readWriteAction?: string | null;
  noVariablesToDisplay?: string | null;
  variableCreated?: string | null;
  variableUpdated?: string | null;
  variableDeleted?: string | null;
  builtIn?: string | null;
  calculated?: string | null;
  modbus?: string | null;
  external?: string | null;
  localeCode?: string | null;
}

export interface WiFiConnectRequestDTO {
  ssid?: string | null;
  password?: string | null;
}

export interface WiFiReport {
  macAddress?: string | null;
  ssid?: string | null;
  /** @format int32 */
  connectionState?: number;
  isConnected?: boolean;
  ipConfiguration?: IpConfigurationDTO;
  /** @format int32 */
  rssi?: number;
  /** @format int32 */
  noise?: number;
  /** @format int32 */
  frequency?: number;
  savedConnections?: string[] | null;
}

export interface WiFiSetIpConfigurationDTO {
  ssid?: string | null;
  ipConfiguration?: IpConfigurationDTO;
}

export interface WiFiSsidDTO {
  ssid?: string | null;
  isConnected?: boolean;
  isSaved?: boolean;
  /** @format int32 */
  quality?: number;
  frequencies?: string | null;
  isOpen?: boolean;
}

export interface WiFiSsidRequestDTO {
  ssid?: string | null;
}
