using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Configuration;
using System.Net;
using ShopApplicationV.Utils;

namespace ShopApplicationV.Utils
{
    public class MailService
    {

        public MailService()
        {}

        public string Code;
        public string Email;
        public string Host;
        public string Pass; 

        public bool ValidateCode(string entered_code)
        {
            if (entered_code.Equals(Code))
            {
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = config.AppSettings.Settings;
                settings["p_key"].Value = EncryptService.Encrypt(Pass);
                settings["Email"].Value = Email;
                settings["Host"].Value = Host;
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(config.AppSettings.SectionInformation.Name);
                return true;
            }
            return false;
        }

        public async Task ValidateMessage(string authemail, string password, string host)
        {
                using (MailMessage mail = new MailMessage($"{authemail}", $"{authemail}"))
                {
                    using (SmtpClient client = new SmtpClient())
                    {
                        client.Port = 25;
                        client.DeliveryMethod = SmtpDeliveryMethod.Network;
                        client.Credentials = new NetworkCredential($"{authemail}", $"{password}");
                        client.Host = host;
                        client.EnableSsl = true;
                        mail.Subject = "Connection check";
                        Code = EncryptService.GeneratePassword();
                        Email = authemail;
                        Pass = password;
                        Host = host;
                        mail.Body = "Connection check code is: " + "\n" + Code;
                        await client.SendMailAsync(mail);
                    }
                }
        }

        public async static Task SendMess(string to,
            string Subject, string Body, bool attach_db = false)
        {
            string from = ConfigurationManager.AppSettings["email"];
            using (MailMessage mail = new MailMessage($"{from}", $"{to}"))
            {
                using (SmtpClient client = new SmtpClient()) {
                    client.Port = 25;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.Credentials = new NetworkCredential(from,
                        EncryptService.Decrypt(ConfigurationManager.AppSettings["p_key"]));
                    client.Host = ConfigurationManager.AppSettings["host"];
                    client.EnableSsl = true;
                    if (attach_db)
                    {
                        var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                        var connectionStringsSection = (ConnectionStringsSection)config.GetSection("connectionStrings");
                        var filePath = connectionStringsSection.ConnectionStrings["DefaultConnection"].ConnectionString;
                        mail.Attachments.Add(new Attachment(ConfigurationManager.AppSettings["filePath"]));
                    }
                    mail.Subject = Subject;
                    mail.Body = Body;
                    await client.SendMailAsync(mail);
                }
            }
        }
    }
}
