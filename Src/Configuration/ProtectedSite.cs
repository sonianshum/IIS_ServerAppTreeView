namespace ServerTreeView.Configuration
{
    #region using statements
    using System.Collections.ObjectModel;
    using System.Runtime.Serialization;
    #endregion

    [DataContract]
    public class ProtectedSite
    {
        [DataMember(Order = 1)]
        public string Name { get; set; }

        [DataMember(Order = 2)]
        public bool IsChecked { get; set; }

        [DataMember(Order = 3)]
        public Collection<Application> Applications { get; set; }

    }

    [DataContract]
    public class Application
    {
        [DataMember(Order = 1)]
        public string Name { get; set; }

        [DataMember(Order = 2)]
        public string VirtualPath { get; set; }

    }
}
