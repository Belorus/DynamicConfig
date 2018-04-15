using System.Collections.Generic;

namespace GeCon.Models
{
    public class PartsResponse
    {
        public PartSection[] sections;

    }

    public class PartSection
    {
        public string id;
        public string display_name;
        public Part[] parts;
    }

    public class Part
    {
        public string id;
        public string display_name;
    }
}
