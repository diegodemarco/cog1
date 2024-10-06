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
