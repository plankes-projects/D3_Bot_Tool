using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;

namespace D3_Bot_Tool
{
    class Tools
    {
        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags,
           UIntPtr dwExtraInfo);
        
        static Random random = new Random();

        static public int PressKey(Keys key, bool random_sleep = true, int lowest_sleep = 100, int highes_sleep = 200)
        {
            int sleep = 0;
            if (random_sleep)
            {
                sleep = random.Next(lowest_sleep, highes_sleep);
                System.Threading.Thread.Sleep(sleep);
            }

            const int KEYEVENTF_EXTENDEDKEY = 0x1;
            const int KEYEVENTF_KEYUP = 0x2;

            keybd_event((byte)key, 0x45, KEYEVENTF_EXTENDEDKEY, (UIntPtr)0);
            System.Threading.Thread.Sleep(random.Next(5, 10));
            keybd_event((byte)key, 0x45, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, (UIntPtr)0);

            return sleep;
        }

        static public int PressKeyHold(Keys key, bool random_sleep = true, int lowest_sleep = 100, int highes_sleep = 200)
        {
            int sleep = 0;
            if (random_sleep)
            {
                sleep = random.Next(lowest_sleep, highes_sleep);
                System.Threading.Thread.Sleep(sleep);
            }

            const int KEYEVENTF_EXTENDEDKEY = 0x1;

            keybd_event((byte)key, 0x45, KEYEVENTF_EXTENDEDKEY, (UIntPtr)0);

            return sleep;
        }

        static public int PressKeyRelease(Keys key, bool random_sleep = true, int lowest_sleep = 100, int highes_sleep = 200)
        {
            int sleep = 0;
            if (random_sleep)
            {
                sleep = random.Next(lowest_sleep, highes_sleep);
                System.Threading.Thread.Sleep(sleep);
            }

            const int KEYEVENTF_KEYUP = 0x2;

            keybd_event((byte)key, 0x45, KEYEVENTF_KEYUP, (UIntPtr)0);

            return sleep;
        }

        static public int Press2Keys(Keys mod, Keys key, bool random_sleep = true, int lowest_sleep = 100, int highes_sleep = 200)
        {
            int sleep = 0;
            if (random_sleep)
            {
                sleep = random.Next(lowest_sleep, highes_sleep);
                System.Threading.Thread.Sleep(sleep);
            }

            const int KEYEVENTF_EXTENDEDKEY = 0x1;
            const int KEYEVENTF_KEYUP = 0x2;

            keybd_event((byte)mod, 0x45, KEYEVENTF_EXTENDEDKEY, (UIntPtr)0);
            System.Threading.Thread.Sleep(random.Next(5, 10));
            keybd_event((byte)key, 0x45, KEYEVENTF_EXTENDEDKEY, (UIntPtr)0);
            System.Threading.Thread.Sleep(random.Next(5, 10));

            keybd_event((byte)key, 0x45, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, (UIntPtr)0);
            System.Threading.Thread.Sleep(random.Next(5, 10));
            keybd_event((byte)mod, 0x45, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, (UIntPtr)0);

            return sleep;
        }

        static public String[] getAllCommands(String txt, String command)
        {
            List<String> list = new List<string>();

            try
            {
                int index = txt.IndexOf(command);
                if (index > 0)
                {
                    list.Add(txt.Substring(0, index));
                    txt = txt.Substring(index);
                }

                while ((index = txt.IndexOf(command, 1)) > 0)
                {

                    list.Add(txt.Substring(0, index));
                    txt = txt.Substring(index);
                }

                if (txt.Count() != 0)
                    list.Add(txt);
            }
            catch { };
            return list.ToArray();
        }

