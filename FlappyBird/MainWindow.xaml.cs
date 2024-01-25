using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace FlappyBird
{
    public partial class MainWindow : Window
    {
        // Game-related variables
        private DispatcherTimer _gameTimer = new DispatcherTimer();
        private double _score;
        private int _gravity = 8;
        private bool _gameOver;
        private Rect _flappyBirdHitBox;

        private MediaPlayer _backgroundMusicPlayer = new MediaPlayer();
        private MediaPlayer _gameoverSoundPlayer = new MediaPlayer();
        private MediaPlayer _scoreSoundPlayer = new MediaPlayer();

        // Constructor
        public MainWindow()
        {
            InitializeComponent();
            InitializeGame();
            LoadAudioFiles();
        }

        private void LoadAudioFiles()
        {
            LoadAudioFile(_backgroundMusicPlayer, "FlappyBird.assets.sounds.background.mp3", 0.2, true);
            LoadAudioFile(_gameoverSoundPlayer, "FlappyBird.assets.sounds.gameover.mp3", 0.4);
        }

        private void LoadAudioFile(MediaPlayer player, string resourcePath, double volume, bool play = false)
        {
            var resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourcePath);
            var fileStream = new FileStream(resourcePath, FileMode.Create, FileAccess.Write);
            resourceStream.CopyTo(fileStream);
            player.Open(new Uri(fileStream.Name, UriKind.Relative));
            player.Volume = volume;
            if (play) player.Play();
        }

        // Initialize game components and start the game timer
        private void InitializeGame()
        {
            _gameTimer.Tick += GameTimer_Tick;
            _gameTimer.Interval = TimeSpan.FromMilliseconds(20);
            StartGame();
        }


        // Game timer tick event handling
        private void GameTimer_Tick(object sender, EventArgs e)
        {
            txtScore.Content = "Score: " + _score;
            UpdateFlappyBirdPosition();
            CheckCollisionWithBoundaries();

            foreach (var obstacle in MyCanvas.Children.OfType<Image>())
            {
                if (IsObstacle(obstacle))
                {
                    UpdateObstaclePosition(obstacle);
                    CheckCollisionWithObstacle(obstacle);
                }

                if (IsCloud(obstacle)) UpdateCloudPosition(obstacle);
            }
        }

        // Update the position of the flappy bird
        private void UpdateFlappyBirdPosition()
        {
            _flappyBirdHitBox = new Rect(Canvas.GetLeft(flappyBird), Canvas.GetTop(flappyBird), flappyBird.Width - 12,
                flappyBird.Height);
            Canvas.SetTop(flappyBird, Canvas.GetTop(flappyBird) + _gravity);
        }

        // Check for collisions with the boundaries of the game
        private void CheckCollisionWithBoundaries()
        {
            if (Canvas.GetTop(flappyBird) < -30 || Canvas.GetTop(flappyBird) + flappyBird.Height > 460) EndGame();
        }

        // Check if the given image is an obstacle
        private bool IsObstacle(Image obstacle)
        {
            return (string)obstacle.Tag == "obs1" || (string)obstacle.Tag == "obs2" || (string)obstacle.Tag == "obs3";
        }

        // Update the position of the obstacle and handle scoring
        private void UpdateObstaclePosition(Image obstacle)
        {
            Canvas.SetLeft(obstacle, Canvas.GetLeft(obstacle) - 5);

            if (Canvas.GetLeft(obstacle) < -100)
            {
                Canvas.SetLeft(obstacle, 800);
                _score += 0.5;
                _scoreSoundPlayer.Play();
            }
        }

        // Check for collisions with obstacles
        private void CheckCollisionWithObstacle(Image obstacle)
        {
            var obstacleHitBox = new Rect(Canvas.GetLeft(obstacle), Canvas.GetTop(obstacle), obstacle.Width,
                obstacle.Height);

            if (_flappyBirdHitBox.IntersectsWith(obstacleHitBox)) EndGame();
        }

        // Check if the given image is a cloud
        private bool IsCloud(Image cloud)
        {
            return (string)cloud.Tag == "clouds";
        }

        // Update the position of the cloud and handle scoring
        private void UpdateCloudPosition(Image cloud)
        {
            Canvas.SetLeft(cloud, Canvas.GetLeft(cloud) - 1);

            if (Canvas.GetLeft(cloud) < -250)
            {
                Canvas.SetLeft(cloud, 550);
                _score += 0.5;
            }
        }

        // Handle key press events for controlling the flappy bird
        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                AdjustFlappyBirdRotation(-20);
                _gravity = -8;
            }

            if (e.Key == Key.R && _gameOver) StartGame();
        }

        // Handle key release events for controlling the flappy bird
        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            AdjustFlappyBirdRotation(5);
            _gravity = 8;
        }

        // Adjust the rotation of the flappy bird
        private void AdjustFlappyBirdRotation(double angle)
        {
            flappyBird.RenderTransform = new RotateTransform(angle, flappyBird.Width / 2, flappyBird.Height / 2);
        }

        // Start the game
        private void StartGame()
        {
            MyCanvas.Focus();
            InitializeGameObjects();
            _gameTimer.Start();
            _gameoverSoundPlayer.Stop();
            _backgroundMusicPlayer.Play();

            // create a new highscore file if it doesn't exist
            if (!File.Exists("highscore.txt")) File.Create("highscore.txt");
        }

        // Initialize positions of game objects at the beginning of the game
        private void InitializeGameObjects()
        {
            var temp = 300;
            _score = 0;
            _gameOver = false;
            Canvas.SetTop(flappyBird, 190);

            foreach (var gameObject in MyCanvas.Children.OfType<Image>())
            {
                if (IsObstacle(gameObject)) InitializeObstaclePosition(gameObject);

                if (IsCloud(gameObject)) InitializeCloudPosition(gameObject, ref temp);
            }
        }

        // Initialize the position of an obstacle based on its tag
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

        // Initialize the position of a cloud and update the temporary variable
        private void InitializeCloudPosition(Image cloud, ref int temp)
        {
            Canvas.SetLeft(cloud, 300 + temp);
            temp = 800;
        }

        // Retrieve the high score from the highscore.txt file
        private double GetHighScore()
        {
            double highScore = 0;
            var highScoreString = File.ReadAllText("highscore.txt");

            if (highScoreString != "") highScore = Convert.ToDouble(highScoreString);

            return highScore;
        }

        // End the game and display the game over screen
        private void EndGame()
        {
            _gameoverSoundPlayer.Play();
            _backgroundMusicPlayer.Stop();
            _gameTimer.Stop();
            _gameOver = true;

            PopupScore.Text = $"Score: {_score}\nHigh Score: {GetHighScore()}";

            if (_score > GetHighScore()) File.WriteAllText("highscore.txt", _score.ToString());
            GameOverGrid.Visibility = Visibility.Visible;
        }

        // Handle the restart button click event
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