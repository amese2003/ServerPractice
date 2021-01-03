﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using ServerCore;

namespace DummyClient
{
	class PlayerInfoReq
	{
		public long playerId;
		public string name;

		public struct Skill
		{
			public int id;
			public short level;
			public float duration;

			public void Read(ReadOnlySpan<byte> s, ref ushort pos)
			{
				this.id = BitConverter.ToInt32(s.Slice(pos, s.Length - pos));
				pos += sizeof(int);

				this.level = BitConverter.ToInt16(s.Slice(pos, s.Length - pos));
				pos += sizeof(short);

				this.duration = BitConverter.ToSingle(s.Slice(pos, s.Length - pos));
				pos += sizeof(float);

			}

			public bool Wrtie(Span<byte> s, ref ushort pos)
			{
				bool success = true;
				success &= BitConverter.TryWriteBytes(s.Slice(pos, s.Length - pos), this.id);
				pos += sizeof(int);

				success &= BitConverter.TryWriteBytes(s.Slice(pos, s.Length - pos), this.level);
				pos += sizeof(short);

				success &= BitConverter.TryWriteBytes(s.Slice(pos, s.Length - pos), this.duration);
				pos += sizeof(float);


				return success;
			}
		}

		public List<Skill> skills = new List<Skill>();



		public void Read(ArraySegment<byte> segment)
		{
			ushort pos = 0;

			ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
			pos += sizeof(ushort);
			pos += sizeof(ushort);
			this.playerId = BitConverter.ToInt64(s.Slice(pos, s.Length - pos));
			pos += sizeof(long);

			ushort nameLeng = BitConverter.ToUInt16(s.Slice(pos, s.Length - pos));
			pos += sizeof(ushort);
			this.name = Encoding.Unicode.GetString(s.Slice(pos, nameLeng));
			pos += nameLeng;

			this.skills.Clear();
			ushort skillLeng = BitConverter.ToUInt16(s.Slice(pos, s.Length - pos));
			pos += sizeof(ushort);
			for (int i = 0; i < skillLeng; i++)
			{
				Skill skill = new Skill();
				skill.Read(s, ref pos);
				skills.Add(skill);
			}

		}

		public ArraySegment<byte> Write()
		{
			ArraySegment<byte> segment = SendBufferHelper.Open(4096);
			ushort pos = 0;
			bool success = true;

			Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

			pos += sizeof(ushort);
			success &= BitConverter.TryWriteBytes(s.Slice(pos, s.Length - pos), (ushort)PacketID.PlayerInfoReq);
			pos += sizeof(ushort);
			success &= BitConverter.TryWriteBytes(s.Slice(pos, s.Length - pos), this.playerId);
			pos += sizeof(long);

			ushort nameLeng = (ushort)Encoding.Unicode.GetBytes(this.name, 0, this.name.Length, segment.Array, segment.Offset + pos + sizeof(ushort));
			success &= BitConverter.TryWriteBytes(s.Slice(pos, s.Length - pos), nameLeng);
			pos += sizeof(ushort);
			pos += nameLeng;

			success &= BitConverter.TryWriteBytes(s.Slice(pos, s.Length - pos), (ushort)this.skills.Count);
			pos += sizeof(ushort);

			foreach (Skill skill in this.skills)
				success &= skill.Wrtie(s, ref pos);

			success &= BitConverter.TryWriteBytes(s, pos);

			if (success == false)
				return null;

			return SendBufferHelper.Close(pos);
		}
	}




	public enum PacketID
	{
		PlayerInfoReq = 1,
		PlayerInfoOk = 2,
	}
	class ServerSession : Session
	{
		static unsafe void ToBytes(byte[] array, int offset, ulong value)
        {
			fixed (byte* ptr = &array[offset])
				*(ulong*)ptr = value;
        }

		static unsafe void ToBytes<T>(byte[] array, int offset, T value) where T : unmanaged
		{
			fixed (byte* ptr = &array[offset])
				*(T*)ptr = value;
		}

		public override void OnConnected(EndPoint endPoint)
		{
			Console.WriteLine($"OnConnected : {endPoint}");

			PlayerInfoReq packet = new PlayerInfoReq() { playerId = 1001, name = "abcd" };
			packet.skills.Add(new PlayerInfoReq.Skill() { id = 101, level = 1 ,duration = 3.0f });
			packet.skills.Add(new PlayerInfoReq.Skill() { id = 201, level = 2, duration = 4.0f });
			packet.skills.Add(new PlayerInfoReq.Skill() { id = 301, level = 3, duration = 5.0f });
			packet.skills.Add(new PlayerInfoReq.Skill() { id = 401, level = 4, duration = 6.0f });

			// 보낸다
			//for (int i = 0; i < 5; i++)
			{
				ArraySegment<byte> s =packet.Write();

				if (s != null)
					Send(s);
			}
		}



		public override void OnDisconnected(EndPoint endPoint)
		{
			Console.WriteLine($"OnDisconnected : {endPoint}");
		}

		public override int OnRecv(ArraySegment<byte> buffer)
		{
			string recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
			Console.WriteLine($"[From Server] {recvData}");
			return buffer.Count;
		}

		public override void OnSend(int numOfBytes)
		{
			Console.WriteLine($"Transferred bytes: {numOfBytes}");
		}

	}
}