        public static Keys ConvertCharToVirtualKey(char ch)
        {
            short vkey = VkKeyScan(ch);
            Keys retval = (Keys)(vkey & 0xff);
            int modifiers = vkey >> 8;
            if ((modifiers & 1) != 0) retval |= Keys.Shift;
            if ((modifiers & 2) != 0) retval |= Keys.Control;
            if ((modifiers & 4) != 0) retval |= Keys.Alt;
            return retval;
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern short VkKeyScan(char ch);

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        static public string GetActiveWindowTitle()
        {
            const int nChars = 256;
            IntPtr handle = IntPtr.Zero;
            StringBuilder Buff = new StringBuilder(nChars);
            handle = GetForegroundWindow();

            if (GetWindowText(handle, Buff, nChars) > 0)
            {
                return Buff.ToString();
            }
            return null;
        }

        [DllImport("user32.dll")]
        static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        public enum MouseEventFlags
        {
            LEFTDOWN = 0x00000002,
            LEFTUP = 0x00000004,
            MIDDLEDOWN = 0x00000020,
            MIDDLEUP = 0x00000040,
            MOVE = 0x00000001,
            ABSOLUTE = 0x00008000,
            RIGHTDOWN = 0x00000008,
            RIGHTUP = 0x00000010
        }

        [DllImport("user32.dll")]
        static extern IntPtr SetActiveWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        static public int LeftClick(int x, int y, bool random_sleep = true, int lowest_sleep = 100, int highes_sleep = 200, bool action_click = false)
        {
            int sleep = 0;
            int action_click_delay = 100;

            if (random_sleep)
            {
                if(action_click)
                    sleep = random.Next(lowest_sleep - action_click_delay, highes_sleep - action_click_delay);
                else
                    sleep = random.Next(lowest_sleep, highes_sleep);

                if(sleep > 0)
                    System.Threading.Thread.Sleep(sleep);
            }

            Point tmp = Cursor.Position;

            mouse_event((int)(MouseEventFlags.LEFTUP), 0, 0, 0, 0);
            Cursor.Position = ConvertToScreenPixel(new Point(x, y));
            if(action_click)
                System.Threading.Thread.Sleep(action_click_delay);

            mouse_event((int)(MouseEventFlags.LEFTDOWN | MouseEventFlags.LEFTUP), 0, 0, 0, 0);
            //mouse_event((int)(MouseEventFlags.LEFTUP), 0, 0, 0, 0);

            if (action_click)
                System.Threading.Thread.Sleep(50);

            Cursor.Position = tmp;

            return sleep;
        }

        
        static public int clickPortal(bool random_sleep = true, int lowest_sleep = 100, int highest_sleep = 200)
        {
            return Tools.LeftClick(random.Next(492, 501), random.Next(586, 607), random_sleep, lowest_sleep, highest_sleep);
        }

        static public int clickInventory(bool random_sleep = true, int lowest_sleep = 100, int highest_sleep = 200)
        {
            return Tools.LeftClick(random.Next(534, 542), random.Next(586, 607), random_sleep, lowest_sleep, highest_sleep);
        }

        static public int clickSkill1(bool random_sleep = true, int lowest_sleep = 100, int highest_sleep = 200)
        {
            return Tools.LeftClick(random.Next(230, 250), random.Next(586, 607), random_sleep, lowest_sleep, highest_sleep);
        }

        static public int clickSkill2(bool random_sleep = true, int lowest_sleep = 100, int highest_sleep = 200)
        {
            return Tools.LeftClick(random.Next(269, 285), random.Next(586, 607), random_sleep, lowest_sleep, highest_sleep);
        }

        static public int clickSkill3(bool random_sleep = true, int lowest_sleep = 100, int highest_sleep = 200)
        {
            return Tools.LeftClick(random.Next(304, 323), random.Next(586, 607), random_sleep, lowest_sleep, highest_sleep);
        }

        static public int clickSkill4(bool random_sleep = true, int lowest_sleep = 100, int highest_sleep = 200)
        {
            return Tools.LeftClick(random.Next(343, 360), random.Next(586, 607), random_sleep, lowest_sleep, highest_sleep);
        }

        static public int rightClick(int x, int y, bool random_sleep = true, int lowest_sleep = 100, int highes_sleep = 200)
        {
            int sleep = 0;
            if (random_sleep)
            {
                sleep = random.Next(lowest_sleep, highes_sleep);
                System.Threading.Thread.Sleep(sleep);
            }

            Point tmp = Cursor.Position;

            mouse_event((int)(MouseEventFlags.LEFTUP), 0, 0, 0, 0);
            Cursor.Position = ConvertToScreenPixel(new Point(x, y));

            mouse_event((int)(MouseEventFlags.RIGHTDOWN | MouseEventFlags.RIGHTUP), 0, 0, 0, 0);

            Cursor.Position = tmp;

            return sleep;
        }

        static public Point rightClickHold(int x, int y, bool random_sleep = true, int lowest_sleep = 100, int highes_sleep = 200)
        {
            int sleep = 0;
            if (random_sleep)
            {
                sleep = random.Next(lowest_sleep, highes_sleep);
                System.Threading.Thread.Sleep(sleep);
            }

            Point tmp = Cursor.Position;

            mouse_event((int)(MouseEventFlags.LEFTUP), 0, 0, 0, 0);
            Cursor.Position = ConvertToScreenPixel(new Point(x, y));

            mouse_event((int)MouseEventFlags.RIGHTDOWN, 0, 0, 0, 0);

            return tmp;
        }
        static public int rightClickHoldOff(int x, int y, bool random_sleep = true, int lowest_sleep = 100, int highes_sleep = 200)
        {
            int sleep = 0;
            if (random_sleep)
            {
                sleep = random.Next(lowest_sleep, highes_sleep);
                System.Threading.Thread.Sleep(sleep);
            }

            Point tmp = Cursor.Position;

            mouse_event((int)(MouseEventFlags.LEFTUP), 0, 0, 0, 0);
            Cursor.Position = ConvertToScreenPixel(new Point(x, y));

            mouse_event((int)MouseEventFlags.RIGHTUP, 0, 0, 0, 0);

            return sleep;
        }

        static public int middleClick(int x, int y, bool random_sleep = true, int lowest_sleep = 100, int highes_sleep = 200)
        {
            int sleep = 0;
            if (random_sleep)
            {
                sleep = random.Next(lowest_sleep, highes_sleep);
                System.Threading.Thread.Sleep(sleep);
            }

            Point tmp = Cursor.Position;

            mouse_event((int)(MouseEventFlags.LEFTUP), 0, 0, 0, 0);
            Cursor.Position = ConvertToScreenPixel(new Point(x, y));

            mouse_event((int)(MouseEventFlags.MIDDLEDOWN | MouseEventFlags.MIDDLEUP), 0, 0, 0, 0);

            Cursor.Position = tmp;

            return sleep;
        }

        [DllImport("user32.dll")]
        private static extern int GetWindowRect(IntPtr hwnd, out Rectangle rect);
        static public Point ConvertToScreenPixel(Point point)
        {
            Rectangle rect;

            GetWindowRect(D3Stuff.getInstance().getD3WinHandle(), out rect);

            Point ret = new Point();

            ret.X = rect.Location.X + point.X + adjust_bot_point.X;
            ret.Y = rect.Location.Y + point.Y + adjust_bot_point.Y;

            return ret;
        }

        static public bool FindAndKillProcess(string name)
        {
            foreach (System.Diagnostics.Process clsProcess in System.Diagnostics.Process.GetProcesses())
            {
                if (clsProcess.ProcessName.StartsWith(name))
                {
                    clsProcess.Kill();
                    return true;
                }
            }

            return false;
        }

        static public void killDiablo3()
        {
            FindAndKillProcess("Diablo III");
        }

        [DllImport("user32.dll")]
        static extern bool GetCursorPos(ref Point lpPoint);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern int BitBlt(IntPtr hDC, int x, int y, int nWidth, int nHeight, IntPtr hSrcDC, int xSrc, int ySrc, int dwRop);

        static Bitmap screenPixel = new Bitmap(1, 1, System.Drawing.Imaging.PixelFormat.Format32bppArgb);


        static public Point GetCursorPos()
        {
            Point cursor = new Point();
            GetCursorPos(ref cursor);

            return cursor;
        }

        static public bool login(String module_name, int max_trys, bool loginBot = false)
        {
            String pw = main.getInstance().getPW();
            if (pw == "")
            {
                main.getInstance().writeToLog(module_name, "No login password.");
                return false;
            }

            int trys_counter = 0;
            int max_invalid_states = 10; //10 sek invalid
            int invalid_state_counter = 0;

            while ((!loginBot && trys_counter < max_trys) || (loginBot && main.getInstance().isLoginBotChecked()))
            {
                if (GameStateChecker.getInstance().current_game_state.game_state == GameState.GameStates.CharScreen_RedEnterGameButton)
                    return true;

                if (GameStateChecker.getInstance().current_game_state.game_state != GameState.GameStates.LoginScreen)
                {
                    invalid_state_counter++;
                    if (invalid_state_counter >= max_invalid_states)
                        break;

                    System.Threading.Thread.Sleep(1000);
                    continue;
                }

                invalid_state_counter = 0;

                trys_counter++;
                if(loginBot)
                    main.getInstance().writeToLog(module_name, "Login " + trys_counter + ".");
                else
                    main.getInstance().writeToLog(module_name, "Login " + trys_counter + "/" + max_trys + ".");
                //click on pw field
                LeftClick(409, 423);
                if (sendString(pw, true) == false)
                    return false;

                PressKey(Keys.Enter);

                do
                {
                    System.Threading.Thread.Sleep(1000);
                } while (GameStateChecker.getInstance().current_game_state.game_state == GameState.GameStates.LoginLoading);

                System.Threading.Thread.Sleep(1000);
                if (GameStateChecker.getInstance().current_game_state.game_state == GameState.GameStates.CharScreen_RedEnterGameButton)
                    return true;

                PressKey(Keys.Enter);
            }

            return false;
        }



        static public bool sendString(String txt, bool secure_writing)
        {
            System.Threading.Thread.Sleep(50);

            foreach (char key_char in txt)
            {
                if (secure_writing && !(GameStateChecker.getInstance().current_game_state.game_state == GameState.GameStates.LoginScreen && GetActiveWindowTitle() == "Diablo III"))
                    return false;
                PressKey(ConvertCharToVirtualKey(key_char), false);
            }
            return true;
        }

        static public Color GetColorAt(Point location)
        {
            try
            {
                return Tools.D3ScreenShot().GetPixel(location.X, location.Y);
            }
            catch { return new Color(); }
        }

        private const int DOWNCLICK = 0x201;
        private const int UPCLICK = 0x202;
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        static public void simulateMouseClick(int X, int Y)
        {
            IntPtr handle = D3Stuff.getInstance().getD3WinHandle();            
            IntPtr lParam = (IntPtr)((Y << 16) | X);
            IntPtr wParam = IntPtr.Zero;

            PostMessage(handle, DOWNCLICK, wParam, lParam);
            PostMessage(handle, UPCLICK, wParam, lParam);
        }

        static public List<Point> checkImageToD3(Bitmap imageToCheck, int number_of_results = 1, int x_speedUp = 1, int y_speedUp = 1, int begin_percent_x = 0, int end_percent_x = 100, int begin_percent_y = 0, int end_percent_y = 100, int fail = 0, LockedFastImage D3 = null )
        {
            ImageChecker i;

            if (D3 != null)
            {
                i = new ImageChecker(D3, imageToCheck);
            }
            else
            {
                Bitmap b = D3ScreenShot();
                if (b == null)
                    return new List<Point>();
                i = new ImageChecker(b, imageToCheck);
            }
            return i.bigContainsSmall(number_of_results, x_speedUp, y_speedUp, begin_percent_x, end_percent_x, begin_percent_y, end_percent_y, fail);
        }

        static public Bitmap D3ScreenShot()
        {
            Rectangle rect;
            GetWindowRect(D3Stuff.getInstance().getD3WinHandle(), out rect);

            try
            {
                Bitmap b = new Bitmap(rect.Width - rect.X, rect.Height - rect.Y);
                Graphics g = Graphics.FromImage(b);
                g.CopyFromScreen(rect.X, rect.Y, 0, 0, b.Size);
                g.Dispose();
                return b;
            }
            catch 
            {
                #if DEBUG
                    main.getInstance().writeToLog("error", "fail at screenshot!");
                #endif
                System.Threading.Thread.Sleep(500); //to avoid mass message
                return null;
            }
        }

        static public Point adjust_bot_point = new Point(0, 0);

        static public void restartDiablo3(String module_name)
        {
            killDiablo3();
            System.Threading.Thread.Sleep(2000);
            launchDiablo3(module_name);
        }

        const short SWP_NOSIZE = 1;
        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);

