using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Lync.Model;
using Microsoft.Lync.Model.Conversation;
using Microsoft.Lync.Model.Extensibility;
using Wox.Plugin;

namespace Flyingdot.Wox.Plugin.S4b
{
    public class S4bPlugin : IPlugin
    {
        private readonly LyncClient _lyncClient;
        private readonly IList<SearchProviders> _activeSearchProviders = new List<SearchProviders>();

        public S4bPlugin()
        {
            _lyncClient = LyncClient.GetClient();
        }

        public List<Result> Query(Query query)
        {
            var list = new List<Result>();

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                IEnumerable<Contact> searchResults = SearchForGroupOrContact(query.Search, 100);
                if (searchResults != null)
                {
                    list.AddRange(searchResults.Select(c => new Result
                    {
                        Title = c.Uri,
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

        public void Init(PluginInitContext context)
        {
            LoadSearchProviders();
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

        private IEnumerable<Contact> SearchForGroupOrContact(string searchName, uint numResults)
        {
            List<Contact> results = new List<Contact>();

            try
            {
                if (_lyncClient.State == ClientState.SignedIn)
                {
                    SearchFields searchFields = _lyncClient.ContactManager.GetSearchFields();
                    object[] asyncState = { _lyncClient.ContactManager, searchName };
                    foreach (var myActiveSearchProvider in _activeSearchProviders)
                    {
                        IAsyncResult result = _lyncClient.ContactManager.BeginSearch(searchName
                            , myActiveSearchProvider
                            , searchFields
                            , SearchOptions.Default
                            , numResults
                            , ar => { }
                            , asyncState);

                        SearchResults searchResults = _lyncClient.ContactManager.EndSearch(result);
                        results.AddRange(searchResults.Contacts);
                    }
                }
            }
            
            catch (Exception ex) { Debug.WriteLine($"Error: {ex.Message}"); }

            return results;
        }

        private void LoadSearchProviders()
        {
            if (_lyncClient.SignInConfiguration.SignedInFromIntranet)
            {
                if (_lyncClient.ContactManager.GetSearchProviderStatus(SearchProviders.ExchangeService) == SearchProviderStatusType.SyncSucceeded)
                {
                    _activeSearchProviders.Add(SearchProviders.ExchangeService);
                }
                if (_lyncClient.ContactManager.GetSearchProviderStatus(SearchProviders.Expert) == SearchProviderStatusType.SyncSucceeded)
                {
                    _activeSearchProviders.Add(SearchProviders.Expert);
                }
                if (_lyncClient.ContactManager.GetSearchProviderStatus(SearchProviders.GlobalAddressList) == SearchProviderStatusType.SyncSucceeded)
                {
                    _activeSearchProviders.Add(SearchProviders.GlobalAddressList);
                }
            }
            else
            {
                if (_lyncClient.ContactManager.GetSearchProviderStatus(SearchProviders.ExchangeService) == SearchProviderStatusType.SyncSucceededForExternalOnly)
                {
                    _activeSearchProviders.Add(SearchProviders.ExchangeService);
                }
                if (_lyncClient.ContactManager.GetSearchProviderStatus(SearchProviders.Expert) == SearchProviderStatusType.SyncSucceededForExternalOnly)
                {
                    _activeSearchProviders.Add(SearchProviders.Expert);
                }
                if (_lyncClient.ContactManager.GetSearchProviderStatus(SearchProviders.GlobalAddressList) == SearchProviderStatusType.SyncSucceededForExternalOnly)
                {
                    _activeSearchProviders.Add(SearchProviders.GlobalAddressList);
                }
            }

            _activeSearchProviders.Add(SearchProviders.Default);
            _activeSearchProviders.Add(SearchProviders.OtherContacts);
            _activeSearchProviders.Add(SearchProviders.PersonalContacts);
            _activeSearchProviders.Add(SearchProviders.WindowsAddressBook);
        }

    }
}
