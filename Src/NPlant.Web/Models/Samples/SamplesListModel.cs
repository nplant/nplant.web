using System.Collections.Generic;

namespace NPlant.Web.Models.Samples
{
    public class SamplesListModel
    {
        private readonly List<SampleGroupModel> _groups = new List<SampleGroupModel>();
 
        public IEnumerable<SampleGroupModel> Groups { get { return _groups; } }

        public SampleGroupModel AddGroup(string groupName)
        {
            var group = new SampleGroupModel(groupName);
            _groups.Add(group);

            return group;
        }
    }
}