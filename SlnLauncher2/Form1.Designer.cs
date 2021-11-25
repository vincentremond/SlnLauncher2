using System;
using System.Windows.Forms;

namespace SlnLauncher2
{
    partial class SlnLauncher
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SlnLauncher));
            this.lstSln = new System.Windows.Forms.ListBox();
            this.tbxSearch = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lstBaseFolders = new System.Windows.Forms.ListBox();
            this.buttonReload = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lstSln
            // 
            this.lstSln.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstSln.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lstSln.FormattingEnabled = true;
            this.lstSln.ItemHeight = 17;
            this.lstSln.Location = new System.Drawing.Point(316, 62);
            this.lstSln.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.lstSln.Name = "lstSln";
            this.lstSln.Size = new System.Drawing.Size(933, 718);
            this.lstSln.TabIndex = 2;
            this.lstSln.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lstSln_MouseDoubleClick);
            // 
            // tbxSearch
            // 
            this.tbxSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbxSearch.Location = new System.Drawing.Point(316, 19);
            this.tbxSearch.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tbxSearch.Name = "tbxSearch";
            this.tbxSearch.ShortcutsEnabled = false;
            this.tbxSearch.Size = new System.Drawing.Size(901, 27);
            this.tbxSearch.TabIndex = 1;
            this.tbxSearch.TextChanged += new System.EventHandler(this.tbxSearch_TextChanged);
            this.tbxSearch.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tbxSearch_KeyUp);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 548);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(229, 160);
            this.label1.TabIndex = 4;
            this.label1.Text = resources.GetString("label1.Text");
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // lstBaseFolders
            // 
            this.lstBaseFolders.FormattingEnabled = true;
            this.lstBaseFolders.ItemHeight = 20;
            this.lstBaseFolders.Location = new System.Drawing.Point(16, 19);
            this.lstBaseFolders.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.lstBaseFolders.Name = "lstBaseFolders";
            this.lstBaseFolders.Size = new System.Drawing.Size(291, 524);
            this.lstBaseFolders.TabIndex = 6;
            this.lstBaseFolders.SelectedIndexChanged += new System.EventHandler(this.lstBranch_SelectedIndexChanged);
            // 
            // buttonReload
            // 
            this.buttonReload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonReload.Location = new System.Drawing.Point(1224, 18);
            this.buttonReload.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.buttonReload.Name = "buttonReload";
            this.buttonReload.Size = new System.Drawing.Size(25, 31);
            this.buttonReload.TabIndex = 7;
            this.buttonReload.Text = "🔄";
            this.buttonReload.UseVisualStyleBackColor = true;
            this.buttonReload.Click += new System.EventHandler(this.buttonReload_Click);
            // 
            // SlnLauncher
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1267, 799);
            this.Controls.Add(this.buttonReload);
            this.Controls.Add(this.lstBaseFolders);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbxSearch);
            this.Controls.Add(this.lstSln);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "SlnLauncher";
            this.Text = "SlnLauncher";
            this.Activated += new System.EventHandler(this.Form1_OnGotFocus);
            this.Load += new System.EventHandler(this.SlnLauncher_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Button buttonReload;

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox lstBaseFolders;
        private System.Windows.Forms.ListBox lstSln;
        private System.Windows.Forms.TextBox tbxSearch;

        #endregion
    }
}

