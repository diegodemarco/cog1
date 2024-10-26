using System.Collections.Generic;
using System.Data;
using cog1.Business;
using cog1.DTO;
using System.Linq;
using cog1.Entities;
using Microsoft.Extensions.Logging;
using System;
using cog1.Exceptions;

namespace cog1.Dao
{
    /// <summary>
    /// Dao class for handling the "Variable" entity
    /// </summary>
    public class VariableDao : DaoBase
    {
        private object _lock = new object();
        private Dictionary<int, VariableDTO> variables = null;
        //private static readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        public VariableDao(Cog1Context context, ILogger logger) : base(context, logger)
        {
        }

        #region Private methods

        private static List<VariableDTO> EnumerateBuiltInVariables()
        {
            return new List<VariableDTO>()
            {
                new VariableDTO() { variableId =  1, description = "Digital input 1", type = VariableType.Binary, direction = VariableDirection.Input },
                new VariableDTO() { variableId =  2, description = "Digital input 2", type = VariableType.Binary, direction = VariableDirection.Input },
                new VariableDTO() { variableId =  3, description = "Digital input 3", type = VariableType.Binary, direction = VariableDirection.Input },
                new VariableDTO() { variableId =  4, description = "Digital input 4", type = VariableType.Binary, direction = VariableDirection.Input },
                new VariableDTO() { variableId =  5, description = "Analog voltage input 1", type = VariableType.FloatingPoint, direction = VariableDirection.Input, units = "V"},
                new VariableDTO() { variableId =  6, description = "Analog voltage input 2", type = VariableType.FloatingPoint, direction = VariableDirection.Input, units = "V"},
                new VariableDTO() { variableId =  7, description = "Analog voltage input 3", type = VariableType.FloatingPoint, direction = VariableDirection.Input, units = "V"},
                new VariableDTO() { variableId =  8, description = "Analog voltage input 4", type = VariableType.FloatingPoint, direction = VariableDirection.Input, units = "V"},
                new VariableDTO() { variableId =  9, description = "Analog current input 1", type = VariableType.FloatingPoint, direction = VariableDirection.Input, units = "mA"},
                new VariableDTO() { variableId = 10, description = "Analog current input 2", type = VariableType.FloatingPoint, direction = VariableDirection.Input, units = "mA"},
                new VariableDTO() { variableId = 11, description = "Analog current input 3", type = VariableType.FloatingPoint, direction = VariableDirection.Input, units = "mA"},
                new VariableDTO() { variableId = 12, description = "Analog current input 4", type = VariableType.FloatingPoint, direction = VariableDirection.Input, units = "mA"},
                new VariableDTO() { variableId = 13, description = "Digital output 1", type = VariableType.Binary, direction = VariableDirection.Output },
                new VariableDTO() { variableId = 14, description = "Digital output 2", type = VariableType.Binary, direction = VariableDirection.Output },
                new VariableDTO() { variableId = 15, description = "Digital output 3", type = VariableType.Binary, direction = VariableDirection.Output },
                new VariableDTO() { variableId = 16, description = "Digital output 4", type = VariableType.Binary, direction = VariableDirection.Output },
            };
        }

        private VariableDTO MakeVariable(DataRow r)
        {
            int variableId = (int)r.Field<long>("variable_id");
            return new VariableDTO
            {
                variableId = variableId,
                description = r.Field<string>("description"),
                variableCode = r.Field<string>("variable_code"),
                type = (VariableType)r.Field<long>("variable_type"),
                direction = (VariableDirection)r.Field<long>("variable_direction"),
                units = r.Field<string>("units"),
                isBuiltIn = variableId < 1000
            };
        }

        private VariableValueDTO MakeVariableValue(DataRow r)
        {
            return new VariableValueDTO
            {
                variableId = (int)r.Field<long>("variable_id"),
                value = r.Field<double?>("value"),
                lastUpdateUtc = Utils.SqlToDateTime(r.Field<string>("utc_last_updated"))
            };
        }

        private void LoadVariables(bool forceReload = false)
        {
            lock (_lock)
            {
                if (forceReload && variables != null)
                {
                    variables.Clear();
                    variables = null;
                }
                if (variables == null)
                {
                    variables = Context.Db.GetDataTable("select * from variables")
                        .AsEnumerable()
                        .Select(row => MakeVariable(row))
                        .ToDictionary(item => item.variableId);
                }
            }
        }

        private List<VariableDTO> _GetVariables()
        {
            lock (_lock)
            {
                LoadVariables();
                return variables.Select(item => item.Value).ToList();      // Clone
            }
        }

