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
        private static Game CurrentGame = new Game();

        public async Task Join(string nickname, int seat)
        {
            if(CurrentGame.JoinPlayer(nickname, seat))
            {
                CurrentGame.State = GameState.ArrangingShips;
                await Clients.All.SendAsync("GameState", "ArrangingShips");
            }
        }

        public async Task Disconnect(int player)
        {
            if(CurrentGame.DisconnectPlayer(player))
            {
                await Clients.All.SendAsync("GameState", "NotStarted");
            }
        }

        public async Task SetShip(int player, byte ship, int x, int y, bool vertical)
        {
            var result = CurrentGame.SetShip(player, ship, x, y, vertical);
            
            await Clients.Caller.SendAsync("ShipSet", result);
        }

        public async Task ReadyUp(int player)
        {
            var result = CurrentGame.ReadyUp(player);

            await Clients.Caller.SendAsync("ReadyUp", result);

            if(CurrentGame.AreBothPlayersReady())
            {
                CurrentGame.Start();
                await Clients.All.SendAsync("GameState", "Started");
            }
        }

        public async Task Fire(int player, int x, int y)
        {
            var result = CurrentGame.Fire(player, x, y);
            
            if(CurrentGame.CheckWin(player) == 1)
            {
                await Clients.All.SendAsync("GameWon", player);
                return;
            }

            if(result)
            {
                await Clients.All.SendAsync("Turn", 1 - player);
            }
        }

        public async Task ChatMessage(string nickname, string message)
        {
            await Clients.All.SendAsync("ChatMessage", nickname, message);
        }
    }
}
