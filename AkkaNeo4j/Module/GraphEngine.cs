using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Neo4jClient;
using Neo4jClient.Cypher;

namespace AkkaNeo4j.Module
{
    public class GraphEngine
    {         
        private readonly GraphClient neo4jClient;
 
        public GraphEngine()
        {                        
            neo4jClient = new GraphClient(new Uri("http://localhost:7474"), 
                "neo4j", "bitnami");           
        }
 
        public async Task<ICypherFluentQuery> GetCypher()
        {
            if (!neo4jClient.IsConnected)
            {
                await neo4jClient.ConnectAsync();
            }
            return neo4jClient.Cypher;
        }
 
        public async Task<GraphClient> GetClient()
        {
            if (!neo4jClient.IsConnected)
            {
                await neo4jClient.ConnectAsync();
            }
            return neo4jClient;
        }
 
        public async Task RemoveAll()
        {
            var cyper = await GetCypher();
 
            await cyper
                .Match("(n)")
                .DetachDelete("n")
                .ExecuteWithoutResultsAsync();
 
        } 
    }
}
