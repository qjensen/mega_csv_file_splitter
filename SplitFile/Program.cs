using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace SplitFile
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
        	if(args.Length > 0) 
        	{
        		//command line
        	}
        	else 
        	{
        		//winform
        		Application.EnableVisualStyles();
	            Application.SetCompatibleTextRenderingDefault(false);
	            Application.Run(new Form1());
        	}
        }
    }
}
