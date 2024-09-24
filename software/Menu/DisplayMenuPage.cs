using Microsoft.VisualBasic;

namespace cog1app
{
    public abstract class DisplayMenuPage
    {
        public abstract void Update();

        public virtual void TickSecond()
        {
        }

        public virtual void TickMinute()
        {
        }

        public virtual DisplayMenuAction EncoderRight(out DisplayMenuPage newPage)
        {
            newPage = null;
            return DisplayMenuAction.NextPage;
        }

        public virtual DisplayMenuAction EncoderLeft(out DisplayMenuPage newPage)
        {
            newPage = null;
            return DisplayMenuAction.PreviousPage;
        }

        public virtual DisplayMenuAction EncoderButtonDown(out DisplayMenuPage newPage)
        {
            newPage = null;
            return DisplayMenuAction.None;
        }

        public virtual DisplayMenuAction EncoderButtonUp(out DisplayMenuPage newPage)
        {
            newPage = null;
            return DisplayMenuAction.None;
        }
    }
}
