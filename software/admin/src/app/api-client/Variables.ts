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

import { VariableDTO, VariableValueDTO } from "./data-contracts";
import { HttpClient, RequestParams } from "./http-client";

export class Variables<SecurityDataType = unknown> extends HttpClient<SecurityDataType> {
  /**
   * No description
   *
   * @tags Variables
   * @name EnumerateVariables
   * @request GET:/api/variables
   */
  EnumerateVariables = (params: RequestParams = {}) =>
    this.request<VariableDTO[], any>({
      path: `/api/variables`,
      method: "GET",
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Variables
   * @name GetVariableValues
   * @request GET:/api/variables/values
   */
  GetVariableValues = (params: RequestParams = {}) =>
    this.request<VariableValueDTO[], any>({
      path: `/api/variables/values`,
      method: "GET",
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Variables
   * @name GetVariableValue
   * @request GET:/api/variables/values/{variableId}
   */
  GetVariableValue = (variableId: number, params: RequestParams = {}) =>
    this.request<VariableValueDTO, any>({
      path: `/api/variables/values/${variableId}`,
      method: "GET",
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Variables
   * @name Ping
   * @request GET:/api/variables/ping
   */
  Ping = (params: RequestParams = {}) =>
    this.request<string, any>({
      path: `/api/variables/ping`,
      method: "GET",
      format: "json",
      ...params,
    });
}
