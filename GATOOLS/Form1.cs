using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace GATOOLS
{
    public partial class Form1 : Form
    {
        //expose these to the whole class for new feature (and just because it's better)
        byte[] key;
        bool doDecryption;
        HttpClient httpClient = new HttpClient();
        int cycle = 0;

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
        public async Task DownloadAsset(string localFileName, string uriDownload, bool decrypt)
        {
            if (JPEXStoggle.Checked && File.Exists(localFileName.Replace(".swf", ".fla")))
            {
                Console.WriteLine(localFileName.Replace(".swf", ".fla"));
                return;
            }
            using (var response = await httpClient.GetAsync(uriDownload))
            {
                var data = await response.Content.ReadAsByteArrayAsync();
                if (decrypt && key == null) DetermineKey(data);
                if (decrypt && doDecryption)
                {
                    data = Decrypt(key, data);
                }

                if (reencrypt.Checked && decrypt)
                {
                    key = Encoding.ASCII.GetBytes($"{key2.Text}");
                    data = Decrypt(key, data);
                }

                File.WriteAllBytes(localFileName, data);

                if (JPEXStoggle.Checked && decrypt)
                {
                    System.Diagnostics.Process process = new System.Diagnostics.Process();
                    var cmd = $"ffdec.bat -export fla \"{System.AppContext.BaseDirectory + localFileName.Substring(2, localFileName.LastIndexOf("\\") - 2)}\"  \"{System.AppContext.BaseDirectory + localFileName.Substring(2)}\"";
                    System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo("cmd", "/c " + cmd);
                    if (hideCMD.Checked)startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
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
                            File.Delete(localFileName);
                        }
                        catch
                        {
                            log.Text = "File in use; Skipping deletion...";
                        }
                    }
                }

                return;
            }
        }

        public async Task StartProceedure()
        {
            if (ThemeCheck.Checked)
            {
                await GoAnimateRip(false);
            }
            else if (ThemeCCCheck.Checked)
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
            button1.Enabled = false;
            button1.Text = "Working...";
            encrypt.Enabled = false;
            reencrypt.Enabled = false;
            key2.Enabled = false;
            checkBox1.Enabled = false;
            tid.Enabled = false;
            dom.Enabled = false;
        }

        //Lifted more or less from GoAnimate itself
        private bool IsFlashPrefix(byte[] data)
		{
			string prefix = System.Text.Encoding.UTF8.GetString(data).Substring(0,3);
			return prefix == "CWS" || prefix == "FWS";
		}


        public void DetermineKey(byte[] data)
        {
            if (!IsFlashPrefix(data))
            {
                foreach (string tkey in encrypt.Items)
                {
                    key = Encoding.ASCII.GetBytes(tkey);
                    if (IsFlashPrefix(Decrypt(key, data)))
                    {
                        return;
                    }
                }
            }
            doDecryption = false;
        }
        public void ReturnWithMessage(String mes)
        {
            log.Text = mes;
            button1.Enabled = true;
            button1.Text = "Start Ripping";
            JPEXStoggle.Enabled = true;
            deleteAfter.Enabled = JPEXStoggle.Checked;
            hideCMD.Enabled = JPEXStoggle.Checked;
            encrypt.Enabled = !JPEXStoggle.Checked;
            reencrypt.Enabled = !JPEXStoggle.Checked && checkBox1.Checked;
            key2.Enabled = !JPEXStoggle.Checked;
            checkBox1.Enabled = !JPEXStoggle.Checked;
            tid.Enabled = true;
            dom.Enabled = true;
            return;
        }
        public async Task GoAnimateCCRip(String carryThemeId)
        {
            LockControl();
            string serverAddress = dom.Text;
            if (serverAddress.Substring(serverAddress.Length - 1) != "/") serverAddress += "/";
            serverAddress = dom.Text + "cc_store/";
            HttpClient httpClient = new HttpClient();
            string themeId = carryThemeId != null ? carryThemeId : tid.Text;
            doDecryption = checkBox1.Checked;
            if (encrypt.Text != "(auto)") key = Encoding.ASCII.GetBytes($"{encrypt.Text}");
            string uri = $"{serverAddress}{themeId}/cc_theme.xml";
            string localFileName = $".\\cc_store\\{themeId}\\cc_theme.xml";
            string dir = $".\\cc_store\\{themeId}\\";
            Directory.CreateDirectory(dir);
            XElement xmlDoc = null;

            await DownloadAsset(localFileName, uri, false);
            try
            {
                xmlDoc = XElement.Load(localFileName);
            }
            catch
            {
                ReturnWithMessage("An error occured parsing the cc_theme.xml file! (Are you sure the theme you typed exists?)");
                return;
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
                        await DownloadAsset(localFileName + "talk_sad_sync.swf", uri + "talk_sad_sync.swf", doDecryption);
                        log.Text = $"Downloaded state 'talk_sad_sync.swf' for component '{componentId}' ({componentType}).";
                        await DownloadAsset(localFileName + "talk_happy_sync.swf", uri + "talk_happy_sync.swf", doDecryption);
                        log.Text = $"Downloaded state 'talk_happy_sync.swf' for component '{componentId}' ({componentType}).";
                        await DownloadAsset(localFileName + "talk_angry_sync.swf", uri + "talk_angry_sync.swf", doDecryption);
                        log.Text = $"Downloaded state 'talk_angry_sync.swf' for component '{componentId}' ({componentType}).";

                    }
                    if (themeId == "anime" || themeId == "ninjaanime" || themeId == "spacecitizen") //wack
                    {
                        await DownloadAsset(localFileName + "side_talk_sync.swf", uri + "side_talk_sync.swf", doDecryption);
                        log.Text = $"Downloaded state 'side_talk_sync.swf' for component '{componentId}' ({componentType}).";
                    }
                    else
                    {
                        await DownloadAsset(localFileName + "talk.swf", uri + "talk.swf", doDecryption);
                        log.Text = $"Downloaded state 'talk.swf' for component '{componentId}' ({componentType}).";
                        await DownloadAsset(localFileName + "talk_sync.swf", uri + "talk_sync.swf", doDecryption);
                        log.Text = $"Downloaded state 'talk_sync.swf' for component '{componentId}' ({componentType}).";
                    }
                }
                await DownloadAsset(localFileName + componentThumb, uri + componentThumb, doDecryption);
                log.Text = $"Downloaded thumbnail for component '{componentId}' ({componentType}).";

                foreach (var state in states)
                {

                    var stateId = state.Attributes().Where(a => a.Name == "id").Single().Value;
                    var stateFilename = state.Attributes().Where(a => a.Name == "filename").Single().Value;

                    uri = $"{serverAddress}{themeId}/{componentType}/{componentId}/{stateFilename}";
                    localDir = $".\\cc_store\\{themeId}\\{componentType}\\{componentId}";
                    Directory.CreateDirectory(localDir);

                    localFileName = $".\\cc_store\\{themeId}\\{componentType}\\{componentId}\\{stateFilename}";
                    await DownloadAsset(localFileName, uri, doDecryption);

                    log.Text = $"Downloaded state '{stateId}' for component '{componentId}' ({componentType}).";
                }
                duration.Value++;
            }
            var bodyshapes = xmlDoc.Elements("bodyshape");
            foreach (var bodyshape in bodyshapes)
            {
                var bodyId = bodyshape.Attributes().Where(a => a.Name == "id").Single().Value;


                    //Console.WriteLine("OK.");
                    
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

                    await DownloadAsset(localFileName + libraryId + ".swf", uri + libraryId + ".swf", false);
                    log.Text = $"Downloaded library '{libraryId}.swf' ({libraryType}).";
                    if (libraryThumb != libraryId + ".swf")
                    {
                        await DownloadAsset(localFileName + libraryThumb, uri + libraryThumb, doDecryption);
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
                        await DownloadAsset(localFileName, uri, doDecryption);
                        log.Text = $"Downloaded freeaction '{actionId}' for bodytype '{bodyId}' (actionpack {actionpackName}).";
                        duration.Value++;
                    }
                }
                components = bodyshape.Elements("component");
                duration.Maximum = components.Count();
                duration.Value = 0;
                foreach (var component in components)
                {
                    //APPARENTLY I need this dumb bullshit now even though my original code didn't.
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
                        await DownloadAsset(localFileName, uri, doDecryption);
                        log.Text = $"Downloaded state '{componentThumb}' for component '{componentId}' ({componentType}).";
                    }

                    foreach (var state in states)
                    {

                        var stateId = state.Attributes().Where(a => a.Name == "id").Single().Value;

                        uri = $"{serverAddress}{themeId}/{componentType}/{componentId}/{stateId}.swf";
                        //localDir = $".\\cc_store\\{themeId}\\{componentType}\\{componentId}";
                        //Directory.CreateDirectory(localDir);

                        localFileName = $".\\cc_store\\{themeId}\\{componentType}\\{componentId}\\{stateId}.swf";
                        await DownloadAsset(localFileName, uri, doDecryption);

                        //Console.WriteLine($"Downloaded {stateId} for {propId}!");
                        log.Text = $"Downloaded state '{stateId}' for component '{componentId}' ({componentType}).";
                    }
                    duration.Value++;
                }

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
            var serverAddress = dom.Text;
            if (serverAddress.Substring(serverAddress.Length - 1) != "/") serverAddress += "/";
            var themeId = tid.Text;
            doDecryption = checkBox1.Checked;
            if (encrypt.Text != "(auto)") key = Encoding.ASCII.GetBytes($"{encrypt.Text}");
            LockControl();

            //Download and load xml
            var uri = $"{serverAddress}{themeId}/theme.xml";
            var localFileName = $".\\{themeId}\\theme.xml";
            var dir = $".\\{themeId}\\";
            String ccThemeRefrence = null;
            Directory.CreateDirectory(dir);
            XElement xmlDoc = null;

            await DownloadAsset(localFileName, uri, false);
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
                        await DownloadAsset(localFileName, uri, doDecryption);

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

                    await DownloadAsset(localFileName, uri, doDecryption);
                    //Console.WriteLine($"Downloaded {propId}!");
                    log.Text = $"Downloaded prop '{propId}'.";
                }
                duration.Value++;
            }
            //Console.WriteLine("Props: OK");



            var effects = xmlDoc.Descendants("effect");
            duration.Maximum = effects.Count();
            duration.Value = 0;
            foreach (var effect in effects)
            {
                var effectId = effect.Attributes().Where(a => a.Name == "id").Single().Value;
                uri = $"{serverAddress}{themeId}/effect/{effectId}";


                var localDir = $".\\{themeId}\\effect";
                Directory.CreateDirectory(localDir);

                localFileName = $".\\{themeId}\\effect\\{effectId}";

                await DownloadAsset(localFileName, uri, doDecryption);

                //Console.WriteLine($"Downloaded {effectId}!");
                log.Text = $"Downloaded effect '{effectId}'.";
                duration.Value++;
            }
            //Console.WriteLine("Effects: OK");

            var backgroundsthumb = xmlDoc.Descendants("compositebg");
            duration.Maximum = backgroundsthumb.Count();
            duration.Value = 0;
            foreach (var compositebg in backgroundsthumb)
            {
                var bgThumb = compositebg.Attributes().Where(a => a.Name == "thumb").Single().Value;
                uri = $"{serverAddress}{themeId}/bg/{bgThumb}";


                var localDir = $".\\{themeId}\\bg";
                Directory.CreateDirectory(localDir);

                localFileName = $".\\{themeId}\\bg\\{bgThumb}";

                await DownloadAsset(localFileName, uri, false);
                //Console.WriteLine($"Downloaded {bgThumb}!");
                log.Text = $"Downloaded background thumbnail '{bgThumb}'.";
                duration.Value++;
            }
            //Console.WriteLine("Background Thumbnails: OK");

            var backgrounds = xmlDoc.Descendants("background");
            duration.Maximum = backgrounds.Count();
            duration.Value = 0;
            foreach (var background in backgrounds)
            {
                var bgId = background.Attributes().Where(a => a.Name == "id").Single().Value;
                uri = $"{serverAddress}{themeId}/bg/{bgId}";


                var localDir = $".\\{themeId}\\bg";
                Directory.CreateDirectory(localDir);

                localFileName = $".\\{themeId}\\bg\\{bgId}";

                await DownloadAsset(localFileName, uri, doDecryption);
                //Console.WriteLine($"Downloaded {bgId}!");
                log.Text = $"Downloaded background '{bgId}'.";
                duration.Value++;
            }
            //Console.WriteLine("Backrounds: OK");

            var sounds = xmlDoc.Descendants("sound");
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

                await DownloadAsset(localFileName, uri, (doDecryption && soundId.Contains(".swf")));
                //Console.WriteLine($"Downloaded {soundId}!");
                log.Text = $"Downloaded sound '{soundId}'.";
                foreach (var variant in variants)
                {
                    soundId = variant.Attributes().Where(a => a.Name == "id").Single().Value;
                    uri = $"{serverAddress}{themeId}/sound/{soundId}";

                    localDir = $".\\{themeId}\\sound";
                    localFileName = $".\\{themeId}\\sound\\{soundId}";

                    await DownloadAsset(localFileName, uri, (doDecryption && soundId.Contains(".swf")));
                    log.Text = $"Downloaded sound varriant '{soundId}'.";
                }
                duration.Value++;
            }
            //Console.WriteLine("Sounds: OK");

            var chars = xmlDoc.Descendants("char");
            duration.Maximum = chars.Count();
            duration.Value = 0;
            foreach (var character in chars)
            {
                var charId = character.Attributes().Where(a => a.Name == "id").Single().Value;
                //get all files contained within a character
                var actions = character.Descendants("action");
                var motions = character.Descendants("motion");
                var facials = character.Descendants("facial");
                log.Text = $"Starting on new character ({charId}).";
                //Console.WriteLine($"Starting on {charId}!");

                foreach (var action in actions)
                {
                    var actionId = action.Attributes().Where(a => a.Name == "id").Single().Value;
                    uri = $"{serverAddress}{themeId}/char/{charId}/{actionId}";


                    var localDir = $".\\{themeId}\\char\\{charId}";
                    Directory.CreateDirectory(localDir);

                    localFileName = $".\\{themeId}\\char\\{charId}\\{actionId}";

                    await DownloadAsset(localFileName, uri, doDecryption);

                    //Console.WriteLine($"Downloaded {actionId} for {charId}!");
                    log.Text = $"Downloaded action '{actionId}' for character '{charId}'.";
                }

                foreach (var motion in motions)
                {
                    var motionId = motion.Attributes().Where(a => a.Name == "id").Single().Value;
                    uri = $"{serverAddress}{themeId}/char/{charId}/{motionId}";


                    var localDir = $".\\{themeId}\\char\\{charId}";
                    Directory.CreateDirectory(localDir);

                    localFileName = $".\\{themeId}\\char\\{charId}\\{motionId}";

                    await DownloadAsset(localFileName, uri, doDecryption);

                    //Console.WriteLine($"Downloaded {motionId} for {charId}!");
                    log.Text = $"Downloaded motion '{motionId}' for character '{charId}'.";
                }

                foreach (var facial in facials)
                {
                    var facialId = facial.Attributes().Where(a => a.Name == "id").Single().Value;
                    uri = $"{serverAddress}{themeId}/char/{charId}/head/{facialId}";


                    var localDir = $".\\{themeId}\\char\\{charId}\\head";
                    Directory.CreateDirectory(localDir);

                    localFileName = $".\\{themeId}\\char\\{charId}\\head\\{facialId}";
                    //Console.WriteLine(localFileName);

                    await DownloadAsset(localFileName, uri, doDecryption);

                    //Console.WriteLine($"Downloaded {facialId} for {charId}!");
                    log.Text = $"Downloaded facial animation '{facialId}' for character '{charId}'.";
                }
                duration.Value++;


            }
            var widgets = xmlDoc.Descendants("widget");
            duration.Maximum = widgets.Count();
            duration.Value = 0;
            foreach (var widget in widgets)
            {
                var widgetThumb = widget.Attributes().Where(a => a.Name == "thumb").Single().Value;
                uri = $"{serverAddress}{themeId}/widget/{widgetThumb}";


                var localDir = $".\\{themeId}\\widget\\";
                Directory.CreateDirectory(localDir);

                localFileName = $".\\{themeId}\\widget\\{widgetThumb}";
                //Console.WriteLine(localFileName);

                await DownloadAsset(localFileName, uri, false);

                //Console.WriteLine($"Downloaded {facialId} for {charId}!");
                log.Text = $"Downloaded widget thumbnail '{widgetThumb}'.";
                duration.Value++;
            }

            var flows = xmlDoc.Descendants("flow");
            duration.Maximum = flows.Count();
            duration.Value = 0;
            foreach (var flow in flows)
            {
                var flowId = flow.Attributes().Where(a => a.Name == "id").Single().Value;
                var flowThumb = flow.Attributes().Where(a => a.Name == "thumb").Single().Value;
                uri = $"{serverAddress}{themeId}/flow/";


                var localDir = $".\\{themeId}\\flow\\";
                Directory.CreateDirectory(localDir);

                localFileName = $".\\{themeId}\\flow\\";
                //Console.WriteLine(localFileName);

                await DownloadAsset(localFileName+flowId, uri+flowId, doDecryption);
                await DownloadAsset(localFileName+flowThumb,uri+flowThumb, false);


                //Console.WriteLine($"Downloaded {facialId} for {charId}!");
                log.Text = $"Downloaded downloaded flow frame '{flowId}'!";
                duration.Value++;

            }
            //Console.WriteLine("Characters: OK");
            //Console.WriteLine("GoAnimateRipper");
            //Console.WriteLine("Written by Poley Magik");
            //Console.WriteLine("Thanks for using this tool");
            //JUMP TO ME
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
            button1.Enabled = false;
            await StartProceedure();
        }

        private void encrypt_SelectedIndexChanged(object sender, EventArgs e)
        {
            var key = encrypt.SelectedItem;

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            reencrypt.Enabled = checkBox1.Checked;
        }

        private void dom_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void statusStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
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
            if (JPEXStoggle.Checked)
            {
                MessageBox.Show("This feature is for developers only! I am NOT offering support for this feature, and I know it\'s incredibly slow. You additionally must have JPEXS installed for it to work.", "I\'m warning you!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                checkBox1.Checked = true;
                checkBox1.Enabled = false;
                encrypt.Text = "(auto)";
                encrypt.Enabled = false;
                reencrypt.Checked = false;
                reencrypt.Enabled = false;
                deleteAfter.Enabled = true;
                hideCMD.Enabled = true;
            }
            else
            {
                checkBox1.Enabled = true;
                encrypt.Enabled = true;
                reencrypt.Enabled = true;
                deleteAfter.Enabled = false;
                hideCMD.Enabled = false;
            }

        }

        private void hideCMD_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
