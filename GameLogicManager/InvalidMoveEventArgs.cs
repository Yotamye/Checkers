using System;

namespace GameLogic
{
    public delegate void InvalidMoveEventHandler(object sender, InvalidMoveEventArgs e);

    public class InvalidMoveEventArgs : EventArgs
    {
        private string m_Message;


        public InvalidMoveEventArgs(string i_Message)
        {
            m_Message = i_Message;
        }

        public string Message
        {
            get
            {
                return m_Message;
            }
        }
    }
}
