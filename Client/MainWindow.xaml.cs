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
using Microsoft.Xaml.Behaviors;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string[] ContextMenuItems = { "Krążownik poziomo\t1x1", "Okręt podwodny poziomo\t2x1",
            "Niszczyciel poziomo\t3x1", "Pancernik poziomo\t4x1", "Krążownik pionowo\t1x1",
            "Okręt podwodny pionowo\t1x2","Niszczyciel pionowo\t1x3", "Pancernik pionowo\t1x4" };

        private Button[,] MyButtons = new Button[10, 10];
        private Button[,] EnemyButtons = new Button[10, 10];

        private HubConnection Connection;
        private GameClient Client;

        public MainWindow()
        {
            InitializeComponent();
            InitializeUi();
        }

        /**
         * Private UI control methods
         */
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

        private void ClearBoards()
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
            b.IsEnabled = false;
            if (myBoard)
            {
                b.ContextMenu = new ContextMenu();
                for (int i = 0; i < ContextMenuItems.Length; i++)
                {
                    var tag = new int[] { x, y, (i % 4) + 1, i / 4 };
                    var item = new MenuItem { Header = ContextMenuItems[i], Tag = tag };
                    item.Click += HandleShipArrangement;
                    b.ContextMenu.Items.Add(item);
                }

                BehaviorCollection behaviors = Interaction.GetBehaviors(b);
                behaviors.Add(new DropDownMenuBehavior());
            }
            else
            {
                b.Tag = new int[2] { x, y };
                b.Click += HandleShot;
            }

            return b;
        }

        private void EnableMyBoard(bool enable)
        {
            foreach (Button b in MyButtons)
            {
                var content = b.Content != null ? b.Content.ToString() : "";
                b.IsEnabled = enable && content.Length == 0;
            }
        }

        private void EnableEnemyBoard(bool enable)
        {
            foreach (Button b in EnemyButtons)
            {
                var content = b.Content != null ? b.Content.ToString() : "";
                b.IsEnabled = enable && content.Length == 0;
            }
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

            Client = new GameClient(SeatCombobox.SelectedIndex);

            Connection = new HubConnectionBuilder()
                .WithUrl(@"http://localhost:4000/hubs/battleship")
                .Build();

            Connection.On<bool>("JoinResult", OnJoinResult);
            Connection.On<string, string>("ChatMessage", OnChatMessage);
            Connection.On<bool, int, int, int, bool>("ShipSet", OnSetShipResult);
            Connection.On<string>("GameState", OnGameStateChanged);
            Connection.On<string>("PlayerReady", OnPlayerReady);
            Connection.On<int, int, int, bool>("ShotFired", OnShotFired);
            Connection.On<string>("GameWon", OnGameWon);

            Connection.StartAsync();
            Connection.SendAsync("Join", NicknameTextbox.Text, Client.Seat);
        }

        private void HandleShipArrangement(object sender, RoutedEventArgs e)
        {
            var b = sender as MenuItem;
            int[] tag = b.Tag as int[];
            int x = tag[0];
            int y = tag[1];
            int size = tag[2];
            bool vertical = tag[3] == 1;

            Connection.SendAsync("SetShip", Client.Seat, size, x, y, vertical);
            EnableMyBoard(false);
        }

        private void HandleClearBoard(object sender, RoutedEventArgs e)
        {
            // TODO: implement this method
        }

        private void HandleShot(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            int[] tag = button.Tag as int[];
            int x = tag[0];
            int y = tag[1];

            Connection.SendAsync("Fire", Client.Seat, x, y);
            EnableEnemyBoard(false);
        }

        private void HandleAction(object sender, RoutedEventArgs e)
        {
            if (Client.State == GameState.ArrangingShips)
            {
                EnableMyBoard(false);
                Connection.SendAsync("ReadyUp", Client.Seat);
            }

            ActionButton.IsEnabled = false;
        }

        private void SendChatMessage(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            if (MessageTextbox.Text.Length == 0) return;

            Connection.SendAsync("ChatMessage", Client.Seat, MessageTextbox.Text);
            MessageTextbox.Text = "";
        }

        /*
         * Game event handlers
         */
        private void OnJoinResult(bool result)
        {
            if (result)
            {
                MessagesListbox.Items.Add("Zajęto miejsce: " + (Client.Seat + 1));
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
                Client.State = GameState.Stopped;
                ClearBoards();
                // Disable the UI completely except chat
            }
            else if (newState == "ArrangingShips")
            {
                Client.State = GameState.ArrangingShips;

                MessagesListbox.Items.Add("Obaj gracze połączeni. Pora rozstawić okręty");
                ActionButton.Content = "Gotowy";
                ActionButton.IsEnabled = true;

                EnableMyBoard(true);
                EnableEnemyBoard(false);
            }
            else if (newState == "Started")
            {
                Client.State = GameState.Started;

                MessagesListbox.Items.Add("Gra rozpoczęta. Kolej gracza 1");

                EnableMyBoard(false);
                EnableEnemyBoard(Client.Turn == Client.Seat);
            }
        }

        private void OnChatMessage(string nickname, string message)
        {
            MessagesListbox.Items.Add(nickname + ": " + message);
        }

        private void OnSetShipResult(bool result, int x, int y, int size, bool vertical)
        {
            if (result)
            {
                if (vertical)
                {
                    for (int i = y; i < y + size; i++)
                    {
                        MyButtons[i, x].Content = "#";
                    }
                }
                else
                {
                    for (int i = x; i < x + size; i++)
                    {
                        MyButtons[y, i].Content = "#";
                    }
                }
            }

            EnableMyBoard(true);
        }

        private void OnPlayerReady(string nickname)
        {
            MessagesListbox.Items.Add("Gracz " +nickname + " gotowy");
        }

        private void OnShotFired(int attacker, int x, int y, bool hit)
        {
            if (attacker == Client.Seat)
            {
                EnemyButtons[y, x].Content = hit ? "X" : "O";
            }
            else
            {
                MyButtons[y, x].Content = hit ? "X" : "O";
            }

            if (!hit)
            {
                Client.ChangeTurn();
            }

            EnableEnemyBoard(Client.Turn == Client.Seat);
        }

        private void OnGameWon(string nickname)
        {
            MessagesListbox.Items.Add("Wygrał gracz " + nickname);

            EnableMyBoard(false);
            EnableEnemyBoard(false);

            Client.State = GameState.Stopped;
        }
    }
}
