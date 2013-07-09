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
        static void DisplayHelp() 
        {
            Output("Outputs an <img /> tag from an image file");
            Output("Usage: img-tag <img file>");
            Output("");
            Output("Experimental");
            Output("--src set what 'src' begins with: img-tag <img file> --src <path>");
            Output("path: Text or number for how many directories to keep acending from file");
            Output("      Number supports adding a leading slash");
            Output("      e.g img-src C:\\assets\\images\\file.jpg /1");
            Output("        > <img src=\"/images/file.jpg\" ... />");
        }

        static void Main(string[] args)
        {
            // No args
            if (args.Length == 0)
            {
                DisplayHelp();
                return;
            }

            string filePath = args[0];

            // Asking for help
            if (filePath == "--help" || filePath.Contains("?"))
            {
                DisplayHelp();
                return;
            }

            FileInfo file;
            try
            {
                file = new FileInfo(filePath);
            }
            catch (Exception ex)
            {
                // ctor can throw lots of different exceptions
                Output("Invalid path to file: {0}", filePath);
                Output("Exception: {0}", ex.Message);
                return;
            }

            // File check
            if (!file.Exists)
            {
                Output("Invalid path to file: {0}", file.FullName);
                return;
            }

            // --src option
            string srcRoot = file.DirectoryName;
            if (args.Length > 2)
            {
                // Experimental - Set what to begin the 'src' attribute with
                // e.g img-tag C:\img\logo.png --src assets/images
                // Will set src to '/assets/images/logo.png'
                string srcArg = args[2];
                srcRoot = srcArg;


                // If a number is provided, keeps that many directories accending from the file
                // e.g img-tag C:\img\logo.png --src 1
                // Will set src to 'img/logo.png'
                int dirCount = 0;
                if (Int32.TryParse(srcArg.TrimStart('/'), out dirCount))
                {
                    srcRoot = Path.Combine(file.DirectoryName.Split('\\').Reverse().Take(dirCount).Reverse().ToArray());

                    // Add forward slash before the number to add a forward slash to the start of src
                    // e.g img-tag C:\img\logo.png --src /1
                    // Will set src to '/img/logo.png'
                    if (srcArg.StartsWith("/")) srcRoot = srcRoot.Insert(0, "/");
                }
            }

            // Using WPF to get meta data
            var frame = BitmapFrame.Create(new Uri(file.FullName));
            var meta = (BitmapMetadata)frame.Metadata;

            // Attributes
            string src = Path.Combine(srcRoot, file.Name).Replace('\\', '/');
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
