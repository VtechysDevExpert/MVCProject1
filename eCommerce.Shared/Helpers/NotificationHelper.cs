using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace eCommerce.Shared.Helpers
{
    public static class NotificationHelper
    {
        public static void SendSms(string MobileNo, string Msgbody, string ProductName)
        {
            //  string Msgbody = string.Empty;
            try
            {

                HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create("http://120.138.8.100/api/mt/SendSMS?user=convergence&password=abc@123&senderid=BPOCON&channel=Trans&DCS=0&flashsms=0&number=" + MobileNo + "&text=" + Msgbody + "&route=11");
                HttpWebResponse myResp = (HttpWebResponse)myReq.GetResponse();
                System.IO.StreamReader respStreamReader = new System.IO.StreamReader(myResp.GetResponseStream());
                string responseString = respStreamReader.ReadToEnd();
                respStreamReader.Close();
            }
            catch (Exception eX)
            {

                throw eX;   /// sms Failure
            }
        }

        public static void SendMail(string mailId, string CandiDate,string msg)
        {
            DataTable dtx = new DataTable();


            string rgem = mailId;
            string head = ConfigurationsHelper.ApplicationName;
            MailAddress fromSrc = new MailAddress("sender@gmail.com", head);
            MailAddress toDst = new MailAddress(rgem, "");
            using (MailMessage mm = new MailMessage(fromSrc, toDst))
            {
                try
                {
                    mm.Subject = "Order Status ChandiBhandar";
                    string body = "Dear  " + CandiDate + "";
                    body += "<br />Greetings from " + head + "";
                    body += "<br /> " +
                        msg+"";
                    body += "<br /><br />Yours sincerely,";
                    body += "<br />" + head + "";
                    body = body.Replace("/sndMail", "");
                    mm.Body = body;
                    mm.IsBodyHtml = true;
                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = "smtp.gmail.com";
                    // smtp.Host = "smtp.office365.com";
                    smtp.EnableSsl = true;
                    NetworkCredential NetworkCred = new NetworkCredential("sender@gmail.com", "p@ssword");
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = NetworkCred;
                    smtp.Port = 587;
                    smtp.Send(mm);
                    mm.Dispose();
                    
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}
