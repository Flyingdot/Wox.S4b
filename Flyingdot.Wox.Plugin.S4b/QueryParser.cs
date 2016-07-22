namespace Flyingdot.Wox.Plugin.S4b
{
    public class QueryParser
    {
        public QueryParserResult Parse(string query)
        {
            string[] splitted = query.Split('"');
            QueryParserResult parserResult = new QueryParserResult
            {
                Search = splitted.Length > 0 ? splitted[0] : string.Empty,
                Message = splitted.Length > 1 ? splitted[1] : string.Empty
            };

            return parserResult;
        }
    }
}