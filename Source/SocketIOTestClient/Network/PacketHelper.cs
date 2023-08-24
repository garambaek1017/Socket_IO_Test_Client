using MessagePack;
using System;

namespace SocketIOTestClient.Network
{
    public class PacketHelper
    {
        // 
        public static string ToByteString(RequestPacket packet)
        {
            byte[] bytes = MessagePackSerializer.Serialize(packet);
            return Convert.ToBase64String(bytes);
        }

        public static T ToPacket<T>(byte[] bytes)
        {
            var responsePacket = MessagePackSerializer.Deserialize<T>(bytes);

            return responsePacket;
        }
    }
}
