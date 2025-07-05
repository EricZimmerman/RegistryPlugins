﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExtensionBlocks;

namespace RegistryPlugin.OpenSavePidlMRU.ShellItems
{
    public class ShellBag0X32 : ShellBag
    {
        public ShellBag0X32(byte[] rawBytes)
        {
            FriendlyName = "File";

            ExtensionBlocks = new List<IExtensionBlock>();

            ShortName = string.Empty;

            var index = 2;

            if (rawBytes[0x28] == 0x2f || rawBytes[0x24] == 0x4e && rawBytes[0x26] == 0x2f && rawBytes[0x28] == 0x41)
            {
                //we have a good date

                try
                {
                    var zip = new ShellBagZipContents(rawBytes);
                    FriendlyName = zip.FriendlyName;
                    LastAccessTime = zip.LastAccessTime;

                    Value = zip.Value;

                    return;
                }
                catch (Exception)
                {
                }
            }

            index += 1;

            //skip unknown byte
            index += 1;

            var fileSize = BitConverter.ToUInt32(rawBytes, index);


            FileSize = (int) fileSize;

            index += 4; // skip file size since always 0 for directory

            var tempBytes = new byte[4];
            Array.Copy(rawBytes, index, tempBytes, 0, 4);
            var lastmodifiedUtcRaw = tempBytes;

            LastModificationTime = Utils.ExtractDateTimeOffsetFromBytes(lastmodifiedUtcRaw);


            index += 4;

            index += 2;

            var len = 0;

            if (rawBytes[2] == 0x32) // ascii
            {
                while (rawBytes[index + len] != 0x0)
                {
                    len += 1;
                }
            }
            else if (rawBytes[2] == 0x36) // unicode
            {
                while (!(rawBytes[index + len] == 0x0 && rawBytes[index + len + 1] == 0x0))
                {
                    len += 2;
                }
            }

            tempBytes = new byte[len];
            Array.Copy(rawBytes, index, tempBytes, 0, len);

            index += len;

            var shortName = CodePagesEncodingProvider.Instance.GetEncoding(1252).GetString(tempBytes);

            ShortName = shortName;

            while (rawBytes[index] == 0x0)
            {
                index += 1;
            }

            //we are at extension blocks, so cut them up and process
            // here is where we need to cut up the rest into extension blocks
            var chunks = new List<byte[]>();

            while (index < rawBytes.Length)
            {
                var subshellitemdatasize = BitConverter.ToInt16(rawBytes, index);

                if (subshellitemdatasize <= 0)
                {
                    break;
                }

                if (subshellitemdatasize == 1)
                {
                    //some kind of separator
                    index += 2;
                }
                else
                {
                    chunks.Add(rawBytes.Skip(index).Take(subshellitemdatasize).ToArray());
                    index += subshellitemdatasize;
                }
            }

            foreach (var bytes in chunks)
            {
                index = 0;

                var extsize = BitConverter.ToInt16(bytes, index);

                var signature = BitConverter.ToUInt32(bytes, 0x04);

                //TODO does this need to check if its a 0xbeef?? regex?
                var block = Utils.GetExtensionBlockFromBytes(signature, bytes);

                ExtensionBlocks.Add(block);

                var beef0004 = block as Beef0004;
                if (beef0004 != null)
                {
                    Value = beef0004.LongName;
                }

                index += extsize;
            }
        }

        public int FileSize { get; }

        /// <summary>
        ///     last modified time of BagPath
        /// </summary>
        public DateTimeOffset? LastModificationTime { get; set; }

        /// <summary>
        ///     Last access time of BagPath
        /// </summary>
        public DateTimeOffset? LastAccessTime { get; set; }

        public string ShortName { get; }


        public override string ToString()
        {
            var sb = new StringBuilder();

            if (ShortName.Length > 0)
            {
                sb.AppendLine($"Short name: {ShortName}");
            }

            if (FileSize > 0)
            {
                sb.AppendLine();

                sb.AppendLine($"File size: {FileSize:N0}");
            }

            //TODO denote custom properties vs standard ones
            if (LastModificationTime.HasValue)
            {
                sb.AppendLine(
                    $"Modified On: {LastModificationTime.Value.ToString(Utils.GetDateTimeFormatWithMilliseconds())}");
            }

            if (LastAccessTime.HasValue)
            {
                sb.AppendLine(
                    $"Accessed On: {LastAccessTime.Value.ToString(Utils.GetDateTimeFormatWithMilliseconds())}");
            }

            sb.AppendLine();

            sb.AppendLine(base.ToString());

            return sb.ToString();
        }
    }
}