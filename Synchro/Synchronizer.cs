using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CopyCat.Synchro
{
    internal class Synchronizer
    {
        private readonly String _OriginPath;
        private readonly String _ReplicaPath;

        public Synchronizer(String originPath, String replicaPath)
        {
            _OriginPath = originPath;
            _ReplicaPath = replicaPath;
        }

        // Main synchronization method
        public void Synchronize()
        {
            Console.WriteLine($"Synchronizing from {_OriginPath} to {_ReplicaPath}");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="originFilePath"> Relative path from origin directory </param>
        /// <param name="fileName"> File name with extension </param>
        public void CopySingleFile(string originFilePath, string fileName)
        {
            String FilePath = Path.Combine(_OriginPath, originFilePath);
            String DestinationFilePath = Path.Combine(_ReplicaPath, originFilePath);

            //DEBUG
            Console.WriteLine($"Copying file from {FilePath} to {DestinationFilePath}");

            FileManager.CheckAndCreateDirectory(DestinationFilePath);
            
            File.Copy(Path.Combine(FilePath, fileName), Path.Combine(DestinationFilePath, fileName), true);
        }
    }
}
