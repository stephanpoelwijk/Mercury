namespace CodeRepublic.Tools.Mercury
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Net.Sockets;
    using System.Xml;
    using Models;
    using Services;
    using S22.Imap;

    /// <summary>
    /// Main application class
    /// </summary>
    class Program
    {
        /// <summary>
        /// Main method
        /// </summary>
        static void Main()
        {
            var mailPath = ConfigurationManager.AppSettings["MailPath"];

            if (string.IsNullOrEmpty(mailPath))
            {
                Console.Error.WriteLine("Missing mailPath configuration value");
                return;
            }

            bool overWriteExistingFiles;
            if (!Boolean.TryParse(ConfigurationManager.AppSettings["OverwriteExistingFiles"], out overWriteExistingFiles))
            {
                Console.Error.WriteLine("Invalid value for application configuration value OverwriteExistingFiles");
                return;
            }

            bool markMessagesAsSeen;
            if (!Boolean.TryParse(ConfigurationManager.AppSettings["MarkMessagesAsSeen"], out markMessagesAsSeen))
            {
                Console.Error.WriteLine("Invalid value for application configuration value MarkMessagesAsSeen");
                return;
            }

            var accountsFileName = ConfigurationManager.AppSettings["AccountConfigurationFileName"];
            if (string.IsNullOrEmpty(accountsFileName))
            {
                Console.Error.WriteLine("Missing AccountConfigurationFileName configuration value");
                return;
            }

            if (!File.Exists(accountsFileName))
            {
                Console.Error.WriteLine("The filename '{0}' does not exist", accountsFileName);
                return;
            }

            var accounts = new List<Account>();

            var document = new XmlDocument();
            document.Load(accountsFileName);

            var emailAddressNodes = document.SelectNodes("//emailaddress");
            if (emailAddressNodes == null)
            {
                Console.Error.WriteLine("No e-mail addresses in {0}", accountsFileName);
                return;
            }

            foreach (XmlNode node in emailAddressNodes)
            {
                if (node.Attributes == null)
                {
                    Console.Error.WriteLine("Missing attributes on node");
                    continue;
                }

                var account = new Account
                {
                    ServerName = node.Attributes["server"].Value,
                    UserName = node.Attributes["username"].Value,
                    Password = node.Attributes["password"].Value
                };

                accounts.Add(account);
            }

            foreach (var account in accounts)
            {
                Console.WriteLine("Checking {0} - {1}", account.ServerName, account.UserName);

                using (var mailService = new MailService())
                {
                    try
                    {
                        mailService.Connect(account.ServerName, account.UserName, account.Password);
                    }
                    catch (InvalidCredentialsException)
                    {
                        Console.Error.WriteLine("Invalid credentials for {0} - {1}", account.ServerName,
                            account.UserName);
                        continue;
                    }
                    catch (SocketException)
                    {
                        Console.Error.WriteLine("The server {0} could not be reached", account.ServerName);
                        continue;
                    }


                    foreach (var message in mailService.GetUnreadMessages(markMessagesAsSeen))
                    {
                        var mailTimestamp = message.Date() ?? DateTime.Now;

                        Console.WriteLine("Retrieving '{0}' from '{1}'", message.Subject, message.From.Address);

                        var tokens = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                        tokens["[%ACCOUNTNAME%]"] = account.ServerName;
                        tokens["[%USERNAME%]"] = account.UserName;
                        tokens["[%FROM%]"] = message.From.Address;
                        tokens["[%DAY%]"] = string.Format("{0:D2}", mailTimestamp.Day);
                        tokens["[%MONTH%]"] = string.Format("{0:D2}", mailTimestamp.Month);
                        tokens["[%YEAR%]"] = string.Format("{0:D4}", mailTimestamp.Year);

                        var targetMailPath = ReplaceTokens(mailPath, tokens);

                        if (!Directory.Exists(targetMailPath))
                        {
                            Directory.CreateDirectory(targetMailPath);
                        }

                        foreach (var attachment in message.Attachments)
                        {
                            Console.WriteLine("\t{0}", attachment.Name);

                            var targetFileName = string.Format("{0}\\{1}", targetMailPath, attachment.Name);

                            if (File.Exists(targetFileName))
                            {
                                if (overWriteExistingFiles)
                                {
                                    Console.WriteLine("Overwriting {0}", targetFileName);
                                }
                                else
                                {
                                    Console.WriteLine("Making filename unique so it is not overwritten");
                                    targetFileName = string.Format("{0}\\{1}_{2}{3}", targetMailPath,
                                        Path.GetFileNameWithoutExtension(targetFileName), Guid.NewGuid().ToString(),
                                        Path.GetExtension(targetFileName));
                                }
                            }

                            Console.WriteLine("Saving {0}", targetFileName);
                            using (var fs = File.Create(targetFileName))
                            {
                                attachment.ContentStream.CopyTo(fs);
                            }
                        }
                    }

                    mailService.Disconnect();
                }
            }
        }

        /// <summary>
        /// Replace tokens in a path string
        /// </summary>
        /// <param name="path">Path with the tokens</param>
        /// <param name="tokens">Token values</param>
        /// <returns>A path string where the tokens have been replaced</returns>
        private static string ReplaceTokens(string path, Dictionary<string, string> tokens)
        {
            return tokens.Keys.Aggregate(path, (current, key) => current.Replace(key, tokens[key]));
        }
    }
}
