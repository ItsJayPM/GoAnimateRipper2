using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace GoAnimateRipper2
{
    public class CCRipper : RipperBase
    {
        public CCRipper(MainControl mainControl, string themeId) : base(mainControl, themeId)
        {
            xmlFilename = "cc_theme.xml";
            folder = "cc_store/";
        }

        /// <summary>
        /// void <c>StartRip</c> starts the ripper.
        /// </summary>
        public async Task StartRip()
        {
            if (!await initializeRipper()) return;

            var components = xmlDoc.Elements("component");
            //Reworked bar that actually isn't shite
            mainControl.setProgressBarMaximum(components.Count());
            mainControl.resetProgressBar();
            foreach (var component in components)
            {
                string componentId = component.Attributes().Where(a => a.Name == "id").Single().Value;
                string componentType = component.Attributes().Where(a => a.Name == "type").Single().Value;
                string componentThumb = component.Attributes().Where(a => a.Name == "thumb").Single().Value;
                var states = component.Elements("state");

                fileLocation = $"cc_store\\{themeId}\\{componentType}\\{componentId}";
                if (componentType == "mouth")
                {
                    if (themeId == "family")
                    {
                        downloadSuccess = await assetManager.DownloadAsset($"{fileLocation}\\talk_sad_sync.swf", mainControl.doDecryption);
                        if (downloadSuccess) mainControl.writeMessage($"Downloaded state 'talk_sad_sync.swf' for component '{componentId}' ({componentType}).", false);
                        downloadSuccess = await assetManager.DownloadAsset($"{fileLocation}\\talk_happy_sync.swf", mainControl.doDecryption);
                        if (downloadSuccess) mainControl.writeMessage($"Downloaded state 'talk_happy_sync.swf' for component '{componentId}' ({componentType}).", false);
                        downloadSuccess = await assetManager.DownloadAsset($"{fileLocation}\\talk_angry_sync.swf", mainControl.doDecryption);
                        if (downloadSuccess) mainControl.writeMessage($"Downloaded state 'talk_angry_sync.swf' for component '{componentId}' ({componentType}).", false);

                    }
                    if (themeId == "anime" || themeId == "ninjaanime" || themeId == "spacecitizen") //wack
                    {
                        downloadSuccess = await assetManager.DownloadAsset($"{fileLocation}\\side_talk_sync.swf", mainControl.doDecryption);
                        if (downloadSuccess) mainControl.writeMessage($"Downloaded state 'side_talk_sync.swf' for component '{componentId}' ({componentType}).", false);
                    }
                    else
                    {
                        downloadSuccess = await assetManager.DownloadAsset($"{fileLocation}\\talk.swf", mainControl.doDecryption);
                        if (downloadSuccess) mainControl.writeMessage($"Downloaded state 'talk.swf' for component '{componentId}' ({componentType}).", false);
                        downloadSuccess = await assetManager.DownloadAsset($"{fileLocation}\\talk_sync.swf", mainControl.doDecryption);
                        if (downloadSuccess) mainControl.writeMessage($"Downloaded state 'talk_sync.swf' for component '{componentId}' ({componentType}).", false);
                    }
                }
                downloadSuccess = await assetManager.DownloadAsset(fileLocation + "\\" + componentThumb, mainControl.doDecryption);
                if (downloadSuccess) mainControl.writeMessage($"Downloaded thumbnail for component '{componentId}' ({componentType}).", false);

                foreach (var state in states)
                {

                    var stateId = state.Attributes().Where(a => a.Name == "id").Single().Value;
                    var stateFilename = state.Attributes().Where(a => a.Name == "filename").Single().Value;

                    fileLocation = $"cc_store\\{themeId}\\{componentType}\\{componentId}\\{stateFilename}";
                    downloadSuccess = await assetManager.DownloadAsset(fileLocation, mainControl.doDecryption);

                    if (downloadSuccess) mainControl.writeMessage($"Downloaded state '{stateId}' for component '{componentId}' ({componentType}).", false);
                }
                mainControl.incrementProgressBar();
            }

            var bodyshapes = xmlDoc.Elements("bodyshape");
            foreach (var bodyshape in bodyshapes)
            {
                var bodyId = bodyshape.Attributes().Where(a => a.Name == "id").Single().Value;

                var actionpacks = bodyshape.Elements("actionpack");
                var libraries = bodyshape.Elements("library");
                mainControl.setProgressBarMaximum(libraries.Count());
                mainControl.resetProgressBar();
                foreach (var library in libraries)
                {
                    var libraryType = library.Attributes().Where(a => a.Name == "type").Single().Value;
                    var libraryId = library.Attributes().Where(a => a.Name == "path").Single().Value;
                    var libraryThumb = library.Attributes().Where(a => a.Name == "thumb").Single().Value;

                    fileLocation = $"cc_store\\{themeId}\\{libraryType}\\";

                    downloadSuccess = await assetManager.DownloadAsset(fileLocation + libraryId + ".swf", false);
                    if (downloadSuccess) mainControl.writeMessage($"Downloaded library '{libraryId}.swf' ({libraryType}).", false);
                    if (libraryThumb != libraryId + ".swf")
                    {
                        downloadSuccess = await assetManager.DownloadAsset(fileLocation + libraryThumb, mainControl.doDecryption);
                        if (downloadSuccess) mainControl.writeMessage($"Downloaded library thumb '{libraryThumb}' ({libraryType}).", false);
                    }
                    mainControl.incrementProgressBar();
                }
                foreach (var actionpack in actionpacks)
                {
                    var actionpackName = actionpack.Attributes().Where(a => a.Name == "id").Single().Value;
                    var actions = actionpack.Elements("action");
                    mainControl.setProgressBarMaximum(actions.Count());
                    mainControl.resetProgressBar();
                    foreach (var action in actions)
                    {
                        var actionId = action.Attributes().Where(a => a.Name == "id").Single().Value;
                        fileLocation = $"cc_store\\{themeId}\\freeaction\\{bodyId}\\{actionId}.swf";
                        //Console.WriteLine(localDir + "," + localFileName);
                        downloadSuccess = await assetManager.DownloadAsset(fileLocation, mainControl.doDecryption);
                        if (downloadSuccess) mainControl.writeMessage($"Downloaded freeaction '{actionId}' for bodytype '{bodyId}' (actionpack {actionpackName}).");
                        mainControl.incrementProgressBar();
                    }
                }
                components = bodyshape.Elements("component");
                mainControl.setProgressBarMaximum(components.Count());
                mainControl.resetProgressBar();
                foreach (var component in components)
                {
                    var componentId = component.Attributes().Where(a => a.Name == "id").Single().Value;
                    var componentThumb = component.Attributes().Where(a => a.Name == "thumb").Single().Value;
                    var componentType = component.Attributes().Where(a => a.Name == "type").Single().Value;
                    if (componentType == "freeaction")
                    {
                        break;
                    }
                    var states = component.Elements("state");

                    //This is a small thing but it created a HUGE bug in one specific case lemme tell you
                    if (componentType != "skeleton")
                    {
                        fileLocation = $"cc_store\\{themeId}\\{componentType}\\{componentId}\\{componentThumb}";
                        downloadSuccess = await assetManager.DownloadAsset(fileLocation, mainControl.doDecryption);
                        if (downloadSuccess) mainControl.writeMessage($"Downloaded state '{componentThumb}' for component '{componentId}' ({componentType}).");
                    }

                    foreach (var state in states)
                    {

                        var stateId = state.Attributes().Where(a => a.Name == "id").Single().Value;

                        fileLocation = $"cc_store\\{themeId}\\{componentType}\\{componentId}\\{stateId}.swf";
                        downloadSuccess = await assetManager.DownloadAsset(fileLocation, mainControl.doDecryption);

                        if (downloadSuccess) mainControl.writeMessage($"Downloaded state '{stateId}' for component '{componentId}' ({componentType}).");
                    }
                    mainControl.incrementProgressBar();
                }

            }
            endRipper();
            return;
        }
    }
}
