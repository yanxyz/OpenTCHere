using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace OpenTCHere
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {

            if (args.Length == 0)
            {
                string tc = Environment.GetEnvironmentVariable("COMMANDER_EXE");
                if (string.IsNullOrEmpty(tc))
                {
                    MessageBox.Show("请先启动 Total Commander", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    RunasAdmin(tc);
                }

            }
            else
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1(args[0]));
            }

        }


        static void RunasAdmin(string tc)
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.Verb = "runas";
                startInfo.FileName = Application.ExecutablePath;
                startInfo.Arguments = tc;
                Process.Start(startInfo);
            }
            catch (Exception)
            {
               // throw;
            }

        }


    }
}
