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
  AccessTokenInfoDTO,
  JsonControllerException,
  LoginRequestDTO,
  LoginResponseDTO,
  UpdateProfileRequestDTO,
  UserDTO,
  UserWithPasswordDTO,
} from "./data-contracts";
import { ContentType, HttpClient, RequestParams } from "./http-client";

export class Security<
  SecurityDataType = unknown,
> extends HttpClient<SecurityDataType> {
  /**
   * No description
   *
   * @tags Security
   * @name Login
   * @request POST:/api/security/login
   */
  login = (data: LoginRequestDTO, params: RequestParams = {}) =>
    this.request<LoginResponseDTO, JsonControllerException>({
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
   * @name GetAccessTokenInfo
   * @request GET:/api/security/access-token
   */
  getAccessTokenInfo = (params: RequestParams = {}) =>
    this.request<AccessTokenInfoDTO, JsonControllerException>({
      path: `/api/security/access-token`,
      method: "GET",
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Security
   * @name EnumerateUsers
   * @request GET:/api/security/users
   */
  enumerateUsers = (params: RequestParams = {}) =>
    this.request<UserDTO[], JsonControllerException>({
      path: `/api/security/users`,
      method: "GET",
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Security
   * @name CreateUser
   * @request POST:/api/security/users
   */
  createUser = (data: UserWithPasswordDTO, params: RequestParams = {}) =>
    this.request<UserDTO, JsonControllerException>({
      path: `/api/security/users`,
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
   * @name EditUser
   * @request PUT:/api/security/users
   */
  editUser = (data: UserWithPasswordDTO, params: RequestParams = {}) =>
    this.request<UserDTO, JsonControllerException>({
      path: `/api/security/users`,
      method: "PUT",
      body: data,
      type: ContentType.Json,
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Security
   * @name GetUser
   * @request GET:/api/security/users/{userId}
   */
  getUser = (userId: number, params: RequestParams = {}) =>
    this.request<UserDTO, JsonControllerException>({
      path: `/api/security/users/${userId}`,
      method: "GET",
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Security
   * @name DeleteUser
   * @request DELETE:/api/security/users/{userId}
   */
  deleteUser = (userId: number, params: RequestParams = {}) =>
    this.request<any, JsonControllerException>({
      path: `/api/security/users/${userId}`,
      method: "DELETE",
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Security
   * @name UpdateUserProfile
   * @request POST:/api/security/users/profile
   */
  updateUserProfile = (
    data: UpdateProfileRequestDTO,
    params: RequestParams = {},
  ) =>
    this.request<UserDTO, JsonControllerException>({
      path: `/api/security/users/profile`,
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
  ping = (params: RequestParams = {}) =>
    this.request<string, JsonControllerException>({
      path: `/api/security/ping`,
      method: "GET",
      format: "json",
      ...params,
    });
}
