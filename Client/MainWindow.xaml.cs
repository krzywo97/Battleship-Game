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
        private Button[,] MyButtons = new Button[10, 10];
        private Button[,] EnemyButtons = new Button[10, 10];

        private HubConnection Connection;
        private int Seat, Turn;

        public MainWindow()
        {
            InitializeComponent();
            InitializeUi();
        }

        private void InitializeUi()
        {
            SeatCombobox.Items.Add("Miejsce 1");
            SeatCombobox.Items.Add("Miejsce 2");
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
            b.Tag = new int[2] { x, y };
            b.Click += myBoard ? HandleShipArrangement : HandleShot;
            b.IsEnabled = false;

            return b;
        }

        /*
         * UI event handlers
         */
        private void HandleJoin(object sender, RoutedEventArgs e)
        {
            MessagesListbox.Items.Add("Łączenie z serwerem...");

            NicknameTextbox.IsEnabled = false;
            SeatCombobox.IsEnabled = false;
            JoinButton.IsEnabled = false;
            MessageTextbox.IsEnabled = true;
            Seat = SeatCombobox.SelectedIndex;

            Connection = new HubConnectionBuilder()
                .WithUrl(@"http://localhost:4000/hubs/battleship")
                .Build();

            Connection.On<bool>("JoinResult", OnJoinResult);
            Connection.On<string, string>("ChatMessage", OnChatMessage);
            Connection.On<string>("GameState", OnGameStateChanged);

            Connection.StartAsync();
            Connection.SendAsync("Join", NicknameTextbox.Text, Seat);
        }

        private void HandleShipArrangement(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            int[] tag = (int[])button.Tag;
            int x = tag[0];
            int y = tag[1];

        }

        private void HandleShot(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            int[] tag = (int[])button.Tag;
            int x = tag[0];
            int y = tag[1];

        }

        private void HandleAction(object sender, RoutedEventArgs e)
        {

        }

        private void SendChatMessage(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            if (MessageTextbox.Text.Length == 0) return;

            Connection.SendAsync("ChatMessage", Seat, MessageTextbox.Text);
            MessageTextbox.Text = "";
        }

        /*
         * Game event handlers
         */

        private void OnJoinResult(bool result)
        {
            if (result)
            {
                MessagesListbox.Items.Add("Zajęto miejsce: " + (Seat + 1));
            }
            else
            {
                NicknameTextbox.IsEnabled = true;
                SeatCombobox.IsEnabled = true;
                JoinButton.IsEnabled = true;
                MessageTextbox.IsEnabled = false;
                MessagesListbox.Items.Add("Nie udało się zająć miejsca");
                Connection.DisposeAsync();
            }
        }

        private void OnGameStateChanged(string newState)
        {
            if (newState == "NotStarted")
            {
                // Disable the UI completely except chat
            }
            else if (newState == "ArrangingShips")
            {
                ActionButton.Content = "Gotowy";

                foreach (Button b in MyButtons)
                {
                    b.IsEnabled = true;
                }
                // Allow arranging ships
            }
            else if (newState == "Started")
            {
                Turn = 0;

                foreach (Button b in MyButtons)
                {
                    b.IsEnabled = false;
                }

                if (Turn == Seat)
                {
                    foreach (Button b in EnemyButtons)
                    {
                        b.IsEnabled = true;
                    }
                }
                // Let players shoot based on current turn
            }
        }

        private void OnChatMessage(string nickname, string message)
        {
            MessagesListbox.Items.Add(nickname + ": " + message);
        }
    }
}
