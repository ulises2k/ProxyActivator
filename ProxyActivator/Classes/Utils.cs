using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ProxyActivator
{
    class Utils
    {
        public static Boolean AppDataRoamingFolderExists(string FolderName)
        {
            String path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\" + FolderName;
            if (System.IO.Directory.Exists(path))
                return true;
            else
                return false;
        }

        public static Boolean AppDataLocalFolderExists(string FolderName)
        {
            String path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\" + FolderName;
            if (System.IO.Directory.Exists(path))
                return true;
            else
                return false;
        }

        public static String KillProcessAndGetExePathWait(string executableName)
        {
            Process[] processlist = Process.GetProcesses();
            foreach (Process theprocess in processlist)
            {
                if (theprocess.ProcessName.ToLower().Contains(executableName.ToLower()))
                {
                    string ExePath = theprocess.Modules[0].FileName;
                    theprocess.Kill();
                    theprocess.WaitForExit();
                    return ExePath;
                }
            }
            return "";
        }

        public static void StartExecutable(string path)
        {
            if ("" != path)
            {
                ProcessStartInfo start = new ProcessStartInfo();
                start.FileName = path;
                start.WindowStyle = ProcessWindowStyle.Hidden;
                start.CreateNoWindow = true;
                Process proc = Process.Start(start);
            }
        }

        /// <summary>
        /// Restarts a Application if its running
        /// </summary>
        /// <param name="ExecutableName">The .exe Name of the Program</param>
        public static void RestartApplicationIfRunning(string executableName, String arguments = "")
        {
            String path = KillProcessAndGetExePathWait(executableName);
            StartExecutable(path);
        }        
    }
}
