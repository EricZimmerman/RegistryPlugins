using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace RegistryPlugin.Services
{
    public class Service
    {
        public string Name { get; }
        public string Description { get; }
        public string DisplayName { get; }
        public ServiceStartMode StartMode { get; }

        public ServiceType ServiceType { get; }

        public DateTimeOffset NameKeyLastWrite { get; }
        public DateTimeOffset? ParametersKeyLastWrite { get; }

        public string Group { get; }

        public string ImagePath { get; }
        public string ServiceDLL { get; }
        public string RequiredPrivileges { get; }


        public Service(string name, string description, string displayName, ServiceStartMode startMode,
            ServiceType serviceType, DateTimeOffset nameKeyLastWrite, DateTimeOffset? parametersKeyLastWrite,
            string group, string imagePath, string serviceDll, string reqPrivs)
        {
            Name = name;
            Description = description;
            DisplayName = displayName;
            StartMode = startMode;
            ServiceType = serviceType;
            NameKeyLastWrite = nameKeyLastWrite;
            ParametersKeyLastWrite = parametersKeyLastWrite;
            Group = group;
            ImagePath = imagePath;
            ServiceDLL = serviceDll;
            RequiredPrivileges = reqPrivs;
        }
    }

  
}
