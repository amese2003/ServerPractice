using System;
using System.Collections.Generic;
using System.Text;

namespace PacketGenerator
{
    class PacketFormat
    {

		// {0} 패킷 이름/번호
		// {1} 패킷 목록
		public static string fileFormat =
@"
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using ServerCore;

public enum PacketID
{{
    {0}
}}

{1}
";
		// {0} 패킷 이름
		// {1} 패킷 번호
		public static string packetEnumFormat =
@"{0} = {1},";




		// {0} 패킷 이름
		// {1} 멤버 변수들
		// {2} 멤버 변수 read
		// {3} 멤버 변수 write

        public static string packetFormat =
@"
class {0}
{{
	{1}	


    public void Read(ArraySegment<byte> segment)
    {{
		ushort pos = 0;

		ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
		pos += sizeof(ushort);
		pos += sizeof(ushort);
		{2}
	}}

	public ArraySegment<byte> Write()
    {{
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort pos = 0;
		bool success = true;

		Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

		pos += sizeof(ushort);
		success &= BitConverter.TryWriteBytes(s.Slice(pos, s.Length - pos), (ushort)PacketID.{0});
		pos += sizeof(ushort);
		{3}
		success &= BitConverter.TryWriteBytes(s, pos);

		if (success == false)
			return null;

		return SendBufferHelper.Close(pos);
	}}
}}
";

		// {0} 변수 형식
		// {1} 변수 이름
		public static string memberFormat =
@"public {0} {1};";


		// {0} 리스트 이름 [대문자]
		// {1} 리스트 이름 [소문자]
		// {2} 멤버 변수들
		// {3} 멤버 변수 read
		// {4} 멤버 변수 write

		public static string memberListFormat =
@"
public struct {0}
{{
	{2}

	public void Read(ReadOnlySpan<byte> s, ref ushort pos)
    {{
		{3}
	}}

	public bool Wrtie(Span<byte> s, ref ushort pos)
    {{
		bool success = true;
		{4}

		return success;
    }}
}}

public List<{0}> {1}s = new List<{0}>();
";


		// {0} 변수 이름
		// {1} To~ 변수 형식
		// {2} 변수 형식
		public static string readFormat =
@"this.{0} = BitConverter.{1}(s.Slice(pos, s.Length - pos));
pos += sizeof({2});
";

		// {0] 변수 이름
		// {1} 변수 형식
		public static string readByteFormat =
@"this.{0} = ({1})segment.Array[segment.Offset + pos];
pos += sizeof({1});
";

		// {0} 변수 이름
		public static string readStringFormat =
@"ushort {0}Leng = BitConverter.ToUInt16(s.Slice(pos, s.Length - pos));
pos += sizeof(ushort);
this.{0} = Encoding.Unicode.GetString(s.Slice(pos, {0}Leng));
pos += {0}Leng;
";

		// {0} 리스트 이름 [대문자]
		// {1} 리스트 이름 [소문자]
		public static string readListFormat =
@"this.{1}s.Clear();
ushort {1}Leng = BitConverter.ToUInt16(s.Slice(pos, s.Length - pos));
pos += sizeof(ushort);			
for (int i = 0; i < {1}Leng; i++)
{{
	{0} {1} = new {0}();
	{1}.Read(s, ref pos);
	{1}s.Add({1});
}}
";

		// {0} 변수 이름
		// {1} 변수 형식
		public static string writeFormat =
@"success &= BitConverter.TryWriteBytes(s.Slice(pos, s.Length - pos), this.{0});
pos += sizeof({1});
";

		public static string writeByteFormat =
@"segment.Array[segment.Offset + pos] = (byte)this.{0};
pos += sizeof({1});
";

		// {0} 변수이름
		public static string writeStringFormat =
@"ushort {0}Leng = (ushort)Encoding.Unicode.GetBytes(this.{0}, 0, this.{0}.Length, segment.Array, segment.Offset + pos + sizeof(ushort));
success &= BitConverter.TryWriteBytes(s.Slice(pos, s.Length - pos), {0}Leng);
pos += sizeof(ushort);
pos += {0}Leng;
";
		// {0} 리스트 이름 [대문자]
		// {1} 리스트 이름 [소문자]
		public static string writeListFormat =
@"success &= BitConverter.TryWriteBytes(s.Slice(pos, s.Length - pos), (ushort)this.{1}s.Count);
pos += sizeof(ushort);

foreach({0} {1} in this.{1}s)
	success &= {1}.Wrtie(s, ref pos);
";



	}
}
