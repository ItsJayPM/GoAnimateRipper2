using GoAnimateRipper2.Managers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GoAnimateRipper2
{
    public class RipperBase
    {
        public MainControl mainControl;
        public AssetManager assetManager;
        DecompManager decompManager;
        public XElement xmlDoc;
        public bool downloadSuccess;
        public string serverAddress;
        public string xmlFilename;
        public string themeId;
        public string folder = "";
        public string fileLocation;

        public RipperBase(MainControl mainControl, string themeId)
        {
            this.mainControl = mainControl;
            assetManager = new AssetManager(mainControl);
            this.themeId = themeId;
        }

        /// <summary>
        /// void <c>initializeRipper</c> does setup that both rippers share.
        /// </summary>
        public async Task<bool> initializeRipper()
        {
            mainControl.LockControl();
            if (mainControl.domain.Substring(mainControl.domain.Length - 1) != "/") mainControl.domain += "/";
            fileLocation = $"{folder.Replace("/", "\\")}{themeId}\\";

            if (Directory.Exists(".\\" + fileLocation) && mainControl.doDecompile) { Directory.Delete(".\\" + fileLocation, true); }

            mainControl.writeMessage(serverAddress);
            await assetManager.DownloadAsset(fileLocation + xmlFilename, false);
            try
            {
                xmlDoc = XElement.Load(assetManager.GetLocalPath(fileLocation + xmlFilename));
                return true;
            }
            catch
            {
                
                mainControl.ReturnWithMessage("An error occured parsing the theme file! (Are you sure the theme you typed exists?)");
                Directory.Delete(".\\" + fileLocation, true);
                return false;
            }
        }
        /// <summary>
        /// void <c>endRipper</c> finishes up the ripping proceedure.
        /// </summary>
        public void endRipper()
        {
            if (mainControl.doDecompile)
            {
                mainControl.writeMessage("Passing results to FFDec...", false);
                decompManager = new DecompManager(mainControl, assetManager.getPathes());
                decompManager.RunFFDec();
                if (mainControl.decOrganize) decompManager.ReorganizeAfterFFDec();
            }
            //if (carryThemeId == null)
            //{
                mainControl.ReturnWithMessage("The proceedure has completed.");
            //}
        }

        /// <summary>
        /// void <c>StartRip</c> starts the ripper; Template function.
        /// </summary>
        public async Task StartRip()
        {
            // Bla
        }
    }
}
