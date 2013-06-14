using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using System.Configuration;
using System.Diagnostics;
using SharpSvn;
using myRSS;
using System.Xml.Xsl;
using System.Xml.XPath;
using System.Xml;
using System.Timers;

namespace SvnLogs
{
    class ConsoleApplication
    {
        bool console_mode;

        string application_path;

        string repository_location;

        long last_revision;

        Output_Type output_type = Output_Type.Null;

        private Timer service_timer;

        public ConsoleApplication(string applicationPath, bool consoleMode)
        {
            this.console_mode = consoleMode;

            this.application_path = applicationPath;

            repository_location = ConfigurationManager.AppSettings["repository_location"];

            last_revision = Convert.ToInt32(ConfigurationManager.AppSettings["last_revision"]);

            switch (ConfigurationManager.AppSettings["output_type"].ToLower().Trim())
            {
                case "console":
                    output_type = Output_Type.Console;

                    break;
                case "txt":
                    output_type = Output_Type.Txt;

                    break;
                case "xml":
                    output_type = Output_Type.XML;

                    break;
                case "xmltransform":
                    output_type = Output_Type.XMLTransform;

                    break;
                case "rss":
                    output_type = Output_Type.RSS;

                    break;
                default:
                    Debug.Write("Unknown output type");

                    break;
            }

            if (consoleMode == true)
            {
                GetLogs(console_mode, application_path + "\\", repository_location, last_revision, output_type);

                Environment.Exit(0);
            }
            else
            {
                service_timer = new Timer();

                service_timer.Elapsed += new ElapsedEventHandler(service_timer_Elapsed);

                service_timer.Interval = Convert.ToDouble(ConfigurationManager.AppSettings["service_interval"]) * 1000 * 60;

                service_timer.Start();
            }
        }

        private void service_timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            service_timer.Stop();

            GetLogs(console_mode, application_path + "\\", repository_location, last_revision, output_type);

            service_timer.Start();
        }

