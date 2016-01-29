using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Snake
{
    /// <summary>
    /// Snake by Anthony Senior (Seniority Labs™).
    /// 1/12/2016
    /// </summary>    
    public partial class Form1 : Form
    {
        System.Media.SoundPlayer musicPlayer = new System.Media.SoundPlayer();

        private List<Circle> Snake = new List<Circle>();
        private Circle food = new Circle();

        public Form1()
        {
            InitializeComponent();

            musicPlayer.SoundLocation = "desertThemeMusic.wav";
            musicPlayer.PlayLooping();

            //Set settings to default
            new Settings();

            //Set game speed and start timer
            gameTimer.Interval = 1000 / Settings.Speed;
            gameTimer.Tick += UpdateScreen;
            gameTimer.Start();

            StartGame();
        }

        private void StartGame()
        {
            gameoverLabel.Visible = false;

            //Set settings to default
            new Settings();
            Snake.Clear();

            //Create new player object
            Circle head = new Circle();
            head.X = 25;
            head.Y = 20;
            Snake.Add(head);

            scoreLabel.Text = Settings.Score.ToString();
            GenerateFood();
        }

        //Place food randomly on canvas
        private void GenerateFood()
        {
            int maxXPos = canvas.Size.Width / Settings.Width;
            int maxYPos = canvas.Size.Height / Settings.Height;

            Random random = new Random();
            food = new Circle();
            food.X = random.Next(0, maxXPos);
            food.Y = random.Next(0, maxYPos);
        }

        private void UpdateScreen(object sender, EventArgs e)
        {
            //Check to see if player has lost
            if (Settings.GameOver == true)
            {
                if (Input.KeyPressed(Keys.Enter))
                {
                    StartGame();
                }
            }
            else
            {
                if (Input.KeyPressed(Keys.Right) && (Settings.direction != Direction.Left))
                {
                    Settings.direction = Direction.Right;
                }
                else if (Input.KeyPressed(Keys.Left) && (Settings.direction != Direction.Right))
                {
                    Settings.direction = Direction.Left;
                }
                else if (Input.KeyPressed(Keys.Up) && (Settings.direction != Direction.Down))
                {
                    Settings.direction = Direction.Up;
                }
                else if (Input.KeyPressed(Keys.Down) && (Settings.direction != Direction.Up))
                {
                    Settings.direction = Direction.Down;
                }

                MovePlayer();
            }

            canvas.Invalidate();
        }

        private void canvas_Paint(object sender, PaintEventArgs e)
        {
            Graphics canvas = e.Graphics;

            if (Settings.GameOver != true)
            {
                //Color of snake
                Brush snakeColor;

                //Draw snake
                for (int i = 0; i < Snake.Count; i++)
                {                    
                    if (i == 0)
                    {
                        snakeColor = Brushes.Black; //head
                    }
                    else
                    {
                        snakeColor = Brushes.SaddleBrown; //body
                    }

                    //Snake design
                    canvas.FillEllipse(snakeColor,
                            new Rectangle(Snake[i].X * Settings.Width, Snake[i].Y * Settings.Height, Settings.Width, Settings.Height));
                    //Food design
                    canvas.FillEllipse(Brushes.Red,
                        new Rectangle(food.X * Settings.Width, food.Y * Settings.Height, Settings.Width, Settings.Height));
                }
            }
            else
            {
                string gameOver = "Game over!\n";
                gameOver += "Your final score is: " + Settings.Score + "\n";
                gameOver += "Press ENTER to try again";
                gameoverLabel.Text = gameOver;
                gameoverLabel.Visible = true;
            }
        }

        private void MovePlayer()
        {
            for (int i = Snake.Count -1; i >= 0; i--)
            {
                if (i == 0)
                {
                    switch (Settings.direction)
                    {
                        case Direction.Right:
                            Snake[i].X++;
                            break;
                        case Direction.Left:
                            Snake[i].X--;
                            break;
                        case Direction.Up:
                            Snake[i].Y--;
                            break;
                        case Direction.Down:
                            Snake[i].Y++;
                            break;
                    }

                    //Get max X and Y position
                    int maxXPos = canvas.Size.Width / Settings.Width;
                    int maxYPos = canvas.Size.Height / Settings.Height;

                    //Detect collision with game "wall"
                    if ((Snake[i].X < 0) || (Snake[i].Y < 0) || (Snake[i].X > maxXPos) || (Snake[i].Y > maxYPos))
                    {
                        Die();
                    }

                    //Detect collision with body
                    for (int j = 1; j < Snake.Count; j++)
                    {
                        if ((Snake[i].X == Snake[j].X) && (Snake[i].Y == Snake[j].Y))
                        {
                            Die();
                        }
                    }

                    //Detect collision with food
                    if ((Snake[0].X == food.X) && (Snake[0].Y == food.Y))
                    {
                        Eat();
                    }
                }
                else
                {
                    Snake[i].X = Snake[i - 1].X;
                    Snake[i].Y = Snake[i - 1].Y;
                }
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            Input.ChangeState(e.KeyCode, true);
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            Input.ChangeState(e.KeyCode, false);
        }

        private void Die()
        {
            Settings.GameOver = true;
        }

        private void Eat()
        {
            Circle food = new Circle();
            food.X = Snake[Snake.Count - 1].X;
            food.Y = Snake[Snake.Count - 1].Y;

            Snake.Add(food);

            //Update score
            Settings.Score += Settings.Points;
            scoreLabel.Text = Settings.Score.ToString();

            GenerateFood();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string about_message = "";
            about_message += "Welcome to the classic game of Snake by Seniority Labs™\n\n";
            about_message += "In Snake, the your objective is to feed the snake by eating\n";
            about_message += "food, which is represented by the stationary dot. In the beginning\n";
            about_message += "of the game, your snake will be small, but as you eat, your\n";
            about_message += "snake will become longer. Be careful to not hit the boundary\n";
            about_message += "walls or you will die and the game will end. The game will also\n";
            about_message += "end if your snake's head collides with it's own body. Good luck\n";
            about_message += "and have fun!";

            MessageBox.Show(about_message, "Snake", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }


    }//end class
}//end namespace
