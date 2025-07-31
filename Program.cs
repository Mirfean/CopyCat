// See https://aka.ms/new-console-template for more information
using CopyCat;
using CopyCat.Synchro;

partial class Program
{
    static void Main(string[] args)
    {
        if (args.Length != 4)
        {
            Console.WriteLine("Please provide four arguments: <OriginPath> <ReplicaPath> <Synchronization Interval> <Logs path>");
            return;
        }

        if (!FileManager.CheckAndCreateDirectory(args[0]))
        {
            Console.WriteLine($"Origin path '{args[0]}' does not exist. Folder has been created.");
        }

        //TODO - Add check if replica path already exists and clear it if it does
        FileManager.CheckAndCreateDirectory(args[1]);
        

        Synchronizer synchronizer = new Synchronizer(args[0], args[1]);

        synchronizer.Synchronize();
    }
}

