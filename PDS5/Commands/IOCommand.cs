namespace PDS5.Commands
{
    public static class IOCommand
    {
        public static string FilePath;

        public static async Task WriteFile(string filePath, string signature)
        {
            try
            {
                await File.WriteAllTextAsync(filePath, signature);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception has occurred while writing to file: " + e.Message);
            }
        }

        public static async Task<byte[]?> ReadFileByte(string filePath)
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

        public static async Task<string?> ReadFileString(string filePath)
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
    }
}
