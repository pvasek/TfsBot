using System.Collections.Generic;
using System.Threading.Tasks;
using TfsBot.Common.Entities;

namespace TfsBot.Common.Db
{
    public interface IRepository
    {
        Task SaveServiceClient(ServerClient serverClient);
        List<ServerClient> GetServerClients(string serverId);
        Task SaveClient(Client client);
        Task<Client> GetClientAsync(string userId, string userName);
        //Task RemoveServerClientAsync(ServerClient client);
    }
}