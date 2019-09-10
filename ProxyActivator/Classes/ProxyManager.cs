using System;
using System.Drawing;

namespace ProxyActivator
{
    class ProxyManager
    {
        private static ProxyManager instance = null;
        public static ProxyManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ProxyManager();
                }
                return instance;
            }
        }


        public string ConfiguredProxyName = "";

        public void ProxyToggleAll(bool active, string ip = "", int port = 0, string ProxyName = "", string exceptions = "")
        {
            this.ProxyToggleSystem(active, ip, port, exceptions);
        }

        public State ProxyStateSystem = new State("No information.", Color.Black);
        public void ProxyToggleSystem(Boolean enable, string ip, int port, string exceptions)
        {
            if (enable ==  true)
            {
                WlanManager.Instance.ActivateProxy(ip, port, enable, exceptions);
                ProxyStateSystem = new State("Enabled", Color.Green);
                
            }
            else
            {
                WlanManager.Instance.DeactivateProxy();
                ProxyStateSystem = new State("Disabled", Color.Orange);
            }
        }

                     
    }
}
