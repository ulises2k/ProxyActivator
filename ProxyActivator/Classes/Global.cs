using System;

namespace ProxyActivator
{
    class Global
    {
        public const Int32 ServerID = 5;
        public const String APSaveFile = "accesspoints.xml";

        public static Version Version
        {
            get { return new Version("1.2.3"); }
        }
    }
}
