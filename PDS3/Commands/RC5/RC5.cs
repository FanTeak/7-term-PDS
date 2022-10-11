using PDS3.Commands.NumberGenerator;
using PDS3.Commands.Word;

namespace PDS3.Commands.RC5
{
    public class RC5
    {
        private readonly PseudoRandomNumberGenerator _numberGenerator;
        private readonly WordFactory _wordsFactory;
        private readonly int _roundsCount;
        public const int BitsPerByte = 8;

        public RC5()
        {
            _numberGenerator = new PseudoRandomNumberGenerator(PseudoRandomNumberGeneratorOptions.Optimal);
            _wordsFactory = new WordFactory();
            _roundsCount = 16;
        }

        public byte[] Encrypt(byte[] input, byte[] key)
        {
            var paddedBytes = ConcatArrays(input, GetPadding(input));
            var bytesPerBlock = _wordsFactory.BytesPerBlock;
            var s = BuildExpandedKeyTable(key);
            var cnPrev = GetRandomBytesForInitVector().Take(bytesPerBlock).ToArray();
            var encodedFileContent = new byte[cnPrev.Length + paddedBytes.Length];

            EncipherECB(cnPrev, encodedFileContent, inStart: 0, outStart: 0, s);

            for (int i = 0; i < paddedBytes.Length; i += bytesPerBlock)
            {
                var cn = new byte[bytesPerBlock];
                Array.Copy(paddedBytes, i, cn, 0, cn.Length);

                XorWith(array: cn, xorArray: cnPrev, inStartIndex: 0, xorStartIndex: 0, length: cn.Length);

                EncipherECB(
                    inBytes: cn,
                    outBytes: encodedFileContent,
                    inStart: 0,
                    outStart: i + bytesPerBlock,
                    s: s);

                Array.Copy(encodedFileContent, i + bytesPerBlock, cnPrev, 0, cn.Length);
            }

            return encodedFileContent;
        }

        public byte[] Decrypt(byte[] input, byte[] key)
        {
            var bytesPerBlock = _wordsFactory.BytesPerBlock;
            var s = BuildExpandedKeyTable(key);
            var cnPrev = new byte[bytesPerBlock];
            var decodedFileContent = new byte[input.Length - cnPrev.Length];

            DecipherECB(
                inBuf: input,
                outBuf: cnPrev,
                inStart: 0,
                outStart: 0,
                s: s);

            for (int i = bytesPerBlock; i < input.Length; i += bytesPerBlock)
            {
                var cn = new byte[bytesPerBlock];
                Array.Copy(input, i, cn, 0, cn.Length);

                DecipherECB(
                    inBuf: cn,
                    outBuf: decodedFileContent,
                    inStart: 0,
                    outStart: i - bytesPerBlock,
                    s: s);

                XorWith(array: decodedFileContent, xorArray: cnPrev, inStartIndex: i - bytesPerBlock, xorStartIndex: 0, length: cn.Length);

                Array.Copy(input, i, cnPrev, 0, cnPrev.Length);
            }

            var decodedWithoutPadding = new byte[decodedFileContent.Length - decodedFileContent.Last()];
            Array.Copy(decodedFileContent, decodedWithoutPadding, decodedWithoutPadding.Length);

            return decodedWithoutPadding;
        }

        private void EncipherECB(byte[] inBytes, byte[] outBytes, int inStart, int outStart, IWord[] s)
        {
            var a = _wordsFactory.CreateFromBytes(inBytes, inStart);
            var b = _wordsFactory.CreateFromBytes(inBytes, inStart + _wordsFactory.BytesPerWord);

            a.Add(s[0]);
            b.Add(s[1]);

            for (var i = 1; i < _roundsCount + 1; ++i)
            {
                a.XorWith(b).ROL(b.ToInt32()).Add(s[2 * i]);
                b.XorWith(a).ROL(a.ToInt32()).Add(s[2 * i + 1]);
            }

            a.FillBytesArray(outBytes, outStart);
            b.FillBytesArray(outBytes, outStart + _wordsFactory.BytesPerWord);
        }

