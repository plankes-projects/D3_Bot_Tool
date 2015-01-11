using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace D3_Bot_Tool
{
    public class GameState
    {
        public enum GameStates { CharScreen_RedEnterGameButton, CharScreen_GrayEnterGameButton, DisconnectDienst, InGame, LoadingScreen, LoginLoading, LoginScreen, Unknown, D3NotLoaded };
        public struct IngameFlags
        {
            public bool inTown;
            public bool needRep;
            public bool isWPopen;
            public bool isDead;
            public bool isStashOpen;
            public bool isinventoryOpen;
        }

        public GameStates game_state;
        public IngameFlags ingame_flags;

        private String prefix = "   ";
        override
        public String ToString()
        {

            String ret = "";
            ret += Enum.GetName(typeof(GameStates), game_state);

            if (game_state == GameStates.InGame)
            {
                ret += Environment.NewLine;
                
                if (ingame_flags.inTown)
                    ret += prefix + "In town" + Environment.NewLine;
                if (ingame_flags.needRep)
                    ret += prefix + "Need repair" + Environment.NewLine;
                if (ingame_flags.isWPopen)
                    ret += prefix + "WP is open" + Environment.NewLine;
                if (ingame_flags.isDead)
                    ret += prefix + "Dead" + Environment.NewLine;
                if (ingame_flags.isStashOpen)
                    ret += prefix + "Stash is open" + Environment.NewLine;
                if (ingame_flags.isinventoryOpen)
                    ret += prefix + "Inventory is open" + Environment.NewLine;
            }

            return ret;
        }

        public String ToStringOneLine()
        {
            String[] lines = ToString().Split(new String[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            if(lines.Count() == 0)
                return "";

            if(lines.Count() == 1)
                return lines[0];

            String ret = lines[0] + " ( ";
            for (int i = 1; i < lines.Count(); i++)
            {
                if(lines[i].IndexOf(prefix) == 0)
                    ret += lines[i].Substring(prefix.Count()) + " ";
                else
                    ret += lines[i] + " ";
            }

            ret += ")";

            return ret;
        }

        public GameState()
        {
            try
            {
                setFlags();

                LockedFastImage image = new LockedFastImage(Tools.D3ScreenShot());

                if (Tools.isCharScreen_RedEnterGameButton(image))
                    game_state = GameStates.CharScreen_RedEnterGameButton;

                else if (Tools.isCharScreen_GrayEnterGameButton(image))
                    game_state = GameStates.CharScreen_GrayEnterGameButton;

                else if (Tools.isDisconnectDienst(image))
                    game_state = GameStates.DisconnectDienst;

                else if (Tools.isInGame(image))
                    game_state = GameStates.InGame;

                else if (Tools.isLoadingScreen(image))
                    game_state = GameStates.LoadingScreen;

                else if (Tools.isLoginLoading(image))
                    game_state = GameStates.LoginLoading;

                else if (Tools.isloginScreen(image))
                    game_state = GameStates.LoginScreen;

                else
                    game_state = GameStates.Unknown;

                if (game_state == GameStates.InGame)
                    setFlags(image);

                image.Dispose();
            }
            catch (Exception e) { game_state = GameStates.D3NotLoaded;}
        }

        static public bool operator==(GameState a, GameState b)
        {
            if (a.game_state != b.game_state)
                return false;

            if (a.ingame_flags.inTown != b.ingame_flags.inTown)
                return false;
            if (a.ingame_flags.needRep != b.ingame_flags.needRep)
                return false;
            if (a.ingame_flags.isWPopen != b.ingame_flags.isWPopen)
                return false;
            if (a.ingame_flags.isDead != b.ingame_flags.isDead)
                return false;
            if (a.ingame_flags.isinventoryOpen != b.ingame_flags.isinventoryOpen)
                return false;
            if (a.ingame_flags.isStashOpen != b.ingame_flags.isStashOpen)
                return false;

            return true;
        }

        private void setFlags(LockedFastImage image = null)
        {
            if (image == null)
            {
                ingame_flags.inTown = false;
                ingame_flags.needRep = false;
                ingame_flags.isWPopen = false;
                ingame_flags.isDead = false;
                ingame_flags.isStashOpen = false;
                ingame_flags.isinventoryOpen = false;
            }
            else
            {
                ingame_flags.inTown = Tools.isInTown(image);
                ingame_flags.needRep = Tools.isNeedRep(image);
                ingame_flags.isWPopen = Tools.isWPopen(image);
                ingame_flags.isDead = Tools.isDead(image);
                ingame_flags.isStashOpen = Tools.isStashOpen(image);
                ingame_flags.isinventoryOpen = Tools.isInventoryOpen(image);
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (GetType() != obj.GetType())
                return false;

            return (GameState)obj == this;
        }

        static public bool operator !=(GameState a, GameState b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        } 
    }
}
