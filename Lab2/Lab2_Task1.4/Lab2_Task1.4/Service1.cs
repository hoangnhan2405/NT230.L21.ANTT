using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading;

namespace Lab2_Task1._4
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            //Cho phép Session thay đổi event để có thể popup thành công*
            this.CanHandleSessionChangeEvent = true;
            InitializeComponent();
        }
        //Khai báo sử dụng file wtsapi32.dll
        [DllImport("wtsapi32.dll", SetLastError = true)]
        //Khai báo hàm WTSSendMessage với các kiểu dữ liệu tương ứng
        static extern bool WTSSendMessage(
        IntPtr hServer,
        [MarshalAs(UnmanagedType.I4)] int SessionId,
        String pTitle,
        [MarshalAs(UnmanagedType.U4)] int TitleLength,
        String pMessage,
        [MarshalAs(UnmanagedType.U4)] int MessageLength,
        [MarshalAs(UnmanagedType.U4)] int Style,
        [MarshalAs(UnmanagedType.U4)] int Timeout,
        [MarshalAs(UnmanagedType.U4)] out int pResponse,
        bool bWait);
        public static IntPtr WTS_CURRENT_SERVER_HANDLE = IntPtr.Zero;
        public static int WTS_CURRENT_SESSION = 1;
        //Khai báo hàm để popup message box có tiêu đề là UIT và thông điệp là MSSV của nhóm
        protected override void OnSessionChange(SessionChangeDescription changeDescription)
        {
            //Kiểm tra trạng thái nếu như vào SessionLogon (Sau khi login thành công) 
            //hoặc SessionUnlock (Sau khi đăng nhập lại các lần sau)
            if (changeDescription.Reason == SessionChangeReason.SessionLogon
                || changeDescription.Reason == SessionChangeReason.SessionUnlock)
            {
                //Hàm PopupMessage
                PopupMessage();
                base.OnSessionChange(changeDescription);
            }
        }
        //Hàm PopupMessage để hiển thị Messagebox
        public void PopupMessage()
        {
            for (int user_session = 10; user_session > 0; user_session--)
            {
                Thread t = new Thread(() =>
                {
                    //Session 0 bị cô lập khỏi Windows 7 nên ta sẽ chọn ngẫu nhiên 1 session có giá trị từ 10 đổ lại
                    try
                    {
                        //Nội dung tiêu đề, thông điệp
                        string title = "UIT";
                        string msg = "18520065 - 18520326";
                        int resp = 1;
                        //Hàm để tạo Message box đã định nghĩa ở trên
                        WTSSendMessage(WTS_CURRENT_SERVER_HANDLE, user_session,
                                       title, title.Length, msg, msg.Length, 0, 0, out resp, true);
                    }
                    catch (Exception ex) { }
                });
                //Start Thread
                t.SetApartmentState(ApartmentState.STA);
                t.Start();
            }
        }
    }
}
