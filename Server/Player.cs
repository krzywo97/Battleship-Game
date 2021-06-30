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
        public int[] ArrangedShipsCount { get; private set; }

        public Player(string name)
        {
            Name = name;
            Initialize();
        }

        public void Initialize()
        {
            Ready = false;
            Board = new byte[10, 10];
            ArrangedShipsCount = new int[4];
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
            // TODO: refactor this method so it looks clearer
            Ready = (ArrangedShipsCount[0] == 4)
                && (ArrangedShipsCount[1] == 3)
                && (ArrangedShipsCount[2] == 2)
                && (ArrangedShipsCount[3] == 1);

            return Ready;
        }

        public bool SetShip(byte ship, int x, int y, bool vertical)
        {
            if (!IsPositionInBounds(x, y)) return false;
            if (!Ship.IsValid(ship)) return false;

            var size = Ship.GetSize(ship);
            if (!ValidatePlacement(x, y, size, vertical)) return false;
            if (ArrangedShipsCount[size - 1] == 5 - size) return false;

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

            ArrangedShipsCount[size - 1]++;
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
            if (x < 0 || x > 9 || y < 0 || y > 9) return false;

            //Does the ship fit inside the bounds?
            if ((vertical && y + size - 1 > 9) || (!vertical && x + size - 1 > 9)) return false;

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
                    if (Board[i, j] != (int)Ship.None) return false;
                }
            }

            return true;
        }
    }
}
