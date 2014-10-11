namespace CodeRepublic.Tools.Mercury.Services
{
    using System.Collections.Generic;
    using System.Net.Mail;
    using S22.Imap;

    /// <summary>
    /// Service to retrieve unread mail messages through IMAP
    /// </summary>
    public class MailService : IMailService
    {
        /// <summary>
        /// IMAP client to retrieve the mail messages from
        /// </summary>
        private ImapClient client;

        public MailService()
        {
            client = null;
        }

        /// <inheritdoc/>
        public bool IsConnected { get; private set; }

        /// <inheritdoc/>
        public void Connect(string hostName, string userName, string password, bool ssl = false, int port = 143)
        {
            client = new ImapClient(hostName, port, userName, password, AuthMethod.Login, ssl );
            IsConnected = true;
        }

        /// <inheritdoc/>
        public void Disconnect()
        {
            if (!IsConnected || client == null)
            {
                return;
            }

            client.Logout();
            IsConnected = false;
        }

        /// <inheritdoc/>
        public List<MailMessage> GetUnreadMessages(bool markAsSeen = true)
        {
            var uids = client.Search(SearchCondition.Unseen());
            var messages = new List<MailMessage>(client.GetMessages(uids, markAsSeen));
            return messages;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (client != null)
            {
                client.Dispose();
            }
        }
    }
}
