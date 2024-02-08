using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GammaDesktop
{
    public interface IHashGenerator
    {
        public ulong GetHash(ulong key);
    }
	// Hashing with linear congruential method
    public class LCGHash : IHashGenerator
    {
        // Constants are used for hashing mod 2^48, but we'll take 32 most significant bits
        // Operation is applied twice, we generate not a single 64-bit number, but two 32-bit numbers for further concatenation.
        private ulong a = 0x5DEECE66D;
        private ulong c = 11;
        private ulong m = 0x1000000000000;
        public ulong GetHash(ulong input) // 64 bits as input
        {
            input = (a * input + c) % m;
            ulong high = (input >> 16);
            input = (a * input + c) % m;
            ulong low = (input >> 16);
            return (high << 32) + low;
        }
    }
	// Hashing ulong -> ulong with XOR and shifts (Xorshift: https://www.jstatsoft.org/article/view/v008i14)
    public class XORHash : IHashGenerator
    {
        public ulong GetHash(ulong input)
        {
            // For input = 0 return 2^63 as (0 XOR anything) = 0 and 2^63 is in the middle of ulong values range [0..2^64 - 1]
            if (input == 0)
                input = 0x8000000000000000;
            input ^= input << 13;
            input ^= input >> 7;
            input ^= input << 17;
            return input;
        }
    }
	// Class to represent random number generator with specified start seed and hash type
    public class HashRandom
    {
        private ulong seed;
        private uint iter;
        private ulong curval;
        private IHashGenerator hashGenerator;
        public HashRandom(ulong seed, IHashGenerator hashGenerator)
        {
            this.seed = seed;
            iter = 0;
            curval = seed;
            this.hashGenerator = hashGenerator;
        }
        public ulong Next()
        {
            curval = hashGenerator.GetHash(curval);
            iter++;
            return curval;
        }
    }
}
