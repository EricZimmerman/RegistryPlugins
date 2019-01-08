using System;
using System.ServiceProcess;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.Services
{
    public class Service:IValueOut
    {
        public Service(string name, string description, string displayName, ServiceStartMode startMode,
            ServiceType serviceType, DateTimeOffset nameKeyLastWrite, DateTimeOffset? parametersKeyLastWrite,
            string group, string imagePath, string serviceDll, string reqPrivs)
        {
            Name = name;
            Description = description;
            DisplayName = displayName;
            StartMode = startMode;
            ServiceType = serviceType;
            NameKeyLastWrite = nameKeyLastWrite.UtcDateTime;
            ParametersKeyLastWrite = parametersKeyLastWrite?.UtcDateTime;
            Group = group;
            ImagePath = imagePath;
            ServiceDLL = serviceDll;
            RequiredPrivileges = reqPrivs;
        }

        public string Name { get; }
        public string Description { get; }
        public string DisplayName { get; }
        public ServiceStartMode StartMode { get; }

        public ServiceType ServiceType { get; }

        public DateTime NameKeyLastWrite { get; }
        public DateTime? ParametersKeyLastWrite { get; }

        public string Group { get; }

        public string ImagePath { get; }
        public string ServiceDLL { get; }
        public string RequiredPrivileges { get; }
        public string BatchKeyPath { get; set; }
        public string BatchValueName { get; set; }
        public string BatchValueData1 => $"Name: {Name} Desc: {Description}";
        public string BatchValueData2 => $"Image path:{ImagePath} ServiceDLL: {ServiceDLL}";
        public string BatchValueData3 => $"Name last write: {NameKeyLastWrite.ToUniversalTime():yyyy-MM-dd HH:mm:ss.fffffff} Parameters last write: {ParametersKeyLastWrite?.ToUniversalTime():yyyy-MM-dd HH:mm:ss.fffffff}";
    }
}