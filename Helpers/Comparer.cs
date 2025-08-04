using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace CopyCat.Helpers
{
    internal class Comparer
    {
        #region Files Comparing
        public bool CompareFiles(string originFile, string replicaFile)
        {

            FileInfo originFileInfo = new FileInfo(originFile);
            FileInfo replicaFileInfo = new FileInfo(replicaFile);

            // Compare files' sizes
            if (originFileInfo.Length != replicaFileInfo.Length)
            {
                return false;
            }

            //Compare files' last use
            if (originFileInfo.LastWriteTime != replicaFileInfo.LastWriteTime)
            {
                return false;
            }

            if(!CompareSHA256(originFile, replicaFile))
            {
                return false;
            }

            return true;
        }

        public bool SameFilesExist(string originFile, string replicaFile)
        {
            return FileManager.FileExist(originFile) && FileManager.FileExist(replicaFile) ? true : false;
        }

        bool CompareSHA256(string originFile, string replicaFile)
        {
            byte[]? originHash = GetSHA256(originFile);
            byte[]? replicaHash = GetSHA256(replicaFile);
            if (originHash == null || replicaHash == null)
            {
                return false;
            }
            return originHash.SequenceEqual(replicaHash);
        }

        byte[]? GetSHA256(string FilePath)
        {
            try
            {
                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] originHash;
                    using (var originStream = File.OpenRead(FilePath))
                    {
                        originHash = sha256.ComputeHash(originStream);
                        return originHash;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error calculating SHA256 for {FilePath}: {ex.Message}");
                return null;
            }
        }
        #endregion
    }


}
