using Microsoft.Win32;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace winp
{
    public class CmdResult
    {
        public string Error { get; set; }
        public string Output { get; set; }
        public string Command { get; set; }

        public CmdResult(string error, string output, string command)
        {
            Error = error;
            Output = output;
            Command = command;
        }
    }

    public class WinTool
    {
        static public readonly string RegistryTools = "regedit";
        static public readonly string TaskManger = "taskmgr";
        static public readonly string UserAccountControl = "uac";
        
        static public bool IsDisabled(string tool)
        {
            try
            {
                switch (tool)
                {
                    case null:
                        break;
                    case "taskmgr":
                        RegistryKey taskmgr = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System", false);
                        int tmvalue = (int)taskmgr.GetValue("DisableTaskMgr", 0);
                        taskmgr.Close();
                        return tmvalue == 1;
                    case "regedit":
                        RegistryKey regedit = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System", false);
                        int revalue = (int)regedit.GetValue("DisableRegistryTools", 0);
                        regedit.Close();
                        return revalue == 1;
                    case "uac":
                        RegistryKey uac = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\System", false);
                        int uacvalue = (int)uac.GetValue("EnableLUA", 1);
                        uac.Close();
                        return uacvalue == 0;
                }
            }
            catch { }

            return false;
        }

        static public void Enable(string tool)
        {
            switch (tool)
            {
                case null:
                    break;
                case "taskmgr":
                    RegistryKey distaskmgr = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System");
                    distaskmgr.SetValue("DisableTaskMgr", (object)0, RegistryValueKind.DWord);
                    break;
                case "regedit":
                    RegistryKey disregedit = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System");
                    disregedit.SetValue("DisableRegistryTools", (object)0, RegistryValueKind.DWord);
                    break;
                case "uac":
                    RegistryKey keyUAC = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\System");
                    keyUAC.SetValue("EnableLUA", (object)1, RegistryValueKind.DWord);
                    break;
            }
        }
        
        static public void Disable(string tool)
        {
            switch (tool)
            {
                case null:
                    break;
                case "taskmgr":
                    RegistryKey distaskmgr = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System");
                    distaskmgr.SetValue("DisableTaskMgr", (object)1, RegistryValueKind.DWord);
                    break;
                case "regedit":
                    RegistryKey disregedit = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System");
                    disregedit.SetValue("DisableRegistryTools", (object)1, RegistryValueKind.DWord);
                    break;
                case "uac":
                    RegistryKey keyUAC = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\System");
                    keyUAC.SetValue("EnableLUA", (object)0, RegistryValueKind.DWord);
                    break;
            }
        }
    }

    public class WinProcess
    {
        [DllImport("ntdll.dll", SetLastError = true)]
        private static extern int NtSetInformationProcess(
            IntPtr hProcess,
            int processInformationClass,
            ref int processInformation,
            int processInformationLength);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool IsProcessCritical(IntPtr hProcess, ref bool Critical);

        static public Process GetCurrentProcess()
        {
            return Process.GetCurrentProcess();
        }

        static public void RunAsAdmin(Process proc)
        {
            proc.StartInfo.Verb = "runas";
            proc.Start();
        }

        static public CmdResult Cmd(string command)
        {
            Process p = new Process();
            
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.Arguments = "/c " + command;
            p.Start();

            string output = p.StandardOutput.ReadToEnd();
            string error  = p.StandardError.ReadToEnd();

            p.WaitForExit();

            return new CmdResult(output, error, command);
        }

        static public void SetCritical(Process proc)
        {
            int isCritical = 1;
            int BreakOnTermination = 29;
            Process.EnterDebugMode();
            NtSetInformationProcess
                (proc.Handle,
                BreakOnTermination,
                ref isCritical, 4);
        }

        static public bool IsAdministrator()
        {
            bool flag;
            using (WindowsIdentity current = WindowsIdentity.GetCurrent())
                flag = new WindowsPrincipal(current).IsInRole(WindowsBuiltInRole.Administrator);
            return flag;
        }

        static private void exeHidden(string file, string arg)
        {
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            psi.FileName = file;
            psi.Arguments = arg;

            Process.Start(psi);
        }

        static public void End(Process process, bool force = true)
        {
            string arg = "/pid " + process.Id;
            if (force)
            {
                arg = "/f " + arg;
            }

            exeHidden("taskkill.exe", arg);
        }

        static public bool IsCritical(Process proc)
        {
            bool procStatus = false;
            if (!IsProcessCritical(proc.Handle, ref procStatus))
                procStatus = true;
            return procStatus;
        }
    }
}
