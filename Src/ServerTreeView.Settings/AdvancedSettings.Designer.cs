using System.Windows.Forms;

namespace Starling.TwoFactor.HttpModule.Settings
{
    using Starling.TwoFactor.HttpModule.Settings.Properties;

    partial class AdvancedSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ServerSites));
            this.groupBoxMappedAttributes = new System.Windows.Forms.GroupBox();
            this.listViewAttributeMap = new System.Windows.Forms.ListView();
            this.columnAttibuteName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnAdAttribute = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.labelUserRepositoryMessage = new System.Windows.Forms.Label();
            this.buttonOk = new System.Windows.Forms.Button();
            this.textBoxAttributeMessage = new TextBox();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBoxMappedAttributes.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxMappedAttributes
            // 
            this.groupBoxMappedAttributes.Controls.Add(this.listViewAttributeMap);
            this.groupBoxMappedAttributes.Controls.Add(this.textBoxAttributeMessage);
            this.groupBoxMappedAttributes.Location = new System.Drawing.Point(10, 34);
            this.groupBoxMappedAttributes.Name = "groupBoxMappedAttributes";
            this.groupBoxMappedAttributes.Size = new System.Drawing.Size(366, 170);
            this.groupBoxMappedAttributes.TabIndex = 1;
            this.groupBoxMappedAttributes.TabStop = false;
            this.groupBoxMappedAttributes.Text = Resources.LableMappedAttributes;
            // 
            // listViewAttributeMap
            // 
            this.listViewAttributeMap.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnAttibuteName,
            this.columnAdAttribute});
            this.listViewAttributeMap.FullRowSelect = true;
            this.listViewAttributeMap.GridLines = true;
            this.listViewAttributeMap.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listViewAttributeMap.Location = new System.Drawing.Point(15, 28);
            this.listViewAttributeMap.Name = "listViewAttributeMap";
            this.listViewAttributeMap.Size = new System.Drawing.Size(334, 82);
            this.listViewAttributeMap.TabIndex = 0;
            this.listViewAttributeMap.UseCompatibleStateImageBehavior = false;
            this.listViewAttributeMap.View = System.Windows.Forms.View.Details;
            this.listViewAttributeMap.ColumnWidthChanging += new System.Windows.Forms.ColumnWidthChangingEventHandler(this.listView1_ColumnWidthChanging);
            // 
            // columnAttibuteName
            // 
            this.columnAttibuteName.Text = global::Starling.TwoFactor.HttpModule.Settings.Properties.Resources.LabelAttributename;
            this.columnAttibuteName.Width = 155;
            // 
            // columnAdAttribute
            // 
            this.columnAdAttribute.Text = global::Starling.TwoFactor.HttpModule.Settings.Properties.Resources.LabelAdAttribute;
            this.columnAdAttribute.Width = 174;
            // 
            // buttonOk
            // 
            this.buttonOk.Enabled = false;
            this.buttonOk.Location = new System.Drawing.Point(128, 251);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 3;
            this.buttonOk.Text = global::Starling.TwoFactor.HttpModule.Settings.Properties.Resources.LabelOk;
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(210, 251);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 0;
            this.buttonCancel.Text = global::Starling.TwoFactor.HttpModule.Settings.Properties.Resources.LabelCancel;
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);

            // 
            // textBox1
            // 
            this.textBoxAttributeMessage.BackColor = System.Drawing.SystemColors.Menu;
            this.textBoxAttributeMessage.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxAttributeMessage.HideSelection = false;
            this.textBoxAttributeMessage.Location = new System.Drawing.Point(20, 120);
            this.textBoxAttributeMessage.Multiline = true;
            this.textBoxAttributeMessage.Name = "textBoxAttributeMessage";
            this.textBoxAttributeMessage.ReadOnly = true;
            this.textBoxAttributeMessage.Size = new System.Drawing.Size(340, 41);
            this.textBoxAttributeMessage.TabIndex = 1;
            this.textBoxAttributeMessage.Text = Resources.UserNameisOptional;
            // 
            // AdvancedSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(396, 286);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Controls.Add(this.labelUserRepositoryMessage);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.groupBoxMappedAttributes);
            this.MaximizeBox = false;
            this.Name = "AdvancedSettings";
            this.Text = Resources.CaptionAdvancedSettings;
            this.Load += new System.EventHandler(this.AdvancedSettings_Load);
            this.groupBoxMappedAttributes.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.GroupBox groupBoxMappedAttributes;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonCancel;
        private TextBox textBoxAttributeMessage;
        private System.Windows.Forms.ListView listViewAttributeMap;
        private System.Windows.Forms.ColumnHeader columnAttibuteName;
        private System.Windows.Forms.ColumnHeader columnAdAttribute;

        #endregion

        private Label labelUserRepositoryMessage;
    }
}