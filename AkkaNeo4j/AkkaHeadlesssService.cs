using System;
using System.Threading;
using System.Threading.Tasks;

using Akka.Actor;
using Akka.DependencyInjection;

using AkkaNeo4j.Actors;
using AkkaNeo4j.Models;
using AkkaNeo4j.Module;

using Microsoft.Extensions.Hosting;

namespace AkkaNeo4j
{
    public sealed class AkkaService : IPublicHashingService, IHostedService
    {
        private IActorRef _graphEventActorRef;
        private ActorSystem _actorSystem;
        private readonly IServiceProvider _serviceProvider;
        private GraphEngine _graphEngine;

        private readonly IHostApplicationLifetime _applicationLifetime;

        public AkkaService(IServiceProvider sp, IHostApplicationLifetime applicationLifetime)
        {
            _serviceProvider = sp;
            _applicationLifetime = applicationLifetime;            
        }

        public void GraphDataInit()
        {
            _graphEngine.RemoveAll().Wait();

             _graphEventActorRef.Tell(new GraphEvent()
            {
                Action = "Create",
                Uid = "AkkaSystem",
                Name = "AkkaSystem"
            });

            _graphEventActorRef.Tell(new GraphEvent()
            {
                Action = "Create",
                Uid = ActorNameRules.RootName,
                Name = ActorNameRules.RootName
            });

            _graphEventActorRef.Tell(new GraphEvent()
            {
                Action = "Relation",                           
                Name = "Parent",
                From = new GraphElementIdenty()
                {
                    UId = ActorNameRules.RootName,
                    Name = ActorNameRules.RootName
                },
                To = new GraphElementIdenty()
                {
                    UId = "AkkaSystem",
                    Name = "AkkaSystem"
                }
            });

            _graphEventActorRef.Tell(new GraphEvent()
            {
                Action = "Create",
                Uid = "GraphEventActor",
                Name = "GraphEventActor"
            });

            _graphEventActorRef.Tell(new GraphEvent()
            {
                Action = "Relation",                           
                Name = "Parent",
                From = new GraphElementIdenty()
                {
                    UId = "GraphEventActor",
                    Name = "GraphEventActor"
                },
                To = new GraphElementIdenty()
                {
                    UId = ActorNameRules.RootName,
                    Name = ActorNameRules.RootName
                }
            });
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var bootstrap = BootstrapSetup.Create();

            // enable DI support inside this ActorSystem, if needed
            var diSetup = DependencyResolverSetup.Create(_serviceProvider);

            // merge this setup (and any others) together into ActorSystemSetup
            var actorSystemSetup = bootstrap.And(diSetup);

            // start ActorSystem
            _actorSystem = ActorSystem.Create("headless-service", actorSystemSetup);            
            _graphEngine = new GraphEngine();            
            _graphEventActorRef = _actorSystem.ActorOf(Props.Create<GraphEventActor>(_graphEngine),"GraphEventActor");

            GraphDataInit();

            // create sample actor
            var depth1_1 = _actorSystem.ActorOf(Props.Create<SimpleActor>("a", _graphEventActorRef),"a");
            var depth1_2 = _actorSystem.ActorOf(Props.Create<SimpleActor>("b", _graphEventActorRef),"b");
            var depth1_3 = _actorSystem.ActorOf(Props.Create<SimpleActor>("c", _graphEventActorRef),"c");

            depth1_1.Tell(new CreateChild(){ ActorName = "a1" } );
            depth1_1.Tell(new CreateChild(){ ActorName = "a2" } );
            depth1_2.Tell(new CreateChild(){ ActorName = "b1" } );
            depth1_2.Tell(new CreateChild(){ ActorName = "b2" } );
            depth1_3.Tell(new CreateChild(){ ActorName = "c1" } );
            depth1_3.Tell(new CreateChild(){ ActorName = "c2" } );
            depth1_3.Tell(new CreateChild(){ ActorName = "c3" } );
            
            Task.Delay(500).Wait(); //wait for actor create
            var depth2_1 =_actorSystem.ActorSelection("user/a/a1");
            var depth2_2 =_actorSystem.ActorSelection("user/b/b1");

            depth2_1.Tell(new CreateChild(){ ActorName = "a11" } );
            depth2_1.Tell(new CreateChild(){ ActorName = "a12" } );
            depth2_1.Tell(new CreateChild(){ ActorName = "a13" } );
            
            depth2_2.Tell(new CreateChild(){ ActorName = "b11" } );
            depth2_2.Tell(new CreateChild(){ ActorName = "b12" } );
            depth2_2.Tell(new CreateChild(){ ActorName = "b13" } );

            Task.Delay(500).Wait(); //0.5초뒤 액터 삭제
            var depth2_2_b13 =_actorSystem.ActorSelection("user/b/b1/b13");
            depth2_2_b13.Tell("stop");

            // add a continuation task that will guarantee shutdown of application if ActorSystem terminates
            _actorSystem.WhenTerminated.ContinueWith(tr => {
                _applicationLifetime.StopApplication();
            });
            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            // strictly speaking this may not be necessary - terminating the ActorSystem would also work
            // but this call guarantees that the shutdown of the cluster is graceful regardless
            await CoordinatedShutdown.Get(_actorSystem).Run(CoordinatedShutdown.ClrExitReason.Instance);
        }

        public ActorSystem GetActorSystem()
        {
            return _actorSystem;
        }

        public GraphEngine GraphEngine()
        {
            return _graphEngine;
        }
    }

    public interface IPublicHashingService
    {
        ActorSystem GetActorSystem();

        GraphEngine GraphEngine();
    }
}
