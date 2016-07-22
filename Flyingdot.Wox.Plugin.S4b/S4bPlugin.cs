using System.Collections.Generic;
using System.Linq;
using Flyingdot.Wox.Plugin.S4b.Extensions;
using Flyingdot.Wox.Plugin.S4b.Services;
using Microsoft.Lync.Model;
using Wox.Plugin;

namespace Flyingdot.Wox.Plugin.S4b
{
    public class S4bPlugin : IPlugin
    {
        private readonly IContactSearch _contactSearch = new ContactSearch();
        private readonly ILync _lync = new Lync();

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
                        Title = $"{c.SafeGetContactInformation(ContactInformationType.FirstName)} {c.SafeGetContactInformation(ContactInformationType.LastName)}",
                        SubTitle = "something",
                        Action = contact =>
                        {
                            _lync.StartConversation(c);
                            return true;
                        }
                    }).ToList());
                }
            }

            return list;
        }

        public void Init(PluginInitContext context)
        {
        }
    }
}
