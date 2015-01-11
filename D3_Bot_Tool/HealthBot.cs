/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace D3_Bot_Tool
{
    class HealthBot
    {
        private System.ComponentModel.BackgroundWorker bw;

        public float HP = 1;
        public float treshhold = 1000;

        public DateTime last_used = new DateTime();
        public int cooldown = 31;

        public TimeSpan check_rate = new TimeSpan(0, 0, 0, 0, 1000);
        private bool run = false;

        private String module_name = "HealthBot";
        private System.Windows.Forms.Keys potion_key;

        public HealthBot(float tresh, TimeSpan checking_rate, char potion_key)
        {
            setPotionKey(potion_key, false);

            bw = new System.ComponentModel.BackgroundWorker();
            bw.DoWork += new System.ComponentModel.DoWorkEventHandler(bw_DoWork);
            treshhold = tresh;
            check_rate = checking_rate;

            main.getInstance().writeToLog(module_name, "Module loaded!");
        }

        private void usePotion()
        {
            int slept = Tools.PressKey(System.Windows.Forms.Keys.Q);
            main.getInstance().writeToLog(module_name, "Used a Postion at " + HP + " HP! After " + slept + " ms");
        }

        public void setPotionKey(char key, bool notify_main = true)
        {
            potion_key = Tools.ConvertCharToVirtualKey(key);

            if (notify_main)
                main.getInstance().writeToLog(module_name, "Changed potion key to " + potion_key.ToString());
        }

        public bool in_game = false;

        private void Run()
        {
            float new_hp;
            while (run)
            {
                new_hp = D3Stuff.getInstance().readHP();

                if (new_hp > 99999999 || new_hp <= 0)
                {
                    in_game = false;
                    main.getInstance().updateHealthBot();
                    System.Threading.Thread.Sleep(check_rate);
                    continue;
                }
                else
                    in_game = true;

                TimeSpan span = DateTime.Now - last_used;

                if (new_hp != HP || span.TotalSeconds < cooldown)
                {
                    HP = new_hp;
                    main.getInstance().updateHealthBot();
                }

                if (new_hp > 0 && new_hp < treshhold && span.TotalSeconds >= cooldown)
                {
                    usePotion();
                    last_used = DateTime.Now;
                }

                System.Threading.Thread.Sleep(check_rate);
            }

            main.getInstance().writeToLog(module_name, "Stoped!");
        }

        private void bw_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
                Run();
        }

        public void start()
        {
            main.getInstance().updateHealthBot();
            run = true;
            if (bw.IsBusy)
            {
                main.getInstance().writeToLog(module_name, "Already running!");
                return;
            }

            bw.RunWorkerAsync();
            main.getInstance().writeToLog(module_name, "Started!");
        }

        public void stop()
        {
            run = false;
        }
    }
}
*/