        static public void launchDiablo3Async(String module_name)
        {
            //Folgenden Code in eigene Methode einfügen:
            System.Threading.ParameterizedThreadStart pts = new System.Threading.ParameterizedThreadStart(AsyncMethod);
            System.Threading.Thread thread = new System.Threading.Thread(pts);
            thread.Start(module_name);
        }

        static private void AsyncMethod(Object parameter)
        {
            launchDiablo3((String)parameter);
        }

        static public bool launchDiablo3(String module_name)
        {
            if (!main.getInstance().validD3Path())
            {
                main.getInstance().writeToLog(module_name, "Can't start Diablo 3! (No valid exe path)");
                return false;
            }

            System.Diagnostics.Process.Start(main.getInstance().getD3Exepath(), "-launch");

            int max_seconds = main.getInstance().getMaxLaunchWaittime();
            for (int i = 0; i < max_seconds; i++)
            {
                System.Threading.Thread.Sleep(1000);

                try
                {
                    D3Stuff.getInstance().init(true);
                }
                catch { continue; }

                if(D3Stuff.getInstance().getD3WinHandle() != new IntPtr(0))
                    SetWindowPos(D3Stuff.getInstance().getD3WinHandle(), 0, 0, 0, 0, 0, SWP_NOSIZE);

                if (GameStateChecker.getInstance().current_game_state.game_state == GameState.GameStates.LoginScreen)
                {
                    main.getInstance().writeToLog(module_name, "Diablo 3 started!");
                    return true;
                }
            }

            main.getInstance().writeToLog(module_name, "Can't start Diablo 3! (Was waiting " + max_seconds + " seconds)");
            return false;
        }

