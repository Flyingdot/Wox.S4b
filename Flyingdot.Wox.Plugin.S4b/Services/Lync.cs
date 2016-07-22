using System.Collections.Generic;
using Microsoft.Lync.Model;
using Microsoft.Lync.Model.Extensibility;

namespace Flyingdot.Wox.Plugin.S4b.Services
{
    public class Lync : ILync
    {
        public void StartConversation(Contact contact)
        {
            Automation lyncAutomation = LyncClient.GetAutomation();

            lyncAutomation.BeginStartConversation(
                AutomationModalities.InstantMessage,
                new[] { contact.Uri },
                new Dictionary<AutomationModalitySettings, object>
                {
                    {AutomationModalitySettings.SendFirstInstantMessageImmediately, false}
                },
                ar =>
                {
                    if (ar.IsCompleted)
                    {
                        ((Automation)ar.AsyncState).EndStartConversation(ar);
                    }
                },
                lyncAutomation);
        }
    }
}