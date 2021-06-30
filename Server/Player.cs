using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server
{
    public class Player
    {
        public string Name { get; }
        public bool Ready { get; private set; }
        public byte[,] Board { get; private set; }

        public Player(string name)
        {
            Name = name;
            Initialize();
        }

        public void Initialize()
        {
            Ready = false;
            Board = new byte[10, 10];
        }

        /// <summary>
        /// Marks the player as ready to start the game
        /// </summary>
        /// <returns>
        /// false if player hasn't set all of their ships
        /// true otherwise
        /// </returns>
        public bool ReadyUp()
        {
            // TODO: Validate player's board
            Ready = true;
            return true;
        }

        public bool SetShip(byte ship, int x, int y, bool vertical)
        {
            if (!IsPositionInBounds(x, y)) return false;
            if (!Ship.IsValid(ship)) return false;

            var size = Ship.GetSize(ship);
            if(!ValidatePlacement(x, y, size, vertical)) return false;

            if (vertical)
            {
                for (int i = y; i < y + size; i++)
                {
                    Board[i, x] = ship;
                }
            }
            else
            {
                for (int i = x; i < x + size; i++)
                {
                    Board[y, i] = ship;
                }
            }
            return true;
        }

        /// <summary>
        /// Handles incoming fire from the enemy
        /// </summary>
        /// <param name="x">X coordinate of the shot</param>
        /// <param name="y">Y coordinate of the shot</param>
        /// <returns>
        /// false if the ship was not hit
        /// true otherwise
        /// </returns>
        public bool HandleIncomingFire(int x, int y)
        {
            if (!IsPositionInBounds(x, y)) return false;

            var ship = Board[y, x];
            if (ship == Ship.None) return false;
            if (Ship.IsHit(ship)) return false;

            Board[y, x] = Ship.MarkAsHit(ship);
            return true;
        }

        public bool HasEnabledShips()
        {
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    var ship = Board[i, j];
                    if (Ship.IsShip(ship) && !Ship.IsHit(ship)) return true;
                }
            }

            return false;
        }

        private bool IsPositionInBounds(int x, int y)
        {
            return ((x >= 0) && (x <= 9) && (y >= 0) && (y <= 9));
        }

        private bool ValidatePlacement(int x, int y, int size, bool vertical)
        {
            // TODO: implement this method

            return true;
        }
    }
}
