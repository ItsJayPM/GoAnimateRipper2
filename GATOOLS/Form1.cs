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
        public static byte[] Decrypt(byte[] pwd, byte[] data)
        { //Not gonna notate this RC4 stuff
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
        public async Task downloadAsset(string localFileName, string uriDownload, bool decrypt, byte[] keyt)
        {
            var httpClient = new HttpClient();
            byte[] key = keyt;
            using (var response = await httpClient.GetAsync(uriDownload))
            {
                var data = await response.Content.ReadAsByteArrayAsync();
                if (decrypt)
                {
                    byte[] deCypheredText = Decrypt(key, data);
                    if (reencrypt.Checked)
                    {
                        key = Encoding.ASCII.GetBytes($"{key2.Text}");
                        deCypheredText = Decrypt(key, deCypheredText);
                    }
                    File.WriteAllBytes(localFileName, deCypheredText);
                }
                else
                {
                    File.WriteAllBytes(localFileName, data);
                }
                return;
            }
        }

        public async Task startTask()
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
        public void lockControl()
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
        public void returnWithMessage(String mes)
        {
            log.Text = mes;
            button1.Enabled = true;
            button1.Text = "Start Ripping";
            encrypt.Enabled = true;
            reencrypt.Enabled = checkBox1.Checked;
            key2.Enabled = true;
            checkBox1.Enabled = true;
            tid.Enabled = true;
            dom.Enabled = true;
            return;
        }
        public async Task GoAnimateCCRip(String carryThemeId)
        {
            lockControl();
            var serverAddress = dom.Text + "cc_store/";
            var httpClient = new HttpClient();
            var themeId = carryThemeId != null ? carryThemeId : tid.Text;
            var doDecryption = checkBox1.Checked;
            byte[] key = Encoding.ASCII.GetBytes($"{encrypt.Text}");

            var uri = $"{serverAddress}{themeId}/cc_theme.xml";
            var localFileName = $".\\cc_store\\{themeId}\\cc_theme.xml";
            var dir = $".\\cc_store\\{themeId}\\";
            Directory.CreateDirectory(dir);
            XElement xmlDoc = null;

            await downloadAsset(localFileName, uri, false, null);
            try
            {
                xmlDoc = XElement.Load(localFileName);
            }
            catch
            {
                returnWithMessage("An error occured parsing the cc_theme.xml file!");
                return;
            }
            var components = xmlDoc.Elements("component");
            //Reworked bar that actually isn't shite
            duration.Maximum = components.Count();
            duration.Value = 0;
            foreach (var component in components)
            {
                var componentId = component.Attributes().Where(a => a.Name == "id").Single().Value;
                var componentType = component.Attributes().Where(a => a.Name == "type").Single().Value;
                var componentThumb = component.Attributes().Where(a => a.Name == "thumb").Single().Value;
                var states = component.Elements("state");
                uri = $"{serverAddress}{themeId}/{componentType}/{componentId}/";
                var localDir = $".\\cc_store\\{themeId}\\{componentType}\\{componentId}";
                Directory.CreateDirectory(localDir);

                localFileName = $".\\cc_store\\{themeId}\\{componentType}\\{componentId}\\";

                if (componentType == "mouth")
                {
                    await downloadAsset(localFileName + "talk.swf", uri + "talk.swf", doDecryption, key);
                    log.Text = $"Downloaded state 'talk.swf' for component '{componentId}' ({componentType}).";
                    await downloadAsset(localFileName + "talk_sync.swf", uri + "talk_sync.swf", doDecryption, key);
                    log.Text = $"Downloaded state 'talk_sync.swf' for component '{componentId}' ({componentType}).";
                    if (themeId == "family")
                    {
                        await downloadAsset(localFileName + "talk_sad_sync.swf", uri + "talk_sad_sync.swf", doDecryption, key);
                        log.Text = $"Downloaded state 'talk_sad_sync.swf' for component '{componentId}' ({componentType}).";
                        await downloadAsset(localFileName + "talk_happy_sync.swf", uri + "talk_happy_sync.swf", doDecryption, key);
                        log.Text = $"Downloaded state 'talk_happy_sync.swf' for component '{componentId}' ({componentType}).";
                        await downloadAsset(localFileName + "talk_angry_sync.swf", uri + "talk_angry_sync.swf", doDecryption, key);
                        log.Text = $"Downloaded state 'talk_angry_sync.swf' for component '{componentId}' ({componentType}).";

                    }
                }
                await downloadAsset(localFileName + componentThumb, uri + componentThumb, doDecryption, key);
                log.Text = $"Downloaded thumbnail for component '{componentId}' ({componentType}).";

                foreach (var state in states)
                {

                    var stateId = state.Attributes().Where(a => a.Name == "id").Single().Value;

                    uri = $"{serverAddress}{themeId}/{componentType}/{componentId}/{stateId}.swf";
                    localDir = $".\\cc_store\\{themeId}\\{componentType}\\{componentId}";
                    Directory.CreateDirectory(localDir);

                    localFileName = $".\\cc_store\\{themeId}\\{componentType}\\{componentId}\\{stateId}.swf";
                    await downloadAsset(localFileName, uri, doDecryption, key);

                    //Console.WriteLine($"Downloaded {stateId} for {propId}!");
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

                    await downloadAsset(localFileName + libraryId + ".swf", uri + libraryId + ".swf", false, null);
                    log.Text = $"Downloaded library '{libraryId}.swf' ({libraryType}).";
                    await downloadAsset(localFileName + libraryThumb, uri + libraryThumb, doDecryption, key);
                    log.Text = $"Downloaded library thumb '{libraryThumb}' ({libraryType}).";
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
                        await downloadAsset(localFileName, uri, doDecryption, key);
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
                    var states = component.Elements("state");
                    var localDir = $".\\cc_store\\{themeId}\\{componentType}\\{componentId}";
                    Directory.CreateDirectory(localDir);
                    uri = $"{serverAddress}{themeId}/{componentType}/{componentId}/{componentThumb}";
                    localFileName = $".\\cc_store\\{themeId}\\{componentType}\\{componentId}\\{componentThumb}";
                    await downloadAsset(localFileName, uri, doDecryption, key);
                    log.Text = $"Downloaded state '{componentThumb}' for component '{componentId}' ({componentType}).";

                    foreach (var state in states)
                    {

                        var stateId = state.Attributes().Where(a => a.Name == "id").Single().Value;

                        uri = $"{serverAddress}{themeId}/{componentType}/{componentId}/{stateId}.swf";
                        //localDir = $".\\cc_store\\{themeId}\\{componentType}\\{componentId}";
                        //Directory.CreateDirectory(localDir);

                        localFileName = $".\\cc_store\\{themeId}\\{componentType}\\{componentId}\\{stateId}.swf";
                        await downloadAsset(localFileName, uri, doDecryption, key);

                        //Console.WriteLine($"Downloaded {stateId} for {propId}!");
                        log.Text = $"Downloaded state '{stateId}' for component '{componentId}' ({componentType}).";
                    }
                    duration.Value++;
                }

            }
            if (carryThemeId == null)
            {
                returnWithMessage("The proceedure has completed.");
            }
            return;
        }

        public async Task GoAnimateRip(bool lookForCCTheme)
        {
            //get user input
            var serverAddress = dom.Text;
            var httpClient = new HttpClient();
            var themeId = tid.Text;
            var doDecryption = checkBox1.Checked;
            byte[] key = Encoding.ASCII.GetBytes($"{encrypt.Text}");
            lockControl();

            //Download and load xml
            var uri = $"{serverAddress}{themeId}/theme.xml";
            var localFileName = $".\\{themeId}\\theme.xml";
            var dir = $".\\{themeId}\\";
            String ccThemeRefrence = null;
            Directory.CreateDirectory(dir);
            XElement xmlDoc = null;

            await downloadAsset(localFileName, uri, false, null);
            try
            {
                xmlDoc = XElement.Load(localFileName);
            }
            catch
            {
                returnWithMessage("An error occured parsing the theme.xml file!");
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
                        await downloadAsset(localFileName, uri, doDecryption, key);

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

                    await downloadAsset(localFileName, uri, doDecryption, key);
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

                await downloadAsset(localFileName, uri, doDecryption, key);

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

                await downloadAsset(localFileName, uri, false, null);
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

                await downloadAsset(localFileName, uri, doDecryption, key);
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

                await downloadAsset(localFileName, uri, (doDecryption && soundId.Contains(".swf")), key);
                //Console.WriteLine($"Downloaded {soundId}!");
                log.Text = $"Downloaded sound '{soundId}'.";
                foreach (var variant in variants)
                {
                    soundId = variant.Attributes().Where(a => a.Name == "id").Single().Value;
                    uri = $"{serverAddress}{themeId}/sound/{soundId}";

                    localDir = $".\\{themeId}\\sound";
                    localFileName = $".\\{themeId}\\sound\\{soundId}";

                    await downloadAsset(localFileName, uri, (doDecryption && soundId.Contains(".swf")), key);
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

                    await downloadAsset(localFileName, uri, doDecryption, key);

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

                    await downloadAsset(localFileName, uri, doDecryption, key);

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

                    await downloadAsset(localFileName, uri, doDecryption, key);

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

                await downloadAsset(localFileName, uri, false, key);

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

                await downloadAsset(localFileName+flowId, uri+flowId, doDecryption, key);
                await downloadAsset(localFileName+flowThumb,uri+flowThumb, false, null);


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
            returnWithMessage("The proceedure has completed.");
        }

        public Form1()
        {
            InitializeComponent();
        }

        async private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            await startTask();
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
    }
}
