using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Akka.Actor;

using AkkaNeo4j.Models;

namespace AkkaNeo4j.Actors
{
    public static class ActorNameRules
    {
        static public string RootName = "user";
    }

    public class CreateChild
    {
        public string ActorName { get;set; }
    }

    public class SimpleActor : ReceiveActor
    {
        readonly private IActorRef _graphActorRef;
        readonly private string _actorName;

        public SimpleActor(string ActorName, IActorRef graphActorRef)
        {
            _graphActorRef = graphActorRef;
            _actorName = ActorName;

            _graphActorRef.Tell(new GraphEvent()
            {
                Action = "Create",
                Alice = ActorName,
                Name = ActorName
            });
            
            var ParentName= Context.Parent.Path.Name;

            bool isFirstDepth = ParentName == "user";

            if(isFirstDepth)
            {
                _graphActorRef.Tell(new GraphEvent()
                {
                    Action = "Relation",                           
                    Name = "Parent",
                    From = new GraphElementIdenty()
                    {
                        Alice = ActorName,
                        Name = ActorName
                    },
                    To = new GraphElementIdenty()
                    {
                        Alice = ActorNameRules.RootName,
                        Name = ActorNameRules.RootName
                    }
                });
            }
            else
            {
                _graphActorRef.Tell(new GraphEvent()
                {
                    Action = "Relation",                           
                    Name = "Parent",
                    From = new GraphElementIdenty()
                    {
                        Alice = _actorName,
                        Name = _actorName
                    },
                    To = new GraphElementIdenty()
                    {
                        Alice = ParentName,
                        Name = ParentName
                    }
                });
            }

            ReceiveAsync<string>(async text =>
            {
                Console.WriteLine($"ReceiveAsync : {0}",text);
            });

            Receive<CreateChild>(async child =>
            {
                Context.ActorOf(Props.Create<SimpleActor>(child.ActorName, _graphActorRef), child.ActorName);
            });
        }
    }
}
