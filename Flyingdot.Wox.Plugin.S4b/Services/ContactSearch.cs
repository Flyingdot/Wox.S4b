using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Lync.Model;

namespace Flyingdot.Wox.Plugin.S4b.Services
{
    public class ContactSearch : IContactSearch
    {
        private static IList<SearchProviders> _activeSearchProviders;
        private readonly ILyncClientFactory _lyncClientFactory;

        public ContactSearch(ILyncClientFactory lyncClientFactory)
        {
            _lyncClientFactory = lyncClientFactory;
        }

        public IEnumerable<Contact> Search(string searchName, uint numResults)
        {
            if (_activeSearchProviders == null)
            {
                Init();
            }

            List<Contact> results = new List<Contact>();

            try
            {
                LyncClient lyncClient = _lyncClientFactory.GetInstance();

                if (lyncClient.State == ClientState.SignedIn)
                {
                    SearchFields searchFields = lyncClient.ContactManager.GetSearchFields();
                    object[] asyncState = { lyncClient.ContactManager, searchName };
                    foreach (var myActiveSearchProvider in _activeSearchProviders)
                    {
                        IAsyncResult result = lyncClient.ContactManager.BeginSearch(searchName
                            , myActiveSearchProvider
                            , searchFields
                            , SearchOptions.Default
                            , numResults
                            , ar => { }
                            , asyncState);

                        SearchResults searchResults = lyncClient.ContactManager.EndSearch(result);
                        results.AddRange(searchResults.Contacts);
                    }
                }
            }

            catch (Exception ex) { Debug.WriteLine($"Error: {ex.Message}"); }

            return results;
        }

        private void Init()
        {
            _activeSearchProviders = new List<SearchProviders>();
            LoadSearchProviders();
        }

        private void LoadSearchProviders()
        {
            LyncClient lyncClient = _lyncClientFactory.GetInstance();

            if (lyncClient.SignInConfiguration.SignedInFromIntranet)
            {
                if (lyncClient.ContactManager.GetSearchProviderStatus(SearchProviders.ExchangeService) == SearchProviderStatusType.SyncSucceeded)
                {
                    _activeSearchProviders.Add(SearchProviders.ExchangeService);
                }
                if (lyncClient.ContactManager.GetSearchProviderStatus(SearchProviders.Expert) == SearchProviderStatusType.SyncSucceeded)
                {
                    _activeSearchProviders.Add(SearchProviders.Expert);
                }
                if (lyncClient.ContactManager.GetSearchProviderStatus(SearchProviders.GlobalAddressList) == SearchProviderStatusType.SyncSucceeded)
                {
                    _activeSearchProviders.Add(SearchProviders.GlobalAddressList);
                }
            }
            else
            {
                if (lyncClient.ContactManager.GetSearchProviderStatus(SearchProviders.ExchangeService) == SearchProviderStatusType.SyncSucceededForExternalOnly)
                {
                    _activeSearchProviders.Add(SearchProviders.ExchangeService);
                }
                if (lyncClient.ContactManager.GetSearchProviderStatus(SearchProviders.Expert) == SearchProviderStatusType.SyncSucceededForExternalOnly)
                {
                    _activeSearchProviders.Add(SearchProviders.Expert);
                }
                if (lyncClient.ContactManager.GetSearchProviderStatus(SearchProviders.GlobalAddressList) == SearchProviderStatusType.SyncSucceededForExternalOnly)
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