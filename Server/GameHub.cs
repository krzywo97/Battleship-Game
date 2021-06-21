using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    public class GameHub : Hub
    {
        private const int STATE_STOPPED = 0;
        private const int STATE_ARRANGING_SHIPS = 1;
        private const int STATE_IN_PROGRESS = 2;

        private const int UNIT_NONE = 0;
        private const int UNIT_DESTROYER = 1;
        private const int UNIT_SUBMARINE = 2;
        private const int UNIT_CRUISER = 3;
        private const int UNIT_BATTLESHIP = 4;
        private const int UNIT_CARRIER = 5;
        private const int FIELD_HIT = 10;

        private int GameState = STATE_STOPPED;
        private int[][,] Board = new int[2][,];
        private bool[] ReadyPlayers = new bool[2];

        GameHub()
        {
            Initialize();
        }

        public async void Initialize()
        {
            GameState = STATE_STOPPED;
            Board[0] = new int[10, 10];
            Board[1] = new int[10, 10];
            ReadyPlayers[0] = false;
            ReadyPlayers[1] = false;

            await Clients.All.SendAsync("GameInitialized");
        }

        public async void SendChatMessage(string name, string message)
        {
            await Clients.All.SendAsync("ChatMessage", name, message);
        }

        public async void SetBoard(int player, int[,] board)
        {
            if (GameState != STATE_ARRANGING_SHIPS) return;
            if (player != 0 || player != 1) return;
            if (board.GetLength(0) != 10 || board.GetLength(1) != 10) return;

            //TODO: do proper validation
            /*int[] shipsCount = { 0, 0, 0, 0, 0 };
            if (shipsCount[0] != 4) return;
            if (shipsCount[1] != 3) return;
            if (shipsCount[2] != 2) return;
            if (shipsCount[3] != 1) return;*/

            ReadyPlayers[player] = true;
            Board[player] = board;

            await Clients.All.SendAsync("BoardSet", player.ToString());

            if(ReadyPlayers[0] == true && ReadyPlayers[1] == true)
            {
                await Clients.All.SendAsync("GameStarted");
            }
        }

        public async void Shoot(int player, int x, int y)
        {
            if (player != 0 || player != 1) return;

            int targetPlayer = 1 - player;
            if (x < 0 || x > Board[targetPlayer].GetLength(1)) return;
            if (y < 0 || y < Board[targetPlayer].GetLength(0)) return;
            if (Board[targetPlayer][y, x] >= FIELD_HIT) return;

            Board[targetPlayer][y, x] += FIELD_HIT;
            await Clients.All.SendAsync("ShotFired", player, x, y);

            //TODO: handle end game scenarios
        }
    }
}
