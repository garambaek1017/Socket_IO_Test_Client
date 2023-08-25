using RLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketIOTestClient
{
    public class PacketTestClient
    {
        public static void Run()
        {
            ShowCommand();

            while (true)
            {
                var cmd = Console.ReadLine();

                switch (cmd)
                {
                    case "connect":
                        var serverIP = "localhost";
                        var serverPort = 4001;
                        SocketIOManager.Instance.StartSocketClient(serverIP, serverPort);
                        break;
                    case "send":
                        SocketIOManager.Instance.SendTestMessage();
                        break;

                    case "disconnect":
                        SocketIOManager.Instance.Disconnect();
                        break;

                    default:
                        Console.Clear();
                        ShowCommand();
                        break;
                    
                }
            }
        }
        static void ShowCommand()
        {
            RLogger.Debug("Enter your command..");
            RLogger.Debug("1. connect - connect to server");
            RLogger.Debug("2. send - send a message to server");
            RLogger.Debug("3. disconnect");
            RLogger.Debug("4. Show command");
        }

        public static void DisplayReceiveMessage(string receivedMessage)
        {
            RLogger.Info(receivedMessage, true);
        }
    }
}
