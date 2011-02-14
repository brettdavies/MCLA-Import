using System;
using System.Text;
using System.Net;
using System.Xml;
using System.Xml.Schema;
using System.IO;

namespace MCLAImport
{
    public partial class Service1
    {
        public static void Main(string[] args)
        {
            //string url = "http://api.mcla.us/request/?api_key=f62001122589294193682bb5ac6897c7&version=1.1&method=game&team=kennesaw_state";
            //string url = "http://api.mcla.us/request/?api_key=f62001122589294193682bb5ac6897c7&version=1.1&method=roster&team=kennesaw_state";
            //string url = "http://api.mcla.us/request/?api_key=f62001122589294193682bb5ac6897c7&version=1.1&method=game&start=02/12/2011&end=02/14/2011&team=kennesaw_state";
            //string url = "http://mcla.us/scores/games/6353/";
            //HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            //HttpWebResponse httpResponse = (HttpWebResponse)webRequest.GetResponse();

            XmlDocument responseXML = new XmlDocument();
            //responseXML.Load(httpResponse.GetResponseStream());
            responseXML.Load(@"C:\Users\Brett\Documents\Visual Studio 2010\Projects\MCLAImport\xml\Roster.xml");

            //StringBuilder sb = new StringBuilder(responseXML.InnerXml);

            roster players = roster.Deserialize(responseXML.InnerXml);

            responseXML.Load(@"C:\Users\Brett\Documents\Visual Studio 2010\Projects\MCLAImport\xml\Schedule.xml");

            schedule sch = schedule.Deserialize(responseXML.InnerXml);


        }
    }
}