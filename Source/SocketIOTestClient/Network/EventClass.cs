using RLog;
using System;

namespace SocketIOTestClient.Network
{
    /// <summary>
    /// handing event
    /// </summary>
    /// <param name="msg"></param>
    public delegate void EventHandler(string msg);

    public class EventPublisher
    {
        public event EventHandler ReceiveEvent;

        public void RaiseEvent(string message)
        {
            ReceiveEvent?.Invoke(message);
        }
    }

    public class EventSubscriber
    {
        public void HandleEvent(string message)
        {
            // 클라로 부터 받은 메시지를 디코드 
            byte[] decoded = Convert.FromBase64String(message);

            // 리스펀스 패킷으로 변환 후 화면 표기 
            var resultPacket = PacketHelper.ToPacket<ResponsePacket>(decoded);

            PacketTestClient.DisplayReceiveMessage(resultPacket.Message);
        }
    }
}
