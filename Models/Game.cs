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
        public string CurrentPlayerName { get; set; }
        public User Player1 { get; set; }
        public User Player2 { get; set; }
        private int roundsPlayed;

        public Game()
        {
            Fields = new FieldStatus[3,3];
            for (int x = 0; x < 3; x++)
                for (int y = 0; x < 3; x++)
                    Fields[x, y] = FieldStatus.Empty;
            roundsPlayed = 0;
        }
        
        

        public User PlayRound(int x, int y)
        {
            FieldStatus status = CurrentPlayerName == Player1.Name ? FieldStatus.Player1 : FieldStatus.Player2;
            roundsPlayed++;
            UpdateField(x, y, status);
            FieldStatus winner = CheckForWinner();
            if (winner == FieldStatus.Empty)
            {
                if (roundsPlayed == 9) return new User();
                return null;
            }

            CurrentPlayerName = CurrentPlayerName == Player1.Name ? Player2.Name : Player1.Name;
            if (winner == FieldStatus.Player1) return Player1;
            return Player2;
        }
        
        public void UpdateField(int x, int y, FieldStatus status) => Fields[x, y] = status;

        public FieldStatus CheckForWinner()
        {
            if (roundsPlayed < 5) return FieldStatus.Empty;
            FieldStatus winner;
            
            // Reihen überprüfen
            // Links -> Rechts
            for (int y = 0; y < 3; y++)
            {
                winner = CompareRows(0, y, 1, 0);
                if (winner != FieldStatus.Empty) return winner;
            }
            
            // Zeilen überprüfen
            // Oben -> Unten
            for (int x = 0; x < 3; x++)
            {
                winner = CompareRows(x, 0, 0, 1);
                if (winner != FieldStatus.Empty) return winner;
            }
            
            // Diagonalen überprüfen
            // Oben links -> Unten rechts
            winner = CompareRows(0, 0, 1, 1);
            if (winner != FieldStatus.Empty) return winner;

            // Oben rechts -> Unten links
            winner = CompareRows(2, 0, -1, 1);
            
            return winner;

        }

        private FieldStatus CompareRows(int x, int y, int dX, int dY)
        {
            FieldStatus first = Fields[y, x];
            if (first == FieldStatus.Empty) return FieldStatus.Empty;

            for (int i = 0; i < 3; i++)
            {
                int startX = x + dX * i;
                int startY = y + dY * i;
                if (Fields[startY, startX] != first) return FieldStatus.Empty;
            }

            return first;
        }
    }
}