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
    public class StandardRipper : RipperBase
    {
        public StandardRipper(MainControl mainControl, string themeId) : base(mainControl, themeId)
        {
            xmlFilename = "theme.xml";
        }

        public async Task StartRip()
        {
            if (!await initializeRipper()) return;

            var props = xmlDoc.Elements("prop");
            //Reworked bar that actually isn't shite
            mainControl.setProgressBarMaximum(props.Count());
            mainControl.resetProgressBar();
            if (!mainControl.skipFlash)
            {
                foreach (var prop in props)
                {

                    var propId = prop.Attributes().Where(a => a.Name == "id").Single().Value;
                    var states = prop.Elements("state");
                    if (states.Count() > 0)
                    {
                        foreach (var state in states)
                        {
                            var stateId = state.Attributes().Where(a => a.Name == "id").Single().Value;

                            fileLocation = $"{themeId}\\prop\\{propId}\\{stateId}";
                            downloadSuccess = await assetManager.DownloadAsset(fileLocation, mainControl.doDecryption);
                            if (downloadSuccess) mainControl.writeMessage($"Downloaded state '{stateId}' for prop '{propId}'.");
                        }
                    }
                    else
                    {
                        fileLocation = $"{themeId}\\prop\\{propId}";

                        downloadSuccess = await assetManager.DownloadAsset(fileLocation, mainControl.doDecryption);
                        if (downloadSuccess) mainControl.writeMessage($"Downloaded prop '{propId}'.");
                    }
                    mainControl.incrementProgressBar();
                }


                var effects = xmlDoc.Elements("effect");
                mainControl.setProgressBarMaximum(effects.Count());
                mainControl.resetProgressBar();
                foreach (var effect in effects)
                {
                    var effectId = effect.Attributes().Where(a => a.Name == "id").Single().Value;
                    if (effectId.Contains(".swf"))
                    {
                        fileLocation = $"{themeId}\\effect\\{effectId}";
                        downloadSuccess = await assetManager.DownloadAsset(fileLocation, mainControl.doDecryption);
                        if (downloadSuccess) mainControl.writeMessage($"Downloaded effect '{effectId}'.");
                    }
                    mainControl.incrementProgressBar();
                }

                var backgrounds = xmlDoc.Elements("background");
                mainControl.setProgressBarMaximum(backgrounds.Count());
                mainControl.resetProgressBar();
                foreach (var background in backgrounds)
                {
                    var bgId = background.Attributes().Where(a => a.Name == "id").Single().Value;
                    fileLocation = $"{themeId}\\bg\\{bgId}";

                    downloadSuccess = await assetManager.DownloadAsset(fileLocation, mainControl.doDecryption);
                    if (downloadSuccess) mainControl.writeMessage($"Downloaded background '{bgId}'.");
                    mainControl.incrementProgressBar();
                }

                var chars = xmlDoc.Elements("char");
                mainControl.setProgressBarMaximum(chars.Count());
                mainControl.resetProgressBar();
                foreach (var character in chars)
                {
                    var charId = character.Attributes().Where(a => a.Name == "id").Single().Value;
                    var actions = character.Descendants("action").Concat(character.Descendants("motion"));
                    var facials = character.Descendants("facial");
                    var libs = character.Descendants("library");
                    mainControl.writeMessage($"Starting on new character ({charId}).");

                    foreach (var action in actions)
                    {
                        var actionId = action.Attributes().Where(a => a.Name == "id").Single().Value;
                        fileLocation = $"{themeId}\\char\\{charId}\\{actionId}";

                        downloadSuccess = await assetManager.DownloadAsset(fileLocation, mainControl.doDecryption);
                        if (downloadSuccess) mainControl.writeMessage($"Downloaded action '{actionId}' for character '{charId}'.");
                    }

                    foreach (var facial in facials)
                    {
                        var facialId = facial.Attributes().Where(a => a.Name == "id").Single().Value;
                        fileLocation = $"{themeId}\\char\\{charId}\\head\\{facialId}";

                        downloadSuccess = await assetManager.DownloadAsset(fileLocation, mainControl.doDecryption);

                        if (downloadSuccess) mainControl.writeMessage($"Downloaded facial animation '{facialId}' for character '{charId}'.");
                    }

                    foreach (var lib in libs)
                    {
                        var libPath = lib.Attributes().Where(a => a.Name == "path").Single().Value;
                        var libType = lib.Attributes().Where(a => a.Name == "type").Single().Value;
                        if (libType == "hands")
                        {
                            fileLocation = $"{themeId}\\charparts\\{libType}\\{libPath}.swf";

                            await assetManager.DownloadAsset(fileLocation, false);

                            //Console.WriteLine($"Downloaded {actionId} for {charId}!");
                            if (downloadSuccess) mainControl.writeMessage($"Downloaded charpart '{libPath}.swf' (hands).");
                        }
                        else
                        {
                            fileLocation = $"{themeId}\\charparts\\{libType}\\{libPath}\\";

                            //await assetManager.DownloadAsset(localFileName, uri, false, true);

                            //what in the chungus??? why so much hardcoding??
                            await assetManager.DownloadAsset(fileLocation + "talk.swf", mainControl.doDecryption);
                            await assetManager.DownloadAsset(fileLocation + "talk_sync.swf", mainControl.doDecryption);
                            await assetManager.DownloadAsset(fileLocation + "talk_happy.swf", mainControl.doDecryption);
                            await assetManager.DownloadAsset(fileLocation + "talk_happy_sync.swf", mainControl.doDecryption);
                            await assetManager.DownloadAsset(fileLocation + "talk_sad.swf", mainControl.doDecryption);
                            await assetManager.DownloadAsset(fileLocation + "talk_sad_sync.swf", mainControl.doDecryption);
                            if (downloadSuccess) mainControl.writeMessage($"Downloaded charpart '{libPath}' (all mouth states).");

                        }

                    }
                    mainControl.incrementProgressBar();



                }
            }

            var sounds = xmlDoc.Elements("sound");
            mainControl.setProgressBarMaximum(sounds.Count());
            mainControl.resetProgressBar();
            foreach (var sound in sounds)
            {
                var variants = sound.Descendants("variation");
                var soundId = sound.Attributes().Where(a => a.Name == "id").Single().Value;

                fileLocation = $"{themeId}\\sound\\{soundId}";

                await assetManager.DownloadAsset(fileLocation, (mainControl.doDecryption && soundId.Contains(".swf")));
                if (downloadSuccess) mainControl.writeMessage($"Downloaded sound '{soundId}'.");
                foreach (var variant in variants)
                {
                    soundId = variant.Attributes().Where(a => a.Name == "id").Single().Value;
                    fileLocation = $"{themeId}\\sound\\{soundId}";

                    downloadSuccess = await assetManager.DownloadAsset(fileLocation, (mainControl.doDecryption && soundId.Contains(".swf")));
                    if (downloadSuccess) mainControl.writeMessage($"Downloaded sound varriant '{soundId}'.");
                }
                mainControl.incrementProgressBar();
            }


            var widgets = xmlDoc.Elements("widget");
            mainControl.setProgressBarMaximum(widgets.Count());
            mainControl.resetProgressBar();
            foreach (var widget in widgets)
            {
                var widgetThumb = widget.Attributes().Where(a => a.Name == "thumb").Single().Value;
                fileLocation = $"{themeId}\\widget\\{widgetThumb}";

                await assetManager.DownloadAsset(fileLocation, false);
                if (downloadSuccess) mainControl.writeMessage($"Downloaded widget thumbnail '{widgetThumb}'.");
                mainControl.incrementProgressBar();
            }

            var flows = xmlDoc.Elements("flow");
            mainControl.setProgressBarMaximum(flows.Count());
            mainControl.resetProgressBar();
            foreach (var flow in flows)
            {
                var flowId = flow.Attributes().Where(a => a.Name == "id").Single().Value;
                var flowThumb = flow.Attributes().Where(a => a.Name == "thumb").Single().Value;

                fileLocation = $"{themeId}\\flow\\";
                await assetManager.DownloadAsset(fileLocation + flowId, mainControl.doDecryption);
                downloadSuccess = await assetManager.DownloadAsset(fileLocation + flowThumb, false);


                if (downloadSuccess) mainControl.writeMessage($"Downloaded flow frame '{flowId}'.");
                mainControl.incrementProgressBar();

            }

            if (!mainControl.skipNonFlash)
            {
                var backgroundsthumb = xmlDoc.Elements("compositebg");
                mainControl.setProgressBarMaximum(backgroundsthumb.Count());
                mainControl.resetProgressBar();
                foreach (var compositebg in backgroundsthumb)
                {
                    var bgThumb = compositebg.Attributes().Where(a => a.Name == "thumb").Single().Value;
                    fileLocation = $"{themeId}\\bg\\{bgThumb}";

                    await assetManager.DownloadAsset(fileLocation, false);
                    if (downloadSuccess) mainControl.writeMessage($"Downloaded background thumbnail '{bgThumb}'.");
                    mainControl.incrementProgressBar();
                }

                var starters = xmlDoc.Elements("starter");
                mainControl.setProgressBarMaximum(starters.Count());
                mainControl.resetProgressBar();
                foreach (var starter in starters)
                {
                    var starterThumb = starter.Attributes().Where(a => a.Name == "thumbnail").Single().Value;
                    var starterTitle = starter.Attributes().Where(a => a.Name == "title").Single().Value;

                    var dir = $".\\{themeId}\\META - Noncompliant\\Starter Thumbs";
                    Directory.CreateDirectory(dir);

                    downloadSuccess = await assetManager.DownloadAssetDirect(starterThumb, $"{dir}\\{starterThumb.Substring(starterThumb.LastIndexOf("/") + 1)}");
                    if (downloadSuccess) mainControl.writeMessage($"Downloaded starter img '{starterThumb.Substring(starterThumb.LastIndexOf("/") + 1)}'.");
                }

                var categories = xmlDoc.Elements("category");
                mainControl.setProgressBarMaximum(starters.Count());
                mainControl.resetProgressBar();
                foreach (var category in categories)
                {
                    var categoryThumb = category.Attributes().Where(a => a.Name == "thumbnail").Single().Value;

                    var dir = $".\\{themeId}\\META - Noncompliant\\Category Thumbs";
                    Directory.CreateDirectory(dir);

                    downloadSuccess = await assetManager.DownloadAssetDirect(categoryThumb, $"{dir}\\{categoryThumb.Substring(categoryThumb.LastIndexOf("/") + 1)}");
                    if (downloadSuccess) mainControl.writeMessage($"Downloaded category img '{categoryThumb.Substring(categoryThumb.LastIndexOf("/") + 1)}'.");
                }
            }

            endRipper();
            return;
        }
    }
}
