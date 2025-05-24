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
  BasicEntitiesContainerDTO,
  JsonControllerException,
} from "./data-contracts";
import { HttpClient, RequestParams } from "./http-client";

export class Entities<
  SecurityDataType = unknown,
> extends HttpClient<SecurityDataType> {
  /**
   * No description
   *
   * @tags Entities
   * @name GetBasicEntities
   * @request GET:/api/entities/basic
   */
  getBasicEntities = (params: RequestParams = {}) =>
    this.request<BasicEntitiesContainerDTO, JsonControllerException>({
      path: `/api/entities/basic`,
      method: "GET",
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Entities
   * @name Ping
   * @request GET:/api/entities/ping
   */
  ping = (params: RequestParams = {}) =>
    this.request<string, JsonControllerException>({
      path: `/api/entities/ping`,
      method: "GET",
      format: "json",
      ...params,
    });
}
