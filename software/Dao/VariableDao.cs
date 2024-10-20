using System.Collections.Generic;
using System.Data;
using cog1.Business;
using cog1.DTO;
using System.Threading;
using System.Linq;
using cog1.Entities;
using Microsoft.Extensions.Logging;

namespace cog1.Dao
{
    /// <summary>
    /// Dao class for handling the "Variable" entity
    /// </summary>
    public class VariableDao : DaoBase
    {
        private Dictionary<int, VariableDTO> variables = null;
        private static readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

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
                new VariableDTO() { variableId =  5, description = "Analog voltage input 1", type = VariableType.FloatingPoint, direction = VariableDirection.Input },
                new VariableDTO() { variableId =  6, description = "Analog voltage input 2", type = VariableType.FloatingPoint, direction = VariableDirection.Input },
                new VariableDTO() { variableId =  7, description = "Analog voltage input 3", type = VariableType.FloatingPoint, direction = VariableDirection.Input },
                new VariableDTO() { variableId =  8, description = "Analog voltage input 4", type = VariableType.FloatingPoint, direction = VariableDirection.Input },
                new VariableDTO() { variableId =  9, description = "Analog current input 1", type = VariableType.FloatingPoint, direction = VariableDirection.Input },
                new VariableDTO() { variableId = 10, description = "Analog current input 2", type = VariableType.FloatingPoint, direction = VariableDirection.Input },
                new VariableDTO() { variableId = 11, description = "Analog current input 3", type = VariableType.FloatingPoint, direction = VariableDirection.Input },
                new VariableDTO() { variableId = 12, description = "Analog current input 4", type = VariableType.FloatingPoint, direction = VariableDirection.Input },
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
            semaphore.Wait();
            try
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
            finally
            {
                semaphore.Release();
            }
        }

        private List<VariableDTO> _GetVariables()
        {
            LoadVariables();
            semaphore.Wait();
            try
            {
                return variables.Select(item => item.Value).ToList();      // Clone
            }
            finally
            {
                semaphore.Release();
            }
        }

        #endregion

        #region CRUD

        public List<VariableDTO> EnumerateVariables()
        {
            return _GetVariables();
        }

        public VariableDTO GetVariable(int VariableId)
        {
            LoadVariables();
            semaphore.Wait();
            try
            {
                if (variables.TryGetValue(VariableId, out VariableDTO Variable))
                    return Variable;
                return null;
            }
            finally
            {
                semaphore.Release();
            }
        }

        public VariableDTO GetVariable(string variableCode)
        {
            LoadVariables();
            semaphore.Wait();
            try
            {
                return variables.Values.FirstOrDefault(item => string.Equals(item.variableCode, variableCode, System.StringComparison.OrdinalIgnoreCase));
            }
            finally
            {
                semaphore.Release();
            }
        }

        public List<VariableValueDTO> GetVariableValues()
        {
            return Context.Db.GetDataTable("select variable_id, value, utc_last_updated from variables")
                .AsEnumerable()
                .Select(row => MakeVariableValue(row))
                .ToList();
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
                    Context.Db.Execute(
                        "insert into variables (variable_id, description, variable_type, variable_direction) " +
                        "values (@variable_id, @description, @variable_type, @variable_direction)",
                        new()
                        {
                            { "@variable_id", v.variableId },
                            { "@description", v.description },
                            { "@variable_type", v.type},
                            { "@variable_direction", v.direction }
                        });
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
