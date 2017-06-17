using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using Microsoft.Win32;
using System.IO;

namespace OpenTCHere
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string key1 = @"HKEY_CLASSES_ROOT\Directory\shell\totalcmd";
        private const string key2 = @"HKEY_CLASSES_ROOT\*\shell\totalcmd";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GetCurrentValues();
            tbTc.Text = Environment.GetEnvironmentVariable("TC");
            SetDefaults();
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            var name = tbName.Text.Trim();
            if (name == String.Empty)
            {
                tbName.Focus();
                return;
            };

            var tc = GetTC();
            if (String.IsNullOrEmpty(tc)) return;

            var args = tbArgs.Text.Trim();
            if (name == String.Empty)
            {
                tbArgs.Focus();
                return;
            };
            args = $"\"{tc}\" {args}";

            Registry.SetValue(key1, "", name);
            Registry.SetValue(key1, "icon", tc);
            Registry.SetValue($@"{key1}\command", "", args);

            Registry.SetValue(key2, "", name);
            Registry.SetValue(key2, "icon", tc);
            Registry.SetValue($@"{key2}\command", "", args);

            GetCurrentValues();
        }

        private void GetCurrentValues()
        {
            var name = Registry.GetValue(key1, "", null);
            var cmd = Registry.GetValue($@"{key1}\command", "", null);
            tbName0.Text = name == null ? "" : name.ToString();
            tbArgs0.Text = cmd == null ? "" : cmd.ToString();
        }

        private string GetTC()
        {
            var tc = tbTc.Text.Trim();
            if (tc == String.Empty)
            {
                MessageBox.Show($"请输入 TC 地址");
                tbTc.Focus();
                return null;
            };

            if (System.IO.Path.IsPathRooted(tc))
            {
                var name = System.IO.Path.GetFileName(tc).ToUpper();
                if (name == "TOTALCMD.EXE" || name == "TOTALCMD64.EXE")
                {
                    if (File.Exists(tc))
                        return tc;
                    else
                    {
                        MessageBox.Show($"找不到 {tc}");
                        return null;
                    }
                }
            }

            MessageBox.Show("输入的不是 TC 地址");
            tbTc.Focus();
            return null;
        }

        private void Unregister_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Registry.ClassesRoot.DeleteSubKeyTree(@"Directory\shell\totalcmd");
                Registry.ClassesRoot.DeleteSubKeyTree(@"*\shell\totalcmd");
            }
            catch (Exception)
            {
                //throw;
            }
            tbName0.Text = "";
            tbArgs0.Text = "";
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/yanxyz/OpenTCHere/");
        }

        private void Help_Click(object sender, RoutedEventArgs e)
        {
            var tc = GetTC();
            if (String.IsNullOrEmpty(tc)) return;

            var chm = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(tc), "TOTALCMD.CHM");
            if (String.IsNullOrEmpty(chm))
            {
                MessageBox.Show($"找不到 {chm}");
                return;
            }

            var startInfo = new ProcessStartInfo("hh.exe")
            {
                Arguments = $@"mk:@MSITStore:{chm}::/commandlineparameters.htm"
            };
            Process.Start(startInfo);
        }

        private void Recommend_Click(object sender, RoutedEventArgs e)
        {
            SetDefaults();
        }

        private void SetDefaults()
        {
            tbName.Text = "在 Total Commander 中打开";
            tbArgs.Text = "/O /T /A /S /L=\"%V\"";
        }
    }
}
