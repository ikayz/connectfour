namespace ConnectFour;

public record MoveRecord(int MoveNumber, int Player, int Column, int Row);

public class GameState
{
    static GameState()
    {
        CalculateWinningPlaces();
    }

    public enum WinState
    {
        No_Winner = 0,
        Player1_Wins = 1,
        Player2_Wins = 2,
        Tie = 3
    }

    /// <summary>
    /// The player whose turn it currently is (1 or 2).
    /// </summary>
    public int PlayerTurn => TheBoard.Count(x => x != 0) % 2 + 1;

    /// <summary>
    /// Number of turns played so far (0 to 42).
    /// </summary>
    public int CurrentTurn => TheBoard.Count(x => x != 0);

    public static readonly List<int[]> WinningPlaces = new();

    public List<int> TheBoard { get; private set; } = new List<int>(new int[42]);

    /// <summary>
    /// Ordered history of moves made during the current game.
    /// </summary>
    public List<MoveRecord> MoveHistory { get; private set; } = new();

    /// <summary>
    /// Board cell indices (0..41) that form the winning four-in-a-row.
    /// </summary>
    public HashSet<int> WinningCells { get; private set; } = new();

    // Persistent match & consecutive win streak tracking across games
    public int Player1Wins { get; private set; }
    public int Player2Wins { get; private set; }
    public int Ties { get; private set; }
    public int Player1Streak { get; private set; }
    public int Player2Streak { get; private set; }
    public int Player1BestStreak { get; private set; }
    public int Player2BestStreak { get; private set; }

    private bool _gameResultRecorded;

    public static void CalculateWinningPlaces()
    {
        WinningPlaces.Clear();

        // Horizontal rows (6 rows x 4 starting columns = 24)
        for (byte row = 0; row < 6; row++)
        {
            byte rowCol1 = (byte)(row * 7);
            byte rowColEnd = (byte)((row + 1) * 7 - 1);
            byte checkCol = rowCol1;
            while (checkCol <= rowColEnd - 3)
            {
                WinningPlaces.Add(new int[]
                {
                    checkCol,
                    (byte)(checkCol + 1),
                    (byte)(checkCol + 2),
                    (byte)(checkCol + 3)
                });
                checkCol++;
            }
        }

        // Vertical columns (7 columns x 3 starting rows = 21)
        for (byte col = 0; col < 7; col++)
        {
            byte colRow1 = col;
            byte colRowEnd = (byte)(35 + col);
            byte checkRow = colRow1;
            while (checkRow <= 14 + col)
            {
                WinningPlaces.Add(new int[]
                {
                    checkRow,
                    (byte)(checkRow + 7),
                    (byte)(checkRow + 14),
                    (byte)(checkRow + 21)
                });
                checkRow += 7;
            }
        }

        // Forward slash diagonal "/" (12)
        for (byte col = 0; col < 4; col++)
        {
            byte colRow1 = (byte)(21 + col);
            byte colRowEnd = (byte)(35 + col);
            byte checkPos = colRow1;
            while (checkPos <= colRowEnd)
            {
                WinningPlaces.Add(new int[]
                {
                    checkPos,
                    (byte)(checkPos - 6),
                    (byte)(checkPos - 12),
                    (byte)(checkPos - 18)
                });
                checkPos += 7;
            }
        }

        // Backslash diagonal "\" (12)
        for (byte col = 0; col < 4; col++)
        {
            byte colRow1 = (byte)(0 + col);
            byte colRowEnd = (byte)(14 + col);
            byte checkPos = colRow1;
            while (checkPos <= colRowEnd)
            {
                WinningPlaces.Add(new int[]
                {
                    checkPos,
                    (byte)(checkPos + 8),
                    (byte)(checkPos + 16),
                    (byte)(checkPos + 24)
                });
                checkPos += 7;
            }
        }
    }

    /// <summary>
    /// Checks whether either player has won or if the game is a tie.
    /// Records win/streak statistics once when a game concludes.
    /// </summary>
    public WinState CheckForWin()
    {
        if (TheBoard.Count(x => x != 0) < 7)
            return WinState.No_Winner;

        foreach (var scenario in WinningPlaces)
        {
            if (TheBoard[scenario[0]] == 0)
                continue;

            if (TheBoard[scenario[0]] == TheBoard[scenario[1]] &&
                TheBoard[scenario[1]] == TheBoard[scenario[2]] &&
                TheBoard[scenario[2]] == TheBoard[scenario[3]])
            {
                WinningCells = new HashSet<int>(scenario);
                int winner = TheBoard[scenario[0]];

                if (!_gameResultRecorded)
                {
                    if (winner == 1)
                    {
                        Player1Wins++;
                        Player1Streak++;
                        Player2Streak = 0;
                        if (Player1Streak > Player1BestStreak)
                            Player1BestStreak = Player1Streak;
                    }
                    else
                    {
                        Player2Wins++;
                        Player2Streak++;
                        Player1Streak = 0;
                        if (Player2Streak > Player2BestStreak)
                            Player2BestStreak = Player2Streak;
                    }
                    _gameResultRecorded = true;
                }

                return (WinState)winner;
            }
        }

        if (TheBoard.Count(x => x != 0) == 42)
        {
            if (!_gameResultRecorded)
            {
                Ties++;
                Player1Streak = 0;
                Player2Streak = 0;
                _gameResultRecorded = true;
            }
            return WinState.Tie;
        }

        return WinState.No_Winner;
    }

    /// <summary>
    /// Plays a piece in the given 0-based column (0..6) and returns the 1-based landing row (1 = top, 6 = bottom).
    /// </summary>
    public byte PlayPiece(byte col)
    {
        if (CheckForWin() != WinState.No_Winner)
            throw new ArgumentException("Game is over");

        if (col > 6)
            throw new ArgumentException("Column must be between 0 and 6");

        if (TheBoard[col] != 0)
            throw new ArgumentException("Column is full");

        int player = PlayerTurn;
        var landingSpot = col;
        for (var i = col; i < 42; i += 7)
        {
            if (TheBoard[i] != 0)
                break;
            landingSpot = (byte)i;
        }

        TheBoard[landingSpot] = player;

        byte landingRow = (byte)(landingSpot / 7 + 1);
        MoveHistory.Add(new MoveRecord(CurrentTurn, player, col + 1, landingRow));

        return landingRow;
    }

    /// <summary>
    /// Returns true if the given 0-based column has no empty spaces left.
    /// </summary>
    public bool IsColumnFull(byte col) => col <= 6 && TheBoard[col] != 0;

    /// <summary>
    /// Resets the board for a new round while preserving win and streak statistics.
    /// </summary>
    public void ResetBoard()
    {
        TheBoard = new List<int>(new int[42]);
        MoveHistory.Clear();
        WinningCells.Clear();
        _gameResultRecorded = false;
    }

    /// <summary>
    /// Resets all win counts and consecutive win streaks.
    /// </summary>
    public void ResetStats()
    {
        Player1Wins = 0;
        Player2Wins = 0;
        Ties = 0;
        Player1Streak = 0;
        Player2Streak = 0;
        Player1BestStreak = 0;
        Player2BestStreak = 0;
    }
}
