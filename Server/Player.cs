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

        public void ReadyUp()
        {
            Ready = true;
        }

        public void SetShip(byte ship, int x, int y)
        {
            if (!IsPositionInBounds(x, y)) return;
            if (!Ship.IsValid(ship)) return;

            Board[y, x] = ship;
        }

        public void HandleFire(int x, int y)
        {
            if (!IsPositionInBounds(x, y)) return;

            var ship = Board[y, x];
            if (Ship.IsHit(ship)) return;

            Board[y, x] = Ship.MarkAsHit(ship); //TODO: return status codes / throw exception if preconditions fail
        }

        private bool IsPositionInBounds(int x, int y)
        {
            return ((x >= 0) && (x <= 9) && (y >= 0) && (y <= 9));
        }
    }
}
