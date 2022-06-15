using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using GameLogic;

namespace UIManager
{
    public class UILogicManager
    {
        private readonly FormBoard r_FormBoard = new FormBoard();
        private readonly FormGameSettings r_FormGameSettings = new FormGameSettings();

        private Game m_Game;

        public UILogicManager()
        {
            r_FormGameSettings.FormClosed += new FormClosedEventHandler(GameSettingsFormClosed);
            r_FormGameSettings.ShowDialog();

            m_Game.InvalidMove += new InvalidMoveEventHandler(InvalidMove_EventHandler);
            r_FormBoard.JustMoved += new JustMovedEventHandler(JustMoved_EventHandler);
            m_Game.BoardUpdated += new BoardUpdatedEventHandler(BoardUpdated_EventHandler);
            m_Game.GameFinished += new EventHandler(GameFinished_EventHandler);
        }

        public void GameFinished_EventHandler(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show(prepareMessageToFinish(), "Checkers", MessageBoxButtons.YesNo);

            if(dialogResult == DialogResult.Yes)
            {
                r_FormBoard.Controls.Clear();
                restartGame();
                
            }
            else if(dialogResult == DialogResult.No)
            {
                 r_FormBoard.Close();
            }
        }

        public void restartGame()
        {
            m_Game.ResetGame();
            m_Game.FinishedGame = false;
            initFormGame();
        }

        private string prepareMessageToFinish()
        {
            StringBuilder msg = new StringBuilder();
            string name;
            if (m_Game.FinishedGame)
            {
                name = m_Game.PlayerOne.Turn ? m_Game.PlayerTwo.Name : m_Game.PlayerOne.Name;
                msg.Append(string.Format("The winner is {0} !", name));
                msg.Append(Environment.NewLine + "Another Round?");
            }

            return msg.ToString();
        }

        public void JustMoved_EventHandler(object sender, JustMovedEventArgs e)
        {
            m_Game.MovePlayerAndUpdateBoard(e.From, e.To);
        }

        public void InvalidMove_EventHandler(object sender, InvalidMoveEventArgs e)
        {
            r_FormBoard.AlertUser(e.Message);
        }

        public void BoardUpdated_EventHandler(object sender, BoardUpdatedEventArgs e)
        {
            r_FormBoard.UpdateBouttonBoard(e);
        }

        public void makeCurrentMove(ButtonBoard[,] m_ButtonBoards, ButtonBoard button)
        {
            List<Cell> possibleMoves = new List<Cell>();
            Player playerTurn;
            if (m_Game.PlayerOne.Turn == true)
            {
                playerTurn = m_Game.PlayerOne;
            }
            else
            {
                playerTurn = m_Game.PlayerTwo;
            }
        }

        public void GameSettingsFormClosed(object sender, EventArgs e)
        {
            initGame();
            initFormGame();
        }

        public void initFormGame()
        {
            r_FormBoard.setFormBoardSize(m_Game.Board.Size);
            r_FormBoard.createButtonBoardMat(m_Game.Board);
            r_FormBoard.createLabels(m_Game.PlayerOne, m_Game.PlayerTwo);
            r_FormBoard.Text = "Checkers";
        }

        public void initGame()
        {
            Game.eGameMode gameMode = Game.eGameMode.OnePlayer;
            if (r_FormGameSettings.checkBoxPlayerTwo == true)
            {
                gameMode = Game.eGameMode.TwoPlayer;
            }

            m_Game = new Game(r_FormGameSettings.BoardSize, r_FormGameSettings.PlayerOneName, r_FormGameSettings.PlayerTwoName, gameMode);
            m_Game.UpdateAllValidMoves();
        }

        public void Run()
        {
            r_FormBoard.ShowDialog();
        }
    }
}
