using System;
using System.Collections.Generic;
using System.Text;
using ServerCore;

namespace Server
{
    class GameRoom : IJobQueue
    {
        List<ClientSession> _sessions = new List<ClientSession>();
        JobQueue _jobQueue = new JobQueue();

        public void Push(Action job)
        {
            _jobQueue.Push(job);
        }

        public void BroadCast(ClientSession session, string chat)
        {
            S_Chat packet = new S_Chat();
            packet.playerId = session.SessionID;
            packet.chat = $" {chat} 난 누굴까? {packet.playerId} ";
            ArraySegment<byte> segment = packet.Write();

            // O(N^2) 으악;;;;
            foreach (ClientSession s in _sessions)
            {
                s.Send(segment);
            }

        }

        public void Enter(ClientSession session)
        {

            _sessions.Add(session);
            session.Room = this;

        }

        public void Leave(ClientSession session)
        {

            _sessions.Remove(session);
            
        }

        
    }
}
