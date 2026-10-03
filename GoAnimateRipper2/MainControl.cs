using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
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

        private readonly String[] STANDARD_THEME_LIST =
        {
            "common",
            "custom",
            "retro",
            "politic",
            "politics2",
            "christmas",
            "monstermsh",
            "monkeytalk",
            "startrek",
            "bunny",
            "akon",
            "willie",
            "fullenergy",
            "stick",
            "sticklybiz",
            "bizmodels",
            "commoncraft",
            "space",
            "toonadv",
            "ben10",
            "sf",
            "street",
            "animal",
            "action",
            "chowder",
            "domo",
            "underdog",
            "vietnam"
        };
        private readonly String[] CC_THEME_LIST =
        {
            "family",
            "cc2",
            "cctoonadventure"
        };
        private readonly String[] EITHER_THEME_LIST =
        {
            "anime",
            "ninjaanime",
            "spacecitzen",
            "business",
            "whiteboard",
            "infographics",
            "botdf",
            "chibi",
            "ninja"
        };

        private static MainControl instance;
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

        /// <summary>
        /// public void <c>setProgressBarMaximum</c> sets the progress bar maximum.
        /// </summary>
        public void setProgressBarMaximum(int max)
        {
            duration.Maximum = max;
        }

        /// <summary>
        /// public void <c>incrementProgressBar</c> increase the progress bar.
        /// </summary>
        public void incrementProgressBar()
        {
            duration.Value++;
        }

        /// <summary>
        /// public void <c>resetProgressBar</c> reset the progress bar.
        /// </summary>
        public void resetProgressBar()
        {
            duration.Value = 0;
        }

        /// <summary>
        /// public static MainControl <c>getInstance</c> returns the instance.
        /// </summary>
        public static MainControl getInstance()
        {
            return instance;
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
                ripper = new CCRipper(themeId);
            }
            else
            {
                ripper = new StandardRipper(themeId);
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
            Debug.Assert(instance != null, "[MainControl.cs] There shouldn't ever be two of this class, something terribly wrong has happened!!!");

            InitializeComponent();
            instance = this;
            
        }

        async private void RipButton_Click(object sender, EventArgs e)
        {
            ripButton.Enabled = false;
            await StartProceedure();
        }

        private void MainControl_Load(object sender, EventArgs e)
        {
            Text = FORM_NAME + " v" + VERSION;

#if DEBUG
            Text = FORM_NAME + " DEBUG Build! WIP ... v" + VERSION;
#endif
            //New combo box function
            String[] fullThemeList = STANDARD_THEME_LIST.Concat(CC_THEME_LIST).Concat(EITHER_THEME_LIST).ToArray();
     
            Array.Sort(fullThemeList);
            themeIdInput.Items.AddRange(fullThemeList);
        }

        //Quick way to bodge in the log actually being useful.
        private void Log_TextChanged(object sender, EventArgs e)
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

        private void decEnabled_Click(object sender, EventArgs e)
        {
            reEncEnabled.Enabled = decEnabled.Checked;
        }

        private void ffdecEnabled_CheckedChanged(object sender, EventArgs e)
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

        private void themeIdInput_TextUpdate(object sender, EventArgs e)
        {
            if (!STANDARD_THEME_LIST.Contains(themeIdInput.Text) && !CC_THEME_LIST.Contains(themeIdInput.Text))
            {
                return;
            }
            if (STANDARD_THEME_LIST.Contains(themeIdInput.Text))
            {
                themeCheck.Checked = true;
            }
            else
            {
                CCCheck.Checked = true;
            }
        }
    }
}
