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
        public int Turn { get; private set; }

        public Game()
        {
            Players = new Player[2];
        }

        public void Initialize()
        {
            State = GameState.NotStarted;
            Players[0]?.Initialize();
            Players[1]?.Initialize();
        }

        public void Start()
        {
            State = GameState.Started;
            Turn = 0;
        }

        /// <summary>
        /// Adds player to the game.
        /// </summary>
        /// <param name="name">A nickname of the player</param>
        /// <param name="seat">A seat the player will sit on</param>
        /// <returns>
        /// false when unable to seat a player
        /// true otherwise
        /// </returns>
        public bool JoinPlayer(string name, int seat)
        {
            if (seat != 0 && seat != 1) return false;
            if (Players[seat] != null) return false;
            if (State != GameState.NotStarted) return false;

            var player = new Player(name);
            Players[seat] = player;

            return true;
        }

        /// <summary>
        /// Removes player from the game and initializes all the fields
        /// </summary>
        /// <param name="seat">Seat the player is sitting on</param>
        /// <returns>
        /// false if the seat parameter is invalid
        /// true otherwise
        /// </returns>
        public bool DisconnectPlayer(int seat)
        {
            if (seat != 0 && seat != 1) return false;

            Players[seat] = null;
            Players[1 - seat]?.Initialize();
            State = GameState.NotStarted;
            return true;
        }

        /// <summary>
        /// Marks player as ready to start the game
        /// </summary>
        /// <param name="player">Player to be marked</param>
        /// <returns>
        /// false if the player hasn't set all of their ships
        /// true otherwise
        /// </returns>
        public bool ReadyUp(int player)
        {
            if (player != 0 && player != 1) return false;
            if (Players[player] == null) return false;

            return Players[player].ReadyUp();
        }

        /// <summary>
        /// Sets a ship on the board
        /// </summary>
        /// <param name="player">Player that requested to set the ship</param>
        /// <param name="size">Size of a ship</param>
        /// <param name="x">X coordinate </param>
        /// <param name="y"></param>
        /// <returns>
        /// false if parameters were incorrect
        /// true otherwise
        /// </returns>
        public bool SetShip(int player, int size, int x, int y, bool vertical)
        {
            if (player != 0 && player != 1) return false;
            if (State != GameState.ArrangingShips) return false;
            var ship = Ship.GetShipBySize(size);
            if (ship == Ship.None) return false;

            return Players[player].SetShip(ship, x, y, vertical);
        }

        /// <summary>
        /// Fires a shot at the opposing player's board
        /// </summary>
        /// <param name="attacker">Attacker</param>
        /// <param name="x">X coordinate of the shot</param>
        /// <param name="y">Y coordinate of the shot</param>
        /// <returns>
        /// false if the parameters were incorrect
        /// true otherwise
        /// </returns>
        public bool Fire(int attacker, int x, int y)
        {
            if (attacker != 0 && attacker != 1) return false;
            if (State != GameState.Started) return false;
            if (Turn != attacker) return false;


            var result = Players[1 - attacker].HandleIncomingFire(x, y);
            Turn = 1 - Turn;
            return result;
        }

        public bool AreBothPlayersConnected()
        {
            return Players[0] != null && Players[1] != null;
        }

        public bool AreBothPlayersReady()
        {
            return Players[0]?.Ready == true && Players[1]?.Ready == true;
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
        public int CheckWin(int player)
        {
            if (player != 0 && player != 1) return -1;
            if (Players[1 - player].HasEnabledShips()) return 0;
            return 1;
        }
    }
}
