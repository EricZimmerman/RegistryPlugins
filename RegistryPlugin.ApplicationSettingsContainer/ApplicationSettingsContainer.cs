using Registry.Abstractions;
using RegistryPluginBase.Classes;
using RegistryPluginBase.Interfaces;
using System.ComponentModel;
using System.Drawing;
using System.Text;

namespace RegistryPlugin.ApplicationSettingsContainer
{
    public class ApplicationSettingsContainer : IRegistryPluginGrid
    {
        private readonly BindingList<ValuesOut> _values;
        public ApplicationSettingsContainer()
        {
            _values = new BindingList<ValuesOut>();
            Errors = new List<string>();
        }
        public string InternalGuid => "722135ec-628c-4dc6-ba96-086002161828";

        public List<string> KeyPaths => new List<string>(new[]
        {
            @"LocalState",
            @"RoamingState",
            @"LocalState\*",
            @"RoamingState\*"
        });

        public string ValueName => null;
        public string AlertMessage { get; private set; }
        public RegistryPluginType.PluginType PluginType => RegistryPluginType.PluginType.Grid;
        public string Author => "ogmini https://ogmini.github.io/";
        public string Email => "ogminimk1@gmail.com";
        public string Phone => "000-0000-0000";
        public string PluginName => "ApplicationSettingsContainer";

        public string ShortDescription
            => "Application Settings/Data stored in settings.dat for Packaged Applications"; 

        public string LongDescription
            => "Can contain settings and other data that are associated with specific applications. RegUwpCompositeValue have not been fully decipered yet. It is possible to manually parse and read them. RegUwpDateTimeOffset are an Int64 that could be representing a Windows FILETIME or DateTime.Ticks. https://ogmini.github.io/tags.html#Registryhive"; //TODO: Add better documentation/writeup link

        public double Version => 0.1;
        public List<string> Errors { get; }

        public void ProcessValues(RegistryKey key)
        {
            _values.Clear();
            Errors.Clear();

            ProcessKeys(key);

        }

