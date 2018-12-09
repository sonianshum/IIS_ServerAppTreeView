
namespace ServerTreeView.Settings
{
    using System.Windows.Forms;
    using Properties;

    partial class ServerSites
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

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ServerSites));
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonApply = new System.Windows.Forms.Button();
            this.tabProtectedSite = new System.Windows.Forms.TabPage();
            this.groupBoxProtectedSites = new System.Windows.Forms.GroupBox();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.labelProtectedSiteMessage = new System.Windows.Forms.Label();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabProtectedSite.SuspendLayout();
            this.groupBoxProtectedSites.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonOK
            // 
            this.buttonOK.Enabled = false;
            this.buttonOK.Location = new System.Drawing.Point(251, 283);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 1;
            this.buttonOK.Text = Properties.Resources.LabelOk;
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.OnOk_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(332, 283);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = Properties.Resources.LabelCancel;
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.OnCancel_Click);
            // 
            // buttonApply
            // 
            this.buttonApply.Enabled = false;
            this.buttonApply.Location = new System.Drawing.Point(413, 283);
            this.buttonApply.Name = "buttonApply";
            this.buttonApply.Size = new System.Drawing.Size(75, 23);
            this.buttonApply.TabIndex = 3;
            this.buttonApply.Text = Properties.Resources.LabelApply;
            this.buttonApply.UseVisualStyleBackColor = true;
            this.buttonApply.Click += new System.EventHandler(this.OnApply_Click);
            // 
            // tabProtectedSite
            // 
            this.tabProtectedSite.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tabProtectedSite.Controls.Add(this.groupBoxProtectedSites);
            this.tabProtectedSite.Location = new System.Drawing.Point(4, 22);
            this.tabProtectedSite.Name = "tabProtectedSite";
            this.tabProtectedSite.Padding = new System.Windows.Forms.Padding(3);
            this.tabProtectedSite.Size = new System.Drawing.Size(478, 231);
            this.tabProtectedSite.TabIndex = 2;
            this.tabProtectedSite.Text = Properties.Resources.LabelProtectedSites;
            // 
            // groupBoxProtectedSites
            // 
            this.groupBoxProtectedSites.Controls.Add(this.treeView1);
            this.groupBoxProtectedSites.Controls.Add(this.labelProtectedSiteMessage);
            this.groupBoxProtectedSites.Location = new System.Drawing.Point(8, 3);
            this.groupBoxProtectedSites.Name = "groupBoxProtectedSites";
            this.groupBoxProtectedSites.Size = new System.Drawing.Size(464, 222);
            this.groupBoxProtectedSites.TabIndex = 0;
            this.groupBoxProtectedSites.TabStop = false;
            // 
            // treeView1
            // 
            this.treeView1.CheckBoxes = true;
            this.treeView1.Location = new System.Drawing.Point(15, 45);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(427, 158);
            this.treeView1.TabIndex = 6;
            this.treeView1.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.treeView_AfterCheck);
            // 
            // labelProtectedSiteMessage
            // 
            this.labelProtectedSiteMessage.AutoSize = true;
            this.labelProtectedSiteMessage.Location = new System.Drawing.Point(15, 12);
            this.labelProtectedSiteMessage.Name = "labelProtectedSiteMessage";
            this.labelProtectedSiteMessage.Size = new System.Drawing.Size(406, 26);
            this.labelProtectedSiteMessage.TabIndex = 5;
            this.labelProtectedSiteMessage.Text = "To add an http module in any server site(s) and web application(s), select checkb" +
    "ox, \r\nTo remove, uncheck.";
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabProtectedSite);
            this.tabControl.Location = new System.Drawing.Point(12, 15);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(486, 257);
            this.tabControl.TabIndex = 0;
            // 
            // ServerSites
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(507, 321);
            this.Controls.Add(this.buttonApply);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.tabControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Load += new System.EventHandler(this.OnLoad);
            this.tabProtectedSite.ResumeLayout(false);
            this.groupBoxProtectedSites.ResumeLayout(false);
            this.groupBoxProtectedSites.PerformLayout();
            this.tabControl.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonApply;
        private TabPage tabProtectedSite;
        private GroupBox groupBoxProtectedSites;
        private TreeView treeView1;
        private Label labelProtectedSiteMessage;
        private TabControl tabControl;
    }
}

#endregion