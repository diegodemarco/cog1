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
  JsonControllerException,
  VariableDTO,
  VariableValueDTO,
} from "./data-contracts";
import { ContentType, HttpClient, RequestParams } from "./http-client";

export class Variables<
  SecurityDataType = unknown,
> extends HttpClient<SecurityDataType> {
  /**
   * No description
   *
   * @tags Variables
   * @name EnumerateVariables
   * @request GET:/api/variables
   */
  enumerateVariables = (params: RequestParams = {}) =>
    this.request<VariableDTO[], JsonControllerException>({
      path: `/api/variables`,
      method: "GET",
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Variables
   * @name CreateVariable
   * @request POST:/api/variables
   */
  createVariable = (data: VariableDTO, params: RequestParams = {}) =>
    this.request<VariableDTO, JsonControllerException>({
      path: `/api/variables`,
      method: "POST",
      body: data,
      type: ContentType.Json,
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Variables
   * @name EditVariable
   * @request PUT:/api/variables
   */
  editVariable = (data: VariableDTO, params: RequestParams = {}) =>
    this.request<VariableDTO, JsonControllerException>({
      path: `/api/variables`,
      method: "PUT",
      body: data,
      type: ContentType.Json,
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Variables
   * @name GetVariableById
   * @request GET:/api/variables/{variableId}
   */
  getVariableById = (variableId: number, params: RequestParams = {}) =>
    this.request<VariableDTO, JsonControllerException>({
      path: `/api/variables/${variableId}`,
      method: "GET",
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Variables
   * @name DeleteVariable
   * @request DELETE:/api/variables/{variableId}
   */
  deleteVariable = (variableId: number, params: RequestParams = {}) =>
    this.request<any, JsonControllerException>({
      path: `/api/variables/${variableId}`,
      method: "DELETE",
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
  getVariableValues = (params: RequestParams = {}) =>
    this.request<VariableValueDTO[], JsonControllerException>({
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
  getVariableValue = (variableId: number, params: RequestParams = {}) =>
    this.request<VariableValueDTO, JsonControllerException>({
      path: `/api/variables/values/${variableId}`,
      method: "GET",
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Variables
   * @name SetVariableValue
   * @request POST:/api/variables/values/{variableId}
   */
  setVariableValue = (
    variableId: number,
    data: number,
    params: RequestParams = {},
  ) =>
    this.request<VariableValueDTO, JsonControllerException>({
      path: `/api/variables/values/${variableId}`,
      method: "POST",
      body: data,
      type: ContentType.Json,
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
  ping = (params: RequestParams = {}) =>
    this.request<string, JsonControllerException>({
      path: `/api/variables/ping`,
      method: "GET",
      format: "json",
      ...params,
    });
}
