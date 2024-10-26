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

import {
  AccessTokenInfoDTO,
  LoginRequestDTO,
  LoginResponseDTO,
  UpdateProfileRequestDTO,
  UserDTO,
  UserWithPasswordDTO,
} from "./data-contracts";
import { ContentType, HttpClient, RequestParams } from "./http-client";

export class Security<SecurityDataType = unknown> extends HttpClient<SecurityDataType> {
  /**
   * No description
   *
   * @tags Security
   * @name login
   * @request POST:/api/security/login
   */
  login = (data: LoginRequestDTO, params: RequestParams = {}) =>
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
   * @name getAccessTokenInfo
   * @request GET:/api/security/access-token
   */
  getAccessTokenInfo = (params: RequestParams = {}) =>
    this.request<AccessTokenInfoDTO, any>({
      path: `/api/security/access-token`,
      method: "GET",
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Security
   * @name enumerateUsers
   * @request GET:/api/security/users
   */
  enumerateUsers = (params: RequestParams = {}) =>
    this.request<UserDTO[], any>({
      path: `/api/security/users`,
      method: "GET",
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Security
   * @name createUser
   * @request POST:/api/security/users
   */
  createUser = (data: UserWithPasswordDTO, params: RequestParams = {}) =>
    this.request<UserDTO, any>({
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
   * @name editUser
   * @request PUT:/api/security/users
   */
  editUser = (data: UserWithPasswordDTO, params: RequestParams = {}) =>
    this.request<UserDTO, any>({
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
   * @name getUser
   * @request GET:/api/security/users/{userId}
   */
  getUser = (userId: number, params: RequestParams = {}) =>
    this.request<UserDTO, any>({
      path: `/api/security/users/${userId}`,
      method: "GET",
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Security
   * @name deleteUser
   * @request DELETE:/api/security/users/{userId}
   */
  deleteUser = (userId: number, params: RequestParams = {}) =>
    this.request<any, any>({
      path: `/api/security/users/${userId}`,
      method: "DELETE",
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Security
   * @name updateUserProfile
   * @request POST:/api/security/users/profile
   */
  updateUserProfile = (data: UpdateProfileRequestDTO, params: RequestParams = {}) =>
    this.request<UserDTO, any>({
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
   * @name ping
   * @request GET:/api/security/ping
   */
  ping = (params: RequestParams = {}) =>
    this.request<string, any>({
      path: `/api/security/ping`,
      method: "GET",
      format: "json",
      ...params,
    });
}
