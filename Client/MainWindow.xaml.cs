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
using Microsoft.AspNetCore.SignalR.Client;

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
        }

        private void InitializeUi()
        {
            SeatCombobox.Items.Add("Seat 1");
            SeatCombobox.Items.Add("Seat 2");
            SeatCombobox.SelectedIndex = 0;

            JoinButton.Click += HandleJoin;

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
        private void HandleJoin(object sender, RoutedEventArgs e)
        {
            NicknameTextbox.IsEnabled = false;
            SeatCombobox.IsEnabled = false;
            JoinButton.IsEnabled = false;
            MessageTextbox.IsEnabled = true;

            GameSession = new Game(@"http://localhost:4000/hubs/battleship", SeatCombobox.SelectedIndex, NicknameTextbox.Text);
            GameSession.OnChatMessage += OnChatMessage;
        }

        /*
         * UI event handlers
         */
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

        /*
         * Game event handlers
         */
        private void SendChatMessage(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;

            GameSession.SendChatMessage(MessageTextbox.Text);
        }

        private void OnChatMessage(string nickname, string message)
        {
            MessagesListbox.Items.Add(nickname + ": " + message);
        }
    }
}
