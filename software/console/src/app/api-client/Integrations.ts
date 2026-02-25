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
  IntegrationConnectionDTO,
  JsonControllerException,
  OutboundIntegrationDTO,
} from "./data-contracts";
import { ContentType, HttpClient, RequestParams } from "./http-client";

export class Integrations<
  SecurityDataType = unknown,
> extends HttpClient<SecurityDataType> {
  /**
   * No description
   *
   * @tags Integrations
   * @name EnumerateConnections
   * @request GET:/api/integrations/connections
   */
  enumerateConnections = (params: RequestParams = {}) =>
    this.request<IntegrationConnectionDTO[], JsonControllerException>({
      path: `/api/integrations/connections`,
      method: "GET",
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Integrations
   * @name CreateConnection
   * @request POST:/api/integrations/connections
   */
  createConnection = (
    data: IntegrationConnectionDTO,
    params: RequestParams = {},
  ) =>
    this.request<IntegrationConnectionDTO, JsonControllerException>({
      path: `/api/integrations/connections`,
      method: "POST",
      body: data,
      type: ContentType.Json,
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Integrations
   * @name EditConnection
   * @request PUT:/api/integrations/connections
   */
  editConnection = (
    data: IntegrationConnectionDTO,
    params: RequestParams = {},
  ) =>
    this.request<IntegrationConnectionDTO, JsonControllerException>({
      path: `/api/integrations/connections`,
      method: "PUT",
      body: data,
      type: ContentType.Json,
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Integrations
   * @name GetConnectionById
   * @request GET:/api/integrations/connections/{id}
   */
  getConnectionById = (id: number, params: RequestParams = {}) =>
    this.request<IntegrationConnectionDTO, JsonControllerException>({
      path: `/api/integrations/connections/${id}`,
      method: "GET",
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Integrations
   * @name DeleteConnection
   * @request DELETE:/api/integrations/connections/{id}
   */
  deleteConnection = (id: number, params: RequestParams = {}) =>
    this.request<any, JsonControllerException>({
      path: `/api/integrations/connections/${id}`,
      method: "DELETE",
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Integrations
   * @name EnumerateOutboundIntegrations
   * @request GET:/api/integrations/outbound
   */
  enumerateOutboundIntegrations = (params: RequestParams = {}) =>
    this.request<OutboundIntegrationDTO[], JsonControllerException>({
      path: `/api/integrations/outbound`,
      method: "GET",
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Integrations
   * @name CreateOutboundIntegration
   * @request POST:/api/integrations/outbound
   */
  createOutboundIntegration = (
    data: OutboundIntegrationDTO,
    params: RequestParams = {},
  ) =>
    this.request<OutboundIntegrationDTO, JsonControllerException>({
      path: `/api/integrations/outbound`,
      method: "POST",
      body: data,
      type: ContentType.Json,
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Integrations
   * @name EditOutboundIntegration
   * @request PUT:/api/integrations/outbound
   */
  editOutboundIntegration = (
    data: OutboundIntegrationDTO,
    params: RequestParams = {},
  ) =>
    this.request<OutboundIntegrationDTO, JsonControllerException>({
      path: `/api/integrations/outbound`,
      method: "PUT",
      body: data,
      type: ContentType.Json,
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Integrations
   * @name GetOutboundIntegrationById
   * @request GET:/api/integrations/outbound/{id}
   */
  getOutboundIntegrationById = (id: number, params: RequestParams = {}) =>
    this.request<OutboundIntegrationDTO, JsonControllerException>({
      path: `/api/integrations/outbound/${id}`,
      method: "GET",
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Integrations
   * @name DeleteOutboundIntegration
   * @request DELETE:/api/integrations/outbound/{id}
   */
  deleteOutboundIntegration = (id: number, params: RequestParams = {}) =>
    this.request<any, JsonControllerException>({
      path: `/api/integrations/outbound/${id}`,
      method: "DELETE",
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Integrations
   * @name Ping
   * @request GET:/api/integrations/ping
   */
  ping = (params: RequestParams = {}) =>
    this.request<string, JsonControllerException>({
      path: `/api/integrations/ping`,
      method: "GET",
      format: "json",
      ...params,
    });
}
