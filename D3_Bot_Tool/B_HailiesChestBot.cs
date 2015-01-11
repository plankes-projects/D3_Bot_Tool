using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace D3_Bot_Tool
{
    class B_HailiesChestBot : B_BaseBot
    {
        
        public B_HailiesChestBot() : base("HailiesChestBot")
        {
            print_run_each = 20;
        }

        protected override void writeToLogAtEnd(int run_number)
        {
        }


        protected override void storeRoute(ref Random rnd, int run_number)
        { }

        protected override void updateAtStop()
        {
            main.getInstance().updateHailieChestBot();
        }

        protected override bool isActiveInMain()
        {
            return main.getInstance().isHailieChecked();
        }

        protected override void moreStartOptions()
        {

        }

        protected override bool route(ref Random rnd, int run_time, int run_number)
        {
            int route = rnd.Next(0, 2);
            if (route == 0)
                route1(ref rnd, run_time);
            else if (route == 1)
                route2(ref rnd, run_time);
            else
                route3(ref rnd, run_time);

            return true;
        }

        private void useDHSKills(ref Random rnd)
        {
            int s = rnd.Next(0, 5);

            switch (s)
            {
                case 0:
                    Tools.clickSkill1(false);
                    Tools.clickSkill3(false);
                    Tools.clickSkill4(false);
                    break;

                case 1:
                    Tools.clickSkill1(false);
                    Tools.clickSkill4(false);
                    Tools.clickSkill3(false);
                    break;

                case 2:
                    Tools.clickSkill3(false);
                    Tools.clickSkill1(false);
                    Tools.clickSkill4(false);
                    break;

                case 3:
                    Tools.clickSkill3(false);
                    Tools.clickSkill4(false);
                    Tools.clickSkill1(false);
                    break;

                case 4:
                    Tools.clickSkill4(false);
                    Tools.clickSkill1(false);
                    Tools.clickSkill3(false);
                    break;

                case 5:
                    Tools.clickSkill4(false);
                    Tools.clickSkill3(false);
                    Tools.clickSkill1(false);
                    break;
            }
        }

        private void route1(ref Random rnd, int run_time)
        {
            int big_run_time_max = run_time + 200;
            bool dh = main.getInstance().isDaemonHunter();

            //first run click
            Tools.LeftClick(rnd.Next(98, 102), rnd.Next(168, 172), false);
            if (dh)
                useDHSKills(ref rnd);

            //second run click
            Tools.LeftClick(rnd.Next(33, 37), rnd.Next(263, 267), true, run_time, big_run_time_max);
            if (dh)
                Tools.clickSkill2(false);

            //click the chest
            Tools.LeftClick(rnd.Next(160, 164), rnd.Next(268, 272), true, run_time, big_run_time_max, true);
            
            //pickup gold
            Tools.LeftClick(rnd.Next(312, 359), rnd.Next(264, 334), true, run_time, big_run_time_max);
        }

        private void route2(ref Random rnd, int run_time)
        {
            int big_run_time_max = run_time + 200;
            bool dh = main.getInstance().isDaemonHunter();

            //first run click
            Tools.LeftClick(rnd.Next(18, 22), rnd.Next(198, 202), false);
            if (dh)
                useDHSKills(ref rnd);

            //second run click
            Tools.LeftClick(rnd.Next(28, 32), rnd.Next(238, 242), true, run_time, big_run_time_max);
            if (dh)
                Tools.clickSkill2(false);

            //click the chest
            Tools.LeftClick(rnd.Next(253, 257), rnd.Next(253, 257), true, run_time, big_run_time_max, true);

            //pickup gold
            Tools.LeftClick(rnd.Next(312, 359), rnd.Next(264, 334), true, run_time, big_run_time_max);
        }

        private void route3(ref Random rnd, int run_time)
        {
            int big_run_time_max = run_time + 200;
            bool dh = main.getInstance().isDaemonHunter();

            //first run click
            Tools.LeftClick(rnd.Next(48, 52), rnd.Next(163, 167), false);
            if (dh)
                useDHSKills(ref rnd);

            //second run click
            Tools.LeftClick(rnd.Next(28, 32), rnd.Next(288, 292), true, run_time, big_run_time_max);
            if (dh)
                Tools.clickSkill2(false);

            //click the chest
            Tools.LeftClick(rnd.Next(218, 222), rnd.Next(248, 252), true, run_time, big_run_time_max, true);

            //pickup gold
            Tools.LeftClick(rnd.Next(312, 359), rnd.Next(264, 334), true, run_time, big_run_time_max);
        }
    }
}
