namespace PDS2.Commands
{
    using System.Diagnostics;
    using System.Text;

    public static class IOCommand
    {
        private const string FilePath = "C:\\Users\\dutch\\source\\repos\\7term\\PDS\\PDS2\\Result.txt";
        public static async Task WriteFile(List<uint> values, uint? count = null)
        {
            StringBuilder textFile = new StringBuilder();

            textFile.Append(string.Join(", ", values));
            textFile.Append("\n");

            if (count.HasValue)
            {
                string newLine = "================================================================\n";
                textFile.Append(newLine);
                textFile.Append("Count: " + count + "\n");
                textFile.Append(newLine);
            }

            try
            {
                await File.AppendAllTextAsync(FilePath, textFile.ToString());
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
