using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;
using System.Security.Principal;

namespace OpenTCHere
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        // 为了获取 TC 地址 %COMMANDER_EXE%，在 TC 中运行此程序
        // 为了修改注册表，需要以管理员权限运行此程序

        private void Application_Startup(object sender, StartupEventArgs e)
        {
#if DEBUG
            Environment.SetEnvironmentVariable("COMMANDER_EXE", @"e:\Apps\Totalcmd\TOTALCMD64.EXE");
#endif
            string tc;
            if (e.Args.Length == 0)
            {
                tc = Environment.GetEnvironmentVariable("COMMANDER_EXE");
                if (string.IsNullOrEmpty(tc))
                {
                    MessageBox.Show("请在 Total Commander 中启动此程序");
                    this.Shutdown();
                }
                else if (!IsAdministrator())
                {
                    RunasAdmin(tc);
                    this.Shutdown();
                }
            }
            else
            {
                tc = e.Args[0];
            }

            Environment.SetEnvironmentVariable("TC", tc);
        }

        private static void RunasAdmin(string tc)
        {
            var startInfo = new ProcessStartInfo()
            {
                FileName = System.Reflection.Assembly.GetEntryAssembly().Location,
                Verb = "runas",
                UseShellExecute = true,
                Arguments = $"\"{tc}\""
            };

            try
            {
                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private static bool IsAdministrator()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}
