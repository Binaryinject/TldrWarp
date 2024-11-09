using System.Diagnostics;
using GrassDemoPark.WPF2.Tiny.RegEdit;

if (args.Length == 0) await AssociateFileExtension(".tldr", "tldrfile", Process.GetCurrentProcess().MainModule.FileName);
else if (args.Length > 0 && File.Exists(args[0])) {
    await ExecuteCommand("");
    await ExecuteCommand(args[0]);
}

return;

async Task AssociateFileExtension(string extension, string fileType, string appName) {
    Console.WriteLine(appName);
    var registerFile = new RegisterFileExtension(extension);
    registerFile.OpenWithProgramIds.Add(fileType);
    registerFile.WriteToCurrentUser();

    var registerProgram = new RegisterProgram(fileType);
    registerProgram.TypeName = "Tldr Document";
    registerProgram.DefaultIcon = $"{appName},0";
    registerProgram.FriendlyTypeName = "tldraw的工程文件";
    registerProgram.Operation = "open";
    registerProgram.Command = $"\"{appName}\" \"%1\"";
    registerProgram.WriteToCurrentUser();
    await RunCMD("taskkill /im explorer.exe /f");
    await RunCMD("ping -n 2 127.0.0.1 > nul");
    await RunCMD("explorer.exe");
}

async Task RunCMD(string command) {
    var process = new Process();
    process.StartInfo.FileName = "cmd.exe";
    process.StartInfo.Arguments = "/c " + command;
    process.StartInfo.UseShellExecute = false;
    process.StartInfo.CreateNoWindow = true;
    process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
    process.StartInfo.RedirectStandardOutput = true;
    process.Start();
    await process.WaitForExitAsync();
}

async Task ExecuteCommand(string command) {
    var envs = Environment.GetEnvironmentVariable("PATH");
    var sp = envs?.Split(';');
    var code = sp!.First(v => v.Contains("Microsoft VS Code"));
    var processStartInfo = new ProcessStartInfo();
    processStartInfo.WorkingDirectory = code;
    processStartInfo.FileName = "code.cmd";
    processStartInfo.Arguments = command;
    processStartInfo.UseShellExecute = false;
    processStartInfo.CreateNoWindow = true;
    processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
    processStartInfo.RedirectStandardOutput = true;

    using var process = new Process();
    process.StartInfo = processStartInfo;
    process.Start();
    await process.WaitForExitAsync();
}