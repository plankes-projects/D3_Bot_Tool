using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace D3_Bot_Tool
{
    class B_levelBot : B_BaseBot
    {

        public B_levelBot()
            : base("LevelBot")
        {
            print_run_each = 5;
        }

        protected override void writeToLogAtEnd(int run_number)
        {
        }

        protected override void updateAtStop()
        {
            main.getInstance().updateLevelBot();
        }

        protected override bool isActiveInMain()
        {
            return main.getInstance().isLevelChecked();
        }

        protected override void moreStartOptions()
        {

        }

        protected override bool route(ref Random rnd, int run_time, int run_number)
        {
            int speed = 600;
            int run_mult = 5;
            int run_mult_2 = 4;
            Tools.clickSkill2();
            Tools.clickSkill3();

            Tools.middleClick(rnd.Next(58, 62), rnd.Next(108, 112), false);
            for(int i= 0; i < 2; i++)
                Tools.middleClick(rnd.Next(58, 62), rnd.Next(108, 112), true, speed * run_mult, speed * run_mult);

            //enter mennor
            Tools.LeftClick(rnd.Next(408, 412), rnd.Next(258, 262), true, speed * run_mult, speed * run_mult, true);


            System.Threading.Thread.Sleep(1000);

            Tools.middleClick(rnd.Next(146, 150), rnd.Next(111, 115), false);
            Tools.middleClick(rnd.Next(146, 150), rnd.Next(111, 115), true, speed * run_mult_2, speed * run_mult_2);

            Point cursor = Tools.rightClickHold(rnd.Next(176, 180), rnd.Next(141, 145), true, speed * run_mult_2, speed * run_mult_2);

            //we will attack 4 seconds... without checks
            System.Threading.Thread.Sleep(4000);

            TimeSpan max = new TimeSpan(0, 0, 60);
            DateTime start = DateTime.Now;
            for (int no_enemy_counter = 0; no_enemy_counter < 5; )
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

            Tools.clickSkill2();
            System.Threading.Thread.Sleep(1000);
            Tools.portToTown(ref rnd, module_name);

            if (GameStateChecker.getInstance().current_game_state.ingame_flags.inTown)
            {
                if (main.getInstance().getAutoRep() && GameStateChecker.getInstance().current_game_state.ingame_flags.needRep)
                {
                    writeToMainLog("Going to repair at run " + run_number + "!");
                    repRoute(ref rnd, run_time);
                }
            }

            return true;
        }

        private bool isEnemy()
        {                                                                                                                   //7, to avoid mercant recognition
            List<Point> points = Tools.checkImageToD3(new Bitmap(Image.FromFile("./config/Image/Common/enemy.png")), 1, 1, 1, 20, 80, 0, 70, 5);

            if (points.Count != 1)
                return false;
            else
                return true;
        }



        protected override void storeRoute(ref Random rnd, int run_number)
        {
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

            return true;
        }
    }
}
