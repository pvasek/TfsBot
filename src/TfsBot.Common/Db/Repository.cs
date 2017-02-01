using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using TfsBot.Common.Entities;

namespace TfsBot.Common.Db
{
    public class Repository : IRepository
    {
        private readonly CloudTable _serviceClientsTable;
        private readonly CloudTable _clientsTable;

        public Repository(string storageConnectionString)
        {
            var storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            var tableClient = storageAccount.CreateCloudTableClient();
            _serviceClientsTable = tableClient.GetTableReference("serviceclients");
            _serviceClientsTable.CreateIfNotExists();
            _clientsTable = tableClient.GetTableReference("clients");
            _clientsTable.CreateIfNotExists();
        }

        public async Task SaveServiceClient(ServerClient serverClient)
        {
            await _serviceClientsTable.ExecuteAsync(TableOperation.InsertOrReplace(serverClient));
        }

        public List<ServerClient> GetServerClients(string serverId)
        {
            var query = new TableQuery<ServerClient>()
                .Where(TableQuery.GenerateFilterCondition(nameof(ServerClient.PartitionKey), QueryComparisons.Equal, serverId));

            return _serviceClientsTable
                .ExecuteQuery(query)
                .ToList();
        }

        public async Task SaveClient(Client client)
        {
            await _clientsTable.ExecuteAsync(TableOperation.InsertOrReplace(client));
        }

        public async Task<Client> GetClientAsync(string userId, string userName)
        {
            var partitionKey = Client.GetPartitionKey(userId);
            var rowKey = Client.GetRowKey(userId, userName);

            var retrieveOperation = TableOperation.Retrieve<Client>(partitionKey, rowKey);
            var retrievedResult = await _clientsTable.ExecuteAsync(retrieveOperation);
            return (Client)retrievedResult.Result;
        }

        public async Task RemoveServerClientAsync(ServerClient client)
        {
            var deleteOperation = TableOperation.Delete(client);
            await _serviceClientsTable.ExecuteAsync(deleteOperation);
        }
    }
}