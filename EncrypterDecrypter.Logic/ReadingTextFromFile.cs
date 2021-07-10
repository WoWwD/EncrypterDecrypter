using System.IO;

namespace EncoderDecoder.Logic
{
    public class ReadingTextFromFile
    {
        private string Path { get; }
        public ReadingTextFromFile(string Path)
        {
            this.Path = Path;
        }
        public string ReadingText()
        {
            string Text;
            try
            {
                using (StreamReader sw = new StreamReader(Path))
                {
                    Text = sw.ReadToEnd();
                }
                return Text;
            }
            catch
            {
                return null;
            }
        }
    }
}
