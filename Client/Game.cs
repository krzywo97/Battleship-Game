using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

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
        //public Action<int, Ship, bool, int, int> OnShipArranged { get; set; }
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
            /*PlayerId = player;
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
            Connection.On<string, string>("ChatMessage", HandleChatMessage);*/
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

        /*private void HandleShipArranged(int player, Ship ship, bool vertical, int x, int y)
        {
            OnShipArranged(player, ship, vertical, x, y);
        }*/

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
        public void Join()
        {
            Players[PlayerId] = PlayerState.Connected;
            Connection.SendAsync("PlayerJoined", PlayerId);
        }

        public void Disconnect()
        {
            Players[PlayerId] = PlayerState.NotConnected;
            Connection.SendAsync("PlayerLeft", PlayerId);
        }

        public void ReadyUp()
        {
            Players[PlayerId] = PlayerState.Ready;
            Connection.SendAsync("PlayerReady", PlayerId);
        }

        /*public void ArrangeShip(Ship ship, bool vertical, int x, int y)
        {
            if (CurrentState != GameState.ArrangingShips) return;
            if (!ValidateArrangement(ship, vertical, x, y)) return;

            var size = GetShipLength(ship);
            if (vertical)
            {
                for (int i = y; i <= y + size; i++)
                {
                    Board[0][i, x] = (int)ship;
                }
            }
            else
            {
                for (int i = x; i <= x + size; i++)
                {
                    Board[0][y, i] = (int)ship;
                }
            }

            Connection.SendAsync("ShipArranged", PlayerId, (int)ship, vertical, x, y);
        }*/

        public void FireShot(int x, int y)
        {
            if (Turn != PlayerId) return;

            Connection.SendAsync("ShotFired", PlayerId, x, y);
        }

        /*
         * Other stuff
         */

        /*private bool ValidateArrangement(Ship ship, bool vertical, int x, int y)
        {
            if (ship != Ship.Destroyer || ship != Ship.Submarine || ship != Ship.Cruiser || ship != Ship.Battleship) return false;
            if (x < 0 || x > 9 || y < 0 || y > 9) return false;

            //Does the ship fit inside the bounds?
            var size = GetShipLength(ship);
            if ((vertical && y + size > 9) || (!vertical && x + size > 9)) return false;

            var x1 = x - 1;
            var x2 = vertical ? x + 1 : x + size;
            var y1 = y - 1;
            var y2 = vertical ? y + size : y + 1;

            //Is there enough free space around the ship?
            for (int i = y1; i <= y2; i++)
            {
                if (i < 0 || i > 9) continue;
                for (int j = x1; j <= x2; j++)
                {
                    if (j < 0 || j > 9) continue;
                    if (Board[0][i, j] != (int)Ship.None) return false;
                }
            }

            return true;
        }*/
    }

    enum GameState
    {
        Stopped,
        ArrangingShips,
        InProgress
    }

    enum PlayerState
    {
        NotConnected,
        Connected,
        Ready
    }
}