        private void _CreateVariable(VariableDTO v)
        {
            Context.Db.Execute(
                "insert into variables (variable_id, description, variable_code, variable_type, variable_direction, units) " +
                "values (@variable_id, @description, @variable_code, @variable_type, @variable_direction, @units)",
                new()
                {
                    { "@variable_id", v.variableId },
                    { "@description", v.description },
                    { "@variable_code", string.IsNullOrWhiteSpace(v.variableCode) ? DBNull.Value : v.variableCode.Trim() },
                    { "@variable_type", v.type},
                    { "@variable_direction", v.direction },
                    { "@units", string.IsNullOrWhiteSpace(v.units) ? DBNull.Value : v.units.Trim() },
                });
        }

        private void _EditVariable(VariableDTO v)
        {
            Context.Db.Execute(
                "update variables set description = @description, variable_code = @variable_code, variable_direction = @variable_direction, units = @units " +
                "where variable_id = @variable_id",
                new()
                {
                    { "@description", v.description },
                    { "@variable_code", string.IsNullOrWhiteSpace(v.variableCode) ? DBNull.Value : v.variableCode.Trim() },
                    { "@variable_direction", v.direction },
                    { "@units", string.IsNullOrWhiteSpace(v.units) ? DBNull.Value : v.units.Trim() },
                    { "@variable_id", v.variableId },
                });
        }

        #endregion

        #region CRUD

        public List<VariableDTO> EnumerateVariables()
        {
            return _GetVariables();
        }

        public VariableDTO GetVariable(int VariableId)
        {
            lock (_lock)
            {
                LoadVariables();
                if (variables.TryGetValue(VariableId, out VariableDTO Variable))
                    return Variable;
                return null;
            }
        }

        public VariableDTO GetVariableByCode(string variableCode)
        {
            lock (_lock)
            {
                LoadVariables();
                if (string.IsNullOrWhiteSpace(variableCode)) 
                    return null;
                variableCode = variableCode.Trim();
                return variables.Values.FirstOrDefault(item => string.Equals(item.variableCode, variableCode, System.StringComparison.OrdinalIgnoreCase));
            }
        }

        public List<VariableValueDTO> GetVariableValues()
        {
            return Context.Db.GetDataTable("select variable_id, value, utc_last_updated from variables")
                .AsEnumerable()
                .Select(row => MakeVariableValue(row))
                .ToList();
        }

        public VariableDTO CreateVariable(VariableDTO v)
        {
            lock (_lock)
            {
                LoadVariables(true);

                // Check for duplicate variable code
                if (!string.IsNullOrWhiteSpace(v.variableCode) && GetVariableByCode(v.variableCode) != null)
                    throw new ControllerException(Context.ErrorCodes.Variables.DUPLICATE_VARIABLE_CODE);

                // Store
                v.variableId = variables.Select(item => item.Value.variableId).Max() + 1;
                if (v.variableId < 1001)
                    v.variableId = 1001;
                _CreateVariable(v);
                LoadVariables(true);
                return variables[v.variableId];
            }
        }

        public VariableDTO EditVariable(VariableDTO v)
        {
            lock (_lock)
            {
                LoadVariables(true);

                // Check for duplicate variable code
                if (!string.IsNullOrWhiteSpace(v.variableCode))
                {
                    var v2 = GetVariableByCode(v.variableCode);
                    if (v2 != null && v2.variableId != v.variableId)
                        throw new ControllerException(Context.ErrorCodes.Variables.DUPLICATE_VARIABLE_CODE);
                }

                var newVar = variables[v.variableId];
                newVar.description = v.description;
                newVar.variableCode = v.variableCode;
                if (!newVar.isBuiltIn)
                {
                    newVar.units = v.units;
                    newVar.direction = v.direction;
                }
                _EditVariable(v);
                LoadVariables(true);
                return variables[v.variableId];
            }
        }

        public void DeleteVariable(int variableId)
        {
            Context.Db.Execute(
                "delete from variables where variable_id = @variable_id",
                new()
                {
                    { "@variable_id", variableId }
                });
        }

        #endregion

        #region Startup fixes and housekeeping

        public void DoStartupFixes()
        {
            var varsCreated = false;
            foreach (var v in EnumerateBuiltInVariables())
            {
                if (GetVariable(v.variableId) == null)
                {
                    _CreateVariable(v);
                    varsCreated = true;
                    Logger.LogInformation($"Created missing variable {v.variableId}: {v.description}");
                }
            }
            if (varsCreated)
                LoadVariables(true);
        }

        #endregion

    }

}
