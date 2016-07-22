using System.Collections.Generic;
using Microsoft.Lync.Model;

namespace Flyingdot.Wox.Plugin.S4b.Services
{
    public interface IContactSearch
    {
        IEnumerable<Contact> Search(string searchName, uint numResults);
    }
}