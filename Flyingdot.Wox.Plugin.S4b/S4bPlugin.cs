using System.Collections.Generic;
using System.Linq;
using Flyingdot.Wox.Plugin.S4b.Services;
using Microsoft.Lync.Model;
using Microsoft.Lync.Model.Extensibility;
using Wox.Plugin;

namespace Flyingdot.Wox.Plugin.S4b
{
    public class S4bPlugin : IPlugin
    {
        private readonly IContactSearch _contactSearch = new ContactSearch();

        public List<Result> Query(Query query)
        {
            var list = new List<Result>();

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                IEnumerable<Contact> searchResults = _contactSearch.Search(query.Search, 100);
                if (searchResults != null)
                {
                    list.AddRange(searchResults.Select(c => new Result
                    {
                        Title = $"{c.GetContactInformation(ContactInformationType.FirstName)} {c.GetContactInformation(ContactInformationType.LastName)}",
                        SubTitle = "something",
                        Action = contact =>
                        {
                            StartConversation(c);
                            return true;
                        }
                    }).ToList());
                }
            }

            return list;
        }

        private void StartConversation(Contact contact)
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

        public void Init(PluginInitContext context)
        {
        }
    }
}
