using RLog;
using System;

namespace SocketIOTestClient
{
    class Program
    {
        public static string server_version = "2.2.3";


        static void Main(string[] args)
        {
            RLogger.Init();

            RLogger.Debug("start socket io test client");

            try
            {
                PacketTestClient.Run();
            }
            catch (Exception e)
            {
                RLogger.Error(e.Message.ToString());
                RLogger.Error(e.StackTrace.ToString());
            }
        }

        
    }
}
