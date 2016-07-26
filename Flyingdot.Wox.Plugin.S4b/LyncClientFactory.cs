using System.Diagnostics;
using Microsoft.Lync.Model;

namespace Flyingdot.Wox.Plugin.S4b
{
    public class LyncClientFactory : ILyncClientFactory
    {
        private static LyncClient _instance;

        public LyncClient GetInstance()
        {
            Debug.WriteLine(_instance?.State.ToString() ?? "null");
            if (_instance == null || _instance.State != ClientState.SignedIn)
            {
                _instance = LyncClient.GetClient();
            }

            return _instance;
        }
    }
}