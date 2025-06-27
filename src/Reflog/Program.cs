using System.Diagnostics;
using System.Text.RegularExpressions;
using Spectre.Console;
using TextCopy;

namespace Reflog
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Directory.SetCurrentDirectory("C:\\dev\\git\\windows-browser");

            var size = 10;

            if (args.Length > 0 && int.TryParse(args[0], out var newSize))
            {
                size = newSize;
            }
            
            var rawLines = GetGitReflogLines().ToList();
            
            var movedBranches = GetMovesToCommitOrBranch(rawLines).Take(size);

            if (!movedBranches.Any())
            {
                AnsiConsole.MarkupLine("[red]No branches found, quitting.[/]");
                return;
            }

            var copyBranch = string.Empty;
            var diffBranch = string.Empty;
            var quitBranch = string.Empty;

            var chosenBranch = AnsiConsole.Prompt(
                new SelectionPrompt<string>().Title("Pick branch:")
                    .PageSize(10)
                    .AddChoices(movedBranches)
                    .Footer("[grey](Press [blue]<c>[/] to copy, [blue]<d>[/] to difftool, [blue]<q>[/] to exit, [green]<enter>[/] to switch to branch)[/]")
                    .WithCustomAction(ConsoleKey.C, s => copyBranch = s)
                    .WithCustomAction(ConsoleKey.Q, s => quitBranch = s)
                    .WithCustomAction(ConsoleKey.D, s => diffBranch = s)
                );


            if (!string.IsNullOrEmpty(quitBranch))
            {
                Environment.Exit(0);
            }
            else if (!string.IsNullOrEmpty(copyBranch))
            {
                ClipboardService.SetText(copyBranch);
                AnsiConsole.Write(new Markup($"[grey]copied [blue]{copyBranch}[/] to clipboard[/]"));
            }
            else if (!string.IsNullOrEmpty(diffBranch))
            {
                AnsiConsole.Write(new Markup($"[grey]diffing with [blue]{diffBranch}[/][/]"));
                var gitResponse = InvokeGit($"difftool {diffBranch} --dir-diff");
                Debug.WriteLine($"> {gitResponse}");
            }
            else
            {
                Console.WriteLine($"git checkout {chosenBranch}");
                var gitResponse = InvokeGit($"checkout {chosenBranch}");
                Debug.WriteLine($"> {gitResponse}");
            }
        }

        private static string[] GetMovesToCommitOrBranch(List<string> rawLines)
        {
            var moves = rawLines.Select(line =>
                {
                    var pattern = @"\b(\w+)\s+HEAD@\{\d+\}:\s+(\w+):\s+moving\s+from\s+(\S+)\s+to\s+(\S+)";
                    var regex = new Regex(pattern);

                    var match = regex.Match(line);
                    return match.Success ? match.Groups[4].Value : null;
                })
                .Where(l => l != null);

            var orderedMoves = new List<string>();

            foreach (var move in moves)
            {
                if (!orderedMoves.Contains(move))
                {
                    orderedMoves.Add(move);
                }
            }

            return orderedMoves.ToArray();
        }

        private static string[] GetGitReflogLines()
        {
            // Set up process start info
            var output = InvokeGit("reflog");

            return output.Split('\n').Select(s => s.Trim()).ToArray();
        }

        private static string InvokeGit(string arguments)
        {
            var startInfo = new ProcessStartInfo();
            startInfo.FileName = "git"; // Assuming git is in the system path
            startInfo.Arguments = arguments;
            startInfo.RedirectStandardOutput = true;
            startInfo.UseShellExecute = false;

            // Start the process
            var process = new Process
            {
                StartInfo = startInfo
            };

            process.Start();

            // Read the output
            var output = process.StandardOutput.ReadToEnd();

            // Wait for the process to finish
            process.WaitForExit();
            return output;
        }
    }
}
