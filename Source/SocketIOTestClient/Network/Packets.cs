using MessagePack;

namespace SocketIOTestClient.Network
{
    /// <summary>
    /// request packet
    /// </summary>
    [MessagePackObject]
    public class RequestPacket
    {
        [Key("nickname")]
        public string Nickname { get; set; }
        [Key("message")]
        public string Message { get; set; }
    }


    /// <summary>
    /// response packet
    /// </summary>
    [MessagePackObject]
    public class ResponsePacket
    {
        [Key("nickname")]
        public string Nickname { get; set; }
        [Key("message")]
        public string Message { get; set; }
    }
}