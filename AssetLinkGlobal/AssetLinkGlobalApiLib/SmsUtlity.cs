using System;
using System.Collections.Generic;
using System.Text;

namespace AssetLinkGlobalApiLib
{
    public static class SmsUtlity
    {
        static int Hex2Value(char hex)
        {
            int result;
            if ('0' <= hex && '9' >= hex)
            {
                result = hex - '0';
            }
            else if ('a' <= hex && 'f' >= hex)
            {
                result = hex - 'a' + 10;
            }
            else if ('A' <= hex && 'F' >= hex)
            {
                result = hex - 'A' + 10;
            }
            else
            {
                throw new ArgumentException("Not a hex digit");
            }
            return result;
        }

        public static List<string> SmsToAssetLinkApiString(string sms)
        {
            if (string.IsNullOrWhiteSpace(sms)) throw new ArgumentNullException("SMS message cannot be null, empty, or all whitespace.");
            if ('>' != sms[0]) throw new ArgumentException("SMS message must start with the '>' character.");
            if (0 == sms.Length % 2) throw new ArgumentException($"SMS message length must be an odd number.  Length:  {sms.Length}.");

            // Convert from hex
            byte[] bytes = new byte[sms.Length / 2];
            for (int i = 1; i < sms.Length; i += 2)
            {
                int high = Hex2Value(sms[i]);
                int low = Hex2Value(sms[i + 1]);
                bytes[i / 2] = (byte)(high * 16 + low);
            }

            // 0x00 after every 4 0xFFs
            byte[] newBytes = null;
            int count = 0;
            for (int i = 0; i < bytes.Length; i++)
            {
                if (0xFF == bytes[i])
                {
                    count += 1;
                    if (4 == count)
                    {
                        newBytes = new byte[bytes.Length + 1];
                        Array.Copy(bytes, newBytes, i + 1);
                        newBytes[i + 1] = 0;
                        Array.Copy(bytes, i + 1, newBytes, i + 2, bytes.Length - i - 1);
                        bytes = newBytes;
                    }
                }
                else
                {
                    count = 0;
                }
            }

            // Append 0xAA and prepend the length
            if (bytes.Length + 1 >= 0xFF) throw new InvalidOperationException($"Byte array length (plus 1) is too long.  Length:  {bytes.Length + 1}  Max Limit {0xFF}");
            newBytes = new byte[bytes.Length + 2];
            newBytes[0] = (byte)(bytes.Length + 1);
            bytes.CopyTo(newBytes, 1);
            newBytes[bytes.Length + 1] = 0xAA;
            bytes = newBytes;

            // Split into 64 byte max chunks
            List<byte[]> parts = new List<byte[]>();
            while (bytes.Length > 64)
            {
                int length = 64;
                while (0xFF == bytes[length - 1])
                {
                    length--;
                }
                newBytes = new byte[length];
                Array.Copy(bytes, newBytes, length);
                parts.Add(newBytes);
                newBytes = new byte[bytes.Length - length];
                Array.Copy(bytes, length, newBytes, 0, bytes.Length - length);
                bytes = newBytes;
            }
            parts.Add(bytes);

            // Put the flash append command at the beginning
            List<string> result = new List<string>();
            foreach (byte[] part in parts)
            {
                StringBuilder data = new StringBuilder();
                data.Append((char)6);
                data.Append("cn 8/");
                foreach (byte b in part)
                {
                    data.Append((char)b);
                }
                result.Add(data.ToString());
            }
            return result;
        }


    }
}
