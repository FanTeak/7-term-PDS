namespace PDS3.Models;

public class FunctionModel
{
    public FunctionModel(string password, string filePath)
    {
        Password = password;
        FilePath = filePath;
    }

    public FunctionModel()
    {
        
    }

    public string Password { get; set; } = "Qwerty123";
    public string FilePath { get; set; } = "C:\\Users\\dutch\\source\\repos\\7term\\PDS\\PDS3\\test.txt";
}