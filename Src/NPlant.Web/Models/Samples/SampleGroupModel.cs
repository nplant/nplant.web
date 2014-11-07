using System;
using System.Collections.Generic;

namespace NPlant.Web.Models.Samples
{
    [Serializable]
    public class SampleGroupModel
    {
        private readonly List<SampleModel> _samples = new List<SampleModel>();
 
        public SampleGroupModel(string name)
        {
            this.Id = Guid.NewGuid().ToString("n");
            this.Name = name;
        }

        public string Id { get; private set; }
        public string Name { get; private set; }
        public IEnumerable<SampleModel> Samples { get { return _samples; } }

        public void AddSample(SampleModel sample)
        {
            _samples.Add(sample);
        }
    }
}