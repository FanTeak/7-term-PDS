namespace PDS2.Commands
{
    using System.Diagnostics;
    using System.Text;

    public static class IOCommand
    {
        private const string FilePath = "C:\\Users\\dutch\\source\\repos\\7term\\PDS\\PDS2\\Result.txt";
        public static async Task WriteFile(string hash)
        {
            try
            {
                await File.WriteAllTextAsync(FilePath, hash);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception has occurred while writing to file: " + e.Message);
            }
        }

        public static async Task<string?> ReadFile(string filePath)
        {
            try
            {
                var data = await File.ReadAllTextAsync(filePath);
                return data;
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception has occurred while reading hash from file: " + e.Message);
            }

            return null;
        }

        public static bool OpenFile()
        {
            try
            {
                var p = new Process();
                p.StartInfo = new ProcessStartInfo(FilePath)
                {
                    UseShellExecute = true
                };
                p.Start();

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception has occurred while opening to file: " + e.Message);
            }

            return false;
        }
    }
}
