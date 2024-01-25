using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace FlappyBird
{
    public partial class MainWindow : Window
    {
        DispatcherTimer gameTimer = new DispatcherTimer();
        double score;
        int gravity = 8;
        bool gameOver;
        Rect flappyBirdHitBox;

        public MainWindow()
        {
            InitializeComponent();
            InitializeGame();
        }

        private void InitializeGame()
        {
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Interval = TimeSpan.FromMilliseconds(20);
            StartGame();
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            UpdateScore();
            UpdateFlappyBirdPosition();
            CheckCollisionWithBoundaries();

            foreach (var obstacle in MyCanvas.Children.OfType<Image>())
            {
                if (IsObstacle(obstacle))
                {
                    UpdateObstaclePosition(obstacle);
                    CheckCollisionWithObstacle(obstacle);
                }

                if (IsCloud(obstacle))
                {
                    UpdateCloudPosition(obstacle);
                }
            }
        }

        private void UpdateScore()
        {
            txtScore.Content = "Score: " + score;
        }

        private void UpdateFlappyBirdPosition()
        {
            flappyBirdHitBox = new Rect(Canvas.GetLeft(flappyBird), Canvas.GetTop(flappyBird), flappyBird.Width - 12, flappyBird.Height);
            Canvas.SetTop(flappyBird, Canvas.GetTop(flappyBird) + gravity);
        }

        private void CheckCollisionWithBoundaries()
        {
            if (Canvas.GetTop(flappyBird) < -30 || Canvas.GetTop(flappyBird) + flappyBird.Height > 460)
            {
                EndGame();
            }
        }

        private bool IsObstacle(Image obstacle)
        {
            return ((string)obstacle.Tag == "obs1" || (string)obstacle.Tag == "obs2" || (string)obstacle.Tag == "obs3");
        }

        private void UpdateObstaclePosition(Image obstacle)
        {
            Canvas.SetLeft(obstacle, Canvas.GetLeft(obstacle) - 5);

            if (Canvas.GetLeft(obstacle) < -100)
            {
                Canvas.SetLeft(obstacle, 800);
                score += 0.5;
            }
        }

        private void CheckCollisionWithObstacle(Image obstacle)
        {
            Rect obstacleHitBox = new Rect(Canvas.GetLeft(obstacle), Canvas.GetTop(obstacle), obstacle.Width, obstacle.Height);

            if (flappyBirdHitBox.IntersectsWith(obstacleHitBox))
            {
                EndGame();
            }
        }

        private bool IsCloud(Image cloud)
        {
            return ((string)cloud.Tag == "clouds");
        }

        private void UpdateCloudPosition(Image cloud)
        {
            Canvas.SetLeft(cloud, Canvas.GetLeft(cloud) - 1);

            if (Canvas.GetLeft(cloud) < -250)
            {
                Canvas.SetLeft(cloud, 550);
                score += 0.5;
            }
        }

        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                AdjustFlappyBirdRotation(-20);
                gravity = -8;
            }

            if (e.Key == Key.R && gameOver)
            {
                StartGame();
            }
        }

        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            AdjustFlappyBirdRotation(5);
            gravity = 8;
        }

        private void AdjustFlappyBirdRotation(double angle)
        {
            flappyBird.RenderTransform = new RotateTransform(angle, flappyBird.Width / 2, flappyBird.Height / 2);
        }

        private void StartGame()
        {
            MyCanvas.Focus();
            InitializeGameObjects();
            gameTimer.Start();
            
            // create a new highscore file if it doesn't exist
            if (!System.IO.File.Exists("highscore.txt"))
            {
                System.IO.File.Create("highscore.txt");
            }
        }

        private void InitializeGameObjects()
        {
            int temp = 300;
            score = 0;
            gameOver = false;
            Canvas.SetTop(flappyBird, 190);

            foreach (var gameObject in MyCanvas.Children.OfType<Image>())
            {
                if (IsObstacle(gameObject))
                {
                    InitializeObstaclePosition(gameObject);
                }

                if (IsCloud(gameObject))
                {
                    InitializeCloudPosition(gameObject, ref temp);
                }
            }
        }
        
        private void InitializeObstaclePosition(Image obstacle)
        {
            switch ((string)obstacle.Tag)
            {
                case "obs1":
                    Canvas.SetLeft(obstacle, 500);
                    break;
                case "obs2":
                    Canvas.SetLeft(obstacle, 800);
                    break;
                case "obs3":
                    Canvas.SetLeft(obstacle, 1100);
                    break;
            }
        }

        private void InitializeCloudPosition(Image cloud, ref int temp)
        {
            Canvas.SetLeft(cloud, 300 + temp);
            temp = 800;
        }
        private double GetHighScore()
        {
                 double highScore = 0;
                string highScoreString = System.IO.File.ReadAllText("highscore.txt");
    
                if (highScoreString != "")
                {
                    highScore = Convert.ToDouble(highScoreString);
                }
    
                return highScore;
        }

        private void EndGame()
        {
            gameTimer.Stop();
            gameOver = true;

            PopupScore.Text = $"Score: {score}\nHigh Score: {GetHighScore()}";
            
            if (score > GetHighScore())
            {
                System.IO.File.WriteAllText("highscore.txt", score.ToString());
            }
            GameOverGrid.Visibility = Visibility.Visible;
        }

        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            GameOverGrid.Visibility = Visibility.Collapsed;
            StartGame();
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
