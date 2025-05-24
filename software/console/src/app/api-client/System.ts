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

import { JsonControllerException, SystemStatsReport } from "./data-contracts";
import { HttpClient, RequestParams } from "./http-client";

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
