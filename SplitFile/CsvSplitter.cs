/*
 * Created by SharpDevelop.
 * User: jenseqxklp
 * Date: 11/8/2016
 * Time: 1:15 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Data;
using System.IO;


namespace SplitFile
{
	/// <summary>
	/// Description of CsvSplitter.
	/// </summary>
	public class CsvSplitter
	{
		private bool _includeHeader;
		private long _chunkSize;
		private long _lineCount;
		private string _srcFile;
		private string _destDir;
		private string _srcFileName;
		private string _srcFileExt;
		private long _chunkCount;
		private long _totalLines;
		private BackgroundWorker _bw;
		
		public event ProgressChangedEventHandler ProgressChanged;
		
		public CsvSplitter(string srcFile, string destDir, long chunkSize, bool includeHeader)
		{
			this._srcFile = srcFile;
			this._srcFileName = Path.GetFileNameWithoutExtension(srcFile);
			this._srcFileExt = Path.GetExtension(srcFile);
			this._destDir = destDir;
			this._chunkSize = chunkSize;
			this._includeHeader = includeHeader;
			this._totalLines = CountSourceLines();
		}
		
		public long getLineCount(){
		    return this._lineCount;
		}
		
		public void Split() {
			this._bw = new BackgroundWorker();
			this._bw.WorkerReportsProgress = true;
			this._bw.DoWork += SplitFile;
			this._bw.ReportProgress(0,new SplitFileMessage(SplitFileMessage.MSG_TYPE_INFO,"Found " + this._totalLines.ToString()));
			this._bw.RunWorkerAsync();
		}
		
		public void SplitFile(object sender, DoWorkEventArgs e)
        {
            long lineNumberIn = 0;
            long lineNumberOut = 0;
            long lineCountGlobal = 0;
            long chunkCount = 1;//counts the number of chunks or files written
            string headerLine = "";
            
            if (!Directory.Exists(this._destDir))
            {
            	//TODO: add try/catch here
                Directory.CreateDirectory(this._destDir);
            }
            string outFile = this._destDir + this._srcFileName + "_chunk" + chunkCount.ToString() + this._srcFileExt;
            TextWriter destWriter = null;

            try
            {
               destWriter = new StreamWriter(outFile);
            }
            catch (Exception ex)
            {
                SplitFileMessage msg = new SplitFileMessage(SplitFileMessage.MSG_TYPE_ERR, ex.Message);
                this._bw.ReportProgress(0, msg);
            }

            try
            {
                using (StreamReader reader = new StreamReader(this._srcFile))
                {
                    string csvLine;
                    while((csvLine = reader.ReadLine()) != null)
                    {
                        lineNumberIn++;
                        if (lineNumberIn == 1 && this._includeHeader)
                        {
                            headerLine = csvLine;

                            //writes header line out first time through
                            destWriter.WriteLine(headerLine);
                            lineNumberOut++;
                            this._lineCount++;
                        }

                        if (lineNumberOut <= this._chunkSize)
                        {
                            destWriter.WriteLine(csvLine);
                            lineNumberOut++;
                            this._lineCount++;
                        }
                        else
                        {
                            destWriter.Flush();
                            destWriter.Close();
                            this._chunkCount++;
                            outFile = this._destDir + this._srcFileName + "_chunk" + chunkCount.ToString() + this._srcFileExt;
                            try
                            {
                                destWriter = new StreamWriter(outFile);
                                lineNumberOut = 1;
                            }
                            catch (Exception ex)
                            {
                                SplitFileMessage msg = new SplitFileMessage(SplitFileMessage.MSG_TYPE_ERR, ex.Message);
                                this._bw.ReportProgress(0, msg);
                            }
                            if (this._includeHeader)
                            {
                                destWriter.WriteLine(headerLine);
                                lineNumberOut++;
                                lineCountGlobal++;
                            }
                            destWriter.WriteLine(csvLine);
                            lineNumberOut++;
                            lineCountGlobal++;
                            //update progress bar after each chunk is processed
                            double pct = ((double)lineNumberIn / (double)this._totalLines) * 100;
                            int displayPct = (int)Math.Round(pct, 0);
                            this._bw.ReportProgress(displayPct);
                            //System.Threading.Thread.Sleep(0);
                        }
                        if (this._bw.CancellationPending)
                        {
                            e.Cancel = true;
                            destWriter.Flush();
                            destWriter.Close();
                        }
                    }
                    destWriter.Flush();
                    destWriter.Close();
                }
            }
            catch (Exception ex)
            {
                SplitFileMessage msg = new SplitFileMessage(SplitFileMessage.MSG_TYPE_ERR, ex.Message);
                this._bw.ReportProgress(0, msg);
            }
            this._bw.ReportProgress(100);
        }
		
		private void HandleProgressChanged(object sender, ProgressChangedEventArgs e)
	    {
	        if (ProgressChanged != null)
	            ProgressChanged.Invoke(this, e);
	    }
		
		/// <summary>
        /// Counts the number of lines in file before process. This has been tested in files
        /// with millions of lines and runs in acceptable time
        /// </summary>
        /// <param name="path">Full path to the source CSV file</param>
        /// <returns></returns>
        public long CountSourceLines()
        {
            long count = 0;
            try
            {
                using (StreamReader r = new StreamReader(this._srcFile))
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
                this._bw.ReportProgress(0, msg);
            }
            return count;
        }
        
        public void cancel() {
            if (this._bw.IsBusy)
            {
                this._bw.CancelAsync();
            }

            //give the backgroundworker a few ms to finish
            while (this._bw.IsBusy)
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
