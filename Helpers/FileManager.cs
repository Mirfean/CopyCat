using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CopyCat.Helpers
{
    public static class FileManager
    {
        public static bool FileExist(string path)
        {
            return File.Exists(path);
        }

        public static bool DirectoryExist(string path)
        {
            return Directory.Exists(path);
        }

        public static bool CreateDirectory(string path)
        {
            try
            {
                if (!DirectoryExist(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating directory '{path}': {ex.Message}");
                return false;
            }
            return true;
        }

        public static void DeleteDirectory(string path)
        {
            if (DirectoryExist(path))
            {
                Directory.Delete(path, true);
            }
        }
        
        public static void DeleteFile(string path)
        {
            if (FileExist(path))
            {
                File.Delete(path);
            }
        }

        public static bool CheckAndCreateDirectory(string path)
        {
            if (!DirectoryExist(path))
            {
                CreateDirectory(path);
                return false;
            }
            return true;
        }

        internal static void CreateFile(string path, string file)
        {
            if (!FileExist(Path.Combine(path, file)))
            {
                File.Create(Path.Combine(path, file)).Dispose();
            }
        }
    }
}
