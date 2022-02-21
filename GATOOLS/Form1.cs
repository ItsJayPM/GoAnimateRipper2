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
        public async Task downloadAsset(string localFileName, string uriDownload, bool decrypt, byte[] key)
        {
            var httpClient = new HttpClient();
            using (var response = await httpClient.GetAsync(uriDownload))
            {
                var data = await response.Content.ReadAsByteArrayAsync();
                if (decrypt)
                {
                    byte[] deCypheredText = Decrypt(key, data);
                    File.WriteAllBytes(localFileName, deCypheredText);
                }
                else
                {
                    File.WriteAllBytes(localFileName, data);
                }
            }
        }
        public async Task GoAnimateRip()
        {
            //get user input
            var serverAddress = dom.Text;
            var httpClient = new HttpClient();
            var themeId = tid.Text;
            var doDecryption = checkBox1.Checked;
            byte[] key = Encoding.ASCII.GetBytes($"{encrypt.Text}");
            button1.Text = "Working...";

            //disable user interaction even though it technically doesn't matter
            encrypt.Enabled = false;
            checkBox1.Enabled = false;
            tid.Enabled = false;
            dom.Enabled = false;

            //Download and load xml
            var uri = $"{serverAddress}{themeId}/theme.xml";
            var localFileName = $".\\{themeId}\\theme.xml";
            var dir = $".\\{themeId}\\";
            Directory.CreateDirectory(dir);

            await downloadAsset(localFileName, uri, false, null);
            XElement xmlDoc = XElement.Load(localFileName);

            //To-Do: Refactor the loading bar thing
            bar.Value = 0;
            var props = xmlDoc.Elements("prop");
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
                        ACTION.Text = $"Downloaded {stateId} for {propId}!";
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
                    ACTION.Text = $"Downloaded {propId}!";
                }
            }
            //Console.WriteLine("Props: OK");
            bar.Value = 1;



            var effects = xmlDoc.Descendants("effect");

            foreach (var effect in effects)
            {
                var effectId = effect.Attributes().Where(a => a.Name == "id").Single().Value;
                uri = $"{serverAddress}{themeId}/effect/{effectId}";


                var localDir = $".\\{themeId}\\effect";
                Directory.CreateDirectory(localDir);

                localFileName = $".\\{themeId}\\effect\\{effectId}";

                await downloadAsset(localFileName, uri, doDecryption, key);

                //Console.WriteLine($"Downloaded {effectId}!");
                ACTION.Text = $"Downloaded {effectId}!";
            }
            //Console.WriteLine("Effects: OK");
            bar.Value = 2;

            var backgroundsthumb = xmlDoc.Descendants("compositebg");

            foreach (var compositebg in backgroundsthumb)
            {
                var bgThumb = compositebg.Attributes().Where(a => a.Name == "thumb").Single().Value;
                uri = $"{serverAddress}{themeId}/bg/{bgThumb}";


                var localDir = $".\\{themeId}\\bg";
                Directory.CreateDirectory(localDir);

                localFileName = $".\\{themeId}\\bg\\{bgThumb}";

                await downloadAsset(localFileName, uri, false, null);
                //Console.WriteLine($"Downloaded {bgThumb}!");
                ACTION.Text = $"Downloaded {bgThumb}!";
            }
            //Console.WriteLine("Background Thumbnails: OK");
            bar.Value = 3;

            var backgrounds = xmlDoc.Descendants("background");

            foreach (var background in backgrounds)
            {
                var bgId = background.Attributes().Where(a => a.Name == "id").Single().Value;
                uri = $"{serverAddress}{themeId}/bg/{bgId}";


                var localDir = $".\\{themeId}\\bg";
                Directory.CreateDirectory(localDir);

                localFileName = $".\\{themeId}\\bg\\{bgId}";

                await downloadAsset(localFileName, uri, doDecryption, key);
                //Console.WriteLine($"Downloaded {bgId}!");
                ACTION.Text = $"Downloaded {bgId}!";
            }
            //Console.WriteLine("Backrounds: OK");
            bar.Value = 4;

            var sounds = xmlDoc.Descendants("sound");

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
                ACTION.Text = $"Downloaded {soundId}!";
                foreach (var variant in variants)
                {
                    soundId = variant.Attributes().Where(a => a.Name == "id").Single().Value;
                    uri = $"{serverAddress}{themeId}/sound/{soundId}";

                    localDir = $".\\{themeId}\\sound";
                    localFileName = $".\\{themeId}\\sound\\{soundId}";

                    await downloadAsset(localFileName, uri, (doDecryption && soundId.Contains(".swf")), key);
                    ACTION.Text = $"Downloaded varriant {soundId}!";
                }
            }
            //Console.WriteLine("Sounds: OK");
            bar.Value = 5;

            var chars = xmlDoc.Descendants("char");
            foreach (var character in chars)
            {
                var charId = character.Attributes().Where(a => a.Name == "id").Single().Value;
                //get all files contained within a character
                var actions = character.Descendants("action");
                var motions = character.Descendants("motion");
                var facials = character.Descendants("facial");
                ACTION.Text = $"Starting on {charId}!";
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
                    ACTION.Text = $"Downloaded {actionId} for {charId}!";
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
                    ACTION.Text = $"Downloaded {motionId} for {charId}!";
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
                    ACTION.Text = $"Downloaded {facialId} for {charId}!";
                }


            }
            var widgets = xmlDoc.Descendants("widget");
            foreach (var widget in widgets)
            {
                var widgetThumb = widget.Attributes().Where(a => a.Name == "thumb").Single().Value;
                uri = $"{serverAddress}{themeId}/widget/{widgetThumb}";


                var localDir = $".\\{themeId}\\widget\\";
                Directory.CreateDirectory(localDir);

                localFileName = $".\\{themeId}\\widget\\{widgetThumb}";
                //Console.WriteLine(localFileName);

                await downloadAsset(localFileName, uri, doDecryption, key);

                //Console.WriteLine($"Downloaded {facialId} for {charId}!");
                ACTION.Text = $"Downloaded {widgetThumb}!";

            }

            var flows = xmlDoc.Descendants("flow");
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
                await downloadAsset(localFileName+flowThumb,uri+flowThumb, doDecryption, key);


                //Console.WriteLine($"Downloaded {facialId} for {charId}!");
                ACTION.Text = $"Downloaded {flowId} and {flowThumb}!";

            }
            //Console.WriteLine("Characters: OK");
            bar.Value = 6;
            //Console.WriteLine("GoAnimateRipper");
            //Console.WriteLine("Written by Poley Magik");
            //Console.WriteLine("Thanks for using this tool");
            ACTION.Text = "Done!";
            button1.Enabled = true;
            button1.Text = "Start Ripping";
            encrypt.Enabled = checkBox1.Checked;
            checkBox1.Enabled = true;
            tid.Enabled = true;
            dom.Enabled = true;
        }

        public Form1()
        {
            InitializeComponent();
        }

        async private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            await GoAnimateRip();
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
            encrypt.Enabled = checkBox1.Checked;
        }

        private void bar_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void dom_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
