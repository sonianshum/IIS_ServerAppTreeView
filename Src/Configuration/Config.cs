namespace ServerTreeView.Configuration
{
    #region using statements
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Xml;
    using Common;
    #endregion using statements

    [DataContract]
    public class Config
    {
        public const string FileName = @"Settings.xml";
        private const string AuthyUrl = @"https://test.com/protected/json/";
        private readonly string _filePath;

        public Config(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            SubscriptionKey = string.Empty;
            AuthyEndPoint = AuthyUrl;
            PushNotificationMessage = "Testing";
            PushNotificationTimeout = 30;
            ProtectedSites = new List<ProtectedSite>();          

            Logger.Debug(string.Format(CultureInfo.InvariantCulture, $"Try to find setting file's path :{path}"));

            _filePath = Path.Combine(path, FileName);

            Logger.Debug(string.Format(CultureInfo.InvariantCulture, $"try to find setting file {_filePath}"));

            if (File.Exists(_filePath))
            {
                Logger.Debug(string.Format(CultureInfo.InvariantCulture, $"Read setting from filePath:{_filePath}"));
                var httpModuleAgentSerializer = new DataContractSerializer(typeof(Config));
                var stream = File.OpenRead(_filePath);
                try
                {
                    var config = (Config)httpModuleAgentSerializer.ReadObject(stream);
                    this.Initialize(config);
                }
                finally
                {
                    stream.Close();
                }
            }
            else
            {
                Logger.Error(string.Format(CultureInfo.InvariantCulture, $"HTTP Module settings file not find in {0} location", path));
            }
        }

        /// <summary>
        /// Authy end point
        /// </summary>     
        [DataMember(Order = 1)]
        public string AuthyEndPoint { get; set; }

        /// <summary>
        /// Defender as a service subscription key
        /// </summary>
        [DataMember(Order = 2)]
        public string SubscriptionKey { get; set; }

        /// <summary>
        /// Push Notification Message
        /// </summary>
        [DataMember(Order = 3)]
        public string PushNotificationMessage { get; set; }

        /// <summary>
        /// Number of seconds to wait for response from DSS
        /// </summary>
        [DataMember(Order = 4)]
        public int PushNotificationTimeout { get; set; }


        /// <summary>
        /// List of Server web Sites
        /// </summary>
        [DataMember(Order = 6)]
        public List<ProtectedSite> ProtectedSites { get; set; }

        public void SaveConfiguration()
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    var fileStream = File.Create(FileName);
                    fileStream.Dispose();
                }

                var dataSerializer = new DataContractSerializer(typeof(Config));
                var settings = new XmlWriterSettings { Indent = true };
                using (var xmlWriter = XmlWriter.Create(_filePath, settings))
                {
                    dataSerializer.WriteObject(xmlWriter, this);
                }
            }
            catch (ArgumentNullException exp)
            {
                Logger.Error(exp);
                throw new HttpModuleException(exp.Message);
            }
            catch (InvalidDataContractException exp)
            {
                Logger.Error(exp);
                throw new HttpModuleException(exp.Message);
            }
            catch (SerializationException exp)
            {
                Logger.Error(exp);
                throw new HttpModuleException(exp.Message);
            }
        }


        /// <summary>
        /// Initialize HttpModule agent configuration
        /// </summary>
        /// <param name="config"></param>
        private void Initialize(Config config)
        {
            this.SubscriptionKey = config.SubscriptionKey;

            if (config.AuthyEndPoint != null)
            {
                // if the value doesn't exists, use default value
                this.AuthyEndPoint = config.AuthyEndPoint;
            }

            if (!string.IsNullOrWhiteSpace(config.PushNotificationMessage))
            {
                this.PushNotificationMessage = config.PushNotificationMessage;
            }

            if (config.PushNotificationTimeout > 0)
            {
                this.PushNotificationTimeout = config.PushNotificationTimeout;
            }

            if (config.ProtectedSites != null)
            {
                this.ProtectedSites = config.ProtectedSites;
            }
        }
    }
}