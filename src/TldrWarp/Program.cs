using System.Diagnostics;
using System.Reflection;
using Microsoft.Win32;
using static Microsoft.Win32.Registry;

if (args.Length == 0) AssociateFileExtension(".tldr", "tldrawfile", Assembly.GetEntryAssembly().Location.Replace(".dll", ".exe"));
else if (args.Length > 0 && File.Exists(args[0])) {
    await ExecuteCommand("");
    await ExecuteCommand(args[0]);
}

return;

static void AssociateFileExtension(string extension, string fileType, string appName) {
    // 步骤 1：判断关联是否存在
    using RegistryKey? key = ClassesRoot.OpenSubKey(extension);
    if (key == null) {
        // 步骤 2：创建文件类型
        using RegistryKey fileTypeKey = ClassesRoot.CreateSubKey(fileType);
        // 步骤 3：创建关联键
        using RegistryKey extensionKey = fileTypeKey.CreateSubKey(@"shell\open\command");
        // 步骤 4：设置关联应用程序信息
        extensionKey.SetValue(null, $"\"{appName}\" \"%1\"");

        // 设置关联应用程序图标信息（可选）
        using (RegistryKey iconKey = fileTypeKey.CreateSubKey("DefaultIcon"))
        {
            iconKey.SetValue(null, $"{appName},0");
        }
    }
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