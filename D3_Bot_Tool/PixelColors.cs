using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace D3_Bot_Tool
{
    class PixelColors
    {
        private string isInGame_ = "";
        public string isInGame { get { return isInGame_; } }
        private string isloginScreen_ = "";
        public string isloginScreen { get { return isloginScreen_; } }
        private string isLoadingScreen_ = "";
        public string isLoadingScreen { get { return isLoadingScreen_; } }
        private string isDisconnectDienst_ = "";
        public string isDisconnectDienst { get { return isDisconnectDienst_; } }
        private string isLoginLoading_ = "";
        public string isLoginLoading { get { return isLoginLoading_; } }
        private string isCharScreen_RedEnterGameButton_ = "";
        public string isCharScreen_RedEnterGameButton { get { return isCharScreen_RedEnterGameButton_; } }
        private string isCharScreen_GrayEnterGameButton_ = "";
        public string isCharScreen_GrayEnterGameButton { get { return isCharScreen_GrayEnterGameButton_; } }
        private string isInTown_ = "";
        public string isInTown { get { return isInTown_; } }

        private string isInTown2_ = "";
        public string isInTown2 { get { return isInTown2_; } }

        private string isWPopen_ = "";
        public string isWPopen { get { return isWPopen_; } }
        private string isNeedRep1_ = "";
        public string isNeedRep1 { get { return isNeedRep1_; } }
        private string isNeedRep2_ = "";
        public string isNeedRep2 { get { return isNeedRep2_; } }
        private string isDead_ = "";
        public string isDead { get { return isDead_; } }
        private string isStashOpen_ = "";
        public string isStashOpen { get { return isStashOpen_; } }
        private string isInventoryOpen_ = "";
        public string isInventoryOpen { get { return isInventoryOpen_; } }

        private PixelColors()
        {
            reload();
        }

        static public string isInGame_key { get { return "isInGame_key"; } }
        static public string isloginScreen_key { get { return "isloginScreen_key"; } }
        static public string isLoadingScreen_key { get { return "isLoadingScreen_key"; } }
        static public string isDisconnectDienst_key { get { return "isDisconnectDienst_key"; } }
        static public string isLoginLoading_key { get { return "isLoginLoading_key"; } }
        static public string isCharScreen_RedEnterGameButton_key { get { return "isCharScreen_RedEnterGameButton_key"; } }
        static public string isCharScreen_GrayEnterGameButton_key { get { return "isCharScreen_GrayEnterGameButton_key"; } }
        static public string isInTown_key { get { return "isInTown_key"; } }
        static public string isInTown2_key { get { return "isInTown2_key"; } }
        static public string isWPopen_key { get { return "isWPopen_key"; } }
        static public string isNeedRep1_key { get { return "isNeedRep1_key"; } }
        static public string isNeedRep2_key { get { return "isNeedRep2_key"; } }
        static public string isDead_key { get { return "isDead_key"; } }
        static public string isStashOpen_key { get { return "isStashOpen_key"; } }
        static public string isInventoryOpen_key { get { return "isInventoryOpen_key"; } }
        static public string xml_file { get { return "config/pixel_colors"; } }


        public void reload()
        {
            try
            {
                MyXML xml = new MyXML(xml_file);
                isInGame_ = xml.read(isInGame_key);
                isloginScreen_ = xml.read(isloginScreen_key);
                isLoadingScreen_ = xml.read(isLoadingScreen_key);
                isDisconnectDienst_ = xml.read(isDisconnectDienst_key);
                isLoginLoading_ = xml.read(isLoginLoading_key);
                isCharScreen_RedEnterGameButton_ = xml.read(isCharScreen_RedEnterGameButton_key);
                isCharScreen_GrayEnterGameButton_ = xml.read(isCharScreen_GrayEnterGameButton_key);
                isInTown_ = xml.read(isInTown_key);
                isInTown2_ = xml.read(isInTown2_key);
                isWPopen_ = xml.read(isWPopen_key);
                isNeedRep1_ = xml.read(isNeedRep1_key);
                isNeedRep2_ = xml.read(isNeedRep2_key);
                isDead_ = xml.read(isDead_key);
                isStashOpen_ = xml.read(isStashOpen_key);
                isInventoryOpen_ = xml.read(isInventoryOpen_key);
            }
            catch (Exception e) { System.Windows.Forms.MessageBox.Show(e.Message); }
        }
        /*
        public void reload()
        {
            isInGame_ = "ff5d574c";
            isloginScreen_ = "ffebb678";
            isLoadingScreen_ = "ff2c200d";
            isDisconnectDienst_ = "ff360a00";
            isLoginLoading_ = "ff0e0406";
            isCharScreen_RedEnterGameButton_ = "ff230400";
            isCharScreen_GrayEnterGameButton_ = "ff100706";
            isInTown_ = "ff101008";
            isWPopen_ = "ff7b6226";
            isNeedRep1_ = "ffffeb00";
            isNeedRep2_ = "ffdf0000";
            isDead_ = "ff2b0100";
            isStashOpen_ = "ff15ae60";
            isInventoryOpen_ = "ff104a9f";
        }*/

        static private PixelColors instance = null;
        static public PixelColors getinstance()
        {
            if (instance == null)
                instance = new PixelColors();

            return instance;
        }
    }
}
