using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Lync.Model;

namespace Flyingdot.Wox.Plugin.S4b.Services
{
    public class ContactSearch : IContactSearch
    {
        private readonly LyncClient _lyncClient;
        private static IList<SearchProviders> _activeSearchProviders;

        public ContactSearch()
        {
            _lyncClient = LyncClient.GetClient();
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

        private void Init()
        {
            _activeSearchProviders = new List<SearchProviders>();
            LoadSearchProviders();
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