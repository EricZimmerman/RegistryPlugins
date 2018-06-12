using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Registry.Abstractions;
using RegistryPluginBase.Classes;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.TaskFlowShellActivities
{
    public class TaskFlowShellActivities : IRegistryPluginGrid
    {
        private readonly BindingList<ValuesOut> _values;

        public TaskFlowShellActivities()
        {
            _values = new BindingList<ValuesOut>();

            Errors = new List<string>();
        }

        public string InternalGuid => "b4570ca3-a09c-4017-848d-c7e7ea42d8f7";

        public List<string> KeyPaths => new List<string>(new[]
        {
            @"Software\Microsoft\Windows\CurrentVersion\CloudStore\Store\Cache\DefaultAccount\$$windows.data.taskflow.shellactivities\Current"
        });

        public string ValueName => null;
        public string AlertMessage { get; private set; }
        public RegistryPluginType.PluginType PluginType => RegistryPluginType.PluginType.Grid;
        public string Author => "Eric Zimmerman";
        public string Email => "saericzimmerman@gmail.com";
        public string Phone => "501-313-3778";
        public string PluginName => "Taskflow Shell Activities";

        public string ShortDescription =>
            "Extracts information from Taskflow Shell Activities";

        public string LongDescription => ShortDescription;

        public double Version => 0.5;
        public List<string> Errors { get; }


        public void ProcessValues(RegistryKey key)
        {
            _values.Clear();
            Errors.Clear();

            try
            {
                var dataVal = key.Values.SingleOrDefault(t => t.ValueName == "Data");

                if (dataVal != null)
                {
                    var rawData = dataVal.ValueDataRaw;
                    var index = 4;

                    var ts = DateTimeOffset.FromFileTime(BitConverter.ToInt64(rawData, index)).ToUniversalTime();

                    var val = new ValuesOut(null, null, null, $"Timestamp: {ts:yyyy-MM-dd HH:mm:ss}");
                    _values.Add(val);

                    index += 8;

                    index += 0xd; //skip unknown

                    while (index < rawData.Length)
                    {
                        if (index + 1 == rawData.Length)
                        {
                            break;
                        }

                        var type = BitConverter.ToInt16(rawData, index);
                        index += 2;
                        var size = rawData[index];
                        index += 1;

                        var fullPath = Encoding.Unicode.GetString(rawData, index, size * 2);
                        index += size * 2;

                        index += 5; //skip unknown

                        if (rawData[index] != 0xd2)
                        {
                            index += 1;
                        }

                        type = BitConverter.ToInt16(rawData, index);
                        index += 2;
                        size = rawData[index];
                        index += 1;
                        var exeName = Encoding.Unicode.GetString(rawData, index, size * 2);
                        index += size * 2;

                        type = BitConverter.ToInt16(rawData, index);

                        var windowTitle = string.Empty;

                        if (type != 0x32c6)
                        {
                            index += 2;
                            size = rawData[index];
                            index += 1;

                            if (rawData[index] == 0x01)
                            {
                                index += 1;
                            }

                            windowTitle = Encoding.Unicode.GetString(rawData, index, size * 2).Trim();
                            index += size * 2;

                            index += 0x1a; //skip unknown
                        }
                        else
                        {
                            index += 0x1a; //skip unknown
                        }


                        val = new ValuesOut(fullPath, exeName, windowTitle, null);
                        _values.Add(val);
                    }
                }
            }
            catch (Exception ex)
            {
                Errors.Add($"Error processing Taskflow Shell Activities: {ex.Message}");
            }

            if (Errors.Count > 0)
            {
                AlertMessage = "Errors detected. See Errors information in lower right corner of plugin window";
            }
        }


        public IBindingList Values => _values;
    }
}