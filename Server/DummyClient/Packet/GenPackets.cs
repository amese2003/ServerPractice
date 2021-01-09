
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using ServerCore;

public enum PacketID
{
    C_Chat = 1,
	S_Chat = 2,
	
}

interface IPacket
{
	ushort Protocol { get; }
	void Read(ArraySegment<byte> segment);
	ArraySegment<byte> Write();
}


class C_Chat : IPacket
{
	public string chat;	


	public ushort Protocol { get { return (ushort)PacketID.C_Chat; } }

    public void Read(ArraySegment<byte> segment)
    {
		ushort pos = 0;

		pos += sizeof(ushort);
		pos += sizeof(ushort);
		ushort chatLeng = BitConverter.ToUInt16(segment.Array, segment.Offset + pos);
		pos += sizeof(ushort);
		this.chat = Encoding.Unicode.GetString(segment.Array, segment.Offset + pos, chatLeng);
		pos += chatLeng;
		
	}

	public ArraySegment<byte> Write()
    {
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort pos = 0;


		pos += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_Chat), 0, segment.Array, segment.Offset + pos, sizeof(ushort));
		pos += sizeof(ushort);
		ushort chatLeng = (ushort)Encoding.Unicode.GetBytes(this.chat, 0, this.chat.Length, segment.Array, segment.Offset + pos + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(chatLeng), 0, segment.Array, segment.Offset + pos, sizeof(ushort));
		pos += sizeof(ushort);
		pos += chatLeng;
		
		Array.Copy(BitConverter.GetBytes(pos), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(pos);
	}
}

class S_Chat : IPacket
{
	public int playerId;
	public string chat;	


	public ushort Protocol { get { return (ushort)PacketID.S_Chat; } }

    public void Read(ArraySegment<byte> segment)
    {
		ushort pos = 0;

		pos += sizeof(ushort);
		pos += sizeof(ushort);
		this.playerId = BitConverter.ToInt32(segment.Array, segment.Offset + pos);
		pos += sizeof(int);
		
		ushort chatLeng = BitConverter.ToUInt16(segment.Array, segment.Offset + pos);
		pos += sizeof(ushort);
		this.chat = Encoding.Unicode.GetString(segment.Array, segment.Offset + pos, chatLeng);
		pos += chatLeng;
		
	}

	public ArraySegment<byte> Write()
    {
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort pos = 0;


		pos += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_Chat), 0, segment.Array, segment.Offset + pos, sizeof(ushort));
		pos += sizeof(ushort);
		
		Array.Copy(BitConverter.GetBytes(playerId), 0, segment.Array, segment.Offset + pos, sizeof(int));
		pos += sizeof(int);
		
		ushort chatLeng = (ushort)Encoding.Unicode.GetBytes(this.chat, 0, this.chat.Length, segment.Array, segment.Offset + pos + sizeof(ushort));
		Array.Copy(BitConverter.GetBytes(chatLeng), 0, segment.Array, segment.Offset + pos, sizeof(ushort));
		pos += sizeof(ushort);
		pos += chatLeng;
		
		Array.Copy(BitConverter.GetBytes(pos), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(pos);
	}
}

