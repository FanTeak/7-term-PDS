namespace PDS1.Commands
{
    using System.Diagnostics;
    using System.Text;

    public static class IOCommand
    {
        private const string FilePath = "C:\\Users\\dutch\\source\\repos\\7-term\\PDS\\PDS1\\Result.txt";
        public static async Task WriteFile()
        {
            StringBuilder textFile = new StringBuilder();
            string newLine = "================================================================\n";

            textFile.Append(newLine);

            try
            {
                await File.WriteAllTextAsync(FilePath, textFile.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception has occurred while writing to file: " + e.Message);
            }
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
