namespace ServerTreeView.Settings
{
    #region using statements
    using System;
    using System.Net;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net.NetworkInformation;
    using System.Windows.Forms;
    using Properties;
    using Microsoft.Web.Administration;
    using System.Collections.ObjectModel;    
    using Common;
    using Configuration;  
    using System.IO;
    using System.Reflection;
    using System.Security;
    using System.ServiceProcess;
    #endregion

#pragma warning disable S110 // Inheritance tree of classes should not be too deep
    public partial class ServerSites : Form
#pragma warning restore S110 // Inheritance tree of classes should not be too deep
    {
        internal virtual bool IsDirty { get; set; }
        internal virtual bool IsProtectedSitesSettingsChanged { get; set; }
        private static string AssemblyFullName { get; set; }
        private readonly Dictionary<string, TreeNode> _treeMap = new Dictionary<string, TreeNode>();       
        private bool _restartRequired;
        private bool _isChecked;

        public ServerSites()
        {
            InitializeComponent();
        }

        private void OnLoad(object sender, EventArgs e)
        {
            CheckMandatorySettings();
            GetFullyQualifiedAssemblyName();
            InitializeAvailableSitesTreeView();            
            ReadProtectedSitesTreeViewSettings();
            AddOnChangeEventHandlers();
        }

        /// <summary>
        /// Function to check mandatory IIS settings before launching the Config tool
        /// </summary>
        private void CheckMandatorySettings()
        {
            //1. Check IIS is in running state
            if (!IsIisRunning())
            {
                var error = string.Format(CultureInfo.InvariantCulture, Resources.IsServerRunning);
                Logger.Error(error);

                CustomMessageBox.Show(this, error,
                    Resources.AssemblyLabel,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button1,
                    0);

                System.Environment.Exit(0);
            }
            if (!CheckServerAccess())
            {
                System.Environment.Exit(0);
            }
        }

        /// <summary>
        /// Check the IIS server status
        /// </summary>
        /// <returns></returns>
        private static bool IsIisRunning()
        {
            bool isRunning = false;

            var controller = new ServiceController(Resources.IISServiceName);
            if (controller.Status == ServiceControllerStatus.Running)
            {
                isRunning = true;
            }
            return isRunning;
        }

        /// <summary>
        /// Mandatory condition checks before launching Config tool
        /// </summary>
        /// <returns></returns>
        private bool CheckServerAccess()
        {
            try
            {
                //check if all sites are in stopped condition
                using (var serverManager = ServerManager.OpenRemote(Resources.ServerIpAddress))
                {
                    //1. site count 0 or all Stopped
                    if ((serverManager.Sites.Count == 0) || serverManager.Sites.All(x => (x.State == ObjectState.Stopped)))
                    {
                        var error = string.Format(CultureInfo.InvariantCulture, Resources.NoRegisteredServerSite);
                        Logger.Error(error);

                        CustomMessageBox.Show(this, error,
                            Resources.AssemblyLabel,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning,
                            MessageBoxDefaultButton.Button1,
                            0);

                        return false;
                    }
                }
            }
            catch (Exception exp)
            {
                var error = string.Format(CultureInfo.InvariantCulture, Resources.RunAsAdministrator, exp.Message);
                Logger.Error(error);

                CustomMessageBox.Show(this,
                    error,
                    Resources.AssemblyLabel,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button1,
                    0);
                return false;

            }
            return true;
        }

        #region Configuration

        private bool ValidateConfiguration()
        {        
            if (!ValidateProtectedSiteTab())
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validate Protected site tab
        /// </summary>
        /// <returns></returns>
        private bool ValidateProtectedSiteTab()
        {
            var isValid = true;

            //validate key with Client
            try
            {
                if ((ConfigurationFactory.Instance.HttpModuleConfig.ProtectedSites.All(x => !x.IsChecked)) && (!HasCheckedChildNode(treeView1.Nodes)))
                {
                    Logger.Error(Resources.NoSiteSelectedForAuthentication);
                    ShowErrorMessageBox(Resources.NoSiteSelectedForAuthentication);
                    return false;

                }
            }
            catch (Exception exp)
            {
                Logger.Error(exp.Message);
                isValid = ShowErrorMessageBox(Resources.ErrorValidateProtectedSiteSettings);
            }
            return isValid;
        }


        /// <summary>
        /// Verify if any child node is selected
        /// </summary>
        /// <param name="Nodes"></param>
        /// <returns></returns>
        private bool HasCheckedChildNode(TreeNodeCollection Nodes)
        {
            _isChecked = false;

            foreach (TreeNode node in Nodes)
            {
                if (node.Checked)
                {
                    return true;
                }

                if ((node.Nodes.Count > 0) && (node.Nodes.Cast<TreeNode>().Any(child => child.Checked)))
                {
                    return true;
                }

                GetNodeRecursive(node);
            }

            return _isChecked;
        }

        private void GetNodeRecursive(TreeNode treeNode)
        {
            if (treeNode.Checked)
            {
                _isChecked = true;
            }

            foreach (TreeNode tn in treeNode.Nodes)
            {
                GetNodeRecursive(tn);
            }
        }

        #endregion

        #region ProtectedSite

        /// <summary>
        /// Function to read the current ProtectedSites & applications from config.xml file.
        /// </summary>
        private void ReadProtectedSitesTreeViewSettings()
        {
            try
            {
                //read existing config protectedsites
                var sites = ConfigurationFactory.Instance.HttpModuleConfig.ProtectedSites;

                foreach (var site in sites)
                {
                    foreach (var node in treeView1.Nodes.Cast<TreeNode>().Where(node => site.Name.Equals(node.Text)))
                    {
                        if (site.IsChecked)
                        {
                            node.Checked = true;
                        }

                        //check for decendants
                        if (node.Nodes.Count > 0)
                        {
                            ReadCheckedTreeNodes(node, site);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var error = string.Format(CultureInfo.InvariantCulture, Resources.ErrorReadingProtectedSiteSettings,
                    ex.Message);
                Logger.Error(error);
                throw new HttpModuleException(error);
            }
        }

        /// <summary>
        /// Function to Read Checked Nodes for treeView
        /// </summary>
        /// <param name="node"></param>
        /// <param name="site"></param>
        private static void ReadCheckedTreeNodes(TreeNode node, ProtectedSite site)
        {
            if (node == null || node.Nodes.Count == 0)
                return;

            foreach (TreeNode childNode in node.Nodes)
            {
                childNode.Checked = site.Applications.Any(x => x.Name.Equals(childNode.Text));

                ReadCheckedTreeNodes(childNode, site);
            }
        }


        #region TreeView

        /// <summary>
        /// function to initialize & Map Treeview saved in settings.xml
        /// </summary>
        private void InitializeAvailableSitesTreeView()
        {
            try
            {
                using (var serverManager = ServerManager.OpenRemote(Resources.ServerIpAddress))
                {
                    foreach (var site in serverManager.Sites)
                    {
                        //Sites which are in valid state should be displayed to UI.
                        if (site.State == ObjectState.Started)
                        {
                            var applications =
                                site.Applications.Where(
                                    a => (a.Path != "/") && !a.Path.Contains(Resources.ModuleAppDirectoryName));

                            //Create root Node, and add it to treeview ad root element
                            TreeNode rootNode = CreateTreeNode(site.Name);
                            _treeMap.Add(site.Name, rootNode);

                            //traverse all application under the site element
                            foreach (
                                var application in
                                    applications.Where(
                                        application => !IsTreeNodeAvailable(_treeMap, site.Name, application.Path)))
                            {
                                //add the node
                                AddTreeNode(_treeMap, site.Name, application.Path);
                            }
                            treeView1.Nodes.Add(rootNode);
                        }
                    }
                }
            }
            catch (ArgumentNullException ex)
            {
                var error = string.Format(CultureInfo.InvariantCulture, Resources.ErrorInitializeWebSites, ex.Message);
                Logger.Error(error);
                throw new HttpModuleException(error);

            }
            catch (NullReferenceException ex)
            {
                var error = string.Format(CultureInfo.InvariantCulture, Resources.ErrorInitializeWebSites, ex.Message);
                Logger.Error(error);
                throw new HttpModuleException(error);

            }
            catch (Exception ex)
            {
                var error = string.Format(CultureInfo.InvariantCulture, Resources.ErrorInitializeWebSites, ex.Message);
                Logger.Error(error);
                throw new HttpModuleException(error);
            }
        }

        /// <summary>
        /// Function to create root (Site) element in treeview
        /// </summary>
        /// <param name="name">app Name</param>
        /// <returns>treenode</returns>
        private static TreeNode CreateTreeNode(string name)
        {
            var index = name.LastIndexOf("/", StringComparison.Ordinal);
            if (index != -1)
                name = name.Substring(index + 1);

            var treeNode = new TreeNode(name) { Name = name };
            return treeNode;
        }

        /// <summary>
        /// Function to check node if already exist in map.
        /// </summary>
        /// <param name="map">SiteName, TreeNode Dictionary</param>
        /// <param name="site">root node</param>
        /// <param name="application">application</param>
        /// <returns>bool</returns>
        private static bool IsTreeNodeAvailable(IDictionary<string, TreeNode> map, string site, string application)
        {
            var isExist = map.FirstOrDefault(x => x.Key.Equals(string.Concat(site, application)));

            return isExist.Value != null;
        }

        /// <summary>
        /// Adding node to root element
        /// </summary>
        /// <param name="map">SiteName, TreeNode Dictionary</param>
        /// <param name="site">Parent node</param>
        /// <param name="application">application</param>
        private static void AddTreeNode(Dictionary<string, TreeNode> map, string site, string application)
        {
            TreeNode parentNode = null;
            var i = application.LastIndexOf("/", StringComparison.Ordinal);

            if (i == 0)
                parentNode = map.FirstOrDefault(x => x.Key.Equals(site)).Value;
            else
            {
                var applicationPath = string.Concat(site, application);
                parentNode =
                    map.FirstOrDefault(
                        x =>
                            x.Key.Equals(applicationPath.Substring(0,
                                applicationPath.LastIndexOf("/", StringComparison.Ordinal)))).Value;
            }

            //if virtual directories: not showing them in view
            if (parentNode == null) return;

            var childNode = CreateTreeNode(application);
            parentNode.Nodes.Add(childNode);

            //Add application to treeMap
            map.Add(string.Concat(site, application), childNode);
        }

        #endregion

        /// <summary>
        /// Function to Save the Protected Sites Tab's Settings in config.xml
        /// </summary>
        private void SaveProtectedSitesTreeSettings()
        {
            //Validate and save the config values for Protected Sites
            //1: check if any checkbox list items are checked
            var protectedSites = new List<ProtectedSite>();

            try
            {
                foreach (TreeNode node in treeView1.Nodes)
                {
                    var protectedSite = new ProtectedSite
                    {
                        Name = node.Text,
                        IsChecked = false,
                        Applications = new Collection<ServerTreeView.Configuration.Application>()
                    };

                    if (node.Checked)
                    {
                        protectedSite.IsChecked = true;
                    }

                    //if child is selected instead parent
                    if (node.Nodes.Count > 0)
                    {
                        CheckedNodes(node, protectedSite, "");
                    }

                    if (protectedSite.Applications.Count > 0)
                    {
                        protectedSite.IsChecked = true;
                    }

                    protectedSites.Add(protectedSite);
                }
            }
            catch (Exception exception)
            {
                var error = string.Format(CultureInfo.InvariantCulture, Resources.ErrorSavingProtectedSiteSettings,
                    exception.Message);
                Logger.Error(error);
                throw new HttpModuleException(error);
            }

            //Remove app Directory from not slected server sites (if exist)
            RemoveModuleWebAppFromServerSites(protectedSites);

            //add the module Directory in all selected sites & application(s) : path given to the registry
            var sites = protectedSites.Where(x => x.IsChecked || x.Applications.Count > 0).ToList();

            AddModuleWebAppToSelectedSites(sites);

            //save the config value to xml
            ConfigurationFactory.Instance.HttpModuleConfig.ProtectedSites = protectedSites;
        }

        private static void CheckedNodes(TreeNode node, ProtectedSite protectedSite, string parent)
        {
            if (node == null || node.Nodes.Count == 0)
                return;

            // foreach (var childNode in node.Nodes.Cast<TreeNode>().Where(childNode => childNode.Checked))
            foreach (var childNode in node.Nodes)
            {
                if (((TreeNode)childNode).Checked)
                {
                    protectedSite.Applications.Add(new ServerTreeView.Configuration.Application
                    {
                        Name = ((TreeNode)childNode).Text,
                        VirtualPath = parent + "/" + ((TreeNode)childNode).Text
                    });
                }
                CheckedNodes(((TreeNode)childNode), protectedSite, parent + "/" + ((TreeNode)childNode).Text);
            }
        }

        /// <summary>
        /// Function to add module Directory folder in the sites & application(S) , checked in protectedSites tabs.
        /// </summary>
        /// <param name="selectedSites"></param>
        internal void AddModuleWebAppToSelectedSites(IEnumerable<ProtectedSite> selectedSites)
        {
            var defaultSiteAppList = new List<string> { "/", Resources.ModuleAppDirectoryName };

            try
            {
                using (var serverManager = ServerManager.OpenRemote(Resources.ServerIpAddress))
                {
                    //get all Sites of server which are not in selectedSite list and check if Module is present : remove it
                    if (serverManager == null)
                    {
                        Logger.Error(string.Format(CultureInfo.InvariantCulture, Resources.ErrorReadingServer));
                        throw new HttpModuleException(string.Format(CultureInfo.InvariantCulture,
                            Resources.ErrorReadingServer));
                    }

                    foreach (var site in selectedSites.Select(selectedsite => serverManager.Sites[selectedsite.Name]))
                    {
                        ICollection<string> addModuleApplicationPaths = new List<string>();

                        //if list of applications are null
                        if (site.Applications == null)
                        {
                            Logger.Error(string.Format(CultureInfo.InvariantCulture,
                                Resources.ErrorReadingServerSiteApplication,
                                Dns.GetHostName()));
                            throw new HttpModuleException(string.Format(CultureInfo.InvariantCulture,
                                Resources.ErrorReadingServerSiteApplication, Dns.GetHostName()));
                        }

                        //check for parent node in selectedSite
                        var parentNode = selectedSites.FirstOrDefault(x => x.Name.Equals(site.Name));
                        if (parentNode == null)
                        {
                            Logger.Error(string.Format(CultureInfo.InvariantCulture, Resources.ErrorReadingServerSite,
                                Dns.GetHostName()));
                            throw new HttpModuleException(string.Format(CultureInfo.InvariantCulture,
                                Resources.ErrorReadingServerSite, Dns.GetHostName()));
                        }

                        // Add webapplication to the selected parentNode and its checked applications
                        var isWebAdded = AddWebApplicationToSelectedSites(site, parentNode, ref addModuleApplicationPaths);

                        //Just check if site has only two apps
                        var isDefault = site.Applications.Select(x => x.Path).Except(defaultSiteAppList).Any();

                        //Default Settings
                        if ((addModuleApplicationPaths.Count > 0) && (isDefault))
                        {
                            addModuleApplicationPaths.Remove("/");
                        }
                        AddModuleAtWebApplication(site, parentNode, ref addModuleApplicationPaths);

                        //Lists to add & remove the Module in Web.config
                        var isAdded = AddWebConfigurationModuleSectionInSelectedSites(serverManager, site.Name, addModuleApplicationPaths);
                        if (isAdded || isWebAdded)
                        {
                            //Restart the Server Site
                            RestartSiteAndCommitChanges(serverManager, site.ToString());
                        }
                    }
                }
            }
            catch (ArgumentNullException exception)
            {
                var error = string.Format(CultureInfo.InvariantCulture, Resources.ErrorAddingAppDirectorytoServer,
                    exception.Message);
                Logger.Error(error);
                throw new HttpModuleException(error);
            }
            catch (Exception exception)
            {
                var error = string.Format(CultureInfo.InvariantCulture, Resources.ErrorAddingAppDirectorytoServer,
                    exception.Message);
                Logger.Error(error);
                throw new HttpModuleException(error);
            }
        }

        /// <summary>
        /// Function to add the web interface module to protectedsites.
        /// </summary>
        /// <param name="site"></param>
        /// <param name="rootSiteNode"></param>
        /// <param name="addModuleApplicationPaths"></param>
        /// <returns></returns>
        private static bool AddWebApplicationToSelectedSites(Site site, ProtectedSite rootSiteNode, ref ICollection<string> addModuleApplicationPaths)
        {
            var isWebAdded = false;
            try
            {
                //Check if site already present: no need to add 
                var moduleApplication = site.Applications.Where(x => x.Path.Equals(Resources.ModuleAppDirectoryName)).ToList();

                if (moduleApplication.Count > 0)
                {
                    //just Add the site to dictionary to add the module  configuration
                    addModuleApplicationPaths.Add("/");
                    return false;
                }

                var siteApplications = site.Applications.Where(x => x.Path.Equals("/")).ToList();

                //Add the module web application in selected sites
                for (var application = 0; application < siteApplications.Count; application++)
                {
                    //read the registry key value
                    var regValue = CommonUtility.InstallationPath;

                    if (site.Applications[application].Path.Equals("/") && rootSiteNode.IsChecked)
                    {
                        //read and modify the config
                        site.Applications.Add(Resources.ModuleAppDirectoryName, regValue);
                        //Add the site to dictionary to add the module  configuration
                        addModuleApplicationPaths.Add(site.Applications[application].Path);
                        isWebAdded = true;
                    }
                }

                return isWebAdded;
            }
            catch (ArgumentNullException exception)
            {
                var error = string.Format(CultureInfo.InvariantCulture, Resources.ErrorAddingAppDirectorytoServer,
                    exception.Message);
                Logger.Error(error);
                throw new HttpModuleException(error);
            }
            catch (Exception exception)
            {
                var error = string.Format(CultureInfo.InvariantCulture, Resources.ErrorAddingAppDirectorytoServer,
                    exception.Message);
                Logger.Error(error);
                throw new HttpModuleException(error);
            }
        }

        private void RestartSiteAndCommitChanges(ServerManager manager, string siteName)
        {
            try
            {
                //Commit the changes to the IIS server
                manager.CommitChanges();

                //restart server site
                RestartServerSite(siteName);
            }
            catch (Exception exception)
            {
                var error = string.Format(CultureInfo.InvariantCulture, Resources.ErrorCommittingChangestoServer,
                    exception.Message);
                Logger.Error(error);
            }
        }

        /// <summary>
        /// Function to add Module element in selected web app(s)
        /// </summary>
        /// <param name="site"></param>
        /// <param name="rootSiteNode"></param>
        /// <param name="addModuleApplicationPaths"></param>

        private static void AddModuleAtWebApplication(Site site, ProtectedSite rootSiteNode, ref ICollection<string> addModuleApplicationPaths)
        {
            try
            {
                //get all the site's application except /Module
                var siteApplications =
                    site.Applications.Where(x => !x.Path.Contains(Resources.ModuleAppDirectoryName)).ToList();

                //Add the module web application in selected sites
                for (int application = 0; application < siteApplications.Count; application++)
                {

                    //2. if app name is present and appdir is null : application is checked and module is not present.
                    var appName =
                        rootSiteNode.Applications.FirstOrDefault(
                            x => x.Name.Equals(site.Applications[application].Path.Replace("/", "").Trim()));

                    if (appName != null)
                    {
                        //check if protected has applied on root: no need to add the module 
                        addModuleApplicationPaths.Add(site.Applications[application].Path);

                    }
                }
            }
            catch (ArgumentNullException exception)
            {
                var error = string.Format(CultureInfo.InvariantCulture, Resources.ErrorAddingAppDirectorytoServer,
                    exception.Message);
                Logger.Error(error);
                throw new HttpModuleException(error);
            }
            catch (Exception exception)
            {
                var error = string.Format(CultureInfo.InvariantCulture, Resources.ErrorAddingAppDirectorytoServer,
                    exception.Message);
                Logger.Error(error);
                throw new HttpModuleException(error);
            }
        }

        /// <summary>
        /// Function to update web.config with add & remove module Attributes
        /// </summary>
        /// <param name="serverManager"></param>
        /// <param name="siteName"></param>        
        /// <param name="addModuleApplicationPaths"></param>
        /// <returns></returns>
        private static bool AddWebConfigurationModuleSectionInSelectedSites(ServerManager serverManager, string siteName, ICollection<string> addModuleApplicationPaths)
        {
            var isAdded = false;
            try
            {
                if ((addModuleApplicationPaths != null) && (addModuleApplicationPaths.Count > 0))
                {
                    foreach (var path in addModuleApplicationPaths)
                    {
                        var config = serverManager.GetWebConfiguration(siteName, path);
                        var modulesSection = config.GetSection(Resources.ServerConfigModuleAttribute);
                        var modulesCollection = modulesSection.GetCollection();

                        if (
                            !modulesCollection.Any(
                                m => m.GetAttributeValue("name").ToString().Equals(Resources.ModuleName)))
                        {
                            ConfigurationElement addElement = modulesCollection.CreateElement(Resources.add);
                            addElement["name"] = Resources.ModuleName;
                            addElement["type"] = string.Format(CultureInfo.InvariantCulture,
                                Resources.ModuleNameSpace + ", " + AssemblyFullName);
                            modulesCollection.Add(addElement);

                            serverManager.CommitChanges();
                            isAdded = true;
                        }
                    }
                }
            }
            catch (ArgumentNullException exception)
            {
                var error = string.Format(CultureInfo.InvariantCulture, Resources.ErroraddingModuleInWebConfig,
                    exception.Message);
                Logger.Error(error);
                throw new HttpModuleException(error);
            }
            catch (Exception exception)
            {
                var error = string.Format(CultureInfo.InvariantCulture, Resources.ErroraddingModuleInWebConfig,
                    exception.Message);
                Logger.Error(error);
                throw new HttpModuleException(error);
            }

            return isAdded;
        }

        /// <summary>
        /// Function to Remove the module app Directory if Site or an Application is not Checked in TreeView but module is present in IIS
        /// </summary>
        /// <param name="selectedSites"></param>
        internal void RemoveModuleWebAppFromServerSites(IEnumerable<ProtectedSite> selectedSites)
        {
            try
            {
                //get all Sites of server which are not in selectedSite list and check if Module is present : remove it
                using (var serverManager = ServerManager.OpenRemote(Resources.ServerIpAddress))
                {
                    //get all Sites of server which are not in selectedSite list and check if Module is present : remove it
                    if (serverManager == null)
                    {
                        var error = string.Format(CultureInfo.InvariantCulture, Resources.ErrorReadingServer);
                        Logger.Error(error);
                        throw new HttpModuleException(error);
                    }

                    foreach (Site site in selectedSites.Select(selectedsite => serverManager.Sites[selectedsite.Name]))
                    {
                        if (site == null)
                        {
                            var error = string.Format(CultureInfo.InvariantCulture, Resources.ErrorReadingServerSite,
                                Dns.GetHostName());
                            Logger.Error(error);
                            throw new HttpModuleException(error);
                        }

                        if (site.Applications == null)
                        {
                            var error = string.Format(CultureInfo.InvariantCulture,
                                Resources.ErrorReadingServerSiteApplication, site.Name);
                            Logger.Error(error);
                            throw new HttpModuleException(error);
                        }

                        //delete web app.
                        var isAppDeleted = DeleteModuleAppFromServerConfiguration(serverManager, site);

                        //Remove Module entry if exists
                        var isModuleDeleted = DeleteModuleElementFromServerConfiguration(serverManager, site);

                        if (isAppDeleted || isModuleDeleted)
                        {
                            RestartSiteAndCommitChanges(serverManager, site.Name);
                        }
                    }
                }
            }
            catch (ArgumentNullException exception)
            {
                var error = string.Format(CultureInfo.InvariantCulture,
                    Resources.ErrorRemovingAppDirectoryFromServer, exception.Message);
                Logger.Error(error);
                throw new HttpModuleException(error);
            }
            catch (Exception exception)
            {
                var error = string.Format(CultureInfo.InvariantCulture,
                    Resources.ErrorRemovingAppDirectoryFromServer, exception.Message);
                Logger.Error(error);
                throw new HttpModuleException(error);
            }
        }

        /// <summary>
        /// Function to remove the ap from selected site
        /// </summary>
        /// <param name="serverManager"></param>
        /// <param name="site"></param>
        /// <returns></returns>
        private static bool DeleteModuleAppFromServerConfiguration(ServerManager serverManager, Site site)
        {
            var isdeleted = false;
            try
            {
                //select sites applications contain Modules
                var moduleApps =
                    site.Applications.Where(x => x.Path.Contains(Resources.ModuleAppDirectoryName)).ToList();
                foreach (var app in moduleApps)
                {
                    site.Applications.Remove(app);
                    isdeleted = true;
                }

                serverManager.CommitChanges();
                return isdeleted;
            }
            catch (ArgumentNullException exception)
            {
                var error = string.Format(CultureInfo.InvariantCulture,
                    Resources.ErrorDeletingAppDirectoryFromServer, exception.Message);
                Logger.Error(error);
                throw new HttpModuleException(error);
            }
            catch (Exception exception)
            {
                var error = string.Format(CultureInfo.InvariantCulture,
                    Resources.ErrorDeletingAppDirectoryFromServer, exception.Message);
                Logger.Error(error);
                throw new HttpModuleException(error);
            }
        }

        /// <summary>
        /// Function to delete module elemnt entry from configuration
        /// </summary>
        /// <param name="serverManager"></param>
        /// <param name="site"></param>       
        /// <returns></returns>
        private static bool DeleteModuleElementFromServerConfiguration(ServerManager serverManager, Site site)
        {
            var isdeleted = false;

            foreach (var app in site.Applications)
            {
                try
                {
                    var config = serverManager.GetWebConfiguration(site.Name, app.Path);
                    var modulesSection = config.GetSection(Resources.ServerConfigModuleAttribute);
                    var modulesCollection = modulesSection.GetCollection();

                    //select all the tags belong to the module elements "AuthenticationModule"
                    var moduleElements =
                        modulesCollection.Select(
                            module =>
                                new { module, element = module.Attributes["name"].Value.Equals(Resources.ModuleName) })
                            .Where(t => t.element)
                            .Select(t => t.module);

                    if (moduleElements.Any())
                    {
                        foreach (var module in moduleElements)
                        {
                            module.Delete();
                            isdeleted = true;

                            serverManager.CommitChanges();
                        }
                    }
                }
                catch (Exception exception)
                {
                    var error = string.Format(CultureInfo.InvariantCulture,
                        Resources.ErrorDeletingModuleElementFromServer, exception.Message);

                    Logger.Error(error);

                }
            }
            return isdeleted;
        }

        #endregion

        #region Helper Methods & Events

        /// <summary>
        /// Function to change the state of buttons based on the event trigged at run time & set isDirty : true
        /// </summary>
        private void AddOnChangeEventHandlers()
        {
            treeView1.AfterCheck += SetTreeViewDirty;
        }

        private void SetAdvancedFormDirty(object sender, EventArgs e)
        {
            IsDirty = true;
            buttonApply.Enabled = IsDirty;
            buttonOK.Enabled = IsDirty;
        }

        private void SetTreeViewDirty(object sender, EventArgs e)
        {
            IsDirty = true;
            IsProtectedSitesSettingsChanged = true;
            buttonApply.Enabled = IsDirty;
            buttonOK.Enabled = IsDirty;
        }

        private void SetFormDirty(object sender, EventArgs e)
        {
            IsDirty = true;
            buttonApply.Enabled = IsDirty;
            buttonOK.Enabled = IsDirty;
        }

        /// <summary>
        /// Function to restart the IIS Site
        /// </summary>
        /// <param name="serverSite"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        internal void RestartServerSite(string serverSite)
        {
            try
            {
                using (var server = ServerManager.OpenRemote(Resources.ServerIpAddress))
                {
                    if (server == null)
                    {
                        Logger.Error(Resources.ErrorReadingServer);
                        throw new HttpModuleException(Resources.ErrorReadingServer);
                    }

                    if (server.Sites == null)
                    {
                        Logger.Error(Resources.ErrorReadingServerSites);
                        throw new HttpModuleException(Resources.ErrorReadingServerSites);
                    }

                    var site = server.Sites.FirstOrDefault(s => s.Name == serverSite);

                    if (site != null)
                    {
                        //stop the site...
                        site.Stop();
                        if (site.State == ObjectState.Stopped)
                        {
                            Logger.Info(string.Format(CultureInfo.InvariantCulture, Resources.MessageServerStop,
                                site.Name));
                        }

                        //restart the site...
                        site.Start();

                        if (site.State == ObjectState.Started)
                        {
                            Logger.Info(string.Format(CultureInfo.InvariantCulture, Resources.MessageServerStart,
                                site.Name));
                        }
                    }
                    else
                    {
                        Logger.Error(Resources.ErrorInServerEvents);
                        throw new HttpModuleException(Resources.ErrorInServerEvents);
                    }
                }
            }
            catch (Exception ex)
            {
                var error = string.Format(CultureInfo.InvariantCulture, Resources.ErrorRestartWebSite, ex.Message);
                Logger.Error(error);
                throw new HttpModuleException(error);
            }
        }

        private bool ShowErrorMessageBox(string errorMessage)
        {
            CustomMessageBox.Show(this,
                errorMessage,
                Resources.AssemblyLabel,
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button1,
                0);

            return false;
        }

        /// <summary>
        /// Parse a string to integer
        /// </summary>
        /// <param name="stringToConvert"></param>
        /// <returns></returns>
        private static int CallTryParse(string stringToConvert)
        {
            int number;
            if (!int.TryParse(stringToConvert, NumberStyles.Integer, CultureInfo.InvariantCulture, out number))
            {
                // Report error
                number = -1;
            }
            return number;
        }

        /// <summary>
        /// Event gets triggered when any node or child is selected in tree view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeView_AfterCheck(object sender, TreeViewEventArgs e)
        {
            try
            {
                e.Node.TreeView.BeginUpdate();
                if (e.Node.Nodes.Count > 0)
                {
                    var parentNode = e.Node;
                    var nodes = e.Node.Nodes;
                    CheckedOrUnCheckedNodes(parentNode, nodes);
                }
            }
            finally
            {
                e.Node.TreeView.EndUpdate();
            }
        }

        /// <summary>
        /// Member Function to be called to decide if child(s) should be selected or not if parent is checked/unchecked
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="nodes"></param>
        private void CheckedOrUnCheckedNodes(TreeNode parentNode, TreeNodeCollection nodes)
        {
            if (nodes.Count > 0)
            {
                foreach (TreeNode node in nodes)
                {
                    node.Checked = parentNode.Checked;
                    CheckedOrUnCheckedNodes(parentNode, node.Nodes);
                }
            }
        }

        /// <summary>
        /// Function to load the fully qualified HttpModule Assembly Name
        /// </summary>
        private static void GetFullyQualifiedAssemblyName()
        {
            try
            {
                var name = new AssemblyName(Resources.ModuleAssemblyName);
                var assembly = Assembly.UnsafeLoadFrom(name.ToString());

                if (assembly != null)
                {
                    AssemblyFullName = assembly.FullName;
                }
            }
            catch (FileNotFoundException exception)
            {
                var error = string.Format(CultureInfo.InvariantCulture, Resources.AssemblyNotFoundException, exception.Message);
                Logger.Error(error);
                throw new HttpModuleException(error);
            }
            catch (FileLoadException exception)
            {
                var error = string.Format(CultureInfo.InvariantCulture, Resources.AssemblyLoadException, exception.Message);
                Logger.Error(error);
                throw new HttpModuleException(error);
            }
            catch (BadImageFormatException exception)
            {
                var error = string.Format(CultureInfo.InvariantCulture, Resources.AssemblyBadFormatException, exception.Message);
                Logger.Error(error);
                throw new HttpModuleException(error);
            }
            catch (SecurityException exception)
            {
                var error = string.Format(CultureInfo.InvariantCulture, Resources.AssemblyPermissionException, exception.Message);
                Logger.Error(error);
                throw new HttpModuleException(error);
            }
            catch (Exception exception)
            {
                var error = string.Format(CultureInfo.InvariantCulture, Resources.AssemblyIsNotRegistered,
                    exception.Message);
                Logger.Error(error);
                throw new HttpModuleException(error);
            }
        }

        #endregion

        #region Button

        /// <summary>
        /// Event for OK button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnOk_Click(object sender, EventArgs e)
        {
            if (OnApplyChanges())
            {
                Close();
            }
        }

        /// <summary>
        /// Apply button click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnApply_Click(object sender, EventArgs e)
        {
            if (OnApplyChanges())
            {
                buttonApply.Enabled = IsDirty;
            }
        }

        /// <summary>
        /// Cancel button click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCancel_Click(object sender, EventArgs e)
        {
            IsDirty = false;
            Close();
        }

        /// <summary>
        /// Function to gets called to apply changes(if any) in any of the tab in UI
        /// It validates the configuration(s) & Save it in xml file.
        /// </summary>
        /// <returns></returns>
        private bool OnApplyChanges()
        {
            try
            {
                if (IsDirty)
                {
                    //1 Configuration Validate
                    if (!ValidateConfiguration())
                    {
                        return false;
                    }                  
                    if (!ResetSettings())
                    {
                        return false;
                    }

                    //reset settings

                    IsProtectedSitesSettingsChanged = false;
                    IsDirty = false;
                    _restartRequired = false;
                }
            }
            catch (NetworkInformationException exp)
            {
                Logger.Error(exp);
                CustomMessageBox.Show(this,
                    exp.Message,
                    Resources.AssemblyLabel,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button1,
                    0);
                return false;
            }
            catch (Exception exp)
            {
                Logger.Error(exp);
                CustomMessageBox.Show(this,
                    exp.Message,
                    Resources.AssemblyLabel,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button1,
                    0);

                return false;
            }

            return true;
        }

        /// <summary>
        /// Function to check the commits and restart the iis services
        /// </summary>
        /// <returns></returns>
        private bool ResetSettings()
        {
            try
            {

                //Restart required??                                     
                if (_restartRequired)
                {
                    return ConfirmAndSaveSettings();
                }
                //Check for protected site tab
                //if any changes in protected sites tab then show the below message box
                if (!IsProtectedSitesSettingsChanged) return true;

                //Check if any site settings has changed: compare with saved settings
                //1. get the count of checked nodes

                var checkedNodes = GetCheckedNodesCount(treeView1.Nodes, new List<string>());
                var checkProtectedSites = GetProtectedSiteCount();

                //see if any item doesnt match
                var isEqual = checkedNodes.All(checkProtectedSites.Contains) && checkProtectedSites.All(checkedNodes.Contains);

                if ((checkedNodes.Count != checkProtectedSites.Count) || (!isEqual))
                {
                    return ConfirmAndSaveSettings();
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Show confirm box and save settings
        /// </summary>
        /// <returns></returns>
        private bool ConfirmAndSaveSettings()
        {
            //after validating the configuration , show confirmation box to proceed
            var confirmed = CustomMessageBox.Show(this, Resources.RestartConfirmBoxMessage,
                Resources.AssemblyLabel, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button1, 0);

            if (confirmed == DialogResult.Cancel)
            {
                return false;
            }
            SaveProtectedSitesTreeSettings();

            //save the settings & restart the server                                  
            ConfigurationFactory.Instance.HttpModuleConfig.SaveConfiguration();
            return true;
        }

        /// <summary>
        /// Get saved protected sites count
        /// </summary>
        /// <returns></returns>
        private static List<string> GetProtectedSiteCount()
        {
            var checkedSites = new List<string>();

            //read existing config protectedsites
            var sites = ConfigurationFactory.Instance.HttpModuleConfig.ProtectedSites;

            foreach (var s in sites)
            {
                if (s.IsChecked)
                {
                    checkedSites.Add(s.Name);
                }
                checkedSites.AddRange(s.Applications.Select(a => a.Name));
            }
            return checkedSites;
        }

        /// <summary>
        /// Get checked treeview nodes name
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="sites"></param>
        /// <returns></returns>
        private List<string> GetCheckedNodesCount(TreeNodeCollection nodes, List<string> sites)
        {
            if (nodes == null) throw new ArgumentNullException(nameof(nodes));

            for (var i = 0; i < nodes.Count; i++)
            {
                var node = nodes[i];
                if (node.Checked)
                {
                    sites.Add(node.Name);
                }

                if (node.Nodes.Count > 0)
                    GetCheckedNodesCount(node.Nodes, sites);
            }
            return sites;
        }       
        #endregion
    }
}
