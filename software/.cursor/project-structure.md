# Project Structure Documentation

## Overview
This is a **C# .NET 8.0 Web API project** (cog1) with an **Angular-based console** frontend. The project implements an industrial IoT system with Modbus protocol support, hardware monitoring, and system management capabilities.

## Architecture Pattern
The project follows a **layered architecture** with clear separation of concerns:
- **Controllers** → **Business Logic** → **Data Access Objects** → **Database**
- **Background Services** for async operations and system monitoring
- **DTOs** for API communication
- **Angular Console** for UI/UX

---

## Folder Structure

### **Root Configuration Files**
- `Program.cs` - Application entry point and dependency injection setup
- `Startup.cs` - Configuration and middleware initialization
- `Global.cs` - Global constants and utilities
- `Config.cs` - Configuration management
- `cog1.csproj` - Project file (.NET 8.0 Web SDK)
- `cog1.sln` - Solution file
- `appsettings.json` - Default app settings
- `appsettings.Development.json` - Development-specific settings
- `.cursor/` - Cursor AI configuration and guidelines

---

## Main Directories

### **1. BackgroundServices/**
Hosted background services for async operations and monitoring:
- `AnalogInputPollerService.cs` - Polls analog input hardware
- `BackgroundTelemetryService.cs` - Collects and processes telemetry data
- `HeartbeatService.cs` - System health monitoring
- `HousekeepingService.cs` - Maintenance and cleanup tasks
- `MenuLoopService.cs` - Console menu handling
- `ModbusInterfaceBaseService.cs` - Base class for Modbus services
- `ModbusRtuService.cs` - RTU protocol handler
- `ModbusService.cs` - Core Modbus logic
- `ModbusTcpService.cs` - TCP protocol handler
- `VariablePollingService.cs` - Polls variable values
- `WiFiMonitorService.cs` - Network connectivity monitoring

### **2. Business/**
Business logic layer implementing core domain operations:
- `BusinessBase.cs` - Abstract base class for business logic
- `Cog1Context.cs` - Application context and state management
- `MasterEntityBusiness.cs` - Master entity operations
- `ModbusBusiness.cs` - Modbus protocol business logic
- `SecurityBusiness.cs` - Authentication and authorization logic
- `UserBusiness.cs` - User account management
- `VariableBusiness.cs` - Variable operations and calculations

### **3. Controllers/**
REST API controllers handling HTTP requests:
- `Cog1ControllerBase.cs` - Abstract base controller with common functionality
- `EntitiesController.cs` - Entity CRUD endpoints
- `ModbusController.cs` - Modbus configuration and monitoring endpoints
- `SecurityController.cs` - Authentication and security endpoints
- `SystemController.cs` - System stats and health endpoints
- `VariablesController.cs` - Variable management endpoints

### **4. Dao/** (Data Access Objects)
Data persistence layer:
- `DaoBase.cs` - Abstract base class for data access
- `ModbusDao.cs` - Modbus data persistence
- `UserDao.cs` - User data persistence
- `VariableDao.cs` - Variable data persistence

### **5. DB/**
Database layer:
- `Cog1DBContext.cs` - Entity Framework Core DbContext

### **6. DTO/**
Data Transfer Objects for API contracts:
- **Authentication:** `LoginRequestDTO.cs`, `LoginResponseDTO.cs`, `AccessTokenInfoDTO.cs`
- **Modbus:** `ModbusDataTypeDTO.cs`, `ModbusRegisterDTO.cs`, `ModbusRegisterTypeDTO.cs`
- **System Monitoring:** `SystemStats.cs`, `SystemStatsReport.cs`, `CPUReport.cs`, `MemoryReportDTO.cs`, `DiskReport.cs`, `TemperatureReport.cs`
- **Networking:** `EthernetReport.cs`, `EthernetLinkConfigurationDTO.cs`, `IpConfigurationDTO.cs`, `WiFiReport.cs`, `WiFiConnectRequestDTO.cs`, `WiFiSetIpConfigurationDTO.cs`, `WiFiSsidDTO.cs`, `WiFiSsidRequestDTO.cs`
- **Entities:** `BasicEntitiesContainerDTO.cs`, `UserDTO.cs`, `UserWithPasswordDTO.cs`
- **Variables:** `VariableDTO.cs`, `VariableValueDTO.cs`, `VariableSourceDTO.cs`, `VariableDirectionDTO.cs`, `VariableTypeDTO.cs`
- **Localization:** `LocaleDTO.cs`
- **System:** `DateReport.cs`, `UpdateProfileRequestDTO.cs`

