namespace PDS3.Commands.Word;

class Word : IWord
{
    public const Int32 WordSizeInBits = BytesPerWord * BitsPerByte;
    public const Int32 BytesPerWord = sizeof(UInt16);
    private const Int32 BitsPerByte = 8;
    private const Int32 ByteMask = 0b11111111;

    public ushort WordValue { get; set; }

    public void CreateFromBytes(byte[] bytes, int startFrom)
    {
        WordValue = 0;

        for (var i = startFrom + BytesPerWord - 1; i > startFrom; --i)
        {
            WordValue = (ushort)(WordValue | bytes[i]);
            WordValue = (ushort)(WordValue << BitsPerByte);
        }

        WordValue = (ushort)(WordValue | bytes[startFrom]);
    }

    public byte[] FillBytesArray(byte[] bytesToFill, int startFrom)
    {
        var i = 0;
        for (; i < BytesPerWord - 1; ++i)
        {
            bytesToFill[startFrom + i] = (byte)(WordValue & ByteMask);
            WordValue = (ushort)(WordValue >> BitsPerByte);
        }

        bytesToFill[startFrom + i] = (byte)(WordValue & ByteMask);

        return bytesToFill;
    }

    public IWord ROL(int offset)
    {
        offset %= BytesPerWord;
        WordValue = (ushort)(WordValue << offset | WordValue >> WordSizeInBits - offset);

        return this;
    }

    public IWord ROR(int offset)
    {
        offset %= BytesPerWord;
        WordValue = (ushort)(WordValue >> offset | WordValue << WordSizeInBits - offset);

        return this;
    }

    public IWord Add(IWord word)
    {
        WordValue = (ushort)(WordValue + (word as Word).WordValue);

        return this;
    }

    public IWord Add(byte value)
    {
        WordValue = (ushort)(WordValue + value);

        return this;
    }

    public IWord Sub(IWord word)
    {
        WordValue = (ushort)(WordValue - (word as Word).WordValue);

        return this;
    }

    public IWord XorWith(IWord word)
    {
        WordValue = (ushort)(WordValue ^ (word as Word).WordValue);

        return this;
    }

    public IWord Clone()
    {
        return (IWord)MemberwiseClone();
    }

    public int ToInt32()
    {
        return WordValue;
    }
}