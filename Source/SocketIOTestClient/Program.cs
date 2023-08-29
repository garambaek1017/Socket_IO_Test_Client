using RLog;
using System;

namespace SocketIOTestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            RLogger.Init();

            RLogger.Debug(" === Start Socket.IO Chatting Client === ");

            try
            {
                var simpleChat = new SimpleChat();

                simpleChat.Run();
            }
            catch (Exception e)
            {
                RLogger.Error(e.Message.ToString());
                RLogger.Error(e.StackTrace.ToString());
            }
        }

    }
}
