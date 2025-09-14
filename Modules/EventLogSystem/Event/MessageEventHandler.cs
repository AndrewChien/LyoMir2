﻿using EventLogSystem.Services;
using MediatR;
using SystemModule.ModuleEvent;

namespace EventLogSystem.Event
{
    public class MessageEventHandler : INotificationHandler<GameMessageEvent>
    {
        private readonly IGameEventSource _gameEventSource;

        public MessageEventHandler(IGameEventSource gameEventSource)
        {
            _gameEventSource = gameEventSource;
        }

        public Task Handle(GameMessageEvent notification, CancellationToken cancellationToken)
        {
            //todo 解析游戏事件

            //_gameEventSource.AddEventLog();
            return Task.CompletedTask;
        }
    }
}