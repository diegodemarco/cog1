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

import { UpdateProfileRequestDTO, UserDTO } from "./data-contracts";
import { ContentType, HttpClient, RequestParams } from "./http-client";

export class Users<SecurityDataType = unknown> extends HttpClient<SecurityDataType> {
  /**
   * No description
   *
   * @tags Users
   * @name updateUserProfile
   * @request POST:/api/users/profile
   */
  updateUserProfile = (data: UpdateProfileRequestDTO, params: RequestParams = {}) =>
    this.request<UserDTO, any>({
      path: `/api/users/profile`,
      method: "POST",
      body: data,
      type: ContentType.Json,
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Users
   * @name ping
   * @request GET:/api/users/ping
   */
  ping = (params: RequestParams = {}) =>
    this.request<string, any>({
      path: `/api/users/ping`,
      method: "GET",
      format: "json",
      ...params,
    });
}
