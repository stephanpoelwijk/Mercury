namespace CodeRepublic.Tools.Mercury.Models
{
    /// <summary>
    /// Mail account information
    /// </summary>
    public class Account
    {
        /// <summary>
        /// Gets or sets the name of the mail server
        /// </summary>
        public string ServerName { get; set; }

        /// <summary>
        /// Gets or sets the username that is used to connect to the mail server
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the password that is used to connect to the mail server
        /// </summary>
        public string Password { get; set; }
    }
}
