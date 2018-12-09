namespace ServerTreeView.Configuration
{
    #region using statements
    using System;
    using System.Globalization;
    using System.IO;
    using Common;
    #endregion

    /// <summary>
    /// HttpModule configuration class
    /// </summary>
    public class ConfigurationFactory
    {
        private static readonly Lazy<ConfigurationFactory> Lazy = new Lazy<ConfigurationFactory>(() => new ConfigurationFactory());

        private DateTime _fileModifyTime;

        private Config _moduleConfig;

        private string _installPath;

        private readonly object _thisLock = new object();

        private ConfigurationFactory()
        {
        }

        /// <summary>
        ///     Provides an instance to the ServiceFactory object. Creates a new instance if the object does not already exist.
        /// </summary>
        public static ConfigurationFactory Instance => Lazy.Value;

        /// <summary>
        /// Gets TwoFactor configuration details
        /// </summary>
        public Config HttpModuleConfig
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this._installPath))
                {
                    this._installPath = CommonUtility.InstallationPath;
                }

                Logger.Debug(string.Format(CultureInfo.InvariantCulture, $"Setting filePath is {this._installPath}"));
                if (this._installPath != null)
                {
                    var time = ValidateFilePermission();

                    if (this._moduleConfig != null && this._fileModifyTime == time)
                    {
                        return this._moduleConfig;
                    }

                    lock (this._thisLock)
                    {
                        if (this._moduleConfig != null && this._fileModifyTime == time)
                        {
                            return this._moduleConfig;
                        }

                        InitailizeConfiguration();

                        this._fileModifyTime = time;
                    }
                }

                return this._moduleConfig;
            }
        }

        private void InitailizeConfiguration()
        {
            try
            {
                this._moduleConfig = new Config(this._installPath);
            }
            catch (DirectoryNotFoundException directoryNotFoundException)
            {
                Logger.Error(directoryNotFoundException);
                throw;
            }
            catch (UnauthorizedAccessException unauthorizedAccessException)
            {
                Logger.Error(unauthorizedAccessException);
                throw;
            }
            catch (FileNotFoundException fileNotFoundException)
            {
                Logger.Error(fileNotFoundException);
                throw;
            }
            catch (IOException ioException)
            {
                Logger.Error(ioException);
                throw;
            }
        }

        private DateTime ValidateFilePermission()
        {
            try
            {
                return File.GetLastWriteTime(this._installPath);
            }
            catch (NotSupportedException exception)
            {
                Logger.Error(exception);
                throw;
            }
            catch (ArgumentNullException exception)
            {
                Logger.Error(exception);
                throw;
            }
            catch (ArgumentException argumentException)
            {
                Logger.Error(argumentException);
                throw;
            }
            
            catch (PathTooLongException exception)
            {
                Logger.Error(exception);
                throw;
            }
        }
    }
}