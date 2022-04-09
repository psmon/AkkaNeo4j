using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Akka.Actor;
using Akka.Event;

using AkkaNeo4j.Models;
using AkkaNeo4j.Module;

namespace AkkaNeo4j.Actors
{
    public class GraphEventActor : ReceiveActor
    {
        private readonly ILoggingAdapter logger = Context.GetLogger();       
        private readonly GraphEngine graphEngine;
 
        public GraphEventActor(GraphEngine _graphEngine)
        {
            logger.Info($"Create GraphEventActor:{Context.Self.Path.Name}");
            graphEngine = _graphEngine;
 
            ReceiveAsync<GraphEvent>(async graphEvent =>
            {
                var cypher = await _graphEngine.GetCypher();
 
                switch (graphEvent.Action)
                {
                    case "Reset":
                        {
                            await _graphEngine.RemoveAll();
                        }
                        break;
                    case "Create":
                        {                           
                            await cypher.Write
                                .Create($"(alice:{graphEvent.Alice} {{name:'{graphEvent.Name}'}})")
                                .ExecuteWithoutResultsAsync();
                        }
                        break;
                    case "Relation":
                        {
                            await cypher.Write
                                .Match($"(a:{graphEvent.From.Alice}),(b:{graphEvent.To.Alice})")
                                .Where($"a.name = '{graphEvent.From.Name}' AND b.name = '{graphEvent.To.Name}'")
                                .Create($"(a)-[r:{graphEvent.Name}]->(b)").ExecuteWithoutResultsAsync();
                        }
                        break;
                }
            });
        }
    }
}
