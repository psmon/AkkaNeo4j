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
                            await cypher
                                .Create($"(element:GraphElementIdenty {{ UId: '{graphEvent.Uid}', Name:'{graphEvent.Name}'}})")
                                .ExecuteWithoutResultsAsync();

                            logger.Info($"Create : {graphEvent.Uid}");
                        }
                        break;
                    case "Delete":
                        {
                            //Delete a element and all inbound relationships
                            await cypher
                                .OptionalMatch("()<-[r]-(element:GraphElementIdenty)")
                                .Where((GraphElementIdenty element) => element.UId == graphEvent.Uid)
                                .Delete("r, element")                                
                                .ExecuteWithoutResultsAsync();

                            logger.Info($"Delete : {graphEvent.Uid}");
                        }
                        break;
                    case "Relation":
                        {
                            await cypher
                                .Match($"(a:GraphElementIdenty),(b:GraphElementIdenty)")
                                .Where($"a.UId = '{graphEvent.From.UId}' AND b.UId = '{graphEvent.To.UId}'")
                                .Create($"(a)-[r:{graphEvent.Name}]->(b)")
                                .ExecuteWithoutResultsAsync();

                            logger.Info($"Relation : {graphEvent.Name} {graphEvent.From.UId} -> {graphEvent.To.UId}");
                        }
                        break;
                }
            });
        }
    }
}
