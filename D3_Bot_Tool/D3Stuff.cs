using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace D3_Bot_Tool
{
    class D3Stuff
    {
        private String module_name = "D3Handles";
        private static D3Stuff instance = null;
        public static D3Stuff getInstance()
        {
            if (instance == null)
                instance = new D3Stuff();

            return instance;
        }

        public String getModuleName()
        {
            return module_name;
        }

        public void init(bool silent = false)
        {
            instance = new D3Stuff();
            
            if(!silent)
                main.getInstance().writeToLog(module_name, "Reloaded!");
        }

        private UInt32 baseAddr = 0;
        private IntPtr Handle = new IntPtr(0);
        private IntPtr WinHandle = new IntPtr(0);
        public Process diabloProcess;
        public D3Stuff()
        {
            Process[] processes = Process.GetProcessesByName("Diablo III");
            if (processes.Count() > 1)
            {
                System.Windows.Forms.MessageBox.Show("Too many Processes named Diablo 3!", "error");
                //throw new Exception("Too many Processes named Diablo 3!");
                return;
            }

            if (processes.Count() == 0)
            {
                System.Windows.Forms.MessageBox.Show("Diablo 3 not found! Reload D3 handels if you start Diablo yourself.", "error");
                //throw new Exception("Diablo 3 not found!");
                return;
            }


            diabloProcess = processes[0];
            Handle = processes[0].Handle;
            WinHandle = processes[0].MainWindowHandle;
            baseAddr = (UInt32)dwGetModuleBaseAddress(processes[0].Id, "Diablo III.exe").ToInt32();
        }

        private IntPtr dwGetModuleBaseAddress(int PID, string ModuleName)
        {
            Process myProcess = Process.GetProcessById(PID);

            ProcessModuleCollection myProcessModuleCollection = myProcess.Modules;

            // Display the 'BaseAddress' of each of the modules.
            for (int i = 0; i < myProcessModuleCollection.Count; i++)
            {
                if (myProcessModuleCollection[i].ModuleName == ModuleName)
                    return myProcessModuleCollection[i].BaseAddress;
            }

            return new IntPtr();
        }

        public IntPtr getD3WinHandle()
        {
            return WinHandle;
        }

        [DllImport("kernel32.dll", EntryPoint = "ReadProcessMemory")]
        public static extern Int32 ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [In, Out] byte[] buffer, int size, out IntPtr lpNumberOfBytesRead);

        public PlayerPos readPlayerPos()
        {
            /*
            ["fmodex.dll" + D8CD0] + 778] + 800] + 594] + 470]
+70 ] = X player position, float.
+74 ] = Y player position, float.
+78 ] = Z player position, float.
             * */
            return new PlayerPos(0, 0, 0);
        }

        public float readHP()
        {
            List<UInt32[]> hp_offsets = new List<uint[]>();

            float hp = readMemmory(new UInt32[] { 0xFF13E4, 0x18, 0xC8, 0xC, 0x2A0, 0x14 });
            if (validHp(hp))
                return hp;
            
            return -1;
        }

        private bool validHp(float hp)
        {
            return (hp >= 0 && hp < 999999);
        }

        private float readMemmory(UInt32[] offsets)
        {
            try
            {
                byte[] buffer = readMemmory_core(offsets);
                return BitConverter.ToSingle(buffer, 0);
            }
            catch { return -1; }
        }

        private byte[] readMemmory_core(UInt32[] offsets)
        {
            IntPtr numBytesRead;
            UInt32 first = baseAddr + 0xFF0B94;

            byte[] buffer = new byte[4];

            //offset 0
            UInt32 value = baseAddr + offsets[0];
            ReadProcessMemory(Handle, (IntPtr)value, buffer, 4, out numBytesRead);

            for(int i = 1; i < offsets.Count(); i++)
            {
                value = BitConverter.ToUInt32(buffer, 0) + offsets[i];
                ReadProcessMemory(Handle, (IntPtr)value, buffer, 4, out numBytesRead);
            }

            return buffer;
        }
    }

    class PlayerPos
    {
        private float _x;
        private float _y;
        private float _z;

        public float X
        {
            get
            {
                return _x;
            }
        }
        public float Y
        {
            get
            {
                return _y;
            }
        }
        public float Z
        {
            get
            {
                return _z;
            }
        }

        public PlayerPos(float x, float y, float z)
        {
            _x = x;
            _y = y;
            _z = z;
        }
    }
}
