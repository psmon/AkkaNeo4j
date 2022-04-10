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
                Uid = _actorName,
                Name = _actorName
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
                        UId = ActorName,
                        Name = ActorName
                    },
                    To = new GraphElementIdenty()
                    {
                        UId = ActorNameRules.RootName,
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
                        UId = _actorName,
                        Name = _actorName
                    },
                    To = new GraphElementIdenty()
                    {
                        UId = ParentName,
                        Name = ParentName
                    }
                });
            }

            ReceiveAsync<string>(async text =>
            {
                Console.WriteLine($"ReceiveAsync : {0}",text);
                if(text == "stop")
                {
                    Context.Stop(Self);
                }
            });

            Receive<CreateChild>(async child =>
            {
                Context.ActorOf(Props.Create<SimpleActor>(child.ActorName, _graphActorRef), child.ActorName);                
            });
        }

        protected override void PostStop()
        {
            _graphActorRef.Tell(new GraphEvent()
            {
                Action = "Delete",
                Uid = _actorName,
                Name = _actorName
            });
        }
    }
}
