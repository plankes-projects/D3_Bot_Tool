using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace D3_Bot_Tool
{
    abstract class B_BaseBot
    {
        private System.ComponentModel.BackgroundWorker bw;
        public bool running = false;
        protected string module_name = "BaseBot";
        protected int death = 0;

        protected bool do_money_calc = true;
        private MoneyCalculator money_calc;
        protected int free_slots = 0;
        protected int gold_per_hour = 0;

        private int maximum_runs;
        private int run_time;
        private int bot_sleeping_time;
        protected int print_run_each = 10;

        public B_BaseBot(String module_name, bool active = true)
        {
            this.module_name = module_name;
            bw = new System.ComponentModel.BackgroundWorker();
            bw.DoWork += new System.ComponentModel.DoWorkEventHandler(bw_DoWork);
            if (active)
                writeToMainLog("Module loaded!");
        }

        protected void writeToMainLog(String txt)
        {
            main.getInstance().writeToLog(module_name, txt);
        }

        private void bw_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            Run();
        }

        abstract protected void moreStartOptions();
        abstract protected void updateAtStop();
        abstract protected bool isActiveInMain();
        abstract protected bool route(ref Random rnd, int big_run_time_min, int run_number);
        abstract protected void writeToLogAtEnd(int run_number);
        abstract protected void storeRoute(ref Random rnd, int run_number);

        //called every time run ends
        private void emergencyStop(String msg, int run_number)
        {
            //GameStateChecker.getInstance().stateChangedEvent -= new GameStateChecker.GameStateChangedHandler(B_BaseBot_stateChangedEvent);
            
            writeToMainLog(msg);
            writeToLogAtEnd(run_number);

            running = false;

            if (isActiveInMain() == false)
                return;

            //rest are options for bot end and setting the checkbox
            updateAtStop();
            

            if (main.getInstance().shutdown() == true)
            {
                try
                {
                    writeToMainLog("Shutting down PC.");
                    String file = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "log_at_shutdown");
                    System.IO.File.WriteAllText(file, main.getInstance().getLog());
                }
                catch { }

                System.Diagnostics.Process.Start("shutdown", "-p -f");
            }

            if (main.getInstance().closeGameAtBotStop() == true)
            {
                writeToMainLog("Closing Diablo 3.");
                Tools.killDiablo3();
            }
        }

        protected enum checkResult { ok, dead, ingame, disc };

        private void Run()
        {
            money_calc = new MoneyCalculator();
            Random rnd = new Random();
            int pause_counter = 0;
            int runs_without_rest = rnd.Next(40, 60);
            int run_counter = 0;
            DateTime last_reconnect = DateTime.Now;
            int next_reconnect_random = rnd.Next(0, 10);
            DateTime start = DateTime.Now;

            while (running && run_counter < maximum_runs)
            {
                run_counter++;
                pause_counter++;
                checkRest(ref rnd, ref pause_counter, ref runs_without_rest, run_counter);

                if(main.getInstance().getPW() != "")
                    checkReconnect(ref rnd, ref last_reconnect, ref next_reconnect_random);

                if (!openGame(ref rnd, run_counter))
                    return;

                System.Threading.Thread.Sleep(main.getInstance().getStartDelay());
                route(ref rnd, run_time, run_counter);

                if (main.getInstance().getStoreToStash())
                    storeRoute(ref rnd, run_counter);

                if (do_money_calc)
                {
                    int gold = D3InventoryStuff.getInstance().getGold(module_name);
                    if(gold != -1)
                        money_calc.add(new MoneyTime(DateTime.Now, gold));
                }

                updateAtMain(money_calc);

                closeGame(ref rnd);

            }

            emergencyStop("Stopped at run " + run_counter + "/" + maximum_runs + "!", run_counter);
        }

        private void updateAtMain(MoneyCalculator money_calc)
        {
            TimeSpan tmp_run_time = new TimeSpan(0,0,0);
            int tmp_start_gold = 0;
            int tmp_current_gold = 0;

            try
            {
                if (do_money_calc)
                {
                    tmp_current_gold = money_calc.current_money;
                    gold_per_hour = money_calc.getGoldperHour();
                    tmp_start_gold = money_calc.start_money;
                }
                tmp_run_time = DateTime.Now - money_calc.start_time;
            }
            catch { }


            main.getInstance().updateBaseBot(tmp_run_time, gold_per_hour, tmp_start_gold, tmp_current_gold, free_slots);
        }

        private void checkRest(ref Random rnd, ref int pause_counter, ref int runs_without_rest, int run_counter)
        {
            if (pause_counter > runs_without_rest)
            {
                pause_counter = 0;
                runs_without_rest = rnd.Next(50, 80);

                int nxt_rest = rnd.Next(bot_sleeping_time, bot_sleeping_time + 30000);
                writeToMainLog("Did " + run_counter + "/" + maximum_runs + " runs, resting now for " + (nxt_rest / 1000) + " seconds!");
                System.Threading.Thread.Sleep(nxt_rest);
            }
            else if ((run_counter % print_run_each) == 0)
                writeToMainLog("Did " + run_counter + "/" + maximum_runs + " runs, already!");
        }
        private void checkReconnect(ref Random rnd, ref DateTime last_reconnect, ref int next_reconnect_random)
        {
            int delay_int = main.getInstance().getRestartDelay();
            if (delay_int == 0)
                return;

            TimeSpan delay = new TimeSpan(0, delay_int + next_reconnect_random, 0);
            if ((DateTime.Now - last_reconnect) >= delay)
            {
                writeToMainLog("Restarting Diablo3 after "+ delay.ToString() + ".");
                last_reconnect = DateTime.Now;
                next_reconnect_random = rnd.Next(0, 10);
                Tools.restartDiablo3(module_name);
            }
        }

        private void closeGame(ref Random rnd)
        {
            if (GameStateChecker.getInstance().current_game_state.game_state != GameState.GameStates.DisconnectDienst)
            {
                if(!(GameStateChecker.getInstance().current_game_state.game_state == GameState.GameStates.InGame))
                    return;

                //ESC click 
                Tools.LeftClick(rnd.Next(567, 576), rnd.Next(589, 610), true, 500, 800);
                Tools.LeftClick(rnd.Next(340, 471), rnd.Next(344, 351), true, 300, 400);

                TimeSpan max = new TimeSpan(0, 0, 13);
                DateTime start = DateTime.Now;
                while (true)
                {
                    System.Threading.Thread.Sleep(300);
                    if (GameStateChecker.getInstance().current_game_state.game_state == GameState.GameStates.CharScreen_RedEnterGameButton)
                        return;

                    if (DateTime.Now - start >= max)
                        break;
                }
            }

            //got a disconnect press 2 times enter to come to login screen
            writeToMainLog("Got a disconnect!");
            Tools.PressKey(Keys.Enter);
            Tools.PressKey(Keys.Enter, true, 5000, 5500);
            return;
        }

        private bool openGame(ref Random rnd, int run_counter)
        {
            int max_d3_restarts = main.getInstance().getMaxRestarts();
            int restarts = 0;

            int invalid_state = 50;
            int invalid_state_counter = 0;
            bool game_is_open = false;
            while (!game_is_open)
            {
                System.Threading.Thread.Sleep(300);

                if (!running)
                {
                    emergencyStop("Stopped at run " + run_counter + "/" + maximum_runs + "!", run_counter);
                    return false;
                }

                switch(GameStateChecker.getInstance().current_game_state.game_state)
                {
                    case GameState.GameStates.D3NotLoaded:
                        writeToMainLog("Starting Diablo3");
                        Tools.restartDiablo3(module_name);
                    break;

                    case GameState.GameStates.InGame:
                        game_is_open = true;
                    break;

                    case GameState.GameStates.LoginScreen:
                        if (!Tools.login(module_name, main.getInstance().getLoginTrys()))
                        {
                            if (main.getInstance().getPW() == "")
                            {
                                emergencyStop("Stopped at run " + run_counter + "/" + maximum_runs + "! (No password for reconnect!)", run_counter);
                                return false;
                            }

                            writeToMainLog("Failed to reconnect.");

                            //this is not nessesary if we dont jump to default
                            invalid_state_counter = invalid_state; // this will force a restart
                            //goto default;
                        }
                        else
                        {
                            invalid_state_counter = 0;
                            restarts = 0;
                            continue;
                        }
                    break;

                    case GameState.GameStates.CharScreen_RedEnterGameButton:
                        //spiel fortführen click
                        Tools.LeftClick(rnd.Next(68, 203), rnd.Next(253, 272));
                        invalid_state_counter = 0;
                        break;

                    case GameState.GameStates.CharScreen_GrayEnterGameButton:
                        invalid_state_counter = 0;
                        break;
 
                    case GameState.GameStates.DisconnectDienst:
                        //ok klick
                        Tools.LeftClick(rnd.Next(371, 440), rnd.Next(375, 389));
                        invalid_state_counter = 0;
                    break;

                    case GameState.GameStates.LoadingScreen:
                        invalid_state_counter = 0;
                    break;

                    default:
                        if (invalid_state_counter >= invalid_state)
                        {
                            if (!main.getInstance().validD3Path())
                            {
                                emergencyStop("Stopped at run " + run_counter + "/" + maximum_runs + "! (Too much invalid states)", run_counter);
                                return false;
                            }

                            if (restarts >= max_d3_restarts)
                            {
                                emergencyStop("Stopped at run " + run_counter + "/" + maximum_runs + "! (Did " + restarts + "/" + max_d3_restarts + " restarts)", run_counter);
                                return false;
                            }

                            writeToMainLog("Restarting D3 at run " + run_counter + "/" + maximum_runs + "! (" + (restarts+1) + "/" + max_d3_restarts + " restarts)");
                            Tools.restartDiablo3(module_name);
 
                            invalid_state_counter = 0;
                            restarts++;

                        }
                        invalid_state_counter++;
                        if (invalid_state_counter % 10 == 0)
                            writeToMainLog(invalid_state_counter + "/" + invalid_state + " invalid states at open game!");
                    break;
                }
            }

            return true;
        }

        public void start(int maximum_runs, int run_time, int bot_sleeping_time)
        {
            running = true;
            if (!bw.IsBusy)
            {
                //GameStateChecker.getInstance().stateChangedEvent += new GameStateChecker.GameStateChangedHandler(B_BaseBot_stateChangedEvent);

                this.run_time = run_time;
                this.bot_sleeping_time = bot_sleeping_time * 1000;
                this.maximum_runs = maximum_runs;

                bw.RunWorkerAsync();
                writeToMainLog("Started for " + this.maximum_runs + " runs!");
                if(main.getInstance().getPW() == "")
                    writeToMainLog("WARNING: No password for reconnect!");
            }
            else
                writeToMainLog("Already Running!");
        }

        public void stop()
        {
            running = false;
        }

        private void B_BaseBot_stateChangedEvent(GameStateChecker game_state_checker, GameStateChangedEventArgs e)
        {
            writeToMainLog("Saw a change from <" + e.OldState.ToStringOneLine() + "> to <" + e.NewState.ToStringOneLine() + ">");
        }
    }

    class MoneyTime
    {
        public DateTime date;
        public int gold;
        public MoneyTime(DateTime date, int gold)
        {
            this.date = date;
            this.gold = gold;
        }
    }

    class MoneyCalculator
    {
        List<MoneyTime> circle = new List<MoneyTime>();

        public int current_money
        {
            get { return circle[1].gold; }
        }

        public int start_money
        {
            get{return circle[0].gold;}
        }

        public DateTime start_time
        {
            get{return circle[0].date;}
        }

        public void add(MoneyTime moneyTime)
        {
            if (circle.Count >= 2)
                circle.RemoveAt(1);

            circle.Add(moneyTime);

            if (circle.Count == 1)
                circle.Add(moneyTime);
        }

        public int getGoldperHour()
        {
            if (circle.Count() != 2)
                return -1;

            try
            {
                TimeSpan span = circle[1].date - circle[0].date;
                double gold_made = circle[1].gold - circle[0].gold;

                return (int)(gold_made / span.TotalMinutes * 60);
            }
            catch
            {
                return -1;
            }
        }
    }
}
