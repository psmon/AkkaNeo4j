using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AkkaNeo4j.Models
{
    public class GraphElementIdenty
    {
        public string UId { get; set; }
 
        public string Name { get; set; }
    }
 
    public class GraphEvent
    {
        //For Create or Delete
        public string Action { get; set; } // Create , Relation, Reset
 
        public string Uid { get; set; } // AliceName
 
        public string Name { get; set; }
 
        //For Relation
        public GraphElementIdenty From { get; set; }
 
        public GraphElementIdenty To { get; set; }
    }
}
