using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace D3_Bot_Tool
{
    class B_LoginBot
    {
        
        private System.ComponentModel.BackgroundWorker bw;
        public bool running = false;
        private string module_name = "LoginBot";

        public B_LoginBot()
        {
            bw = new System.ComponentModel.BackgroundWorker();
            bw.DoWork += new System.ComponentModel.DoWorkEventHandler(bw_DoWork);
            writeToMainLog("Module loaded!");
        }

        private void Run()
        {
            while (running)
            {
                if (main.getInstance().getPW() == "")
                {
                    writeToMainLog("No login password!");
                    break;
                }

                if (!Tools.login(module_name, int.MaxValue, true) && running)
                {
                    if (!main.getInstance().validD3Path())
                    {
                        writeToMainLog("Stopped! (Too much invalid states)");
                        break;
                    }

                    writeToMainLog("Restarting D3, because of invalid state.");
                    Tools.restartDiablo3(module_name);
                }
                else if(running)
                {
                    writeToMainLog("Logged in!");
                    break;
                }
            }

            if(!running)
                writeToMainLog("Stopped!");

            running = false;
            updateAtStop();
        }

        private void updateAtStop()
        {
            main.getInstance().updateLoginBot();
        }

        protected void writeToMainLog(String txt)
        {
            main.getInstance().writeToLog(module_name, txt);
        }

        private void bw_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            Run();
        }

        public void start(int maximum_runs, int run_time, int bot_sleeping_time)
        {
            if (main.getInstance().getPW() == "")
            {
                writeToMainLog("No login password!");
                running = false;
                updateAtStop();
                return;
            }

            running = true;
            if (!bw.IsBusy)
            {
                bw.RunWorkerAsync();
                writeToMainLog("Started!");
            }
            else
                writeToMainLog("Already Running!");

        }

        public void stop()
        {
            running = false;
        }
    }
}
