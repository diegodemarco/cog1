using cog1.Business;
using cog1.Dao;
using cog1.Entities;
using cog1.Hardware;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace cog1.BackgroundServices
{

    /// <summary>
    /// The VariablePolling background service takes care of updating variables
    /// that need polling, on a regular basis. This is needed for built-in 
    /// read-only variables as well as Modbus variables that need the bus to be
    /// polled in order to read the variables.
    /// </summary>
    /// <param name="logger">
    /// Logger used by the background service
    /// </param>
    /// <param name="scopeFactory">
    /// Scope factory used to create new scopes as needed, mostly to instantiate contexts 
    /// to access the database.
    /// </param>
    public class VariablePollingService(ILogger<VariablePollingService> logger, IServiceScopeFactory scopeFactory) : BackgroundService
    {
        private class VariableDefEntry : VariableDao.BasicVariableDefinition
        {
            public long nextPoll;
        }

        private long variablesSignature = 0;
        private Stopwatch stopWatch = Stopwatch.StartNew();
        private List<VariableDefEntry> variableDefs = new();

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("VariablePolling service started");

            //VariableBusiness.AddChangeHandler(this)

            // Signal that the background task has started, while postponing the first polling for 1 second
            await Utils.CancellableDelay(1000, stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var nextPoll = PollVariables();

                    // Wait for the next time when polling will be needed
                    await Utils.CancellableDelay(nextPoll, stoppingToken);
                }
                catch (Exception ex)
                {
                    logger.LogError($"Error in VariablePolling service: {ex}");
                    await Utils.CancellableDelay(30000, stoppingToken);
                }
            }

            logger.LogInformation("VariablePolling service stopped");
        }

        private IServiceScope CreateCope()
        {
            return scopeFactory.CreateScope();
        }

        private Cog1Context CreateContext(IServiceScope scope)
        {
            return scope.ServiceProvider.GetService<Cog1Context>();
        }

        private void UpdateVariableDefinitions()
        {
            var vars = VariableDao.GetInMemoryVariableDefinitions(ref variablesSignature);
            if (vars == null)
                // Nothing changed
                return;

            // Update existing variables, and add missing ones
            foreach (var v in vars.Where(item => item.pollIntervalMs > 0))
            {
                var currentVar = variableDefs.Find(item => item.variableId == v.variableId);
                if (currentVar == null)
                {
                    // New variable: add
                    variableDefs.Add(new VariableDefEntry()
                    {
                        variableId = v.variableId,
                        source = v.source,
                        type = v.type,
                        accessType = v.accessType,
                        pollIntervalMs = v.pollIntervalMs,
                        nextPoll = stopWatch.ElapsedMilliseconds + v.pollIntervalMs,
                        modbusRegister = v.modbusRegister
                    });
                }
                else
                {
                    // Existing variable: update
                    currentVar.accessType = v.accessType;
                    currentVar.pollIntervalMs = v.pollIntervalMs;
                    currentVar.nextPoll = stopWatch.ElapsedMilliseconds + v.pollIntervalMs;
                }

            }

            // Remove variables that don't exist anymore
            variableDefs.RemoveAll(item => !vars.Any(x => x.variableId == item.variableId));

            // Done
            logger.LogInformation($"Updated variable definitions, change detected");
        }

        private void PollBuiltInVariable(int variableId)
        {
            bool d;
            double a;

            switch (variableId)
            {
                case IOManager.DI1_VARIABLE_ID:
                    IOManager.ReadDI(out d, out _, out _, out _);
                    IOManager.SetVariableValue(IOManager.DI1_VARIABLE_ID, d ? 1 : 0);
                    break;
                case IOManager.DI2_VARIABLE_ID:
                    IOManager.ReadDI(out _, out d, out _, out _);
                    IOManager.SetVariableValue(IOManager.DI2_VARIABLE_ID, d ? 1 : 0);
                    break;
                case IOManager.DI3_VARIABLE_ID:
                    IOManager.ReadDI(out _, out _, out d, out _);
                    IOManager.SetVariableValue(IOManager.DI3_VARIABLE_ID, d ? 1 : 0);
                    break;
                case IOManager.DI4_VARIABLE_ID:
                    IOManager.ReadDI(out _, out _, out _, out d);
                    IOManager.SetVariableValue(IOManager.DI4_VARIABLE_ID, d ? 1 : 0);
                    break;
                case IOManager.AV1_VARIABLE_ID:
                    IOManager.Read010V(out a, out _, out _, out _);
                    IOManager.SetVariableValue(IOManager.AV1_VARIABLE_ID, a);
                    break;
                case IOManager.AV2_VARIABLE_ID:
                    IOManager.Read010V(out _, out a, out _, out _);
                    IOManager.SetVariableValue(IOManager.AV2_VARIABLE_ID, a);
                    break;
                case IOManager.AV3_VARIABLE_ID:
                    IOManager.Read010V(out _, out _, out a, out _);
                    IOManager.SetVariableValue(IOManager.AV3_VARIABLE_ID, a);
                    break;
                case IOManager.AV4_VARIABLE_ID:
                    IOManager.Read010V(out _, out _, out _, out a);
                    IOManager.SetVariableValue(IOManager.AV4_VARIABLE_ID, a);
                    break;
                case IOManager.AC1_VARIABLE_ID:
                    IOManager.Read020mA(out a, out _, out _, out _);
                    IOManager.SetVariableValue(IOManager.AC1_VARIABLE_ID, a);
                    break;
                case IOManager.AC2_VARIABLE_ID:
                    IOManager.Read020mA(out _, out a, out _, out _);
                    IOManager.SetVariableValue(IOManager.AC2_VARIABLE_ID, a);
                    break;
                case IOManager.AC3_VARIABLE_ID:
                    IOManager.Read020mA(out _, out _, out a, out _);
                    IOManager.SetVariableValue(IOManager.AC3_VARIABLE_ID, a);
                    break;
                case IOManager.AC4_VARIABLE_ID:
                    IOManager.Read020mA(out _, out _, out _, out a);
                    IOManager.SetVariableValue(IOManager.AC4_VARIABLE_ID, a);
                    break;
            }
        }

        private int PollVariables()
        {
            UpdateVariableDefinitions();

            if (variableDefs.Count <= 0)
                return 1000;

            // Poll variables that need to be updated
            var now = stopWatch.ElapsedMilliseconds;
            var varsToPoll = variableDefs.Where(item => item.nextPoll <= now);
            foreach (var v in varsToPoll)
            {
                if (v.source == Entities.VariableSource.BuiltIn)
                {
                    PollBuiltInVariable(v.variableId);
                    v.nextPoll = now + v.pollIntervalMs;
                }
                else if (v.source == VariableSource.Modbus)
                {
                    //Console.WriteLine($"Queing Modbus variable read ({v.variableId}): {JsonConvert.SerializeObject(v.modbusRegister)}");
                    ModbusService.QueueRead(v);
                    v.nextPoll = now + v.pollIntervalMs;
                }
            }

            // Calculate next overall poll, from 100 ms to 1 s
            var result = now - variableDefs.Select(item => item.nextPoll).Min();
            if (result < 100)
                result = 100;
            if (result > 1000)
                result = 1000;

            // Done
            return (int)result;
        }

    }

}
