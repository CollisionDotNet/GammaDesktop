using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GammaDesktop
{
    internal class StringBinaryEncoder
    {
		// Encodes text with specified random number generator and seed
        public string Encode64UTF8String(string source, ulong seed, IHashGenerator generator)
        {
            HashRandom hashRandom = new HashRandom(seed, generator);
            hashRandom.Next();
            StringBuilder encodedSource = new StringBuilder();
            int firstCharPos = 0;
			// Encoding text by dividing it into 8 char blocks
            while (firstCharPos + 7 < source.Length)
            {
                string curStr = source.Substring(firstCharPos, 8); // Get new block
                ulong blockBinaryValue = GetBinariesLongFromUTF8String(curStr); // Get its ulong binaries
                ulong gamma = hashRandom.Next(); // XOR it with next random number and concatenate result with already encoded string
                ulong xored = blockBinaryValue ^ gamma;
                encodedSource.Append(GetUTF8StringFromBinariesLong(xored));
                firstCharPos += 8;
            }
			// Encoding remains if exists
            int smallBlockSize = source.Length % 8;
            if (smallBlockSize != 0)
            {
                int startpos = source.Length - smallBlockSize;
                string smallBlockStr = source.Substring(startpos, smallBlockSize); // Get remaining block
                ulong blockBinaryValue = GetBinariesLongFromUTF8String(smallBlockStr); // Get its ulong binaries
                ulong gamma = hashRandom.Next(); // If remaining block is smaller than 8 chars - fix gamma size to fit its length
                if (smallBlockSize != 8)
                {
                    gamma %= (ulong)Math.Pow(256, smallBlockSize);
                }
                ulong xored = blockBinaryValue ^ gamma; // XOR it and concatenate result with already encoded string
                encodedSource.Append(GetUTF8StringFromBinariesLong(xored));
            }

            return encodedSource.ToString();
        }
		// Converts UTF8 string to ulong number according to chars' numeric representations 
        private ulong GetBinariesLongFromUTF8String(string str)
        {
            if (str.Length > 8)
                throw new ArgumentException("String is too large to be converted to 64-bit number!");

            ulong binaries = 0;
            ulong mult = 1;

            for (int i = str.Length - 1; i >= 0; i--) // Starting with the end of string, with low bits
            {
                binaries += str[i] * mult;
                mult <<= 8;
            }
            return binaries;
        }
		// Converts ulong number to UTF8 string according to chars' numeric representations 
        private string GetUTF8StringFromBinariesLong(ulong binaries)
        {
            StringBuilder stringBuilder = new StringBuilder();

            while (binaries > 0)
            {
                ulong code = binaries % 256;
                stringBuilder.Insert(0, (char)code);
                binaries /= 256;
            }
            return stringBuilder.ToString();
        }
    }
}
