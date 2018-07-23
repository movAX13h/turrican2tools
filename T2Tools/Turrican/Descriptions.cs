using System.Runtime.Serialization;

namespace T2Tools.Turrican
{
    [DataContract]
    internal class DescriptionsList
    {
        [DataMember(Name = "descriptions")]
        private Description[] descriptions;

        public Description ByName(string name)
        {
            foreach (Description d in descriptions) if (d.Name == name.ToLower()) return d;
            return null;
        }        
    }


    [DataContract]
    internal class Description
    {
        [DataMember(Name = "name")]
        public string Name;

        [DataMember(Name = "info")]
        public string Info;

        [DataMember(Name = "nopreview")]
        public bool NoPreview = false;
    }
}
