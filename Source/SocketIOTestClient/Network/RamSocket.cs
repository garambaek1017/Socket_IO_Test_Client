using RLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SocketIOTestClient.Network
{
    public enum RamSocketIOState
    {
        None = 0,
        Connected = 1,
        Disconnected = 2,
    }

    /// <summary>
    /// For node.js - socket io 
    /// using .net Client webSocket.
    /// </summary>
    public class RamSocket
    {
        private Uri ServerUri = null;

        public RamSocketIOState SocketState = RamSocketIOState.None;
        // .net client websocekt
        public ClientWebSocket ClientWebSocket = new ClientWebSocket();

        public event Action OnConnected;        // onConnection event
        public event Action OnError;            // onErrorEvent, to be implemented later
        public event Action OnClosed;           // OnClosed, to be implemented later

        // event Handlers
        public Dictionary<string, EventPublisher> EventHandlers { get; set; } = new Dictionary<string, EventPublisher>();

        public RamSocket()
        {

        }

        public RamSocket(Uri uri)
        {
            SocketState = RamSocketIOState.None;
            ServerUri = uri;
        }

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
        /// Connection to Server
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
        /// Run Callback Function When I Get the Receive Event
        /// </summary>
        /// <param name="resEventName"></param>
        public void On(string resEventName)
        {
            EventPublisher publisher = new EventPublisher();
            EventSubscriber subscriber = new EventSubscriber();

            publisher.ReceiveEvent += subscriber.HandleEvent;

            EventHandlers.Add(resEventName, publisher);
        }

        /// <summary>
        /// Packet Dispatcher
        /// </summary>
        async void Dispatcher()
        {
            byte[] receiveBuffer = new byte[1024];

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
                            if (EventHandlers.TryGetValue(eventName, out var eventHandler) == true)
                            {
                                eventHandler.RaiseEvent(data);
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

    }
}
