using PDS2.Models;

namespace PDS2.Commands;

public static class FunctionCommand
{
    private static HashSet<uint> ExistingValues { get; set; } = new();
    private static List<uint> ListOfValues { get; set; } = new();
    public static async Task Calculate(InputModel model)
    {
        ExistingValues.Add(model.FirstValue);
        ListOfValues.Add(model.FirstValue);
        
        uint m = IntPow(model.Modulus, model.ModulusStep) - 1;
        uint a = IntPow(model.Multiplier, model.MultiplierStep);
        
        uint tempValue = (a * model.FirstValue + model.Increment) % m;
        uint period = 0;

        while (!ExistingValues.Contains(tempValue))
        {
            if (ListOfValues.Count >= 100_000)
            {
                period += 100_000;
                await IOCommand.WriteFile(ListOfValues);
                ListOfValues.Clear();
            }

            if (ExistingValues.Count <= 10_000)
            {
                ExistingValues.Add(tempValue);
            }

            ListOfValues.Add(tempValue);

            tempValue = (a * tempValue + model.Increment) % m;
        }

        period += (uint) ListOfValues.Count;
        await IOCommand.WriteFile(ListOfValues, period);
        ListOfValues.Clear();
        ExistingValues.Clear();
    }
    private static uint IntPow(uint x, int pow)
    {
        uint ret = 1;
        while (pow != 0)
        {
            if ((pow & 1) == 1)
                ret *= x;
            x *= x;
            pow >>= 1;
        }
        return ret;
    }
}