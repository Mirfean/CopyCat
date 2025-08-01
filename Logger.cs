using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CopyCat
{
    internal class Logger
    {
        readonly String _LogPath;
        String LogFileName;

        public Logger(String logPath)
        {

            //If creating new log file
            _LogPath = logPath;
            LogFileName = $"CopyCatLogs-{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.log";
        }

        public void Log(string message)
        {
            try
            {
                using (var writer = new StreamWriter(_LogPath, true))
                {
                    writer.WriteLine($"{DateTime.Now}: {message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing to log: {ex.Message}");
                //ADD 
            }
        }

    }
}
