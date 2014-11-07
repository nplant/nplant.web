using System;

namespace NPlant.Web.Models.Samples
{
    [Serializable]
    public class SampleModel
    {
        public SampleModel(string id, string name, string description)
        {
            Id = id;
            Name = name;
            Description = description;
        }

        public string Id { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
    }
}