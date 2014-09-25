using System.IO;
using System.IO.Compression;
using System.Text;

namespace NPlant.Web.Services
{
    public class PlantUmlUrl
    {
        public PlantUmlUrl(string diagramText)
        {
            this.DiagramText = diagramText;
        }

        public string DiagramText { get; private set; }

        public string GetUrl()
        {
            byte[] compressed = Compress(this.DiagramText);
            return "http://www.plantuml.com/plantuml/png/{0}".FormatWith(Encode64(compressed));
        }

        private string Encode64(byte[] bytes)
        {
            StringBuilder buffer = new StringBuilder();

            for (var i=0; i<bytes.Length; i+=3) 
            {
                if (i+2==bytes.Length)
                {
                    buffer.Append(Append3Bytes(bytes[i], bytes[i + 1], 0));
                } 
                else if (i+1==bytes.Length)
                {
                    buffer.Append(Append3Bytes(bytes[i], 0, 0));
                } else
                {
                    buffer.Append(Append3Bytes(bytes[i], bytes[i + 1], bytes[i + 2]));
                }
            }

            return buffer.ToString();
        }

        private string Append3Bytes(int b1, int b2, int b3)
        {
            const int sixtyThree = 0x3F;

            var c1 = b1 >> 2;
            var c2 = ((b1 & 0x3) << 4) | (b2 >> 4);
            var c3 = ((b2 & 0xF) << 2) | (b3 >> 6);
            var c4 = b3 & sixtyThree;
	        
            return string.Concat(Encode6bit(c1 & sixtyThree), Encode6bit(c2 & sixtyThree), Encode6bit(c3 & sixtyThree), Encode6bit(c4 & sixtyThree));
        }

        
        private string Encode6bit(int b) 
        {
            if (b < 10)
                return char.ConvertFromUtf32(48 + b);

            b -= 10;
	        
            if (b < 26)
                return char.ConvertFromUtf32(65 + b);

            b -= 26;

            if (b < 26)
                return char.ConvertFromUtf32(97 + b);
            b -= 26;

            if (b == 0)
                return "-";

            if (b == 1)
                return "_";

            return "?";
        }

        private static byte[] Compress(string text)
        {
            using (MemoryStream output = new MemoryStream())
            {
                using (DeflateStream gzip = new DeflateStream(output, CompressionMode.Compress))
                {
                    using (StreamWriter writer = new StreamWriter(gzip, System.Text.Encoding.UTF8))
                    {
                        writer.Write(text);
                    }
                }

                return output.ToArray();
            }
        }
    }
}