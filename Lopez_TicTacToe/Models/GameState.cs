using System;

namespace Lopez_TicTacToe.Models
{
    public class GameState
    {
        public string[,] Board { get; private set; }
        public string CurrentPlayer { get; private set; }
        public string Winner { get; private set; }
        public int XWins { get; private set; }
        public int OWins { get; private set; }
        public int Draws { get; private set; }

        public GameState()
        {
            Reset();
        }

        public void Reset()
        {
            Board = new string[3, 3];
            for (int r = 0; r < 3; r++)
                for (int c = 0; c < 3; c++)
                    Board[r, c] = "";

            CurrentPlayer = "X";
            Winner = "";
        }

        public bool MakeMove(int row, int col)
        {
            if (Board[row, col] == "" && Winner == "")
            {
                Board[row, col] = CurrentPlayer;
                CheckWin();

                if (Winner == "")
                {
                    CurrentPlayer = (CurrentPlayer == "X") ? "O" : "X";
                }
                return true;
            }
            return false;
        }

        public int MakeAIMove()
        {
            if (Winner != "" || CurrentPlayer != "O")
                return -1; 

            for (int r = 0; r < 3; r++)
            {
                for (int c = 0; c < 3; c++)
                {
                    if (Board[r, c] == "")
                    {
                        Board[r, c] = "O"; 
                        CheckWin();

                        if (Winner == "")
                        {
                            CurrentPlayer = "X"; 
                        }

                        return (r * 3) + c; 
                    }
                }
            }

            return -1; // 
        }


        public void CheckWin()
        {
            string[][] patterns =
            {
                new string[] { Board[0, 0], Board[0, 1], Board[0, 2] }, // Row 1
                new string[] { Board[1, 0], Board[1, 1], Board[1, 2] }, // Row 2
                new string[] { Board[2, 0], Board[2, 1], Board[2, 2] }, // Row 3
                new string[] { Board[0, 0], Board[1, 0], Board[2, 0] }, // Column 1
                new string[] { Board[0, 1], Board[1, 1], Board[2, 1] }, // Column 2
                new string[] { Board[0, 2], Board[1, 2], Board[2, 2] }, // Column 3
                new string[] { Board[0, 0], Board[1, 1], Board[2, 2] }, // Diagonal 1
                new string[] { Board[0, 2], Board[1, 1], Board[2, 0] }  // Diagonal 2
            };

            foreach (var pattern in patterns)
            {
                if (pattern[0] != "" && pattern[0] == pattern[1] && pattern[1] == pattern[2])
                {
                    Winner = pattern[0];
                    if (Winner == "X") XWins++;
                    else if (Winner == "O") OWins++;
                    return;
                }
            }

            if (IsBoardFull())
            {
                Winner = "Draw";
                Draws++;
            }
        }

        public bool IsBoardFull()
        {
            for (int r = 0; r < 3; r++)
            {
                for (int c = 0; c < 3; c++)
                {
                    if (Board[r, c] == "")
                        return false;
                }
            }
            return true;
        }
    }
}
