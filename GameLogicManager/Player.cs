using System.Collections.Generic;

namespace GameLogic
{
    public class Player
    {
        private readonly string m_Name;
        private List<Cell> m_EatingCellsOptions = new List<Cell>();
        private List<Cell> m_CellsOfSoldiers = new List<Cell>();
        private readonly List<Cell> m_AllValidMoves = new List<Cell>();
        private bool m_Turn;
        private int m_NumOfSoldiers;
        private int m_NumOfKings = 0;
        private int m_TotalScore = 0;

        public Player(string i_Name, int i_NumOfSoldiers, bool i_Turn)
        {
            m_Name = i_Name;
            m_NumOfSoldiers = i_NumOfSoldiers;
            m_Turn = i_Turn;
        }

        public string Name
        {
            get { return m_Name; }
        }

        public List<Cell> EatingCellsOptions
        {
            get { return m_EatingCellsOptions; }
            set { m_EatingCellsOptions = value; }
        }

        public List<Cell> CellsOfSoldiers
        {
            get { return m_CellsOfSoldiers; }
            set { m_CellsOfSoldiers = value; }
        }

        public List<Cell> AllValidMoves
        {
            get { return m_AllValidMoves; }
        }

        public bool Turn
        {
            get { return m_Turn; }
            set { m_Turn = value; }
        }

        public int NumOfSoldiers
        {
            get { return m_NumOfSoldiers; }
            set { m_NumOfSoldiers = value; }
        }

        public int NumOfKings
        {
            get { return m_NumOfKings; }
            set { m_NumOfKings = value; }
        }

        public int TotalScore
        {
            get { return m_TotalScore; }
            set { m_TotalScore = value; }
        }
    }
}
