using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Lync.Model;
using Microsoft.Lync.Model.Conversation;
using Microsoft.Lync.Model.Extensibility;

namespace Flyingdot.Wox.Plugin.S4b.Services
{
    public class Lync : ILync
    {
        private readonly ILyncClientFactory _lyncClientFactory;

        public Lync(ILyncClientFactory lyncClientFactory)
        {
            _lyncClientFactory = lyncClientFactory;
        }

        public void StartConversation(Contact contact)
        {
            Automation lyncAutomation = LyncClient.GetAutomation();

            lyncAutomation.BeginStartConversation(
                AutomationModalities.InstantMessage,
                new[] { contact.Uri },
                new Dictionary<AutomationModalitySettings, object>
                {
                    {AutomationModalitySettings.SendFirstInstantMessageImmediately, false}
                },
                ar =>
                {
                    if (ar.IsCompleted)
                    {
                        ((Automation)ar.AsyncState).EndStartConversation(ar);
                    }
                },
                lyncAutomation);
        }

        public void SendMessage(Contact contact, string message)
        {
            LyncClient lyncClient = _lyncClientFactory.GetInstance();
            lyncClient.ConversationManager.ConversationAdded += (sender, args) =>
            {
                args.Conversation.AddParticipant(contact);
            };
            var conversation = lyncClient.ConversationManager.AddConversation();
            SendMessage(message, conversation);
        }

        private void SendMessage(string messageToSend, Conversation conversation)
        {
            try
            {
                IDictionary<InstantMessageContentType, string> textMessage = new Dictionary<InstantMessageContentType, string>();
                textMessage.Add(InstantMessageContentType.PlainText, messageToSend);
                if (((InstantMessageModality)conversation.Modalities[ModalityTypes.InstantMessage]).CanInvoke(ModalityAction.SendInstantMessage))
                {
                    ((InstantMessageModality)conversation.Modalities[ModalityTypes.InstantMessage]).BeginSendMessage(
                        textMessage
                        , SendMessageCallback
                        , conversation);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        private void SendMessageCallback(IAsyncResult ar)
        {
            var conversation = ar.AsyncState as Conversation;
            if (conversation == null) return;
            if (!ar.IsCompleted) return;

            ((InstantMessageModality)conversation.Modalities[ModalityTypes.InstantMessage]).BeginSetComposing(false, null, null);
            try
            {
                ((InstantMessageModality)conversation.Modalities[ModalityTypes.InstantMessage]).EndSendMessage(ar);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }
    }
}