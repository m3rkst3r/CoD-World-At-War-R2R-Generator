using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net;
using System.Windows.Forms;
namespace SharpUpdate
{

    internal partial class SharpUpdateDownloadForm : Form
    {
        private WebClient webClient;
        private BackgroundWorker bgWorker;
        private string md5;
        internal string TempFilePath { get; }

        internal SharpUpdateDownloadForm(Uri location, string md5, Icon programIcon)
        {
            InitializeComponent();

            if (programIcon != null)
                this.Icon = programIcon;
            TempFilePath = Path.GetTempFileName();
            this.md5 = md5;
            webClient = new WebClient();
            webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(webClient_DownloadProgressChanged);
            webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(webClient_DownloadFileCompleted);
            bgWorker = new BackgroundWorker();
            bgWorker.DoWork += new DoWorkEventHandler(bgWorker_DoWork);
            bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgWorker_RunWorkerCompleted);
            try { webClient.DownloadFileAsync(location, this.TempFilePath); }
            catch { this.DialogResult = DialogResult.No; this.Close(); }
        }

        private void webClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            this.lblProgress.Text = string.Format("Downloaded {0} of {1}", FormatBytes(e.BytesReceived, 1, true), FormatBytes(e.TotalBytesToReceive, 1, true));
            this.progressBar.Value = e.ProgressPercentage;
        }

        private void webClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                this.DialogResult = DialogResult.No;
                this.Close();
            }
            else if (e.Cancelled)
            {
                this.DialogResult = DialogResult.Abort;
                this.Close();
            }
            else
            {
                this.lblProgress.Text = "Verifying Download...";
                this.progressBar.Style = ProgressBarStyle.Marquee;
                bgWorker.RunWorkerAsync(new string[] { this.TempFilePath, this.md5 });
            }
        }

        private string FormatBytes(long bytes, int decimalPlaces, bool showByteType)
        {
            double newBytes = bytes;
            string formatString = "{0";
            string byteType = "B";
            if (newBytes > 1024 && newBytes < 1048576)
            {
                newBytes /= 1024;
                byteType = "KB";
            }
            else if (newBytes > 1048576 && newBytes < 1073741824)
            {
                newBytes /= 1048576;
                byteType = "MB";
            }
            else
            {
                newBytes /= 1073741824;
                byteType = "GB";
            }
            if (decimalPlaces > 0)
                formatString += ":0.";
            for (int i = 0; i < decimalPlaces; i++)
                formatString += "0";
            formatString += "}";
            if (showByteType)
                formatString += byteType;
            return string.Format(formatString, newBytes);
        }

        private void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            string file = ((string[])e.Argument)[0];
            string updateMD5 = ((string[])e.Argument)[1];
            if (Hasher.HashFile(file, HashType.MD5).ToUpper() != updateMD5.ToUpper())
                e.Result = DialogResult.No;
            else
                e.Result = DialogResult.OK;
        }

        private void bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.DialogResult = (DialogResult)e.Result;
            this.Close();
        }

        private void SharpUpdateDownloadForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (webClient.IsBusy)
            {
                webClient.CancelAsync();
                this.DialogResult = DialogResult.Abort;
            }

            if (bgWorker.IsBusy)
            {
                bgWorker.CancelAsync();
                this.DialogResult = DialogResult.Abort;
            }
        }
    }
}