        public void ProcessKeys(RegistryKey key)
        {
            try
            {
                foreach (var k in key.Values)
                {
                    int numRecs = 0;
                    string val = string.Empty;

                    switch (k.VkRecord.DataTypeRaw)
                    {
                        case 257: //RegUwpByte
                            _values.Add(new ValuesOut(k.ValueName, key.KeyPath, "RegUwpByte", $"0x{k.ValueDataRaw[0]:X2}", 
                                DateTime.FromFileTimeUtc(BitConverter.ToInt64(k.ValueDataRaw, 1))));
                            break;
                        case 258: //RegUwpInt16
                            _values.Add(new ValuesOut(k.ValueName, key.KeyPath, "RegUwpInt16", BitConverter.ToInt16(k.ValueDataRaw, 0).ToString(), 
                                DateTime.FromFileTimeUtc(BitConverter.ToInt64(k.ValueDataRaw, 2))));
                            break;
                        case 259: //RegUwpUint16
                            _values.Add(new ValuesOut(k.ValueName, key.KeyPath, "RegUwpUint16", BitConverter.ToUInt16(k.ValueDataRaw, 0).ToString(), 
                                DateTime.FromFileTimeUtc(BitConverter.ToInt64(k.ValueDataRaw, 2))));
                            break;
                        case 260: //RegUwpInt32
                            _values.Add(new ValuesOut(k.ValueName, key.KeyPath, "RegUwpInt32", BitConverter.ToInt32(k.ValueDataRaw, 0).ToString(),
                                DateTime.FromFileTimeUtc(BitConverter.ToInt64(k.ValueDataRaw, 4))));
                            break;
                        case 261: //RegUwpUint32
                            _values.Add(new ValuesOut(k.ValueName, key.KeyPath, "RegUwpUint32", BitConverter.ToUInt32(k.ValueDataRaw, 0).ToString(), 
                                DateTime.FromFileTimeUtc(BitConverter.ToInt64(k.ValueDataRaw, 4))));
                            break;
                        case 262: //RegUwpInt64
                            _values.Add(new ValuesOut(k.ValueName, key.KeyPath, "RegUwpInt64", BitConverter.ToInt64(k.ValueDataRaw, 0).ToString(), 
                                DateTime.FromFileTimeUtc(BitConverter.ToInt64(k.ValueDataRaw, 8))));
                            break;
                        case 263: //RegUwpUint64
                            _values.Add(new ValuesOut(k.ValueName, key.KeyPath, "RegUwpUint64", BitConverter.ToUInt64(k.ValueDataRaw, 0).ToString(), 
                                DateTime.FromFileTimeUtc(BitConverter.ToInt64(k.ValueDataRaw, 8))));
                            break;
                        case 264: //RegUwpSingle
                            _values.Add(new ValuesOut(k.ValueName, key.KeyPath, "RegUwpSingle", BitConverter.ToSingle(k.ValueDataRaw, 0).ToString(), 
                                DateTime.FromFileTimeUtc(BitConverter.ToInt64(k.ValueDataRaw, 4))));
                            break;
                        case 265: //RegUwpDouble
                            _values.Add(new ValuesOut(k.ValueName, key.KeyPath, "RegUwpDouble", BitConverter.ToDouble(k.ValueDataRaw, 0).ToString(), 
                                DateTime.FromFileTimeUtc(BitConverter.ToInt64(k.ValueDataRaw, 8))));
                            break;
                        case 266: //RegUwpChar
                            _values.Add(new ValuesOut(k.ValueName, key.KeyPath, "RegUwpChar", BitConverter.ToChar(k.ValueDataRaw, 0).ToString(), 
                                DateTime.FromFileTimeUtc(BitConverter.ToInt64(k.ValueDataRaw, 2))));
                            break;
                        case 267: //RegUwpBoolean
                            _values.Add(new ValuesOut(k.ValueName, key.KeyPath, "RegUwpBoolean", BitConverter.ToBoolean(k.ValueDataRaw, 0).ToString(), 
                                DateTime.FromFileTimeUtc(BitConverter.ToInt64(k.ValueDataRaw, 1))));
                            break;
                        case 268: //RegUwpString
                            int end = 0;
                            while (end + 1 < k.ValueDataRaw.Length)
                            {
                                if (k.ValueDataRaw[end] == 0x00 && k.ValueDataRaw[end + 1] == 0x00)
                                    break;
                                end += 2;
                            }

                            val = Encoding.Unicode.GetString(k.ValueDataRaw, 0, end);
                            _values.Add(new ValuesOut(k.ValueName, key.KeyPath, "RegUwpString", val, DateTime.FromFileTimeUtc(BitConverter.ToInt64(k.ValueDataRaw, end + 2))));
                            break;
                        case 269: //RegUwpCompositeValue //TODO: Decipher this. For now we just output the raw bytes
                            numRecs = (int)(k.VkRecord.DataLength - 8) / 1;
                            byte[] valComposite = new byte[numRecs];
                            Array.Copy(k.ValueDataRaw, 0, valComposite, 0, numRecs);

                            val = string.Format("[{0}]", string.Join(", ", valComposite.Select(b => $"0x{b:X2}")));
                            _values.Add(new ValuesOut(k.ValueName, key.KeyPath, "RegUwpCompositeValue", val, 
                                DateTime.FromFileTimeUtc(BitConverter.ToInt64(k.ValueDataRaw, numRecs)),
                                "Composite Value has not been fully decipered. This is a collection of other RegUwp keys."));
                            break;
                        case 270: //RegUwpDateTimeOffset
                            _values.Add(new ValuesOut(k.ValueName, key.KeyPath, "RegUwpDateTimeOffset",
                                (BitConverter.ToInt64(k.ValueDataRaw, 0)).ToString(),
                                DateTime.FromFileTimeUtc(BitConverter.ToInt64(k.ValueDataRaw, 8)),
                                "This Int64 value has been observered to be representing Windows FILETIME or DateTime.Ticks."));
                            break;
                        case 271: //RegUwpTimeSpan
                            _values.Add(new ValuesOut(k.ValueName, key.KeyPath, "RegUwpTimeSpan", new TimeSpan(BitConverter.ToInt64(k.ValueDataRaw, 0)).ToString(), 
                                DateTime.FromFileTimeUtc(BitConverter.ToInt64(k.ValueDataRaw, 8))));
                            break;
                        case 272: //RegUwpGuid
                            byte[] guidBytes = new byte[16];
                            Array.Copy(k.ValueDataRaw, 0, guidBytes, 0, 16);

                            _values.Add(new ValuesOut(k.ValueName, key.KeyPath, "RegUwpGuid", 
                                new Guid(guidBytes).ToString(), 
                                DateTime.FromFileTimeUtc(BitConverter.ToInt64(k.ValueDataRaw, 16))));
                            break;
                        case 273: //RegUwpPoinp
                            _values.Add(new ValuesOut(k.ValueName, key.KeyPath, "RegUwpPoint", 
                                new Point((int)BitConverter.ToSingle(k.ValueDataRaw, 0), 
                                (int)BitConverter.ToSingle(k.ValueDataRaw, 4)).ToString(), 
                                DateTime.FromFileTimeUtc(BitConverter.ToInt64(k.ValueDataRaw, 8))));
                            break;
                        case 274: //RegUwpSize
                            _values.Add(new ValuesOut(k.ValueName, key.KeyPath, "RegUwpSize", 
                                new Size((int)BitConverter.ToSingle(k.ValueDataRaw, 0), 
                                (int)BitConverter.ToSingle(k.ValueDataRaw, 4)).ToString(), 
                                DateTime.FromFileTimeUtc(BitConverter.ToInt64(k.ValueDataRaw, 8))));
                            break;
                        case 275: //RegUwpRect
                            _values.Add(new ValuesOut(k.ValueName, key.KeyPath, "RegUwpRect", 
                                new Rectangle((int)BitConverter.ToSingle(k.ValueDataRaw, 0), 
                                (int)BitConverter.ToSingle(k.ValueDataRaw, 4), 
                                (int)BitConverter.ToSingle(k.ValueDataRaw, 8), 
                                (int)BitConverter.ToSingle(k.ValueDataRaw, 12)).ToString(), 
                                DateTime.FromFileTimeUtc(BitConverter.ToInt64(k.ValueDataRaw, 16))));
                            break;
                        case 276: //RegUwpArrayByte
                            numRecs = (int)(k.VkRecord.DataLength - 8) / 1;
                            byte[] valBytes = new byte[numRecs];
                            Array.Copy(k.ValueDataRaw, 0, valBytes, 0, numRecs);

                            val = string.Format("[{0}]", string.Join(", ", valBytes.Select(b => $"0x{b:X2}")));
                            _values.Add(new ValuesOut(k.ValueName, key.KeyPath, "RegUwpArrayByte", val, 
                                DateTime.FromFileTimeUtc(BitConverter.ToInt64(k.ValueDataRaw, numRecs))));
                            break;
                        case 277: //RegUwpArrayInt16
                            numRecs = (int)(k.VkRecord.DataLength - 8) / 2;
                            Int16[] valInt16 = new Int16[numRecs];

                            for (int i = 0; i < numRecs; i++)
                            {
                                valInt16[i] = BitConverter.ToInt16(k.ValueDataRaw, i * 2);
                            }
                            val = string.Format("[{0}]", string.Join(",", valInt16));
                            _values.Add(new ValuesOut(k.ValueName, key.KeyPath, "RegUwpArrayInt16", val, 
                                DateTime.FromFileTimeUtc(BitConverter.ToInt64(k.ValueDataRaw, numRecs * 2))));
                            break;
                        case 278: //RegUwpArrayUint16
                            numRecs = (int)(k.VkRecord.DataLength - 8) / 2;
                            UInt16[] valUint16 = new UInt16[numRecs];

                            for (int i = 0; i < numRecs; i++)
                            {
                                valUint16[i] = BitConverter.ToUInt16(k.ValueDataRaw, i * 2);
                            }
                            val = string.Format("[{0}]", string.Join(",", valUint16));
                            _values.Add(new ValuesOut(k.ValueName, key.KeyPath, "RegUwpArrayUint16", val, 
                                DateTime.FromFileTimeUtc(BitConverter.ToInt64(k.ValueDataRaw, numRecs * 2))));
                            break;
                        case 279: //RegUwpArrayInt32
                            numRecs = (int)(k.VkRecord.DataLength - 8) / 4;
                            Int32[] valInt32 = new Int32[numRecs];

                            for (int i = 0; i < numRecs; i++)
                            {
                                valInt32[i] = BitConverter.ToInt32(k.ValueDataRaw, i * 4);
                            }
                            val = string.Format("[{0}]", string.Join(",", valInt32));
                            _values.Add(new ValuesOut(k.ValueName, key.KeyPath, "RegUwpArrayInt32", val, 
                                DateTime.FromFileTimeUtc(BitConverter.ToInt64(k.ValueDataRaw, numRecs * 4))));
                            break;
                        case 280: //RegUwpArrayUint32
                            numRecs = (int)(k.VkRecord.DataLength - 8) / 4;
                            UInt32[] valUint32 = new UInt32[numRecs];

                            for (int i = 0; i < numRecs; i++)
                            {
                                valUint32[i] = BitConverter.ToUInt32(k.ValueDataRaw, i * 4);
                            }
                            val = string.Format("[{0}]", string.Join(",", valUint32));
                            _values.Add(new ValuesOut(k.ValueName, key.KeyPath, "RegUwpArrayUint32", val, 
                                DateTime.FromFileTimeUtc(BitConverter.ToInt64(k.ValueDataRaw, numRecs * 4))));
                            break;
                        case 281: //RegUwpArrayInt64
                            numRecs = (int)(k.VkRecord.DataLength - 8) / 8;
                            Int64[] valInt64 = new Int64[numRecs];

                            for (int i = 0; i < numRecs; i++)
                            {
                                valInt64[i] = BitConverter.ToInt64(k.ValueDataRaw, i * 8);
                            }
                            val = string.Format("[{0}]", string.Join(",", valInt64));
                            _values.Add(new ValuesOut(k.ValueName, key.KeyPath, "RegUwpArrayInt64", val, 
                                DateTime.FromFileTimeUtc(BitConverter.ToInt64(k.ValueDataRaw, numRecs * 8))));
                            break;
                        case 282: //RegUwpArrayUint64
                            numRecs = (int)(k.VkRecord.DataLength - 8) / 8;
                            UInt64[] valUint64 = new UInt64[numRecs];

                            for (int i = 0; i < numRecs; i++)
                            {
                                valUint64[i] = BitConverter.ToUInt64(k.ValueDataRaw, i * 8);
                            }
                            val = string.Format("[{0}]", string.Join(",", valUint64));
                            _values.Add(new ValuesOut(k.ValueName, key.KeyPath, "RegUwpArrayUint64", val, 
                                DateTime.FromFileTimeUtc(BitConverter.ToInt64(k.ValueDataRaw, numRecs * 8))));
                            break;
                        case 283: //RegUwpArraySingle
                            numRecs = (int)(k.VkRecord.DataLength - 8) / 4;
                            Single[] valSingle = new Single[numRecs];

                            for (int i = 0; i < numRecs; i++)
                            {
                                valSingle[i] = BitConverter.ToSingle(k.ValueDataRaw, i * 4);
                            }
                            val = string.Format("[{0}]", string.Join(",", valSingle));
                            _values.Add(new ValuesOut(k.ValueName, key.KeyPath, "RegUwpArraySingle", val, 
                                DateTime.FromFileTimeUtc(BitConverter.ToInt64(k.ValueDataRaw, numRecs * 4))));
                            break;
                        case 284: //RegUwpArrayDouble
                            numRecs = (int)(k.VkRecord.DataLength - 8) / 8;
                            Double[] valDouble = new Double[numRecs];

                            for (int i = 0; i < numRecs; i++)
                            {
                                valDouble[i] = BitConverter.ToDouble(k.ValueDataRaw, i * 8);
                            }
                            val = string.Format("[{0}]", string.Join(",", valDouble));
                            _values.Add(new ValuesOut(k.ValueName, key.KeyPath, "RegUwpArrayDouble", val, 
                                DateTime.FromFileTimeUtc(BitConverter.ToInt64(k.ValueDataRaw, numRecs * 8))));
                            break;
                        case 285: //RegUwpArrayChar
                            numRecs = (int)(k.VkRecord.DataLength - 8) / 2;
                            Char[] valChar = new Char[numRecs];

                            for (int i = 0; i < numRecs; i++)
                            {
                                valChar[i] = BitConverter.ToChar(k.ValueDataRaw, i * 2);
                            }
                            val = string.Format("[{0}]", string.Join(",", valChar));
                            _values.Add(new ValuesOut(k.ValueName, key.KeyPath, "RegUwpArrayChar", val, 
                                DateTime.FromFileTimeUtc(BitConverter.ToInt64(k.ValueDataRaw, numRecs * 2))));
                            break;
                        case 286: //RegUwpArrayBoolean
                            numRecs = (int)(k.VkRecord.DataLength - 8);
                            bool[] valBool = new bool[numRecs];

                            for (int i = 0; i < numRecs; i++)
                            {
                                valBool[i] = BitConverter.ToBoolean(k.ValueDataRaw, i);
                            }
                            val = string.Format("[{0}]", string.Join(",", valBool));
                            _values.Add(new ValuesOut(k.ValueName, key.KeyPath, "RegUwpArrayBoolean", val, 
                                DateTime.FromFileTimeUtc(BitConverter.ToInt64(k.ValueDataRaw, numRecs))));
                            break;
                        case 287: //RegUwpArrayString
                            int check = 0;
                            List<string> elStrings = new List<string>();

                            while (check < (int)(k.VkRecord.DataLength - 8))
                            {
                                int lengthString = BitConverter.ToInt32(k.ValueDataRaw, check);
                                string elementString = Encoding.Unicode.GetString(k.ValueDataRaw, check + 4, lengthString - 2);
                                elStrings.Add(elementString);
                                check = check + lengthString + 4;
                            }
                            val = string.Format("[{0}]", string.Join(",", elStrings));
                            _values.Add(new ValuesOut(k.ValueName, key.KeyPath, "RegUwpArrayString", val, 
                                DateTime.FromFileTimeUtc(BitConverter.ToInt64(k.ValueDataRaw, (int)(k.VkRecord.DataLength - 8)))));
                            break;
                        case 288: //RegUwpArrayDateTimeOffset
                            numRecs = (int)(k.VkRecord.DataLength - 8) / 8;
                            Int64[] valDateTimeOffset = new Int64[numRecs];

                            for (int i = 0; i < numRecs; i++)
                            {
                                valDateTimeOffset[i] = BitConverter.ToInt64(k.ValueDataRaw, i * 8);
                            }
                            val = string.Format("[{0}]", string.Join(",", valDateTimeOffset));
                            _values.Add(new ValuesOut(k.ValueName, key.KeyPath, "RegUwpArrayDateTimeOffset", val, 
                                DateTime.FromFileTimeUtc(BitConverter.ToInt64(k.ValueDataRaw, numRecs * 8)),
                                "These Int64 values have been observered to be representing Windows FILETIME or DateTime.Ticks."));
                            break;
                        case 289: //RegUwpArrayTimeSpan 
                            numRecs = (int)(k.VkRecord.DataLength - 8) / 8;
                            TimeSpan[] valTimeSpan = new TimeSpan[numRecs];

                            for (int i = 0; i < numRecs; i++)
                            {
                                valTimeSpan[i] = new TimeSpan(BitConverter.ToInt64(k.ValueDataRaw, i * 8));
                            }
                            val = string.Format("[{0}]", string.Join(",", valTimeSpan));
                            _values.Add(new ValuesOut(k.ValueName, key.KeyPath, "RegUwpArrayTimeSpan", val, 
                                DateTime.FromFileTimeUtc(BitConverter.ToInt64(k.ValueDataRaw, numRecs * 8))));
                            break;
                        case 290: //RegUwpArrayGuid
                            numRecs = (int)(k.VkRecord.DataLength - 8) / 16;
                            Guid[] valGUID = new Guid[numRecs];

                            for (int i = 0; i < numRecs; i++)
                            {
                                byte[] gBytes = new byte[16];
                                Array.Copy(k.ValueDataRaw, i * 16, gBytes, 0, 16);
                                valGUID[i] = new Guid(gBytes);
                            }
                            val = string.Format("[{0}]", string.Join(",", valGUID));
                            _values.Add(new ValuesOut(k.ValueName, key.KeyPath, "RegUwpArrayGuid", val, 
                                DateTime.FromFileTimeUtc(BitConverter.ToInt64(k.ValueDataRaw, numRecs * 16))));
                            break;
                        case 291: //RegUwpArrayPoint
                            numRecs = (int)(k.VkRecord.DataLength - 8) / 8;
                            List<Point> valPoint = new List<Point>();

                            for (int i = 0; i < numRecs; i++)
                            {
                                valPoint.Add(new Point((int)BitConverter.ToSingle(k.ValueDataRaw, i * 8), (int)BitConverter.ToSingle(k.ValueDataRaw, (i * 8) + 4)));
                            }

                            val = string.Format("[{0}]", string.Join(",", valPoint));
                            _values.Add(new ValuesOut(k.ValueName, key.KeyPath, "RegUwpArrayPoint", val, 
                                DateTime.FromFileTimeUtc(BitConverter.ToInt64(k.ValueDataRaw, numRecs * 8))));
                            break;
                        case 292: //RegUwpArraySize
                            numRecs = (int)(k.VkRecord.DataLength - 8) / 8;
                            List<Size> valSize = new List<Size>();

                            for (int i = 0; i < numRecs; i++)
                            {
                                valSize.Add(new Size((int)BitConverter.ToSingle(k.ValueDataRaw, i * 8), (int)BitConverter.ToSingle(k.ValueDataRaw, (i * 8) + 4)));
                            }

                            val = string.Format("[{0}]", string.Join(",", valSize));
                            _values.Add(new ValuesOut(k.ValueName, key.KeyPath, "RegUwpArraySize", val, 
                                DateTime.FromFileTimeUtc(BitConverter.ToInt64(k.ValueDataRaw, numRecs * 8))));
                            break;
                        case 293: //RegUwpArrayRect
                            numRecs = (int)(k.VkRecord.DataLength - 8) / 16;
                            List<Rectangle> valRect = new List<Rectangle>();

                            for (int i = 0; i < numRecs; i++)
                            {
                                valRect.Add(new Rectangle((int)BitConverter.ToSingle(k.ValueDataRaw, i * 8),
                                    (int)BitConverter.ToSingle(k.ValueDataRaw, (i * 8) + 4),
                                    (int)BitConverter.ToSingle(k.ValueDataRaw, (i * 8) + 8),
                                    (int)BitConverter.ToSingle(k.ValueDataRaw, (i * 8) + 12)));
                            }

                            val = string.Format("[{0}]", string.Join(",", valRect));
                            _values.Add(new ValuesOut(k.ValueName, key.KeyPath, "RegUwpArrayRect", val, 
                                DateTime.FromFileTimeUtc(BitConverter.ToInt64(k.ValueDataRaw, numRecs * 16))));
                            break;
                        default:
                            break;
                    }

                }
            }
            catch (Exception ex)
            {
                Errors.Add($"Error processing ApplicationSettingsContainer: {ex.Message}");
            }

            if (Errors.Count > 0)
            {
                AlertMessage = "Errors detected. See Errors information in lower right corner of plugin window";
            }
        }

        public IBindingList Values => _values;
    }
}

