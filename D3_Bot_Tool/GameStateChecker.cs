using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace D3_Bot_Tool
{
    class GameStateChecker
    {
        private System.ComponentModel.BackgroundWorker bw;
        private bool running = false;

        private GameState current_state = new GameState();
        public GameState current_game_state
        {
            get
            {
                return current_state;
            }
        }

        static public GameStateChecker instance = null;
        static public GameStateChecker getInstance()
        {
            if (instance == null)
                instance = new GameStateChecker();

            return instance;
        }

        private GameStateChecker()
        {
            bw = new System.ComponentModel.BackgroundWorker();
            bw.DoWork += new System.ComponentModel.DoWorkEventHandler(bw_DoWork);
        }

        private void Run()
        {
            int checks = 0;
            Circuit c = new Circuit(200);
            int min_loop_time = 50;
            int sleep_time = 0;

            TimeSpan needed_time = new TimeSpan(0, 0, 0);
            DateTime start;

            while (running)
            {
                start = DateTime.Now;
                sleep_time = min_loop_time - (int)(needed_time.TotalMilliseconds);
                if (sleep_time > 0)
                    System.Threading.Thread.Sleep(sleep_time);

                changeGameStateTo(new GameState());
                checks++;
                c.add(checks);

                main.getInstance().updateGamestate(current_state.ToString() + Environment.NewLine + Environment.NewLine + "Checks per seconds: " + c.getAnzPerSecond() + " over " + c.overSeconds() + " seconds." + Environment.NewLine + "Sleeptime: " + sleep_time + " ms.");
                needed_time = DateTime.Now - start;
            }
        }

        private void changeGameStateTo(GameState state)
        {
            if (current_state == state)
                return;

            if (stateChangedEvent != null)
                stateChangedEvent(this, new GameStateChangedEventArgs(current_state, state));

            current_state = state;
        }

        private void bw_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            Run();
        }

        public void start()
        {
            running = true;
            if (!bw.IsBusy)
            {
                bw.RunWorkerAsync();
            }
        }

        public void stop()
        {
            running = false;
        }

        public delegate void GameStateChangedHandler(GameStateChecker game_state_checker, GameStateChangedEventArgs e);
        public event GameStateChangedHandler stateChangedEvent;
    }


    public class GameStateChangedEventArgs : EventArgs
    {
        private GameState old_state;
        private GameState new_state;

        public GameStateChangedEventArgs(GameState old_state, GameState new_state)
        {
            this.old_state = old_state;
            this.new_state = new_state;
        }

        public GameState OldState
        {
            get { return old_state; }
        }

        public GameState NewState
        {
            get { return new_state; }
        }
    }

    class Circuit
    {
        public struct data
        {
            public DateTime stamp;
            public int current;
        }

        public Circuit(int max)
        {
            max_ = max;
        }

        private int max_ = 0;
        private List<data> list_ = new List<data>();
        public void add(int current)
        {
            data d = new data();
            d.current = current;
            d.stamp = DateTime.Now;

            list_.Add(d);

            if (list_.Count > max_)
                list_.RemoveAt(0);
        }

        public int getAnzPerSecond()
        {
            try
            {
                TimeSpan dauer = list_.ElementAt(list_.Count - 1).stamp - list_.ElementAt(0).stamp;
                int checks = list_.ElementAt(list_.Count - 1).current - list_.ElementAt(0).current;

                return checks / (int)(dauer.TotalSeconds);
            }
            catch { return 0; }
        }

        public int overSeconds()
        {
            try
            {
                return (int)((list_.ElementAt(list_.Count - 1).stamp - list_.ElementAt(0).stamp).TotalSeconds);
            }
            catch { return 0; }
        }
    }

    
}
