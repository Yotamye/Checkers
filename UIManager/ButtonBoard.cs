using System.Windows.Forms;
using System.Drawing;

namespace UIManager
{
    public class ButtonBoard : Button
    {
        private Point m_Point;

        public ButtonBoard(int i_X, int i_Y)
        {
            m_Point = new Point(i_X, i_Y);
        }

        public Point Point
        {
            get
            {
                return m_Point;
            }

            set
            {
                m_Point = value;
            }

        }
    }

}
