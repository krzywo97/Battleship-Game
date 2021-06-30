using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class GameClient
    {
        public GameState State { get; set; } = GameState.Stopped;
        public int Seat { get; }
        public int Turn { get; private set; }

        public GameClient(int seat)
        {
            Seat = seat;
            Turn = 0;
        }

        public void ChangeTurn()
        {
            Turn = 1 - Turn;
        }

        public bool IsMyTurn()
        {
            return Seat == Turn;
        }
    }
}
