using PDS3.Commands.Word;

namespace PDS3.Commands.RC5;

class WordFactory
{
    public Int32 BytesPerWord => Word.Word.BytesPerWord;
    public const UInt16 P16 = 0xB7E1;
    public const UInt16 Q16 = 0x9E37;

    public Int32 BytesPerBlock => BytesPerWord * 2;

    public IWord Create()
    {
        return CreateConcrete();
    }

    public IWord CreateP()
    {
        return CreateConcrete(P16);
    }

    public IWord CreateQ()
    {
        return CreateConcrete(Q16);
    }

    public IWord CreateFromBytes(Byte[] bytes, Int32 startFromIndex)
    {
        var word = Create();
        word.CreateFromBytes(bytes, startFromIndex);

        return word;
    }

    private Word.Word CreateConcrete(UInt16 value = 0)
    {
        return new Word.Word
        {
            WordValue = value
        };
    }
}