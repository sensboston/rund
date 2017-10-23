using System;
using System.Linq;
using System.Diagnostics;

public class Program
{
    public static void Main(string[] args)
    {
        if (args.Length > 0)
        {
            var arguments = string.Empty;

            string[] exts = { ".com", ".exe", ".bat", ".cmd" };
            var exeName = args.Where(arg => exts.Any(ext => arg.Contains(ext))).FirstOrDefault();
            if (!string.IsNullOrEmpty(exeName))
            {
                var exeIndex = args.ToList().IndexOf(exeName);
                var cmd = args.Take(exeIndex + 1).Aggregate((i, j) => i + " " + j);
                if (exeIndex + 1 < args.Length) arguments = args.Skip(exeIndex + 1).Aggregate((i, j) => i + " " + j);

                Process p = new Process();
                p.StartInfo.FileName = exeName;
                p.StartInfo.Arguments = arguments;
                p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                try { p.Start(); } catch (Exception e) { Console.WriteLine(e.Message); }
                return;
            }
        }
        Console.WriteLine("Please provide a program name [with arguments]");        
    }
}
