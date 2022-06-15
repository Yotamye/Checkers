using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
namespace UIManager
{
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            UILogicManager uiLogicManger = new UILogicManager();
            uiLogicManger.Run();
        }
    }
}