        private static void GetLogs(bool console_mode, string application_path, string repository_location, long last_revision, Output_Type output_type)
        {
            SvnTarget repository;

            SvnClient client = new SvnClient();

            long current_revision = -1;

            if (SvnTarget.TryParse(repository_location, out repository) == true)
            {
                try
                {
                    SvnInfoEventArgs info;
                    client.GetInfo(new Uri(repository_location), out info);
                    current_revision = info.Revision;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);

                    if (console_mode == true)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }

                if (last_revision < current_revision)
                {
                    DataTable datatable = new DataTable("log");
                    datatable.Columns.Add("Revision", typeof(long));
                    datatable.Columns.Add("Author", typeof(string));
                    datatable.Columns.Add("Time", typeof(DateTime));
                    datatable.Columns.Add("ChangedPaths", typeof(string));
                    datatable.Columns.Add("LogMessage", typeof(string));

                    try
                    {
                        System.Collections.ObjectModel.Collection<SvnLogEventArgs> logitems = new System.Collections.ObjectModel.Collection<SvnLogEventArgs>();

                        SvnLogArgs logargs = new SvnLogArgs(new SvnRevisionRange(current_revision, last_revision + 1));

                        client.GetLog(new Uri(repository_location), logargs, out logitems);

                        datatable.BeginLoadData();

                        foreach (SvnLogEventArgs logitem in logitems)
                        {
                            StringBuilder ChangedPaths = new StringBuilder();

                            if (logitem.ChangedPaths != null)
                            {
                                foreach (SvnChangeItem path in logitem.ChangedPaths)
                                {
                                    ChangedPaths.AppendFormat("{1} {2}{0}", Environment.NewLine, path.Action, path.Path);

                                    if (path.CopyFromRevision != -1)
                                    {
                                        ChangedPaths.AppendFormat("{1} -> {2}{0}", Environment.NewLine, path.CopyFromPath, path.CopyFromRevision);
                                    }
                                }
                            }

                            DataRow datarow = datatable.NewRow();
                            datarow["Revision"] = logitem.Revision;
                            datarow["Author"] = logitem.Author;
                            datarow["Time"] = logitem.Time.ToLocalTime();
                            datarow["ChangedPaths"] = ChangedPaths.ToString();
                            datarow["LogMessage"] = logitem.LogMessage;

                            datatable.Rows.Add(datarow);
                        }

                        datatable.EndLoadData();

                        datatable.AcceptChanges();

                        switch (output_type)
                        {
                            case Output_Type.Console:
                                OutputToConsole(console_mode, application_path, datatable);

                                break;
                            case Output_Type.Txt:
                                OutputToTxt(console_mode, application_path, datatable);

                                break;
                            case Output_Type.XML:
                                OutputToXML(console_mode, application_path, datatable);

                                break;
                            case Output_Type.XMLTransform:
                                OutputToXMLTransform(console_mode, application_path, datatable);

                                break;
                            case Output_Type.RSS:
                                OutputToRSS(console_mode, application_path, datatable);

                                break;
                            default:
                                break;
                        }

                        last_revision = Convert.ToInt32(datatable.Compute("max(Revision)", string.Empty));

                        System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                        config.AppSettings.Settings.Remove("last_revision");
                        config.AppSettings.Settings.Add("last_revision", last_revision.ToString());
                        config.Save();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);

                        if (console_mode == true)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                }

                client.Dispose();
            }
            else
            {
                Debug.WriteLine("Unable to connect to repository");

                if (console_mode == true)
                {
                    Console.WriteLine("Unable to connect to repository");
                }
            }
        }

        private static void OutputToConsole(bool console_mode, string application_path, DataTable datatable)
        {
            try
            {
                DataView dataview = new DataView(datatable);
                dataview.Sort = "Revision";

                foreach (DataRow datarow in dataview.ToTable().Rows)
                {
                    Console.WriteLine(string.Format("Revision: {1}{0}Author: {2}{0}Time: {3}{0}Changed Paths: {4}{0}Log Message: {5}{0}{0}", Environment.NewLine, datarow["Revision"], datarow["Author"], datarow["Time"], datarow["ChangedPaths"], datarow["LogMessage"]));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);

                if (console_mode == true)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private static void OutputToTxt(bool console_mode, string application_path, DataTable datatable)
        {
            try
            {
                FileStream FileStream = new FileStream(application_path + "log.txt", FileMode.OpenOrCreate, FileAccess.Write);
                StreamWriter StreamWriter = new StreamWriter(FileStream);
                StreamWriter.BaseStream.Seek(0, SeekOrigin.End);

                DataView dataview = new DataView(datatable);
                dataview.Sort = "Revision";

                foreach (DataRow datarow in dataview.ToTable().Rows)
                {
                    StreamWriter.Write(string.Format("Revision: {1}{0}Author: {2}{0}Time: {3}{0}Changed Paths: {4}{0}Log Message: {5}{0}{0}", Environment.NewLine, datarow["Revision"], datarow["Author"], datarow["Time"], datarow["ChangedPaths"], datarow["LogMessage"]));
                }

                StreamWriter.Flush();
                StreamWriter.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);

                if (console_mode == true)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private static void OutputToXML(bool console_mode, string application_path, DataTable datatable)
        {
            try
            {
                if (File.Exists(application_path + "log.xml") == true)
                {
                    DataTable original = new DataTable("log");
                    original.ReadXml("log.xml");

                    DataView dataview = new DataView(datatable);
                    dataview.Sort = "Revision";

                    original.Merge(dataview.ToTable());

                    original.WriteXml("log.xml", XmlWriteMode.WriteSchema);
                }
                else
                {
                    DataView dataview = new DataView(datatable);
                    dataview.Sort = "Revision";

                    dataview.ToTable().WriteXml(application_path + "log.xml", XmlWriteMode.WriteSchema);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);

                if (console_mode == true)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private static void OutputToXMLTransform(bool console_mode, string application_path, DataTable datatable)
        {
            try
            {
                if (File.Exists(application_path + "log.xml") == true)
                {
                    DataTable original = new DataTable("log");
                    original.ReadXml(application_path + "log.xml");

                    DataView dataview = new DataView(datatable);
                    dataview.Sort = "Revision";

                    original.Merge(dataview.ToTable());

                    MemoryStream memorystream = new MemoryStream();
                    original.WriteXml(memorystream);
                    memorystream.Position = 0;
                    XPathDocument xpathdocument = new XPathDocument(memorystream);

                    XslCompiledTransform xsltransform = new XslCompiledTransform();
                    xsltransform.Load(application_path + "log.xsl");

                    XmlTextWriter textwriter = new XmlTextWriter(application_path + "log.transform", null);
                    xsltransform.Transform(xpathdocument, null, textwriter);
                    textwriter.Close();
                }
                else
                {
                    DataView dataview = new DataView(datatable);
                    dataview.Sort = "Revision";

                    dataview.ToTable().WriteXml(application_path + "log.xml", XmlWriteMode.WriteSchema);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);

                if (console_mode == true)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private static void OutputToRSS(bool console_mode, string application_path, DataTable datatable)
        {
            try
            {
                DataSet original = new DataSet();

                if (File.Exists(application_path + "log.rss") == true)
                {
                    original.ReadXml(application_path + "log.rss");
                }

                TextWriter textwriter = new StreamWriter(application_path + "log.rss");

                RSS_Writer rsswriter = new myRSS.RSS_Writer(textwriter);
                rsswriter.WriteStartDocument();
                rsswriter.WriteStartChannel("title", "url", "description", "Copyright © Matt Brown", "Matt Brown");

                if (original.Tables["item"] != null)
                {
                    foreach (DataRow datarow in original.Tables["item"].Select(string.Format("pubdate >= #{0}#", DateTime.Today.AddDays(-1)), "pubdate"))
                    {
                        rsswriter.WriteItem(Convert.ToString(datarow["title"]), Convert.ToString(datarow["link"]), Convert.ToString(datarow["description"]), Convert.ToString(datarow["author"]), Convert.ToDateTime(datarow["pubdate"]), Convert.ToString(datarow["subject"]));
                    }
                }

                DataView dataview = new DataView(datatable);
                dataview.Sort = "Revision";

                foreach (DataRow datarow in dataview.ToTable().Rows)
                {
                    rsswriter.WriteItem(Convert.ToString(datarow["Revision"]), string.Empty, Convert.ToString(datarow["LogMessage"]).Replace(Environment.NewLine, "<br />") + "<br />" + Convert.ToString(datarow["ChangedPaths"]).Replace(Environment.NewLine, "<br />"), Convert.ToString(datarow["Author"]), Convert.ToDateTime(datarow["Time"]), Convert.ToString(datarow["Revision"]).Replace(Environment.NewLine, "<br />"));
                }

                rsswriter.WriteEndChannel();
                rsswriter.WriteEndDocument();
                rsswriter.Close();

                textwriter.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);

                if (console_mode == true)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private enum Output_Type
        {
            Null,
            Console,
            Txt,
            XML,
            XMLTransform,
            RSS
        }

        public void Close()
        {
        }
    }
}
