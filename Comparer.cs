using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CopyCat
{
    internal class Comparer
    {

        public bool Compare(String originFile, String replicaFile)
        {
            if (!FileManager.FileExist(originFile) || !FileManager.FileExist(replicaFile))
            {
                return false;
            }
            
            //TODO Compare by size and hash

            return true;
        }



    }
}
