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
   * @name enumerateVariables
   * @request GET:/api/variables
   */
  enumerateVariables = (params: RequestParams = {}) =>
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
   * @name getVariableValues
   * @request GET:/api/variables/values
   */
  getVariableValues = (params: RequestParams = {}) =>
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
   * @name getVariableValue
   * @request GET:/api/variables/values/{variableId}
   */
  getVariableValue = (variableId: number, params: RequestParams = {}) =>
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
   * @name ping
   * @request GET:/api/variables/ping
   */
  ping = (params: RequestParams = {}) =>
    this.request<string, any>({
      path: `/api/variables/ping`,
      method: "GET",
      format: "json",
      ...params,
    });
}
