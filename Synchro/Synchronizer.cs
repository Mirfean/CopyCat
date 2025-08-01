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
        private Comparer comparer;

        public Synchronizer(String originPath, String replicaPath)
        {
            _OriginPath = originPath;
            _ReplicaPath = replicaPath;
            comparer = new Comparer();
        }

        // Main synchronization method
        public void Synchronize()
        {
            Console.WriteLine($"Synchronizing from {_OriginPath} to {_ReplicaPath}");
            
            var Files = System.IO.Directory.GetFiles(_OriginPath, "*", System.IO.SearchOption.AllDirectories);

            // TODO
            // 1. Check every file and copy it if not exists or different
            // 2. Substract replica files from origin and delete remaining
            // 3. Repeat every X seconds
        }

        //TO CHANGE
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