        static public bool isInGame(LockedFastImage image = null)
        {
            Point pixel = new Point(125, 598);

            Color c;
            if (image != null)
                c = image[pixel];
            else
                c = GetColorAt(pixel);

            return (c.Name == PixelColors.getinstance().isInGame);
        }

        static public bool isloginScreen(LockedFastImage image = null)
        {
            Point pixel = new Point(278, 178);

            Color c;
            if (image != null)
                c = image[pixel];
            else
                c = GetColorAt(pixel);

            return (c.Name == PixelColors.getinstance().isloginScreen);
        }

        static public bool isLoadingScreen(LockedFastImage image = null)
        {
            Point pixel = new Point(450, 562);

            Color c;
            if (image != null)
                c = image[pixel];
            else
                c = GetColorAt(pixel);

            return (c.Name == PixelColors.getinstance().isLoadingScreen);
        }

        static public bool isDisconnectDienst(LockedFastImage image = null)
        {
            Point pixel = new Point(430, 381);

            Color c;
            if (image != null)
                c = image[pixel];
            else
                c = GetColorAt(pixel);

            return (c.Name == PixelColors.getinstance().isDisconnectDienst);
        }

        static public bool isLoginLoading(LockedFastImage image = null)
        {
            Point pixel = new Point(438, 401);

            Color c;
            if (image != null)
                c = image[pixel];
            else
                c = GetColorAt(pixel);

            return (c.Name == PixelColors.getinstance().isLoginLoading);
        }

