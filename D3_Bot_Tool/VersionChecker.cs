using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace D3_Bot_Tool
{
    class VersionChecker
    {

        double current_version = 1.11;


        private System.ComponentModel.BackgroundWorker bw = new System.ComponentModel.BackgroundWorker();
        String update_file_url = "http://www.xxxxxx.com/projectversions/D3BT";
        private bool silent_ = false;

        public double getVersion()
        {
            return current_version;
        }

        public VersionChecker( bool silent = false)
        {
            silent_ = silent;
            bw.DoWork += new System.ComponentModel.DoWorkEventHandler(bw_DoWork);
        }

        public void RunAsync()
        {
                bw.RunWorkerAsync();
        }

        private void showMessage(String text, bool force = false)
        {
            if (force || !silent_)
                System.Windows.Forms.MessageBox.Show(text);
        }

        private void bw_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                Run();
            }
            catch (Exception exc)
            {
                showMessage("Error: " + exc.Message); 
            }
        }

        private void Run()
        {
            System.Net.WebClient web_client = new System.Net.WebClient();

            String version_string = web_client.DownloadString(update_file_url);

            String[] version_lines = version_string.Split(new String[] { Environment.NewLine , "\n"}, StringSplitOptions.RemoveEmptyEntries);

            String[] latest_version = version_lines[version_lines.Count() - 1].Split(new char[] { '\t' });

            if (current_version < Convert.ToDouble(latest_version[0]))
            {
                foundNewerVersion(version_lines);
            }
            else
            {
                showMessage("Your version is up to date.");
            }
        }

        private void foundNewerVersion(String[] version_lines)
        {
            String information = "New version available:" + Environment.NewLine;

            foreach (String version_line in version_lines)
            {
                String[] version_split = version_line.Split(new char[] { '\t' });

                double verion_double = Convert.ToDouble(version_split[0]);
                if (current_version < verion_double)
                    information += "  " + verion_double.ToString("0.00").Replace(",", ".") + ": " + version_split[1] + Environment.NewLine;
            }
            information += Environment.NewLine + Environment.NewLine + "Do you want to upgrade?";
            System.Windows.Forms.DialogResult res = 
                System.Windows.Forms.MessageBox.Show(information, "", System.Windows.Forms.MessageBoxButtons.YesNo);

            if (res == System.Windows.Forms.DialogResult.Yes)
                new Updater().Run();
        }
    }

}
