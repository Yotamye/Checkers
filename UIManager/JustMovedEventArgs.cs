using System;
using GameLogic;
namespace UIManager
{
    public delegate void JustMovedEventHandler(object sender, JustMovedEventArgs e);

    public class JustMovedEventArgs : EventArgs
    {
        private Cell m_From;
        private Cell m_To;

        public Cell From
        {
            get
            {
                return m_From;
            }

            set
            {
                m_From = value;
            }
        }

        public Cell To
        {
            get
            {
                return m_To;
            }

            set
            {
                m_To = value;
            }
        }

        public JustMovedEventArgs(Cell i_From, Cell i_To)
        {
            m_From = i_From;
            m_To = i_To;
        }
    }
}
