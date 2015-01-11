using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace D3_Bot_Tool
{
    public partial class AdjustPixelColors : Form
    {
        public AdjustPixelColors()
        {
            InitializeComponent();
        }
        MyXML xml = new MyXML(PixelColors.xml_file);

        private void b_isIngame_Click(object sender, EventArgs e)
        {
            xml.write(PixelColors.isInGame_key, Tools.GetColorAt(new Point(125, 598)).Name);
            PixelColors.getinstance().reload();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            xml.write(PixelColors.isloginScreen_key, Tools.GetColorAt(new Point(278, 178)).Name);
            PixelColors.getinstance().reload();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            xml.write(PixelColors.isLoadingScreen_key, Tools.GetColorAt(new Point(450, 562)).Name);
            PixelColors.getinstance().reload();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            xml.write(PixelColors.isDisconnectDienst_key, Tools.GetColorAt(new Point(430, 381)).Name);
            PixelColors.getinstance().reload();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            xml.write(PixelColors.isLoginLoading_key, Tools.GetColorAt(new Point(438, 401)).Name);
            PixelColors.getinstance().reload();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            xml.write(PixelColors.isCharScreen_RedEnterGameButton_key, Tools.GetColorAt(new Point(73, 262)).Name);
            PixelColors.getinstance().reload();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            xml.write(PixelColors.isCharScreen_GrayEnterGameButton_key, Tools.GetColorAt(new Point(73, 262)).Name);
            PixelColors.getinstance().reload();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            xml.write(PixelColors.isInTown_key, Tools.GetColorAt(new Point(258, 545)).Name);
            PixelColors.getinstance().reload();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            xml.write(PixelColors.isWPopen_key, Tools.GetColorAt(new Point(158, 66)).Name);
            PixelColors.getinstance().reload();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            xml.write(PixelColors.isNeedRep1_key, Tools.GetColorAt(new Point(585, 49)).Name);
            PixelColors.getinstance().reload();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            xml.write(PixelColors.isNeedRep2_key, Tools.GetColorAt(new Point(585, 49)).Name);
            PixelColors.getinstance().reload();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            xml.write(PixelColors.isDead_key, Tools.GetColorAt(new Point(522, 502)).Name);
            PixelColors.getinstance().reload();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            xml.write(PixelColors.isStashOpen_key, Tools.GetColorAt(new Point(167, 60)).Name);
            PixelColors.getinstance().reload();
        }

        private void button13_Click(object sender, EventArgs e)
        {
            xml.write(PixelColors.isInventoryOpen_key, Tools.GetColorAt(new Point(672, 61)).Name);
            PixelColors.getinstance().reload();
        }

        private void button14_Click(object sender, EventArgs e)
        {
            groupBox1.Enabled = true;
        }

        private void button15_Click(object sender, EventArgs e)
        {
            D3InventoryStuff.writeEmtyInvColors();
        }

        private void button16_Click(object sender, EventArgs e)
        {
            xml.write(PixelColors.isInTown2_key, Tools.GetColorAt(new Point(284, 546)).Name);
            PixelColors.getinstance().reload();
        }
    }
}
