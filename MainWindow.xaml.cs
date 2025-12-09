using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Zuzda
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer timer = new DispatcherTimer();
        private Rectangle player;
        private Random random = new Random();
        private int score = 0;
        private double playerSpeed = 0;
        private double obstacleSpeed = 5;   //Itt kisebb bugok voltak az időzítővel, ezekkel kapcsolatban kértem ki az ai véleményét 

        public MainWindow()
        {
            InitializeComponent();

            BorderRect.Width = GameCanvas.ActualWidth;
            BorderRect.Height = GameCanvas.ActualHeight;

            player = new Rectangle
            {
                Width = 30,
                Height = 30,
                Fill = Brushes.Blue
            };
            Canvas.SetLeft(player, 50);
            Canvas.SetTop(player, 150);
            GameCanvas.Children.Add(player);

            timer.Interval = TimeSpan.FromMilliseconds(20);
            timer.Tick += GameLoop;
            timer.Start();
        }

        private void GameLoop(object sender, EventArgs e)
        {
            Canvas.SetTop(player, Canvas.GetTop(player) + playerSpeed);

            if (Canvas.GetTop(player) < 0)
                Canvas.SetTop(player, 0);
            if (Canvas.GetTop(player) + player.Height > GameCanvas.ActualHeight)
                Canvas.SetTop(player, GameCanvas.ActualHeight - player.Height);

            if (random.Next(0, 100) < 2)
            {
                Rectangle obstacle = new Rectangle
                {
                    Width = 20,
                    Height = random.Next(30, 100),
                    Fill = Brushes.Red
                };
                Canvas.SetLeft(obstacle, GameCanvas.ActualWidth);
                Canvas.SetTop(obstacle, random.Next(0, (int)(GameCanvas.ActualHeight - obstacle.Height)));  
                GameCanvas.Children.Add(obstacle);
            }

            for (int i = GameCanvas.Children.Count - 1; i >= 0; i--)
            {
                if (GameCanvas.Children[i] is Rectangle rect && rect != player && rect != BorderRect)
                {
                    Canvas.SetLeft(rect, Canvas.GetLeft(rect) - obstacleSpeed);

                    if (Canvas.GetLeft(rect) + rect.Width < 0)
                    {
                        GameCanvas.Children.Remove(rect);
                        score++;

                        if (score % 10 == 0)
                            obstacleSpeed += 0.5;
                    }

                    Rect playerRect = new Rect(Canvas.GetLeft(player), Canvas.GetTop(player), player.Width, player.Height);
                    Rect rectRect = new Rect(Canvas.GetLeft(rect), Canvas.GetTop(rect), rect.Width, rect.Height);  //Ehhez használtam online és ai segítséget is (az ütközés mechanikához)
                    if (playerRect.IntersectsWith(rectRect))
                        GameOver();
                }
            }

            ScoreText.Text = "Pont: " + score;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up) playerSpeed = -5;
            if (e.Key == Key.Down) playerSpeed = 5;
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up || e.Key == Key.Down) playerSpeed = 0;
        }

        private void GameOver()
        {
            timer.Stop();
            MessageBox.Show("Játék vége! Pontszám: " + score);
            Application.Current.Shutdown();
        }
    }
}
