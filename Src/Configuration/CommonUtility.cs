namespace ServerTreeView.Configuration
{
    #region using statements
    using System;
    using System.IO;
    using System.Security;
    using Microsoft.Win32;
    using System.Diagnostics;
    #endregion
    public static class CommonUtility
    {
        private const string RegistryKeyPath = @"SOFTWARE\Module";
        private const string DiagnosticKey = @"Diagnostics";
        internal const string EventSource = @"Module";
        /// <summary>
        /// Gets HttpModule installation path from registry
        /// </summary>
        /// <returns></returns>
        public static string InstallationPath
        {
            get
            {
                try
                {
                    using (var baseRegistryKey = GetBaseRegisteryKey())
                    {
                        using (var registryKey = baseRegistryKey.OpenSubKey(RegistryKeyPath, RegistryKeyPermissionCheck.ReadSubTree))
                        {
                            if (registryKey != null)
                            {
                                return (string)registryKey.GetValue("");
                            }
                        }
                    }
                }
                catch (UnauthorizedAccessException exp)
                {
                    WriteIntoEventLog(exp.Message, EventLogEntryType.Error, (int)EventLogEntryType.Error);
                    throw;
                }
                catch (ObjectDisposedException exp)
                {
                    WriteIntoEventLog(exp.Message, EventLogEntryType.Error, (int)EventLogEntryType.Error);
                    throw;
                }
                catch (SecurityException exp)
                {
                    WriteIntoEventLog(exp.Message, EventLogEntryType.Error, (int)EventLogEntryType.Error);
                    throw;
                }
                catch (IOException exp)
                {
                    WriteIntoEventLog(exp.Message, EventLogEntryType.Error, (int)EventLogEntryType.Error);
                    throw;
                }

                return string.Empty;
            }
        }

        private static RegistryKey GetBaseRegisteryKey()
        {
            return RegistryKey.OpenBaseKey(RegistryHive.LocalMachine,
                Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32);
        }

        private static RegistryKey GetSubRegisteryKey(RegistryKey baseRegistryKey)
        {
            return baseRegistryKey.OpenSubKey(RegistryKeyPath, RegistryKeyPermissionCheck.ReadSubTree);
        }
        public static bool LogLevel
        {
            get
            {
                try
                {
                    using (var baseRegistryKey = GetBaseRegisteryKey())
                    {
                        using (var registryKey = GetSubRegisteryKey(baseRegistryKey))
                        {
                            if (registryKey != null)
                            {
                                return (bool)registryKey.GetValue(DiagnosticKey, false);
                            }
                        }
                    }
                }
                catch (UnauthorizedAccessException exp)
                {
                    WriteIntoEventLog(exp.Message, EventLogEntryType.Error, (int)EventLogEntryType.Error);
                    throw;
                }
                catch (ObjectDisposedException exp)
                {
                    WriteIntoEventLog(exp.Message, EventLogEntryType.Error, (int)EventLogEntryType.Error);
                    throw;
                }
                catch (SecurityException exp)
                {
                    WriteIntoEventLog(exp.Message, EventLogEntryType.Error, (int)EventLogEntryType.Error);
                    throw;
                }
                catch (IOException exp)
                {
                    WriteIntoEventLog(exp.Message, EventLogEntryType.Error, (int)EventLogEntryType.Error);
                    throw;
                }

                return false;
            }
        }

        /// <summary>
        ///  Writing into the event logs
        /// </summary>
        /// <param name="errormessage">error message</param>
        /// <param name="logEntryType">type of an event log entry</param>
        /// <param name="eventIdType"></param>
        private static void WriteIntoEventLog(string errormessage, EventLogEntryType logEntryType, int eventIdType)
        {
            // Configure event logging
            using (var eventLog = new EventLog {Source = EventSource})
            {
                eventLog.WriteEntry(errormessage, logEntryType, eventIdType);
            }
        }
    }
}
