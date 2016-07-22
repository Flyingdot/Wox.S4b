using System;
using Microsoft.Lync.Model;

namespace Flyingdot.Wox.Plugin.S4b.Extensions
{
    public static class ContactExtension
    {
        public static string SafeGetContactInformation(this Contact contact, ContactInformationType contactInformationType, string defaultValue = "")
        {
            try
            {
                return contact.GetContactInformation(contactInformationType) as string;
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }
    }
}