### **7. Entities/**
Domain entity definitions:
- `OutputStartupType.cs` - Output initialization types
- `VariableDirection.cs` - Input/output direction enum
- `VariableSource.cs` - Variable source types
- `VariableType.cs` - Variable data types

### **8. Exceptions/**
Custom exception handling:
- `ControllerException.cs` - Base exception for controller operations
- `ErrorCode.cs` - Individual error code definitions
- `ErrorCodes.cs` - Collection of error codes

### **9. Hardware/**
Hardware interface and management:
- `EthernetManager.cs` - Ethernet configuration and monitoring
- `IOManager.cs` - Input/output management
- Additional hardware-specific implementations (see `...` in structure)

### **10. Modbus/**
Modbus protocol implementation:
- `ModbusErrorInfo.cs` - Error information and handling
- `ModbusRtuServer.cs` - RTU server implementation
- `ModbusServer.cs` - Base Modbus server
- `ModbusTcpServer.cs` - TCP server implementation
- `TcpSlave.cs` - TCP slave device handler

### **11. Middleware/**
HTTP middleware components for request/response handling

### **12. Shared/**
Shared utilities and common code (excluded from compilation per csproj)

### **13. Display/**
Display and rendering logic:
- `DisplayCanvas.cs` - Canvas for rendering UI elements
- `bitmaps/` - Bitmap resources
- `Menu/` - Menu UI components

### **14. Literals/**
String constants and literal values

### **15. Utils/**
Utility functions and helper classes

### **16. Properties/**
.NET project properties

### **17. console/** (Angular Frontend)
Separate Angular 17+ application:
- `src/` - TypeScript/HTML source code
  - `app/` - Angular components and services
  - `assets/` - Static assets (images, styles, etc.)
  - `components/` - Reusable UI components
  - `scss/` - SCSS stylesheets
  - `index.html` - Main HTML template
  - `main.ts` - Application bootstrap
  - `test.ts` - Test configuration
- `package.json` - Node.js dependencies
- `angular.json` - Angular CLI configuration
- `tsconfig.json` - TypeScript configuration
- `karma.conf.js` - Test runner configuration

### **18. bin/ & obj/**
Build output directories (Debug/Release)

### **19. .github/**
GitHub configuration (workflows, actions, etc.)

### **20. .config/**
Configuration files

---

## Key Technologies & Frameworks

### Backend
- **.NET 8.0** - Latest LTS framework
- **ASP.NET Core** - Web API framework
- **Entity Framework Core** - ORM for database access
- **Modbus Protocol** - Industrial communication (RTU & TCP)

### Frontend
- **Angular** - Modern SPA framework
- **TypeScript** - Typed JavaScript
- **SCSS** - CSS preprocessing
- **Karma** - Test runner
- **Angular CLI** - Build tooling

### Protocols & Communication
- **Modbus RTU** - Serial protocol
- **Modbus TCP** - Network protocol
- **WiFi/Ethernet** - Network connectivity

---

## Development Workflow

1. **API Development:**
   - Create DTOs in `DTO/` for request/response contracts
   - Implement business logic in `Business/`
   - Add data access in `Dao/`
   - Create controller endpoints in `Controllers/`

2. **Background Tasks:**
   - Inherit from service base classes in `BackgroundServices/`
   - Register in dependency injection (Program.cs/Startup.cs)
   - Implement `ExecuteAsync()` or scheduled operations

3. **Hardware Integration:**
   - Implement hardware interfaces in `Hardware/`
   - Use managers for device communication
   - Log errors using Exception/ErrorCode system

4. **Frontend Development:**
   - Work in `console/src/` with Angular CLI
   - Use component architecture for reusability
   - Consume API endpoints from backend
   - Deploy via `console/deploy.cmd`

---

## Configuration & Secrets

- **App Settings:** `appsettings.json` (default), `appsettings.Development.json` (dev overrides)
- **User Secrets:** Managed via .NET User Secrets (configured in csproj)
- **Never hardcode secrets** - use environment variables or secret manager

---

## Guidelines for Contributors

- Follow guidelines in `.cursor/general-guidelines.mdc`
- Maintain separation of concerns (Controllers → Business → DAO)
- Use DTOs for all API communication
- Add meaningful logging and error handling
- Write unit/integration tests where applicable
- Keep frontend and backend changes in separate commits
- Update this documentation when adding new major components
