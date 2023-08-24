using MessagePack;

namespace SocketIOTestClient.Network
{
    /// <summary>
    /// request packet
    /// </summary>
    [MessagePackObject]
    public class RequestPacket
    {
        [Key("Nickname")]
        public string Nickname { get; set; }
        [Key("Message")]
        public string Message { get; set; }
    }


    /// <summary>
    /// response packet
    /// </summary>
    [MessagePackObject]
    public class ResponsePacket
    {
        [Key("ErrorCode")]
        public int ErrorCode { get; set; }
        [Key("Message")]
        public string Message { get; set; }
    }
}