using RLog;
using Socket_IO_LIb.Network;
using System;

namespace SocketIOTestClient
{
    public class SocketIOClientManager
    {
        static SocketIOClientManager m_Instance = new SocketIOClientManager();

        RamSocket RamSocket = null;

        public static SocketIOClientManager Instance
        {
            get { return m_Instance; }
        }

        #region private 
        private void OnConnect()
        {
            RamSocket.SocketState = RamSocketIOState.Connected;
            RLogger.Debug($"Connection is success : {RamSocket.SocketState}, {RamSocket.ClientWebSocket.State}");
        }
        private void OnError()
        {
            throw new NotImplementedException();
        }
        private void OnClosed()
        {

        }
        #endregion

        #region public 
        public void ChainingEvent()
        {
            if (RamSocket != null)
            {
                RamSocket.OnConnected += OnConnect;

                RamSocket.OnError += OnError;

                RamSocket.OnClosed += OnClosed;
            }
        }
        // 소켓 생성
        public async void CreateSocket(string server, int port, int maxPacketBuffer)
        {
            Uri serverUri = new Uri($"ws://{server}:{port}/socket.io/?EIO=4&transport=websocket");

            RamSocket = new RamSocket(serverUri, maxPacketBuffer);

        }
        public async void TryConnect()
        {
            if (RamSocket != null)
            {
                await RamSocket.TryConnect();
            }
        }
        public async void TryDisconnect()
        {
            if (RamSocket != null && RamSocket.SocketState == RamSocketIOState.Connected)
                await RamSocket.TryDisconnect();
            else
                RLogger.Debug($"Try Connect first");
        }
        public void On(string eventName, DResponseProcessor responseProcessor)
        {
            if (RamSocket != null)
            {
                RamSocket.On(eventName, responseProcessor);
            }
        }
        public async void SendTestMessage<T>(string eventName, RequestPacket packet)
        {
            await RamSocket.Emit(eventName, PacketHelper.ToByteString(packet));
        }
        #endregion
    }
}
