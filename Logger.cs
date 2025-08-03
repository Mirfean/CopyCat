using CopyCat.Synchro;
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
        
        static readonly object _lock = new object();

        public Logger(String logPath)
        {

            //If creating new log file
            _LogPath = logPath;
            if (!File.Exists(_LogPath))
            {
                LogFileName = $"CopyCatLogs-{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.log";
                if(!Directory.Exists(_LogPath))
                {
                    FileManager.CreateDirectory(_LogPath);
                }
                FileManager.CreateFile(_LogPath, LogFileName);
            }
            else
            {
                LogFileName = Path.GetFileName(_LogPath);
                _LogPath = Path.GetDirectoryName(_LogPath);
            }
        }

        public void Log(InteractionType interactionType, string message)
        {
            try
            {
                lock (_lock)
                {
                    using (var writer = new StreamWriter(Path.Combine(_LogPath, LogFileName), true))
                    {
                        writer.WriteLine($"{DateTime.Now}: {message}");
                    }
                }

                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing to log: {ex.Message}");
            }
        }

    }
}
