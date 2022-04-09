using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AkkaNeo4j.Models
{
    public class GraphElementIdenty
    {
        public string Alice { get; set; }
 
        public string Name { get; set; }
    }
 
    public class GraphEvent
    {
        public string Action { get; set; } // Create , Relation, Reset
 
        public string Alice { get; set; } // AliceName
 
        public string Name { get; set; }
 
        public GraphElementIdenty From { get; set; }
 
        public GraphElementIdenty To { get; set; }
    }
}