        static public bool isCharScreen_RedEnterGameButton(LockedFastImage image = null)
        {
            Point pixel = new Point(73, 262);

            Color c;
            if (image != null)
                c = image[pixel];
            else
                c = GetColorAt(pixel);

            return (c.Name == PixelColors.getinstance().isCharScreen_RedEnterGameButton);
        }

        static public bool isCharScreen_GrayEnterGameButton(LockedFastImage image = null)
        {
            Point pixel = new Point(73, 262);

            Color c;
            if (image != null)
                c = image[pixel];
            else
                c = GetColorAt(pixel);

            return (c.Name == PixelColors.getinstance().isCharScreen_GrayEnterGameButton);
        }

        static public bool isInTown(LockedFastImage image = null)
        {
            Point pixel = new Point(258, 545);
            Point pixel2 = new Point(284, 546);

            Color c;
            if (image != null)
                c = image[pixel];
            else
                c = GetColorAt(pixel);

            Color c2;
            if (image != null)
                c2 = image[pixel2];
            else
                c2 = GetColorAt(pixel2);

            return (c.Name == PixelColors.getinstance().isInTown || isStashOpen(image) || c2.Name == PixelColors.getinstance().isInTown2);
        }

        static public bool isWPopen(LockedFastImage image = null)
        {
            Point pixel = new Point(158, 66);

            Color c;
            if (image != null)
                c = image[pixel];
            else
                c = GetColorAt(pixel);

            return (c.Name == PixelColors.getinstance().isWPopen);
        }

