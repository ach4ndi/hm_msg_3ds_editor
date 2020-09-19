using System;

namespace BokujoMessage4
{
    public enum GameSeries
    {
        ANBJP,ANBUS,ANBEU,
        SOSJP,SOSUS,SOSEU,
        SOSKOR,
        SOS2JP
    }

    public struct ppmsg4
    {
        public DateTime Date;
        public string Author;
        public pmsg4[] Content;
    }

    public struct pmsg4
    {
        public string VardId;
        public string Text;
    }
}
