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

import { LiteralsContainerDTO } from "./data-contracts";
import { HttpClient, RequestParams } from "./http-client";

export class Literals<SecurityDataType = unknown> extends HttpClient<SecurityDataType> {
  /**
   * No description
   *
   * @tags Literals
   * @name GetLiterals
   * @request GET:/api/literals
   */
  GetLiterals = (params: RequestParams = {}) =>
    this.request<LiteralsContainerDTO, any>({
      path: `/api/literals`,
      method: "GET",
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Literals
   * @name Ping
   * @request GET:/api/literals/ping
   */
  Ping = (params: RequestParams = {}) =>
    this.request<string, any>({
      path: `/api/literals/ping`,
      method: "GET",
      format: "json",
      ...params,
    });
}
