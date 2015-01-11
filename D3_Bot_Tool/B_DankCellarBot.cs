using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace D3_Bot_Tool
{
    class B_DankCellarBot : B_BaseBot
    {
        private enum possibleRuns{rep, open, closed, invalid};
        static public bool active = false;

        private int open_anz = 0;
        private int closed_anz = 0;
        private int set_anz = 0;
        private int legendary_anz = 0;
        private int rare_anz = 0;
        private int rep_anz = 0;
        private int finished_store_runs = 0;

        private TimeSpan time_open_runs;
        private TimeSpan time_closed_runs;
        private TimeSpan time_rep_runs;
        private possibleRuns run_was = possibleRuns.invalid;

        public B_DankCellarBot()
            : base("DankCellarBot", active)
        {
            print_run_each = 5;
        }

        protected override void writeToLogAtEnd(int run_number)
        {
            int open_in_procent = 100 * open_anz / run_number;
            int tmp_rep = 0;
            try
            {
                tmp_rep = (int)time_rep_runs.TotalSeconds / (run_number - closed_anz - open_anz);
            }
            catch { }
            int tmp_closed = 0;
            try
            {
                tmp_closed = (int)time_closed_runs.TotalSeconds / closed_anz;
            }
            catch { }

            int tmp_open = 0;
            try
            {
                tmp_open = (int)time_open_runs.TotalSeconds / open_anz;
            }
            catch { }

            writeToMainLog("Death: " + death + " -- Reps: " + rep_anz + " -- Opens " + open_in_procent + "% -- open (s/run): " + tmp_open + " -- closed (s/run): " + tmp_closed + " -- rep (s/run) " + tmp_rep);
            writeToMainLog("GPH" + (int)(gold_per_hour/1000) + "k | Loots: " + set_anz + "x set -- " + legendary_anz + "x legendary -- " + rare_anz + "x rare");
        }

        protected override void updateAtStop()
        {
            main.getInstance().updateNextBot();
        }

        protected override bool isActiveInMain()
        {
            return main.getInstance().isnextBotChecked();
        }

        protected override void moreStartOptions()
        {
            death = 0;
            open_anz = 0;
            closed_anz = 0;
            set_anz = 0;
            legendary_anz = 0;
            rare_anz = 0;
            rep_anz = 0;

            time_open_runs = new TimeSpan(0, 0, 0);
            time_closed_runs = new TimeSpan(0, 0, 0);
            time_rep_runs = new TimeSpan(0, 0, 0);

            updateAtmain(0);
        }

        private void updateAtmain(int run_number)
        {
            int open_in_procent = 100 * open_anz / run_number;
            int tmp_rep = 0;
            try
            {
                tmp_rep = (int)time_rep_runs.TotalSeconds / (run_number - closed_anz - open_anz);
            }
            catch { }
            int tmp_closed = 0;
            try
            {
                tmp_closed = (int)time_closed_runs.TotalSeconds / closed_anz;
            }
            catch { }

            int tmp_open = 0;
            try
            {
                tmp_open = (int)time_open_runs.TotalSeconds / open_anz;
            }
            catch { }

            main.getInstance().updateDankBot(run_number, open_in_procent, death, set_anz, legendary_anz, rare_anz, -1, tmp_open, tmp_closed, tmp_rep);
        }
        protected override bool route(ref Random rnd, int run_time, int run_number)
        {
            DateTime start = DateTime.Now;

            farmRoute(ref rnd, run_time, run_number);

            TimeSpan run_lasted = DateTime.Now - start;

            switch (run_was)
            {
                case possibleRuns.closed:
                    time_closed_runs += run_lasted;
                    break;
                case possibleRuns.open:
                    time_open_runs += run_lasted;
                    break;
                case possibleRuns.rep:
                    time_rep_runs += run_lasted;
                    break;
            }

            if (GameStateChecker.getInstance().current_game_state.ingame_flags.inTown)
            {
                if (main.getInstance().getAutoRep() && GameStateChecker.getInstance().current_game_state.ingame_flags.needRep)
                {
                    writeToMainLog("Going to repair at run " + run_number + "!");
                    repRoute(ref rnd, run_time);
                }
            }

            updateAtmain(run_number);

            return true;
        }

        protected override void storeRoute(ref Random rnd, int run_number)
        {
            if (!GameStateChecker.getInstance().current_game_state.ingame_flags.inTown)
                return;

            run_was = possibleRuns.invalid;
            Tools.LeftClick(rnd.Next(452, 480), rnd.Next(150, 185), true, 100, 200, true);

            //3 sec max waiting for stash open
            TimeSpan max = new TimeSpan(0, 0, 3);
            DateTime start = DateTime.Now;
            for (int i = 0; !GameStateChecker.getInstance().current_game_state.ingame_flags.isStashOpen; i++)
            {
                if (DateTime.Now - start >= max)
                    return;

                System.Threading.Thread.Sleep(500);    
            }
            int free_slots = 0;
            if ((free_slots=D3InventoryStuff.getInstance().getNumberOfFreeInventorySlots(module_name, false)) <= 20)
            {
                int clicks = D3InventoryStuff.getInstance().rightClickToAllFullPositions(module_name);
                writeToMainLog(free_slots + " free slots were left. Stored items to stash with " + clicks + " clicks at inventory.");
                finished_store_runs++;
                this.free_slots = D3InventoryStuff.getInstance().getNumberOfFreeInventorySlots(module_name, true);
            }
            else
                this.free_slots = free_slots;

            return;
        }

        private bool repRoute(ref Random rnd, int run_time)
        {
            //first run click
            Tools.LeftClick(rnd.Next(777, 781), rnd.Next(66, 70), false);
            //vendor click
            Tools.LeftClick(rnd.Next(407, 411), rnd.Next(126, 130), true, run_time * 15, (run_time * 15) + 500, true);


            //at vendor
            //click rep
            Tools.LeftClick(rnd.Next(283, 289), rnd.Next(296, 312), true, run_time * 5, (run_time * 5) + 500);
            Tools.LeftClick(rnd.Next(92, 211), rnd.Next(352, 360), true, 1000, 1500);

            run_was = possibleRuns.rep;
            rep_anz++;
            return true;
        }

        static public bool isCellarOpen()
        {
            List<Point> points = Tools.checkImageToD3(new Bitmap(Image.FromFile("./config/Image/DankCellar/CellarAtMinimap.png")), 1, 1, 1, 75, 90, 0, 30, 20);

            if (points.Count != 1)
                return false;
            else
                return true;
        }

        private bool inCellar()
        {
            List<Point> points = Tools.checkImageToD3(new Bitmap(Image.FromFile("./config/Image/DankCellar/KlammerKeller.png")), 1, 1, 1, 75, 100, 0, 20);

            if (points.Count != 1)
                return false;
            else
                return true;
        }

        private bool isEnemy()
        {                                                                                                                   //7, to avoid mercant recognition
            List<Point> points = Tools.checkImageToD3(new Bitmap(Image.FromFile("./config/Image/Common/enemy.png")), 1, 1, 1, 7, 50, 0, 50, 5);

            if (points.Count != 1)
                return false;
            else
                return true;
        }

        int failed_to_enter_cellar = 0;

        private bool farmRoute(ref Random rnd, int run_time, int run_number)
        {
            /*
            Point item_watch_area = new Point(20, 80);
            List<Point> p;
            for (int i = 0; ; i++)
            {
                p = Tools.checkImageToD3(new Bitmap(Image.FromFile("./config/Image/Common/set.png")), 1, 1, 1, item_watch_area.X, item_watch_area.Y, item_watch_area.X, item_watch_area.Y, 5);
                if (p.Count == 1)
                {
                    writeToMainLog(i + "YES!");
                }
                else
                    writeToMainLog(i + "NO!");

                System.Threading.Thread.Sleep(500);
            }
            /* testing image recognition
            */

            int rt_mult = 5;

            Tools.clickSkill4();
            Tools.clickSkill1();
            Tools.middleClick(rnd.Next(25, 29), rnd.Next(230, 234));

            Tools.middleClick(rnd.Next(25, 29), rnd.Next(230, 234), true,  run_time * rt_mult, run_time * rt_mult + 200);
            Tools.clickSkill1(true, run_time * rt_mult, run_time * rt_mult + 200);
            Tools.middleClick(rnd.Next(25, 29), rnd.Next(230, 234));

            Tools.clickSkill2(true, run_time * rt_mult, run_time * rt_mult + 200);
            Tools.clickSkill3();

            if(!isCellarOpen())
            {
                Tools.middleClick(rnd.Next(570, 580), rnd.Next(540, 550), false);

                Tools.clickSkill1();
                Tools.clickSkill2(true, run_time * (rt_mult - 3), run_time * (rt_mult-3) + 200);

                Tools.portToTown(ref rnd, module_name);
                run_was = possibleRuns.closed;
                closed_anz++;
                return true;
            }

            //click to enter
            Tools.LeftClick(rnd.Next(114, 118), rnd.Next(163, 167), false, 50, 100, true);
            Tools.clickSkill1();

            int max_wait = 4;
            for (int i = 0; !inCellar(); i++)
            {
                System.Threading.Thread.Sleep(1000);
                if (i >= max_wait)
                {
                    writeToMainLog("Failed to enter cellar (" + (++failed_to_enter_cellar) + ")");
                    run_was = possibleRuns.closed;
                    closed_anz++;
                    return true;
                }
            }


            //in the cellar
            run_was = possibleRuns.open;
            open_anz++;

            Tools.middleClick(rnd.Next(96, 100), rnd.Next(502, 506));
            Tools.middleClick(rnd.Next(243, 247), rnd.Next(285, 289), true, run_time * rt_mult, run_time * rt_mult + 200);


            Tools.clickSkill2(true, 800, 1000);
            Tools.rightClick(rnd.Next(174, 178), rnd.Next(104, 108), false);

            Point cursor = Tools.rightClickHold(rnd.Next(174, 178), rnd.Next(104, 108));
            //attack for 2 sec
            System.Threading.Thread.Sleep(2000);

            //now attack as long there are enemys (max 10 sec)

            TimeSpan max = new TimeSpan(0, 0, 8);
            DateTime start = DateTime.Now;
            for (int no_enemy_counter = 0; no_enemy_counter < 5;)
            {
                System.Threading.Thread.Sleep(100);
                if (!isEnemy())
                    no_enemy_counter++;
                else
                    no_enemy_counter = 0;

                if (DateTime.Now - start >= max)
                    break;
            }
            Tools.rightClickHoldOff(cursor.X, cursor.Y, false);

            //loot the shit
            loot(ref rnd, run_time, rt_mult);

            Tools.portToTown(ref rnd, module_name);

            return true;
        }

        private void showItemsOnTheFloor()
        {
            Tools.PressKey(System.Windows.Forms.Keys.Menu);
        }

        public void loot(ref Random rnd, int run_time, int rt_mult)
        {
            Tools.clickSkill1(false);
            Tools.middleClick(rnd.Next(130, 140), rnd.Next(130, 140));
            showItemsOnTheFloor();
            System.Threading.Thread.Sleep(run_time * (rt_mult+2));

            Point item_watch_area = new Point(20, 80);

            List<Point> p;

            if (main.getInstance().getLootSet())
            {
                for (int i = 0; i < 2; i++)
                {                                                                                                                                                         //70, to avoind green point of skill cost
                    p = Tools.checkImageToD3(new Bitmap(Image.FromFile("./config/Image/Common/set.png")), 1, 1, 1, item_watch_area.X, item_watch_area.Y, item_watch_area.X, item_watch_area.Y, 5);
                    if (p.Count == 1)
                    {
                        set_anz++;
                        writeToMainLog("Picking up set!" + p.ElementAt(0));
                        Tools.LeftClick(p.ElementAt(0).X, p.ElementAt(0).Y, true, 100, 200, true);
                        System.Threading.Thread.Sleep(1000);
                    }
                    else
                        break;
                }
                showItemsOnTheFloor();
            }
            
            if (main.getInstance().getLootLegendary())
            {        
                for (int i = 0; i < 2; i++)
                {                                                                                              //to avaid recognizing the light as legendary
                    p = Tools.checkImageToD3(new Bitmap(Image.FromFile("./config/Image/Common/legendary.png")), 1, 1, 1, 30, item_watch_area.Y, item_watch_area.X, item_watch_area.Y, 2);
                    if (p.Count == 1)
                    {
                        legendary_anz++;
                        writeToMainLog("Picking up legendary!");
                        Tools.LeftClick(p.ElementAt(0).X, p.ElementAt(0).Y, true, 100, 200, true);
                        System.Threading.Thread.Sleep(1000);
                    }
                    else
                        break;
                }
                showItemsOnTheFloor();
            }

            if (main.getInstance().getLootRare())
            {
                for (int i = 0; i < 2; i++)
                {
                    p = Tools.checkImageToD3(new Bitmap(Image.FromFile("./config/Image/Common/rare.png")), 1, 1, 1, item_watch_area.X, item_watch_area.Y, item_watch_area.X, item_watch_area.Y, 5);
                    if (p.Count == 1)
                    {
                        rare_anz++;
                        writeToMainLog("Picking up rare!");
                        Tools.LeftClick(p.ElementAt(0).X, p.ElementAt(0).Y, true, 100, 200, true);
                        System.Threading.Thread.Sleep(1000);
                    }
                    else
                        break;
                }
                showItemsOnTheFloor();
            }
        }
    }
}
