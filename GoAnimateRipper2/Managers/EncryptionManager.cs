using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoAnimateRipper2
{
    internal class EncryptionManager
    {
        static readonly String[] ENCRYPTION_KEYS = 
        { 
            "sorrypleasetryagainlater",
            "g0o1a2n3i4m5a6t7e",
            "(auto)"
        };
       public EncryptionManager() 
        { 
            // Anything to put here?
            // I don't think you need to pass anything to this class tbh ...
        }

        public byte[] Decrypt(byte[] pwd, byte[] data)
        {
            int a, i, j, k, tmp;
            int[] key, box;
            byte[] cipher;

            key = new int[256];
            box = new int[256];
            cipher = new byte[data.Length];

            for (i = 0; i < 256; i++)
            {
                key[i] = pwd[i % pwd.Length];
                box[i] = i;
            }
            for (j = i = 0; i < 256; i++)
            {
                j = (j + box[i] + key[i]) % 256;
                tmp = box[i];
                box[i] = box[j];
                box[j] = tmp;
            }
            for (a = j = i = 0; i < data.Length; i++)
            {
                a++;
                a %= 256;
                j += box[a];
                j %= 256;
                tmp = box[a];
                box[a] = box[j];
                box[j] = tmp;
                k = box[((box[a] + box[j]) % 256)];
                cipher[i] = (byte)(data[i] ^ k);
            }
            return cipher;
        }

        //Lifted more or less from GoAnimate itself
        /// <summary>
        /// bool <c>IsFlashPrefix</c> returns if byte[] <c>data</c> has a valid SWF header.
        /// </summary>
        public bool IsFlashPrefix(byte[] data)
        {
            string prefix = Encoding.UTF8.GetString(data).Substring(0, 3);
            return prefix == "CWS" || prefix == "FWS";
        }

        /// <summary>
        /// bool <c>DetermineKey</c> tries to determine the key. It returns true if it succeeds.
        /// </summary>
        public byte[] DetermineKey(byte[] data)
        {
            if (!IsFlashPrefix(data))
            {
                foreach (string tkey in ENCRYPTION_KEYS)
                {
                    byte[] key = Encoding.ASCII.GetBytes(tkey);
                    if (IsFlashPrefix(Decrypt(key, data)))
                    {
                        return key;
                    }
                }
            }
            return null;
        }
    }
}
