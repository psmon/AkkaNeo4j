# AkkaNeo4j

Akka의 액터모델 계층구조를, Neo4j Graph DB를 활용하여 시각화하는 변종 프로젝트로~

Asp.net Core에서 DI를 활용하여 액터를 구성하는 방법을 살펴볼수 있습니다.

DockerCompose를 활용하여, Neo4j + AkkaNetApp 통합로컬 실행 가능합니다.



## 로컬실행

- Neo4j : http://localhost:7474/browser/


## 액터계층구조

![graph1](./Doc/graph1.png)


## Code Sample
                var depth1_1 = _actorSystem.ActorOf(Props.Create<SimpleActor>("a", _graphEventActorRef),"a");
                var depth1_2 = _actorSystem.ActorOf(Props.Create<SimpleActor>("b", _graphEventActorRef),"b");
                var depth1_3 = _actorSystem.ActorOf(Props.Create<SimpleActor>("c", _graphEventActorRef),"c");


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

