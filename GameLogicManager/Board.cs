namespace GameLogic
{
    public class Board
    {
        private readonly Cell[,] m_Board;
        private int m_Size;

        public Board(int i_SizeOfBoard)
        {
            m_Size = i_SizeOfBoard;
            int lineAmountOfSoldiers = (i_SizeOfBoard / 2) - 1;
            m_Board = new Cell[i_SizeOfBoard, i_SizeOfBoard];

            // init i,j for all cells
            for (int i = 0; i < i_SizeOfBoard; i++)
            {
                for (int j = 0; j < i_SizeOfBoard; j++)
                {
                    m_Board[i, j] = new Cell(i, j, Game.eBoardKey.BLANK);
                }
            }

            // init keys to required cells
            for (int i = 0; i < i_SizeOfBoard; i++)
            {
                for (int j = 0; j < i_SizeOfBoard; j += 2)
                {
                    // O lines
                    if (i < lineAmountOfSoldiers)
                    {
                        if (i % 2 == 0 && j == 0)
                        {
                            j++;
                        }

                        m_Board[i, j].BoardSigned = Game.eBoardKey.O;
                    }

                    // X lines
                    else if (i > lineAmountOfSoldiers + 1)
                    {
                        if (i % 2 == 0 && j == 0)
                        {
                            j++;
                        }

                        m_Board[i, j].BoardSigned = Game.eBoardKey.X;
                    }
                }
            }
        }

        public int Size
        {
            get { return m_Size; }
            set { m_Size = value; }
        }

        public Cell this[int i_Index, int i_Jindex]
        {
            get { return m_Board[i_Index, i_Jindex]; }
            set { m_Board[i_Index, i_Jindex] = value; }
        }
    }
}
