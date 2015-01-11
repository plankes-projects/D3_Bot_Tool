using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace D3_Bot_Tool
{
    public partial class main : Form
    {
        private static main instance = null;
        public static main getInstance()
        {
            return instance;
        }

        public bool loaded_d3_mem = false;
        private B_LoginBot loginBot;
        private B_HailiesChestBot HailiesChestBot;
        private B_DankCellarBot nxtBot;
        private B_levelBot lvlBot;

        private String module_name = "main";
        private bool finished_init = false;
        
        private String config_dir = "config";
        private String config_file = "pd3b_config";
        private String config_path;

        private String check_at_startup_key = "check_at_start_up";

        private String D3_exe = "";
        private String D3_exe_key = "D3_exe_path";

        public main()
        {

            int this_will_create_an_unque_hash = 100;
            this_will_create_an_unque_hash++;
            if (!System.IO.Directory.Exists(config_dir))
                System.IO.Directory.CreateDirectory(config_dir);

            config_path = System.IO.Path.Combine(config_dir, config_file);

            instance = this;
            try
            {
                System.IO.File.Delete("PlankesProjectsUpdater.exe");
            }
            catch { }

            InitializeComponent();

            this.Text = "PlankesD3BotTool v" + new VersionChecker().getVersion().ToString().Replace(',', '.');

            try
            {
                D3Stuff.getInstance();
                loaded_d3_mem = true;
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "error");
            }


            loaded_d3_mem = true; // if we dont want to start the tool without D3, move to Debug
#if DEBUG
            b_testButton.Visible = true;
#endif

            try
            {
                c_check_at_start_up.Checked = Convert.ToBoolean(new MyXML(config_path).read(check_at_startup_key));

                if (c_check_at_start_up.Checked)
                    new VersionChecker(true).RunAsync();
            }
            catch { }

            try
            {
                D3_exe = new MyXML(config_path).read(D3_exe_key);
            }
            catch { }
            
            if (System.IO.Path.GetFileName(D3_exe) == "Diablo III.exe" && System.IO.File.Exists(D3_exe))
                b_d3_exe.BackColor = Color.Green;
            
        }

        public String getLog()
        {
            return r_log.Text;
        }

        private void main_Load(object sender, EventArgs e)
        {
            PixelColors.getinstance();
            GameStateChecker.getInstance().start();

            writeToLog(module_name, D3Stuff.getInstance().getModuleName() + " loaded!");

            //load bot options
            MyXML xml = new MyXML(config_path);

            try
            {
                n_restart_delay.Value = Convert.ToDecimal(xml.read(restart_delay));
            }
            catch { }

            try
            {
                n_max_waittime.Value = Convert.ToDecimal(xml.read(max_waittime));
            }
            catch { }
            try
            {
                n_max_d3_restarts.Value = Convert.ToDecimal(xml.read(max_restarts));
            }
            catch { }

            try
            {
                n_start_delay.Value = Convert.ToDecimal(xml.read(start_delay));
            }
            catch { }
            try
            {
                n_login_trys.Value = Convert.ToDecimal(xml.read(login_trys));
            }
            catch { }
            try
            {
                c_demonHunter.Checked = Convert.ToBoolean(xml.read(demon_hunter));
            }
            catch { }
            try
            {
                c_remember_pass.Checked = Convert.ToBoolean(xml.read(remember_pass));
                if (c_remember_pass.Checked)
                {
                    String dec_pass = xml.read(encrypted_pass);

                    for (int i = 0; i < enc_times; i++)
                        dec_pass = Cypher.Decrypt(dec_pass);

                    t_pw.Text = dec_pass;
                }
            }
            catch { }
            try
            {
                Tools.adjust_bot_point.X = Convert.ToInt32(xml.read(adjust_point_x));
                Tools.adjust_bot_point.Y = Convert.ToInt32(xml.read(adjust_point_y));
            }
            catch { }

            //inithealthBot();
            try
            {
                D3InventoryStuff.getInstance();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }

            initHailiesBot();
            initNxtBot();
            initLoginBot();

            lvlBot = new B_levelBot();

            r_log.Text += "------------------------------" + Environment.NewLine;

            r_log.SelectionStart = r_log.Text.Length;
            r_log.ScrollToCaret();

            finished_init = true;

            try
            {
                if (xml.read("first_start") == "")
                {
                    MessageBox.Show("You are using this tool for the first time. Please adjust the bots with the \"" + b_adjust_bot.Text + "\" button.");
                    xml.write("first_start", "!");
                }
            }
            catch { }
        }

        private void initHailiesBot()
        {
            MyXML xml = new MyXML(config_path);

            try
            {
                n_hailiesChest_run_number.Value = Convert.ToDecimal(xml.read(hb_run_number));
            }
            catch { }

            try
            {
                n_hailieRunDalay.Value = Convert.ToDecimal(xml.read(hb_run_delay));
            }
            catch { }

            try
            {
                n_hailie_rest.Value = Convert.ToDecimal(xml.read(hb_rest));
            }
            catch { }
            
            HailiesChestBot = new B_HailiesChestBot();
        }

        private void initNxtBot()
        {
            MyXML xml = new MyXML(config_path);
            
            try
            {
                n_nxt_bot_num_runs.Value = Convert.ToDecimal(xml.read(nb_run_number));
            }
            catch { }

            try
            {
                n_nxt_bot_run_delay.Value = Convert.ToDecimal(xml.read(nb_run_delay));
            }
            catch { }

            try
            {
                n_nxt_bot_sleep_time.Value = Convert.ToDecimal(xml.read(nb_rest));
            }
            catch { }

            try
            {
                B_DankCellarBot.active = true;
                g_nxt_bot.Enabled = B_DankCellarBot.active;
            }
            catch { }

            try
            {
                c_nxt_auto_rep.Checked = Convert.ToBoolean(xml.read(nb_rep));
            }
            catch { }


            try
            {
                c_loot_set.Checked = Convert.ToBoolean(xml.read(loot_set));
            }
            catch { }
            try
            {
                c_loot_legendary.Checked = Convert.ToBoolean(xml.read(loot_legendary));
            }
            catch { }
            try
            {
                c_store_to_stash.Checked = Convert.ToBoolean(xml.read(loot_store_to_stash));
            }
            catch { }
            try
            {
                c_loot_rare.Checked = Convert.ToBoolean(xml.read(loot_rare));
            }
            catch { }

            nxtBot = new B_DankCellarBot();
        }

        private void initLoginBot()
        {
            loginBot = new B_LoginBot();   
        }


        public delegate void serviceGUIDelegate();

        String log_msg_DONT_USE_THIS = "";
        String log_module_name_DONT_USE_THIS = "";
        public void writeToLog(String module_name, String msg)
        {
            try
            {
                log_module_name_DONT_USE_THIS = module_name;
                log_msg_DONT_USE_THIS = msg;
                this.Invoke(new serviceGUIDelegate(writeToLogDelegate));
            }
            catch { }
        }
        private void writeToLogDelegate()
        {
            r_log.Text += DateTime.Now.ToShortTimeString() + " [" + log_module_name_DONT_USE_THIS + "] " + log_msg_DONT_USE_THIS + Environment.NewLine;
            r_log.SelectionStart = r_log.Text.Length;
            r_log.ScrollToCaret();
        }        

        public void updateHailieChestBot()
        {
            this.Invoke(new serviceGUIDelegate(updateHailieChestBotDelegate));
        }
        private void updateHailieChestBotDelegate()
        {
            c_HailieBot.Checked = HailiesChestBot.running;
        }

        public void updateLoginBot()
        {
            this.Invoke(new serviceGUIDelegate(updateLoginBotDelegate));
        }
        private void updateLoginBotDelegate()
        {
            c_loginbot.Checked = loginBot.running;
        }

        public void updateNextBot()
        {
            this.Invoke(new serviceGUIDelegate(updateNextBotDelegate));
        }
        private void updateNextBotDelegate()
        {
            c_nxt_bot.Checked = nxtBot.running;
        }

        private void c_healthbot_CheckedChanged(object sender, EventArgs e)
        {
            if (c_loginbot.Checked)
                loginBot.start(0,0,0);
            else
                loginBot.stop();
        }

        private void b_check_for_update_Click(object sender, EventArgs e)
        {
            new VersionChecker().RunAsync();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(linkLabel1.Text);
        }

        private void c_check_at_start_up_CheckedChanged(object sender, EventArgs e)
        {
            new MyXML(config_path).write(check_at_startup_key, c_check_at_start_up.Checked.ToString());
        }

        private void c_farm_bot_CheckedChanged(object sender, EventArgs e)
        {
            if (c_HailieBot.Checked)
                HailiesChestBot.start(Convert.ToInt32(n_hailiesChest_run_number.Value), Convert.ToInt32(n_hailieRunDalay.Value), Convert.ToInt32(n_hailie_rest.Value));
            else
                HailiesChestBot.stop();
        }

        public bool isHailieChecked()
        {
            return c_HailieBot.Checked;
        }

        public bool isLoginBotChecked()
        {
            return c_loginbot.Checked;
        }

        public bool isnextBotChecked()
        {
            return c_nxt_bot.Checked;
        }

        public bool shutdown()
        {
            return c_shut_down.Checked;
        }

        public bool closeGameAtBotStop()
        {
            return c_close_game_at_end.Checked;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                D3Stuff.getInstance().init();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "error");
            }
        }

        private String hb_run_number = "hb_run_number";
        private String hb_run_delay = "hb_run_delay";
        private String hb_rest = "hb_rest";

        private void n_hailiesChest_run_number_ValueChanged(object sender, EventArgs e)
        {
            if (!finished_init)
                return;

            new MyXML(config_path).write(hb_run_number, n_hailiesChest_run_number.Value.ToString());
        }

        private void n_hailieRunDalay_ValueChanged(object sender, EventArgs e)
        {
            if (!finished_init)
                return;

            new MyXML(config_path).write(hb_run_delay, n_hailieRunDalay.Value.ToString());
        }

        private void n_hailie_rest_ValueChanged(object sender, EventArgs e)
        {
            if (!finished_init)
                return;

            new MyXML(config_path).write(hb_rest, n_hailie_rest.Value.ToString());
        }

        private void TestButton(object sender, EventArgs e)
        {
            /*
            D3InventoryStuff i = new D3InventoryStuff();
            i.rightClickOnSlot(new Point(0, 0));
            return;
            //D3InventoryStuff.writeEmtyInvColors();
            /*
            D3InventoryStuff i = new D3InventoryStuff();
            LockedFastImage l = new LockedFastImage(Tools.D3ScreenShot());

            for(int x = 0; x < 10; x++)
                for(int y = 0; y < 6; y++)
                {
                    Point slot = new Point(x, y);
                    if(i.isFull(slot, l))
                        writeToLog("test", slot + ": full");
                }
            /*
            */
        }

        public String getPW()
        {
            return t_pw.Text;
        }


        private String nb_run_number = "nb_run_number";
        private String nb_run_delay = "nb_run_delay";
        private String nb_rest = "nb_rest";
        private String nb_rep = "nb_rep";

        private void n_nxt_bot_num_runs_ValueChanged(object sender, EventArgs e)
        {
            if (!finished_init)
                return;

            new MyXML(config_path).write(nb_run_number, n_nxt_bot_num_runs.Value.ToString());
        }

        private void n_xt_bot_run_delay_ValueChanged(object sender, EventArgs e)
        {
            if (!finished_init)
                return;

            new MyXML(config_path).write(nb_run_delay, n_nxt_bot_run_delay.Value.ToString());
        }

        private void n_next_bot_sleep_time_ValueChanged(object sender, EventArgs e)
        {
            if (!finished_init)
                return;

            new MyXML(config_path).write(nb_rest, n_nxt_bot_sleep_time.Value.ToString());
        }

        private void c_nxt_bot_CheckedChanged(object sender, EventArgs e)
        {
            if (c_nxt_bot.Checked)
                nxtBot.start(Convert.ToInt32(n_nxt_bot_num_runs.Value), Convert.ToInt32(n_nxt_bot_run_delay.Value), Convert.ToInt32(n_nxt_bot_sleep_time.Value));
            else
                nxtBot.stop();
        }

        private void c_nxt_auto_rep_CheckedChanged(object sender, EventArgs e)
        {
            if (!finished_init)
                return;
            new MyXML(config_path).write(nb_rep, c_nxt_auto_rep.Checked.ToString());
        }

        public bool isDaemonHunter()
        {
            return c_demonHunter.Checked;
        }

        private void l_HailieChest_info_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            String info = "Requirements:\n";
            info += "* WindowMode\n";
            info += "* Resize D3 window to smallest size (800x600)\n";
            info += "* Quest: Heart of the sin / Herz der Sünde\n";

            MessageBox.Show(info);
        }

        private void l_daemonHunter_info_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            String info = "Requirements:\n";
            info += "* Skill 1: Smokescreen (opt. with speed rune)\n";
            info += "* Skill 2: Shadowforce (opt. with speed rune)\n";
            info += "* Skill 3: Prepare (+10 disc rune)\n";
            info += "* Skill 4: Ferrets\n";
            info += "* Passive 1: Tactical penefits\n";
            info += "* Passive 2: Perfectionist\n";
            info += "* Passive 3: Hunt\n";

            info += "\n";
            info += "With 25% runspeed set 1200 rundelay\n";

            MessageBox.Show(info);
        }

        public int getStartDelay()
        {
            return Convert.ToInt32(n_start_delay.Value);
        }

        public int getLoginTrys()
        {
            return Convert.ToInt32(n_login_trys.Value);
        }

        string demon_hunter = "demon_hunter";
        private void c_daemonHunter_CheckedChanged(object sender, EventArgs e)
        {
            if (!finished_init)
                return;
            new MyXML(config_path).write(demon_hunter, c_demonHunter.Checked.ToString());
        }

        String login_trys = "login_trys";
        private void n_login_trys_ValueChanged(object sender, EventArgs e)
        {
            if (!finished_init)
                return;

            new MyXML(config_path).write(login_trys, n_login_trys.Value.ToString());
        }

        String start_delay = "start_delay";
        private void n_start_delay_ValueChanged(object sender, EventArgs e)
        {
            if (!finished_init)
                return;

            new MyXML(config_path).write(start_delay, n_start_delay.Value.ToString());
        }

        private Point divideAndConquerSearch(int last_fail, int last_ok)
        {
            int current_variance = last_fail + ((last_ok - last_fail) / 2);
            writeToLog(module_name, "Checking variance " + current_variance);
            Application.DoEvents();

            List<Point> points = Tools.checkImageToD3(new Bitmap(Image.FromFile("./config/Image/Adjust/login_logo.png")), 2, 5, 2, 60, 100, 75, 100, current_variance);

            if ((last_ok == current_variance ||  last_fail == current_variance) && points.Count != 1)
                return FAIL_POINT;
            
            if (points.Count > 1)
                return divideAndConquerSearch(last_fail, current_variance);
            else if (points.Count == 0)
                return divideAndConquerSearch(current_variance , last_ok);
            else
                return points[0];
        }

        private Point FAIL_POINT = new Point(-1, -1);
        private String adjust_point_x = "adjust_point_x";
        private String adjust_point_y = "adjust_point_y";
        private void b_adjust_bot_Click(object sender, EventArgs e)
        {
            Point found_point = divideAndConquerSearch(0, 500);

            if(found_point == FAIL_POINT)
            {
                writeToLog(module_name, "Could not adjust Bot!");
                return;
            }

            Point to_match = new Point(607, 585);

            Point adjust_point_p = new Point(found_point.X - to_match.X, found_point.Y - to_match.Y);

            if (Tools.adjust_bot_point == adjust_point_p)
            {
                writeToLog(module_name, "Bot was correctly adjusted.");
            }
            else
            {
                MyXML xml = new MyXML(config_path);
                xml.write(adjust_point_x, adjust_point_p.X.ToString());
                xml.write(adjust_point_y, adjust_point_p.Y.ToString());
                Tools.adjust_bot_point = adjust_point_p;

                writeToLog(module_name, "Bot correctly adjusted from " + Tools.adjust_bot_point.ToString() + " to " + adjust_point_p.ToString() + ".");
            }

            BackgroundWorker bgw = new BackgroundWorker();
            bgw.DoWork += new DoWorkEventHandler(AsyncAdjustPixelColors);
            bgw.RunWorkerAsync();
        }

        private void AsyncAdjustPixelColors(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            new AdjustPixelColors().ShowDialog();
        }

        private void b_d3_exe_Click(object sender, EventArgs e)
        {
            OpenFileDialog fi = new OpenFileDialog();
            fi.Filter = "Diablo III.exe|Diablo III.exe";
            if (fi.ShowDialog() != DialogResult.OK)
                return;

            D3_exe = fi.FileName;

            try
            {
                new MyXML(config_path).write(D3_exe_key, D3_exe);
            }
            catch
            {
                //on win xp the above try could send an error.
                //retry with absolute path
                try
                {
                    String path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase), config_path);

                    String to_remove = "file:\\";

                    path = path.Substring(to_remove.Count());
                    new MyXML(path).write(D3_exe_key, D3_exe);
                }
                catch (Exception ex) { writeToLog("error", ex.Message); }

            }

            if (System.IO.Path.GetFileName(D3_exe) == "Diablo III.exe" && System.IO.File.Exists(D3_exe))
                b_d3_exe.BackColor = Color.Green;
            else
            {
                b_d3_exe.BackColor = Color.Red;
                MessageBox.Show("You need to choose Diablo.exe");
            }
        }

        public String getD3Exepath()
        {
            return D3_exe;
        }

        public bool validD3Path()
        {
            if (b_d3_exe.BackColor == Color.Green)
                return true;
            else
                return false;
        }

        private void b_start_D3_Click(object sender, EventArgs e)
        {
            Tools.launchDiablo3Async(module_name);
        }
        
        private String max_waittime = "max_waittime";
        private void n_max_waittime_ValueChanged(object sender, EventArgs e)
        {
            if (!finished_init)
                return;
            
            new MyXML(config_path).write(max_waittime, n_max_waittime.Value.ToString());
        }

        public int getMaxLaunchWaittime()
        {

            return Convert.ToInt32(n_max_waittime.Value);
        }

        private String max_restarts = "max_restarts";
        private void n_max_d3_restarts_ValueChanged(object sender, EventArgs e)
        {
            if (!finished_init)
                return;

            new MyXML(config_path).write(max_restarts, n_max_d3_restarts.Value.ToString());
        }

        public int getMaxRestarts()
        {
            return Convert.ToInt32(n_max_d3_restarts.Value);
        }

        String restart_delay = "restart_delay";
        private void n_restart_delay_ValueChanged(object sender, EventArgs e)
        {
            if (!finished_init)
                return;

            new MyXML(config_path).write(restart_delay, n_restart_delay.Value.ToString());
        }

        public int getRestartDelay()
        {
            return Convert.ToInt32(n_restart_delay.Value);
        }

        private void c_stateChecker_CheckedChanged(object sender, EventArgs e)
        {
            if (c_stateChecker.Checked)
                GameStateChecker.getInstance().start();
            else
            {
                l_gamestate.Text = "";
                GameStateChecker.getInstance().stop();
            }
        }

        String gameStateCheckerMessage_DONT_USE_THIS = "";
        public void updateGamestate(String msg)
        {
            try
            {
                if (!c_stateChecker.Checked)
                    return;
                gameStateCheckerMessage_DONT_USE_THIS = msg;
                this.Invoke(new serviceGUIDelegate(updateGamestateDelegate));
            }
            catch { }
        }
        private void updateGamestateDelegate()
        {
            l_gamestate.Text = gameStateCheckerMessage_DONT_USE_THIS;
        }

        Point location_DONT_USE_THIS;
        public void setlocation(Point p)
        {
            try
            {
                location_DONT_USE_THIS = p;
                this.Invoke(new serviceGUIDelegate(setlocationDelegate));
            }
            catch { }
        }
        private void setlocationDelegate()
        {
            Location = location_DONT_USE_THIS;
        }

        int runs_DONT_USE_THIS = 0;
        int open_DONT_USE_THIS = 0;
        int death_DONT_USE_THIS = 0;
        int set_DONT_USE_THIS = 0;
        int leg_DONT_USE_THIS = 0;

        int open_sec_DONT_USE_THIS = 0;
        int closed_sec_DONT_USE_THIS = 0;
        int rep_sec_DONT_USE_THIS = 0;
        int rar_DONT_USE_THIS = 0;

        public void updateDankBot(int runs, int open, int death, int set, int leg, int rar, int mak, int open_sec, int closed_sec, int rep_sec)
        {
            try
            {
                runs_DONT_USE_THIS = runs;
                open_DONT_USE_THIS = open;
                death_DONT_USE_THIS = death;

                set_DONT_USE_THIS = set;
                leg_DONT_USE_THIS = leg;
                rar_DONT_USE_THIS = rar;

                open_sec_DONT_USE_THIS = open_sec;
                closed_sec_DONT_USE_THIS = closed_sec;
                rep_sec_DONT_USE_THIS = rep_sec;

                this.Invoke(new serviceGUIDelegate(updateDankBotDelegate));
            }
            catch { }
        }
        private void updateDankBotDelegate()
        {
            dank_death.Text = death_DONT_USE_THIS.ToString();
            dank_open.Text = open_DONT_USE_THIS.ToString() + "%";
            dank_runs.Text = runs_DONT_USE_THIS.ToString();
            
            c_loot_set.Text = "loot set (" + set_DONT_USE_THIS + ")";
            c_loot_legendary.Text = "loot legendary (" + leg_DONT_USE_THIS + ")";
            c_loot_rare.Text = "loot rare (" + rar_DONT_USE_THIS + ")";

            sec_closed.Text = closed_sec_DONT_USE_THIS.ToString();
            sec_open.Text = open_sec_DONT_USE_THIS.ToString();
            sec_rep.Text = rep_sec_DONT_USE_THIS.ToString();
        }

        string loot_set = "loot_set";
        private void c_loot_set_CheckedChanged(object sender, EventArgs e)
        {
            if (!finished_init)
                return;
            new MyXML(config_path).write(loot_set, c_loot_set.Checked.ToString());
        }

        string loot_legendary = "loot_legendary";
        private void c_loot_legendary_CheckedChanged(object sender, EventArgs e)
        {
            if (!finished_init)
                return;
            new MyXML(config_path).write(loot_legendary, c_loot_legendary.Checked.ToString());
        }

        string loot_store_to_stash = "loot_store_to_stash";
        private void c_store_to_stash_CheckedChanged(object sender, EventArgs e)
        {
            if (!finished_init)
                return;
            new MyXML(config_path).write(loot_store_to_stash, c_store_to_stash.Checked.ToString());
        }

        string loot_rare = "loot_rare";
        private void c_loot_rare_CheckedChanged(object sender, EventArgs e)
        {
            if (!finished_init)
                return;
            new MyXML(config_path).write(loot_rare, c_loot_rare.Checked.ToString());
        }

        public bool getLootSet()
        {
            return c_loot_set.Checked;
        }

        public bool getLootLegendary()
        {
            return c_loot_legendary.Checked;
        }

        public bool getStoreToStash()
        {
            return c_store_to_stash.Checked;
        }

        public bool getLootRare()
        {
            return c_loot_rare.Checked;
        }

        public bool getAutoRep()
        {
            return c_nxt_auto_rep.Checked;
        }

        private void l_dankCellarInfo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            String str = "";

            str += "Für bot is window mode 800x600 pflicht!\n";
            str += "bot im login screen eichen (adjust-button)\n";
            str += "\n";
            str += "skill1: rauchwolke (+dauer)\n";
            str += "skill2: slow am boden (+slow)\n";
            str += "skill3: Disziplin auffüllen (+leben reg)\n";
            str += "skill4: den gold begleiter\n";
            str += "\n";
            str += "rechtsklick: elementar pfeil(blitzkugel)\n";
            str += "\n";
            str += "passiv1: tactical benefits\n";
            str += "passiv2: sharpshooter\n";
            str += "passiv3: runspeed\n";
            str += "\n";
            str += "20kdps für inferno needed + mind 25 goldpick radius\n";
            str += "mit runspeed auf deine laufgeschwindigkeit trimmen, ca 300 = 25% runspeed\n";
            str += "\n";
            str += "Quest starten: 2.2 (erkundet den keller)\n";
            str += "nimmst wp rennst nach links bis du den checkpoint bekommst, leavst game startest bot \n";

            MessageBox.Show(str);
        }

        String remember_pass = "remember_pass";
        private void c_remember_pass_CheckedChanged(object sender, EventArgs e)
        {
            if (!finished_init)
                return;
            new MyXML(config_path).write(remember_pass, c_remember_pass.Checked.ToString());

            if (!c_remember_pass.Checked)
                new MyXML(config_path).write(encrypted_pass, "");
        }

        private void t_pw_TextChanged(object sender, EventArgs e)
        {
            if (c_remember_pass.Checked)
                storePass();
        }

        String encrypted_pass = "encrypted_pass";
        int enc_times = 5;
        private void storePass()
        {
            String enc = getPW();
            for (int i = 0; i < enc_times; i++)
                enc = Cypher.Encrypt(enc);

            new MyXML(config_path).write(encrypted_pass, enc);
        }


        private TimeSpan run_time_DO_NOT_USE_THIS;
        private int gold_per_hour_DO_NOT_USE_THIS;
        private int start_gold_DO_NOT_USE_THIS;
        private int current_gold_DO_NOT_USE_THIS;
        private int free_inv_slots_DO_NOT_USE_THIS;

        public void updateBaseBot(TimeSpan run_time, int gold_per_hour, int start_gold, int current_gold, int free_inv_slots)
        {
            try
            {
                run_time_DO_NOT_USE_THIS = run_time;
                if (gold_per_hour >= 0) gold_per_hour_DO_NOT_USE_THIS = gold_per_hour; else gold_per_hour_DO_NOT_USE_THIS = 0;
                start_gold_DO_NOT_USE_THIS = start_gold;
                current_gold_DO_NOT_USE_THIS = current_gold;
                free_inv_slots_DO_NOT_USE_THIS = free_inv_slots;

                this.Invoke(new serviceGUIDelegate(updateBaseBotDelegate));
            }
            catch { }
        }
        private void updateBaseBotDelegate()
        {
            String runtimestring = run_time_DO_NOT_USE_THIS.ToString();
            try
            {
                runtimestring = runtimestring.Substring(0, runtimestring.IndexOf('.'));
            } catch{}
            l_runtime.Text = "runtime: " + runtimestring;
            l_start_gold.Text = "start gold: " + (start_gold_DO_NOT_USE_THIS/1000) + "k";
            l_current_gold.Text = "current gold: " + (current_gold_DO_NOT_USE_THIS / 1000) + "k";
            l_gph.Text = "per hour: " + (gold_per_hour_DO_NOT_USE_THIS / 1000) + "k";

            l_free_inv_slots.Text = "free inv slots: " + free_inv_slots_DO_NOT_USE_THIS;

            l_farmed_gold.Text = "farmed gold: " + ((current_gold_DO_NOT_USE_THIS - start_gold_DO_NOT_USE_THIS) / 1000) + "k";
        }

        private void c_level_bot_CheckedChanged(object sender, EventArgs e)
        {
            if (c_level_bot.Checked)
                lvlBot.start(Convert.ToInt32(n_nxt_bot_num_runs.Value), Convert.ToInt32(n_nxt_bot_run_delay.Value), Convert.ToInt32(n_nxt_bot_sleep_time.Value));
            else
                lvlBot.stop();
        }

        public void updateLevelBot()
        {
            this.Invoke(new serviceGUIDelegate(updateLevelBotDelegate));
        }
        private void updateLevelBotDelegate()
        {
            c_level_bot.Checked = lvlBot.running;
        }

        public bool isLevelChecked()
        {
            return c_level_bot.Checked;
        }
    }
}
