using Microsoft.AspNetCore.SignalR.Client;
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

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Game GameSession;

        private Button[,] MyButtons = new Button[10, 10];
        private Button[,] EnemyButtons = new Button[10, 10];

        public MainWindow()
        {
            InitializeComponent();
            InitializeUi();

            GameSession = new Game(@"http://localhost:8000/hubs/battleship");
        }

        private void InitializeUi()
        {
            for (int y = 0; y < 10; y++)
            {
                for (int x = 0; x < 10; x++)
                {
                    var myButton = CreateButton(true, x, y);
                    var enemyButton = CreateButton(false, x, y);

                    MyButtons[y, x] = myButton;
                    EnemyButtons[y, x] = enemyButton;

                    Grid.SetRow(myButton, y + 1);
                    Grid.SetColumn(myButton, x + 1);
                    MyBoard.Children.Add(myButton);

                    Grid.SetRow(enemyButton, y + 1);
                    Grid.SetColumn(enemyButton, x + 1);
                    EnemyBoard.Children.Add(enemyButton);
                }
            }
        }

        private void ClearBoard()
        {
            for (int y = 0; y < 10; y++)
            {
                for (int x = 0; x < 10; x++)
                {
                    MyButtons[y, x].Content = "";
                    MyButtons[y, x].IsEnabled = false;

                    EnemyButtons[y, x].Content = "";
                    MyButtons[y, x].IsEnabled = false;
                }
            }
        }

        private Button CreateButton(bool myBoard, int x, int y)
        {
            Button b = new Button();
            b.Name = (myBoard ? "M_" : "E_") + y + "_" + x;
            b.Click += myBoard ? HandleShipArrangement : HandleShot;
            b.IsEnabled = false;

            return b;
        }


        private void HandleShipArrangement(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;

        }

        private void HandleShot(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;

        }

        private void SurrenderButton_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
