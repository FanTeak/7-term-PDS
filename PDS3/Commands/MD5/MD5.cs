using System.Security.Cryptography;
using System.Text;

namespace PDS3.Commands.MD5;

public class MD5
{
    private const int OptimalChunkSizeMultiplier = 100_000;
    private const UInt32 OptimalChunkSize = MD5C.BytesCountPerBits512Block * OptimalChunkSizeMultiplier;

    /// <summary>
    /// Computed hash as <see cref="string"/>
    /// </summary>
    public string HashAsString => Hash.ToString();

    /// <summary>
    /// Computed hash as <see cref="MessageDigest"/> model
    /// </summary>
    public MessageDigest Hash { get; private set; }

    /// <summary>
    /// Computes MD5 hash of input message
    /// </summary>
    /// <param name="message">Input message</param>
    /// <returns>Returns hash as <see cref="MessageDigest"/> model</returns>
    public void ComputeHash(string message)
    {
        ComputeHash(Encoding.ASCII.GetBytes(message));
    }

    /// <summary>
    /// Computes MD5 hash of input message
    /// </summary>
    /// <param name="message">Input message, byte array</param>
    /// <returns>Returns hash as <see cref="MessageDigest"/> model</returns>
    public MessageDigest ComputeHash(Byte[] message)
    {
        Hash = MessageDigest.InitialValue;

        var paddedMessage = JoinArrays(message, GetMessagePadding((UInt32)message.Length));

        for (UInt32 bNo = 0; bNo < paddedMessage.Length / MD5C.BytesCountPerBits512Block; ++bNo)
        {
            UInt32[] X = Extract32BitWords(
                paddedMessage,
                bNo,
                MD5C.Words32BitArraySize * MD5C.BytesPer32BitWord);

            FeedMessageBlockToBeHashed(X);
        }

        return Hash;
    }

    /// <summary>
    /// Computes MD5 hash of input file
    /// </summary>
    /// <param name="filePath">Path to file to be hashed</param>
    /// <param name="chunkSizeMultiplier"></param>
    /// <returns></returns>
    public async Task<MessageDigest> ComputeFileHashAsync(String filePath)
    {
        Hash = MessageDigest.InitialValue;

        using (var fs = File.OpenRead(filePath))
        {
            UInt64 totalBytesRead = 0;
            Int32 currentBytesRead = 0;
            bool isFileEnd = false;

            do
            {
                var chunk = new Byte[OptimalChunkSize];

                currentBytesRead = await fs.ReadAsync(chunk, 0, chunk.Length);
                totalBytesRead += (UInt64)currentBytesRead;


                if (currentBytesRead < chunk.Length)
                {
                    Byte[] lastChunk;

                    if (currentBytesRead == 0)
                    {
                        lastChunk = GetMessagePadding(totalBytesRead);
                    }
                    else
                    {
                        lastChunk = new Byte[currentBytesRead];
                        Array.Copy(chunk, lastChunk, currentBytesRead);

                        lastChunk = JoinArrays(lastChunk, GetMessagePadding(totalBytesRead));
                    }

                    chunk = lastChunk;
                    isFileEnd = true;
                }

                for (UInt32 bNo = 0; bNo < chunk.Length / MD5C.BytesCountPerBits512Block; ++bNo)
                {
                    UInt32[] X = Extract32BitWords(
                        chunk,
                        bNo,
                        MD5C.Words32BitArraySize * MD5C.BytesPer32BitWord);

                    FeedMessageBlockToBeHashed(X);
                }
            }
            while (isFileEnd == false);
        }

        return Hash;
    }

    private void FeedMessageBlockToBeHashed(UInt32[] X)
    {
        UInt32 F, i, k;
        var blockSize = MD5C.BytesCountPerBits512Block;
        var MDq = Hash.Clone();

        // first round
        for (i = 0; i < blockSize / 4; ++i)
        {
            k = i;
            F = FuncF(MDq.B, MDq.C, MDq.D);

            MDq.MD5IterationSwap(F, X, i, k);
        }
        // second round
        for (; i < blockSize / 2; ++i)
        {
            k = (1 + (5 * i)) % (blockSize / 4);
            F = FuncG(MDq.B, MDq.C, MDq.D);

            MDq.MD5IterationSwap(F, X, i, k);
        }
        // third round
        for (; i < blockSize / 4 * 3; ++i)
        {
            k = (5 + (3 * i)) % (blockSize / 4);
            F = FuncH(MDq.B, MDq.C, MDq.D);

            MDq.MD5IterationSwap(F, X, i, k);
        }
        // fourth round
        for (; i < blockSize; ++i)
        {
            k = 7 * i % (blockSize / 4);
            F = FuncI(MDq.B, MDq.C, MDq.D);

            MDq.MD5IterationSwap(F, X, i, k);
        }

        Hash += MDq;
    }

