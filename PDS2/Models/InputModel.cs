namespace PDS2.Models;

public class InputModel
{
    public uint Modulus { get; set; } = 2;
    public int ModulusStep { get; set; } = 16;
    public uint Multiplier { get; set; } = 3;
    public int MultiplierStep { get; set; } = 3;
    public uint Increment { get; set; } = 13;
    public uint FirstValue { get; set; } = 128;
}