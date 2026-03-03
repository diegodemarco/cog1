using cog1.BackgroundServices;
using cog1.DTO;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Diagnostics;
using cog1.System;
using Microsoft.Extensions.DependencyInjection;

namespace cog1.Display.Menu
{
    public static partial class DisplayMenu
    {

        /// <summary>
        /// The menu loop service lives inside the DisplayMenu singleton, to 
        /// manage the display menu in response to events such as using the
        /// built-in encoder and its switch.
        /// This class is nested inside the DisplayMenu to have access to private 
        /// DisplayMenu fields and methods.
        /// </summary>
        /// <param name="logger">logger used by the background service</param>
        public class MenuLoopService(ILogger<MenuLoopService> logger, IServiceScopeFactory scopeFactory) : BaseBackgroundService(logger, scopeFactory, "Menu loop", LogCategory.System)
        {

            protected override async Task Run(CancellationToken stoppingToken)
            {
                await Task.Yield();
                try
                {
                    const int tick_interval_second = 1000;
                    const int timeout_interval = 30 * 1000;

                    int lastMinute = DateTime.Now.Minute;
                    var sw = Stopwatch.StartNew();
                    long nextTick_second = tick_interval_second;
                    long nextTimeout = timeout_interval;
                    DisplayMenuPage newPage;

                    currentPage.Update();

                    while (!stoppingToken.IsCancellationRequested)
                    {
                        EncoderEventType ev;
                        lock (_lock)
                        {
                            if (!encoderEvents.TryDequeue(out ev))
                                ev = EncoderEventType.None;
                        }
                        switch (ev)
                        {
                            case EncoderEventType.Right:
                                nextTimeout = sw.ElapsedMilliseconds + timeout_interval;
                                ProcessAction(currentPage.EncoderRight(out newPage), newPage);
                                break;
                            case EncoderEventType.Left:
                                nextTimeout = sw.ElapsedMilliseconds + timeout_interval;
                                ProcessAction(currentPage.EncoderLeft(out newPage), newPage);
                                break;
                            case EncoderEventType.ButtonDown:
                                nextTimeout = sw.ElapsedMilliseconds + timeout_interval;
                                ProcessAction(currentPage.EncoderButtonDown(out newPage), newPage);
                                break;
                            case EncoderEventType.ButtonUp:
                                nextTimeout = sw.ElapsedMilliseconds + timeout_interval;
                                ProcessAction(currentPage.EncoderButtonUp(out newPage), newPage);
                                break;
                            default:
                                // Wait for an encoder event or until the next second elapses, whatever happens first.
                                var sleepTime = (int)Math.Max(1000, nextTick_second - sw.ElapsedMilliseconds);
                                WaitHandle.WaitAny([encoderEvent, stoppingToken.WaitHandle], sleepTime);
                                break;
                        }
                        if (sw.ElapsedMilliseconds >= nextTick_second)
                        {
                            nextTick_second += tick_interval_second;
                            currentPage.TickSecond();
                        }
                        if (DateTime.Now.Minute != lastMinute)
                        {
                            lastMinute = DateTime.Now.Minute;
                            currentPage.TickMinute();
                        }
                        if ((pageIndex != 0 || pageStack.Count > 0) && sw.ElapsedMilliseconds > nextTimeout)
                        {
                            pageStack.Clear();
                            pageIndex = 0;
                            currentPage.Update();
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError($"Display menu loop error: {ex}");
                }
            }
        }
    }
}