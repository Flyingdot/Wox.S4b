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
        private readonly IContactSearch _contactSearch = new ContactSearch(new LyncClientFactory());
        private readonly ILync _lync = new Lync(new LyncClientFactory());

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
                        Title = SetTitle(c, parserResult),
                        SubTitle = "something",
                        Action = _ =>
                        {
                            OnContactSelection(c, parserResult);
                            return true;
                        }
                    }).ToList());
                }
            }

            return list;
        }

        private static string SetTitle(Contact contact, QueryParserResult parserResult)
        {
            string title = $"{contact.SafeGetContactInformation(ContactInformationType.FirstName)} {contact.SafeGetContactInformation(ContactInformationType.LastName)}";
            if (!string.IsNullOrWhiteSpace(parserResult.Message))
            {
                string msg = parserResult.Message.Length > 5
                    ? parserResult.Message.Substring(0, 5) + "..."
                    : parserResult.Message;
                title = $"Send '{msg}' to {title}";
            }

            return title;
        }

        private void OnContactSelection(Contact c, QueryParserResult parserResult)
        {
            if (!string.IsNullOrWhiteSpace(parserResult.Message))
            {
                _lync.SendMessage(c, parserResult.Message);
            }
            else
            {
                _lync.StartConversation(c);
            }
        }

        public void Init(PluginInitContext context)
        {
        }
    }
}
