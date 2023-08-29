using RLog;
using SocketIOClient.Messages;
using SocketIOTestClient.Network;
using System;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace SocketIOTestClient
{
    public enum SimpleChatState
    {
        None = 0,
        Connect = 1,
        Login,
        Chatting,
    }

    public class SimpleChat
    {
        private SimpleChatState ChatState { get; set; }

        private ChatUser ChatUser = null;

        public SimpleChat()
        {
            ChatState = SimpleChatState.None;
        }

        // 프로그램 실행
        public void Run()
        {
            ShowCommand();

            while (true)
            {
                if (ChatState == SimpleChatState.None)
                {
                    var cmd = Console.ReadLine().ToLower();

                    RLogger.Debug(" >>> chat program state : " + ChatState);

                    switch (cmd)
                    {
                        case "1":
                            CreateUser();
                            ShowCommand();
                            break;
                        case "2":
                            Connect();
                            ShowCommand();
                            break;
                        case "3":
                            ChatState = SimpleChatState.Chatting;
                            break;
                        case "4":
                            Disconnect();
                            break;
                        default:
                            ShowCommand();
                            break;

                    }
                }
                else if (ChatState == SimpleChatState.Chatting)
                {
                    SendMessage();
                }
            }
        }

        // 커넥트 
        void Connect()
        {
            var serverIP = "localhost";
            var serverPort = 4001;
            var maxPacketBufferSize = 1024 * 128;

            SocketIOClientManager.Instance.CreateSocket(serverIP, serverPort, maxPacketBufferSize);
            SocketIOClientManager.Instance.TryConnect();

            // 이벤트 설정 
            SocketIOClientManager.Instance.On("ReceiveMessage", DisplayReceiveMessage);
        }

        // 연결 끊기 
        void Disconnect()
        {
            SocketIOClientManager.Instance.TryDisconnect();
        }

        // 명령 보여주기 
        void ShowCommand()
        {
            Thread.Sleep(2000);

            Console.Clear();

            Console.WriteLine();
            RLogger.Debug(" === Enter your command === ");
            RLogger.Debug("1. create chat user");
            RLogger.Debug("2. connect - connect to server");
            RLogger.Debug("3. send - send a message to server");
            RLogger.Debug("4. disconnect");
            RLogger.Debug("5. Show command");
            Console.WriteLine();
        }

        // 유저 생성 
        void CreateUser()
        {
            Console.Write(" >>> Enter your nickname : ");

            var nickname = Console.ReadLine().ToLower();

            ChatUser = new ChatUser
            {
                Nickname = nickname
            };

            RLogger.Debug(" >>> SET Client nickname setting : " + ChatUser.Nickname);
        }

        // 메시지 보내기 
        void SendMessage()
        {
            Console.Write("input message >>> ");

            var message = Console.ReadLine();

            if (message == "5")
            {
                ChatState = SimpleChatState.None;

                Console.Clear();

                ShowCommand();
            }
            else
            {
                var packet = new RequestPacket()
                {
                    Nickname = ChatUser.Nickname,
                    Message = message,
                };

                SocketIOClientManager.Instance.SendTestMessage<RequestPacket>("SendMessage", packet);
            }
        }
        // 받은 메시지 화면 표시 
        void DisplayReceiveMessage(string receivedMessage)
        {
            byte[] decoded = Convert.FromBase64String(receivedMessage);
            var resultPacket = PacketHelper.ToPacket<ResponsePacket>(decoded);
            RLogger.Info($"{resultPacket.Nickname} : {resultPacket.Message}", true);
        }
    }
}
