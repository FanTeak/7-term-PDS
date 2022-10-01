using PDS2.Models;

namespace PDS2.Commands;

public static class FunctionCommand
{
    private static HashSet<uint> ExistingValues { get; set; } = new();
    private static List<uint> ListOfValues { get; set; } = new();
    public static async Task<FunctionModel> Calculate(string toHash)
    {
        return new FunctionModel();
    }
}