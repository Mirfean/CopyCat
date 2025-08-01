using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CopyCat.Synchro
{
    internal class Synchronizer
    {
        private readonly String _OriginPath;
        private readonly String _ReplicaPath;
        private Comparer comparer;
        private Logger logger;

        public Synchronizer(String originPath, String replicaPath, String logPath)
        {
            _OriginPath = originPath;
            _ReplicaPath = replicaPath;
            comparer = new Comparer();
            logger = new Logger(logPath);
        }

        // Main synchronization method
        public void Synchronize()
        {
            Console.WriteLine($"Synchronizing from {_OriginPath} to {_ReplicaPath}");
            
            var OriginFiles = Directory.GetFiles(_OriginPath, "*", SearchOption.AllDirectories);

            string relativePath;
            string fileName;

            foreach (var File in OriginFiles)
            {
                relativePath = Path.GetRelativePath(_OriginPath, File);
                fileName = Path.GetFileName(File);

                if (!comparer.Compare(Path.Combine(_OriginPath, relativePath, fileName), 
                    Path.Combine(_ReplicaPath, relativePath, fileName)))
                {
                    CopySingleFile(relativePath, fileName);
                    //LOG
                }
            }

            // TODO
            // 1. Copy files from origin to replica
            // 2. Substract replica files and DIRECTORIES from origin and delete remaining
            // 3. Repeat every X seconds
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="relativePath"> Relative path from origin directory </param>
        /// <param name="fileName"> File name with extension </param>
        public void CopySingleFile(string relativePath, string fileName)
        {
            String FilePath = Path.Combine(_OriginPath, relativePath);
            String DestinationFilePath = Path.Combine(_ReplicaPath, relativePath);

            //DEBUG
            Console.WriteLine($"Copying file from {FilePath} to {DestinationFilePath}");

            FileManager.CheckAndCreateDirectory(DestinationFilePath);
            
            File.Copy(Path.Combine(_OriginPath, relativePath, fileName), 
                Path.Combine(_ReplicaPath, relativePath, fileName), true);
        }
    }
}
