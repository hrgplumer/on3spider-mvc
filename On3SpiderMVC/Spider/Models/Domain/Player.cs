using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spider.Models.Domain
{
    public class Player
    {
        public string FirstLast { get; set; }
        public string Height { get; set; }
        public string Weight { get; set; }
        public string Position { get; set; }
        public string Class { get; set; }
        public string Major { get; set; }

        #region Static

        public static bool Equals(Player player1, Player player2)
        {
            return
                player1.FirstLast == player2.FirstLast &&
                player1.Height == player2.Height &&
                player1.Weight == player2.Weight &&
                player1.Position == player2.Position &&
                player1.Class == player2.Class &&
                player1.Major == player2.Major;
        }

        #endregion
    }
}
