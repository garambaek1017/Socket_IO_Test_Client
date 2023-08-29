using MessagePack;
using System;

namespace SocketIOTestClient.Network
{
    public class PacketHelper
    {
        // packet을 byte로 바꿔줌 
        public static string ToByteString<T>(T packet)
        {
            byte[] bytes = MessagePackSerializer.Serialize(packet);
            return Convert.ToBase64String(bytes);
        }

        // byte를 패킷으로 변경 
        public static T ToPacket<T>(byte[] bytes)
        {
            var responsePacket = MessagePackSerializer.Deserialize<T>(bytes);

            return responsePacket;
        }
    }
}
