using System;
using System.Collections.Generic;
using System.ComponentModel;
using Registry.Abstractions;
using RegistryPluginBase.Classes;
using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.TimeZoneInformation
{
    public class TimeZoneInfo : IRegistryPluginGrid
    {
        private readonly BindingList<ValuesOut> _values;

        public TimeZoneInfo()
        {
            _values = new BindingList<ValuesOut>();

            Errors = new List<string>();
        }

        public string InternalGuid => "dccbcb67-e729-4ce5-aae8-a91a5ed22a60";

        public List<string> KeyPaths => new List<string>(new[]
        {
            @"ControlSet001\Control\TimeZoneInformation",
            @"ControlSet002\Control\TimeZoneInformation",
            @"ControlSet003\Control\TimeZoneInformation",
            @"ControlSet004\Control\TimeZoneInformation",
            @"ControlSet005\Control\TimeZoneInformation"
        });

        public string ValueName => null;
        public string AlertMessage { get; private set; }
        public RegistryPluginType.PluginType PluginType => RegistryPluginType.PluginType.Grid;
        public string Author => "Eric Zimmerman";
        public string Email => "saericzimmerman@gmail.com";
        public string Phone => "501-313-3778";
        public string PluginName => "TimeZoneInformation";

        public string ShortDescription =>
            "Displays values from TimeZoneInformation key in a more usable format for timezone bias, etc."
            ;

        public string LongDescription =>
            "Displays values from TimeZoneInformation key in a more usable format for timezone bias, etc."
            ;

        public double Version => 0.5;

        public List<string> Errors { get; }

        public void ProcessValues(RegistryKey key)
        {
            _values.Clear();
            Errors.Clear();


            var vn = string.Empty;

            try
            {
                foreach (var keyValue in key.Values)
                {
                    vn = keyValue.ValueName;

                    switch (keyValue.ValueName)
                    {
                        case "Bias":
                            var b0 = BitConverter.ToInt32(keyValue.ValueDataRaw, 0);

                            _values.Add(new ValuesOut(keyValue.ValueName, b0.ToString(), keyValue.ValueData));

                            break;
                        case "StandardName":

                            _values.Add(new ValuesOut(keyValue.ValueName, keyValue.ValueData, keyValue.ValueData));

                            break;
                        case "StandardBias":

                            var b1 = BitConverter.ToInt32(keyValue.ValueDataRaw, 0);

                            _values.Add(new ValuesOut(keyValue.ValueName, b1.ToString(), keyValue.ValueData));

                            break;
                        case "StandardStart":
                            //santiago utc - 4
                            //
                            //start			00 00 0A 00 02 00 17 00 3B 00 3B 00 E7 03 06 00
                            //hour			24
                            //day of week		6
                            //week of month		2
                            //month			10
                            //
                            //			00 00 
                            //			0A == 10	Month
                            //			00 
                            //			02 		week of month
                            //			00 
                            //			17 == 23	hour
                            //			00 
                            //			3B == 59	minute
                            //			00 
                            //			3B == 59	second
                            //			00 
                            //			E7 03 == 999	millisecond?
                            //			06 00		day of week

                            var month0 = keyValue.ValueDataRaw[2];
                            var weekOfMonth0 = keyValue.ValueDataRaw[4];
                            var hour0 = keyValue.ValueDataRaw[6];
                            var minute0 = keyValue.ValueDataRaw[8];
                            var second0 = keyValue.ValueDataRaw[10];
                            var millisecond0 = BitConverter.ToInt16(keyValue.ValueDataRaw, 12);
                            var dayOfWeek0 = keyValue.ValueDataRaw[14];

                            var ss =
                                $"Month {month0}, week of month {weekOfMonth0}, day of week {dayOfWeek0}, Hours:Minutes:Seconds:Milliseconds {hour0}:{minute0}:{second0}:{millisecond0}";

                            _values.Add(new ValuesOut(keyValue.ValueName, ss, keyValue.ValueData));

                            break;
                        case "DaylightName":
                            _values.Add(new ValuesOut(keyValue.ValueName, keyValue.ValueData, keyValue.ValueData));

                            break;
                        case "DaylightBias":
                            var b2 = BitConverter.ToInt32(keyValue.ValueDataRaw, 0);

                            _values.Add(new ValuesOut(keyValue.ValueName, b2.ToString(), keyValue.ValueData));

                            break;
                        case "DaylightStart":

                            var month1 = keyValue.ValueDataRaw[2];
                            var weekOfMonth1 = keyValue.ValueDataRaw[4];
                            var hour1 = keyValue.ValueDataRaw[6];
                            var minute1 = keyValue.ValueDataRaw[8];
                            var second1 = keyValue.ValueDataRaw[10];
                            var millisecond1 = BitConverter.ToInt16(keyValue.ValueDataRaw, 12);
                            var dayOfWeek1 = keyValue.ValueDataRaw[14];

                            var ss1 =
                                $"Month {month1}, week of month {weekOfMonth1}, day of week {dayOfWeek1}, Hours:Minutes:Seconds:Milliseconds {hour1}:{minute1}:{second1}:{millisecond1}";

                            _values.Add(new ValuesOut(keyValue.ValueName, ss1, keyValue.ValueData));


                            break;
                        case "ActiveTimeBias":

                            var b3 = BitConverter.ToInt32(keyValue.ValueDataRaw, 0);

                            _values.Add(new ValuesOut(keyValue.ValueName, b3.ToString(), keyValue.ValueData));

                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Errors.Add($"Error processing TimeZoneInformation value '{vn}': {ex.Message}");
            }


            if (Errors.Count > 0)
            {
                AlertMessage = "Errors detected. See Errors information in lower right corner of plugin window";
            }
        }

        public IBindingList Values => _values;
    }
}