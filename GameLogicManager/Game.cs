using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace GameLogic
{
    public class Game
    {
        private Board m_Board;
        private Player m_PlayerOne;
        private Player m_PlayerTwo;
        private bool m_finishedGame;
        private bool m_JustEat;
        private Game.eGameMode m_GameMode;
        private Cell m_HaveToStartFromCell = null;

        public event InvalidMoveEventHandler InvalidMove;
        public event BoardUpdatedEventHandler BoardUpdated;
        public event EventHandler GameFinished;

        public enum eBoardKey
        {
            BLANK,
            X,
            O,
            U,
            K,
        }

        public enum eGameMode
        {
            OnePlayer = 1,
            TwoPlayer,
        }

        public Game(int i_BoardSize, string i_NameOfPlayerOne, string i_NameOfPlayerTwo, Game.eGameMode i_GameMode)
        {
            m_Board = new Board(i_BoardSize);
            int numOfSoldier = CalcNumOfSoldiers(i_BoardSize);
            m_PlayerOne = new Player(i_NameOfPlayerOne, numOfSoldier, true);
            m_PlayerTwo = new Player(i_NameOfPlayerTwo, numOfSoldier, false);
            m_finishedGame = false;
            InitPlayersListOfSoldiersCells();
            GameMode = i_GameMode;
        }

        public Board Board
        {
            get
            {
                return m_Board;
            }
        }

        public Cell HaveToStartFromCell
        {
            get
            {
                return m_HaveToStartFromCell;
            }

            set
            {
                m_HaveToStartFromCell = value;
            }
        }

        public Player PlayerOne
        {
            get
            {
                return m_PlayerOne;
            }
        }

        public Player PlayerTwo
        {
            get
            {
                return m_PlayerTwo;
            }
        }

        public bool FinishedGame
        {
            get
            {
                return m_finishedGame;
            }

            set
            {
                m_finishedGame = value;
            }
        }

        public bool JustEat
        {
            get
            {
                return m_JustEat;
            }

            set
            {
                m_JustEat = value;
            }
        }

        public eGameMode GameMode
        {
            get
            {
                return m_GameMode;
            }

            set
            {
                m_GameMode = value;
            }
        }

        public void MovePlayerAndUpdateBoard(Cell i_From, Cell i_To)
        {
            int toRow = i_To.Point.Row;
            int fromRow = i_From.Point.Row;
            i_From.BoardSigned = Board[i_From.Point.Row, i_From.Point.Col].BoardSigned;
            i_To.BoardSigned = Board[i_To.Point.Row, i_To.Point.Col].BoardSigned;
            Player refPlayerToMove;
            Player refPlayerToDelete;
            BoardUpdatedEventArgs BoardUpdated = null;
            Cell toDelete = null;
            bool HaveToEatAgain = false;
            bool messageWasShownAlready = false;
            List<Cell> possibleMoves = new List<Cell>();
            if (PlayerOne.Turn == true)
            {
                refPlayerToMove = PlayerOne;
                refPlayerToDelete = PlayerTwo;
            }
            else
            {
                refPlayerToMove = PlayerTwo;
                refPlayerToDelete = PlayerOne;
            }

            if (refPlayerToMove.CellsOfSoldiers.Count != 0)
            {
                if (isMySign(refPlayerToMove, i_From.BoardSigned) && IsMoveValid(i_From, i_To, possibleMoves, refPlayerToMove, 0, ref messageWasShownAlready))
                {
                    // Player ate another player
                    if (toRow == fromRow - 2 || toRow == fromRow + 2)
                    {
                        MoveSoldier(i_From, refPlayerToMove, i_To);
                        UpdateMiddleCell(i_From, i_To, refPlayerToMove, refPlayerToDelete, out toDelete);
                        UpdateAllValidMoves();
                        JustEat = true;
                        if (refPlayerToMove.EatingCellsOptions.Count != 0)
                        {
                            i_To.BoardSigned = i_From.BoardSigned;
                            if (HaveAnotherMoveToEatButWithSameSoldier(i_To))
                            {

                                HaveToEatAgain = true;
                                HaveToStartFromCell = i_To;
                            }
                        }
                    }

                    // player just moved without eating
                    else
                    {
                        MoveSoldier(i_From, refPlayerToMove, i_To);
                        UpdateAllValidMoves();
                        JustEat = false;
                    }

                    if (!HaveToEatAgain)
                    {
                        FlipTurns();
                        HaveToStartFromCell = null;
                    }

                    BoardUpdated = new BoardUpdatedEventArgs(i_From, i_To, JustEat, toDelete, m_Board.Size);
                    OnBoardUpdate(BoardUpdated);

                    if (GameMode == eGameMode.OnePlayer && PlayerTwo.Turn == true && PlayerTwo.CellsOfSoldiers.Count != 0)
                    {
                        MakeRandomMove(ref i_From, ref i_To, HaveToStartFromCell);
                        
                        MovePlayerAndUpdateBoard(i_From, i_To);
                    }
                }
                else if (!messageWasShownAlready)
                {
                    OnInvalidMove(new InvalidMoveEventArgs("Invalid Move!"));
                }
            }

            if(!FinishedGame)
            {
                checkIfGameFinished();
            }
        }

        public void UpdateAllValidMoves()
        {
            Cell currCell;
            List<Cell> currMovesOptionOfSoldier;
            PlayerOne.AllValidMoves.Clear();
            PlayerTwo.AllValidMoves.Clear();
            PlayerTwo.EatingCellsOptions.Clear();
            PlayerOne.EatingCellsOptions.Clear();

            int flag = 1;
            for (int i = 0; i < PlayerOne.CellsOfSoldiers.Count; i++)
            {
                currCell = PlayerOne.CellsOfSoldiers[i];
                currMovesOptionOfSoldier = GetPossibleMovesOfSoldier(currCell, flag);
                for (int k = 0; k < currMovesOptionOfSoldier.Count; k++)
                {
                    if (CheckIfCellIsInList(PlayerOne.AllValidMoves, currMovesOptionOfSoldier[k]) == false)
                    {
                        PlayerOne.AllValidMoves.Add(currMovesOptionOfSoldier[k]);
                    }
                }
            }

            flag = 2;
            for (int i = 0; i < PlayerTwo.CellsOfSoldiers.Count; i++)
            {
                currCell = PlayerTwo.CellsOfSoldiers[i];
                currMovesOptionOfSoldier = GetPossibleMovesOfSoldier(currCell, flag);
                for (int k = 0; k < currMovesOptionOfSoldier.Count; k++)
                {
                    if (CheckIfCellIsInList(PlayerTwo.AllValidMoves, currMovesOptionOfSoldier[k]) == false)
                    {
                        PlayerTwo.AllValidMoves.Add(currMovesOptionOfSoldier[k]);
                    }
                }
            }
        }

        public bool IsMoveValid(Cell i_From, Cell i_To, List<Cell> io_PossibleMoves, Player i_PlayerToMove, int flag, ref bool messageWasShownAlready)
        {
            bool valid = false;
            if (HaveToStartFromCell != null)
            {
                if(m_HaveToStartFromCell.Equals(i_From))
                {
                    valid = true;
                }
                else
                {
                    OnInvalidMove(new InvalidMoveEventArgs("You have to chooce the same soldier that you ate with!"));
                    messageWasShownAlready = true;
                    return false;
                }

                if(CheckIfCellIsInList(i_PlayerToMove.EatingCellsOptions, i_To))
                {
                    valid = true;
                }
                else
                {
                    OnInvalidMove(new InvalidMoveEventArgs("You have to eat!"));
                    messageWasShownAlready = true;
                    return false;
                }
            }
            else if(i_PlayerToMove.EatingCellsOptions.Count != 0)
            {
                for (int i = 0; i < i_PlayerToMove.EatingCellsOptions.Count; i++)
                {
                    if (i_PlayerToMove.EatingCellsOptions[i].Equals(i_To) == true)
                    {
                        valid = true;
                    }
                }

                if (valid == false)
                {
                    OnInvalidMove(new InvalidMoveEventArgs("You have an option to eat! Please enter other move"));
                    messageWasShownAlready = true;
                    return false;
                }
            }

            // check if i_To is not out of range
            io_PossibleMoves = GetPossibleMovesOfSoldier(i_From, flag);
            for (int i = 0; i < io_PossibleMoves.Count; i++)
            {
                if (io_PossibleMoves[i].Equals(i_To))
                {
                    valid = true;
                    break;
                }
            }
            return valid;
        }

        public List<Cell> GetPossibleMovesOfSoldier(Cell i_From, int i_Flag)
        {
            eBoardKey playerKey = i_From.BoardSigned;
            eBoardKey keyXsoldier = eBoardKey.X;
            eBoardKey keyOsoldier = eBoardKey.O;
            List<Cell> cellsOptions = new List<Cell>();
            if (playerKey == keyXsoldier)
            {
                GetPossibleMovesUpperSoldier(i_From, cellsOptions, i_Flag);
            }
            else if (playerKey == keyOsoldier)
            {
                GetPossibleMovesLowerSoldier(i_From, cellsOptions, i_Flag);
            }
            else
            {
                GetPossibleKingMove(i_From, cellsOptions, i_Flag);
            }

            return cellsOptions;
        }

        public void UpdateMiddleCell(Cell i_From, Cell i_To, Player io_RefPlayerToMove, Player io_RefPlayerToDelete, out Cell CelltoDelete)
        {
            
            int toCol = i_To.Point.Col;
            int toRow = i_To.Point.Row;
            int fromRow = i_From.Point.Row;
            int fromCol = i_From.Point.Col;
            int toDeleteRow;
            int toDeleteCol;

            Player refPlayerToDeleteDataFrom = io_RefPlayerToDelete;

            // moved down right
            if (toCol > fromCol && toRow > fromRow)
            {
                toDeleteRow = fromRow + 1;
                toDeleteCol = fromCol + 1;
            }

            // moved down left
            else if (toCol < fromCol && toRow > fromRow)
            {
                toDeleteRow = fromRow + 1;
                toDeleteCol = fromCol - 1;
            }

            // moved up right
            else if (toCol > fromCol && toRow < fromRow)
            {
                toDeleteRow = fromRow - 1;
                toDeleteCol = fromCol + 1;
            }

            // moved up left
            else
            {
                toDeleteRow = fromRow - 1;
                toDeleteCol = fromCol - 1;
            }

            Point toDelete = new Point(toDeleteRow, toDeleteCol);
            CelltoDelete = new Cell(toDeleteRow, toDeleteCol, eBoardKey.BLANK);
            // update player and board
            for (int i = 0; i < refPlayerToDeleteDataFrom.CellsOfSoldiers.Count; i++)
            {
                if (refPlayerToDeleteDataFrom.CellsOfSoldiers[i].Point.Equals(toDelete))
                {
                    Board[toDeleteRow, toDeleteCol].BoardSigned = eBoardKey.BLANK;
                    if (CheckIfCellIsInList(io_RefPlayerToMove.EatingCellsOptions, i_To) == true)
                    {
                        RemoveSoldierFromEatingList(io_RefPlayerToMove, i_To);
                    }

                    if (refPlayerToDeleteDataFrom.CellsOfSoldiers[i].BoardSigned == eBoardKey.K || refPlayerToDeleteDataFrom.CellsOfSoldiers[i].BoardSigned == eBoardKey.U)
                    {
                        refPlayerToDeleteDataFrom.NumOfKings -= 1;
                    }
                    else
                    {
                        refPlayerToDeleteDataFrom.NumOfSoldiers -= 1;
                    }

                    refPlayerToDeleteDataFrom.CellsOfSoldiers.Remove(refPlayerToDeleteDataFrom.CellsOfSoldiers[i]);
                    break;
                }
            }
        }

        public void MoveSoldier(Cell i_From, Player io_Player, Cell i_To)
        {
            int toRow = i_To.Point.Row;
            int toCol = i_To.Point.Col;
            int fromRow = i_From.Point.Row;
            int fromCol = i_From.Point.Col;
            eBoardKey newKey;

            // turn O soldier to king
            if (i_From.BoardSigned == eBoardKey.O && toRow == Board.Size - 1)
            {
                newKey = eBoardKey.U;
                ChangeSoldierToKing(i_From, io_Player, newKey);
                io_Player.NumOfSoldiers -= 1;
                io_Player.NumOfKings += 1;
            }

            // turn X soldier to king
            else if (i_From.BoardSigned == eBoardKey.X && toRow == 0)
            {
                newKey = eBoardKey.K;
                ChangeSoldierToKing(i_From, io_Player, newKey);
                io_Player.NumOfSoldiers -= 1;
                io_Player.NumOfKings += 1;
            }

            // stay the same key
            else
            {
                newKey = i_From.BoardSigned;
            }

            Board[toRow, toCol].BoardSigned = newKey; // update board
            Board[fromRow, fromCol].BoardSigned = eBoardKey.BLANK;

            // update player
            for (int i = 0; i < io_Player.CellsOfSoldiers.Count; i++)
            {
                if (io_Player.CellsOfSoldiers[i].Point.Equals(i_From.Point) == true)
                {
                    io_Player.CellsOfSoldiers.Remove(io_Player.CellsOfSoldiers[i]);
                    io_Player.CellsOfSoldiers.Add(new Cell(toRow, toCol, newKey));
                    break;
                }
            }
        }

        public void ChangeSoldierToKing(Cell i_From, Player io_Player, eBoardKey i_NewKey)
        {
            for (int i = 0; i < io_Player.CellsOfSoldiers.Count; i++)
            {
                if (io_Player.CellsOfSoldiers[i].Equals(i_From) == true)
                {
                    io_Player.CellsOfSoldiers[i].BoardSigned = i_NewKey;
                    break;
                }
            }
        }

        public void GetPossibleMovesUpperSoldier(Cell i_From, List<Cell> io_CellsOptions, int i_Flag)
        {
            eBoardKey playerKey = i_From.BoardSigned;
            int row = i_From.Point.Row;
            int col = i_From.Point.Col;
            eBoardKey keyEnemySoldier;
            eBoardKey keyEnemyKing;
            eBoardKey keyBlank = eBoardKey.BLANK;
            eBoardKey keyOking = eBoardKey.U;
            eBoardKey keyXking = eBoardKey.K;
            eBoardKey keyXsoldier = eBoardKey.X;
            eBoardKey keyOsoldier = eBoardKey.O;
            Player currPlayer;
            if (i_Flag == 0)
            {
                if (PlayerOne.Turn == true)
                {
                    currPlayer = PlayerOne;
                }
                else
                {
                    currPlayer = PlayerTwo;
                }
            }
            else
            {
                if (i_Flag == 1)
                {
                    currPlayer = PlayerOne;
                }
                else
                {
                    currPlayer = PlayerTwo;
                }
            }

            if (playerKey == keyXking || playerKey == keyXsoldier)
            {
                keyEnemySoldier = keyOsoldier;
                keyEnemyKing = keyOking;
            }
            else
            {
                keyEnemySoldier = keyXsoldier;
                keyEnemyKing = keyXking;
            }

            // not in the first row to avoid boundry overflow
            if (row != 0)
            {
                if (col != 0)
                {
                    // checking left top cell
                    if (Board[row - 1, col - 1].BoardSigned == keyBlank)
                    {
                        io_CellsOptions.Add(Board[row - 1, col - 1]);
                    }
                    else
                    {
                        // checks if top left is enemy
                        if (Board[row - 1, col - 1].BoardSigned == keyEnemySoldier || Board[row - 1, col - 1].BoardSigned == keyEnemyKing)
                        {
                            if (col != 1 && row != 1)
                            {
                                // checks for blank after enemy soldier
                                if (Board[row - 2, col - 2].BoardSigned == keyBlank)
                                {
                                    io_CellsOptions.Add(Board[row - 2, col - 2]);
                                    Cell cellToCheck = new Cell(row - 2, col - 2, keyBlank);
                                    if (CheckIfCellIsInList(currPlayer.EatingCellsOptions, cellToCheck) == false)
                                    {
                                        currPlayer.EatingCellsOptions.Add(Board[row - 2, col - 2]);
                                    }
                                }
                            }
                        }
                    }
                }

                // not in the last col
                if (col != Board.Size - 1)
                {
                    // checks if top right is blank
                    if (Board[row - 1, col + 1].BoardSigned == keyBlank)
                    {
                        io_CellsOptions.Add(Board[row - 1, col + 1]);
                    }
                    else
                    {
                        // checks if top right is enemy
                        if (Board[row - 1, col + 1].BoardSigned == keyEnemySoldier || Board[row - 1, col + 1].BoardSigned == keyEnemyKing)
                        {
                            if (row != 1 && col != Board.Size - 2)
                            {
                                // checks for blank after enemy soldier
                                if (Board[row - 2, col + 2].BoardSigned == keyBlank)
                                {
                                    io_CellsOptions.Add(Board[row - 2, col + 2]);
                                    Cell cellToCheck = new Cell(row - 2, col + 2, keyBlank);

                                    if (CheckIfCellIsInList(currPlayer.EatingCellsOptions, cellToCheck) == false)
                                    {
                                        currPlayer.EatingCellsOptions.Add(Board[row - 2, col + 2]);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public void GetPossibleMovesLowerSoldier(Cell i_From, List<Cell> io_CellsOptions, int i_Flag)
        {
            eBoardKey playerKey = i_From.BoardSigned;
            int row = i_From.Point.Row;
            int col = i_From.Point.Col;
            eBoardKey keyEnemySoldier;
            eBoardKey keyEnemyKing;
            eBoardKey keyBlank = eBoardKey.BLANK;
            eBoardKey keyOking = eBoardKey.U;
            eBoardKey keyXking = eBoardKey.K;
            eBoardKey keyXsoldier = eBoardKey.X;
            eBoardKey keyOsoldier = eBoardKey.O;
            Player currPlayer;
            if (i_Flag == 0)
            {
                if (PlayerOne.Turn == true)
                {
                    currPlayer = PlayerOne;
                }
                else
                {
                    currPlayer = PlayerTwo;
                }
            }
            else
            {
                if (i_Flag == 1)
                {
                    currPlayer = PlayerOne;
                }
                else
                {
                    currPlayer = PlayerTwo;
                }
            }

            if (playerKey == keyXking || playerKey == keyXsoldier)
            {
                keyEnemySoldier = keyOsoldier;
                keyEnemyKing = keyOking;
            }
            else
            {
                keyEnemySoldier = keyXsoldier;
                keyEnemyKing = keyXking;
            }

            // not in the Last row to avoid boundry overflow
            if (row != Board.Size - 1)
            {
                if (col != 0)
                {
                    // checking Bot left cell is blank
                    if (Board[row + 1, col - 1].BoardSigned == keyBlank)
                    {
                        io_CellsOptions.Add(Board[row + 1, col - 1]);
                    }
                    else
                    {
                        // checks if Bot left cell is enemy
                        if (Board[row + 1, col - 1].BoardSigned == keyEnemySoldier || Board[row + 1, col - 1].BoardSigned == keyEnemyKing)
                        {
                            if (row != Board.Size - 2 && col != 1)
                            {
                                // checks for blank after enemy soldier
                                if (Board[row + 2, col - 2].BoardSigned == keyBlank)
                                {
                                    io_CellsOptions.Add(Board[row + 2, col - 2]);
                                    Cell cellToCheck = new Cell(row + 2, col - 2, keyBlank);

                                    if (CheckIfCellIsInList(currPlayer.EatingCellsOptions, cellToCheck) == false)
                                    {
                                        currPlayer.EatingCellsOptions.Add(Board[row + 2, col - 2]);
                                    }
                                }
                            }
                        }
                    }
                }

                // not in the last col
                if (col != Board.Size - 1)
                {
                    // checks if Bot right is blank
                    if (Board[row + 1, col + 1].BoardSigned == keyBlank)
                    {
                        io_CellsOptions.Add(Board[row + 1, col + 1]);
                    }
                    else
                    {
                        // checks if down right is enemy
                        if (Board[row + 1, col + 1].BoardSigned == keyEnemySoldier || Board[row + 1, col + 1].BoardSigned == keyEnemyKing)
                        {
                            if (row != Board.Size - 2 && col != Board.Size - 2)
                            {
                                // checks for blank after enemy soldier
                                if (Board[row + 2, col + 2].BoardSigned == keyBlank)
                                {
                                    io_CellsOptions.Add(Board[row + 2, col + 2]);
                                    Cell cellToCheck = new Cell(row + 2, col + 2, keyBlank);

                                    if (CheckIfCellIsInList(currPlayer.EatingCellsOptions, cellToCheck) == false)
                                    {
                                        currPlayer.EatingCellsOptions.Add(Board[row + 2, col + 2]);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public void GetPossibleKingMove(Cell i_From, List<Cell> io_CellsOptions, int i_Flag)
        {
            GetPossibleMovesLowerSoldier(i_From, io_CellsOptions, i_Flag);
            GetPossibleMovesUpperSoldier(i_From, io_CellsOptions, i_Flag);
        }

        public void ResetGame()
        {
            Board newBoard = new Board(Board.Size);
            int numOfSoldier = CalcNumOfSoldiers(newBoard.Size);
            ResetPlayer(PlayerOne, numOfSoldier, true);
            ResetPlayer(PlayerTwo, numOfSoldier, false);
            m_Board = newBoard;
            InitPlayersListOfSoldiersCells();
        }

        public void InitPlayersListOfSoldiersCells()
        {
            for (int row = 0; row < Board.Size; row++)
            {
                for (int col = 0; col < Board.Size; col++)
                {
                    if (Board[row, col].BoardSigned == eBoardKey.O)
                    {
                        PlayerOne.CellsOfSoldiers.Add(new Cell(row, col, eBoardKey.O));
                    }
                    else if (Board[row, col].BoardSigned == eBoardKey.X)
                    {
                        PlayerTwo.CellsOfSoldiers.Add(new Cell(row, col, eBoardKey.X));
                    }
                }
            }
        }

        public int CalcNumOfSoldiers(int i_SizeOfBoard)
        {
            int res = 0;
            if (i_SizeOfBoard == 6)
            {
                res = 6;
            }

            if (i_SizeOfBoard == 8)
            {
                res = 12;
            }

            if (i_SizeOfBoard == 10)
            {
                res = 20;
            }

            return res;
        }

        public int CalcNumOfValidMoves(int i_SizeOfBoard)
        {
            int res = 0;
            if (i_SizeOfBoard == 6)
            {
                res = 5;
            }

            if (i_SizeOfBoard == 8)
            {
                res = 7;
            }

            if (i_SizeOfBoard == 10)
            {
                res = 9;
            }

            return res;
        }

        public int CalcPointsRewardFromGame(Player i_CurrPlayerWon, Player i_CurrPlayerLost)
        {
            int sum = 0;
            sum += i_CurrPlayerWon.NumOfSoldiers + (i_CurrPlayerWon.NumOfKings * 4);
            sum -= i_CurrPlayerLost.NumOfSoldiers - (i_CurrPlayerLost.NumOfKings * 4);
            return sum;
        }

        public bool CheckIfCellIsInList(List<Cell> io_PlayerListToCheck, Cell i_Cell)
        {
            bool valid = false;
            for (int i = 0; i < io_PlayerListToCheck.Count; i++)
            {
                if (io_PlayerListToCheck[i].Equals(i_Cell) == true)
                {
                    valid = true;
                    break;
                }
            }

            return valid;
        }

        public void RemoveSoldierFromEatingList(Player io_Player, Cell io_To)
        {
            for (int i = 0; i < io_Player.EatingCellsOptions.Count; i++)
            {
                if (io_Player.EatingCellsOptions[i].Equals(io_To))
                {
                    io_Player.EatingCellsOptions.Remove(io_Player.EatingCellsOptions[i]);
                }
            }
        }

        public bool HaveAnotherMoveToEatButWithSameSoldier(Cell i_To)
        {
            bool valid = false;
            Player currPlayer;
            if (m_PlayerOne.Turn == true)
            {
                currPlayer = m_PlayerOne;
            }
            else
            {
                currPlayer = m_PlayerTwo;
            }

            List<Cell> currMovesOptionOfSoldier;
            currMovesOptionOfSoldier = GetPossibleMovesOfSoldier(i_To, 0);
            for (int k = 0; k < currMovesOptionOfSoldier.Count; k++)
            {
                if (CheckIfCellIsInList(currPlayer.EatingCellsOptions, currMovesOptionOfSoldier[k]) == true)
                {
                    valid = true;
                }
            }

            return valid;
        }

        public void FlipTurns()
        {
            if (PlayerOne.Turn == true)
            {
                PlayerOne.Turn = false;
                PlayerTwo.Turn = true;
            }
            else
            {
                PlayerOne.Turn = true;
                PlayerTwo.Turn = false;
            }
        }

        public void PlayerWinARound(Player io_CurrPlayerWon, Player io_CurrPlayerLost)
        {
            int pointsGained = CalcPointsRewardFromGame(io_CurrPlayerWon, io_CurrPlayerLost);
            io_CurrPlayerWon.TotalScore += pointsGained;
        }

        public void ResetPlayer(Player io_Player, int i_NumOfSoldier, bool i_Turn)
        {
            io_Player.AllValidMoves.Clear();
            io_Player.CellsOfSoldiers.Clear();
            io_Player.EatingCellsOptions.Clear();
            io_Player.Turn = i_Turn;
            io_Player.NumOfSoldiers = i_NumOfSoldier;
            io_Player.NumOfKings = 0;
        }

        public void MakeRandomMove(ref Cell io_FromCell, ref Cell io_ToCell, Cell io_HaveToStartFrom)
        {
            List<Cell> possibleMoves;
            bool valid = false;

            // have anyone to eat
            if (m_PlayerTwo.EatingCellsOptions.Count != 0)
            {
                // have to eat again with the same soldier
                if (io_HaveToStartFrom != null)
                {
                    io_FromCell = io_HaveToStartFrom;
                    possibleMoves = GetPossibleMovesOfSoldier(io_FromCell, 0);
                    if (possibleMoves.Count != 0)
                    {
                        for (int k = 0; k < possibleMoves.Count; k++)
                        {
                            if (CheckIfCellIsInList(m_PlayerTwo.EatingCellsOptions, possibleMoves[k]) == true)
                            {
                                io_ToCell = possibleMoves[k];
                                break;
                            }
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < m_PlayerTwo.CellsOfSoldiers.Count && valid == false; i++)
                    {
                        io_FromCell = m_PlayerTwo.CellsOfSoldiers[i];
                        possibleMoves = GetPossibleMovesOfSoldier(io_FromCell, 0);
                        if (possibleMoves.Count != 0)
                        {
                            for (int k = 0; k < possibleMoves.Count; k++)
                            {
                                if (isEatMove(io_FromCell, possibleMoves[k]) && CheckIfCellIsInList(m_PlayerTwo.EatingCellsOptions, possibleMoves[k]) == true)
                                {
                                    valid = true;
                                    io_ToCell = possibleMoves[k];
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                Random rand = new Random();
                int cellNum, moveNum;
                for (int i = 0; i < m_PlayerTwo.CellsOfSoldiers.Count; i++)
                {
                    cellNum = rand.Next(m_PlayerTwo.CellsOfSoldiers.Count);
                    io_FromCell = m_PlayerTwo.CellsOfSoldiers[cellNum];
                    possibleMoves = GetPossibleMovesOfSoldier(io_FromCell, 0);
                    if (possibleMoves.Count != 0)
                    {
                        moveNum = rand.Next(possibleMoves.Count);
                        io_ToCell = possibleMoves[moveNum];
                        break;
                    }
                }
            }
        }

        public bool isEatMove(Cell i_FromCell, Cell i_To)
        {
            return Math.Abs(i_FromCell.Point.Row - i_To.Point.Row) == 2;
        }

        public void OnBoardUpdate(BoardUpdatedEventArgs e)
        {
            if(BoardUpdated != null)
            {
                BoardUpdated.Invoke(this, e);
            }
        }

        public void OnInvalidMove(InvalidMoveEventArgs e)
        {
            if (InvalidMove != null)
            {
                InvalidMove.Invoke(this, e);
            }
        }

        public bool isMySign(Player i_PlayerToMove, eBoardKey i_BoardSign)
        {
            eBoardKey soldier, king;

            if (i_BoardSign == eBoardKey.O || i_BoardSign == eBoardKey.U)
            {
                soldier = eBoardKey.O;
                king = eBoardKey.U;
            }
            else
            {
                soldier = eBoardKey.X;
                king = eBoardKey.K;
            }

            return i_PlayerToMove.CellsOfSoldiers[0].BoardSigned == soldier || i_PlayerToMove.CellsOfSoldiers[0].BoardSigned == king;
        }

        public void checkIfGameFinished()
        {
            Player currPlayerWon, currPlayerLost;
            if (PlayerOne.AllValidMoves.Count == 0 || PlayerTwo.AllValidMoves.Count == 0)
            {
                if (PlayerOne.Turn == true)
                {
                    currPlayerLost = PlayerOne;
                    currPlayerWon = PlayerTwo;
                }

                // playerOne Won
                else
                {
                    currPlayerLost = PlayerTwo;
                    currPlayerWon = PlayerOne;
                }

                PlayerWinARound(currPlayerWon, currPlayerLost);
                FinishedGame = true;
                OnGameFinish();
                
            }

         
        }

        public void OnGameFinish()
        {
            if (GameFinished != null)
            {
                GameFinished.Invoke(this, new EventArgs());
            }
        }
    }
}
