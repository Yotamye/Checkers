using System;

namespace GameLogic
{
    public class Cell
    {
        private Point m_Point;
        private Game.eBoardKey m_BoardSign;

        public Cell(int i_Row, int i_Col, Game.eBoardKey i_Key)
        {
            m_Point = new Point(i_Row, i_Col);
            m_BoardSign = i_Key;
        }

        public Game.eBoardKey BoardSigned
        {
            get { return m_BoardSign; }
            set { m_BoardSign = value; }
        }

        public Point Point
        {
            get { return m_Point; }
            set { m_Point = value; }
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            else
            {
                Cell cell = (Cell)obj;
                return m_Point.Equals(cell.m_Point) && (BoardSigned == cell.BoardSigned);
            }
        }

        public override int GetHashCode()
        {
            return Tuple.Create(m_Point, m_BoardSign).GetHashCode();
        }
    }
}
