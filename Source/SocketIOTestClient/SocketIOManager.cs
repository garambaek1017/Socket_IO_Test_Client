using RLog;
using SocketIOTestClient.Network;
using System;

namespace SocketIOTestClient
{
    public class SocketIOManager
    {
        static SocketIOManager m_Instance = new SocketIOManager();

        RamSocket RamSocket = null;

        public static SocketIOManager Instance
        {
            get { return m_Instance; }
        }


        public async void StartSocketClient(string server, int port)
        {
            Uri serverUri = new Uri($"ws://{server}:{port}/socket.io/?EIO=4&transport=websocket");

            RamSocket = new RamSocket(serverUri);

            RamSocket.OnConnected += () =>
            {
                RamSocket.SocketState = RamSocketIOState.Connected;
                RLogger.Debug($"Connection is success : {RamSocket.SocketState}, {RamSocket.ClientWebSocket.State}");
            };

            await RamSocket.TryConnect();

            RamSocket.On("ResponseTestMsg");
        }

        public async void SendTestMessage()
        {
            var packet = new RequestPacket()
            {
                Nickname = "Garamy",
                Message = "Hello My name is Garamy"
            };

            await RamSocket.Emit("RequestTestMsg", PacketHelper.ToByteString(packet));
        }
    }
}
