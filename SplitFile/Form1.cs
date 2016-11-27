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
        private CsvSplitter csvSplitter;
        
        public Form1()
        {
            InitializeComponent();
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
            long chunkSize = Convert.ToInt64(this.txtChunkSize.Text);
            string srcFileFull = this.txtSrcFile.Text;
            string srcFileName = Path.GetFileNameWithoutExtension(srcFileFull);
            bool includeHeader = this.checkBox1.Checked;
            string destDir = Directory.GetParent(srcFileFull) + "\\split\\";
            this.txtMessages.Text = "Counting lines in CSV File" + Environment.NewLine;
            this.csvSplitter = new CsvSplitter(srcFileFull, destDir, chunkSize, includeHeader);
            csvSplitter.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
            csvSplitter.Split();
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
            this.txtMessages.Text += "Completed File Splitting Process" + Environment.NewLine + " Wrote " + csvSplitter.getLineCount().ToString() + "Lines";
            MessageBox.Show("Completed File Splitting Process!");
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


        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(this.csvSplitter != null)
            {
                this.csvSplitter.cancel();
            }
        }
    }
}
