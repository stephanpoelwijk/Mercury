namespace CodeRepublic.Tools.Mercury.Services
{
    using System;
    using System.Collections.Generic;
    using System.Net.Mail;
    
    /// <summary>
    /// Defines methods to retrieve unread mail messages
    /// </summary>
    public interface IMailService : IDisposable
    {
        /// <summary>
        /// Gets a value indicating whether the application is connected to the mail server
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Connects to a mail server
        /// </summary>
        /// <param name="hostName">Name of the mailserver</param>
        /// <param name="userName">Username that will be used to connect to the mailserver</param>
        /// <param name="password">Password that will be used to connect to the mailserver</param>
        /// <param name="ssl">Value indicating whether to connect through SSL</param>
        /// <param name="port">Mail server port</param>
        void Connect(string hostName, string userName, string password, bool ssl = false, int port = 143);

        /// <summary>
        /// Disconnects from the mail server
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Gets a list of unread messages
        /// </summary>
        /// <param name="markAsSeen">Value indicating whether to mark the messages as seen after retrieving them</param>
        /// <returns>List of messages from the mailserver or an empty list if there were no unread messages</returns>
        List<MailMessage> GetUnreadMessages(bool markAsSeen = true);
    }
}
