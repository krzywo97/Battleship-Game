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
        public async Task ChatMessage(string nickname, string message)
        {
            await Clients.All.SendAsync("ChatMessage", nickname, message);
        }
    }
}
