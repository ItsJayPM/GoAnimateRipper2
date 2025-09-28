using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace GATOOLS
{
    public partial class Form1 : Form
    {
        //expose these to the whole class for new feature (and just because it's better)
        byte[] key;
        bool doDecryption;
        bool autoMode;
        HttpClient httpClient = new HttpClient();

        public static byte[] Decrypt(byte[] pwd, byte[] data)
        {
            int a, i, j, k, tmp;
            int[] key, box;
            byte[] cipher;

            key = new int[256];
            box = new int[256];
            cipher = new byte[data.Length];

            for (i = 0; i < 256; i++)
            {
                key[i] = pwd[i % pwd.Length];
                box[i] = i;
            }
            for (j = i = 0; i < 256; i++)
            {
                j = (j + box[i] + key[i]) % 256;
                tmp = box[i];
                box[i] = box[j];
                box[j] = tmp;
            }
            for (a = j = i = 0; i < data.Length; i++)
            {
                a++;
                a %= 256;
                j += box[a];
                j %= 256;
                tmp = box[a];
                box[a] = box[j];
                box[j] = tmp;
                k = box[((box[a] + box[j]) % 256)];
                cipher[i] = (byte)(data[i] ^ k);
            }
            return cipher;
        }

        public async Task RunFFDec()
        {
                Directory.CreateDirectory($".\\ffdec_output");
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                String cmd = $"ffdec.bat -export fla \"{System.AppContext.BaseDirectory}ffdec_output\"  \"{System.AppContext.BaseDirectory}ffdec_working\"";
                Console.WriteLine(cmd);
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo("cmd", "/c " + cmd);
                if (hideCmd.Checked) startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                startInfo.FileName = "cmd.exe";
                startInfo.WorkingDirectory = @"C:\Program Files (x86)\FFDec";
                Console.WriteLine(startInfo.Arguments);
                process.StartInfo = startInfo;
                process.Start();
                process.WaitForExit();
                if (deleteAfter.Checked)
                {
                    try
                    {
                        Directory.Delete($".\\ffdec_working", true);
                    }
                    catch
                    {
                        log.Text = "Dir in use; Skipping deletion...";
                    }
                }
        }
        public async Task DownloadAsset(string localFileName, string uriDownload, bool decrypt, bool isSwf)
        {
            //If this file already is ripped and the rip redundant flag is off, exit this function
            if (ffdecEnabled.Checked && !isSwf && doDecryption)
            {
                return;
            }
            if (!ffdecEnabled.Checked && File.Exists(localFileName) && !ripRedundant.Checked)
            {
                return;
            }    

            using (var response = await httpClient.GetAsync(uriDownload))
            {
                var data = await response.Content.ReadAsByteArrayAsync();
                bool succeeded = true;

                // If auto mode, try and figure out the key.
                if (decrypt && autoMode && isSwf)
                {
                    succeeded = DetermineKey(data);
                }
                // If the key was found, decrypt. If it isn't auto mode, it will always work.
                if (decrypt && doDecryption && succeeded && isSwf)
                {
                    data = Decrypt(key, data);
                }
                // If re-encrypt is checked and everything succeeded, do re-encryption.
                if (reEncEnabled.Checked && decrypt && succeeded && isSwf)
                {
                    data = Decrypt(Encoding.ASCII.GetBytes($"{reEncryptKey.Text}"), data);
                }

                if (ffdecEnabled.Checked && doDecryption)
                {
                    String[] b = localFileName.Split('\\');
                    String dir = $".\\\\ffdec_working";
                    Directory.CreateDirectory(dir);
                    dir = $".\\\\ffdec_working\\" + b[b.Length - 3] +"_" + b[b.Length - 2] + "_" + b[b.Length - 1];

                    File.WriteAllBytes(dir, data);
                }
                else
                {
                    File.WriteAllBytes(localFileName, data);
                }

                return;
            }
        }

        public async Task StartProceedure()
        {
            if (ffdecEnabled.Checked)
            {
                try
                {
                    Directory.Delete($".\\ffdec_working", true);
                }
                catch
                {
                    
                }
            }
            if (themeCheck.Checked)
            {
                await GoAnimateRip(false);
            }
            else if (themeCCCheck.Checked)
            {
                await GoAnimateRip(true);
            }
            else if (CCCheck.Checked)
            {
                await GoAnimateCCRip(null);
            }
        }
        public void LockControl()
        {
            ripButton.Enabled = false;
            ripButton.Text = "Working...";
            encryptKey.Enabled = false;
            reEncEnabled.Enabled = false;
            reEncryptKey.Enabled = false;
            decEnabled.Enabled = false;
            themeId.Enabled = false;
            ffdecEnabled.Enabled = false;
            deleteAfter.Enabled = false;
            hideCmd.Enabled = false;
            ripRedundant.Enabled = false;
            domain.Enabled = false;
        }

        //Lifted more or less from GoAnimate itself
        private bool IsFlashPrefix(byte[] data)
        {
            string prefix = System.Text.Encoding.UTF8.GetString(data).Substring(0, 3);
            return prefix == "CWS" || prefix == "FWS";
        }


        public bool DetermineKey(byte[] data)
        {
            if (!IsFlashPrefix(data))
            {
                foreach (string tkey in encryptKey.Items)
                {
                    key = Encoding.ASCII.GetBytes(tkey);
                    if (IsFlashPrefix(Decrypt(key, data)))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public void ReturnWithMessage(String mes)
        {
            log.Text = mes;
            ripButton.Enabled = true;
            ripButton.Text = "Start Ripping";
            ffdecEnabled.Enabled = true;
            deleteAfter.Enabled = ffdecEnabled.Checked;
            hideCmd.Enabled = ffdecEnabled.Checked;
            encryptKey.Enabled = !ffdecEnabled.Checked;
            reEncEnabled.Enabled = !ffdecEnabled.Checked && decEnabled.Checked;
            reEncryptKey.Enabled = !ffdecEnabled.Checked;
            decEnabled.Enabled = !ffdecEnabled.Checked;
            themeId.Enabled = true;
            ripRedundant.Enabled = true;
            domain.Enabled = true;
            return;
        }
        public async Task GoAnimateCCRip(String carryThemeId)
        {
            LockControl();
            string serverAddress = domain.Text;
            if (serverAddress.Substring(serverAddress.Length - 1) != "/") serverAddress += "/";
            serverAddress = domain.Text + "cc_store/";
            HttpClient httpClient = new HttpClient();
            string themeId = carryThemeId != null ? carryThemeId : this.themeId.Text;
            string uri = $"{serverAddress}{themeId}/cc_theme.xml";
            string localFileName = $".\\cc_store\\{themeId}\\cc_theme.xml";
            string dir = $".\\cc_store\\{themeId}\\";
            Directory.CreateDirectory(dir);
            XElement xmlDoc = null;
            doDecryption = false;

            await DownloadAsset(localFileName, uri, false, false);
            try
            {
                xmlDoc = XElement.Load(localFileName);
            }
            catch
            {
                ReturnWithMessage("An error occured parsing the cc_theme.xml file! (Are you sure the theme you typed exists?)");
                return;
            }
            doDecryption = decEnabled.Checked;
            if (encryptKey.Text != "(auto)")
            {
                autoMode = false;
                key = Encoding.ASCII.GetBytes($"{encryptKey.Text}");
            }
            else
            {
                autoMode = true;
            }
            var components = xmlDoc.Elements("component");
            //Reworked bar that actually isn't shite
            duration.Maximum = components.Count();
            duration.Value = 0;
            foreach (var component in components)
            {
                string componentId = component.Attributes().Where(a => a.Name == "id").Single().Value;
                string componentType = component.Attributes().Where(a => a.Name == "type").Single().Value;
                string componentThumb = component.Attributes().Where(a => a.Name == "thumb").Single().Value;
                var states = component.Elements("state");
                uri = $"{serverAddress}{themeId}/{componentType}/{componentId}/";
                string localDir = $".\\cc_store\\{themeId}\\{componentType}\\{componentId}";
                Directory.CreateDirectory(localDir);

                localFileName = $".\\cc_store\\{themeId}\\{componentType}\\{componentId}\\";
                if (componentType == "mouth")
                {
                    if (themeId == "family")
                    {
                        await DownloadAsset(localFileName + "talk_sad_sync.swf", uri + "talk_sad_sync.swf", doDecryption, true);
                        log.Text = $"Downloaded state 'talk_sad_sync.swf' for component '{componentId}' ({componentType}).";
                        await DownloadAsset(localFileName + "talk_happy_sync.swf", uri + "talk_happy_sync.swf", doDecryption, true);
                        log.Text = $"Downloaded state 'talk_happy_sync.swf' for component '{componentId}' ({componentType}).";
                        await DownloadAsset(localFileName + "talk_angry_sync.swf", uri + "talk_angry_sync.swf", doDecryption, true);
                        log.Text = $"Downloaded state 'talk_angry_sync.swf' for component '{componentId}' ({componentType}).";

                    }
                    if (themeId == "anime" || themeId == "ninjaanime" || themeId == "spacecitizen") //wack
                    {
                        await DownloadAsset(localFileName + "side_talk_sync.swf", uri + "side_talk_sync.swf", doDecryption, true);
                        log.Text = $"Downloaded state 'side_talk_sync.swf' for component '{componentId}' ({componentType}).";
                    }
                    else
                    {
                        await DownloadAsset(localFileName + "talk.swf", uri + "talk.swf", doDecryption, true);
                        log.Text = $"Downloaded state 'talk.swf' for component '{componentId}' ({componentType}).";
                        await DownloadAsset(localFileName + "talk_sync.swf", uri + "talk_sync.swf", doDecryption, true);
                        log.Text = $"Downloaded state 'talk_sync.swf' for component '{componentId}' ({componentType}).";
                    }
                }
                await DownloadAsset(localFileName + componentThumb, uri + componentThumb, doDecryption, true);
                log.Text = $"Downloaded thumbnail for component '{componentId}' ({componentType}).";

                foreach (var state in states)
                {

                    var stateId = state.Attributes().Where(a => a.Name == "id").Single().Value;
                    var stateFilename = state.Attributes().Where(a => a.Name == "filename").Single().Value;

                    uri = $"{serverAddress}{themeId}/{componentType}/{componentId}/{stateFilename}";
                    localDir = $".\\cc_store\\{themeId}\\{componentType}\\{componentId}";
                    Directory.CreateDirectory(localDir);

                    localFileName = $".\\cc_store\\{themeId}\\{componentType}\\{componentId}\\{stateFilename}";
                    await DownloadAsset(localFileName, uri, doDecryption, true);

                    log.Text = $"Downloaded state '{stateId}' for component '{componentId}' ({componentType}).";
                }
                duration.Value++;
            }
            var bodyshapes = xmlDoc.Elements("bodyshape");
            foreach (var bodyshape in bodyshapes)
            {
                var bodyId = bodyshape.Attributes().Where(a => a.Name == "id").Single().Value;

                var actionpacks = bodyshape.Elements("actionpack");
                var libraries = bodyshape.Elements("library");
                duration.Maximum = libraries.Count();
                duration.Value = 0;
                foreach (var library in libraries)
                {
                    var libraryType = library.Attributes().Where(a => a.Name == "type").Single().Value;
                    var libraryId = library.Attributes().Where(a => a.Name == "path").Single().Value;
                    var libraryThumb = library.Attributes().Where(a => a.Name == "thumb").Single().Value;
                    uri = $"{serverAddress}{themeId}/{libraryType}/";
                    var localDir = $".\\cc_store\\{themeId}\\{libraryType}\\";
                    Directory.CreateDirectory(localDir);

                    localFileName = $".\\cc_store\\{themeId}\\{libraryType}\\";

                    await DownloadAsset(localFileName + libraryId + ".swf", uri + libraryId + ".swf", false, true);
                    log.Text = $"Downloaded library '{libraryId}.swf' ({libraryType}).";
                    if (libraryThumb != libraryId + ".swf")
                    {
                        await DownloadAsset(localFileName + libraryThumb, uri + libraryThumb, doDecryption, true);
                        log.Text = $"Downloaded library thumb '{libraryThumb}' ({libraryType}).";
                    }
                    duration.Value++;
                }
                foreach (var actionpack in actionpacks)
                {
                    var actionpackName = actionpack.Attributes().Where(a => a.Name == "id").Single().Value;
                    var actions = actionpack.Elements("action");
                    duration.Maximum = actions.Count();
                    duration.Value = 0;
                    foreach (var action in actions)
                    {
                        var actionId = action.Attributes().Where(a => a.Name == "id").Single().Value;
                        uri = $"{serverAddress}{themeId}/freeaction/{bodyId}/{actionId}.swf";

                        var localDir = $".\\cc_store\\{themeId}\\freeaction\\{bodyId}";
                        Directory.CreateDirectory(localDir);

                        localFileName = $".\\cc_store\\{themeId}\\freeaction\\{bodyId}\\{actionId}.swf";
                        //Console.WriteLine(localDir + "," + localFileName);
                        await DownloadAsset(localFileName, uri, doDecryption, true);
                        log.Text = $"Downloaded freeaction '{actionId}' for bodytype '{bodyId}' (actionpack {actionpackName}).";
                        duration.Value++;
                    }
                }
                components = bodyshape.Elements("component");
                duration.Maximum = components.Count();
                duration.Value = 0;
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
                    var localDir = $".\\cc_store\\{themeId}\\{componentType}\\{componentId}";
                    Directory.CreateDirectory(localDir);

                    //This is a small thing but it created a HUGE bug in one specific case lemme tell you
                    if (componentType != "skeleton")
                    {
                        uri = $"{serverAddress}{themeId}/{componentType}/{componentId}/{componentThumb}";
                        localFileName = $".\\cc_store\\{themeId}\\{componentType}\\{componentId}\\{componentThumb}";
                        await DownloadAsset(localFileName, uri, doDecryption, true);
                        log.Text = $"Downloaded state '{componentThumb}' for component '{componentId}' ({componentType}).";
                    }

                    foreach (var state in states)
                    {

                        var stateId = state.Attributes().Where(a => a.Name == "id").Single().Value;

                        uri = $"{serverAddress}{themeId}/{componentType}/{componentId}/{stateId}.swf";

                        localFileName = $".\\cc_store\\{themeId}\\{componentType}\\{componentId}\\{stateId}.swf";
                        await DownloadAsset(localFileName, uri, doDecryption, true);

                        log.Text = $"Downloaded state '{stateId}' for component '{componentId}' ({componentType}).";
                    }
                    duration.Value++;
                }

            }
            if (ffdecEnabled.Checked)
            {
                log.Text = "Passing results to FFDec...";
                await RunFFDec();
            }
            if (carryThemeId == null)
            {
                ReturnWithMessage("The proceedure has completed.");
            }
            return;
        }

        public async Task GoAnimateRip(bool lookForCCTheme)
        {
            //get user input
            var serverAddress = domain.Text;
            if (serverAddress.Substring(serverAddress.Length - 1) != "/") serverAddress += "/";
            var themeId = this.themeId.Text;
            LockControl();

            //Download and load xml
            var uri = $"{serverAddress}{themeId}/theme.xml";
            var localFileName = $".\\{themeId}\\theme.xml";
            var dir = $".\\{themeId}\\";
            String ccThemeRefrence = null;
            Directory.CreateDirectory(dir);
            XElement xmlDoc = null;
            doDecryption = false;

            await DownloadAsset(localFileName, uri, false, false);
            try
            {
                xmlDoc = XElement.Load(localFileName);
            }
            catch
            {
                ReturnWithMessage("An error occured parsing the theme.xml file! (Are you sure the theme you typed exists?)");
                return;
            }
            if (lookForCCTheme)
            {
                ccThemeRefrence = xmlDoc.Attributes().Where(a => a.Name == "cc_theme_id").Single().Value;
            }
            doDecryption = decEnabled.Checked;
            if (encryptKey.Text != "(auto)")
            {
                autoMode = false;
                key = Encoding.ASCII.GetBytes($"{encryptKey.Text}");
            }
            else
            {
                autoMode = true;
            }
            var props = xmlDoc.Elements("prop");
            //Reworked bar that actually isn't shite
            duration.Maximum = props.Count();
            duration.Value = 0;
            foreach (var prop in props)
            {

                var propId = prop.Attributes().Where(a => a.Name == "id").Single().Value;
                var states = prop.Elements("state");
                if (states.Count() > 0)
                {
                    foreach (var state in states)
                    {
                        var stateId = state.Attributes().Where(a => a.Name == "id").Single().Value;

                        uri = $"{serverAddress}{themeId}/prop/{propId}/{stateId}";
                        var localDir = $".\\{themeId}\\prop\\{propId}";
                        Directory.CreateDirectory(localDir);

                        localFileName = $".\\{themeId}\\prop\\{propId}\\{stateId}";
                        await DownloadAsset(localFileName, uri, doDecryption, true);

                        //Console.WriteLine($"Downloaded {stateId} for {propId}!");
                        log.Text = $"Downloaded state '{stateId}' for prop '{propId}'.";
                    }
                }
                else
                {
                    uri = $"{serverAddress}{themeId}/prop/{propId}";


                    var localDir = $".\\{themeId}\\prop";
                    Directory.CreateDirectory(localDir);

                    localFileName = $".\\{themeId}\\prop\\{propId}";

                    await DownloadAsset(localFileName, uri, doDecryption, true);
                    log.Text = $"Downloaded prop '{propId}'.";
                }
                duration.Value++;
            }


            var effects = xmlDoc.Elements("effect");
            duration.Maximum = effects.Count();
            duration.Value = 0;
            foreach (var effect in effects)
            {
                var effectId = effect.Attributes().Where(a => a.Name == "id").Single().Value;
                if (effectId.Contains(".swf"))
                {
                    uri = $"{serverAddress}{themeId}/effect/{effectId}";


                    var localDir = $".\\{themeId}\\effect";
                    Directory.CreateDirectory(localDir);

                    localFileName = $".\\{themeId}\\effect\\{effectId}";

                    await DownloadAsset(localFileName, uri, doDecryption, true);
                }
                log.Text = $"Downloaded effect '{effectId}'.";
                duration.Value++;
            }

            var backgroundsthumb = xmlDoc.Elements("compositebg");
            duration.Maximum = backgroundsthumb.Count();
            duration.Value = 0;
            foreach (var compositebg in backgroundsthumb)
            {
                var bgThumb = compositebg.Attributes().Where(a => a.Name == "thumb").Single().Value;
                uri = $"{serverAddress}{themeId}/bg/{bgThumb}";


                var localDir = $".\\{themeId}\\bg";
                Directory.CreateDirectory(localDir);

                localFileName = $".\\{themeId}\\bg\\{bgThumb}";

                await DownloadAsset(localFileName, uri, false, false);
                log.Text = $"Downloaded background thumbnail '{bgThumb}'.";
                duration.Value++;
            }

            var backgrounds = xmlDoc.Elements("background");
            duration.Maximum = backgrounds.Count();
            duration.Value = 0;
            foreach (var background in backgrounds)
            {
                var bgId = background.Attributes().Where(a => a.Name == "id").Single().Value;
                uri = $"{serverAddress}{themeId}/bg/{bgId}";


                var localDir = $".\\{themeId}\\bg";
                Directory.CreateDirectory(localDir);

                localFileName = $".\\{themeId}\\bg\\{bgId}";

                await DownloadAsset(localFileName, uri, doDecryption, true);
                log.Text = $"Downloaded background '{bgId}'.";
                duration.Value++;
            }

            var sounds = xmlDoc.Elements("sound");
            duration.Maximum = sounds.Count();
            duration.Value = 0;
            foreach (var sound in sounds)
            {
                var variants = sound.Descendants("variation");
                var soundId = sound.Attributes().Where(a => a.Name == "id").Single().Value;
                uri = $"{serverAddress}{themeId}/sound/{soundId}";


                var localDir = $".\\{themeId}\\sound";
                Directory.CreateDirectory(localDir);

                localFileName = $".\\{themeId}\\sound\\{soundId}";

                await DownloadAsset(localFileName, uri, (doDecryption && soundId.Contains(".swf")), soundId.Contains(".swf"));
                log.Text = $"Downloaded sound '{soundId}'.";
                foreach (var variant in variants)
                {
                    soundId = variant.Attributes().Where(a => a.Name == "id").Single().Value;
                    uri = $"{serverAddress}{themeId}/sound/{soundId}";

                    localDir = $".\\{themeId}\\sound";
                    localFileName = $".\\{themeId}\\sound\\{soundId}";

                    await DownloadAsset(localFileName, uri, (doDecryption && soundId.Contains(".swf")), soundId.Contains(".swf"));
                    log.Text = $"Downloaded sound varriant '{soundId}'.";
                }
                duration.Value++;
            }

            var chars = xmlDoc.Elements("char");
            duration.Maximum = chars.Count();
            duration.Value = 0;
            foreach (var character in chars)
            {
                var charId = character.Attributes().Where(a => a.Name == "id").Single().Value;
                var actions = character.Descendants("action").Concat(character.Descendants("motion"));
                var facials = character.Descendants("facial");
                var libs = character.Descendants("library");
                log.Text = $"Starting on new character ({charId}).";

                foreach (var action in actions)
                {
                    var actionId = action.Attributes().Where(a => a.Name == "id").Single().Value;
                    uri = $"{serverAddress}{themeId}/char/{charId}/{actionId}";


                    var localDir = $".\\{themeId}\\char\\{charId}";
                    Directory.CreateDirectory(localDir);

                    localFileName = $".\\{themeId}\\char\\{charId}\\{actionId}";

                    await DownloadAsset(localFileName, uri, doDecryption, true);

                    //Console.WriteLine($"Downloaded {actionId} for {charId}!");
                    log.Text = $"Downloaded action '{actionId}' for character '{charId}'.";
                }

                foreach (var facial in facials)
                {
                    var facialId = facial.Attributes().Where(a => a.Name == "id").Single().Value;
                    uri = $"{serverAddress}{themeId}/char/{charId}/head/{facialId}";


                    var localDir = $".\\{themeId}\\char\\{charId}\\head";
                    Directory.CreateDirectory(localDir);

                    localFileName = $".\\{themeId}\\char\\{charId}\\head\\{facialId}";
                    //Console.WriteLine(localFileName);

                    await DownloadAsset(localFileName, uri, doDecryption, true);

                    //Console.WriteLine($"Downloaded {facialId} for {charId}!");
                    log.Text = $"Downloaded facial animation '{facialId}' for character '{charId}'.";
                }

                foreach (var lib in libs)
                {
                    var libPath = lib.Attributes().Where(a => a.Name == "path").Single().Value;
                    var libType = lib.Attributes().Where(a => a.Name == "type").Single().Value;
                    if (libType == "hands")
                    {
                        uri = $"{serverAddress}{themeId}/charparts/{libType}/{libPath}.swf";
                        var localDir = $".\\{themeId}\\charparts\\{libType}";
                        Directory.CreateDirectory(localDir);

                        localFileName = $".\\{themeId}\\charparts\\{libType}\\{libPath}.swf";

                        await DownloadAsset(localFileName, uri, false, true);

                        //Console.WriteLine($"Downloaded {actionId} for {charId}!");
                        log.Text = $"Downloaded charpart '{libPath}.swf' (hands).";
                    }
                    else
                    {

                        uri = $"{serverAddress}{themeId}/charparts/{libType}/{libPath}/";
                        var localDir = $".\\{themeId}\\charparts\\{libType}\\{libPath}\\";
                        Directory.CreateDirectory(localDir);

                        localFileName = $".\\{themeId}\\charparts\\{libType}\\{libPath}\\";

                        //await DownloadAsset(localFileName, uri, false, true);

                        //what in the chungus??? why so much hardcoding??
                        await DownloadAsset(localFileName + "talk.swf", uri + "talk.swf", doDecryption, true);
                        await DownloadAsset(localFileName + "talk_sync.swf", uri + "talk_sync.swf", doDecryption, true);
                        await DownloadAsset(localFileName + "talk_happy.swf", uri + "talk_happy.swf", doDecryption, true);
                        await DownloadAsset(localFileName + "talk_happy_sync.swf", uri + "talk_happy_sync.swf", doDecryption, true);
                        await DownloadAsset(localFileName + "talk_sad.swf", uri + "talk_sad.swf", doDecryption, true);
                        await DownloadAsset(localFileName + "talk_sad_sync.swf", uri + "talk_sad_sync.swf", doDecryption, true); 
                        log.Text = $"Downloaded charpart '{libPath}' (all mouth states).";

                    }

                }
                duration.Value++;


            }
            var widgets = xmlDoc.Elements("widget");
            duration.Maximum = widgets.Count();
            duration.Value = 0;
            foreach (var widget in widgets)
            {
                var widgetThumb = widget.Attributes().Where(a => a.Name == "thumb").Single().Value;
                uri = $"{serverAddress}{themeId}/widget/{widgetThumb}";


                var localDir = $".\\{themeId}\\widget\\";
                Directory.CreateDirectory(localDir);

                localFileName = $".\\{themeId}\\widget\\{widgetThumb}";

                await DownloadAsset(localFileName, uri, false, false);
                log.Text = $"Downloaded widget thumbnail '{widgetThumb}'.";
                duration.Value++;
            }

            var flows = xmlDoc.Elements("flow");
            duration.Maximum = flows.Count();
            duration.Value = 0;
            foreach (var flow in flows)
            {
                var flowId = flow.Attributes().Where(a => a.Name == "id").Single().Value;
                var flowThumb = flow.Attributes().Where(a => a.Name == "thumb").Single().Value;
                uri = $"{serverAddress}{themeId}/flow/";


                var localDir = $".\\{themeId}\\flow\\";
                Directory.CreateDirectory(localDir);

                await DownloadAsset(localDir + flowId, uri + flowId, doDecryption, true);
                await DownloadAsset(localDir + flowThumb, uri + flowThumb, false, false);


                log.Text = $"Downloaded downloaded flow frame '{flowId}'!";
                duration.Value++;

            }

            /*var starters = xmlDoc.Elements("starter");
            duration.Maximum = starters.Count();
            duration.Value = 0;
            foreach (var starter in starters)
            {
                var starterThumb = starter.Attributes().Where(a => a.Name == "thumbnail").Single().Value;

                var localDir = $".\\{themeId}\\META - Noncompliant\\Starter Thumbs";
                Directory.CreateDirectory(localDir);

                localFileName = $".\\{themeId}\\flow\\";

            }*/
            if (ffdecEnabled.Checked)
            {
                log.Text = "Passing results to FFDec...";
                await RunFFDec();
            }
            if (ccThemeRefrence != null)
            {
                await GoAnimateCCRip(ccThemeRefrence);
            }
            ReturnWithMessage("The proceedure has completed.");
        }

        public Form1()
        {
            InitializeComponent();
        }

        async private void button1_Click(object sender, EventArgs e)
        {
            ripButton.Enabled = false;
            await StartProceedure();
        }

        private void encrypt_SelectedIndexChanged(object sender, EventArgs e)
        {
            var key = encryptKey.SelectedItem;

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            reEncEnabled.Enabled = decEnabled.Checked;
        }

        private void dom_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void ThemeCCCheck_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void CCCheck_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void label2_Click_1(object sender, EventArgs e)
        {

        }

        private void JPEXStoggle_CheckedChanged(object sender, EventArgs e)
        {
            if (ffdecEnabled.Checked)
            {
                MessageBox.Show("I do not gurantee this feature will work for you, I wrote it so it would work on my computer but I don\'t know how well it\'ll work with yours. I think so long as you\'re on Windows and have JPEXS it should be fine but I\'m not really sure. Use at your own discretion, and just letting you know you probably shouldn\'t touch the JPEXS settings, they are for debugging and WILL make the process harder to follow or do tons of repeat work for no reason.", "I\'m warning you!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                decEnabled.Checked = true;
                ripRedundant.Checked = false;
                decEnabled.Enabled = false;
                encryptKey.Text = "(auto)";
                encryptKey.Enabled = false;
                reEncEnabled.Checked = false;
                reEncEnabled.Enabled = false;
                deleteAfter.Enabled = true;
                hideCmd.Enabled = true;
            }
            else
            {
                decEnabled.Enabled = true;
                encryptKey.Enabled = true;
                reEncEnabled.Enabled = true;
                deleteAfter.Enabled = false;
                hideCmd.Enabled = false;
            }

        }

        private void hideCMD_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
