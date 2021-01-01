using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using ServerCore;

namespace Server
{

    class Packet
    {
        public ushort size;
        public ushort packetId;
    }
    class PlayerInfoReq : Packet
    {
        public long playerId;
    }

    class PlayerInfoOk : Packet
    {
        public int hp;
        public int attack;
    }

    public enum PacketID
    {
        PlayerInfoReq = 1,
        PlayerInfoOk
    }

    class ClientSession : PacketSession
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected: {endPoint}");

            // Packet packet = new Packet() { size = 100, packetId = 10 };

            //byte[] sendBuff = Encoding.UTF8.GetBytes("Welcome to MMORPG Server.");

            //ArraySegment<byte> openSegment = SendBufferHelper.Open(4096);
            //byte[] buffer = BitConverter.GetBytes(packet.size);
            //byte[] buffer2 = BitConverter.GetBytes(packet.packetId);
            //Array.Copy(buffer, 0, openSegment.Array, openSegment.Offset, buffer.Length);
            //Array.Copy(buffer2, 0, openSegment.Array, openSegment.Offset + buffer.Length, buffer2.Length);
            //ArraySegment<byte> sendBuff = SendBufferHelper.Close(buffer.Length + buffer2.Length);

            //Send(sendBuff);
            Thread.Sleep(5000);
            Disconnect();
        }

        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            int pos = 0;

            ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
            pos += 2;
            ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + pos);
            pos += 2;

            switch ((PacketID)id)
            {
                case PacketID.PlayerInfoReq:
                    long playerId = BitConverter.ToInt64(buffer.Array, buffer.Offset + pos);
                    pos += 8;
                    break;

                case PacketID.PlayerInfoOk:
                    int hp = BitConverter.ToInt32(buffer.Array, buffer.Offset + pos);
                    pos += 4;
                    int attack = BitConverter.ToInt32(buffer.Array, buffer.Offset + pos);
                    pos += 4;
                    break;
                default:
                    break;
            }



            //ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
            //ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + 2);
            Console.WriteLine($"RecvPacketId : {id}, Size : {size}");
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected: {endPoint}");
        }


        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Transferred bytes: {numOfBytes}");
        }
    }
}
