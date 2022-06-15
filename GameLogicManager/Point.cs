using System;

namespace GameLogic
{
    public class Point
    {
        private int m_Row;
        private int m_Col;

        public int Row
        {
            get { return m_Row; }
            set { m_Row = value; }
        }

        public int Col
        {
            get { return m_Col; }
            set { m_Col = value; }
        }

        public Point(int i_Row, int i_Col)
        {
            m_Row = i_Row;
            m_Col = i_Col;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            else
            {
                Point point = (Point)obj;
                return (m_Row == point.m_Row) && (m_Col == point.m_Col);
            }
        }

        public override int GetHashCode()
        {
            return Tuple.Create(m_Row, m_Col).GetHashCode();
        }
    }
}