        private void DecipherECB(byte[] inBuf, byte[] outBuf, int inStart, int outStart, IWord[] s)
        {
            var a = _wordsFactory.CreateFromBytes(inBuf, inStart);
            var b = _wordsFactory.CreateFromBytes(inBuf, inStart + _wordsFactory.BytesPerWord);

            for (var i = _roundsCount; i > 0; --i)
            {
                b = b.Sub(s[2 * i + 1]).ROR(a.ToInt32()).XorWith(a);
                a = a.Sub(s[2 * i]).ROR(b.ToInt32()).XorWith(b);
            }

            a.Sub(s[0]);
            b.Sub(s[1]);

            a.FillBytesArray(outBuf, outStart);
            b.FillBytesArray(outBuf, outStart + _wordsFactory.BytesPerWord);
        }

        private byte[] GetPadding(byte[] inBytes)
        {
            var paddingLength = _wordsFactory.BytesPerBlock - inBytes.Length % _wordsFactory.BytesPerBlock;

            var padding = new byte[paddingLength];

            for (int i = 0; i < padding.Length; ++i)
            {
                padding[i] = (byte)paddingLength;
            }

            return padding;
        }

        private byte[] GetRandomBytesForInitVector()
        {
            var ivParts = new List<byte[]>();

            while (ivParts.Sum(ivp => ivp.Length) < _wordsFactory.BytesPerBlock)
            {
                ivParts.Add(BitConverter.GetBytes(_numberGenerator.NextNumber()));
            }

            return ConcatArrays(ivParts.ToArray());
        }

        private IWord[] BuildExpandedKeyTable(byte[] key)
        {
            var keysWordArrLength = key.Length % _wordsFactory.BytesPerWord > 0
                ? key.Length / _wordsFactory.BytesPerWord + 1
                : key.Length / _wordsFactory.BytesPerWord;

            var lArr = new IWord[keysWordArrLength];

            for (int i = 0; i < lArr.Length; i++)
            {
                lArr[i] = _wordsFactory.Create();
            }

            for (var i = key.Length - 1; i >= 0; i--)
            {
                lArr[i / _wordsFactory.BytesPerWord].ROL(BitsPerByte).Add(key[i]);
            }

            var sArray = new IWord[2 * (_roundsCount + 1)];
            sArray[0] = _wordsFactory.CreateP();
            var q = _wordsFactory.CreateQ();

            for (var i = 1; i < sArray.Length; i++)
            {
                sArray[i] = sArray[i - 1].Clone();
                sArray[i].Add(q);
            }

            var x = _wordsFactory.Create();
            var y = _wordsFactory.Create();

            var n = 3 * Math.Max(sArray.Length, lArr.Length);

            for (int k = 0, i = 0, j = 0; k < n; ++k)
            {
                sArray[i].Add(x).Add(y).ROL(3);
                x = sArray[i].Clone();

                lArr[j].Add(x).Add(y).ROL(x.ToInt32() + y.ToInt32());
                y = lArr[j].Clone();

                i = (i + 1) % sArray.Length;
                j = (j + 1) % lArr.Length;
            }

            return sArray;
        }

        static void XorWith(byte[] array, byte[] xorArray, int inStartIndex, int xorStartIndex, int length)
        {
            for (int i = 0; i < length; ++i)
            {
                array[i + inStartIndex] ^= xorArray[i + xorStartIndex];
            }
        }

        public static T[] ConcatArrays<T>(params T[][] arrays)
        {
            var position = 0;
            var outputArray = new T[arrays.Sum(a => a.Length)];

            foreach (var a in arrays)
            {
                Array.Copy(a, 0, outputArray, position, a.Length);
                position += a.Length;
            }

            return outputArray;
        }

        public enum KeyLengthInBytesEnum
        {
            Bytes_8 = 8,
            Bytes_16 = 16,
            Bytes_32 = 32
        }
    }
}
