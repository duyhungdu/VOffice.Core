using Aspose.Words;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

public partial class Util
{
    public static string DetecVowel(string keyword)
    {
        if (string.IsNullOrEmpty(keyword))
        {
            keyword = "";
        }
        keyword = keyword.ToLower();
        char[] charArr = keyword.ToCharArray();
        keyword = "";
        foreach (char c in charArr)
        {
            string tmp = "";
            tmp = c.ToString();

            if (c == 'a')
            {
                tmp = "[aáàạảãâấầậẩẫăắằặẳẵ]";
            }
            if (c == 'e')
            {
                tmp = "[eéèẹẻẽêếềệểễ]";
            }
            if (c == 'o')
            {
                tmp = "[oóòọỏõôốồộổỗơớờợởỡ]";
            }
            if (c == 'i')
            {
                tmp = "[iíìịỉĩ]";
            }
            if (c == 'u')
            {
                tmp = "[uúùụủũưứừựửữ]";
            }
            if (c == 'd')
            {
                tmp = "[dđ]";
            }
            if (c == 'y')
            {
                tmp = "[ýyỳỷỹ]";
            }
            keyword += tmp;
        }
        return keyword;
    }

    public static string GetDayOfWeek(int i)
    {
        switch (i)
        {
            case 0:
                return "Thứ hai";
            case 1:
                return "Thứ ba";
            case 2:
                return "Thứ tư";
            case 3:
                return "Thứ năm";
            case 4:
                return "Thứ sáu";
            case 5:
                return "Thứ bảy";
            case 6:
                return "Chủ nhật";
            default:
                return "";
        }
    }

    #region Generate Document
    public static Document GenerateDocument(string content, int padding = 10, bool landScape = false)
    {
        Document doc = new Document();

        #region Default Font
        //doc.NodeChangingCallback = new HandleNodeChanging_FontChanger();
        #endregion Default Font

        DocumentBuilder builder = new DocumentBuilder(doc);

        #region Phân trang
        doc.FirstSection.PageSetup.RestartPageNumbering = true;
        doc.FirstSection.PageSetup.PageStartingNumber = 1;
        builder.MoveToHeaderFooter(HeaderFooterType.FooterPrimary);
        builder.ParagraphFormat.Alignment = ParagraphAlignment.Right;

        builder.InsertField("Page");
        builder.MoveToDocumentEnd();
        #endregion Phân trang

        #region Paper Size, Margin, Orientation
        PageSetup pageSetup = builder.PageSetup;
        pageSetup.PaperSize = Aspose.Words.PaperSize.A4;
        pageSetup.TopMargin = ConvertUtil.MillimeterToPoint(padding);
        pageSetup.RightMargin = ConvertUtil.MillimeterToPoint(padding);
        pageSetup.BottomMargin = ConvertUtil.MillimeterToPoint(padding);
        pageSetup.LeftMargin = ConvertUtil.MillimeterToPoint(padding);
        pageSetup.Orientation = landScape ? Aspose.Words.Orientation.Landscape : Orientation.Portrait;
        #endregion

        #region Insert HTML
        builder.InsertHtml(content);
        #endregion
        return doc;
    }
    #endregion
    #region Create document
    public static string CreateDocument(string content, string preName, string tempName, int padding = 10, bool landScape = false)
    {
        //delete old file
        DirectoryInfo dirFile = new DirectoryInfo(HttpContext.Current.Server.MapPath("~/Temp"));
        foreach (FileInfo file in dirFile.GetFiles())
        {
            file.Delete();
        }
        LoadOptions loadOptions = new LoadOptions();
        loadOptions.LoadFormat = LoadFormat.Doc;
        Document wordDocument = Util.GenerateDocument(content, padding, landScape);
        string fileDocName = preName + "-" + Guid.NewGuid().ToString() + ".doc";
        wordDocument.Save(HttpContext.Current.Server.MapPath("~/Temp/" + tempName + ".doc"), SaveFormat.Doc);
        Document docFile = new Document(HttpContext.Current.Server.MapPath("~/Temp/" + tempName + ".doc"), loadOptions);
        docFile.Save(HttpContext.Current.Server.MapPath("~/Temp/" + fileDocName), SaveFormat.Doc);

        return "temp/" + fileDocName;
    }
    #endregion

