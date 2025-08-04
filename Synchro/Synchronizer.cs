using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CopyCat.Helpers;

namespace CopyCat.Synchro
{
    public enum InteractionType
    {
        COPY,
        UPDATE,
        DELETE,
        ERROR
    }

    internal class Synchronizer
    {
        private readonly String _OriginPath;
        private readonly String _ReplicaPath;
        private readonly int _SyncInterval;
        private Comparer comparer;
        private Logger logger;

        //Locker for file operations
        private static readonly object _syncLock = new object();

        public Synchronizer(String originPath, String replicaPath, String syncInterval, String logPath)
        {
            _OriginPath = originPath;
            _ReplicaPath = replicaPath;
            logger = new Logger(logPath);
            comparer = new Comparer();

            //TODO: Add cool parsing for syncInterval (100ms, 13[s], 2m, 3h)
            _SyncInterval = int.Parse(syncInterval) * 1000;
        }

        public void SynchronizeLoop()
        {
            using (Timer timer = new Timer(Synchronize, null, 0, _SyncInterval))
            {
                Console.ReadKey();
            }
            Console.WriteLine("Synchronization ended.");
        }

        /// <summary>
        /// Main synchronization method triggered by the timer
        /// </summary>
        private void Synchronize(object? state)
        {
            if (!CheckDirAndHandle(_OriginPath) || !CheckDirAndHandle(_OriginPath))
            {
                //End application if directories are not valid
                logger.AddMessage(InteractionType.ERROR, "Synchronization aborted due to invalid directories.");
                logger.Log("Synchronization canceled due to invalid directories.");
                Environment.Exit(1);
            }

            lock (_syncLock)
            {
                try
                {
                    SynchronizeFiles();
                }
                catch (Exception ex)
                {
                    logger.AddMessage(InteractionType.ERROR, $"Error during file synchronization: {ex.Message}");
                }
            }

            lock (_syncLock)
            {
                try
                {
                    SynchronizeDir();
                }
                catch (Exception ex)
                {
                    logger.AddMessage(InteractionType.ERROR, $"Error during directory synchronization: {ex.Message}");
                }
            }


        }

        private bool CheckDirAndHandle(string path)
        {
            if (!FileManager.DirectoryExist(path))
            {
                Console.WriteLine($"Sync path '{path}' does not exist anymore.");
                if (Program.UserChoice("Do you want to create it?"))
                {
                    FileManager.CreateDirectory(path);
                    logger.AddMessage(InteractionType.COPY, $"Created directory: {path}");
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        private void SynchronizeFiles()
        {
            var OriginFiles = Directory.GetFiles(_OriginPath, "*", SearchOption.AllDirectories);

            UpdateReplicaFiles(OriginFiles);

            RemoveExcessiveFiles(OriginFiles);
        }

        private void UpdateReplicaFiles(string[] OriginFiles)
        {
            string? relativePath;
            string fileName;

            string originFile;
            string replicaFile;

            foreach (var File in OriginFiles)
            {
                fileName = Path.GetFileName(File);
                relativePath = Path.GetDirectoryName(Path.GetRelativePath(_OriginPath, File));

                if (relativePath == null)
                {
                    relativePath = string.Empty; // Handle case where the file is in the root directory
                }

                originFile = Path.Combine(_OriginPath, relativePath, fileName);
                replicaFile = Path.Combine(_ReplicaPath, relativePath, fileName);

                if (comparer.SameFilesExist(originFile, replicaFile))
                {
                    if (!comparer.CompareFiles(Path.Combine(_OriginPath, relativePath, fileName),
                    Path.Combine(_ReplicaPath, relativePath, fileName)))
                    {
                        CopyUpdateFile(relativePath, fileName, InteractionType.UPDATE);
                    }
                }
                else 
                {
                    CopyUpdateFile(relativePath, fileName);
                }
            }
            logger.Log("Copying files");
        }

        private void RemoveExcessiveFiles(string[] OriginFiles)
        {
            string? relativePath;
            string fileName;

            string[] TempOriginFiles = new string[OriginFiles.Length];
            string[] ReplicaFiles = Directory.GetFiles(_ReplicaPath, "*", SearchOption.AllDirectories);
            

            for (int i = 0; i < TempOriginFiles.Length; i++)
            {
                TempOriginFiles[i] = Path.GetRelativePath(_OriginPath, OriginFiles[i]);
            }

            for (int i = 0; i < ReplicaFiles.Length; i++)
            {
                ReplicaFiles[i] = Path.GetRelativePath(_ReplicaPath, ReplicaFiles[i]);
            }

            List<string> ExcessiveFiles = ReplicaFiles.Except(TempOriginFiles).ToList();

            foreach (var ExcessiveFile in ExcessiveFiles)
            {
                fileName = Path.GetFileName(ExcessiveFile);
                relativePath = Path.GetDirectoryName(Path.Combine(_ReplicaPath, ExcessiveFile));

                if (relativePath == null)
                {
                    relativePath = string.Empty;
                }

                RemoveFileAndLog(relativePath, fileName);
            }
            logger.Log("Removing excessive files");
        }

        private void RemoveFileAndLog(string relativePath, string fileName)
        {
            try
            {
                FileManager.DeleteFile(Path.Combine(_ReplicaPath, relativePath, fileName));
                logger.AddMessage(InteractionType.DELETE, Path.Combine(_ReplicaPath, relativePath, fileName));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error removing file {fileName} from {Path.Combine(_ReplicaPath, relativePath)}: {ex.Message}");
                logger.AddMessage(InteractionType.ERROR, $"Error removing file {fileName} from {Path.Combine(_ReplicaPath, relativePath)}: {ex.Message}");
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
                    FileManager.DeleteDirectory(Path.Combine(_ReplicaPath, excessiveDir));
                    logger.AddMessage(InteractionType.DELETE, Path.Combine(_ReplicaPath, excessiveDir));
                }

                logger.Log("Removing excessive directories");

                foreach (var missingDir in MissingDirectories)
                {
                    FileManager.CreateDirectory(Path.Combine(_ReplicaPath, missingDir));
                    logger.AddMessage(InteractionType.COPY, Path.Combine(_ReplicaPath, missingDir));
                }

                logger.Log("Creating missing directories");
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
        bool CopyUpdateFile(string relativePath, string fileName, InteractionType interactionType = InteractionType.COPY)
        {
            String FilePath = Path.Combine(_OriginPath, relativePath);
            String DestinationFilePath = Path.Combine(_ReplicaPath, relativePath);

            try
            {
                FileManager.CheckAndCreateDirectory(DestinationFilePath);

                File.Copy(Path.Combine(FilePath, fileName), 
                    Path.Combine(DestinationFilePath, fileName), true);

                logger.AddMessage(interactionType, Path.Combine(DestinationFilePath, fileName));

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error copying file {fileName} from {FilePath} to {DestinationFilePath}: {ex.Message}");
                logger.AddMessage(InteractionType.ERROR, $"Error copying file {fileName} from {FilePath} to {DestinationFilePath}: {ex.Message}");
                return false;
            }
        }

        string[] getRelativePath(string fullPath, string[] paths)
        {
            string[] relativePaths = new string[paths.Length];
            for (int i = 0; i < paths.Length; i++)
            {
                relativePaths[i] = Path.GetRelativePath(fullPath, paths[i]);
            }
            return relativePaths;
        }

        void CheckBothPaths()
        {
             
        }
    }
}
