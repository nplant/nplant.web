using System;
using System.IO;
using System.Text;
using NPlant.Generation;

namespace NPlant.Web.ManifestGen
{
    class Program
    {
        private static int Main(string[] args)
        {
            try
            {
                if (args == null || args.Length < 1)
                    throw new Exception("No args were provided");

                if(args.Length != 3)
                    throw new Exception("Expected 3 arguments, but {0} were provided".FormatWith(args.Length));

                var runner = new Runner(args[0], args[1], args[2]);
                runner.Run();
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception Occurred");
                Console.WriteLine(ex.Message);
                return 1;
            }
        }

        public class Runner
        {
            public Runner(string sourceDirectory, string samplesAssembly, string outputPath)
            {
                this.SourceDirectory = sourceDirectory;
                this.SamplesAssembly = samplesAssembly;
                this.OutputPath = outputPath;
            }

            private string SamplesAssembly { get; set; }

            private string SourceDirectory { get; set; }

            private string OutputPath { get; set; }
            public void Run()
            {
                const string baseName = "NPlant.Samples";
                string baseDir = Path.Combine(this.SourceDirectory, baseName);

                var buffer = new StringBuilder();
                buffer.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
                buffer.AppendLine("<samples>");

                string samplesAssemblyPath = this.SamplesAssembly.IsNullOrEmpty() ? "NPlant.Samples.dll" : this.SamplesAssembly;

                var loader = new NPlantAssemblyLoader();
                var samplesAssembly = loader.Load(samplesAssemblyPath);
                var sampleTypes = samplesAssembly.GetExportedTypes();

                foreach (var sampleType in sampleTypes)
                {
                    if (typeof(ClassDiagram).IsAssignableFrom(sampleType))
                    {
                        var sample = sampleType.GetAttributeOf<SampleAttribute>();

                        string id = sampleType.FullName;

                        string sourcePath = "{0}\\{1}".FormatWith(baseDir, "{0}.cs".FormatWith(id.Substring(baseName.Length + 1).Replace(".", "\\")));

                        if (File.Exists(sourcePath))
                        {
                            string name = sampleType.Name;
                            string description = name;
                            int start = baseName.Length + 1;
                            string group = id.Substring(start, (id.Length - start) - (name.Length + 1));

                            if (sample != null)
                            {
                                name = sample.Name.IsNullOrEmpty() ? name : sample.Name;
                                description = sample.Description.IsNullOrEmpty() ? description : sample.Description;
                            }

                            buffer.AppendLine("  <sample id=\"{0}\" name=\"{1}\" description=\"{2}\" group=\"{3}\">".FormatWith(id, name, description, group));
                            buffer.AppendLine("  <![CDATA[{0}".FormatWith(File.ReadAllText(sourcePath, Encoding.UTF8)));
                            buffer.AppendLine("  ]]>");
                            buffer.AppendLine("  </sample>");
                        }
                        else
                        {
                            Console.WriteLine("Yikes! Source file not found ({0})...".FormatWith(sourcePath));
                        }
                    }
                }

                buffer.AppendLine("</samples>");

                File.WriteAllText(this.OutputPath, buffer.ToString());
            }
        }
    }
}
