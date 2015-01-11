using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace D3_Bot_Tool
{
    static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            main f = new main();
            if(f.loaded_d3_mem)
                Application.Run(f);
        }
    }
}
