using System;
using System.Linq;
using ExtensionBlocks;

namespace RegistryPluginBase.Classes
{
    public class Helpers
    {
        public static string Rot13Transform(string value)
        {
            var array = value.ToCharArray();
            for (var i = 0; i < array.Length; i++)
            {
                int number = array[i];

                if (number >= 'a' && number <= 'z')
                {
                    if (number > 'm')
                    {
                        number -= 13;
                    }
                    else
                    {
                        number += 13;
                    }
                }
                else if (number >= 'A' && number <= 'Z')
                {
                    if (number > 'M')
                    {
                        number -= 13;
                    }
                    else
                    {
                        number += 13;
                    }
                }
                array[i] = (char) number;
            }
            return new string(array);
        }

        private void ImportExt()
        {
            var e = Utils.ReplaceNulls("");
        }

        public static string ConvertHexStringToSidString(byte[] hex)
        {
            //If your SID is S-1-5-21-2127521184-1604012920-1887927527-72713, then your raw hex SID is 01 05 00 00 00 00 00 05 15000000 A065CF7E 784B9B5F E77C8770 091C0100

            //This breaks down as follows:
            //01 S-1
            //05 (seven dashes, seven minus two = 5)
            //000000000005 (5 = 0x000000000005, big-endian)
            //15000000 (21 = 0x00000015, little-endian)
            //A065CF7E (2127521184 = 0x7ECF65A0, little-endian)
            //784B9B5F (1604012920 = 0x5F9B4B78, little-endian)
            //E77C8770 (1887927527 = 0X70877CE7, little-endian)
            //091C0100 (72713 = 0x00011c09, little-endian)

            //page 191 http://amnesia.gtisc.gatech.edu/~moyix/suzibandit.ltd.uk/MSc/Registry%20Structure%20-%20Appendices%20V4.pdf

            //"01- 05- 00-00-00-00-00-05- 15-00-00-00- 82-F6-13-90- 30-42-81-99- 23-04-C3-8F- 51-04-00-00"
            //"01-01-00-00-00-00-00-05-12-00-00-00" == S-1-5-18  Local System 
            //"01-02-00-00-00-00-00-05-20-00-00-00-20-02-00-00" == S-1-5-32-544 Administrators
            //"01-01-00-00-00-00-00-05-0C-00-00-00" = S-1-5-12  Restricted Code 
            //"01-02-00-00-00-00-00-0F-02-00-00-00-01-00-00-00"

            const string header = "S";


            var sidVersion = hex[0].ToString();

            var authId = BitConverter.ToInt32(hex.Skip(4).Take(4).Reverse().ToArray(), 0);

            var index = 8;


            var sid = $"{header}-{sidVersion}-{authId}";

            do
            {
                var tempAuthHex = hex.Skip(index).Take(4).ToArray();

                var tempAuth = BitConverter.ToUInt32(tempAuthHex, 0);

                index += 4;

                sid = $"{sid}-{tempAuth}";
            } while (index < hex.Length);

            //some tests
            //var hexStr = BitConverter.ToString(hex);

            //switch (hexStr)
            //{
            //    case "01-01-00-00-00-00-00-05-12-00-00-00":

            //        Check.That(sid).IsEqualTo("S-1-5-18");

            //        break;

            //    case "01-02-00-00-00-00-00-05-20-00-00-00-20-02-00-00":

            //        Check.That(sid).IsEqualTo("S-1-5-32-544");

            //        break;

            //    case "01-01-00-00-00-00-00-05-0C-00-00-00":
            //        Check.That(sid).IsEqualTo("S-1-5-12");

            //        break;
            //    default:

            //        break;
            //}


            return sid;
        }
    }
}