using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server
{
    public class Game
    {
        public GameState State;
        public Player[] Players { get; private set; }

        public Game()
        {
            Players = new Player[2];
        }

        public void Initialize()
        {
            State = GameState.NotStarted;
        }

        public void JoinPlayer(string name, int seat)
        {
            if (seat != 0 && seat != 1) return;
            if (Players[seat] != null) return;
            if (State != GameState.NotStarted) return;

            var player = new Player(name);
            Players[seat] = player;
        }

        public void DisconnectPlayer(int seat)
        {
            if (seat != 0 && seat != 1) return;

            Players[seat] = null;
            Players[1 - seat].Initialize();
            State = GameState.NotStarted;
        }

        public void SetShip(byte ship, int player, int x, int y)
        {
            if (player != 0 && player != 1) return;
            if (State != GameState.ArrangingShips) return;

            Players[player].SetShip(ship, x, y); // TODO: emit event if success
        }

        public void Fire(int player, int x, int y)
        {
            if (player != 0 && player != 1) return;
            if (State != GameState.Started) return;

            Players[1 - player].HandleFire(x, y); // TODO: emit event based on result of the call
        }

        /// <summary>
        /// Method checks whether a game has finished or not
        /// </summary>
        /// <param name="player">Seat of a player that has just fired a shot</param>
        /// <returns>
        /// -1 when precondition fails
        /// 0 when the game is still in progress
        /// 1 when the player has won
        /// </returns>
        private int CheckWin(int player)
        {
            if (player != 0 && player != 1) return -1;
            if (Players[1 - player].HasEnabledShips()) return 0;
            return 1;
        }
    }
}
