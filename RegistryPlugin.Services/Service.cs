using System;
using System.ServiceProcess;

namespace RegistryPlugin.Services
{
    public class Service
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
    }
}