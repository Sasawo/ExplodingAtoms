using Microsoft.VisualBasic;
using Microsoft.VisualBasic.Logging;
using System.Diagnostics;
using System;
using System.Drawing;
using System.Reflection.Emit;
using System.Reflection.Metadata.Ecma335;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrayNotify;

namespace Exploding_atoms
{
    static class Shuffler
    {
        public static void Shuffle<T>(this Random rng, T[] array, int ceiling) //Used to shuffle arrays
        {
            int i = ceiling;
            while (i > 1)
            {
                int k = rng.Next(i--);
                T temp = array[i];
                array[i] = array[k];
                array[k] = temp;
            }
        }
    }
    public partial class Form1 : Form
    {
        //Initiating necessary variables
        static private readonly int maxLevel = 2;
        static private int AIturn = 1;
        static private string[] names = { "Blue", "Orange" };
        static private int playSize = 5;
        static private int minPlaySize = 2;
        static private int maxPlaySize = 10;
        static private int minTurnTime = 300;
        static private int playerCount = 2;
        static private int difficultyCount = 4;
        static private int boomWait = 5;
        static private int height = Screen.PrimaryScreen.Bounds.Height;
        static private int width = Screen.PrimaryScreen.Bounds.Width;
        private Random random = new Random();
        private Bitmap bitmap;
        private int[] skipper = new int[maxLevel / 2];
        private int[] minimaxExport;
        private int turn = 0;
        private bool count = false;
        private bool simplified = false;
        private bool opponent = true;
        private bool finish = false;
        private int counter = 0;
        private int winturn = 0;
        private int maxBooms = 20;
        private int hard = 0;
        private int[] scores = new int[playerCount];
        private int[,] resetField = new int[maxPlaySize, maxPlaySize];
        private int[,] DisksPlayed = new int[maxPlaySize, maxPlaySize];
        private int[,] Colors = new int[maxPlaySize, maxPlaySize];
        private int[,] Amounts = new int[maxPlaySize, maxPlaySize];
        private PictureBox BackGround = new PictureBox();
        //Loading necessary 'textures'
        private static string[] diffs = [@"../../../Resources/easy.jpg", @"../../../Resources/medium.jpg", 
            @"../../../Resources/hard.jpg", @"../../../Resources/impossible.jpg", @"../../../Resources/hardOff.jpg"];
        private Image playButton = Image.FromFile(@"../../../Resources/play.jpg");
        private Image optionsButton = Image.FromFile(@"../../../Resources/options.jpg");
        private Image exitButton = Image.FromFile(@"../../../Resources/exit.jpg");
        private Image backButton = Image.FromFile(@"../../../Resources/back.jpg");
        private Image backButton2 = Image.FromFile(@"../../../Resources/back.jpg");
        private Image logo = Image.FromFile(@"../../../Resources/logo.jpg");
        private Image space = Image.FromFile(@"../../../Resources/space.jpg");
        private Image blue = Image.FromFile(@"../../../Resources/blue.png");
        private Image orange = Image.FromFile(@"../../../Resources/orange.png");
        private Image simpleText = Image.FromFile(@"../../../Resources/option1.jpg");
        private Image size = Image.FromFile(@"../../../Resources/size.jpg");
        private Image tickBox = Image.FromFile(@"../../../Resources/unticked.jpg");
        private Image behind = Image.FromFile(@"../../../Resources/background.jpg");
        private Image enemy = Image.FromFile(@"../../../Resources/enemyAI.jpg");
        private Image diff = Image.FromFile(@"../../../Resources/difficulty.jpg");
        private Image replay = Image.FromFile(@"../../../Resources/replay.jpg");
        private Image diffSelect;
        //Loading necessary locations for 'textures'
        private Rectangle play = new Rectangle(width / 2 - height / 10, height / 2, height / 5, height / 12);
        private Rectangle options = new Rectangle(width / 2 - height / 10, height * 5 / 8, height * 2 / 10, height / 12);
        private Rectangle exit = new Rectangle(width / 2 - height / 10, height * 6 / 8, height * 2 / 10, height / 12);
        private Rectangle logoRect = new Rectangle(width / 2 - height * 2 / 5, height / 10, height * 4 / 5, height * 9 / 20);
        private Rectangle board = new Rectangle(width / 2 - height / 3, height / 6, height * 2/ 3, height * 2 / 3);
        private Rectangle option1 = new Rectangle(width / 2 - height / 2, height * 2 / 5, height * 4 / 5, height / 12);
        private Rectangle option2 = new Rectangle(width / 2 - height / 2, height * 3 / 5, height * 4 / 5, height / 12);
        private Rectangle option3 = new Rectangle(width / 2 - height / 2, height * 4 / 5, height * 4 / 5, height / 12);
        private Rectangle option4 = new Rectangle(width / 2 - height / 2, height / 5, height * 4 / 5, height / 12);
        private Rectangle minus = new Rectangle(width * 5 / 8 - height / 24, height / 5, height / 12, height / 12);
        private Rectangle plus = new Rectangle(width * 5 / 8 + height / 24, height / 5, height / 12, height / 12);
        private Rectangle sizeText = new Rectangle(width * 5 / 8, height / 5, height / 12, height / 12);
        private Rectangle difficulty = new Rectangle(width / 2, height * 4 / 5, height * 2 / 5, height / 12);
        private Rectangle box = new Rectangle(width * 5 / 8, height * 2 / 5, height / 12, height / 12);
        private Rectangle bG = new Rectangle(0, 0, width, height);
        private Rectangle back = new Rectangle(width - height / 20, 0, height / 20, height / 40);
        private Rectangle pts1 = new Rectangle(width * 3 / 4, height / 2 - height / 16, height / 8, height / 8);
        private Rectangle pts2 = new Rectangle(width * 3 / 4 + height / 7, height / 2 - height / 16, height / 8, height / 8);
        private Rectangle gameOver = new Rectangle(width * 3 / 8, height * 2 / 5, width / 4, height / 5);
        private Rectangle gameOverText = new Rectangle(width * 3 / 8, height * 2 / 5, width / 4, height / 8);
        private Rectangle retry = new Rectangle(width * 13 / 34, height / 2, width / 10, height / 20);
        private Rectangle leave = new Rectangle(width * 21 / 34 - width / 10, height / 2, width / 10, height / 20);
        private StringFormat format = new StringFormat();
        private Stopwatch stopWatch = new Stopwatch();
        private void Menu_Paint(object sender, PaintEventArgs draw) //Draws the menu screen
        {
            Graphics drawer = draw.Graphics;
            drawer.DrawImage(logo, logoRect);
            drawer.DrawImage(playButton, play);
            drawer.DrawImage(optionsButton, options);
            drawer.DrawImage(exitButton, exit);
        }
        private void Options_Paint(object sender, PaintEventArgs draw) //Draws the options screen
        {
            Graphics drawer = draw.Graphics;
            SolidBrush whitebrush = new SolidBrush(Color.White);
            Font font = new Font("Arial", height / 40);
            drawer.DrawImage(backButton, back);
            drawer.DrawImage(simpleText, option1);
            drawer.DrawImage(enemy, option2);
            drawer.DrawImage(diff, option3);
            drawer.DrawImage(size, option4);
            drawer.DrawImage(diffSelect, difficulty);
            drawer.DrawImage(tickBox, box);
            drawer.DrawString("-", font, whitebrush, minus, format);
            drawer.DrawString("+", font, whitebrush, plus, format);
            drawer.DrawString(string.Format("{0}", playSize), font, whitebrush, sizeText, format);
        }
        private void End_Paint(object sender, PaintEventArgs draw) //Draws the victory screen
        {
            SolidBrush blackbrush = new SolidBrush(Color.Black);
            SolidBrush whitebrush = new SolidBrush(Color.White);
            Font font = new Font("Arial", height / 40);
            Pen whitepen = new Pen(Color.White);
            whitepen.Width = 4.0F;
            Graphics drawer = draw.Graphics;
            drawer.FillRectangle(blackbrush, gameOver);
            drawer.DrawRectangle(whitepen, gameOver);
            drawer.DrawString(string.Format("{0} Wins", winturn == 0 ? "Blue" : "Orange"), font, whitebrush, gameOverText, format);
            drawer.DrawImage(replay, retry);
            drawer.DrawImage(backButton2, leave);
        }
        private void Create_Background() //Draws te game board and parts that need not be refreshed
        {
            bitmap = new Bitmap(width, height);
            using (Graphics drawer = Graphics.FromImage(bitmap))
            {
                Pen whitepen = new Pen(Color.White);
                whitepen.Width = 1.0F;
                Rectangle exit = new Rectangle(width - 100, 0, 100, 50);
                if (!simplified)
                {
                    drawer.DrawImage(behind, bG);
                    drawer.DrawImage(space, board);
                    drawer.DrawImage(blue, pts1);
                    drawer.DrawImage(orange, pts2);
                    whitepen.Width = 4.0F;
                }
                else
                {
                    Pen bluepen = new Pen(Color.Blue);
                    Pen orangepen = new Pen(Color.Orange);
                    drawer.DrawEllipse(bluepen , pts1);
                    drawer.DrawEllipse(orangepen, pts2);
                }
                for (int i = 0; i < playSize; i++)
                {
                    for (int j = 0; j < playSize; j++)
                    {
                        drawer.DrawRectangle(whitepen, new Rectangle(width / 2 - height / 3, height / 6, 
                            height * 2 / (3 * playSize) * (i + 1), height * 2 / (3 * playSize) * (j + 1)));
                    }
                }
            }
            BackGround.Image = null;
        }
        private void Form1_Paint_Simple(object sender, PaintEventArgs draw) //Draws atoms and counter during th game if 'SIMPLIFIED GRAPHICS' is on
        {
            int[,] DiskShift = { {0, 0}, { height / (4 * playSize), 0},
            { 0, height / (4 * playSize) }, { height / (4 * playSize), height / (4 * playSize) } };
            Graphics graph = draw.Graphics;
            Pen bluepen = new Pen(Color.Blue);
            Pen orangepen = new Pen(Color.Orange);
            SolidBrush whitebrush = new SolidBrush(Color.White);
            Font font = new Font("Arial", height / 40);
            for (int i = 0; i < playSize; i++)
            {
                for (int j = 0; j < playSize; j++)
                {
                    for (int k = 0; k < DisksPlayed[i, j] && k < 4; k++)
                    {
                        graph.DrawEllipse(Colors[i, j] == 1 ? bluepen : orangepen, width / 2 - height / 3 + height * 2 * i / (3 * playSize) + height / (13 * playSize) + DiskShift[k, 0],
                            height / 6 + height * 2 * j / (3 * playSize) + height / (13 * playSize) + DiskShift[k, 1],
                            height / (4 * playSize), height / (4 * playSize));
                    }

                }
            }
            graph.DrawString(string.Format("{0}", scores[0]), font, whitebrush, pts1, format);
            graph.DrawString(string.Format("{0}", scores[1]), font, whitebrush, pts2, format);
            graph.DrawImage(backButton, back);

        }
        private void Form1_Paint(object sender, PaintEventArgs draw) //Draws atoms and counter during th game if 'SIMPLIFIED GRAPHICS' is off
        {
            int[,] DiskShift = { {0, 0}, { height / (4 * playSize), 0},
            { 0, height / (4 * playSize) }, { height / (4 * playSize), height / (4 * playSize) } };
            Graphics graph = draw.Graphics;
            Font font = new Font("Arial", height / 40);
            SolidBrush whitebrush = new SolidBrush(Color.White);
            for (int i = 0; i < playSize; i++)
            {
                for (int j = 0; j < playSize; j++)
                {
                    for (int k = 0; k < DisksPlayed[i, j] && k < 4; k++)
                    {
                        Rectangle disk = new Rectangle(width / 2 - height / 3 + height * 2 * i / (3 * playSize) + height / (13 * playSize) + DiskShift[k, 0],
                            height / 6 + height * 2 * j / (3 * playSize) + height / (13 * playSize) + DiskShift[k, 1],
                            height / (4 * playSize), height / (4 * playSize));
                        graph.DrawImage(Colors[i, j] == 1 ? blue : orange, disk);
                    }

                }
            }
            graph.DrawString(string.Format("{0}", scores[0]), font, whitebrush, pts1, format);
            graph.DrawString(string.Format("{0}", scores[1]), font, whitebrush, pts2, format);
            graph.DrawImage(backButton, back);
        }
        private void Amount_Fill() //Since the board size can be changed, the maximum amount of atoms in each box is calculated here
        {
            for (int i = 0; i < playSize; i++)
            {
                for (int j = 0; j < playSize; j++)
                {
                    if ((i == 0 || i == playSize - 1) && (j == 0 || j == playSize - 1)) 
                    { 
                        Amounts[i, j] = 2; 
                    }
                    else if ((i == 0 || i == playSize - 1) || (j == 0 || j == playSize - 1))
                    { 
                        Amounts[i, j] = 3; 
                    }
                    else 
                    { 
                        Amounts[i, j] = 4; 
                    }
                }
            }
        }
        public Form1() //Initializes the game onto the menu screen
        {
            InitializeComponent();
            diffSelect = Image.FromFile(diffs[hard]);
            format.LineAlignment = StringAlignment.Center;
            format.Alignment = StringAlignment.Center;
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
            BackGround.Dock = DockStyle.Fill;
            BackGround.BackColor = Color.Black;
            Menu_Controls(true);
            Controls.Add(BackGround);
            Amount_Fill();
        }
        private int[,] Copier(int[,] toDo) //Copies arrays by value, so no values are accidently changed
        {
            int[,] temp = new int[playSize, playSize];
            for (int i = 0; i < playSize; i++)
            {
                for (int j = 0; j < playSize; j++)
                {
                    temp[i, j] = toDo[i, j];
                }
            }
            return temp;
        }
        private int[] Check_Larger(int altTurn, bool justCheck = false) //Checks the amount of atoms of each color and returns the result
        {
            int player = 0;
            int enemy = 0;
            for (int i = 0; i < playSize; i++)
            {
                for (int j = 0; j < playSize; j++)
                {
                    if (Colors[i, j] == altTurn + 1) { player += DisksPlayed[i, j]; }
                    else if (Colors[i, j] != 0) { enemy += DisksPlayed[i, j]; }
                }
            }
            if (justCheck) //The 'justCheck' flag is used, when the function is called by the counter on the right, to get the total amounts of atoms
            {
                scores = turn % 2 == 0 ? [player, enemy] : [enemy, player];
            }
            return [enemy, player];
        }
        private bool Check_Win(int altTurn) //Checks if the player who's turn it is has won
        {
            for (int i = 0; i < playSize; i++)
            {
                for (int j = 0; j < playSize; j++)
                {
                    if (Colors[i, j] != altTurn + 1 && Colors[i, j] != 0) { return false; }
                }
            }
            return true;
        }
        private int[] Sorter(int altTurn, int level) //Sorts the positions that need to be gone through to optimize the MiniMax algorithm
        {
            int[] sorted = new int[playSize * playSize + 2];
            int[] others = new int[playSize * playSize];
            int indexer1 = 0;
            int indexer2 = 0;
            int indexer3 = 0;
            int skipColumn = (level < maxLevel && level % 2 == 0) ? skipper[level / 2] : 0;
            for (int i = skipColumn; i < playSize; i++)
            {
                for (int j = 0; j < playSize; j++)
                {
                    if (Colors[i, j] == altTurn + 1 && DisksPlayed[i, j] == Amounts[i, j] - 1)
                    {
                        if (i * j == 0 || ((DisksPlayed[i - 1, j] < Amounts[i - 1, j] - 1 || Colors[i - 1, j] != altTurn + 1) 
                            && (DisksPlayed[i, j - 1] < Amounts[i, j - 1] - 1 || Colors[i, j - 1] != altTurn + 1)))
                        {
                            sorted[indexer1++] = i + (j * playSize) + 1;
                        }
                    }
                    else if (Colors[i, j] == altTurn + 1 && DisksPlayed[i, j] > 0)
                    {
                        sorted[playSize * playSize - 1 - indexer2++] = i + (j * playSize) + 1;
                    }
                    else if (Colors[i, j] == 0)
                    { 
                        others[indexer3++] = i + (j * playSize) + 1;
                    }
                }
            }
            random.Shuffle(others, indexer3);
            for (int i = 0; i < indexer2; i++)
            {
                sorted[indexer1 + i] = sorted[playSize * playSize - i - 1];
                sorted[playSize * playSize - i - 1] = 0;
            }
            for (int i = 0; i < indexer3; i++)
            {
                sorted[indexer1 + indexer2 + i] = others[i];
            }
            sorted[sorted.Length - 1] = indexer1;
            sorted[sorted.Length - 2] = indexer2;
            return sorted; //The array returned is sorted so that game squares with 1 missing atom come first, then squares with at least 1 atom, and finally empty squares. All the squares are either in the player's color or still empty
        }
        private int MiniMax(int level) //Calculates the best next move for the AI
        {
            int tempTurn = (turn + level) % playerCount;
            int levelCalc = level % playerCount;
            int[,] currentDisksPlayed = Copier(DisksPlayed);
            int[,] currentColors = Copier(Colors);
            int[] order = Sorter(tempTurn, level);
            int control = levelCalc == 0 ? int.MinValue : int.MaxValue;
            for (int k = 0; k < (level == maxLevel ? order[order.Length - 1] : order.Length - 2); k++)
            {
                if ((control == int.MaxValue - 1 || control == int.MinValue + 1) && level > 0) { return control; }
                if (order[k] == 0) { break; }
                int i = (order[k] - 1) % playSize;
                int j = (order[k] - 1) / playSize;
                if ((Colors[i, j] == 0 || Colors[i, j] == tempTurn + 1) && DisksPlayed[i, j] < Amounts[i, j])
                {
                    DisksPlayed[i, j]++;
                    Colors[i, j] = tempTurn + 1;
                    if (DisksPlayed[i, j] >= Amounts[i, j])
                    {
                        Check_Full(i, j, true, tempTurn);
                    }
                    int returnCheck;
                    if (level == maxLevel)
                    {
                        int[] checks = Check_Larger(tempTurn);
                        returnCheck = levelCalc == 0 ? int.MaxValue - checks[0] - 1 : int.MinValue + checks[0] + 1;
                    }
                    else
                    {
                        if (level % 2 == 0 && level < maxLevel) { skipper[level / 2] = i; } //'skipper' is used so that positions are not explored twice
                        if (Check_Win(tempTurn))
                        {
                            returnCheck = levelCalc == 0 ? int.MaxValue - 1 : int.MinValue + 1;
                        }
                        else
                        {
                            returnCheck = MiniMax(level + 1);
                        }
                        skipper[level / 2] = 0;
                    }
                    if ((levelCalc == 0 && returnCheck > control) || (levelCalc != 0 && returnCheck < control))
                    {
                        if (level == 0) { minimaxExport = [i, j]; }
                        control = returnCheck;
                    }
                    DisksPlayed = Copier(currentDisksPlayed);
                    Colors = Copier(currentColors);
                }
            }
            return control;
        }
        private void End() //Initiates the victory screen
        {
            End_Controls(0);
            finish = true;
            Update_Scores();
            BackGround.Invalidate(gameOver);
            BackGround.Invalidate(pts1);
            BackGround.Invalidate(pts2);
            BackGround.Update();
        }
        private void Check_Full(int x, int y, bool justCheck = false, int altTurn = -1) //Recursively handles all explosions withing the game
        {
            if (count) { counter++; }
            if (counter >= maxBooms) 
            {
                if (finish) { return; }
                End(); 
            }
            int[,] temp = { { 0, 1 }, { 0, -1 }, { 1, 0 }, { -1, 0 } };
            if (Check_Win(altTurn) && justCheck) { return; }
            DisksPlayed[x, y] -= Amounts[x, y];
            if (!justCheck) { Thread.Sleep(boomWait); Update_Region(x, y); }
            for (int i = 0; i < 4; i++)
            {
                try //Here access to all possibe neighbours is attempted when an explosion occurs, if hey don't exist, they are skipped
                {
                    DisksPlayed[x + temp[i, 0], y + temp[i, 1]] += 1;
                    Colors[x + temp[i, 0], y + temp[i, 1]] = Colors[x, y];
                    if (!justCheck) { Thread.Sleep(boomWait); Update_Region(x + temp[i, 0], y + temp[i, 1]); }
                    if (DisksPlayed[x + temp[i, 0], y + temp[i, 1]] >= Amounts[x + temp[i, 0], y + temp[i, 1]])
                    {
                        Check_Full(x + temp[i, 0], y + temp[i, 1], justCheck, altTurn);
                        if (finish) { return; }
                    }
                }
                catch { }
            }
            if (DisksPlayed[x, y] == 0) { Colors[x, y] = 0; }
            if (Check_Win(turn) && !justCheck) { count = true; winturn = turn; }
            if (Check_Win(altTurn) && justCheck) { return; }
        }
        private void Update_Region(int x, int y) //Updates a specified square on the game board
        {
            BackGround.Invalidate(new Rectangle(width / 2 - height / 3 + height * 2 / (3 * playSize) * x, 
                height / 6 + height * 2 / (3 * playSize) * y, height * 2 / (3 * playSize), 
                height * 2 / (3 * playSize)));
            BackGround.Update();
        }
        private void Update_Scores() //Updates the atom counter on the right
        {
            Check_Larger(turn, true);
            BackGround.Invalidate(pts1);
            BackGround.Invalidate(pts2);
            BackGround.Update();
        }
        private void End_Controls(int add) //Called when entering or exiting the victory screen to initiate correct controls
        {
            if (add == 0)
            {
                BackGround.Paint += new PaintEventHandler(this.End_Paint);
                BackGround.Click += new EventHandler(this.End_Click);
                BackGround.MouseMove += new MouseEventHandler(this.End_Hover);
                BackGround.Click -= new EventHandler(this.Form1_Click_1);
                BackGround.MouseMove -= new MouseEventHandler(this.Back_Hover);
            }
            else if (add == 1)
            {
                BackGround.Paint -= new PaintEventHandler(this.End_Paint);
                BackGround.Click -= new EventHandler(this.End_Click);
                if (simplified) { BackGround.Paint -= new PaintEventHandler(this.Form1_Paint_Simple); }
                else { BackGround.Paint -= new PaintEventHandler(this.Form1_Paint); }
                BackGround.MouseMove -= new MouseEventHandler(this.End_Hover);
                BackGround.MouseMove -= new MouseEventHandler(this.Back_Hover);
                BackGround.Click -= new EventHandler(this.Form1_Click_1);
            }
            else
            {
                BackGround.Paint -= new PaintEventHandler(this.End_Paint);
                BackGround.Click -= new EventHandler(this.End_Click);
                BackGround.MouseMove -= new MouseEventHandler(this.End_Hover);
                Game_Controls(false);
                Menu_Controls(true);
            }
        }
        private void Menu_Controls(bool add) //Called when entering or exiting the menu screen to initiate correct controls
        {
            if (add)
            {
                BackGround.Paint += new PaintEventHandler(this.Menu_Paint);
                BackGround.Click += new EventHandler(this.Menu_Click);
                BackGround.MouseMove += new MouseEventHandler(this.Menu_Hover);
            }
            else
            {
                BackGround.Paint -= new PaintEventHandler(this.Menu_Paint);
                BackGround.Click -= new EventHandler(this.Menu_Click);
                BackGround.MouseMove -= new MouseEventHandler(this.Menu_Hover);
            }
        }
        private void Options_Controls(bool add) //Called when entering or exiting the options screen to initiate correct controls
        {
            if (add)
            {
                BackGround.Paint += new PaintEventHandler(this.Options_Paint);
                BackGround.Click += new EventHandler(this.Options_Click);
                BackGround.MouseMove += new MouseEventHandler(this.Back_Hover);
            }
            else
            {
                BackGround.Paint -= new PaintEventHandler(this.Options_Paint);
                BackGround.Click -= new EventHandler(this.Options_Click);
                BackGround.MouseMove -= new MouseEventHandler(this.Back_Hover);
            }
        }
        private void Game_Controls(bool add) //Called when entering or exiting the game screen to initiate correct controls
        {
            if (add)
            {
                if (simplified)
                {
                    BackGround.Paint += new PaintEventHandler(this.Form1_Paint_Simple);
                }
                else
                {
                    BackGround.Paint += new PaintEventHandler(this.Form1_Paint);
                }
                BackGround.Click += new EventHandler(this.Form1_Click_1);
                BackGround.MouseMove += new MouseEventHandler(this.Back_Hover);
                DisksPlayed = Copier(resetField);
                Colors = Copier(resetField);
                Create_Background();
                turn = 0;
                count = false;
                counter = 0;
                finish = false;
                Update_Scores();
                BackGround.Image = bitmap;
            }
            else
            {
                if (simplified) 
                {
                    BackGround.Paint -= new PaintEventHandler(this.Form1_Paint_Simple);
                }
                else
                {
                    BackGround.Paint -= new PaintEventHandler(this.Form1_Paint);
                }
                BackGround.Click -= new EventHandler(this.Form1_Click_1);
                BackGround.MouseMove -= new MouseEventHandler(this.Back_Hover);
                BackGround.Image = null;
            }
        }
        private void AI_turn(int[,] tempDisksPlayed, int[,] tempColors) //Handles the turn of the AI
        {
            stopWatch.Start();
            if (random.Next(7) > hard * 2)
            {
                int[] list = Sorter(turn, 0);
                int top = list[list.Length - 1] + list[list.Length - 2];
                int choose = random.Next(0, top);
                int generated = list[choose];
                minimaxExport = [(generated - 1) % playSize, (generated - 1) / playSize];
            }
            else
            {
                int toCheck = MiniMax(0);
                DisksPlayed = tempDisksPlayed;
                Colors = tempColors;
            }
            stopWatch.Stop();
            if (stopWatch.Elapsed.TotalMilliseconds < minTurnTime)
            {
                Thread.Sleep(minTurnTime - Convert.ToInt16(stopWatch.Elapsed.TotalMilliseconds));
            }
            DisksPlayed[minimaxExport[0], minimaxExport[1]]++;
            Colors[minimaxExport[0], minimaxExport[1]] = turn + 1;
            Update_Region(minimaxExport[0], minimaxExport[1]);
            if (DisksPlayed[minimaxExport[0], minimaxExport[1]] >= Amounts[minimaxExport[0], minimaxExport[1]])
            {
                Check_Full(minimaxExport[0], minimaxExport[1]);
                if (finish) { return; }
            }
            Update_Scores();
            if (count) { End(); }
            turn = (turn + 1) % playerCount;
            BackGround.Click += new EventHandler(this.Form1_Click_1);
        }
        private void Form1_Click_1(object sender, EventArgs e) //Handes click events for the game screen and hands control to the AI when needed
        {
            int checkX = MousePosition.X - width / 2 + height / 3;
            int checkY = MousePosition.Y - height / 6;
            int a = checkX / (height * 2 / (3 * playSize));
            int b = checkY / (height * 2 / (3 * playSize));
            if (checkX >= 0 && a < playSize && checkY >= 0 && b < playSize)
            {
                if (Colors[a, b] == turn + 1 || Colors[a, b] == 0)
                {
                    Colors[a, b] = turn + 1;
                    DisksPlayed[a, b]++;
                    Update_Region(a, b);
                    if (DisksPlayed[a, b] >= Amounts[a, b])
                    {
                        Check_Full(a, b);
                    }
                    Update_Scores();
                    if (count) { End(); }
                    int[,] tempDisksPlayed = Copier(DisksPlayed);
                    int[,] tempColors = Copier(Colors);
                    count = false;
                    counter = 0;
                    turn = (turn + 1) % playerCount;
                    if (turn == AIturn && opponent && !finish)
                    {
                        AI_turn(tempDisksPlayed, tempColors);
                        BackGround.Click -= new EventHandler(this.Form1_Click_1);
                    }
                }
            }
            else if (MousePosition.X > width - height / 20 && MousePosition.Y < height / 40)
            {
                Menu_Controls(true);
                Game_Controls(false);
                BackGround.Refresh();
            }
        }
        private void Menu_Click(object sender, EventArgs e) //Handles click events for the menu screen
        {
            if (play.Contains(MousePosition))
            {
                Menu_Controls(false);
                Game_Controls(true);
                BackGround.Refresh();
            }
            else if (options.Contains(MousePosition))
            {
                Menu_Controls(false);
                Options_Controls(true);
                BackGround.Refresh();
            }
            else if (exit.Contains(MousePosition))
            {
                Application.Exit(); Close();
            }
        }
        private void Options_Click(object sender, EventArgs e) //Handles click events for the options screen
        {
            if (box.Contains(MousePosition))
            {
                simplified = !simplified;
                tickBox = Image.FromFile(simplified ? @"../../../Resources/ticked.jpg" : @"../../../Resources/unticked.jpg");
            }
            if (option2.Contains(MousePosition))
            {
                opponent = !opponent;
                enemy = Image.FromFile(opponent ? @"../../../Resources/enemyAI.jpg" : @"../../../Resources/enemyPlayer.jpg");
                diffSelect = Image.FromFile(opponent ? diffs[hard] : diffs[4]);
                diff = Image.FromFile(opponent ? @"../../../Resources/difficulty.jpg" : @"../../../Resources/difficultyOff.jpg");
            }
            if (difficulty.Contains(MousePosition) && opponent)
            {
                hard = (hard + 1) % difficultyCount;
                diffSelect = Image.FromFile(diffs[hard]);
            }
            if (minus.Contains(MousePosition))
            {
                playSize = playSize == minPlaySize ? minPlaySize : playSize - 1;
                Amount_Fill();
            }
            else if (plus.Contains(MousePosition))
            {
                playSize = playSize == maxPlaySize ? maxPlaySize : playSize + 1;
                Amount_Fill();
            }
            if (back.Contains(MousePosition)) 
            {
                Menu_Controls(true);
                Options_Controls(false);
                backButton = Image.FromFile(@"../../../Resources/back.jpg");
            }
            BackGround.Refresh();
        }
        private void End_Click(object sender, EventArgs e) //Handles click events for the victory screen
        {
            if (retry.Contains(MousePosition))
            {
                End_Controls(1);
                Game_Controls(true);
                BackGround.Refresh();
            }
            else if (leave.Contains(MousePosition))
            {
                End_Controls(2);
                turn = 0;
                BackGround.Refresh();
                finish = false;
            }
        }
        private void End_Hover(object sender, MouseEventArgs e) //Handles hover events for the victory screen
        {
            backButton2 = Image.FromFile(leave.Contains(e.Location) ? @"../../../Resources/backHover.jpg" : @"../../../Resources/back.jpg");
            replay = Image.FromFile(retry.Contains(e.Location) ? @"../../../Resources/replayHover.jpg" : @"../../../Resources/replay.jpg");
            BackGround.Invalidate(gameOver);
            BackGround.Update();
        }
        private void Back_Hover(object sender, MouseEventArgs e) //Handles hover events for the 'BACK' button in the top right corner of the game and options screens
        {
            backButton = Image.FromFile(back.Contains(e.Location) ? @"../../../Resources/backHover.jpg" : @"../../../Resources/back.jpg");
            BackGround.Invalidate(back);
            BackGround.Update();
        }
            private void Menu_Hover(object sender, MouseEventArgs e) //Handles hover events for the menu screen
        {
            if (play.Contains(e.Location))
            {
                playButton = Image.FromFile(@"../../../Resources/playHover.jpg");
            }
            else 
            { 
                playButton = Image.FromFile(@"../../../Resources/play.jpg"); 
            }
            BackGround.Invalidate(play);
            if (options.Contains(e.Location))
            {
                optionsButton = Image.FromFile(@"../../../Resources/optionsHover.jpg");
            }
            else
            {
                optionsButton = Image.FromFile(@"../../../Resources/options.jpg");
            }
            BackGround.Invalidate(options);
            if (exit.Contains(e.Location))
            {
                exitButton = Image.FromFile(@"../../../Resources/exitHover.jpg");
            }
            else
            {
                exitButton = Image.FromFile(@"../../../Resources/exit.jpg");
            }
            BackGround.Invalidate(exit);
            BackGround.Update();
        }
    }
}
