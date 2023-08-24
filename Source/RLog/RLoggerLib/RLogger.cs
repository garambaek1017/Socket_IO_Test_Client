using log4net;
using System;
using System.IO;

namespace RLog
{
    public class RLogger
    {
        /// <summary>
        /// 로그용 설정 파일 초기화 
        /// </summary>
        public static void Init()
        {
            string logFilePath = AppDomain.CurrentDomain.BaseDirectory + @"Log.config";
            FileInfo finfo = new FileInfo(logFilePath);
            log4net.Config.XmlConfigurator.ConfigureAndWatch(finfo);
        }

        public static void Debug(string msg, bool isWriteConsole = true)
        {
            var log = LogManager.GetLogger(LogName.Debug);
            log.Debug(msg);
        }

        public static void Error(string msg, bool isWriteConsole = true)
        {
            var log = LogManager.GetLogger(LogName.Error);
            log.Debug(msg);

            if (isWriteConsole == true)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($">> {msg}");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        public static void Info(string msg, bool isWriteConsole = true)
        {
            var log = LogManager.GetLogger(LogName.Info);
            log.Debug(msg);

            if (isWriteConsole == true)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($">> {msg}");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
    }
}
