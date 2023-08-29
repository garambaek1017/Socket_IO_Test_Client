using RLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Socket_IO_LIb.Network
{
    public enum RamSocketIOState
    {
        None = 0,
        Connected = 1,
        Disconnected = 2,
    }

    // packet 처리용 델리게이트 
    public delegate void DResponseProcessor(string responseData);

    public class RamSocket
    {
        #region 프로퍼티 
        private int MaxPacketBuffer = 0;                                    // 패킷이 처리할 최대 버퍼 
        private Uri ServerUri = null;                                       // 접속시 사용할 서버 URI
        public RamSocketIOState SocketState = RamSocketIOState.None;        // 소켓 상태 
        public event Action OnConnected;                                    // 커넥션 이벤트 발생
        public event Action OnError;                                        // 에러 이벤트 발생
        public event Action OnClosed;                                       // 닫기 이벤트 발생
        public ClientWebSocket ClientWebSocket { get; set; } = new ClientWebSocket();   // .net에서 사용하는 웹소캣 클래스 

        // event Handlers
        public Dictionary<string, EventPublisher> EventHandlers { get; set; } = new Dictionary<string, EventPublisher>();
        public Dictionary<string, DResponseProcessor> ResponseHandlers { get; set; } = new Dictionary<string, DResponseProcessor>();
        #endregion

        public RamSocket()
        {

        }

        public RamSocket(Uri uri, int maxPacketBuffer)
        {
            SocketState = RamSocketIOState.None;
            ServerUri = uri;

            MaxPacketBuffer = maxPacketBuffer;
        }

        #region private 
        /// <summary>
        /// 수신한 패킷 처리기 
        /// </summary>
        async void Dispatcher()
        {

            byte[] receiveBuffer = new byte[MaxPacketBuffer];

            while (ClientWebSocket.State == WebSocketState.Open)
            {
                WebSocketReceiveResult result = await ClientWebSocket.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    string responseMessage = Encoding.UTF8.GetString(receiveBuffer, 0, result.Count);

                    // 40
                    if (result.Count == 2)
                    {
                        continue;
                    }
                    else
                    {
                        // 0
                        if (responseMessage[0] == '0')
                        {
                            var newResponseString = responseMessage.Substring(1, responseMessage.Count() - 1);

                            // Invoke Connection Event
                            this.OnConnected.Invoke();
                        }
                        // 42
                        else if (responseMessage[0] == '4' && responseMessage[1] == '2')
                        {
                            var newResponseString = responseMessage.Substring(2, responseMessage.Count() - 2);

                            newResponseString = newResponseString.Replace("]", string.Empty);
                            newResponseString = newResponseString.Replace("[", string.Empty);

                            //first eventName, second data
                            var responseArray = newResponseString.Split(',');

                            var (eventName, data) = Filter(responseArray);

                            //if (EventHandlers.TryGetValue(eventName, out var eventHandler) == true)
                            //{
                            //    eventHandler.RaiseEvent(data);
                            //}
                            //else
                            //{
                            //    RLogger.Debug($"Not found event : {eventName}"); 
                            //}

                            if (ResponseHandlers.TryGetValue(eventName, out var responseHandler) == true)
                            {
                                responseHandler.Invoke(data);
                            }
                            else
                            {
                                RLogger.Debug($"Not found event : {eventName}");
                            }
                        }
                    }
                }
            }
        }

        // 받은 패킷 정리 
        (string eventName, string data) Filter(string[] response)
        {
            // \ remove
            string modifiedString = response[0].Remove(0, 1);
            modifiedString = modifiedString.Remove(modifiedString.Count() - 1, 1);

            // \ remove
            string modifiedString2 = response[1].Remove(0, 1);
            modifiedString2 = modifiedString2.Remove(modifiedString2.Count() - 1, 1);
            return (modifiedString, modifiedString2);
        }
        #endregion

        #region public 
        /// <summary>
        /// Send Message to Server
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task Emit(string eventName, string data)
        {
            if (SocketState == RamSocketIOState.Connected)
            {
                string message = $"42[\"{eventName}\", \"{data}\"]";
                byte[] buffer = Encoding.UTF8.GetBytes(message);
                await ClientWebSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

        /// <summary>
        /// 커넥션 
        /// </summary>
        /// <returns></returns>
        public async Task TryConnect()
        {
            if (SocketState == RamSocketIOState.None)
            {
                await ClientWebSocket.ConnectAsync(ServerUri, CancellationToken.None);

                RLogger.Debug($"Connect to uri {ServerUri.ToString()} is Done!");

                Dispatcher(); // start receive 
            }
        }

        /// <summary>
        /// 커넥션 종료 
        /// </summary>
        /// <returns></returns>
        public async Task TryDisconnect()
        {
            if (SocketState == RamSocketIOState.Connected)
            {
                // todo : 이 토큰 클래스가 뭔지 확인 
                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken token = source.Token;

                await ClientWebSocket.CloseAsync(WebSocketCloseStatus.Empty, "", token);

                RLogger.Debug($"disconnect to uri {ServerUri.ToString()} is Done!");
            }
        }

        /// <summary>
        /// 리스브 이벤트 처리기 추가 
        /// </summary>
        /// <param name="eventName"></param>
        public void On(string eventName)
        {
            EventPublisher publisher = new EventPublisher();
            EventSubscriber subscriber = new EventSubscriber();

            publisher.ReceiveEvent += subscriber.HandleEvent;

            EventHandlers.Add(eventName, publisher);
        }

        /// <summary>
        /// 리시브 처리용 델리게이트 등록 
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="responseHandler"></param>
        public void On(string eventName, DResponseProcessor responseHandler)
        {
            ResponseHandlers.Add(eventName, responseHandler);
        }
        #endregion
    }
}
