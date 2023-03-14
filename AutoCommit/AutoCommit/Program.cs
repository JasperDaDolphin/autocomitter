using System.Diagnostics;

const string GitRepo = "E:\\git\\Auto";
const string CommitsDirectory = $"{GitRepo}\\Commits\\";
const int MaxNumberOfDailyCommits = 5;

static void ExecuteCommand(string command, string workingDirectory = null)
{
    int exitCode;
    ProcessStartInfo processInfo;
    Process process;

    processInfo = new ProcessStartInfo("cmd.exe", "/c " + command);
    processInfo.CreateNoWindow = true;
    processInfo.UseShellExecute = false;

    processInfo.RedirectStandardError = true;
    processInfo.RedirectStandardOutput = true;

    if (workingDirectory != null)
    {
        processInfo.WorkingDirectory = workingDirectory;
    }

    process = Process.Start(processInfo);
    process.WaitForExit();

    var output = process.StandardOutput.ReadToEnd();
    var error = process.StandardError.ReadToEnd();

    exitCode = process.ExitCode;

    Console.WriteLine("output>>" + (string.IsNullOrEmpty(output) ? "(none)" : output));
    Console.WriteLine("error>>" + (string.IsNullOrEmpty(error) ? "(none)" : error));
    Console.WriteLine("ExitCode: " + exitCode, "ExecuteCommand");
    process.Close();
}

static int GenerateWaveNumber(int previousNumber, int range, int maxNumber)
{
    var rnd = new Random();

    var min = previousNumber - range;
    var max = previousNumber + range;

    if (min < 0)
    {
        min = 0;
    }

    if (max > maxNumber)
    {
        max = maxNumber;
    }

    var randomNumber = rnd.Next(min, max + 1);

    if (randomNumber < min)
    {
        randomNumber = min;
    }
    else if (randomNumber > max)
    {
        randomNumber = max;
    }

    return randomNumber;
}

static void Run(DateTime date)
{
    var dateAsString = date.ToString("yy-MM-dd");

    Directory.CreateDirectory(CommitsDirectory);

    var newDirectory = $"{CommitsDirectory}\\{dateAsString}";
    if (!Directory.Exists(newDirectory))
    {
        Console.WriteLine("Starting.");
        var di = new DirectoryInfo(CommitsDirectory);
        var files = di.GetFileSystemInfos();
        var orderedFiles = files.OrderBy(f => f.CreationTime).FirstOrDefault();

        var previous = 1;
        if (orderedFiles != null)
        {
            previous = Directory.GetFiles(orderedFiles.FullName).Length;
        }

        Directory.CreateDirectory(newDirectory);

        var number = GenerateWaveNumber(previous, 1, MaxNumberOfDailyCommits);
        for (var i = 0; i < number; i++)
        {
            var fileName = $"{newDirectory}\\{i}.txt";
            var content = $"{dateAsString} {i}";

            File.WriteAllText(fileName, content);

            ExecuteCommand($"git add {fileName}", GitRepo);

            ExecuteCommand($"git commit -m \"{content}\"", GitRepo);
        }

        ExecuteCommand("git push", GitRepo);
    }
    else
    {
        Console.WriteLine("Already run today.");
    }
}

Run(DateTime.UtcNow);
Thread.Sleep(1000);