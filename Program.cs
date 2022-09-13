using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

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

        private readonly DirectoryInfo _root;

        private readonly Regex _regex = new Regex(@"([0-9][.][0-9][0-9][.][0-9])");
        
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

        public bool IsInstalled()
        {
            var context = ExecuteCmd("code --version");

            var res = _regex.Match(context.Trim());

            return res.Length > 0;
        }

        private Program()
        {
            _root = Directory.GetParent(Directory.GetParent(Environment.CurrentDirectory).ToString());

            var vscodeFolderPath = Path.Combine(_root.ToString(), ".vscode");

            var di = new DirectoryInfo(vscodeFolderPath);

            di.Create();
            
            File.WriteAllText(Path.Combine(vscodeFolderPath, "extensions.json"), Extensions, Encoding.UTF8);

            if (!IsInstalled())
                return;
            
            var resultText = ExecuteCmd(InstallCommand);
            var result = ExecuteCmd($"code {_root}");
        }
        
        public static void Main(string[] args)
        {
            new Program();
        }
    }
}
