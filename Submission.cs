using System;
using System.Xml.Schema;
using System.Xml;
using Newtonsoft.Json;
using System.IO;
using System.Net;

namespace ConsoleApp1
{
    public class Submission
    {
        public static string xmlURL = "https://aj1230984567-sudo.github.io/cse445_assignment4_1224714829/NationalParks.xml";
        public static string xmlErrorURL = "https://aj1230984567-sudo.github.io/cse445_assignment4_1224714829/NationalParksErrors.xml";
        public static string xsdURL = "https://aj1230984567-sudo.github.io/cse445_assignment4_1224714829/NationalParks.xsd";

        public static void Main(string[] args)
        {
            // CRITICAL: Force TLS 1.2 so WebClient can successfully download from GitHub Pages
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string result = Verification(xmlURL, xsdURL);
            Console.WriteLine(result);

            result = Verification(xmlErrorURL, xsdURL);
            Console.WriteLine(result);

            result = Xml2Json(xmlURL);
            Console.WriteLine(result);
        }

        // Q2.1
        public static string Verification(string xmlUrl, string xsdUrl)
        {
            try
            {
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.ValidationType = ValidationType.Schema;

                string xsdContent = DownloadContent(xsdUrl);
                using (StringReader xsdSr = new StringReader(xsdContent))
                {
                    using (XmlReader xsdReader = XmlReader.Create(xsdSr))
                    {
                        settings.Schemas.Add(null, xsdReader);
                    }
                }

                string errorMessage = "";
                settings.ValidationEventHandler += (sender, e) =>
                {
                    errorMessage += e.Message + "\n";
                };

                string xmlContent = DownloadContent(xmlUrl);
                using (StringReader xmlSr = new StringReader(xmlContent))
                {
                    using (XmlReader reader = XmlReader.Create(xmlSr, settings))
                    {
                        while (reader.Read()) { }
                    }
                }

                if (string.IsNullOrEmpty(errorMessage))
                {
                    return "No errors are found";
                }
                else
                {
                    return errorMessage.TrimEnd('\n');
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        // Q2.2
        public static string Xml2Json(string xmlUrl)
        {
            try
            {
                string xmlContent = DownloadContent(xmlUrl);
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xmlContent);

                string jsonText = JsonConvert.SerializeXmlNode(doc, Newtonsoft.Json.Formatting.Indented);
                return jsonText;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        // Helper method to download content from URL
        private static string DownloadContent(string url)
        {
            using (WebClient client = new WebClient())
            {
                return client.DownloadString(url);
            }
        }
    }
}