    private static Byte[] GetMessagePadding(UInt64 messageLength)
    {
        UInt32 paddingLengthInBytes = default;
        var mod = (UInt32)(messageLength * MD5C.BitsPerByte % MD5C.Bits512BlockSize);

        // Append Padding Bits
        if (mod == MD5C.BITS_448)
        {
            paddingLengthInBytes = MD5C.Bits512BlockSize / MD5C.BitsPerByte;
        }
        else if (mod > MD5C.BITS_448)
        {
            paddingLengthInBytes = (MD5C.Bits512BlockSize - mod + MD5C.BITS_448) / MD5C.BitsPerByte;
        }
        else if (mod < MD5C.BITS_448)
        {
            paddingLengthInBytes = (MD5C.BITS_448 - mod) / MD5C.BitsPerByte;
        }

        var padding = new Byte[paddingLengthInBytes + MD5C.BitsPerByte];
        padding[0] = MD5C.BITS_128;

        // Append Length
        var messageLength64bit = messageLength * MD5C.BitsPerByte;

        for (var i = 0; i < MD5C.BitsPerByte; ++i)
        {
            padding[paddingLengthInBytes + i] = (Byte)(messageLength64bit
                >> (Int32)(i * MD5C.BitsPerByte)
                & MD5C.BITS_255);
        }

        return padding;
    }

    private static T[] JoinArrays<T>(T[] first, T[] second)
    {
        T[] joinedArray = new T[first.Length + second.Length];
        Array.Copy(first, joinedArray, first.Length);
        Array.Copy(second, 0, joinedArray, first.Length, second.Length);

        return joinedArray;
    }

    public static UInt32[] Extract32BitWords(Byte[] message, UInt32 blockNo, UInt32 blockSizeInBytes)
    {
        var messageStartIndex = blockNo * blockSizeInBytes;
        var extractedArray = new UInt32[blockSizeInBytes / MD5C.BytesPer32BitWord];

        for (UInt32 i = 0; i < blockSizeInBytes; i += MD5C.BytesPer32BitWord)
        {
            var j = messageStartIndex + i;

            extractedArray[i / MD5C.BytesPer32BitWord] = // form 32-bit word from four bytes
                  message[j]                                                   // first byte
                | (((UInt32)message[j + 1]) << ((Int32)MD5C.BitsPerByte * 1))  // second byte
                | (((UInt32)message[j + 2]) << ((Int32)MD5C.BitsPerByte * 2))  // third byte
                | (((UInt32)message[j + 3]) << ((Int32)MD5C.BitsPerByte * 3)); // fourth byte
        }

        return extractedArray;
    }

    /// <summary>
    /// Elementary function F(B, C, D)
    /// </summary>
    public static UInt32 FuncF(UInt32 B, UInt32 C, UInt32 D) => (B & C) | (~B & D);

    /// <summary>
    /// Elementary function G(B, C, D)
    /// </summary>
    public static UInt32 FuncG(UInt32 B, UInt32 C, UInt32 D) => (D & B) | (C & ~D);

    /// <summary>
    /// Elementary function H(B, C, D)
    /// </summary>
    public static UInt32 FuncH(UInt32 B, UInt32 C, UInt32 D) => B ^ C ^ D;

    /// <summary>
    /// Elementary function I(B, C, D)
    /// </summary>
    public static UInt32 FuncI(UInt32 B, UInt32 C, UInt32 D) => C ^ (B | ~D);

    public static Byte[] GetMD5HashedKeyForRC5(Byte[] key, RC5.RC5.KeyLengthInBytesEnum keyLengthInBytes)
    {
        if (key is null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        var hasher = new MD5();
        var bytesHash = hasher.ComputeHash(key).ToByteArray();

        if (keyLengthInBytes == RC5.RC5.KeyLengthInBytesEnum.Bytes_8)
        {
            bytesHash = bytesHash.Take(bytesHash.Length / 2).ToArray();
        }
        else if (keyLengthInBytes == RC5.RC5.KeyLengthInBytesEnum.Bytes_32)
        {
            bytesHash = bytesHash
                .Concat(hasher.ComputeHash(bytesHash).ToByteArray())
                .ToArray();
        }

        if (bytesHash.Length != (int)keyLengthInBytes)
        {
            throw new InvalidOperationException(
                $"Internal error at {nameof(GetMD5HashedKeyForRC5)} method, " +
                $"hash result is not equal to {(int)keyLengthInBytes}.");
        }

        return bytesHash;
    }
}