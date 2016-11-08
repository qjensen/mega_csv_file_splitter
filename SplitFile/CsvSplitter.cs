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
		
		public event ProgressChangedEventHandler ProgressChanged;
		
		public CsvSplitter(string srcFile, string destDir, long chunkSize, bool includeHeader)
		{
			this._srcFile = srcFile;
			this._srcFileName = Path.GetFileNameWithoutExtension(srcFile);
			this._srcFileExt = Path.GetExtension(srcFile);
			this._destDir = destDir;
			this._chunkSize = chunkSize;
			this._includeHeader = includeHeader;
			this._totalLines = CountSourceLines(srcFile);
		}
		
		public Split() {
			BackgroundWorker bw = new BackgroundWorker();
			bw.DoWork += SplitFile;
		}
		
		public void SplitFile(object sender, DoWorkEventArgs e)
        {
            long lineNumberIn = 0;
            long lineNumberOut = 0;
            lineCountGlobal = 0;
            long chunkCount = 1;//counts the number of chunks or files written
            string headerLine = "";
			bw.
            bw.ReportProgress(0,new SplitFileMessage(SplitFileMessage.MSG_TYPE_INFO,"Found " + totalLines.ToString() + Environment.NewLine));
            if (!Directory.Exists(destDir))
            {
            	//TODO: add try/catch here
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
                using (StreamReader r = new StreamReader(this.srcFile))
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
