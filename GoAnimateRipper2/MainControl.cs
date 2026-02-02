using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;


namespace GoAnimateRipper2
{
    public partial class MainControl : Form
    {
        private readonly String FORM_NAME = "GoAnimateRipper2";
        private readonly String VERSION = Assembly.GetExecutingAssembly().GetName().Version.ToString().Substring(0, Assembly.GetExecutingAssembly().GetName().Version.ToString().LastIndexOf("."));
        //Publicly exposed control options
        //Set when start button pressed
        public bool doDecryption;
        public bool doReEncryption;
        public bool doDecompile;
        public bool skipFlash;
        public bool skipNonFlash;
        public bool decOrganize;
        public bool decDontShowCmd = false;
        public bool decImageOut;
        public bool overwriteFiles;
        public bool logErrorsOnly;

        public String themeId;
        public String domain;
        public String decryptionKey;
        public String reEncryptionKey;


        public void setProgressBarMaximum(int max)
        {
            duration.Maximum = max;
        }

        public void incrementProgressBar()
        {
            duration.Value++;
        }

        public void resetProgressBar()
        {
            duration.Value = 0;
        }

        /// <summary>
        /// void <c>StartProceedure</c> initiates the correct ripping process based on the selected settings.
        /// </summary>
        public async Task StartProceedure()
        {
            logHistory.Text = "";

            doDecryption = decEnabled.Checked;
            doReEncryption = reEncEnabled.Checked;
            doDecompile = ffdecEnabled.Checked;
            skipFlash = skipFlashCheckBox.Checked;
            skipNonFlash = skipNonFlashCheckBox.Checked;
            decOrganize = reOrgDecomp.Checked;
            decImageOut = expPreview.Checked;
            overwriteFiles = ripRedundant.Checked;
            logErrorsOnly = logErrors.Checked;

            themeId = themeIdInput.Text;
            domain = domainInput.Text;
            decryptionKey = encryptKey.Text;
            reEncryptionKey = reEncryptKey.Text;

            if (ffdecEnabled.Checked)
            {
                if (Directory.Exists($".\\ffdec_output")) Directory.Delete($".\\ffdec_output", true);
                if (Directory.Exists($".\\ffdec_working")) Directory.Delete($".\\ffdec_working", true);

            }
            if (skipFlashCheckBox.Checked && skipNonFlashCheckBox.Checked || CCCheck.Checked && skipFlashCheckBox.Checked)
            {
                ReturnWithMessage("What are you trying to do?");
                return;
            }
            RipperBase ripper;
            if (CCCheck.Checked)
            {
                ripper = new CCRipper(this, themeId);
            }
            else
            {
                ripper = new StandardRipper(this, themeId);
            }
            await ripper.StartRip();

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
            themeIdInput.Enabled = false;
            ffdecEnabled.Enabled = false;
            reOrgDecomp.Enabled = false;
            expPreview.Enabled = false;
            ripRedundant.Enabled = false;
            domainInput.Enabled = false;
            skipNonFlashCheckBox.Enabled = false;
            skipFlashCheckBox.Enabled = false;
            logErrors.Enabled = false;
        }

        /// <summary>
        /// void <c>ReturnWithMessage</c> Sends a message to the log.
        /// </summary>
        /// (<paramref name="mes"/>,<paramref name="isError"/>)
        /// <param name="mes">The message.</param>
        /// <param name="isError">If the message is an error.</param>
        public void writeMessage(String mes, bool isError = false)
        {
            if (isError || !logErrorsOnly)
            {
                log.Text = mes;
            }
        }
        /// <summary>
        /// void <c>ReturnWithMessage</c> returns user control and sends a message to the log.
        /// </summary>
        public void ReturnWithMessage(String mes)
        {
            System.Media.SystemSounds.Hand.Play();
            log.Text = mes;
            ripButton.Enabled = true;
            ripButton.Text = "Start Ripping";
            ffdecEnabled.Enabled = true;
            reOrgDecomp.Enabled = ffdecEnabled.Checked;
            expPreview.Enabled = ffdecEnabled.Checked;
            encryptKey.Enabled = !ffdecEnabled.Checked;
            reEncEnabled.Enabled = !ffdecEnabled.Checked && decEnabled.Checked;
            reEncryptKey.Enabled = !ffdecEnabled.Checked;
            decEnabled.Enabled = !ffdecEnabled.Checked;
            themeIdInput.Enabled = true;
            ripRedundant.Enabled = true;
            domainInput.Enabled = true;
            skipNonFlashCheckBox.Enabled = true;
            skipFlashCheckBox.Enabled = true;
            logErrors.Enabled = true;
            return;
        }

        public MainControl()
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
            Text = FORM_NAME + " v" + VERSION;

#if DEBUG
            Text = FORM_NAME + " DEBUG Build! WIP ... v" + VERSION;
#endif
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
                reOrgDecomp.Enabled = true;
                expPreview.Enabled = true;
            }
            else
            {
                decEnabled.Enabled = true;
                encryptKey.Enabled = true;
                reEncEnabled.Enabled = true;
                reOrgDecomp.Enabled = false;
                expPreview.Enabled = false;
            }

        }

        private void hideCMD_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void log_Click(object sender, EventArgs e)
        {

        }

        //Quick way to bodge in the log actually being useful.
        private void log_TextChanged(object sender, EventArgs e)
        {
            if (!logErrors.Checked || log.Text.Contains("[ERROR]"))
            {
                if (logHistory.Text.Length > 0)
                {
                    logHistory.Text += "\n" + log.Text;
                }
                else
                {
                    logHistory.Text = log.Text;
                }
                logHistory.SelectionStart = logHistory.Text.Length;
                logHistory.ScrollToCaret();
            }
        }

        private void skipNonFlash_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
