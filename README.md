# winp.dll
Winp is a small C# Library for Windows, that has some special funcitons for Windows Processes and Tools

# Functions (WinProcess)
- SetCritical = Set a process critical
- IsCritical = Check if a process is critical
- IsAdministrator = Check if the current process runs as administrator
- RunAsAdmin = Run a file as administrator
- GetCurrentProcess = Get the current process
- Cmd = Execute a cmd command and get the output
- End = End a process (with force end option)

# Functions (WinTool)
- Disable = Disable a windows tool (Regedit, Taskmgr, UAC)
- Enable = Enable a windows tool (Regedit, Taskmgr, UAC)
- IsDisabled = Check if a windows tool (Regedit, Taskmgr, UAC) is disabled

# How to use winp
1. Create/Open a new C# .NET Project
2. Add winp.dll to references
3. Add winp to the usings: using winp;
4. Write your code
