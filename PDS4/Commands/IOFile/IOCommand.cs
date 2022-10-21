namespace PDS4.Commands.IOFile
{
    using System.Diagnostics;

    public static class IOCommand
    {
        private const string FilePath = "C:\\Users\\dutch\\source\\repos\\7term\\PDS\\PDS4\\";
        public static async Task WriteFile(byte[] data, string password, string? filePath = null)
        {
            try
            {
                var path = filePath ?? FilePath + $"{password}.txt";
                await File.WriteAllBytesAsync(path, data);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception has occurred while writing to file: " + e.Message);
            }
        }

        public static async Task<byte[]?> ReadFile(string filePath)
        {
            try
            {
                var data = await File.ReadAllBytesAsync(filePath);
                return data;
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception has occurred while reading hash from file: " + e.Message);
            }

            return null;
        }

        public static bool OpenFile(string filePath)
        {
            try
            {
                var p = new Process();
                p.StartInfo = new ProcessStartInfo(filePath)
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
