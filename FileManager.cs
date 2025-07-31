using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CopyCat
{
    public static class FileManager
    {
        public static bool FileExist(string path)
        {
            return System.IO.File.Exists(path);
        }

        public static bool DirectoryExist(string path)
        {
            return System.IO.Directory.Exists(path);
        }

        public static void CreateDirectory(string path)
        {
            if (!DirectoryExist(path))
            {
                System.IO.Directory.CreateDirectory(path);
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
    }
}
