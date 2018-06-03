// Original by Andrew Schofield: https://github.com/andrew-schofield/keepass2-haveibeenpwned
//
// This very cut-down version thrown together by Phlash because Debian 9 doesn't have a TLS1.2 capable mono runtime (seriously!)

using System;
using System.Linq;
using System.Diagnostics;
using System.Windows.Forms;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using KeePass.Plugins;
using KeePassLib;

[assembly: AssemblyTitle("HIBP checker")]
[assembly: AssemblyDescription("Checks KeePass entries for passwords that have been exposed in various breaches by using the haveibeenpwned.com API")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Phlash")]
[assembly: AssemblyProduct("KeePass Plugin")]
[assembly: AssemblyCopyright("Phil Ashby")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
[assembly: AssemblyVersion("1.1.0.0")]
[assembly: AssemblyFileVersion("1.1.0.0")]

namespace HIBP {
    public sealed class HIBPExt : Plugin {
        private static object _running = (Boolean)false;
        private IPluginHost _host;
        private string api = "https://api.pwnedpasswords.com/range/{0}";
        public HIBPExt() {
        }
        public override bool Initialize(IPluginHost host) {
            _host = host;
            ToolStripItemCollection tsMenu = _host.MainWindow.ToolsMenu.DropDownItems;
            tsMenu.Add(new ToolStripSeparator());
            ToolStripMenuItem tsItem = new ToolStripMenuItem();
            tsItem.Text = "Have I Been Pwned?";
            tsItem.Click += this.RunMe;
            tsMenu.Add(tsItem);
            return true;
        }
        public override void Terminate() {
        }
        private async void RunMe(object sender, EventArgs evt) {
            if (!_host.Database.IsOpen) {
                MessageBox.Show(_host.MainWindow, "No database open", "HIBP check", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            bool alreadyRunning = false;
            lock (_running) {
                if ((Boolean)_running)
                    alreadyRunning = true;
                else
                    _running = (Boolean)true;
            }
            if (alreadyRunning) {
                MessageBox.Show(_host.MainWindow, "Already running HIBP check", "HIBP check", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            SHA1 sha = new SHA1CryptoServiceProvider();
            var entries = _host.Database.RootGroup.GetEntries(true);
            int cnt = 0;
            foreach (var entry in entries) {
                 ++cnt;
                 if(entry.Strings.Get(PwDefs.PasswordField) == null || string.IsNullOrWhiteSpace(entry.Strings.ReadSafe(PwDefs.PasswordField)) || entry.Strings.ReadSafe(PwDefs.PasswordField).StartsWith("{REF:")) continue;
                 var passwordHash = string.Join("", sha.ComputeHash(entry.Strings.Get(PwDefs.PasswordField).ReadUtf8()).Select(x => x.ToString("x2"))).ToUpperInvariant();
                 var prefix = passwordHash.Substring(0, 5);
                 string stat = string.Format("{0} of {1} entries: {2}/{3}", cnt, entries.Count(), entry.Strings.ReadSafe(PwDefs.TitleField), prefix);
                 string url = string.Format(api, prefix);
                 ProcessStartInfo pcurl = new ProcessStartInfo("curl", string.Format("-s {0}", url));
                 ProcessStartInfo pwget = new ProcessStartInfo("wget", string.Format("-q -O - {0}", url));
                 pcurl.UseShellExecute = false;
                 pcurl.RedirectStandardOutput = true;
                 pwget.UseShellExecute = false;
                 pwget.RedirectStandardOutput = true;
                 // We try curl first, then wget
                 Process fetch = null;
                 try {
                    fetch = Process.Start(pcurl);
                 } catch (Exception e) {}
                 if (null == fetch || fetch.HasExited) {
                    _host.MainWindow.SetStatusEx(stat + " (wget)");
                    try {
                        fetch = Process.Start(pwget);
                    } catch (Exception e) {}
                 } else {
                     _host.MainWindow.SetStatusEx(stat + " (curl)");
                 }
                 if (null == fetch || fetch.HasExited) {
                     MessageBox.Show(_host.MainWindow, "Unable to find curl or wget - please install one :)", "Sorry", MessageBoxButtons.OK, MessageBoxIcon.Error);
                     break;
                 }
                 string line;
                 while ((line = await fetch.StandardOutput.ReadLineAsync()) != null) {
                     var parts = line.Split(':');
                     var suffx = parts[0];
                     var count = parts[1];
                     if (prefix + suffx == passwordHash)
                         MessageBox.Show(_host.MainWindow,
                             entry.Strings.ReadSafe(PwDefs.TitleField) +
                             "\nuser: " + entry.Strings.ReadSafe(PwDefs.UserNameField) +
                             "\nhash: " + passwordHash + 
                             "\noccurances: " + count,
                             "Pwned!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                 }
                 fetch.WaitForExit();
                 fetch.Close();
            }
            _host.MainWindow.SetStatusEx("HIBP check done.");
            lock (_running) {
                _running = (Boolean)false;
            }
        }
    }
}
