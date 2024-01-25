using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace scool_FlappyBird
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        double score;
        bool gameOver;
        public MainWindow()
        {
            InitializeComponent();
        }
        
        private void InitializeGame()
        {
            StartGame();
        }
        
        private void StartGame()
        {
            MyCanvas.Focus();
            InitializeGameObjects();
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
        
        private static bool IsObstacle(Image obstacle)
        {
            return (string)obstacle.Tag == "obs1" || (string)obstacle.Tag == "obs2" || (string)obstacle.Tag == "obs3";
        }
        
        private static bool IsCloud(Image cloud)
        {
            return (string)cloud.Tag == "clouds";
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

        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}