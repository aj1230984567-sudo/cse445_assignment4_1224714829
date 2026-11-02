using System;
using System.Xml.Schema;
using System.Xml;
using Newtonsoft.Json;
using System.IO;

namespace ConsoleApp1
{
    public class Submission
    {
        public static string xmlURL = "https://aj1230984567-sudo.github.io/cse445_assignment4_1224714829/NationalParks.xml";
        public static string xmlErrorURL = "https://aj1230984567-sudo.github.io/cse445_assignment4_1224714829/NationalParksErrors.xml";
        public static string xsdURL = "https://aj1230984567-sudo.github.io/cse445_assignment4_1224714829/NationalParks.xsd";

        public static void Main(string[] args)
        {
            // Q3.1 verify valid XML
            string result = Verification(xmlURL, xsdURL);
            Console.WriteLine(result);

            // Q3.2 verify XML with errors
            result = Verification(xmlErrorURL, xsdURL);
            Console.WriteLine(result);

            // Q3.3 convert XML to JSON
            result = Xml2Json(xmlURL);
            Console.WriteLine(result);
        }

        // Q2.1 validation method
        public static string Verification(string xmlUrl, string xsdUrl)
        {
            try
            {
                // set up XmlReaderSettings for XSD validation
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.ValidationType = ValidationType.Schema;

                // download XSD schema content and load it
                string xsdContent = DownloadContent(xsdUrl);
                using (StringReader xsdSr = new StringReader(xsdContent))
                {
                    using (XmlReader xsdReader = XmlReader.Create(xsdSr))
                    {
                        settings.Schemas.Add(null, xsdReader);
                    }
                }

                string errorMessage = "";
                // handler to capture validation errors
                settings.ValidationEventHandler += (sender, e) =>
                {
                    errorMessage += e.Message + "\n";
                };

                // download XML content and read/validate
                string xmlContent = DownloadContent(xmlUrl);
                using (StringReader xmlSr = new StringReader(xmlContent))
                {
                    using (XmlReader reader = XmlReader.Create(xmlSr, settings))
                    {
                        while (reader.Read()) { } // read through whole document to trigger validation
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

        // Q2.2 XML to JSON conversion method
        public static string Xml2Json(string xmlUrl)
        {
            try
            {
                string xmlContent = DownloadContent(xmlUrl);
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xmlContent);

                // convert XmlDocument to JSON string format compatible with Newtonsoft.Json
                string jsonText = JsonConvert.SerializeXmlNode(doc, Newtonsoft.Json.Formatting.Indented);
                return jsonText;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        // helper method to download content from URL
        private static string DownloadContent(string url)
        {
            using (System.Net.WebClient client = new System.Net.WebClient())
            {
                return client.DownloadString(url);
            }
        }
    }
}
