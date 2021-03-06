﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    interface ITask
    {
        void Execute();
    }

    class BroadcastTask : ITask
    {
        GameRoom _room;
        ClientSession _session;
        string _chat;

        BroadcastTask(GameRoom room,ClientSession session, string chat)
        {
            _room = room;
            _session = session;
            _chat = chat;
        }
        public void Execute()
        {
            _room.BroadCast(_session, _chat);
        }
    }

    class TaskQueue 
    {
        Queue<ITask> _queue = new Queue<ITask>();

    }
}
