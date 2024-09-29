using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using cog1.Hardware;

namespace cog1.Display.Menu
{
    public enum DisplayMenuAction
    {
        None,
        NextPage,
        PreviousPage,
        Push,
        Pop
    }

    public static class DisplayMenu
    {
        private static bool initialized = false;
        private static object _lock = new();
        private static int pageIndex = 0;
        private static Stack<DisplayStackItem> pageStack = new();
        private static Queue<EncoderEventType> encoderEvents = new();

        private static List<DisplayMenuPage> pages = new()
        {
            new DisplayMenuPage_ScreenSaver(),
            new DisplayMenuPage_Home(),
            new DisplayMenuPage_DigitalIO(),
            new DisplayMenuPage_AnalogIN(),
            new DisplayMenuPage_WiFi(),
            new DisplayMenuPage_System(),
        };

        private class DisplayStackItem
        {
            public DisplayMenuPage page;
            public int previousPageIndex;
        }

        private static DisplayMenuPage currentPage
        {
            get
            {
                if (pageStack.Count == 0)
                    return pages[pageIndex];
                return pageStack.Peek().page;
            }
        }

        public static void Init()
        {
            if (initialized)
                return;
            Task.Run(() => MenuLoop());
            initialized = true;
        }

        public static void Deinit()
        {
            initialized = false;
        }

        public static void EncoderEvent(EncoderEventType eventType)
        {
            const int EVENT_QUEUE_LIMIT = 2;

            switch (eventType)
            {

                case EncoderEventType.Right:
                    lock (_lock)
                    {
                        if (encoderEvents.Count <= EVENT_QUEUE_LIMIT)
                            encoderEvents.Enqueue(EncoderEventType.Right);
                    }
                    break;
                case EncoderEventType.Left:
                    lock (_lock)
                    {
                        if (encoderEvents.Count <= EVENT_QUEUE_LIMIT)
                            encoderEvents.Enqueue(EncoderEventType.Left);
                    }
                    break;
                case EncoderEventType.ButtonDown:
                    lock (_lock)
                    {
                        if (encoderEvents.Count <= EVENT_QUEUE_LIMIT)
                            encoderEvents.Enqueue(EncoderEventType.ButtonDown);
                    }
                    break;
                case EncoderEventType.ButtonUp:
                    lock (_lock)
                    {
                        if (encoderEvents.Count <= EVENT_QUEUE_LIMIT)
                            encoderEvents.Enqueue(EncoderEventType.ButtonUp);
                    }
                    break;
            }
        }

        private static void ProcessAction(DisplayMenuAction action, DisplayMenuPage newPage)
        {
            switch (action)
            {
                case DisplayMenuAction.NextPage:
                    if (pageStack.Count == 0)
                    {
                        pageIndex++;
                        if (pageIndex >= pages.Count)
                            pageIndex = 1;
                        currentPage.Update();
                    }
                    break;
                case DisplayMenuAction.PreviousPage:
                    if (pageStack.Count == 0)
                    {
                        pageIndex--;
                        if (pageIndex < 1)
                            pageIndex = pages.Count - 1;
                        currentPage.Update();
                    }
                    break;
                case DisplayMenuAction.Push:
                    pageStack.Push(new DisplayStackItem()
                    {
                        previousPageIndex = pageIndex,
                        page = newPage
                    });
                    currentPage.Update();
                    break;
                case DisplayMenuAction.Pop:
                    if (pageStack.Count > 0)
                    {
                        var item = pageStack.Pop();
                        pageIndex = item.previousPageIndex;
                        currentPage.Update();
                    }
                    break;
            }
        }

        private static void MenuLoop()
        {
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

                for (; ; )
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
                            Thread.Sleep(100);
                            break;
                    }
                    if (sw.ElapsedMilliseconds > nextTick_second)
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
                Console.WriteLine("Unhandled error in DisplayMenu.MenuLoop(): " + ex.ToString());
            }
        }

    }
}
