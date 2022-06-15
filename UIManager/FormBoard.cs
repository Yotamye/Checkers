using System;
using System.Drawing;
using GameLogic;
using System.Windows.Forms;

namespace UIManager
{
    public partial class FormBoard : Form
    {
        private ButtonBoard[,] m_ButtonBoards;
        private const int k_ButtonSize = 50;
        private Label m_PlayerOneName = new Label();
        private Label m_PlayerTwoName = new Label();
        private bool m_IsToCell = false;
        private ButtonBoard m_FromButton;

        public event JustMovedEventHandler JustMoved;

        public FormBoard()
        {
            InitializeComponent();
        }

        private void ButtonBoard_Clicked(object sender, EventArgs e)
        {
            ButtonBoard button = sender as ButtonBoard;
            JustMovedEventArgs moveEventArgs = null;
            if (m_IsToCell)
            {
                if (button.Text == string.Empty)
                {
                    Game.eBoardKey sign = getSign(m_FromButton.Text);
                    Cell from = new Cell(m_FromButton.Point.X, m_FromButton.Point.Y, sign);
                    sign = getSign(button.Text);
                    Cell to = new Cell(button.Point.X, button.Point.Y, sign);

                    moveEventArgs = new JustMovedEventArgs(from, to);
                    m_FromButton.BackColor = SystemColors.Control;
                    m_IsToCell = false;
                    if (JustMoved != null)
                    {
                        JustMoved.Invoke(this, moveEventArgs);
                    }
                }
                else if(button.Text == m_FromButton.Text)
                {
                    m_FromButton.BackColor = SystemColors.Control;
                    m_FromButton = button;
                    m_FromButton.BackColor = SystemColors.ActiveCaption;
                    m_IsToCell = true;
                }
            }
            else
            {
                if (button.Text != string.Empty)
                {
                    m_FromButton = button;
                    m_FromButton.BackColor = SystemColors.ActiveCaption;
                    m_IsToCell = true;
                }
            }
        }

        public void UpdateBouttonBoard(BoardUpdatedEventArgs e)
        {
            m_ButtonBoards[e.To.Point.Row, e.To.Point.Col].Text = m_ButtonBoards[e.From.Point.Row, e.From.Point.Col].Text;
            m_ButtonBoards[e.From.Point.Row, e.From.Point.Col].Text = string.Empty;
            if (e.JustEat)
            {
                m_ButtonBoards[e.ToDelete.Point.Row, e.ToDelete.Point.Col].Text = string.Empty;
            }

            if (e.To.Point.Row == 0 && m_ButtonBoards[e.To.Point.Row, e.To.Point.Col].Text == "X")
            {
                m_ButtonBoards[e.To.Point.Row, e.To.Point.Col].Text = "K";
            }

            if (e.To.Point.Row == e.GameBoardSize - 1 && m_ButtonBoards[e.To.Point.Row, e.To.Point.Col].Text == "O")
            {
                m_ButtonBoards[e.To.Point.Row, e.To.Point.Col].Text = "U";
            }
        }

        public void createButtonBoardMat(Board i_Board)
        {
            bool newLine = false;
            bool isFirstButton = true;
            ButtonBoard lastButton = new ButtonBoard(0, 0);
            m_ButtonBoards = new ButtonBoard[i_Board.Size, i_Board.Size];
            for (int i = 0; i < i_Board.Size; i++)
            {
                for (int j = 0; j < i_Board.Size; j++)
                {
                    m_ButtonBoards[i, j] = new ButtonBoard(i, j);
                    m_ButtonBoards[i, j].Size = new Size(k_ButtonSize, k_ButtonSize);
                    setButtonLocation(m_ButtonBoards[i, j], newLine, isFirstButton, lastButton, i_Board.Size);
                    m_ButtonBoards[i, j].Text = i_Board[i, j].BoardSigned.ToString();
                    m_ButtonBoards[i, j].Click += ButtonBoard_Clicked;

                    if (i_Board[i, j].BoardSigned == Game.eBoardKey.BLANK)
                    {
                        m_ButtonBoards[i, j].Text = string.Empty;
                    }

                    if((j % 2 == 0 && i % 2 == 0) || (j % 2 == 1 && i % 2 == 1))
                    {
                        m_ButtonBoards[i, j].BackColor = Color.Gray;
                    }

                    this.Controls.Add(m_ButtonBoards[i, j]);
                    lastButton = m_ButtonBoards[i, j];
                    newLine = false;
                    isFirstButton = false;
                }

                newLine = true;
            }
        }

        private void setButtonLocation(ButtonBoard io_CurrentButton, bool i_NewLine, bool i_IsFirstButton, ButtonBoard i_LastButton, int i_BoardSize)
        {
            System.Drawing.Point newLocation;
            int pictureBoxMatrixMaxLine = i_BoardSize - 1;
            int pictureBoxMatrixMaxCol = i_BoardSize - 1;

            if (i_IsFirstButton)
            {
                newLocation = new System.Drawing.Point(0, 40);
            }
            else
            {
                newLocation = i_LastButton.Location;
                if (!i_NewLine)
                {
                    newLocation.Offset(i_LastButton.Width, 0);
                }
                else
                {
                    newLocation.X = 0;
                    newLocation.Offset(0, i_LastButton.Height);
                }
            }

            io_CurrentButton.Location = newLocation;
        }

        public void setLabelsLocations()
        {
            m_PlayerOneName.Location = new System.Drawing.Point(100, 10);
            m_PlayerTwoName.Location = new System.Drawing.Point(200, 10);
        }

        public void createLabels(Player i_PlayerOne, Player i_PlayerTwo)
        {
            m_PlayerOneName.Text = string.Format("{0}: {1}", i_PlayerOne.Name, i_PlayerOne.TotalScore);
            m_PlayerTwoName.Text = string.Format("{0}: {1}", i_PlayerTwo.Name, i_PlayerTwo.TotalScore);
            setLabelsLocations();
            this.Controls.Add(m_PlayerOneName);
            this.Controls.Add(m_PlayerTwoName);

        }

        public void setFormBoardSize(int i_BoardSize)
        {
            Height = (k_ButtonSize * i_BoardSize) + 80;
            Width = (k_ButtonSize * i_BoardSize) + 20;
        }

        public void AlertUser(string i_MessageToShow)
        {
            MessageBox.Show(i_MessageToShow);
        }

        public ButtonBoard FromButton
        {
            get
            {
                return m_FromButton;
            }
        }

        public Game.eBoardKey getSign(string i_Sign)
        {
            switch (i_Sign)
            {
                case "X":
                    return Game.eBoardKey.X;
                case "O":
                    return Game.eBoardKey.O;
                case "U":
                    return Game.eBoardKey.U;
                case "K":
                    return Game.eBoardKey.K;
                default:
                    return Game.eBoardKey.BLANK;

            }
        }
    }
}
