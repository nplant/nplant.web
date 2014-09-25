using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using NPlant.Web.Models.Samples;

namespace NPlant.Web.Services
{
    public class ManifestReader
    {
        private static readonly XmlDocument Doc;
        private static readonly object DocSyncRoot = new object();

        static ManifestReader()
        {
            Doc = new XmlDocument();
            Doc.Load(Path.Combine(GetExecutionDirectory(), "Manifest.xml"));
        }

        public static string GetExecutionDirectory(string @default = ".")
        {
            var executingAssemblyPath = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;
            return Path.GetDirectoryName(executingAssemblyPath) ?? @default;
        }

        public static SamplesListModel GetSamplesListModel()
        {
            SamplesListModel model = new SamplesListModel();

            lock (DocSyncRoot)
            {
                var sampleNodes = Doc.SelectNodes("//samples/sample");

                if (sampleNodes != null)
                {
                    foreach (XmlNode sampleNode in sampleNodes)
                    {
                        string id = GetAttributeValue(sampleNode, "id");
                        string name = GetAttributeValue(sampleNode, "name");
                        string description = GetAttributeValue(sampleNode, "description");
                        string groupName = GetAttributeValue(sampleNode, "group");

                        var group = model.Groups.FirstOrDefault(x => x.Name == groupName);

                        if (group == null)
                            group = model.AddGroup(groupName);

                        group.AddSample(new SampleModel(id, name, description));
                    }
                }
            }

            return model;
        }

        private static string GetAttributeValue(XmlNode node, string name)
        {
            if (node == null || node.Attributes == null)
                return null;

            var attribute = node.Attributes[name];

            if (attribute == null)
                return null;

            return attribute.Value;
        }

        internal static string GetText(XmlNode node)
        {
            foreach (XmlNode child in node.ChildNodes)
            {
                if (child.NodeType == XmlNodeType.Text || child.NodeType == XmlNodeType.CDATA)
                {
                    return child.Value;
                }
            }

            return null;
        }

        public static string GetSource(string id)
        {
            lock (DocSyncRoot)
            {
                var sampleNode = Doc.SelectSingleNode("//samples/sample[@id='{0}']".FormatWith(id));

                if (sampleNode == null)
                    return null;

                return GetText(sampleNode);
            }
        }
    }
}