    #region Generate Document
    public static Document GenerateContent(string content, int padding = 10)
    {
        Document doc = new Document();

        #region Default Font
        //doc.NodeChangingCallback = new HandleNodeChanging_FontChanger();
        #endregion Default Font

        DocumentBuilder builder = new DocumentBuilder(doc);

        #region Phân trang
        doc.FirstSection.PageSetup.RestartPageNumbering = true;
        doc.FirstSection.PageSetup.PageStartingNumber = 1;
        builder.MoveToHeaderFooter(HeaderFooterType.FooterPrimary);
        builder.ParagraphFormat.Alignment = ParagraphAlignment.Right;

        builder.InsertField("Page");
        builder.MoveToDocumentEnd();
        #endregion Phân trang

        #region Paper Size, Margin, Orientation
        PageSetup pageSetup = builder.PageSetup;
        pageSetup.PaperSize = Aspose.Words.PaperSize.A4;
        pageSetup.TopMargin = ConvertUtil.MillimeterToPoint(padding);
        pageSetup.RightMargin = ConvertUtil.MillimeterToPoint(padding);
        pageSetup.BottomMargin = ConvertUtil.MillimeterToPoint(padding);
        pageSetup.LeftMargin = ConvertUtil.MillimeterToPoint(padding);
        pageSetup.Orientation = Aspose.Words.Orientation.Landscape;
        #endregion

        #region Insert HTML
        builder.InsertHtml(content);
        #endregion
        return doc;
    }
    #endregion
    #region Create document
    public static string CreateContent(string content, string preName, string tempName, int padding = 10)
    {
        //delete old file
        DirectoryInfo dirFile = new DirectoryInfo(HttpContext.Current.Server.MapPath("~/Temp"));
        try
        {
            foreach (FileInfo file in dirFile.GetFiles())
            {
                file.Delete();
            }
        }
        catch
        {
        }
        LoadOptions loadOptions = new LoadOptions();
        loadOptions.LoadFormat = LoadFormat.Doc;
        Document wordDocument = Util.GenerateContent(content, padding);
        string fileDocName = preName + "-" + Guid.NewGuid().ToString() + ".doc";
        wordDocument.Save(HttpContext.Current.Server.MapPath("~/Temp/" + tempName + ".doc"), SaveFormat.Doc);
        Document docFile = new Document(HttpContext.Current.Server.MapPath("~/Temp/" + tempName + ".doc"), loadOptions);
        docFile.Save(HttpContext.Current.Server.MapPath("~/Temp/" + fileDocName), SaveFormat.Doc);
        return "temp/" + fileDocName;
    }
    #endregion
}
public static partial class DateTimeExtension
{
    public static DateTime GetFirstDayOfWeek(this DateTime date)
    {
        CultureInfo myCI = new CultureInfo("vi-VN");
        Calendar myCal = myCI.Calendar;
        var firstDayOfWeek = myCI.DateTimeFormat.FirstDayOfWeek;

        while (date.DayOfWeek != firstDayOfWeek)
        {
            date = date.AddDays(-1);
        }
        return date;
    }
}


public partial class EmailHelper
{

    public async static Task Send_Email(string mailFrom, string passmailFrom, string mailTo, string cc, string bcc, string subject, string body)
    {
        string _mailServer = "smtp.gmail.com";
        int _mailPort = 587;

        MailMessage mailMessage = new MailMessage();
        SmtpClient mailClient = new SmtpClient(_mailServer, _mailPort);
        mailClient.Timeout = 15000;
        mailClient.Credentials = new NetworkCredential(mailFrom, passmailFrom);
        mailClient.EnableSsl = true;

        mailMessage.IsBodyHtml = true;
        mailMessage.From = new MailAddress(mailFrom);
        mailMessage.Subject = subject;
        foreach (string s in bcc.Split(','))
        {
            if (!string.IsNullOrEmpty(s))
            {
                if (s != mailTo)
                {
                    MailAddress emailBcc = new MailAddress(s);
                    if (!mailMessage.Bcc.Contains(emailBcc))
                    {
                        mailMessage.Bcc.Add(emailBcc);
                    }
                }
            }
        }
        foreach (string s in cc.Split(','))
        {
            if (!string.IsNullOrEmpty(s))
            {
                if (s != mailTo)
                {
                    MailAddress emailCc = new MailAddress(s);
                    if (!mailMessage.CC.Contains(emailCc))
                    {
                        mailMessage.CC.Add(emailCc);
                    }
                }
            }
        }
        mailMessage.Body = body;
        mailMessage.To.Add(mailTo);
        try
        {
            await mailClient.SendMailAsync(mailMessage);
        }
        catch
        {

        }
    }
}
public class FCMNotificationCenter
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public Nullable<int> RecordId { get; set; }
    public Nullable<int> RelateRecordId { get; set; }
    public Nullable<int> SubRelateRecordId { get; set; }
    public Nullable<int> GroupId { get; set; }
    public string RecordNumber { get; set; }
    public int Type { get; set; }
    public string ReceivedUserId { get; set; }
    public bool HaveSeen { get; set; }
    public string CreatedBy { get; set; }
    public System.DateTime CreatedOn { get; set; }
    public string FullName { get; set; }
    public string Avatar { get; set; }
    public string DeviceId { get; set; }
}
public class FCMPushNotification
{
    public FCMPushNotification()
    {
        // TODO: Add constructor logic here
    }

