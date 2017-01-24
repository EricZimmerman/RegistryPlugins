using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Registry.Abstractions;
using RegistryPluginBase.Classes;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.Services
{
    public class Services : IRegistryPluginGrid
    {
        private readonly BindingList<Service> _values;

        public Services()
        {
            _values = new BindingList<Service>();

            Errors = new List<string>();
        }

        public string InternalGuid => "bad7dc6f-d7cf-47ba-b138-82d0a6a9740b";

        public List<string> KeyPaths => new List<string>(new[]
        {
            @"ControlSet001\Services",
            @"ControlSet002\Services",
            @"ControlSet003\Services",
            @"ControlSet004\Services"
        });


        public string ValueName => null;
        public string AlertMessage { get; private set; }
        public RegistryPluginType.PluginType PluginType => RegistryPluginType.PluginType.Grid;
        public string Author => "Eric Zimmerman";
        public string Email => "saericzimmerman@gmail.com";
        public string Phone => "501-313-3778";
        public string PluginName => "Services";
        public string ShortDescription => "Displays details about Windows services";

        public string LongDescription
            =>
            ""
            ;

        public double Version => 0.5;
        public List<string> Errors { get; }

        public void ProcessValues(RegistryKey key)
        {
            _values.Clear();
            Errors.Clear();

            foreach (var rd in ProcessKey(key))
            {
                _values.Add(rd);
            }
        }

        public IBindingList Values => _values;


        private IEnumerable<Service> ProcessKey(RegistryKey key)
        {
            var l = new List<Service>();

            try
            {
                
                foreach (var keyValue in key.SubKeys)
                {
                   

                    DateTimeOffset nameLastWrite = keyValue.LastWriteTime.Value;

                    var name = keyValue.KeyName;

                  
                    var descVal = keyValue.Values.SingleOrDefault(t => t.ValueName == "Description");
                    var desc = descVal?.ValueData ?? string.Empty;

                    var dispVal = keyValue.Values.SingleOrDefault(t => t.ValueName == "DisplayName");
                    var disp = dispVal?.ValueData ?? string.Empty;

                    var group = keyValue.Values.SingleOrDefault(t => t.ValueName == "Group")?.ValueData ?? String.Empty;
                    var imagePath= keyValue.Values.SingleOrDefault(t => t.ValueName == "ImagePath")?.ValueData ?? String.Empty;
                    var reqPrivs= keyValue.Values.SingleOrDefault(t => t.ValueName == "RequiredPrivileges")?.ValueData ?? String.Empty;

                    ServiceType startType = ServiceType.Adapter; 

                    var ssv = keyValue.Values.SingleOrDefault(t => t.ValueName == "Type");

                    if (ssv != null)
                    {
                        startType = (ServiceType)int.Parse(ssv.ValueData);
                    }

                    ServiceStartMode startMode = ServiceStartMode.Disabled;

                    var stv = keyValue.Values.SingleOrDefault(t => t.ValueName == "Start");

                    if (stv != null)
                    {
                        startMode = (ServiceStartMode)int.Parse(stv.ValueData);
                    }

                  

                    var paramKey = keyValue.SubKeys.SingleOrDefault(t => t.KeyName == "Parameters");

                    var paramLastWrite = paramKey?.LastWriteTime;

                    var serviceDll = paramKey?.Values.SingleOrDefault(t => t.ValueName == "ServiceDLL")?.ValueData ?? String.Empty;


                    var ff = new Service(name,desc,disp,startMode, startType, nameLastWrite,paramLastWrite,group,imagePath,serviceDll,reqPrivs);

                    l.Add(ff);
                }
            }
            catch (Exception ex)
            {
                Errors.Add($"Error processing Services key: {ex.Message}");
            }


            if (Errors.Count > 0)
            {
                AlertMessage = "Errors detected. See Errors information in lower right corner of plugin window";
            }

            return l;
        }
    }
}