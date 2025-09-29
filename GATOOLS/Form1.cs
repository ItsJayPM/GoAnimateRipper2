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
        List<string> pathes = new List<string>();

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

        /// <summary>
        /// Task <c>RunFFDec</c> handles the batch decompilation of assets using JPEXS.
        /// </summary>
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

                    try
                    {
                        Directory.Delete($".\\ffdec_working", true);
                    }
                    catch
                    {
                        log.Text = "Dir in use; Skipping deletion...";
                    }
        }

        /// <summary>
        /// void <c>GetPathAsUrl</c> returns path as full URL path.
        /// </summary>
        public String GetPathAsUrl(string path)
        {
            return domain.Text + path.Replace("\\","/");
        }
        /// <summary>
        /// void <c>GetLocalPath</c> returns path as full directory path.
        /// </summary>
        public String GetLocalPath(string path)
        {
            Directory.CreateDirectory(path.Substring(0, path.LastIndexOf("\\")));
            return ".\\" + path;
        }
        /// <summary>
        /// void <c>ReorganizeAfterFFDec</c> moves the JPEXS output back to their proper locations, then cleans vars.
        /// </summary>
        public void ReorganizeAfterFFDec()
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
                    File.Move(finalLocation+$"\\{i}.fla", finalLocation+$"\\{finalName}.fla");
                }
            }
            Directory.Delete($".\\ffdec_output", true);
            pathes.Clear();
        }
        /// <summary>
        /// Task <c>DownloadAsset</c> downloads the asset, and if applicable, decrypts, re-encrypts, and sets up for decompiling.
        /// </summary>
        /// (<paramref name="path"/>,<paramref name="isSupposedToHaveEncryption"/>)
        /// <param name="path">The path fragment. The function handles the local and download with the fragment.</param>
        /// <param name="isSupposedToHaveEncryption">If the file is SUPPOSED to have encryption; Only applicable to SWFs.</param>
        public async Task DownloadAsset(string path,  bool isSupposedToHaveEncryption)
        {
            //Setup Phase.
            
            bool isSwf = true;
            if (!path.ToLower().Contains(".swf"))
            {
                isSwf = false;
            }

            if (!ffdecEnabled.Checked && File.Exists(GetLocalPath(path)) && !ripRedundant.Checked)
            {
                return;
            }

            //Execution Phase.
            using (var response = await httpClient.GetAsync(GetPathAsUrl(path)))
            {
                var data = await response.Content.ReadAsByteArrayAsync();
                bool keySuccessfullyMatched = true;
                bool isFileEncrypted = !IsFlashPrefix(data);
                // If it's a SWF, deal with encryption jargon.
                if (isSwf)
                {

                    // If auto mode, try and figure out the key.
                    if (isSupposedToHaveEncryption && autoMode)
                    {
                        keySuccessfullyMatched = DetermineKey(data);
                    }
                    // If the key was found, decrypt. If it isn't auto mode, it will always work.
                    if (isSupposedToHaveEncryption && doDecryption && keySuccessfullyMatched)
                    {
                        data = Decrypt(key, data);
                    }
                    // If re-encrypt is checked and everything succeeded (or a specific edge case occured where auto mode was on and the file was supposed to be encrypted but wasn't), do re-encryption.
                    if (reEncEnabled.Checked && isSupposedToHaveEncryption && (keySuccessfullyMatched || !isFileEncrypted))
                    {
                        data = Decrypt(Encoding.ASCII.GetBytes($"{reEncryptKey.Text}"), data);
                    }
                }

                //Output phase.
                //If we're in FFDec mode, save the path for later, and place the file in the JPEXS target folder.
                if (ffdecEnabled.Checked && isSwf)
                {
                    if (pathes.Contains(GetLocalPath(path))) return; //Dupes CANNOT exist. It's kindof a whatev thing normally but in this context we need to make sure.
                    String dir = $".\\\\ffdec_working";
                    Directory.CreateDirectory(dir);
                    pathes.Add(GetLocalPath(path));
                    dir = $".\\\\ffdec_working\\" + (pathes.Count - 1) + ".swf";

                    File.WriteAllBytes(dir, data);
                }
                else
                {
                    File.WriteAllBytes(GetLocalPath(path), data);
                }

                return;
            }
        }

        /// <summary>
        /// void <c>StartProceedure</c> initiates the correct ripping process based on the selected settings.
        /// </summary>
        public async Task StartProceedure()
        {
            if (ffdecEnabled.Checked)
            {
                if (Directory.Exists($".\\ffdec_output")) Directory.Delete($".\\ffdec_output", true);
                if (Directory.Exists($".\\ffdec_working")) Directory.Delete($".\\ffdec_working", true);

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

        /// <summary>
        /// void <c>LockControl</c> locks the user interface.
        /// </summary>
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
        /// <summary>
        /// bool <c>IsFlashPrefix</c> returns if byte[] <c>data</c> has a valid SWF header.
        /// </summary>
        private bool IsFlashPrefix(byte[] data)
        {
            string prefix = System.Text.Encoding.UTF8.GetString(data).Substring(0, 3);
            return prefix == "CWS" || prefix == "FWS";
        }

        /// <summary>
        /// bool <c>DetermineKey</c> tries to determine the key. It returns true if it succeeds.
        /// </summary>
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

        /// <summary>
        /// void <c>ReturnWithMessage</c> returns user control and sends a message to the log.
        /// </summary>
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
            pathes = new List<string>();
            LockControl();
            string serverAddress = domain.Text;
            if (domain.Text.Substring(domain.Text.Length - 1) != "/") domain.Text += "/";
            serverAddress = domain.Text + "cc_store/";
            HttpClient httpClient = new HttpClient();
            string themeId = carryThemeId != null ? carryThemeId : this.themeId.Text;
            string fileLocation = $"cc_store\\{themeId}\\cc_theme.xml";
            if (Directory.Exists($".\\cc_store\\{themeId}\\") && ffdecEnabled.Checked) { Directory.Delete($".\\cc_store\\{themeId}\\", true); }
            XElement xmlDoc = null;
            doDecryption = false;

            await DownloadAsset(fileLocation, false);
            try
            {
                xmlDoc = XElement.Load(GetLocalPath(fileLocation));
            }
            catch
            {
                ReturnWithMessage("An error occured parsing the cc_theme.xml file! (Are you sure the theme you typed exists?)");
                Directory.Delete($".\\cc_store\\{themeId}\\", true);
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

                fileLocation = $"cc_store\\{themeId}\\{componentType}\\{componentId}";
                if (componentType == "mouth")
                {
                    if (themeId == "family")
                    {
                        await DownloadAsset($"{fileLocation}\\talk_sad_sync.swf", doDecryption);
                        log.Text = $"Downloaded state 'talk_sad_sync.swf' for component '{componentId}' ({componentType}).";
                        await DownloadAsset($"{fileLocation}\\talk_happy_sync.swf", doDecryption);
                        log.Text = $"Downloaded state 'talk_happy_sync.swf' for component '{componentId}' ({componentType}).";
                        await DownloadAsset($"{fileLocation}\\talk_angry_sync.swf", doDecryption);
                        log.Text = $"Downloaded state 'talk_angry_sync.swf' for component '{componentId}' ({componentType}).";

                    }
                    if (themeId == "anime" || themeId == "ninjaanime" || themeId == "spacecitizen") //wack
                    {
                        await DownloadAsset($"{fileLocation}\\side_talk_sync.swf", doDecryption);
                        log.Text = $"Downloaded state 'side_talk_sync.swf' for component '{componentId}' ({componentType}).";
                    }
                    else
                    {
                        await DownloadAsset($"{fileLocation}\\talk.swf", doDecryption);
                        log.Text = $"Downloaded state 'talk.swf' for component '{componentId}' ({componentType}).";
                        await DownloadAsset($"{fileLocation}\\talk_sync.swf", doDecryption);
                        log.Text = $"Downloaded state 'talk_sync.swf' for component '{componentId}' ({componentType}).";
                    }
                }
                await DownloadAsset(fileLocation + "\\" + componentThumb, doDecryption);
                log.Text = $"Downloaded thumbnail for component '{componentId}' ({componentType}).";

                foreach (var state in states)
                {

                    var stateId = state.Attributes().Where(a => a.Name == "id").Single().Value;
                    var stateFilename = state.Attributes().Where(a => a.Name == "filename").Single().Value;

                    fileLocation = $"cc_store\\{themeId}\\{componentType}\\{componentId}\\{stateFilename}";
                    await DownloadAsset(fileLocation, doDecryption);

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

                    fileLocation = $"cc_store\\{themeId}\\{libraryType}\\";

                    await DownloadAsset(fileLocation + libraryId + ".swf", false);
                    log.Text = $"Downloaded library '{libraryId}.swf' ({libraryType}).";
                    if (libraryThumb != libraryId + ".swf")
                    {
                        await DownloadAsset(fileLocation + libraryThumb, doDecryption);
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
                        fileLocation = $"cc_store\\{themeId}\\freeaction\\{bodyId}\\{actionId}.swf";
                        //Console.WriteLine(localDir + "," + localFileName);
                        await DownloadAsset(fileLocation, doDecryption);
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

                    //This is a small thing but it created a HUGE bug in one specific case lemme tell you
                    if (componentType != "skeleton")
                    {
                        fileLocation = $"cc_store\\{themeId}\\{componentType}\\{componentId}\\{componentThumb}";
                        await DownloadAsset(fileLocation, doDecryption);
                        log.Text = $"Downloaded state '{componentThumb}' for component '{componentId}' ({componentType}).";
                    }

                    foreach (var state in states)
                    {

                        var stateId = state.Attributes().Where(a => a.Name == "id").Single().Value;

                        fileLocation = $"cc_store\\{themeId}\\{componentType}\\{componentId}\\{stateId}.swf";
                        await DownloadAsset(fileLocation, doDecryption);

                        log.Text = $"Downloaded state '{stateId}' for component '{componentId}' ({componentType}).";
                    }
                    duration.Value++;
                }

            }
            if (ffdecEnabled.Checked)
            {
                log.Text = "Passing results to FFDec...";
                await RunFFDec();
                if (deleteAfter.Checked) ReorganizeAfterFFDec();
            }
            if (carryThemeId == null)
            {
                ReturnWithMessage("The proceedure has completed.");
            }
            return;
        }

        public async Task GoAnimateRip(bool lookForCCTheme)
        {
            pathes = new List<string>();
            //get user input
            if (domain.Text.Substring(domain.Text.Length - 1) != "/") domain.Text += "/";
            var themeId = this.themeId.Text;
            LockControl();

            //Download and load xml
            var fileLocation = $"{themeId}\\theme.xml";
            String ccThemeRefrence = null;
            if (Directory.Exists($".\\{themeId}\\") && ffdecEnabled.Checked) { Directory.Delete($".\\{themeId}\\", true); }
            XElement xmlDoc = null;
            doDecryption = false;

            await DownloadAsset(fileLocation, false);
            try
            {
                xmlDoc = XElement.Load(GetLocalPath(fileLocation));
            }
            catch
            {
                ReturnWithMessage("An error occured parsing the theme.xml file! (Are you sure the theme you typed exists?)");
                Directory.Delete($".\\{themeId}\\", true);
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

                        fileLocation = $"{themeId}\\prop\\{propId}\\{stateId}";
                        await DownloadAsset(fileLocation, doDecryption);
                        log.Text = $"Downloaded state '{stateId}' for prop '{propId}'.";
                    }
                }
                else
                {
                    fileLocation = $"{themeId}\\prop\\{propId}";

                    await DownloadAsset(fileLocation, doDecryption);
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
                    fileLocation = $"{themeId}\\effect\\{effectId}";
                    await DownloadAsset(fileLocation, doDecryption);
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
                fileLocation = $"{themeId}\\bg\\{bgThumb}";

                await DownloadAsset(fileLocation, false);
                log.Text = $"Downloaded background thumbnail '{bgThumb}'.";
                duration.Value++;
            }

            var backgrounds = xmlDoc.Elements("background");
            duration.Maximum = backgrounds.Count();
            duration.Value = 0;
            foreach (var background in backgrounds)
            {
                var bgId = background.Attributes().Where(a => a.Name == "id").Single().Value;
                fileLocation = $"{themeId}\\bg\\{bgId}";

                await DownloadAsset(fileLocation, doDecryption);
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

                fileLocation = $"{themeId}\\sound\\{soundId}";

                await DownloadAsset(fileLocation, (doDecryption && soundId.Contains(".swf")));
                log.Text = $"Downloaded sound '{soundId}'.";
                foreach (var variant in variants)
                {
                    soundId = variant.Attributes().Where(a => a.Name == "id").Single().Value;
                    fileLocation = $"{themeId}\\sound\\{soundId}";

                    await DownloadAsset(fileLocation, (doDecryption && soundId.Contains(".swf")));
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
                    fileLocation = $"{themeId}\\char\\{charId}\\{actionId}";

                    await DownloadAsset(fileLocation, doDecryption);
                    log.Text = $"Downloaded action '{actionId}' for character '{charId}'.";
                }

                foreach (var facial in facials)
                {
                    var facialId = facial.Attributes().Where(a => a.Name == "id").Single().Value;
                    fileLocation = $"{themeId}\\char\\{charId}\\head\\{facialId}";

                    await DownloadAsset(fileLocation, doDecryption);

                    log.Text = $"Downloaded facial animation '{facialId}' for character '{charId}'.";
                }

                foreach (var lib in libs)
                {
                    var libPath = lib.Attributes().Where(a => a.Name == "path").Single().Value;
                    var libType = lib.Attributes().Where(a => a.Name == "type").Single().Value;
                    if (libType == "hands")
                    {
                        fileLocation = $"{themeId}\\charparts\\{libType}\\{libPath}.swf";

                        await DownloadAsset(fileLocation, false);

                        //Console.WriteLine($"Downloaded {actionId} for {charId}!");
                        log.Text = $"Downloaded charpart '{libPath}.swf' (hands).";
                    }
                    else
                    {
                        fileLocation = $"{themeId}\\charparts\\{libType}\\{libPath}\\";

                        //await DownloadAsset(localFileName, uri, false, true);

                        //what in the chungus??? why so much hardcoding??
                        await DownloadAsset(fileLocation + "talk.swf", doDecryption);
                        await DownloadAsset(fileLocation + "talk_sync.swf", doDecryption);
                        await DownloadAsset(fileLocation + "talk_happy.swf", doDecryption);
                        await DownloadAsset(fileLocation + "talk_happy_sync.swf", doDecryption);
                        await DownloadAsset(fileLocation + "talk_sad.swf", doDecryption);
                        await DownloadAsset(fileLocation + "talk_sad_sync.swf", doDecryption); 
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
                fileLocation = $"{themeId}\\widget\\{widgetThumb}";

                await DownloadAsset(fileLocation, false);
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

                fileLocation = $"{themeId}\\flow\\";
                await DownloadAsset(fileLocation + flowId, doDecryption);
                await DownloadAsset(fileLocation + flowThumb, false);


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
                if (deleteAfter.Checked) ReorganizeAfterFFDec();
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
                if (!Directory.Exists("C:\\Program Files (x86)\\FFDec"))
                {
                    MessageBox.Show("JPEXS is either not installed, or is installed in a place other then where it is expected. The decompilation features will requires that JPEXS is in it's normal Windows install location. JPEXS mode will not be initialized.", "Could not find JPEXS!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    ffdecEnabled.Checked = false;
                    return;
                }
                MessageBox.Show("The other settings for JPEXS are PURELY for testing only. For your purposes, you\'ll likely want them set to the values they begin at.", "A quick disclaimer...", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
