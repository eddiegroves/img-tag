using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

namespace ImageTagFromFile
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Output("Outputs an <img /> tag from an image file");
                Output("Usage: img-tag <img file>");
                return;
            }

            string filePath = args[0];
            FileInfo file = new FileInfo(filePath);

            if (!file.Exists)
            {
                Output("Invalid path to file: {0}", file.FullName);
                return;
            }

            var frame = BitmapFrame.Create(new Uri(file.FullName));
            var meta = (BitmapMetadata)frame.Metadata;

            // Attributes
            string src = filePath.Replace('\\', '/');
            string width = frame.PixelWidth.ToString();
            string height = frame.PixelHeight.ToString();
            string alt = String.Empty;
            try { alt = meta.Title ?? String.Empty; }
            catch (NotSupportedException) { alt = String.Empty; }

            var imgTag = new XElement("img",
                new XAttribute("src", src),
                new XAttribute("width", width),
                new XAttribute("height", height),
                new XAttribute("alt", alt));

            Output(imgTag.ToString());
        }

        static void Output(string text, params object[] args)
        {
            if (args != null) Console.WriteLine(text, args);
            else Console.WriteLine(text);
        }
    }
}
