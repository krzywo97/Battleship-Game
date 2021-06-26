using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Game
    {
        /*
         * Message callbacks for the UI
         */
        public Action<int> OnPlayerJoined { get; set; }
        public Action<int> OnPlayerLeft { get; set; }
        public Action<int> OnPlayerReady { get; set; }
        public Action<int, Ship, bool, int, int> OnShipArranged { get; set; }
        public Action OnGameStarted { get; set; }
        public Action OnGameFinished { get; set; }
        public Action<int, int, int> OnShotFired { get; set; }
        public Action<string, string> OnChatMessage { get; set; }

        /*
         * Game state variables
         */
        private int PlayerId, Turn;
        private string Nickname;
        private GameState CurrentState;
        private PlayerState[] Players = new PlayerState[2];
        private int[][,] Board = new int[2][,];

        private HubConnection Connection;

        public Game(string hubUrl, int player, string nickname)
        {
            PlayerId = player;
            Nickname = nickname;

            Connection = new HubConnectionBuilder()
            .WithUrl(hubUrl)
            .Build();

            Connection.On<int>("PlayerJoined", HandlePlayerJoined);
            Connection.On<int>("PlayerLeft", HandlePlayerLeft);
            Connection.On<int>("PlayerReady", HandlePlayerReady);
            Connection.On<int, Ship, bool, int, int>("ShipArranged", HandleShipArranged);
            Connection.On("GameStarted", HandleGameStarted);
            Connection.On("GameFinished", HandleGameFinished);
            Connection.On<int, int, int>("ShotFired", HandleShotFired);
            Connection.On<string, string>("MessageSent", HandleChatMessage);
        }

        private void Initialize()
        {
            CurrentState = GameState.Stopped;
            Board[0] = new int[10, 10];
            Board[1] = new int[10, 10];
            Players[0] = PlayerState.Connected;
            Players[1] = PlayerState.Connected;
        }

        /*
         * Local message handlers
         */
        private void HandlePlayerJoined(int player)
        {
            Players[player] = PlayerState.Connected;
            OnPlayerJoined(player);
        }

        private void HandlePlayerLeft(int player)
        {
            Players[player] = PlayerState.NotConnected;
            OnPlayerLeft(player);
        }

        private void HandlePlayerReady(int player)
        {
            Players[player] = PlayerState.Ready;
            OnPlayerReady(player);
        }

        private void HandleShipArranged(int player, Ship ship, bool vertical, int x, int y)
        {
            OnShipArranged(player, ship, vertical, x, y);
        }

        private void HandleGameStarted()
        {
            CurrentState = GameState.InProgress;
            OnGameStarted();
        }

        private void HandleGameFinished()
        {
            CurrentState = GameState.Stopped;
            OnGameFinished();
        }

        private void HandleShotFired(int player, int x, int y)
        {
            OnShotFired(player, x, y);
        }

        private void HandleChatMessage(string nickname, string message)
        {
            OnChatMessage(nickname, message);
        }

        /*
         * Public methods for performing actions
         */
        public void ArrangeShip(int shipType, bool vertical, int x, int y)
        {
            if (CurrentState != GameState.ArrangingShips) return;


        }

        public void FireShot(int x, int y)
        {
            if (Turn != PlayerId) return;

            Connection.SendAsync("ShotFired", PlayerId, x, y);
        }

        public void SendChatMessage(string message)
        {
            Connection.SendAsync("ChatMessage", Nickname, message);
        }
    }

    enum GameState
    {
        Stopped,
        ArrangingShips,
        InProgress
    }

    enum Ship
    {
        None = 0,
        Destroyer = 1,
        Submarine = 2,
        Cruiser = 4,
        Battleship = 8,
        Carrier = 16,
        Hit = 32
    }

    enum PlayerState
    {
        NotConnected,
        Connected,
        Ready
    }
}
