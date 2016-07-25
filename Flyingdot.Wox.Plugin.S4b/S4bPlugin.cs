using System.Collections.Generic;
using System.Diagnostics;
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
                QueryParserResult parserResult = new QueryParser().Parse(query.Search);

                IEnumerable<Contact> searchResults = _contactSearch.Search(parserResult.Search, 100);
                if (searchResults != null)
                {
                    list.AddRange(searchResults.Select(c => new Result
                    {
                        Title = $"{c.SafeGetContactInformation(ContactInformationType.FirstName)} {c.SafeGetContactInformation(ContactInformationType.LastName)}",
                        SubTitle = "something",
                        Action = contact =>
                        {
                            if (!string.IsNullOrWhiteSpace(parserResult.Message))
                            {
                                _lync.SendMessage(c, parserResult.Message);
                            }
                            else
                            {
                                _lync.StartConversation(c);
                            }

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
