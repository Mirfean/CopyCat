using CopyCat;
using CopyCat.Synchro;

partial class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("CopyCat Synchronization Tool by Patryk Gusarzewski");

        InitCheckers(args);

        //Start synchronization
        Synchronizer synchronizer = new Synchronizer(args[0], args[1], args[3]);
        synchronizer.Synchronize();
    }

    public static bool UserChoice(string message)
    {

        Console.WriteLine(message + " [y/n]");
        string choice = Console.ReadLine().ToLower();
        if (choice == null)
        {
            Console.WriteLine("Invalid input");
            return UserChoice(message);
        }
        if (choice != "y" && choice != "n")
        {
            Console.WriteLine("Invalid input");
            return UserChoice(message);
        }
        return choice == "y";
    }

    static void InitCheckers(string[] args)
    {
        // ARGUMENTS CHECK

        if (args.Length != 4)
        {
            Console.WriteLine("Please provide four arguments: <OriginPath> <ReplicaPath> <Synchronization Interval> <Logs path>");
            return;
        }

        // ORIGIN PATH CHECK

        if (!FileManager.DirectoryExist(args[0]))
        {
            Console.WriteLine($"Origin path '{args[0]}' does not exist.");
            if (UserChoice("Do you want to create it?"))
            {
                FileManager.CreateDirectory(args[0]);
            }
            else return;
        }

        // REPLICA PATH CHECK

        if (FileManager.DirectoryExist(args[1]))
        {
            Console.WriteLine($"Replica path '{args[1]}' already exists.");
            if (UserChoice("Do you want to delete it?"))
            {
                FileManager.DeleteDirectory(args[1]);
            }
            else return;
        }
        FileManager.CreateDirectory(args[1]);

        // TIME INTERVAL CHECK
        

        // LOGS PATH CHECK

    }
}

