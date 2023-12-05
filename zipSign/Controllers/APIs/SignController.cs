using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Http;
using System.Xml;
using System.Xml.Linq;

namespace zipSign.Controllers.APIs
{
    public class SignController : ApiController
    {

        [HttpPost]
        [Route("api/pdfsignature")]
        //public IHttpActionResult PDFSignature([FromBody] XElement xmlData)
        //{
        //    try
        //    {
        //        // Extract values from XML
        //        string name = xmlData.Element("Name")?.Value;
        //        string age = xmlData.Element("Age")?.Value;

        //        return Ok($"Name: {name}, Age: {age}");
        public IHttpActionResult PDFSignature(HttpRequestMessage request)
        {
            string xmlData = request.Content.ReadAsStringAsync().Result;
            int xmlDeclarationStart = xmlData.IndexOf("<?xml");
            int filepath = xmlData.IndexOf("<PathToXML");

            // If an XML declaration is found, extract the XML content from that point
            if (xmlDeclarationStart != -1)
            {
                xmlData = xmlData.Substring(xmlDeclarationStart);
            }
            if (xmlDeclarationStart != -1)
            {
                xmlData = xmlData.Substring(filepath);
            }

            // Load the XML document
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlData);

            // Specify the XML namespace manager if needed
            XmlNamespaceManager nsManager = new XmlNamespaceManager(xmlDoc.NameTable);
            nsManager.AddNamespace("ns", "http://www.w3.org/2000/09/xmldsig#");

            // Use XPath to select the specific tag content
            XmlNode pathToXMLNode = xmlDoc.SelectSingleNode("/xmlData/PathToXML");
            string pathToXML = pathToXMLNode.InnerText;

            // You can similarly extract other data using XPath
            XmlNode signatureValueNode = xmlDoc.SelectSingleNode("//ns:SignatureValue", nsManager);
            string signatureValue = signatureValueNode.InnerText;

            // Now you can use the extracted data
            return Ok("PathToXML: " + pathToXML + ", SignatureValue: " + signatureValue);
        }
    }
    
}

