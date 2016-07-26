using Microsoft.Lync.Model;

namespace Flyingdot.Wox.Plugin.S4b
{
    public interface ILyncClientFactory
    {
        LyncClient GetInstance();
    }
}