using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server
{
    public static class Ship
    {
        public static byte None = 0;
        public static byte Destroyer = 1;
        public static byte Submarine = 2;
        public static byte Cruiser = 4;
        public static byte Battleship = 8;
        public static byte Hit = 16;

        public static int GetSize(byte ship)
        {
            ship = UnmarkAsHit(ship);
            if (ship == Destroyer) return 1;
            if (ship == Submarine) return 2;
            if (ship == Cruiser) return 3;
            if (ship == Battleship) return 4;
            return 0;
        }

        public static bool IsShip(byte ship)
        {
            ship = UnmarkAsHit(ship);

            return ((ship != Ship.Destroyer)
                || (ship != Ship.Submarine)
                || (ship != Ship.Cruiser)
                || (ship == Ship.Battleship));
        }

        public static bool IsValid(byte ship)
        {
            ship = UnmarkAsHit(ship);

            return ((ship == Ship.None)
                || IsShip(ship));
        }

        public static bool IsHit(byte ship)
        {
            return (ship & Hit) == Hit;
        }

        public static byte MarkAsHit(byte ship)
        {
            return (byte)(ship | Hit);
        }

        public static byte UnmarkAsHit(byte ship)
        {
            return (byte)(ship & (Destroyer | Submarine | Cruiser | Battleship));
        }

    }
}