    public bool Successful
    {
        get;
        set;
    }

    public string Response
    {
        get;
        set;
    }
    public Exception Error
    {
        get;
        set;
    }

    public FCMPushNotification SendNotification(FCMNotificationCenter notificationcenter, string _clientId)
    {
        FCMPushNotification result = new FCMPushNotification();
        try
        {
            result.Successful = true;
            result.Error = null;
            // var value = message;
            var requestUri = "https://fcm.googleapis.com/fcm/send";

            WebRequest webRequest = WebRequest.Create(requestUri);
            webRequest.Method = "POST";
            webRequest.Headers.Add(string.Format("Authorization: key={0}", "AAAAZVqqKsI:APA91bHD5xaB8tdiiUvRF0qTN-PAfp6FBKUCOl9H_rAdLD4j0ii29HMFY7H9XZgehDdE2KaiDneXvsQygJl3BFEUWsxgPHmyux0JwkSPYn2DGYUlMt_fNflu0_pbLNdMlbzLi76ALRLH"));
            webRequest.Headers.Add(string.Format("Sender: id={0}", "AIzaSyDMEeOtCPk7uIdl8V_t5XUkL78AfGqYXmM"));
            webRequest.ContentType = "application/json";

            var data = new
            {
                to = _clientId,
                data = new FCMNotificationCenter
                {
                    Title = notificationcenter.Title,
                    Content = notificationcenter.Content,
                    Type = notificationcenter.Type,
                    FullName = notificationcenter.FullName,
                    Avatar = notificationcenter.Avatar,
                    RecordId = notificationcenter.RecordId,
                    RelateRecordId = notificationcenter.RelateRecordId,
                    SubRelateRecordId = notificationcenter.SubRelateRecordId,
                    RecordNumber = notificationcenter.RecordNumber,
                    CreatedOn = DateTime.Now,
                    CreatedBy = notificationcenter.CreatedBy,
                    HaveSeen = true,
                    Id = notificationcenter.Id
                }
            };
            var camelCaseFormatter = new JsonSerializerSettings();
            camelCaseFormatter.ContractResolver = new CamelCasePropertyNamesContractResolver();
            var json = JsonConvert.SerializeObject(data, camelCaseFormatter);
            Byte[] byteArray = Encoding.UTF8.GetBytes(json);

            webRequest.ContentLength = byteArray.Length;
            using (Stream dataStream = webRequest.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);

                using (WebResponse webResponse = webRequest.GetResponse())
                {
                    using (Stream dataStreamResponse = webResponse.GetResponseStream())
                    {
                        using (StreamReader tReader = new StreamReader(dataStreamResponse))
                        {
                            String sResponseFromServer = tReader.ReadToEnd();
                            result.Response = sResponseFromServer;
                        }
                    }
                }
            }

        }
        catch (Exception ex)
        {
            result.Successful = false;
            result.Response = null;
            result.Error = ex;
        }
        return result;
    }
}


public enum NotificationCode
{
    Document,
    CreateTask,
    CreateOpinion,
    ReplyOpinion,
    CreateEvent
}
public static class TaskActivityAction
{
    public const string ADD = "ADD";
    public const string ASSIGN = "ASSIGN"; 
    public const string CLOSE = "CLOSE"; 
    public const string FORWARD = "FORWARD"; 
    public const string REOPEN = "REOPEN"; 
    public const string RESOLVE = "RESOLVE"; 
    public const string START = "START";
    public const string VIEW = "VIEW";
}