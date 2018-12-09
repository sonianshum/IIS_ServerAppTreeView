namespace ServerTreeView.Settings
{
    #region Using Statements
    using System;
    using System.Globalization;
    using System.Windows.Forms;
    using log4net.Config;
    using Common;
    using Properties;
    #endregion
    internal static class Settings
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            try
            {
                Logger.Enter();
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                using (var formSettings = new ServerSites())
                {
                    Application.Run(formSettings);
                }
                Logger.Exit();
            }
            catch (Exception exp)
            {
                Logger.Error(string.Format(CultureInfo.InvariantCulture, $"Application Error: {exp}"));
                CustomMessageBox.Show(null,
                    Resources.UnexpectedError,
                    Resources.AssemblyLabel,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1,
                    0);
            }
            Application.Exit();
        }
    }
}
