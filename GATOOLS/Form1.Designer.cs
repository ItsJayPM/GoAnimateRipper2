
namespace GATOOLS
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.button1 = new System.Windows.Forms.Button();
            this.encrypt = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.tid = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.dom = new System.Windows.Forms.TextBox();
            this.ThemeCheck = new System.Windows.Forms.RadioButton();
            this.ThemeCCCheck = new System.Windows.Forms.RadioButton();
            this.CCCheck = new System.Windows.Forms.RadioButton();
            this.log = new System.Windows.Forms.Label();
            this.duration = new System.Windows.Forms.ProgressBar();
            this.reencrypt = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.key2 = new System.Windows.Forms.ComboBox();
            this.experimentalGroup = new System.Windows.Forms.GroupBox();
            this.hideCMD = new System.Windows.Forms.CheckBox();
            this.deleteAfter = new System.Windows.Forms.CheckBox();
            this.JPEXStoggle = new System.Windows.Forms.CheckBox();
            this.experimentalGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(7, 211);
            this.button1.Margin = new System.Windows.Forms.Padding(4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(773, 30);
            this.button1.TabIndex = 0;
            this.button1.Text = "Start Ripping";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // encrypt
            // 
            this.encrypt.AccessibleName = "";
            this.encrypt.FormattingEnabled = true;
            this.encrypt.Items.AddRange(new object[] {
            "g0o1a2n3i4m5a6t7e",
            "sorrypleasetryagainlater"});
            this.encrypt.Location = new System.Drawing.Point(471, 99);
            this.encrypt.Margin = new System.Windows.Forms.Padding(4);
            this.encrypt.Name = "encrypt";
            this.encrypt.Size = new System.Drawing.Size(309, 24);
            this.encrypt.TabIndex = 1;
            this.encrypt.Text = "(auto)";
            this.encrypt.SelectedIndexChanged += new System.EventHandler(this.encrypt_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(355, 106);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label1.Size = new System.Drawing.Size(106, 16);
            this.label1.TabIndex = 2;
            this.label1.Text = "Encrypytion Key:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.checkBox1.Location = new System.Drawing.Point(7, 104);
            this.checkBox1.Margin = new System.Windows.Forms.Padding(4);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(172, 21);
            this.checkBox1.TabIndex = 3;
            this.checkBox1.Text = "Decryption Enabled";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // tid
            // 
            this.tid.Location = new System.Drawing.Point(81, 15);
            this.tid.Margin = new System.Windows.Forms.Padding(4);
            this.tid.Name = "tid";
            this.tid.Size = new System.Drawing.Size(698, 22);
            this.tid.TabIndex = 7;
            this.tid.Text = "retro";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 18);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 16);
            this.label3.TabIndex = 8;
            this.label3.Text = "Theme ID:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(4, 52);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(57, 16);
            this.label4.TabIndex = 9;
            this.label4.Text = "Domain:";
            // 
            // dom
            // 
            this.dom.Location = new System.Drawing.Point(81, 48);
            this.dom.Margin = new System.Windows.Forms.Padding(4);
            this.dom.Name = "dom";
            this.dom.Size = new System.Drawing.Size(698, 22);
            this.dom.TabIndex = 10;
            this.dom.Text = "https://flashthemes.net/static/store/";
            this.dom.TextChanged += new System.EventHandler(this.dom_TextChanged);
            // 
            // ThemeCheck
            // 
            this.ThemeCheck.AutoSize = true;
            this.ThemeCheck.Checked = true;
            this.ThemeCheck.Location = new System.Drawing.Point(7, 77);
            this.ThemeCheck.Name = "ThemeCheck";
            this.ThemeCheck.Size = new System.Drawing.Size(71, 20);
            this.ThemeCheck.TabIndex = 13;
            this.ThemeCheck.TabStop = true;
            this.ThemeCheck.Text = "Theme\r\n";
            this.ThemeCheck.UseVisualStyleBackColor = true;
            this.ThemeCheck.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // ThemeCCCheck
            // 
            this.ThemeCCCheck.AutoSize = true;
            this.ThemeCCCheck.Location = new System.Drawing.Point(83, 77);
            this.ThemeCCCheck.Name = "ThemeCCCheck";
            this.ThemeCCCheck.Size = new System.Drawing.Size(299, 20);
            this.ThemeCCCheck.TabIndex = 14;
            this.ThemeCCCheck.Text = "Theme (+Attatched Character Creator Theme)";
            this.ThemeCCCheck.UseVisualStyleBackColor = true;
            this.ThemeCCCheck.CheckedChanged += new System.EventHandler(this.ThemeCCCheck_CheckedChanged);
            // 
            // CCCheck
            // 
            this.CCCheck.AutoSize = true;
            this.CCCheck.Location = new System.Drawing.Point(403, 77);
            this.CCCheck.Name = "CCCheck";
            this.CCCheck.Size = new System.Drawing.Size(179, 20);
            this.CCCheck.TabIndex = 15;
            this.CCCheck.Text = "Character Creator Theme";
            this.CCCheck.UseVisualStyleBackColor = true;
            this.CCCheck.CheckedChanged += new System.EventHandler(this.CCCheck_CheckedChanged);
            // 
            // log
            // 
            this.log.AutoSize = true;
            this.log.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.log.Location = new System.Drawing.Point(4, 274);
            this.log.Name = "log";
            this.log.Size = new System.Drawing.Size(323, 16);
            this.log.TabIndex = 16;
            this.log.Text = "(When an action is preformed, logging appears here.)";
            // 
            // duration
            // 
            this.duration.BackColor = System.Drawing.SystemColors.Window;
            this.duration.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.duration.Location = new System.Drawing.Point(7, 248);
            this.duration.Maximum = 0;
            this.duration.Name = "duration";
            this.duration.Size = new System.Drawing.Size(773, 23);
            this.duration.TabIndex = 17;
            // 
            // reencrypt
            // 
            this.reencrypt.AutoSize = true;
            this.reencrypt.Enabled = false;
            this.reencrypt.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.reencrypt.Location = new System.Drawing.Point(7, 127);
            this.reencrypt.Margin = new System.Windows.Forms.Padding(4);
            this.reencrypt.Name = "reencrypt";
            this.reencrypt.Size = new System.Drawing.Size(174, 21);
            this.reencrypt.TabIndex = 18;
            this.reencrypt.Text = "Re-encrypt Enabled";
            this.reencrypt.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(334, 130);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(127, 16);
            this.label2.TabIndex = 20;
            this.label2.Text = "Re-encrypytion Key:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.label2.Click += new System.EventHandler(this.label2_Click_1);
            // 
            // key2
            // 
            this.key2.AccessibleName = "";
            this.key2.FormattingEnabled = true;
            this.key2.Items.AddRange(new object[] {
            "g0o1a2n3i4m5a6t7e",
            "sorrypleasetryagainlater"});
            this.key2.Location = new System.Drawing.Point(471, 127);
            this.key2.Margin = new System.Windows.Forms.Padding(4);
            this.key2.Name = "key2";
            this.key2.Size = new System.Drawing.Size(309, 24);
            this.key2.TabIndex = 19;
            this.key2.Text = "sorrypleasetryagainlater";
            // 
            // experimentalGroup
            // 
            this.experimentalGroup.Controls.Add(this.hideCMD);
            this.experimentalGroup.Controls.Add(this.deleteAfter);
            this.experimentalGroup.Controls.Add(this.JPEXStoggle);
            this.experimentalGroup.Location = new System.Drawing.Point(7, 156);
            this.experimentalGroup.Name = "experimentalGroup";
            this.experimentalGroup.Size = new System.Drawing.Size(773, 51);
            this.experimentalGroup.TabIndex = 21;
            this.experimentalGroup.TabStop = false;
            this.experimentalGroup.Text = "JPEXS";
            // 
            // hideCMD
            // 
            this.hideCMD.AutoSize = true;
            this.hideCMD.Checked = true;
            this.hideCMD.CheckState = System.Windows.Forms.CheckState.Checked;
            this.hideCMD.Enabled = false;
            this.hideCMD.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.hideCMD.Location = new System.Drawing.Point(567, 22);
            this.hideCMD.Margin = new System.Windows.Forms.Padding(4);
            this.hideCMD.Name = "hideCMD";
            this.hideCMD.Size = new System.Drawing.Size(194, 21);
            this.hideCMD.TabIndex = 24;
            this.hideCMD.Text = "Hide Command Prompt";
            this.hideCMD.UseVisualStyleBackColor = true;
            this.hideCMD.CheckedChanged += new System.EventHandler(this.hideCMD_CheckedChanged);
            // 
            // deleteAfter
            // 
            this.deleteAfter.AutoSize = true;
            this.deleteAfter.Enabled = false;
            this.deleteAfter.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.deleteAfter.Location = new System.Drawing.Point(309, 22);
            this.deleteAfter.Margin = new System.Windows.Forms.Padding(4);
            this.deleteAfter.Name = "deleteAfter";
            this.deleteAfter.Size = new System.Drawing.Size(232, 21);
            this.deleteAfter.TabIndex = 23;
            this.deleteAfter.Text = "Delete SWF after decompile";
            this.deleteAfter.UseVisualStyleBackColor = true;
            // 
            // JPEXStoggle
            // 
            this.JPEXStoggle.AutoSize = true;
            this.JPEXStoggle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.JPEXStoggle.Location = new System.Drawing.Point(7, 22);
            this.JPEXStoggle.Margin = new System.Windows.Forms.Padding(4);
            this.JPEXStoggle.Name = "JPEXStoggle";
            this.JPEXStoggle.Size = new System.Drawing.Size(282, 21);
            this.JPEXStoggle.TabIndex = 22;
            this.JPEXStoggle.Text = "Attempt to decompile automatically";
            this.JPEXStoggle.UseVisualStyleBackColor = true;
            this.JPEXStoggle.CheckedChanged += new System.EventHandler(this.JPEXStoggle_CheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(788, 295);
            this.Controls.Add(this.experimentalGroup);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.key2);
            this.Controls.Add(this.reencrypt);
            this.Controls.Add(this.duration);
            this.Controls.Add(this.log);
            this.Controls.Add(this.CCCheck);
            this.Controls.Add(this.ThemeCCCheck);
            this.Controls.Add(this.ThemeCheck);
            this.Controls.Add(this.dom);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tid);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.encrypt);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "GoAnimate Ripper 2";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.experimentalGroup.ResumeLayout(false);
            this.experimentalGroup.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ComboBox encrypt;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.TextBox tid;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox dom;
        private System.Windows.Forms.RadioButton ThemeCheck;
        private System.Windows.Forms.RadioButton ThemeCCCheck;
        private System.Windows.Forms.RadioButton CCCheck;
        private System.Windows.Forms.Label log;
        private System.Windows.Forms.ProgressBar duration;
        private System.Windows.Forms.CheckBox reencrypt;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox key2;
        private System.Windows.Forms.GroupBox experimentalGroup;
        private System.Windows.Forms.CheckBox hideCMD;
        private System.Windows.Forms.CheckBox deleteAfter;
        private System.Windows.Forms.CheckBox JPEXStoggle;
    }
}

