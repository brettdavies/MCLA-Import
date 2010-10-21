using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Data.SqlClient;
using System.Net;
using System.IO;
using System.Threading;
using System.Xml;
using System.Web;

namespace MCLAImport
{
    public partial class Service1 : ServiceBase
    {
        private bool serviceStarted = false;
        private Thread tracktimes_wt;

        public Service1()
        {
            InitializeComponent();
        }

        static void Main(string[] args)
        {
            if ((args.Length == 1 && args[0] == "-c") || System.Diagnostics.Debugger.IsAttached)
            {
                Service1 svc = new Service1();
                svc.OnStart(args);
                System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
                return;
            }

            ServiceBase[] ServicesToRun;

            // More than one user Service may run within the same process. To add
            // another service to this process, change the following line to
            // create a second service object. For example,
            //
            //   ServicesToRun = new ServiceBase[] {new MainService(), new MySecondUserService()};
            //
            ServicesToRun = new ServiceBase[] { new Service1() };

            ServiceBase.Run(ServicesToRun);
        }


        protected override void OnStart(string[] args)
        {
            serviceStarted = true;

            ThreadStart tracktimes_st = new ThreadStart(tracktimes_wf);
            tracktimes_wt = new Thread(tracktimes_st);
            tracktimes_wt.Start();
        }

        protected override void OnStop()
        {
            serviceStarted = false;
            tracktimes_wt.Join(new TimeSpan(0, 0, 5));
        }

        private void tracktimes_wf()
        {
            tracktimes_action();
            Thread.CurrentThread.Abort();
        }

        private static void tracktimes_action()
        {
            SqlConnection dbConn = new SqlConnection();

            try
            {
                ////create SQL variables
                //dbConn = new SqlConnection("Data Source=10.0.25.245;Failover Partner=10.0.25.246;Initial Catalog=letitride;User ID=asp_user;Password=l3t1tr1d3;");
                //SqlCommand dbCmd = new SqlCommand();
                //dbCmd.Connection = dbConn;

                //create WR variables
                DateTime time = DateTime.Today.AddHours(3);
                //string url = "http://api.mcla.us/request/?api_key=f62001122589294193682bb5ac6897c7&version=1.0&method=game&team=kennesaw_state";
                string url = "http://api.mcla.us/request/?api_key=f62001122589294193682bb5ac6897c7&version=1.0&method=roster&team=kennesaw_state";
                //string url = "http://api.mcla.us/request/?api_key=f62001122589294193682bb5ac6897c7&version=1.0&method=game&start=01/10/2010&end=09/30/2010";
                //string url = "http://mcla.us/scores/games/6353/";
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse httpResponse = (HttpWebResponse)webRequest.GetResponse();

                //StreamReader sr = new StreamReader(httpResponse.GetResponseStream());
                //StringBuilder sb = new StringBuilder(sr.ReadToEnd());

                ////get game detail
                //StringBuilder detail = new StringBuilder(sb.ToString());
                //int index = -1;
                ////remove up to beginning of div
                //string search = "<div class='fancy_home_game_report'><blockquote>";
                //index = detail.ToString().IndexOf(search);
                //if (index <= 0)
                //{
                //    search = "<div class='fancy_away_game_report'><blockquote>";
                //    index = detail.ToString().IndexOf(search);
                //}
                //detail = detail.Remove(0, index + search.Length);
                ////remove after end of div
                //index = detail.ToString().IndexOf("</blockquote>", 0);
                //detail = detail.Remove(index, detail.Length - index);

                //Console.WriteLine(HttpUtility.HtmlDecode(detail.ToString()));
                XmlDocument responseXML = new XmlDocument();
                responseXML.Load(httpResponse.GetResponseStream());

                StringBuilder sb = new StringBuilder(responseXML.InnerXml);
                Console.WriteLine("\n" + sb.ToString() + "\n");
                ////create string array
                //char[] delim = new char[1];
                //delim[0] = '|';
                //string[] test = sb.ToString().Split(delim, StringSplitOptions.RemoveEmptyEntries);

                ////setup DataTable for uploading to db
                //DataTable dt = new DataTable();
                //dt.Columns.Add("timeclass");
                //dt.Columns.Add("time");
                //dt.Columns.Add("trackclass");
                //dt.Columns.Add("track");
                //dt.Columns["timeclass"].DataType = System.Type.GetType("System.String");
                //dt.Columns["time"].DataType = System.Type.GetType("System.DateTime");
                //dt.Columns["trackclass"].DataType = System.Type.GetType("System.String");
                //dt.Columns["track"].DataType = System.Type.GetType("System.String");

                ////populate DataTable
                //int rowcount = 0;
                //DataRow dr = dt.NewRow();
                //foreach (string str in test)
                //{
                //    if (rowcount.Equals(0))
                //    {
                //        dr[0] = str;
                //        rowcount++;
                //    }
                //    else if (rowcount.Equals(1))
                //    {
                //        DateTime.TryParse(time.ToShortDateString() + " " + str, out time);
                //        dr[1] = time;
                //        rowcount++;
                //    }
                //    else if (rowcount.Equals(2))
                //    {
                //        dr[2] = str;
                //        rowcount++;
                //    }
                //    else if (rowcount.Equals(3))
                //    {
                //        dr[3] = str;
                //        dt.Rows.Add(dr);
                //        dr = dt.NewRow();
                //        rowcount = 0;
                //    }
                //}

                ////setup SqlBulkCopy
                //SqlBulkCopy bulkCopy = new SqlBulkCopy(dbConn);
                //SqlBulkCopyColumnMapping mapping1 = new SqlBulkCopyColumnMapping("timeclass", "timeclass");
                //SqlBulkCopyColumnMapping mapping2 = new SqlBulkCopyColumnMapping("time", "time");
                //SqlBulkCopyColumnMapping mapping3 = new SqlBulkCopyColumnMapping("trackclass", "trackclass");
                //SqlBulkCopyColumnMapping mapping4 = new SqlBulkCopyColumnMapping("track", "track");
                //bulkCopy.ColumnMappings.Add(mapping1);
                //bulkCopy.ColumnMappings.Add(mapping2);
                //bulkCopy.ColumnMappings.Add(mapping3);
                //bulkCopy.ColumnMappings.Add(mapping4);
                //bulkCopy.BatchSize = 100;
                //bulkCopy.BulkCopyTimeout = 30;
                //bulkCopy.DestinationTableName = "TrackTimes";

                ////setup dbCmd to clear previous TrackTimes
                //dbCmd.Parameters.Clear();
                //dbCmd.CommandType = CommandType.Text;
                //dbCmd.CommandText = "DELETE FROM TrackTimes";

                ////perform DB actions
                //dbConn.Open();
                //dbCmd.ExecuteNonQuery();
                //bulkCopy.WriteToServer(dt);
                //dbConn.Close();
            }
            catch (Exception) { }

            finally
            {
                dbConn.Close();
                dbConn.Dispose();
            }
        }

    }
}
