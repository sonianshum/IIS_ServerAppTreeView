namespace Starling.TwoFactor.HttpModule.Settings
{
    using System.Collections.Generic;
    using System.Linq;
    using Starling.TwoFactor.HttpModule.Common;
    using Starling.TwoFactor.HttpModule.Configuration;
    using System;
    using System.Windows.Forms;
    using Starling.TwoFactor.HttpModule.Settings.Properties;

    public partial class AdvancedSettings : Form
    {
        private bool _isDirty;
        private const string Email = "E-mail";
        private const string Phone = "Mobile";
        private const string UserName = "Logon Name";
        private readonly string[] _userAttributes = { UserName, Email, Phone };
        private ListViewHitTestInfo _hitinfo;
        private readonly TextBox _editbox = new TextBox();
        internal bool restartRequired = false;

        private enum UserAttributes
        {
            Name,
            Email,
            Phone
        }
        public AdvancedSettings()
        {
            InitializeComponent();
            AddOnChangeEventHandlers();
        }

        private void AddOnChangeEventHandlers()
        {
            //Add event handler for the form dirty things.
           this._editbox.TextChanged += this.SetFormDirty;
        }

        private void SetFormDirty(object sender, EventArgs e)
        {
            _isDirty = true;
            this.buttonOk.Enabled = true;
        }


        private void buttonOk_Click(object sender, EventArgs e)
        {
            var values = GetAttributeValues();

            if (ConfigurationFactory.Instance != null)
            {
                for (var i = 0; i < _userAttributes.Length; i++)
                {
                    switch ((UserAttributes) i)
                    {
                        case UserAttributes.Email:
                        {
                            if (ConfigurationFactory.Instance.HttpModuleConfig.UserRepositorySettings.ActiveDirectory.MapAttributes.Email != values.ToArray()[i])
                            {
                                ConfigurationFactory.Instance.HttpModuleConfig.UserRepositorySettings.ActiveDirectory
                                    .MapAttributes.Email = values.ToArray()[i];                                    
                                this._isDirty = true;
                                 restartRequired = true;
                            }
                            break;
                        }

                        case UserAttributes.Phone:
                        {
                            if (ConfigurationFactory.Instance.HttpModuleConfig.UserRepositorySettings.ActiveDirectory.MapAttributes.Mobile != values.ToArray()[i])
                            {
                                ConfigurationFactory.Instance.HttpModuleConfig.UserRepositorySettings.ActiveDirectory
                                    .MapAttributes.Mobile = values.ToArray()[i];
                                this._isDirty = true;
                                    restartRequired = true;
                                }

                            break;
                        }
                        case UserAttributes.Name:
                        {
                            if (ConfigurationFactory.Instance.HttpModuleConfig.UserRepositorySettings.ActiveDirectory.MapAttributes.UserName != values.ToArray()[i])
                            {
                                ConfigurationFactory.Instance.HttpModuleConfig.UserRepositorySettings.ActiveDirectory
                                    .MapAttributes.UserName = values.ToArray()[i];
                                this._isDirty = true;
                                    restartRequired = true;
                                }

                            break;
                        }
                        default:
                            throw new HttpModuleException(Resources.UnknownAttribute);
                    }
                }
            }
            this.DialogResult = this._isDirty ? DialogResult.OK : DialogResult.Cancel;
            this._isDirty = false;
            this.Close();
        }

        private List<string> GetAttributeValues()
        {
            return listViewAttributeMap.Items.Cast<ListViewItem>().Select(lvi => lvi.SubItems.Count - 1 < 1 ? "" : lvi.SubItems[1].Text).ToList();
        }

        private void LoadAttributeMapping()
        {
            foreach (var attribute in _userAttributes)
            {
                var listViewItem = new ListViewItem(attribute);
                string adAttribute;
                switch (attribute)
                {
                    case Email:
                        adAttribute =
                            ConfigurationFactory.Instance.HttpModuleConfig.UserRepositorySettings.ActiveDirectory
                                .MapAttributes.Email;
                        break;
                    case Phone:
                        adAttribute = ConfigurationFactory.Instance.HttpModuleConfig.UserRepositorySettings.ActiveDirectory
                                .MapAttributes.Mobile;
                        break;
                    case UserName:
                        adAttribute = ConfigurationFactory.Instance.HttpModuleConfig.UserRepositorySettings.ActiveDirectory
                                .MapAttributes.UserName;
                        break;
                    default:
                        adAttribute = string.Empty;
                        break;
                }
                listViewItem.SubItems.Add(adAttribute);
                listViewAttributeMap.Items.Add(listViewItem);

                _editbox.Parent = listViewAttributeMap;
                _editbox.Hide();
                _editbox.LostFocus += editbox_LostFocus;
                _editbox.KeyPress += editbox_KeyDown;

                listViewAttributeMap.MouseDoubleClick += listView_MouseDoubleClick;
                listViewAttributeMap.FullRowSelect = true;
            }
        }

        #region Events
        private void editbox_KeyDown(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13 || e.KeyChar == (char)27)
            {
                this._isDirty = true;
                _hitinfo.SubItem.Text = _editbox.Text;
                _editbox.Hide();
            }
        }

        private void listView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            _hitinfo = listViewAttributeMap.HitTest(e.X, e.Y);
            _editbox.Bounds = _hitinfo.SubItem.Bounds;
            var result = Array.Find(_userAttributes, s => s.Equals(_hitinfo.SubItem.Text));
            if (result == null)
            {
                _editbox.Text = _hitinfo.SubItem.Text;
                _editbox.Focus();
                _editbox.Show();
            }
        }

        private void editbox_LostFocus(object sender, EventArgs e)
        {
            this._isDirty = true;
            _hitinfo.SubItem.Text = _editbox.Text;
            _editbox.Hide();
        }

        private void listView1_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            e.Cancel = true;
            e.NewWidth = listViewAttributeMap.Columns[e.ColumnIndex].Width;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this._isDirty = false;
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void AdvancedSettings_Load(object sender, EventArgs e)
        {
            this._isDirty = false;
            LoadAttributeMapping();
        }

        #endregion
    }
}
