using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.IO;
using System.Configuration;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;

namespace LogFileReader {
    class Program {
        static void Main(string[] args) {
            ReadLogFiles r = new ReadLogFiles();
            r.Execute();
        }
    }

    public class Log {
        public DateTime Date { get; set; }
        public string S_ip { get; set; }
        public string Cs_method { get; set; }
        public string Cs_uri_stem { get; set; }
        public string Cs_uri_query { get; set; }
        public string S_port { get; set; }
        public string Cs_username { get; set; }
        public string C_ip { get; set; }
        public string Cs_user_agent { get; set; }
        public string Sc_status { get; set; }
        public int Sc_substatus { get; set; }
        public int Sc_win32_status { get; set; }
        public int Time_taken { get; set; }
        public int Iis_Id { get; set; }
    }

    public class Report {
        public DateTime Date { get; set; }
        public string Cs_username { get; set; }
        public int Hit { get; set; }
    }

    public class ReadLogFiles {
        public DataTable ToDataTable<T>(IList<T> data) {
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            for (int i = 0; i < props.Count; i++) {
                PropertyDescriptor prop = props[i];
                table.Columns.Add(prop.Name, prop.PropertyType);
            }

            object[] values = new object[props.Count];
            foreach (T item in data) {
                for (int i = 0; i < values.Length; i++) {
                    values[i] = props[i].GetValue(item);
                }
                table.Rows.Add(values);
            }
            return table;
        }

        public void Execute() {
            //List<Report> reports = new List<Report>();

            //get ref to file
            List<Log> logs = new List<Log>();

            foreach (FileInfo fi in new DirectoryInfo(ConfigurationManager.AppSettings["path"] + ConfigurationManager.AppSettings["iisId"]).GetFiles("*.log").Where(c => c.CreationTime > Convert.ToDateTime("2019-02-14"))) {
                try {
                    //open file
                    using (StreamReader sr = new StreamReader(fi.FullName)) {

                        Console.WriteLine("Opening:" + fi.FullName);

                        bool isValid = false;

                        while (sr.Peek() > -1) {
                            //get ref to line
                            string line = sr.ReadLine();

                            //start processing after this line
                            if (line.StartsWith("#Fields:")) {
                                isValid = true;
                                continue;
                            }

                            //stop processing here
                            if (line.StartsWith("#Software")) {
                                isValid = false;
                            }

                            if (isValid) {
                                string[] cols = line.Split(' ');

                                Log newLog = new Log();

                                //get each col data
                                newLog.Date = Convert.ToDateTime(cols[0] + " " + cols[1]);
                                newLog.S_ip = cols[2];
                                newLog.Cs_method = cols[3];
                                newLog.Cs_uri_stem = cols[4];
                                newLog.Cs_uri_query = cols[5];
                                newLog.S_port = cols[6];
                                newLog.Cs_username = cols[7];
                                newLog.C_ip = cols[8];
                                newLog.Cs_user_agent = cols[9];
                                newLog.Sc_status = cols[10];
                                newLog.Sc_substatus = Convert.ToInt32(cols[11]);
                                newLog.Sc_win32_status = Convert.ToInt32(cols[12]);
                                newLog.Time_taken = Convert.ToInt32(cols[13]);
                                newLog.Iis_Id = Convert.ToInt32(ConfigurationManager.AppSettings["iisId"]);

                                logs.Add(newLog);
                            }

                        }
                        sr.Close();
                    }
                } catch (IOException ie) {
                    Console.WriteLine("Cannot process " + fi.FullName);
                } finally {

                }
            }

            DataTable dtLog = ToDataTable<Log>(logs);

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["logFilesConnectionString"].ConnectionString)) {
                conn.Open();
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn)) {
                    foreach (DataColumn dc in dtLog.Columns) {
                        bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping(dc.ColumnName, dc.ColumnName));
                    }

                    bulkCopy.DestinationTableName = "dbo.iis_log_files";
                    bulkCopy.WriteToServer(dtLog);
                }
                conn.Close();
            }

        }
    }
}
