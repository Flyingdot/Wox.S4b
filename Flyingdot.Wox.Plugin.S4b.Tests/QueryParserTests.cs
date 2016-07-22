using NUnit.Framework;

namespace Flyingdot.Wox.Plugin.S4b.Tests
{
    [TestFixture]
    public class QueryParserTests
    {
        [Test]
        public void Search_with_name_only_returns_the_name()
        {
            string expected = "Bill Gates";
            var sut = new QueryParser();

            string actual = sut.Parse(expected)[0];

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void Search_with_a_message_returns_message()
        {
            var expected = "says hello";
            var sut = new QueryParser();

            string actual = sut.Parse($"Bill Gates \"{expected}\"")[1];

            Assert.That(actual, Is.EqualTo(expected));
        }
    }

    public class QueryParser
    {
        public string[] Parse(string query)
        {
            string[] result = new string[2];
            string[] splitted = query.Split('"');
            result[0] = splitted[0];
            result[1] = splitted.Length > 1 ? splitted[1] : string.Empty;
            return result;
        }
    }
}
