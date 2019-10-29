using System.Collections.Generic;

namespace Models
{
    public enum FieldStatus
    {
        Player1,
        Player2,
        Empty
    }
    
    public class Game
    {
        public FieldStatus[,] Fields { get; set; }
        public User CurrentPlayer { get; set; }
        public User Player1 { get; set; }
        public User Player2 { get; set; }
        public int RoundsPlayed { get; set; }
        public List<Turn> Turns { get; set; }
          
    }
}