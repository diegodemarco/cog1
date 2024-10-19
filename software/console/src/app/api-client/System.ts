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

import { SystemStatsReport } from "./data-contracts";
import { HttpClient, RequestParams } from "./http-client";

export class System<SecurityDataType = unknown> extends HttpClient<SecurityDataType> {
  /**
   * No description
   *
   * @tags System
   * @name getSystemStats
   * @request GET:/api/system/stats
   */
  getSystemStats = (params: RequestParams = {}) =>
    this.request<SystemStatsReport, any>({
      path: `/api/system/stats`,
      method: "GET",
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags System
   * @name getCpuHistory5Min
   * @request GET:/api/system/stats/cpu/history-5min
   */
  getCpuHistory5Min = (params: RequestParams = {}) =>
    this.request<number[], any>({
      path: `/api/system/stats/cpu/history-5min`,
      method: "GET",
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags System
   * @name ping
   * @request GET:/api/system/ping
   */
  ping = (params: RequestParams = {}) =>
    this.request<string, any>({
      path: `/api/system/ping`,
      method: "GET",
      format: "json",
      ...params,
    });
}
