using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace simple_tetris
{
    public partial class Form1 : Form
    {
        string direction = "down";
        int boxSize = 32;
        int playWidth = 5;
        int playHeight = 10;
        int score = 0;


        PictureBox current = null;
        List<PictureBox> boxes = new List<PictureBox>();
        public Form1()
        {
           
            InitializeComponent();
            this.KeyPreview = true;
        }
        bool IsCollidingWithAny(PictureBox box)
        {
            foreach (PictureBox other in boxes)
            {
                if (other != box && box.Bounds.IntersectsWith(other.Bounds))
                    return true;
            }
            return false;
        }

         void addBox()
        {
            current = new PictureBox();
            current.Size = new Size(boxSize, boxSize);
            current.Location = new Point(label1.Left + 64, label1.Top); // Ensure this starting position is correct for your game setup.
            current.BackColor = Color.Black;
            Controls.Add(current);

            boxes.Add(current);  // Add the new box to the list

            if (IsCollidingWithAny(current))
            {
                GameOver(); // Handle game over condition
            }

            label1.SendToBack();
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            Debug.WriteLine("Key Pressed: " + e.KeyChar);


            if (e.KeyChar == 'a')
            {
                direction = "left";
            }
            else if (e.KeyChar == 'd')
            {
                direction = "right";
            }
            this.KeyPress += new KeyPressEventHandler(Form1_KeyPress);
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            label1.Height = boxSize * playHeight;
            label1.Width = boxSize * playWidth;
            ClientSize = new Size(button1.Left + button1.Width, label1.Height);

            label2.Text = "Score: 0"; // Initialize the score label
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (current == null)
            {
                addBox();
            }
            else
            {
                current.Top += boxSize;
                if (IsCollidingWithAny(current) || current.Top + current.Height > label1.Top + label1.Height)
                {
                    current.Top -= boxSize;  // Revert move if collision occurs
                    addBox();
                }

                int previousLeft = current.Left;
                if (direction == "right")
                {
                    current.Left += boxSize;
                }
                else if (direction == "left")
                {
                    current.Left -= boxSize;
                }

                if (IsCollidingWithAny(current) || current.Left < 0 || current.Left + current.Width > label1.Width)
                {
                    current.Left = previousLeft;  // Revert move if collision occurs
                }

                direction = null;  // Reset direction after moving

                CheckAndRemoveCompleteRows();  // Check for complete rows and remove them
            }
        }
        void GameOver()
        {
            timer1.Stop(); // Stop the game loop
            DialogResult dialogResult = MessageBox.Show("Game Over! Your final score is: " + score + "\nWould you like to restart?", "Game Over", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (dialogResult == DialogResult.Yes)
            {
                RestartGame();
            }
            else
            {
                this.Close(); // Close the game form
            }
        }

        void RestartGame()
        {
            foreach (PictureBox box in boxes)
            {
                Controls.Remove(box);
                box.Dispose();
            }
            boxes.Clear();
            score = 0;
            label2.Text = "Score: 0";
            addBox();
            timer1.Start(); // Restart the timer
        }




        void CheckAndRemoveCompleteRows()
        {
            Dictionary<int, List<PictureBox>> rows = new Dictionary<int, List<PictureBox>>();

            foreach (PictureBox box in boxes)
            {
                int row = box.Top;
                if (!rows.ContainsKey(row))
                {
                    rows[row] = new List<PictureBox>();
                }
                rows[row].Add(box);
            }

            List<int> rowsToRemove = new List<int>();
            foreach (var row in rows)
            {
                if (row.Value.Count == playWidth) // Assuming playWidth is 5, meaning row is full
                {
                    rowsToRemove.Add(row.Key);
                }
            }

            foreach (int row in rowsToRemove)
            {
                foreach (PictureBox box in rows[row])
                {
                    Controls.Remove(box);
                    box.Dispose();
                }
                boxes.RemoveAll(box => box.Top == row);
            }

            // Update the score and move down all boxes above each removed row
            if (rowsToRemove.Count > 0)
            {
                score += rowsToRemove.Count; // Add points per cleared row
                label2.Text = "Score: " + score; // Update the score display
            }

            rowsToRemove.Sort();
            foreach (int removedRow in rowsToRemove)
            {
                foreach (PictureBox box in boxes.Where(box => box.Top < removedRow))
                {
                    box.Top += boxSize;
                }
            }
        }




        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            // Sätt fokus på fönstret, det ser till att saker så som Form_KeyPress fungerar som det ska
            Focus();
            timer1.Start();
        }

        
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
