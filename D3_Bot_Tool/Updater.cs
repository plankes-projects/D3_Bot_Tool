using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace D3_Bot_Tool
{
    class Updater
    {
        //remember to remove updater at main startup <------------------------------------------------------------------------------------------------------------------
        private String updater = "XXXUpdater.exe";
        private String updated_file = "updated_file_name.exe";
        private String local_updated_file;
        private String update_location = "http://www.xxxxxx.com/updates/";

        public Updater()
        {
            local_updated_file = updated_file + ".tmp";
        }

        public void Run()
        {
            System.ComponentModel.BackgroundWorker bw = new System.ComponentModel.BackgroundWorker();
            bw.DoWork += new System.ComponentModel.DoWorkEventHandler(bw_DoWork);
            bw.RunWorkerAsync();

            try
            {
                System.Net.WebClient myClient = new System.Net.WebClient();

                myClient.DownloadFile(update_location + updated_file, local_updated_file);
                myClient.DownloadFile(update_location + updater, updater);

                System.IO.FileInfo exe = new System.IO.FileInfo(System.Windows.Forms.Application.ExecutablePath);
                String param = "\"" + exe.Name + "\" " + "\"" + local_updated_file + "\"";
                System.Diagnostics.Process.Start(updater, param);
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
            }

            bw.CancelAsync();
        }

        private void bw_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                UpdaterInfo ui = new UpdaterInfo("PlankesD3BotTool");
                ui.ShowDialog();
            }
            catch
            {
            }
        }
    }
}
