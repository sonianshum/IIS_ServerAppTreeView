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
        private readonly string _filePath;

        public Config(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path));
            }
        
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
            if (config.ProtectedSites != null)
            {
                this.ProtectedSites = config.ProtectedSites;
            }
        }
    }
}