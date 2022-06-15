using System;
using System.Windows.Forms;

namespace UIManager
{
    public partial class FormGameSettings : Form
    {
        public FormGameSettings()
        {
            InitializeComponent();
            this.buttonDone.Click += new EventHandler(ButtonDone_Click);
        }

        private void ButtonDone_Click(object sender, EventArgs e)
        {
            if (textBoxPlayer1.Text != string.Empty || (textBoxPlayer2.Enabled == true && textBoxPlayer2.Text != string.Empty))
            {
                if (textBoxPlayer2.Enabled == false)
                {
                    textBoxPlayer2.Text = "Computer";
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Invalid Input!");
            }
        }

        public int BoardSize
        {
            get
            {
                int res;
                if (radioButtonSize6.Checked)
                {
                    res = 6;
                }
                else if (radioButtonSize8.Checked)
                {
                    res = 8;
                }
                else
                {
                    res = 10;
                }

                return res;
            }
        }

        public string PlayerOneName
        {
            get
            {
                return textBoxPlayer1.Text;
            }
        }

        public string PlayerTwoName
        {
            get
            {
                return textBoxPlayer2.Text;
            }

        }

        public bool checkBoxPlayerTwo
        {
            get
            {
                return checkBoxPlayer2.Checked;
            }

        }

        private void checkBoxPlayer2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxPlayer2.Checked == true)
            {
                textBoxPlayer2.Enabled = true;
                textBoxPlayer2.Clear();
            }
        }
    }
}
