using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CopyCat.Synchro
{
    public enum InteractionType
    {
        COPY,
        UPDATE,
        DELETE
    }

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

            SynchronizeFiles();

            SynchronizeDir();
        }

        private void SynchronizeFiles()
        {
            var OriginFiles = Directory.GetFiles(_OriginPath, "*", SearchOption.AllDirectories);

            string? relativePath;
            string fileName;

            foreach (var File in OriginFiles)
            {
                fileName = Path.GetFileName(File);
                relativePath = Path.GetDirectoryName(Path.GetRelativePath(_OriginPath, File));

                if (relativePath == null)
                {
                    relativePath = string.Empty; // Handle case where the file is in the root directory
                }

                if (!comparer.CompareFiles(Path.Combine(_OriginPath, relativePath, fileName),
                    Path.Combine(_ReplicaPath, relativePath, fileName)))
                {
                    CopySingleFile(relativePath, fileName);
                    //LOG
                }
            }

            // Removing excessive files from replica
            string[] ReplicaFiles = Directory.GetFiles(_ReplicaPath, "*", SearchOption.AllDirectories);

            List<String> ExcessiveFiles = ReplicaFiles.Except(OriginFiles).ToList();

            foreach (var ExcessiveFile in ExcessiveFiles)
            {
                relativePath = Path.GetRelativePath(_ReplicaPath, ExcessiveFile);
                fileName = Path.GetFileName(ExcessiveFile);
                FileManager.DeleteFile(Path.Combine(_ReplicaPath, relativePath, fileName));
            }
        }

        private void SynchronizeDir()
        {
            string[] OriginDirectories = Directory.GetDirectories(_OriginPath, "*", SearchOption.AllDirectories);
            string[] ReplicaDirectories = Directory.GetDirectories(_ReplicaPath, "*", SearchOption.AllDirectories);

            OriginDirectories = getRelativePath(_OriginPath, OriginDirectories);
            ReplicaDirectories = getRelativePath(_ReplicaPath, ReplicaDirectories);

            List<String> ExcessiveDirectories = ReplicaDirectories.Except(OriginDirectories).ToList();
            List<String> MissingDirectories = OriginDirectories.Except(ReplicaDirectories).ToList();

            try
            {
                foreach (var excessiveDir in ExcessiveDirectories)
                {
                    //DEBUG
                    Console.WriteLine($"Deleting excessive directory: {excessiveDir}");
                    FileManager.DeleteDirectory(Path.Combine(_ReplicaPath, excessiveDir));
                }

                foreach (var missingDir in MissingDirectories)
                {
                    //DEBUG
                    Console.WriteLine($"Creating missing directory: {missingDir}");
                    FileManager.CreateDirectory(Path.Combine(_ReplicaPath, missingDir));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error synchronizing directories: {ex.Message}");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="relativePath"> Relative path from origin directory </param>
        /// <param name="fileName"> File name with extension </param>
        public bool CopySingleFile(string relativePath, string fileName)
        {
            String FilePath = Path.Combine(_OriginPath, relativePath);
            String DestinationFilePath = Path.Combine(_ReplicaPath, relativePath);

            //DEBUG
            Console.WriteLine($"Copying file from {FilePath} to {DestinationFilePath}");

            try
            {
                FileManager.CheckAndCreateDirectory(DestinationFilePath);

                File.Copy(Path.Combine(FilePath, fileName), 
                    Path.Combine(DestinationFilePath, fileName), true);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error copying file {fileName} from {FilePath} to {DestinationFilePath}: {ex.Message}");
                //LOG the error
                return false;
            }
        }

        string[] getRelativePath(string fullPath, string[] paths)
        {
            string[] relativePaths = new string[paths.Length];
            for (int i = 0; i < paths.Length; i++)
            {
                relativePaths[i] = Path.GetRelativePath(fullPath, paths[i]);
                Console.WriteLine($"Relative path: {relativePaths[i]}");
            }
            return relativePaths;
        }
    }
}
