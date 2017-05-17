using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Registry.Abstractions;
using RegistryPluginBase.Classes;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.Ares
{
    public class Ares : IRegistryPluginGrid
    {
        private readonly BindingList<ValuesOut> _values;

        public Ares()
        {
            _values = new BindingList<ValuesOut>();

            Errors = new List<string>();
        }

        public string InternalGuid => "7cbfaa3d-ef29-4e4b-8282-82f5a6f17a4a";

        public List<string> KeyPaths => new List<string>(new[]
        {
            @"Software\Ares"
        });

        public string ValueName => null;
        public string AlertMessage { get; private set; }
        public RegistryPluginType.PluginType PluginType => RegistryPluginType.PluginType.Grid;
        public string Author => "Eric Zimmerman";
        public string Email => "saericzimmerman@gmail.com";
        public string Phone => "501-313-3778";
        public string PluginName => "Ares P2P information";

        public string ShortDescription =>
            "Decodes various Ares P2P information including last connect, search terms, port #, etc";

        public string LongDescription =>
            "Decodes various Ares P2P information including last connect, search terms, port #, etc";

        public double Version => 0.5;
        public List<string> Errors { get; }

        public void ProcessValues(RegistryKey key)
        {
            _values.Clear();
            Errors.Clear();

            try
            {
                var networkDHTID = key.Values.SingleOrDefault(t => t.ValueName == "Network.DHTID");
                if (networkDHTID != null)
                {
                    var dh = networkDHTID.ValueData.Replace("-", "");

                    var v = new ValuesOut($"Ares Network DHTID", dh);

                    _values.Add(v);
                }

                var dlFolderVal = key.Values.SingleOrDefault(t => t.ValueName == "Download.Folder");
                if (dlFolderVal != null)
                {
                    var dlf = DecodeHexToAscii(dlFolderVal.ValueData);

                    var v = new ValuesOut($"Download folder", dlf);

                    _values.Add(v);
                }

                var customMshVal = key.Values.SingleOrDefault(t => t.ValueName == "Personal.CustomMessage");
                if (customMshVal != null)
                {
                    var customMsg = customMshVal.ValueData;

                    var v = new ValuesOut($"Custom message", customMsg);

                    _values.Add(v);
                }

                var nickVal = key.Values.SingleOrDefault(t => t.ValueName == "Personal.Nickname");
                if (nickVal != null)
                {
                    var customMsg = DecodeHexToAscii(nickVal.ValueData);

                    var v = new ValuesOut($"User nickname", customMsg);

                    _values.Add(v);
                }

                var awayMsgVal = key.Values.SingleOrDefault(t => t.ValueName == "PrivateMessage.AwayMessage");
                if (awayMsgVal != null)
                {
                    if (
                        awayMsgVal.ValueData.Equals(
                            "5468697320697320616E206175746F6D617469632061776179206D6573736167652067656E65726174656420627920417265732070726F6772616D2C20757365722069736E27742068657265206E6F772E") ==
                        false)
                    {
                        //user has changed default

                        var customMsg = DecodeHexToAscii(awayMsgVal.ValueData);

                        var v = new ValuesOut($"Away message", customMsg);

                        _values.Add(v);
                    }
                }


                var lastConnectedVal = key.Values.SingleOrDefault(t => t.ValueName == "Stats.LstConnect");
                if (lastConnectedVal != null)
                {
                    var lastConnect = DateTimeOffset.FromUnixTimeSeconds(int.Parse(lastConnectedVal.ValueData));

                    var v = new ValuesOut($"Last connection time", lastConnect.ToUniversalTime().ToString());

                    _values.Add(v);
                }


                var portVal = key.Values.SingleOrDefault(t => t.ValueName == "Transfer.ServerPort");

                if (portVal != null)
                {
                    var portNum = int.Parse(portVal.ValueData);

                    if (portNum > 0)
                    {
                        var v = new ValuesOut($"Port number", portNum.ToString());

                        _values.Add(v);
                    }
                }

                var guidVal = key.Values.SingleOrDefault(t => t.ValueName == "Personal.GUID");

                if (guidVal != null)
                {
                    var v = new ValuesOut($"Personal GUID", guidVal.ValueData);

                    _values.Add(v);
                }

                var searchKey = key.SubKeys.SingleOrDefault(t => t.KeyName == "Search.History");

                if (searchKey != null)
                {
                    foreach (var registryKey in searchKey.SubKeys)
                    {
                        if (registryKey.Values.Count == 0)
                        {
                            continue;
                        }

                        var terms = new List<string>();

                        foreach (var keyValue in registryKey.Values)
                        {
                            try
                            {
                                var st = DecodeHexToAscii(keyValue.ValueName);

                                terms.Add(st);
                            }
                            catch (Exception ex)
                            {
                                Errors.Add(
                                    $"Key: {registryKey.KeyName}, Value name: {keyValue.ValueName}, message: {ex.Message}");
                            }
                        }

                        var searchType = registryKey.KeyName.Substring(0, registryKey.KeyName.Length - 4);
                        if (searchType == "gen")
                        {
                            searchType = "all";
                        }

                        var v = new ValuesOut($"Search history for '{searchType}'", string.Join(", ", terms));

                        _values.Add(v);
                    }
                }
            }
            catch (Exception ex)
            {
                Errors.Add($"Error processing Ares search history: {ex.Message}");
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