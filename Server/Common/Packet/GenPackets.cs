
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

		ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
		pos += sizeof(ushort);
		pos += sizeof(ushort);
		ushort chatLeng = BitConverter.ToUInt16(s.Slice(pos, s.Length - pos));
		pos += sizeof(ushort);
		this.chat = Encoding.Unicode.GetString(s.Slice(pos, chatLeng));
		pos += chatLeng;
		
	}

	public ArraySegment<byte> Write()
    {
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort pos = 0;
		bool success = true;

		Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

		pos += sizeof(ushort);
		success &= BitConverter.TryWriteBytes(s.Slice(pos, s.Length - pos), (ushort)PacketID.C_Chat);
		pos += sizeof(ushort);
		ushort chatLeng = (ushort)Encoding.Unicode.GetBytes(this.chat, 0, this.chat.Length, segment.Array, segment.Offset + pos + sizeof(ushort));
		success &= BitConverter.TryWriteBytes(s.Slice(pos, s.Length - pos), chatLeng);
		pos += sizeof(ushort);
		pos += chatLeng;
		
		success &= BitConverter.TryWriteBytes(s, pos);

		if (success == false)
			return null;

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

		ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
		pos += sizeof(ushort);
		pos += sizeof(ushort);
		this.playerId = BitConverter.ToInt32(s.Slice(pos, s.Length - pos));
		pos += sizeof(int);
		
		ushort chatLeng = BitConverter.ToUInt16(s.Slice(pos, s.Length - pos));
		pos += sizeof(ushort);
		this.chat = Encoding.Unicode.GetString(s.Slice(pos, chatLeng));
		pos += chatLeng;
		
	}

	public ArraySegment<byte> Write()
    {
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort pos = 0;
		bool success = true;

		Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

		pos += sizeof(ushort);
		success &= BitConverter.TryWriteBytes(s.Slice(pos, s.Length - pos), (ushort)PacketID.S_Chat);
		pos += sizeof(ushort);
		success &= BitConverter.TryWriteBytes(s.Slice(pos, s.Length - pos), this.playerId);
		pos += sizeof(int);
		
		ushort chatLeng = (ushort)Encoding.Unicode.GetBytes(this.chat, 0, this.chat.Length, segment.Array, segment.Offset + pos + sizeof(ushort));
		success &= BitConverter.TryWriteBytes(s.Slice(pos, s.Length - pos), chatLeng);
		pos += sizeof(ushort);
		pos += chatLeng;
		
		success &= BitConverter.TryWriteBytes(s, pos);

		if (success == false)
			return null;

		return SendBufferHelper.Close(pos);
	}
}

