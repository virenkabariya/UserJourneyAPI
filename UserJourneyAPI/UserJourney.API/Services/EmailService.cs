namespace UserJourney.API.Services
{
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using MailKit.Net.Smtp;
    using MailKit.Security;
    using MimeKit;
    using Microsoft.Extensions.Options;
    using System.Net.Mail;
    using UserJourney.API.Code;
    using UserJourney.API.Contracts;

    public class EmailService : IEmailService
    {
        private EMailSettings _emailSettings;
        public EmailService(IOptions<EMailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task<bool> SendEmailAsync(List<string> to, List<string> cc, string subject, string templateName, Dictionary<string, string> values, byte[] attachment = null, string attachmentName = null)
        {
            try
            {
                var emailMessage = new MimeMessage();

                string templatePath = _emailSettings.EmailTemplatePath;
                string masterTemplateFullPath = Path.Combine(templatePath, "MasterEmailTemplate.html");
                StringBuilder masterTemplateContent = new StringBuilder();

                string filePath = Path.Combine(templatePath, templateName);
                string templateFullPath = Path.Combine(templatePath, templateName);
                StringBuilder Content = new StringBuilder();
                cc = cc ?? new List<string>();
                var builder = new BodyBuilder();


                using (var stream = new FileStream(masterTemplateFullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string lineStr;
                        while ((lineStr = reader.ReadLine()) != null)
                        {
                            masterTemplateContent.Append(lineStr);
                        }

                    }
                }

                using (var stream = new FileStream(templateFullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string lineStr;
                        while ((lineStr = reader.ReadLine()) != null)
                        {
                            Content.Append(lineStr);
                        }
                    }
                }

                masterTemplateContent.Replace("[@MainContent]", Content.ToString());
                masterTemplateContent.Replace("[@Subject]", subject.ToString());

                foreach (var entry in values)
                    masterTemplateContent.Replace(entry.Key, entry.Value);
                builder.HtmlBody = masterTemplateContent.ToString();
                if (to.Any())
                {
                    foreach (var entry in to)
                    {
                        if (ValidateEmail(entry))
                        {
                            emailMessage.To.Add(new MailboxAddress(string.Empty, entry.Trim()));
                        }
                    }
                }

                if (cc.Any())
                {
                    foreach (var entry in cc)
                    {
                        if (ValidateEmail(entry))
                        {
                            emailMessage.Cc.Add(new MailboxAddress(string.Empty, entry.Trim()));
                        }
                    }
                }

                if (attachment != null)
                    builder.Attachments.Add(attachmentName, attachment);

                emailMessage.Subject = subject;
                emailMessage.Body = builder.ToMessageBody();

                emailMessage.From.Add(new MailboxAddress("UserJourney", _emailSettings.From));

                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    if (!_emailSettings.SSLEnabled)
                        client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                    client.Connect(_emailSettings.Host, _emailSettings.Port, SecureSocketOptions.Auto);

                    if (!_emailSettings.BypassAuthentication)
                    {
                        client.Authenticate(_emailSettings.UserName, _emailSettings.Password);
                    }

                    client.Send(emailMessage);
                    client.Disconnect(true);
                }

                return true;
            }
            catch (System.Exception ex)
            {
                return false;
            }
        }

        private static bool ValidateEmail(string emails)
        {
            bool isValid = true;

            if (!string.IsNullOrEmpty(emails))
            {
                emails = emails.Replace(" ", string.Empty);
                string[] emailList = null;
                try
                {
                    emails = emails.Replace(';', ',');
                    emailList = emails.Split(',');
                }
                catch
                {
                    isValid = false;
                }

                if (emailList != null && emailList.Count() > 0)
                {
                    foreach (string email in emailList)
                    {
                        if (!IsEmail(email))
                        {
                            isValid = false;
                        }
                    }
                }
                else
                {
                    isValid = false;
                }
            }
            else
            {
                isValid = false;
            }

            return isValid;
        }

        private static bool IsEmail(string email)
        {
            string strRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                  @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                  @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
            Regex re = new Regex(strRegex);
            if (re.IsMatch(email))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }

}
