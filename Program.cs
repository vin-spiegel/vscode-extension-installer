using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace VSCode
{
    internal class Program
    {
        private const string VSCodePath = @"C:\Users\{0}\AppData\Local\Programs\Microsoft VS Code\Code.exe";
        
        
        private const string Extensions = @"
{
    ""recommendations"": [
        ""sumneko.lua"",
        ]
}";
        
        private const string InstallCommand = "code --install-extension sumneko.lua@3.5.5";

        private string _localUserName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
        
        private DirectoryInfo _root;
        
        /// <summary>
        /// command : 실행할 명령어 
        /// </summary>
        private static string ExecuteCmd(string command)
        {
            var pri = new ProcessStartInfo();
            var pro = new Process();

            pri.FileName = @"cmd.exe";
            pri.CreateNoWindow = true;
            pri.UseShellExecute = false;
            
            //표준 출력을 리다이렉트
            pri.RedirectStandardInput = true;          
            pri.RedirectStandardOutput = true;
            pri.RedirectStandardError = true;
            
            //어플리케이션 실
            pro.StartInfo = pri;
            pro.Start();   

            pro.StandardInput.Write(command + Environment.NewLine);
            // pro.StandardInput.Write(command);
            pro.StandardInput.Close();

            var sr = pro.StandardOutput;

            var resultValue = sr.ReadToEnd();
            pro.WaitForExit();
            pro.Close();

            return resultValue == "" ? "" : resultValue;
        }
        
        public static void Main(string[] args)
        {
            new Program();
        }

        private Program()
        {
            _root = Directory.GetParent(Directory.GetParent(Environment.CurrentDirectory).ToString());

            var vscodeFolderPath = Path.Combine(_root.ToString(), ".vscode");

            var di = new DirectoryInfo(vscodeFolderPath);

            di.Create();
            
            File.WriteAllText(Path.Combine(vscodeFolderPath, "extensions.json"), Extensions, Encoding.UTF8);

            var path = string.Format(VSCodePath, Environment.UserName);
            var fi = new FileInfo(path);

            if (!fi.Exists)
                return;
            
            var resultText = ExecuteCmd(InstallCommand);
            var result = ExecuteCmd($"code {_root}");
        }
    }
}