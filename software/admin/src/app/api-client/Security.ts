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

import { LoginRequestDTO, LoginResponseDTO } from "./data-contracts";
import { ContentType, HttpClient, RequestParams } from "./http-client";

export class Security<SecurityDataType = unknown> extends HttpClient<SecurityDataType> {
  /**
   * No description
   *
   * @tags Security
   * @name Login
   * @request POST:/api/security/login
   */
  Login = (data: LoginRequestDTO, params: RequestParams = {}) =>
    this.request<LoginResponseDTO, any>({
      path: `/api/security/login`,
      method: "POST",
      body: data,
      type: ContentType.Json,
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Security
   * @name Ping
   * @request GET:/api/security/ping
   */
  Ping = (params: RequestParams = {}) =>
    this.request<string, any>({
      path: `/api/security/ping`,
      method: "GET",
      format: "json",
      ...params,
    });
}
