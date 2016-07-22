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

            string actual = sut.Parse(expected).Search;

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void Search_with_a_message_returns_message()
        {
            var expected = "says hello";
            var sut = new QueryParser();

            string actual = sut.Parse($"Bill Gates \"{expected}\"").Message;

            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}
