using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoAnimateRipper2.Managers
{
    internal class DecompManager
    {
        MainControl mainControl;
        List<string> pathes;
        public DecompManager(MainControl mainControl, List<string> pathes)
        {
            this.mainControl = mainControl;
            this.pathes = pathes;
        }

        /// <summary>
        /// Task <c>RunFFDec</c> handles the batch decompilation of assets using JPEXS.
        /// </summary>
        public void RunFFDec()
        {
            Directory.CreateDirectory($".\\ffdec_output");
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            String cmd = $"ffdec.bat -export fla \"{System.AppContext.BaseDirectory}ffdec_output\"  \"{System.AppContext.BaseDirectory}ffdec_working\"";
            //Console.WriteLine(cmd);
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo("cmd", "/c " + cmd);
            if (mainControl.decDontShowCmd) startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.WorkingDirectory = @"C:\Program Files (x86)\FFDec";
            Console.WriteLine(startInfo.Arguments);
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();

            try
            {
                Directory.Delete($".\\ffdec_working", true);
            }
            catch
            {
                mainControl.writeMessage("Dir in use; Skipping deletion...", true);
            }
        }

        /// <summary>
        /// void <c>ReorganizeAfterFFDec</c> moves the JPEXS output back to their proper locations, then cleans vars.
        /// </summary>
        public void ReorganizeAfterFFDec()
        {
            if (mainControl.decOrganize)
            {
                //Console.WriteLine(pathes.Count);
                //Console.WriteLine("SUCCESS!");
                for (int i = 0; i < pathes.Count; i++)
                {
                    String projectedPath = $".\\ffdec_output\\{i}.swf";
                    String finalLocation = pathes[i].Substring(0, pathes[i].LastIndexOf("."));
                    String finalName = finalLocation.Substring(finalLocation.LastIndexOf("\\"));
                    if (Directory.Exists(projectedPath))
                    {
                        //Console.WriteLine($"{i} is associated to {finalLocation}");
                        Directory.Move(projectedPath, finalLocation);
                        File.Move(finalLocation + $"\\{i}.fla", finalLocation + $"\\{finalName}.fla");
                    }
                }
                Directory.Delete($".\\ffdec_output", true);
                pathes.Clear();
            }
        }
    }
}
