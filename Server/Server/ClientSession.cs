using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using ServerCore;

namespace Server
{

    public abstract class Packet
    {
        public ushort size;
        public ushort packetId;

        public abstract ArraySegment<byte> Write();
        public abstract void Read(ArraySegment<byte> s);
    }

    class PlayerInfoReq : Packet
    {
        public long playerId;

        // UTF-16!
        public string name;

        public PlayerInfoReq()
        {
            this.packetId = (ushort)PacketID.PlayerInfoReq;
        }

        public override void Read(ArraySegment<byte> segment)
        {
            ushort pos = 0;

            ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);

            //ushort size = BitConverter.ToUInt16(s.Array, s.Offset);
            pos += sizeof(ushort);
            //ushort id = BitConverter.ToUInt16(s.Array, s.Offset + pos);
            pos += sizeof(ushort);

            this.playerId = BitConverter.ToInt64(s.Slice(pos, s.Length - pos));

            pos += sizeof(long);

            // string
            ushort nameLeng = BitConverter.ToUInt16(s.Slice(pos, s.Length - pos));
            pos += sizeof(ushort);
            this.name = Encoding.Unicode.GetString(s.Slice(pos, nameLeng));

        }

        public override ArraySegment<byte> Write()
        {
            ArraySegment<byte> segment = SendBufferHelper.Open(4096);

            ushort pos = 0;
            bool success = true;

            Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

            pos += sizeof(ushort);
            success &= BitConverter.TryWriteBytes(s.Slice(pos, s.Length - pos), packetId);
            pos += sizeof(ushort);
            success &= BitConverter.TryWriteBytes(s.Slice(pos, s.Length - pos), this.playerId);
            pos += sizeof(long);

            // string
            ushort nameLeng = (ushort)Encoding.Unicode.GetByteCount(this.name);
            success &= BitConverter.TryWriteBytes(s.Slice(pos, s.Length - pos), nameLeng);
            pos += sizeof(ushort);
            Array.Copy(Encoding.Unicode.GetBytes(this.name), 0, segment.Array, pos, nameLeng);
            pos += nameLeng;

            success &= BitConverter.TryWriteBytes(s, pos);

            if (success == false)
                return null;

            return SendBufferHelper.Close(pos);
        }
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
            ushort pos = 0;

            ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
            pos += 2;
            ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + pos);
            pos += 2;

            switch ((PacketID)id)
            {
                case PacketID.PlayerInfoReq:
                    PlayerInfoReq p = new PlayerInfoReq();
                    p.Read(buffer);
                    Console.WriteLine($"PlayerInfoReq: {p.playerId} {p.name}");
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
