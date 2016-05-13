using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace SplitFile
{
    public partial class Form1 : Form
    {
        BackgroundWorker bw;
        long lineCountGlobal;

        public Form1()
        {
            InitializeComponent();
            bw = new BackgroundWorker();
        }

        private void btnSelFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog fb = new OpenFileDialog();

            DialogResult dr = fb.ShowDialog();

            if (dr == DialogResult.OK)
            {
                this.txtSrcFile.Text = fb.FileName;
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.txtMessages.Text = "Counting lines in CSV File" + Environment.NewLine;
            bw.DoWork += SplitFile;
            bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
            bw.WorkerSupportsCancellation = true;
            bw.WorkerReportsProgress = true;
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
            bw.RunWorkerAsync();
        }

        void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.UserState != null)
            {
                SplitFileMessage msg = (SplitFileMessage)e.UserState;

                if (msg.MessageType == SplitFileMessage.MSG_TYPE_ERR)
                {
                    throw new Exception(msg.Message);
                }
                else
                {
                    this.txtMessages.Text += msg.Message + Environment.NewLine;
                }
            }

            progressBar1.Value = e.ProgressPercentage;
            progressBar1.Update();
        }

        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.txtMessages.Text += "Completed File Splitting Process" + Environment.NewLine + " Wrote " + lineCountGlobal.ToString() + "Lines";
            MessageBox.Show("Completed File Splitting Process!");
        }

        public void SplitFile(object sender, DoWorkEventArgs e)
        {
            long lineNumberIn = 0;
            long lineNumberOut = 0;
            lineCountGlobal = 0;
            long chunkSize = Convert.ToInt64(this.txtChunkSize.Text);
            long chunkCount = 1;//counts the number of chunks or files written
            string srcFileFull = this.txtSrcFile.Text;
            long totalLines = CountSourceLines(srcFileFull);
            string srcFileName = Path.GetFileNameWithoutExtension(srcFileFull);
            bool includeHeader = this.checkBox1.Checked;
            string headerLine = "";
            string destDir = Directory.GetParent(srcFileFull) + "\\split\\";
            string ext = Path.GetExtension(srcFileFull);

            bw.ReportProgress(0,new SplitFileMessage(SplitFileMessage.MSG_TYPE_INFO,"Found " + totalLines.ToString() + Environment.NewLine));
            if (!Directory.Exists(destDir))
            {
                Directory.CreateDirectory(destDir);
            }
            string outFile = destDir + srcFileName + "_chunk" + chunkCount.ToString() + ext;
            TextWriter destWriter = null;

            try
            {
               destWriter = new StreamWriter(outFile);
            }
            catch (Exception ex)
            {
                SplitFileMessage msg = new SplitFileMessage(SplitFileMessage.MSG_TYPE_ERR, ex.Message);
                bw.ReportProgress(0, msg);
            }

            try
            {
                using (StreamReader reader = new StreamReader(srcFileFull))
                {
                    string csvLine;
                    while((csvLine = reader.ReadLine()) != null)
                    {
                        lineNumberIn++;
                        if (lineNumberIn == 1 && includeHeader)
                        {
                            headerLine = csvLine;

                            //writes header line out first time through
                            destWriter.WriteLine(headerLine);
                            lineNumberOut++;
                            lineCountGlobal++;
                        }

                        if (lineNumberOut <= chunkSize)
                        {
                            destWriter.WriteLine(csvLine);
                            lineNumberOut++;
                            lineCountGlobal++;
                        }
                        else
                        {
                            destWriter.Flush();
                            destWriter.Close();
                            chunkCount++;
                            outFile = destDir + srcFileName + "_chunk" + chunkCount.ToString() + ext;
                            try
                            {
                                destWriter = new StreamWriter(outFile);
                                lineNumberOut = 1;
                            }
                            catch (Exception ex)
                            {
                                SplitFileMessage msg = new SplitFileMessage(SplitFileMessage.MSG_TYPE_ERR, ex.Message);
                                bw.ReportProgress(0, msg);
                            }
                            if (includeHeader)
                            {
                                destWriter.WriteLine(headerLine);
                                lineNumberOut++;
                                lineCountGlobal++;
                            }
                            destWriter.WriteLine(csvLine);
                            lineNumberOut++;
                            lineCountGlobal++;
                            //update progress bar after each chunk is processed
                            double pct = ((double)lineNumberIn / (double)totalLines) * 100;
                            int displayPct = (int)Math.Round(pct, 0);
                            bw.ReportProgress(displayPct);
                            //System.Threading.Thread.Sleep(0);
                        }
                        if (bw.CancellationPending)
                        {
                            e.Cancel = true;
                            destWriter.Flush();
                            destWriter.Close();
                            Application.Exit();
                        }
                    }
                    destWriter.Flush();
                    destWriter.Close();
                }
            }
            catch (Exception ex)
            {
                SplitFileMessage msg = new SplitFileMessage(SplitFileMessage.MSG_TYPE_ERR, ex.Message);
                bw.ReportProgress(0, msg);
            }
            bw.ReportProgress(100);
        }

        /// <summary>
        /// filters key presses so only numbers are able to be entered
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtChunkSize_KeyPress(object sender, KeyPressEventArgs e)
        {
            const char Delete = (char)8;
            e.Handled = !Char.IsDigit(e.KeyChar) && e.KeyChar != Delete;
        }

        /// <summary>
        /// Counts the number of lines in file before process. This should be a pretty fast process
        /// even if the file has millions of lines.
        /// </summary>
        /// <param name="path">Full path to the source CSV file</param>
        /// <returns></returns>
        public long CountSourceLines(string path)
        {
            long count = 0;
            try
            {
                using (StreamReader r = new StreamReader(path))
                {
                    string line;
                    while ((line = r.ReadLine()) != null)
                    {
                        count++;
                    }
                }
            }
            catch (Exception ex)
            {
                SplitFileMessage msg = new SplitFileMessage(SplitFileMessage.MSG_TYPE_ERR, ex.Message);
                bw.ReportProgress(0, msg);
            }
            return count;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (bw.IsBusy)
            {
                bw.CancelAsync();
            }

            //give the backgroundworker a few ms to finish
            while (bw.IsBusy)
            {
                System.Threading.Thread.Sleep(100);
            }
        }
    }

    public class SplitFileMessage
    {
        public const int MSG_TYPE_INFO = 1;
        public const int MSG_TYPE_ERR = 2;

        private int _MessageType;

        public int MessageType
        {
            get { return _MessageType; }
            set { _MessageType = value; }
        }

        private string _Message;

        public string Message
        {
            get { return _Message; }
            set { _Message = value; }
        }

        public SplitFileMessage()
        {
        }

        public SplitFileMessage(int MessageType, string Message)
        {
            this.MessageType = MessageType;
            this.Message = Message;
        }
    }
}
