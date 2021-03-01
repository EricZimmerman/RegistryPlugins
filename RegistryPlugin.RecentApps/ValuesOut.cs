using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.RecentApps
{
   public class ValuesOut:IValueOut
    {
        public ValuesOut(string keyName,string appId, string appPath, DateTimeOffset lastAccessed, int launchCount)
        {
            RecentItems = new List<RecentItem>();

            KeyName = keyName;
            AppId = appId;
            AppPath = appPath;
            LastAccessed = lastAccessed;
            LaunchCount = launchCount;
        }
        public string KeyName { get; }
        public string AppId { get; }
        public string AppPath { get; }
        public DateTimeOffset LastAccessed { get; }
        public int LaunchCount { get; }

        public string RecentDocs => string.Join(",",RecentItems);

        public List<RecentItem> RecentItems { get; }
        public string BatchKeyPath { get; set; }
        public string BatchValueName { get; set; }
        public string BatchValueData1 => $"AppId: {AppId} App path: {AppPath}";
        public string BatchValueData2 => $"Last Accessed: {LastAccessed.ToUniversalTime():yyyy-MM-dd HH:mm:ss.fffffff}";
        public string BatchValueData3 => $"Launch count: {LaunchCount}" ;
    }


    public class RecentItem
    {
        public RecentItem(string keyName,string displayName, DateTimeOffset lastAccessed, string path)
        {
            KeyName = keyName;
            DisplayName = displayName;
            LastAccessed = lastAccessed;
            Path = path;
        }
        public string KeyName { get; }
        public string DisplayName { get; }
        public DateTimeOffset LastAccessed { get; }
        public string Path { get; }

        public override string ToString()
        {
            return $"{DisplayName}/{Path}: {LastAccessed}";
        }
    }

}
