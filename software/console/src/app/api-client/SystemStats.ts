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

export class SystemStats<SecurityDataType = unknown> extends HttpClient<SecurityDataType> {
  /**
   * No description
   *
   * @tags SystemStats
   * @name GetSystemStats
   * @request GET:/api/systemstats
   */
  GetSystemStats = (params: RequestParams = {}) =>
    this.request<SystemStatsReport, any>({
      path: `/api/systemstats`,
      method: "GET",
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags SystemStats
   * @name Ping
   * @request GET:/api/systemstats/ping
   */
  Ping = (params: RequestParams = {}) =>
    this.request<string, any>({
      path: `/api/systemstats/ping`,
      method: "GET",
      format: "json",
      ...params,
    });
}
