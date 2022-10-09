namespace PDS3.Commands.NumberGenerator;

public class PseudoRandomNumberGenerator
{
    private ulong _currentNumber;
    private readonly ulong _mod;
    private readonly ulong _multiplier;
    private readonly ulong _cummulative;
    private readonly ulong _startValue;

    public PseudoRandomNumberGenerator(PseudoRandomNumberGeneratorOptions options)
    {
        _mod = options.Mod;
        _multiplier = options.Multiplier;
        _cummulative = options.Cummulative;
        _startValue = options.StartValue;
        _currentNumber = options.StartValue;
    }

    public ulong NextNumber()
    {
        var nextNumber = _currentNumber;

        _currentNumber = SequenceFormula(
            _multiplier,
            _cummulative,
            _mod,
            nextNumber);

        return nextNumber;
    }

    public ulong NextNumber(ulong currentNumber)
    {
        return SequenceFormula(
            _multiplier,
            _cummulative,
            _mod,
            currentNumber);
    }

    public void Reset()
    {
        _currentNumber = _startValue;
    }

    private static ulong SequenceFormula(
        ulong mult,
        ulong c,
        ulong mod,
        ulong x)
    {
        return (mult * x + c) % mod;
    }
}