using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Registry.Abstractions;
using RegistryPluginBase.Classes;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.Adobe.TrustManager_Websites
{
public class AdobeTrustManager : IRegistryPluginGrid
    {
        private readonly BindingList<ValuesOut> _values;

        public AdobeTrustManager()
        {
            _values = new BindingList<ValuesOut>();

            Errors = new List<string>();
        }

        public string InternalGuid => "91d835da-fb76-4b34-8732-22fabcc5ebbb";

        public List<string> KeyPaths => new List<string>(new[]
        {
            @"Software\Adobe"
        });

        public string ValueName => null;
        public string AlertMessage { get; private set; }
        public RegistryPluginType.PluginType PluginType => RegistryPluginType.PluginType.Grid;
        public string Author => "Mahmoud Soheem";
        public string Email => "mahmood_soheem@yahoo.com";
        public string Phone => "+201069641596";
        public string PluginName => "Adobe User Accessed Websites";

        public string ShortDescription =>
            "Decodes Adobe Trust Manager Websites list";

        public string LongDescription =>
            "Parses Adobe trust manager websites accessed, and User Choice (Allowed or Blocked) ";

        public double Version => 0.5;
        public List<string> Errors { get; }

        public void ProcessValues(RegistryKey key)
        {
            _values.Clear();
            Errors.Clear();

            try
            {

                foreach (var productKey in key.SubKeys)
                {
                    foreach (var versionKey in productKey.SubKeys)
                    {
                      
                        var trustManager = versionKey.SubKeys.SingleOrDefault(t => t.KeyName == "TrustManager");

                        var cDefaultLaunchURLPerms = trustManager?.SubKeys.SingleOrDefault(t => t.KeyName == "cDefaultLaunchURLPerms");

                        if (cDefaultLaunchURLPerms == null)
                        {
                            continue;
                        }
                        var tHostPerms = cDefaultLaunchURLPerms.Values.SingleOrDefault(t => t.ValueName == "tHostPerms")?.ValueData;
                        var websites = tHostPerms.Split('|');
                        foreach (var website in websites)
                        {
                            if (website.Contains("version"))
                            {
                                continue;
                            }
                            var vWebsite = new ValuesOut(website.Substring(0, website.Length - 2), website[website.Length - 1]);
                            vWebsite.BatchKeyPath = cDefaultLaunchURLPerms.KeyPath;
                            vWebsite.BatchValueName = cDefaultLaunchURLPerms.KeyName;

                            _values.Add(vWebsite);
                        }
                     
                    }
                }  
                
            }
            catch (Exception ex)
            {
                Errors.Add($"Error processing Adobe products: {ex.Message}");
            }

            if (Errors.Count > 0)
            {
                AlertMessage = "Errors detected. See Errors information in lower right corner of plugin window";
            }
        }

        

        public IBindingList Values => _values;

        private static string DecodeHexToAscii(string hex)
        {
            var sb = new StringBuilder();

            for (var i = 0; i < hex.Length - 1; i += 2)
            {
                var chunk = hex.Substring(i, 2);

                sb.Append(Convert.ToChar(Convert.ToUInt32(chunk, 16)).ToString());
            }

            return sb.ToString();
        }
    }
}
