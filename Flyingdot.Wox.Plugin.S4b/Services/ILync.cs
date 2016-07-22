using Microsoft.Lync.Model;

namespace Flyingdot.Wox.Plugin.S4b.Services
{
    public interface ILync
    {
        void StartConversation(Contact contact);
        void SendMessage(Contact contact, string message);
    }
}