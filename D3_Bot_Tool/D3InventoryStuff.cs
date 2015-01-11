using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace D3_Bot_Tool
{
    class D3InventoryStuff
    {
        static private int max_slot_x = 10;
        static private int max_slot_y = 6;
        private int slot_number = 0;
        static private int x_step = 26;
        static private int y_step = 25;
        static private String file_name = "config/inv_empty_pixel_colors";
        private Color[,] inventory_free_colors;

        static private D3InventoryStuff instance = null;
        static public D3InventoryStuff getInstance()
        {
            if (instance == null)
                instance = new D3InventoryStuff();
            return instance;
        }

        private D3InventoryStuff()
        {
            inventory_free_colors = loadEmtyinvColorsFromFile();
            slot_number = max_slot_x * max_slot_y;
        }

        public bool isFull(Point slot, LockedFastImage D3Shot)
        {
            
            return !(inv_getColorOf(slot, D3Shot).Name == inventory_free_colors[slot.X, slot.Y].Name);
        }

        static public void writeEmtyInvColors()
        {
            LockedFastImage d3Shot = new LockedFastImage(Tools.D3ScreenShot());

            String to_file = "";
            for (int y = 0; y < max_slot_y; y++)
            {
                for (int x = 0; x < max_slot_x; x++)
                    to_file += inv_getColorOf(new Point(x, y), d3Shot).Name + " ";
                to_file += Environment.NewLine;
            }

            System.IO.File.WriteAllText(file_name, to_file);
        }

        private Color[,] loadEmtyinvColorsFromFile()
        {
            if (!System.IO.File.Exists(file_name)) throw new Exception("Inventory file does not exist! Store to trash will fail!");

            String file_content = System.IO.File.ReadAllText(file_name);

            String[] lines = file_content.Split(new String[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            if (lines.Count() != max_slot_y) throw new Exception("Invalid inventory file (fail number of cols) Store to trash will fail!");

            Color[,] ret = new Color[max_slot_x, max_slot_y];

            for (int y = 0; y < lines.Count(); y++)
            {
                String[] rows = lines[y].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (rows.Count() != max_slot_x) throw new Exception("Invalid inventory file (fail number of rows) Store to trash will fail!");

                for (int x = 0; x < rows.Count(); x++)
                    ret[x, y] = Color.FromName(rows[x]);
            }

            return ret;
        }

        static private Color inv_getColorOf(Point slot, LockedFastImage D3ScreenShot)
        {
            if (slot.X < 0 || slot.X > max_slot_x-1) throw new Exception("Invalid x parameter");
            if (slot.Y < 0 || slot.Y > max_slot_y-1) throw new Exception("Invalid y parameter");

            return D3ScreenShot[getMiddleCoordsOfSlot(slot)];
        }

        static public Point getMiddleCoordsOfSlot(Point slot)
        {
            if (slot.X < 0 || slot.X > max_slot_x - 1) throw new Exception("Invalid x parameter");
            if (slot.Y < 0 || slot.Y > max_slot_y - 1) throw new Exception("Invalid y parameter");


            Point initial_pixel = new Point(539, 367);
            return new Point(initial_pixel.X + x_step * slot.X, initial_pixel.Y + y_step * slot.Y);
        }

        public int leftClickOnSlot(Point slot, bool random_sleep = true, int lowest_sleep = 100, int highest_sleep = 200)
        {
            Point click = getMiddleCoordsOfSlot(slot);
            return Tools.LeftClick(click.X, click.Y, random_sleep, lowest_sleep, highest_sleep);
        }

        public int rightClickOnSlot(Point slot, bool random_sleep = true, int lowest_sleep = 100, int highest_sleep = 200)
        {
            Point click = getMiddleCoordsOfSlot(slot);
            return Tools.rightClick(click.X, click.Y, random_sleep, lowest_sleep, highest_sleep);
        }

        public int rightClickToAllFullPositions(String module_name, bool random_sleep = true, int lowest_sleep = 100, int highest_sleep = 200)
        {
            if (!GameStateChecker.getInstance().current_game_state.ingame_flags.isinventoryOpen)
            {
                main.getInstance().writeToLog(module_name, "Can't right click on inventory. Inventory not open!");
                return -1;
            }

            LockedFastImage l = new LockedFastImage(Tools.D3ScreenShot());
            int clicks = 0;
            for (int x = 0; x < 10; x++)
                for (int y = 0; y < 6; y++)
                {
                    Point slot = new Point(x, y);
                    if (isFull(slot, l))
                    {
                        clicks++;
                        rightClickOnSlot(slot, random_sleep, lowest_sleep, highest_sleep);
                    }
                }

            return clicks;
        }

        public int getNumberOfFreeInventorySlots(String module_name, bool random_sleep = true, int lowest_sleep = 100, int highest_sleep = 200)
        {
            if (!GameStateChecker.getInstance().current_game_state.ingame_flags.isinventoryOpen)
            {
                main.getInstance().writeToLog(module_name, "Can't right click on inventory. Inventory not open!");
                return -1;
            }

            LockedFastImage l = new LockedFastImage(Tools.D3ScreenShot());
            int count = 0;
            for (int x = 0; x < 10; x++)
                for (int y = 0; y < 6; y++)
                {
                    Point slot = new Point(x, y);
                    if (isFull(slot, l))
                        count++;
                }

            return slot_number - count;
        }

        public int getGold(String module_name)
        {
            GameState game_state = GameStateChecker.getInstance().current_game_state;

            if (game_state.game_state != GameState.GameStates.InGame || game_state.ingame_flags.isDead)
                return -1;

            if (game_state.ingame_flags.isinventoryOpen)
                return readGoldFromInventory(module_name, game_state);

            if ((game_state = openInventory()).game_state != GameState.GameStates.Unknown)
            {
                int ret = readGoldFromInventory(module_name, game_state);
                closeInventory();
                return ret;
            }
            else
                return -1;         
        }

        public void closeInventory()
        {
            Tools.clickInventory();
        }

        public GameState openInventory()
        {
            GameState game_state;
            if ((game_state = GameStateChecker.getInstance().current_game_state).ingame_flags.isinventoryOpen)
                return game_state;

            if (GameStateChecker.getInstance().current_game_state.game_state != GameState.GameStates.InGame)
            {
                game_state.game_state = GameState.GameStates.Unknown;
                return game_state;
            }

            Tools.clickInventory();

            DateTime start = DateTime.Now;
            for (; !((game_state = new GameState()).ingame_flags.isinventoryOpen); )
            {
                
                System.Threading.Thread.Sleep(50);

                if ((DateTime.Now - start).TotalMilliseconds >= 2000)
                {
                    game_state.game_state = GameState.GameStates.Unknown;
                    return game_state;
                }
            }

            return game_state;
        }

        public int readGoldFromInventory(String module_name, GameState gamestate)
        {
            try
            {
                if (!gamestate.ingame_flags.isinventoryOpen)
                {
                    main.getInstance().writeToLog(module_name, "Can't read gold. Inventory not open!");
                    return -1;
                }

                LockedFastImage d3Shot = new LockedFastImage(Tools.D3ScreenShot());

                List<GoldHelpClass> list = new List<GoldHelpClass>();
                int number_fail = 10;
                //read 0
                List<Point> p = Tools.checkImageToD3(new Bitmap(Image.FromFile("./config/Image/Common/gold_0.png")), 99, 1, 1, 68, 78, 48, 54, number_fail, d3Shot);
                foreach (Point tmp in p)
                    list.Add(new GoldHelpClass(tmp, 0));

                //read 1
                p = Tools.checkImageToD3(new Bitmap(Image.FromFile("./config/Image/Common/gold_1.png")), 99, 1, 1, 68, 78, 48, 54, number_fail, d3Shot);
                foreach (Point tmp in p)
                    list.Add(new GoldHelpClass(tmp, 1));

                //read 2
                p = Tools.checkImageToD3(new Bitmap(Image.FromFile("./config/Image/Common/gold_2.png")), 99, 1, 1, 68, 78, 48, 54, number_fail, d3Shot);
                foreach (Point tmp in p)
                    list.Add(new GoldHelpClass(tmp, 2));

                //read 3
                p = Tools.checkImageToD3(new Bitmap(Image.FromFile("./config/Image/Common/gold_3.png")), 99, 1, 1, 68, 78, 48, 54, number_fail, d3Shot);
                foreach (Point tmp in p)
                    list.Add(new GoldHelpClass(tmp, 3));

                //read 4
                p = Tools.checkImageToD3(new Bitmap(Image.FromFile("./config/Image/Common/gold_4.png")), 99, 1, 1, 68, 78, 48, 54, number_fail, d3Shot);
                foreach (Point tmp in p)
                    list.Add(new GoldHelpClass(tmp, 4));

                //read 5
                p = Tools.checkImageToD3(new Bitmap(Image.FromFile("./config/Image/Common/gold_5.png")), 99, 1, 1, 68, 78, 48, 54, number_fail, d3Shot);
                foreach (Point tmp in p)
                    list.Add(new GoldHelpClass(tmp, 5));

                //read 6
                p = Tools.checkImageToD3(new Bitmap(Image.FromFile("./config/Image/Common/gold_6.png")), 99, 1, 1, 68, 78, 48, 54, number_fail, d3Shot);
                foreach (Point tmp in p)
                    list.Add(new GoldHelpClass(tmp, 6));

                //read 7
                p = Tools.checkImageToD3(new Bitmap(Image.FromFile("./config/Image/Common/gold_7.png")), 99, 1, 1, 68, 78, 48, 54, number_fail, d3Shot);
                foreach (Point tmp in p)
                    list.Add(new GoldHelpClass(tmp, 7));

                //read 8
                p = Tools.checkImageToD3(new Bitmap(Image.FromFile("./config/Image/Common/gold_8.png")), 99, 1, 1, 68, 78, 48, 54, number_fail, d3Shot);
                foreach (Point tmp in p)
                    list.Add(new GoldHelpClass(tmp, 8));

                //read 9
                p = Tools.checkImageToD3(new Bitmap(Image.FromFile("./config/Image/Common/gold_9.png")), 99, 1, 1, 68, 78, 48, 54, number_fail, d3Shot);
                foreach (Point tmp in p)
                    list.Add(new GoldHelpClass(tmp, 9));

                return convertGoldHelpClassListToInt(list);
            }
            catch { return -1; }
        }

        int convertGoldHelpClassListToInt(List<GoldHelpClass> list)
        {
            //insertion sort list
            List<int> int_list = new List<int>();
            
            while(list.Count() > 0)
            {
                int highest_index = 0;
                int highest_x = 0;

                for (int i = 0; i < list.Count(); i++)
                    if (list[i].location.X > highest_x)
                    {
                        highest_x = list[i].location.X;
                        highest_index = i;
                    }

                int_list.Add(list[highest_index].number);
                list.RemoveAt(highest_index);
            }

            int value = 0;
            for (int i = 0; i < int_list.Count(); i++)
                value += int_list[i] * (int)(Math.Pow(10, i));

            return value;
        }

        public int getNumberOfSlots()
        {
            return slot_number;
        }
    }

    class GoldHelpClass
    {
        public Point location;
        public int number;
        public GoldHelpClass(Point location, int number)
        {
            this.location = location;
            this.number = number;
        }
    }
}
