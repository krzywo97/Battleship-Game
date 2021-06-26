using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Game
    {

        private const int STATE_STOPPED = 0;
        private const int STATE_ARRANGING_SHIPS = 1;
        private const int STATE_IN_PROGRESS = 2;

        public Action OnGameStarted { get; set; }
        public Action OnGameFinished { get; set; }
        public Action<int, int> OnShotFired { get; set; }
        public Action<string> OnChatMessage { get; set; }

        private int State;

        private HubConnection Connection;

        public Game(string hubUrl)
        {
            Connection = new HubConnectionBuilder()
            .WithUrl(hubUrl)
            .Build();

            Connection.On("GameStarted", HandleGameStarted);
            Connection.On("GameFinished", HandleGameFinished);
            Connection.On<int, int>("ShotFired", HandleShotFired);
            Connection.On<string>("MessageSent", HandleChatMessage);
        }

        private void Initialize()
        {
            State = STATE_STOPPED;
        }

        private void HandleGameStarted()
        {
            State = STATE_IN_PROGRESS;
            OnGameStarted();
        }

        private void HandleGameFinished()
        {
            State = STATE_STOPPED;
            OnGameFinished();
        }

        private void HandleShotFired(int x, int y)
        {
            OnShotFired(x, y);
        }

        private void HandleChatMessage(string message)
        {
            OnChatMessage(message);
        }
    }
}
