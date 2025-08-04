using CopyCat.Helpers;
using CopyCat.Synchro;

partial class Program
{
    readonly static char[] INTERVAL_CHARS = { 's', 'm', 'h', 'd' };

    static void Main(string[] args)
    {
        Console.WriteLine("CopyCat Synchronization Tool by Patryk Gusarzewski");

        InitCheckers(args);

        //Start synchronization
        Synchronizer synchronizer = new Synchronizer(args[0], args[1], args[2], args[3]);
        synchronizer.SynchronizeLoop();
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
        if (args.Length != 4)
        {
            Console.WriteLine("Please provide four arguments: <OriginPath> <ReplicaPath> <Synchronization Interval> <Logs path>");
            Environment.Exit(1);
        }

        OriginPathCheck(args[0]);

        ReplicaPathCheck(args[1]);

        args[2] = IntervalCheck(args[2]);
        Console.WriteLine($"Synchronization interval set to {args[2]} seconds.");

        LogPathCheck(args[3]);
    }

    private static void OriginPathCheck(string originPath)
    {
        if (!FileManager.DirectoryExist(originPath))
        {
            Console.WriteLine($"Origin path '{originPath}' does not exist.");
            if (UserChoice("Do you want to create it?"))
            {
                if (!FileManager.CreateDirectory(originPath))
                {
                    Environment.Exit(1);
                }
            }
            else Environment.Exit(1);
        }
    }

    private static void ReplicaPathCheck(string replicaPath)
    {
        if (FileManager.DirectoryExist(replicaPath))
        {
            Console.WriteLine($"Replica path '{replicaPath}' already exists.");
            if (UserChoice("Are you sure you want to use it? All content inside will be removed before synchronization start."))
            {
                FileManager.DeleteDirectory(replicaPath);
            }
            else Environment.Exit(1);
        }
        FileManager.CreateDirectory(replicaPath);
    }

    private static void LogPathCheck(string logPath)
    {
        if (!FileManager.DirectoryExist(logPath))
        {
            Console.WriteLine($"Log path '{logPath}' does not exist.");
            if (UserChoice("Do you want to create it?"))
            {
                if (FileManager.CreateDirectory(logPath))
                    Environment.Exit(1);
            }
            else Environment.Exit(1);
        }
    }

    private static string IntervalCheck(string intervalArg)
    {
        int timeInterval = 0;

        if (string.IsNullOrWhiteSpace(intervalArg))
        {
            Console.WriteLine("Synchronization interval cannot be empty.");
            Environment.Exit(1);
        }

        if (int.TryParse(intervalArg, out timeInterval))
        {

        }
        else if ((!int.TryParse(intervalArg, out var value) && intervalArg.Length >= 2))
        {
            char lastChar = intervalArg.ToLower().Last();
            Console.WriteLine($"Last character of synchronization interval: '{lastChar}'");
            if (INTERVAL_CHARS.Contains(lastChar) &&
                int.TryParse(intervalArg.Substring(0, intervalArg.Length - 1), out var modTimeInterval))
            {
                switch (lastChar)
                {
                    case 's':
                        timeInterval = modTimeInterval;
                        break;
                    case 'm':
                        timeInterval = modTimeInterval * 60;
                        Console.WriteLine($"minutes");
                        break;
                    case 'h':
                        timeInterval = modTimeInterval * 60 * 60;
                        break;
                    case 'd':
                        timeInterval = modTimeInterval * 60 * 60 * 24;
                        break;
                    default:
                        Console.WriteLine("Invalid synchronization interval format.");
                        Environment.Exit(1);
                        break;
                }
            }
            else
            {
                Console.WriteLine("Invalid synchronization interval format.");
                Environment.Exit(1);
            }
        }
        else
        {
            Console.WriteLine("Invalid synchronization interval format.");
            Environment.Exit(1);
        }

        if (timeInterval < 0)
        {
            Console.WriteLine("Synchronization interval must be a positive number.");
            Environment.Exit(1);
        }

        //if everything is fine, set time interval
        return timeInterval.ToString();
    }
}

