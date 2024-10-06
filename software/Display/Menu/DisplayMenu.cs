using cog1.Hardware;
using System.Collections.Generic;

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

    public static partial class DisplayMenu
    {
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

    }
}
