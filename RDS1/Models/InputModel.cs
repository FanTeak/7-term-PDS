namespace RDS1.Models;

public class InputModel
{
    public int Modulus { get; set; } = 2;
    public int ModulusStep { get; set; } = 16;
    public int Multiplier { get; set; } = 3;
    public int MultiplierStep { get; set; } = 3;
    public int Increment { get; set; } = 13;
    public int FirstValue { get; set; } = 128;
}