namespace PDS3.Commands.NumberGenerator;

public class PseudoRandomNumberGeneratorOptions
{
    static PseudoRandomNumberGeneratorOptions()
    {
        Optimal = new PseudoRandomNumberGeneratorOptions
        {
            Mod = (ulong)Math.Pow(2, 16) - 1,
            Cummulative = 13,
            Multiplier = (ulong)Math.Pow(3, 3),
            StartValue = 128
        };
    }

    public static PseudoRandomNumberGeneratorOptions Optimal { get; }

    public ulong Mod { get; set; }

    public ulong Multiplier { get; set; }

    public ulong Cummulative { get; set; }

    public ulong StartValue { get; set; }
}