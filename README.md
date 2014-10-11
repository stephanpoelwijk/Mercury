Mercury
=======

Commandline tool to checks e-mails marked as unread over IMAP, downloads attachments for those e-mails to a local folder and optionally marks the messages as read.

Account Configuration
---------------------
Accounts are configured through the accounts.xml file. Every e-mail addres that needs to be checked is in the following format:

    <emailaddress server="mailserver" username="username" password="password"/>

Server attribute contains the full server name. Credentials are specified in the username and password attributes.

Attachment File Path 
--------------------
The path where the attachments are saved can be set in the Mercury.app.config file. The path can contain multiple tokens:

    [%SERVERNAME%] - Name of the server from accounts.xml
    [%USERNAME%] - Username of the account from accounts.xml
    [%DAY%] - Double digit day the message was sent
    [%MONTH%] - Double digit month the message was sent
    [%YEAR%] - Full four digit year the message was sent
    [%FROM%] - Sender's email address

If the server does not return a date the message has been sent, the current system time is used.

Example File Path
-----------------
If the path `c:\tmp\[%SERVERNAME%]\[%YEAR%]-[%MONTH%]-[%DAY%]` has been configured in the .config file, the file `cutecatpicture.jpg` attached to a mail coming in at 2014-11-18 through the server mail.whatever.com will be stored in the path `c:\tmp\mail.whatever.com\2014-18-11\cutecatpicture.jpg`.


Misc Configuration Options
--------------------------
The .config file contains some other configuration options, like:

`OverwriteExistingFiles` - If two files with the same name are sent to a mail address, the second file will overwrite the first file.

`MarkMessagesAsSeen` - When the attachment has been downloaded or a mail does not have an attachment but has been examined, the message is marked as read and will not be processed further.

`AccountConfigurationFileName` - Name of the file containing the account information entries.




