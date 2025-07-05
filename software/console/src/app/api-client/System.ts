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

import {
  EthernetLinkConfigurationDTO,
  IpConfigurationDTO,
  JsonControllerException,
  SystemStatsReport,
  WiFiConnectRequestDTO,
  WiFiSetIpConfigurationDTO,
  WiFiSsidDTO,
  WiFiSsidRequestDTO,
} from "./data-contracts";
import { ContentType, HttpClient, RequestParams } from "./http-client";

export class System<
  SecurityDataType = unknown,
> extends HttpClient<SecurityDataType> {
  /**
   * No description
   *
   * @tags System
   * @name GetSystemStats
   * @request GET:/api/system/stats
   */
  getSystemStats = (params: RequestParams = {}) =>
    this.request<SystemStatsReport, JsonControllerException>({
      path: `/api/system/stats`,
      method: "GET",
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags System
   * @name GetCpuHistory5Min
   * @request GET:/api/system/stats/cpu/history-5min
   */
  getCpuHistory5Min = (params: RequestParams = {}) =>
    this.request<number[], JsonControllerException>({
      path: `/api/system/stats/cpu/history-5min`,
      method: "GET",
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags System
   * @name GetWiFiScan
   * @request GET:/api/system/wifi/scan
   */
  getWiFiScan = (params: RequestParams = {}) =>
    this.request<WiFiSsidDTO[], JsonControllerException>({
      path: `/api/system/wifi/scan`,
      method: "GET",
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags System
   * @name WiFiConnect
   * @request POST:/api/system/wifi/connect
   */
  wiFiConnect = (data: WiFiConnectRequestDTO, params: RequestParams = {}) =>
    this.request<any, JsonControllerException>({
      path: `/api/system/wifi/connect`,
      method: "POST",
      body: data,
      type: ContentType.Json,
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags System
   * @name WiFiReconnect
   * @request POST:/api/system/wifi/reconnect
   */
  wiFiReconnect = (data: WiFiSsidRequestDTO, params: RequestParams = {}) =>
    this.request<any, JsonControllerException>({
      path: `/api/system/wifi/reconnect`,
      method: "POST",
      body: data,
      type: ContentType.Json,
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags System
   * @name WiFiDisconnect
   * @request POST:/api/system/wifi/disconnect
   */
  wiFiDisconnect = (data: WiFiSsidRequestDTO, params: RequestParams = {}) =>
    this.request<any, JsonControllerException>({
      path: `/api/system/wifi/disconnect`,
      method: "POST",
      body: data,
      type: ContentType.Json,
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags System
   * @name WiFiForget
   * @request POST:/api/system/wifi/forget
   */
  wiFiForget = (data: WiFiSsidRequestDTO, params: RequestParams = {}) =>
    this.request<any, JsonControllerException>({
      path: `/api/system/wifi/forget`,
      method: "POST",
      body: data,
      type: ContentType.Json,
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags System
   * @name WiFiGetIpConfiguration
   * @request GET:/api/system/wifi/ip-configuration
   */
  wiFiGetIpConfiguration = (
    query?: {
      ssid?: string;
    },
    params: RequestParams = {},
  ) =>
    this.request<IpConfigurationDTO, JsonControllerException>({
      path: `/api/system/wifi/ip-configuration`,
      method: "GET",
      query: query,
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags System
   * @name WiFiSetIpConfiguration
   * @request POST:/api/system/wifi/ip-configuration
   */
  wiFiSetIpConfiguration = (
    data: WiFiSetIpConfigurationDTO,
    params: RequestParams = {},
  ) =>
    this.request<any, JsonControllerException>({
      path: `/api/system/wifi/ip-configuration`,
      method: "POST",
      body: data,
      type: ContentType.Json,
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags System
   * @name EthernetGetIpConfiguration
   * @request GET:/api/system/ethernet/ip-configuration
   */
  ethernetGetIpConfiguration = (params: RequestParams = {}) =>
    this.request<IpConfigurationDTO, JsonControllerException>({
      path: `/api/system/ethernet/ip-configuration`,
      method: "GET",
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags System
   * @name EthernetSetIpConfiguration
   * @request POST:/api/system/ethernet/ip-configuration
   */
  ethernetSetIpConfiguration = (
    data: IpConfigurationDTO,
    params: RequestParams = {},
  ) =>
    this.request<any, JsonControllerException>({
      path: `/api/system/ethernet/ip-configuration`,
      method: "POST",
      body: data,
      type: ContentType.Json,
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags System
   * @name EthernetGetLinkConfiguration
   * @request GET:/api/system/ethernet/link-configuration
   */
  ethernetGetLinkConfiguration = (params: RequestParams = {}) =>
    this.request<EthernetLinkConfigurationDTO, JsonControllerException>({
      path: `/api/system/ethernet/link-configuration`,
      method: "GET",
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags System
   * @name EthernetSetLinkConfiguration
   * @request POST:/api/system/ethernet/link-configuration
   */
  ethernetSetLinkConfiguration = (
    data: EthernetLinkConfigurationDTO,
    params: RequestParams = {},
  ) =>
    this.request<any, JsonControllerException>({
      path: `/api/system/ethernet/link-configuration`,
      method: "POST",
      body: data,
      type: ContentType.Json,
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags System
   * @name Ping
   * @request GET:/api/system/ping
   */
  ping = (params: RequestParams = {}) =>
    this.request<string, JsonControllerException>({
      path: `/api/system/ping`,
      method: "GET",
      format: "json",
      ...params,
    });
}
