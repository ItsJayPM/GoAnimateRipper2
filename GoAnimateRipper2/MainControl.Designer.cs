
namespace GoAnimateRipper2
{
    partial class MainControl
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainControl));
            this.ripButton = new System.Windows.Forms.Button();
            this.encryptKey = new System.Windows.Forms.ComboBox();
            this.encLabel = new System.Windows.Forms.Label();
            this.decEnabled = new System.Windows.Forms.CheckBox();
            this.themeIdInput = new System.Windows.Forms.TextBox();
            this.tIdLabel = new System.Windows.Forms.Label();
            this.domainLabel = new System.Windows.Forms.Label();
            this.domainInput = new System.Windows.Forms.TextBox();
            this.themeCheck = new System.Windows.Forms.RadioButton();
            this.themeCCCheck = new System.Windows.Forms.RadioButton();
            this.CCCheck = new System.Windows.Forms.RadioButton();
            this.log = new System.Windows.Forms.Label();
            this.duration = new System.Windows.Forms.ProgressBar();
            this.reEncEnabled = new System.Windows.Forms.CheckBox();
            this.reEncLabel = new System.Windows.Forms.Label();
            this.reEncryptKey = new System.Windows.Forms.ComboBox();
            this.jpexsGroup = new System.Windows.Forms.GroupBox();
            this.hideCmd = new System.Windows.Forms.CheckBox();
            this.reOrgDecomp = new System.Windows.Forms.CheckBox();
            this.ffdecEnabled = new System.Windows.Forms.CheckBox();
            this.ripRedundant = new System.Windows.Forms.CheckBox();
            this.skipNonFlashCheckBox = new System.Windows.Forms.CheckBox();
            this.miscGroup = new System.Windows.Forms.GroupBox();
            this.logErrors = new System.Windows.Forms.CheckBox();
            this.skipFlashCheckBox = new System.Windows.Forms.CheckBox();
            this.encryptionGroup = new System.Windows.Forms.GroupBox();
            this.logHistory = new System.Windows.Forms.RichTextBox();
            this.jpexsGroup.SuspendLayout();
            this.miscGroup.SuspendLayout();
            this.encryptionGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // ripButton
            // 
            this.ripButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ripButton.Location = new System.Drawing.Point(6, 302);
            this.ripButton.Margin = new System.Windows.Forms.Padding(4);
            this.ripButton.Name = "ripButton";
            this.ripButton.Size = new System.Drawing.Size(773, 30);
            this.ripButton.TabIndex = 0;
            this.ripButton.Text = "Start Ripping";
            this.ripButton.UseVisualStyleBackColor = true;
            this.ripButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // encryptKey
            // 
            this.encryptKey.AccessibleName = "";
            this.encryptKey.FormattingEnabled = true;
            this.encryptKey.Items.AddRange(new object[] {
            "g0o1a2n3i4m5a6t7e",
            "sorrypleasetryagainlater"});
            this.encryptKey.Location = new System.Drawing.Point(471, 17);
            this.encryptKey.Margin = new System.Windows.Forms.Padding(4);
            this.encryptKey.Name = "encryptKey";
            this.encryptKey.Size = new System.Drawing.Size(291, 24);
            this.encryptKey.TabIndex = 1;
            this.encryptKey.Text = "(auto)";
            this.encryptKey.SelectedIndexChanged += new System.EventHandler(this.encrypt_SelectedIndexChanged);
            // 
            // encLabel
            // 
            this.encLabel.AutoSize = true;
            this.encLabel.Location = new System.Drawing.Point(355, 24);
            this.encLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.encLabel.Name = "encLabel";
            this.encLabel.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.encLabel.Size = new System.Drawing.Size(106, 16);
            this.encLabel.TabIndex = 2;
            this.encLabel.Text = "Encrypytion Key:";
            this.encLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.encLabel.Click += new System.EventHandler(this.label1_Click);
            // 
            // decEnabled
            // 
            this.decEnabled.AutoSize = true;
            this.decEnabled.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.decEnabled.Location = new System.Drawing.Point(7, 22);
            this.decEnabled.Margin = new System.Windows.Forms.Padding(4);
            this.decEnabled.Name = "decEnabled";
            this.decEnabled.Size = new System.Drawing.Size(172, 21);
            this.decEnabled.TabIndex = 3;
            this.decEnabled.Text = "Decryption Enabled";
            this.decEnabled.UseVisualStyleBackColor = true;
            this.decEnabled.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // themeIdInput
            // 
            this.themeIdInput.Location = new System.Drawing.Point(81, 15);
            this.themeIdInput.Margin = new System.Windows.Forms.Padding(4);
            this.themeIdInput.Name = "themeIdInput";
            this.themeIdInput.Size = new System.Drawing.Size(698, 22);
            this.themeIdInput.TabIndex = 7;
            this.themeIdInput.Text = "family";
            // 
            // tIdLabel
            // 
            this.tIdLabel.AutoSize = true;
            this.tIdLabel.Location = new System.Drawing.Point(4, 18);
            this.tIdLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.tIdLabel.Name = "tIdLabel";
            this.tIdLabel.Size = new System.Drawing.Size(69, 16);
            this.tIdLabel.TabIndex = 8;
            this.tIdLabel.Text = "Theme ID:";
            // 
            // domainLabel
            // 
            this.domainLabel.AutoSize = true;
            this.domainLabel.Location = new System.Drawing.Point(4, 52);
            this.domainLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.domainLabel.Name = "domainLabel";
            this.domainLabel.Size = new System.Drawing.Size(57, 16);
            this.domainLabel.TabIndex = 9;
            this.domainLabel.Text = "Domain:";
            // 
            // domainInput
            // 
            this.domainInput.Location = new System.Drawing.Point(81, 48);
            this.domainInput.Margin = new System.Windows.Forms.Padding(4);
            this.domainInput.Name = "domainInput";
            this.domainInput.Size = new System.Drawing.Size(698, 22);
            this.domainInput.TabIndex = 10;
            this.domainInput.Text = "https://flashthemes.net/static/store/";
            this.domainInput.TextChanged += new System.EventHandler(this.dom_TextChanged);
            // 
            // themeCheck
            // 
            this.themeCheck.AutoSize = true;
            this.themeCheck.Location = new System.Drawing.Point(7, 77);
            this.themeCheck.Name = "themeCheck";
            this.themeCheck.Size = new System.Drawing.Size(71, 20);
            this.themeCheck.TabIndex = 13;
            this.themeCheck.Text = "Theme\r\n";
            this.themeCheck.UseVisualStyleBackColor = true;
            this.themeCheck.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // themeCCCheck
            // 
            this.themeCCCheck.AutoSize = true;
            this.themeCCCheck.Location = new System.Drawing.Point(83, 77);
            this.themeCCCheck.Name = "themeCCCheck";
            this.themeCCCheck.Size = new System.Drawing.Size(299, 20);
            this.themeCCCheck.TabIndex = 14;
            this.themeCCCheck.Text = "Theme (+Attatched Character Creator Theme)";
            this.themeCCCheck.UseVisualStyleBackColor = true;
            this.themeCCCheck.CheckedChanged += new System.EventHandler(this.ThemeCCCheck_CheckedChanged);
            // 
            // CCCheck
            // 
            this.CCCheck.AutoSize = true;
            this.CCCheck.Checked = true;
            this.CCCheck.Location = new System.Drawing.Point(403, 77);
            this.CCCheck.Name = "CCCheck";
            this.CCCheck.Size = new System.Drawing.Size(179, 20);
            this.CCCheck.TabIndex = 15;
            this.CCCheck.TabStop = true;
            this.CCCheck.Text = "Character Creator Theme";
            this.CCCheck.UseVisualStyleBackColor = true;
            this.CCCheck.CheckedChanged += new System.EventHandler(this.CCCheck_CheckedChanged);
            // 
            // log
            // 
            this.log.AutoSize = true;
            this.log.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.log.Location = new System.Drawing.Point(4, 515);
            this.log.Name = "log";
            this.log.Size = new System.Drawing.Size(286, 16);
            this.log.TabIndex = 16;
            this.log.Text = "(The most recent action will be displayed here.)";
            this.log.TextChanged += new System.EventHandler(this.log_TextChanged);
            this.log.Click += new System.EventHandler(this.log_Click);
            // 
            // duration
            // 
            this.duration.BackColor = System.Drawing.SystemColors.Window;
            this.duration.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.duration.Location = new System.Drawing.Point(7, 339);
            this.duration.Maximum = 0;
            this.duration.Name = "duration";
            this.duration.Size = new System.Drawing.Size(772, 23);
            this.duration.TabIndex = 17;
            // 
            // reEncEnabled
            // 
            this.reEncEnabled.AutoSize = true;
            this.reEncEnabled.Enabled = false;
            this.reEncEnabled.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.reEncEnabled.Location = new System.Drawing.Point(7, 45);
            this.reEncEnabled.Margin = new System.Windows.Forms.Padding(4);
            this.reEncEnabled.Name = "reEncEnabled";
            this.reEncEnabled.Size = new System.Drawing.Size(174, 21);
            this.reEncEnabled.TabIndex = 18;
            this.reEncEnabled.Text = "Re-encrypt Enabled";
            this.reEncEnabled.UseVisualStyleBackColor = true;
            // 
            // reEncLabel
            // 
            this.reEncLabel.AutoSize = true;
            this.reEncLabel.Location = new System.Drawing.Point(334, 48);
            this.reEncLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.reEncLabel.Name = "reEncLabel";
            this.reEncLabel.Size = new System.Drawing.Size(127, 16);
            this.reEncLabel.TabIndex = 20;
            this.reEncLabel.Text = "Re-encrypytion Key:";
            this.reEncLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.reEncLabel.Click += new System.EventHandler(this.label2_Click_1);
            // 
            // reEncryptKey
            // 
            this.reEncryptKey.AccessibleName = "";
            this.reEncryptKey.FormattingEnabled = true;
            this.reEncryptKey.Items.AddRange(new object[] {
            "g0o1a2n3i4m5a6t7e",
            "sorrypleasetryagainlater"});
            this.reEncryptKey.Location = new System.Drawing.Point(471, 45);
            this.reEncryptKey.Margin = new System.Windows.Forms.Padding(4);
            this.reEncryptKey.Name = "reEncryptKey";
            this.reEncryptKey.Size = new System.Drawing.Size(291, 24);
            this.reEncryptKey.TabIndex = 19;
            this.reEncryptKey.Text = "sorrypleasetryagainlater";
            // 
            // jpexsGroup
            // 
            this.jpexsGroup.Controls.Add(this.hideCmd);
            this.jpexsGroup.Controls.Add(this.reOrgDecomp);
            this.jpexsGroup.Controls.Add(this.ffdecEnabled);
            this.jpexsGroup.Location = new System.Drawing.Point(7, 186);
            this.jpexsGroup.Name = "jpexsGroup";
            this.jpexsGroup.Size = new System.Drawing.Size(772, 51);
            this.jpexsGroup.TabIndex = 21;
            this.jpexsGroup.TabStop = false;
            this.jpexsGroup.Text = "Decompilation options (JPEXS required)";
            // 
            // hideCmd
            // 
            this.hideCmd.AutoSize = true;
            this.hideCmd.Enabled = false;
            this.hideCmd.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.hideCmd.Location = new System.Drawing.Point(562, 22);
            this.hideCmd.Margin = new System.Windows.Forms.Padding(4);
            this.hideCmd.Name = "hideCmd";
            this.hideCmd.Size = new System.Drawing.Size(194, 21);
            this.hideCmd.TabIndex = 24;
            this.hideCmd.Text = "Hide Command Prompt";
            this.hideCmd.UseVisualStyleBackColor = true;
            this.hideCmd.CheckedChanged += new System.EventHandler(this.hideCMD_CheckedChanged);
            // 
            // reOrgDecomp
            // 
            this.reOrgDecomp.AutoSize = true;
            this.reOrgDecomp.Checked = true;
            this.reOrgDecomp.CheckState = System.Windows.Forms.CheckState.Checked;
            this.reOrgDecomp.Enabled = false;
            this.reOrgDecomp.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.reOrgDecomp.Location = new System.Drawing.Point(297, 22);
            this.reOrgDecomp.Margin = new System.Windows.Forms.Padding(4);
            this.reOrgDecomp.Name = "reOrgDecomp";
            this.reOrgDecomp.Size = new System.Drawing.Size(257, 21);
            this.reOrgDecomp.TabIndex = 23;
            this.reOrgDecomp.Text = "Reorganize after decompilation";
            this.reOrgDecomp.UseVisualStyleBackColor = true;
            // 
            // ffdecEnabled
            // 
            this.ffdecEnabled.AutoSize = true;
            this.ffdecEnabled.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.ffdecEnabled.Location = new System.Drawing.Point(7, 22);
            this.ffdecEnabled.Margin = new System.Windows.Forms.Padding(4);
            this.ffdecEnabled.Name = "ffdecEnabled";
            this.ffdecEnabled.Size = new System.Drawing.Size(282, 21);
            this.ffdecEnabled.TabIndex = 22;
            this.ffdecEnabled.Text = "Attempt to decompile automatically";
            this.ffdecEnabled.UseVisualStyleBackColor = true;
            this.ffdecEnabled.CheckedChanged += new System.EventHandler(this.JPEXStoggle_CheckedChanged);
            // 
            // ripRedundant
            // 
            this.ripRedundant.AutoSize = true;
            this.ripRedundant.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.ripRedundant.Location = new System.Drawing.Point(336, 22);
            this.ripRedundant.Margin = new System.Windows.Forms.Padding(4);
            this.ripRedundant.Name = "ripRedundant";
            this.ripRedundant.Size = new System.Drawing.Size(199, 21);
            this.ripRedundant.TabIndex = 22;
            this.ripRedundant.Text = "Overwrite existing files ";
            this.ripRedundant.UseVisualStyleBackColor = true;
            // 
            // skipNonFlashCheckBox
            // 
            this.skipNonFlashCheckBox.AutoSize = true;
            this.skipNonFlashCheckBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.skipNonFlashCheckBox.Location = new System.Drawing.Point(8, 22);
            this.skipNonFlashCheckBox.Margin = new System.Windows.Forms.Padding(4);
            this.skipNonFlashCheckBox.Name = "skipNonFlashCheckBox";
            this.skipNonFlashCheckBox.Size = new System.Drawing.Size(169, 21);
            this.skipNonFlashCheckBox.TabIndex = 23;
            this.skipNonFlashCheckBox.Text = "Skip non-flash files";
            this.skipNonFlashCheckBox.UseVisualStyleBackColor = true;
            this.skipNonFlashCheckBox.CheckedChanged += new System.EventHandler(this.skipNonFlash_CheckedChanged);
            // 
            // miscGroup
            // 
            this.miscGroup.Controls.Add(this.logErrors);
            this.miscGroup.Controls.Add(this.skipFlashCheckBox);
            this.miscGroup.Controls.Add(this.skipNonFlashCheckBox);
            this.miscGroup.Controls.Add(this.ripRedundant);
            this.miscGroup.Location = new System.Drawing.Point(7, 243);
            this.miscGroup.Name = "miscGroup";
            this.miscGroup.Size = new System.Drawing.Size(772, 52);
            this.miscGroup.TabIndex = 24;
            this.miscGroup.TabStop = false;
            this.miscGroup.Text = "Misc. options";
            // 
            // logErrors
            // 
            this.logErrors.AutoSize = true;
            this.logErrors.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.logErrors.Location = new System.Drawing.Point(543, 22);
            this.logErrors.Margin = new System.Windows.Forms.Padding(4);
            this.logErrors.Name = "logErrors";
            this.logErrors.Size = new System.Drawing.Size(212, 21);
            this.logErrors.TabIndex = 25;
            this.logErrors.Text = "Only log errors to history";
            this.logErrors.UseVisualStyleBackColor = true;
            // 
            // skipFlashCheckBox
            // 
            this.skipFlashCheckBox.AutoSize = true;
            this.skipFlashCheckBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.skipFlashCheckBox.Location = new System.Drawing.Point(185, 22);
            this.skipFlashCheckBox.Margin = new System.Windows.Forms.Padding(4);
            this.skipFlashCheckBox.Name = "skipFlashCheckBox";
            this.skipFlashCheckBox.Size = new System.Drawing.Size(136, 21);
            this.skipFlashCheckBox.TabIndex = 24;
            this.skipFlashCheckBox.Text = "Skip flash files";
            this.skipFlashCheckBox.UseVisualStyleBackColor = true;
            // 
            // encryptionGroup
            // 
            this.encryptionGroup.Controls.Add(this.decEnabled);
            this.encryptionGroup.Controls.Add(this.encryptKey);
            this.encryptionGroup.Controls.Add(this.encLabel);
            this.encryptionGroup.Controls.Add(this.reEncLabel);
            this.encryptionGroup.Controls.Add(this.reEncEnabled);
            this.encryptionGroup.Controls.Add(this.reEncryptKey);
            this.encryptionGroup.Location = new System.Drawing.Point(6, 103);
            this.encryptionGroup.Name = "encryptionGroup";
            this.encryptionGroup.Size = new System.Drawing.Size(773, 77);
            this.encryptionGroup.TabIndex = 25;
            this.encryptionGroup.TabStop = false;
            this.encryptionGroup.Text = "Encryption options";
            // 
            // logHistory
            // 
            this.logHistory.Location = new System.Drawing.Point(6, 365);
            this.logHistory.Name = "logHistory";
            this.logHistory.ReadOnly = true;
            this.logHistory.Size = new System.Drawing.Size(773, 147);
            this.logHistory.TabIndex = 26;
            this.logHistory.Text = "(Actions will be logged here.)";
            this.logHistory.WordWrap = false;
            this.logHistory.TextChanged += new System.EventHandler(this.richTextBox1_TextChanged);
            // 
            // MainControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(788, 540);
            this.Controls.Add(this.logHistory);
            this.Controls.Add(this.encryptionGroup);
            this.Controls.Add(this.miscGroup);
            this.Controls.Add(this.jpexsGroup);
            this.Controls.Add(this.duration);
            this.Controls.Add(this.log);
            this.Controls.Add(this.CCCheck);
            this.Controls.Add(this.themeCCCheck);
            this.Controls.Add(this.themeCheck);
            this.Controls.Add(this.domainInput);
            this.Controls.Add(this.domainLabel);
            this.Controls.Add(this.tIdLabel);
            this.Controls.Add(this.themeIdInput);
            this.Controls.Add(this.ripButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.Name = "MainControl";
            this.Text = "GoAnimateRipper2 v1.6.1b";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.jpexsGroup.ResumeLayout(false);
            this.jpexsGroup.PerformLayout();
            this.miscGroup.ResumeLayout(false);
            this.miscGroup.PerformLayout();
            this.encryptionGroup.ResumeLayout(false);
            this.encryptionGroup.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button ripButton;
        private System.Windows.Forms.ComboBox encryptKey;
        private System.Windows.Forms.Label encLabel;
        private System.Windows.Forms.CheckBox decEnabled;
        private System.Windows.Forms.TextBox themeIdInput;
        private System.Windows.Forms.Label tIdLabel;
        private System.Windows.Forms.Label domainLabel;
        private System.Windows.Forms.TextBox domainInput;
        private System.Windows.Forms.RadioButton themeCheck;
        private System.Windows.Forms.RadioButton themeCCCheck;
        private System.Windows.Forms.RadioButton CCCheck;
        private System.Windows.Forms.Label log;
        private System.Windows.Forms.ProgressBar duration;
        private System.Windows.Forms.CheckBox reEncEnabled;
        private System.Windows.Forms.Label reEncLabel;
        private System.Windows.Forms.ComboBox reEncryptKey;
        private System.Windows.Forms.GroupBox jpexsGroup;
        private System.Windows.Forms.CheckBox hideCmd;
        private System.Windows.Forms.CheckBox reOrgDecomp;
        private System.Windows.Forms.CheckBox ffdecEnabled;
        private System.Windows.Forms.CheckBox ripRedundant;
        private System.Windows.Forms.CheckBox skipNonFlashCheckBox;
        private System.Windows.Forms.GroupBox miscGroup;
        private System.Windows.Forms.GroupBox encryptionGroup;
        private System.Windows.Forms.RichTextBox logHistory;
        private System.Windows.Forms.CheckBox skipFlashCheckBox;
        private System.Windows.Forms.CheckBox logErrors;
    }
}

