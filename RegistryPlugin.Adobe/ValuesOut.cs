using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.Adobe
{
    public class ValuesOut:IValueOut
    {
        public ValuesOut(string productName, string productVersion, string fullPath, DateTimeOffset lastOpened, string fileName, int fileSize, string fileSource, int pageCount)
        {
            ProductName = productName;
            ProductVersion = productVersion;
            FullPath = fullPath;
            LastOpened = lastOpened;
            FileName = fileName;
            FileSize = fileSize;
            FileSource = fileSource;
            PageCount = pageCount;
        }

        public string ProductName { get; }
        public string ProductVersion { get; }
        public string FullPath { get; }
        public DateTimeOffset LastOpened { get; }
        public string FileName { get; }
        public int FileSize { get; }
        public string FileSource { get; }
        public int PageCount { get; }

        public string BatchKeyPath { get; set; }
        public string BatchValueName { get; set; }
        public string BatchValueData1 => $"Product: {ProductName} {ProductVersion}";
        public string BatchValueData2 => $"FullPath: {FullPath}";
        public string BatchValueData3 => $"LastOpened: {LastOpened}";
    }
}
