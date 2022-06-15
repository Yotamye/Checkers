using System;

namespace GameLogic
{
    public delegate void BoardUpdatedEventHandler(object sender, BoardUpdatedEventArgs e);

    public class BoardUpdatedEventArgs : EventArgs
    {
        private Cell m_From;
        private Cell m_To;
        private bool m_JustEat;
        private Cell m_ToDelete;
        private int m_GameBoardSize;

        public BoardUpdatedEventArgs(Cell i_From, Cell i_To, bool i_JustEat, Cell i_ToDelete, int i_GameBoardSize)
        {
            m_From = i_From;
            m_To = i_To;
            m_JustEat = i_JustEat;
            m_ToDelete = i_ToDelete;
            m_GameBoardSize = i_GameBoardSize;
        }

        public Cell From
        {
            get
            {
                return m_From;
            }
        }

        public Cell To
        {
            get
            {
                return m_To;
            }
        }

        public bool JustEat
        {
            get
            {
                return m_JustEat;
            }
        }

        public Cell ToDelete
        {
            get
            {
                return m_ToDelete;
            }
        }

        public int GameBoardSize
        {
            get
            {
                return m_GameBoardSize;
            }
        }
    }
}
