# AkkaNeo4j

Akka�� ���͸� ����������, Neo4j Graph DB�� Ȱ���Ͽ� �ð�ȭ�ϴ� ���� ������Ʈ��~

Asp.net Core���� DI�� Ȱ���Ͽ� ���͸� �����ϴ� ����� ���캼�� �ֽ��ϴ�.

DockerCompose�� Ȱ���Ͽ�, Neo4j + AkkaNetApp ���շ��� ���� �����մϴ�.



## ���ý���

- Neo4j : http://localhost:7474/browser/


## ���Ͱ�������

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
