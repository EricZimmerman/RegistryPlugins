using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExtensionBlocks;

namespace RegistryPlugin.Taskband.ShellItems
{
    public class ShellBag0X3A : ShellBag
    {
        public ShellBag0X3A(byte[] rawBytes)
        {

            FriendlyName = "Directory";


            ExtensionBlocks = new List<IExtensionBlock>();


            var index = 0xA;

            var size = (int) rawBytes[index];

            index = 0xB;

            if (rawBytes[0xB] == 0x1)
            {
                index = 0xC;
            }

            var rawVal = CodePagesEncodingProvider.Instance.GetEncoding(1252).GetString(rawBytes, index, size);

            var segs = rawVal.Split('|');

            Value = segs.Last();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine(base.ToString());

            return sb.ToString();
        }
    }
}