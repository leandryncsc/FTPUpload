using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace FTPUpload
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        struct FtpSetting
        {
            public string Server { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public string FileName { get; set; }
            public string FullName { get; set; }
        }

        //FtpSetting _inputParameter;
        private void backgroundWorker_DoWork(string fileName, string fullName, string userName, string password, string server)
        {
            /*
             = "WhatsApp.jpeg";
             = @"C:\WhatsApp.jpeg";
             = "rufiao";
             = "99004433";
             = "ftp://192.168.0.141";
            */

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(new Uri(string.Format("{0}/{1}", server, fileName)));
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential(userName, password);
            Stream ftpStream = request.GetRequestStream();
            FileStream fs = File.OpenRead(fullName);
            byte[] buffer = new byte[1024];
            double total = (double)fs.Length;
            int byteRead = 0;
            double read = 0;
            do
            {
                if (!backgroundWorker.CancellationPending)
                {
                    byteRead = fs.Read(buffer, 0, 1024);
                    ftpStream.Write(buffer, 0, byteRead);
                    read += (double)byteRead;
                    double percentage = read / total * 100;
                    backgroundWorker.ReportProgress((int)percentage);
                }
            }
            while (byteRead != 0);
            fs.Close();
            ftpStream.Close();
        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            lblStatus.Text = $"Upload {e.ProgressPercentage} %";
            progressBar.Value = e.ProgressPercentage;
            progressBar.Update();
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            lblStatus.Text = "Upload Completo ! ";
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog() { Multiselect = false, ValidateNames = true, Filter = "All files|*.*" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    FileInfo fi = new FileInfo(ofd.FileName);

                    backgroundWorker_DoWork(fi.Name, fi.FullName, txtUserName.Text, txtPassword.Text, txtServer.Text);
                }
            }
        }
    }
}