        static public bool isNeedRep(LockedFastImage image = null)
        {
            Point pixel = new Point(585, 49);


            Color c;
            if (image != null)
                c = image[pixel];
            else
                c = GetColorAt(pixel);

            return (c.Name == PixelColors.getinstance().isNeedRep1 || c.Name == PixelColors.getinstance().isNeedRep2);
        }

        static public bool isDead(LockedFastImage image = null)
        {
            Point pixel = new Point(522, 502);


            Color c;
            if (image != null)
                c = image[pixel];
            else
                c = GetColorAt(pixel);

            return (c.Name == PixelColors.getinstance().isDead);
        }

        static public bool isStashOpen(LockedFastImage image = null)
        {
            Point pixel = new Point(167, 60);

            Color c;
            if (image != null)
                c = image[pixel];
            else
                c = GetColorAt(pixel);

            return (c.Name == PixelColors.getinstance().isStashOpen);
        }

        static public bool isInventoryOpen(LockedFastImage image = null)
        {
            Point pixel = new Point(672, 61);


            Color c;
            if (image != null)
                c = image[pixel];
            else
                c = GetColorAt(pixel);

            return (c.Name == PixelColors.getinstance().isInventoryOpen);
        }



        static public bool portToTown(ref Random rnd, string module_name)
        {
            Tools.clickPortal();

            TimeSpan max = new TimeSpan(0, 0, 10);
            DateTime start = DateTime.Now;
            for (; GameStateChecker.getInstance().current_game_state.ingame_flags.inTown == false; )
            {
                System.Threading.Thread.Sleep(500);

                if (DateTime.Now - start >= max)
                {
                    main.getInstance().writeToLog(module_name, "Port to town failed!");
                    return false;
                }
                if (GameStateChecker.getInstance().current_game_state.ingame_flags.isDead)
                {
                    main.getInstance().writeToLog(module_name, "Dead!");
                    return false;
                }
            }
            return true;
        }
    }
}
